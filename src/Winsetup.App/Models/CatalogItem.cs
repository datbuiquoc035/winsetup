using System.Text.Json.Serialization;

namespace Winsetup.App.Models;

public class CatalogItem
{
    [JsonPropertyName("id")] public string Id { get; set; } = "";
    [JsonPropertyName("name")] public string Name { get; set; } = "";
    [JsonPropertyName("category")] public string Category { get; set; } = "";
    [JsonPropertyName("description")] public string Description { get; set; } = "";
    [JsonPropertyName("winget_id")] public string? WingetId { get; set; }
    [JsonPropertyName("download_url")] public string? DownloadUrl { get; set; }
    [JsonPropertyName("silent_args")] public string? SilentArgs { get; set; }
    [JsonIgnore] public bool IsSelected { get; set; }
    [JsonIgnore] public string Status { get; set; } = "";
}
