namespace Winsetup.App.Models;

public class CatalogItem
{
    public string Id { get; set; } = "";
    public string Name { get; set; } = "";
    public string Category { get; set; } = "";
    public string Description { get; set; } = "";
    public string? WingetId { get; set; }
    public string? DownloadUrl { get; set; }
    public string? SilentArgs { get; set; }
    public bool IsSelected { get; set; }
    public string Status { get; set; } = "";
}
