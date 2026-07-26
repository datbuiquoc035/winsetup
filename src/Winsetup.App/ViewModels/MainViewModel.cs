using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Winsetup.App.Models;
using Winsetup.App.Services;

namespace Winsetup.App.ViewModels;

public class MainViewModel : INotifyPropertyChanged
{
    private readonly IpcClient _client;
    public ObservableCollection<CatalogItem> Items { get; } = [];

    private bool _isInstalling;
    public bool IsInstalling
    {
        get => _isInstalling;
        set { _isInstalling = value; OnPropertyChanged(); }
    }

    public MainViewModel()
    {
        var backendPath = Path.Combine(
            AppContext.BaseDirectory, "winsetup-core.exe");
        _client = new IpcClient(backendPath);
        _ = LoadCatalogAsync();
    }

    private async Task LoadCatalogAsync()
    {
        var items = await _client.GetCatalogAsync();
        foreach (var item in items)
        {
            Items.Add(new CatalogItem
            {
                Id = item.Id,
                Name = item.Name,
                Category = item.Category,
                Description = item.Description,
                WingetId = item.WingetId,
                DownloadUrl = item.DownloadUrl,
                SilentArgs = item.SilentArgs,
            });
        }
    }

    public async Task InstallSelectedAsync()
    {
        IsInstalling = true;
        var selected = Items.Where(i => i.IsSelected).Select(i => i.Id).ToList();

        await _client.InstallAsync(selected, (id, status, message) =>
        {
            var item = Items.FirstOrDefault(i => i.Id == id);
            if (item != null)
            {
                item.Status = status switch
                {
                    "Queued" => "⏳ Queued",
                    "Downloading" => "⬇ Downloading",
                    "Installing" => "⚙ Installing",
                    "Done" => "✅ Done",
                    "Failed" => "❌ Failed",
                    _ => status,
                };
            }
        });

        IsInstalling = false;
    }

    public void Filter(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            foreach (var item in Items)
                item.IsSelected = item.IsSelected;
            return;
        }

        foreach (var item in Items)
        {
            var visible = item.Name.Contains(query, StringComparison.OrdinalIgnoreCase)
                || item.Description.Contains(query, StringComparison.OrdinalIgnoreCase);
            // Visibility filtering would need a different approach in WinUI 3
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string? name = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
