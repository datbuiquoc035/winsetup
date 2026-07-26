using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Winsetup.App.ViewModels;

namespace Winsetup.App;

public partial class MainWindow : Window
{
    private readonly MainViewModel _vm;

    public MainWindow()
    {
        InitializeComponent();
        _vm = new MainViewModel();
        AppList.ItemsSource = _vm.Items;
        InstallButton.IsEnabled = false;

        _vm.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(MainViewModel.IsInstalling))
            {
                InstallButton.IsEnabled = !_vm.IsInstalling;
                InstallProgress.Visibility = _vm.IsInstalling
                    ? Visibility.Visible
                    : Visibility.Collapsed;
                InstallButton.Content = _vm.IsInstalling ? "Installing..." : "Install Selected";
            }
        };
    }

    private async void OnInstallClicked(object sender, RoutedEventArgs e)
    {
        InstallProgress.IsIndeterminate = true;
        await _vm.InstallSelectedAsync();
        InstallProgress.IsIndeterminate = false;
    }

    private void OnSearchSubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
    {
        _vm.Filter(args.QueryText);
    }

    private void OnSearchChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
    {
        if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
        {
            _vm.Filter(sender.Text);
        }
    }
}