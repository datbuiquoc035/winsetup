using Winsetup.App.Models;
using Winsetup.App.Services;

namespace Winsetup.App;

public partial class MainForm : Form
{
    private readonly IpcClient _client;
    private List<CatalogItem> _items = [];

    private readonly TextBox _searchBox;
    private readonly CheckedListBox _appList;
    private readonly Button _installBtn;
    private readonly ProgressBar _progress;
    private readonly Label _statusLabel;

    public MainForm()
    {
        Text = "WinSetup";
        Size = new Size(600, 500);
        StartPosition = FormStartPosition.CenterScreen;
        Font = new Font("Segoe UI", 10);

        _searchBox = new TextBox { PlaceholderText = "Search apps...", Dock = DockStyle.Top, Padding = new Padding(6) };

        _appList = new CheckedListBox { Dock = DockStyle.Fill, DisplayMember = "Name", CheckOnClick = true };

        _installBtn = new Button { Text = "Install Selected", Dock = DockStyle.Bottom, Height = 40, Enabled = false };
        _progress = new ProgressBar { Dock = DockStyle.Bottom, Style = ProgressBarStyle.Marquee, Visible = false };
        _statusLabel = new Label { Dock = DockStyle.Bottom, Height = 24, TextAlign = ContentAlignment.MiddleLeft };

        _installBtn.Click += OnInstallClicked;
        _searchBox.TextChanged += OnSearchChanged;
        _appList.ItemCheck += OnItemCheck;
        FormClosing += (_, _) => _client.Dispose();

        Controls.Add(_appList);
        Controls.Add(_searchBox);
        Controls.Add(_progress);
        Controls.Add(_statusLabel);
        Controls.Add(_installBtn);

        var backendPath = Path.Combine(AppContext.BaseDirectory, "winsetup-core.exe");
        _client = new IpcClient(backendPath);
        _ = LoadCatalogAsync();
    }

    private async Task LoadCatalogAsync()
    {
        _statusLabel.Text = "Loading catalog...";
        _items = await _client.GetCatalogAsync();
        _appList.Items.Clear();
        foreach (var item in _items)
            _appList.Items.Add(item, false);
        _statusLabel.Text = $"{_items.Count} apps loaded";
        _installBtn.Enabled = true;
    }

    private void OnSearchChanged(object? sender, EventArgs e)
    {
        var q = _searchBox.Text;
        _appList.Items.Clear();
        foreach (var item in _items)
        {
            if (string.IsNullOrEmpty(q) ||
                item.Name.Contains(q, StringComparison.OrdinalIgnoreCase) ||
                item.Description.Contains(q, StringComparison.OrdinalIgnoreCase))
            {
                _appList.Items.Add(item, item.IsSelected);
            }
        }
    }

    private void OnItemCheck(object? sender, ItemCheckEventArgs e)
    {
        if (e.Index >= 0 && e.Index < _appList.Items.Count)
            ((CatalogItem)_appList.Items[e.Index]).IsSelected = e.NewValue == CheckState.Checked;
    }

    private async void OnInstallClicked(object? sender, EventArgs e)
    {
        var selected = _items.Where(i => i.IsSelected).Select(i => i.Id).ToList();
        if (selected.Count == 0) return;

        _installBtn.Enabled = false;
        _progress.Visible = true;

        await _client.InstallAsync(selected, (id, status, message) =>
        {
            var item = _items.FirstOrDefault(i => i.Id == id);
            var name = item?.Name ?? id;
            BeginInvoke(() => _statusLabel.Text = $"[{status}] {name}");
        });

        _progress.Visible = false;
        _installBtn.Enabled = true;
        _statusLabel.Text = "Done!";
    }
}
