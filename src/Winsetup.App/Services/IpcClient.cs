using System.Diagnostics;
using System.Text.Json;
using Winsetup.App.Models;

namespace Winsetup.App.Services;

public class IpcClient : IDisposable
{
    private readonly Process _process;
    private readonly StreamWriter _stdin;
    private readonly StreamReader _stdout;

    public IpcClient(string backendPath)
    {
        var psi = new ProcessStartInfo
        {
            FileName = backendPath,
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true,
        };

        _process = Process.Start(psi)
            ?? throw new InvalidOperationException("Failed to start backend");
        _stdin = _process.StandardInput;
        _stdout = _process.StandardOutput;
    }

    public async Task<List<CatalogItem>> GetCatalogAsync()
    {
        await SendAsync(new { type = "GetCatalog" });
        var response = await ReadResponseAsync();
        return response?.RootElement.GetProperty("items")
            .Deserialize<List<CatalogItem>>() ?? [];
    }

    public async Task InstallAsync(List<string> ids, Action<string, string, string> onProgress)
    {
        await SendAsync(new { type = "Install", ids });

        while (true)
        {
            var response = await ReadResponseAsync();
            if (response == null) break;

            var type = response.RootElement.GetProperty("type").GetString();
            if (type == "Summary") break;

            if (type == "Progress")
            {
                var id = response.RootElement.GetProperty("id").GetString() ?? "";
                var status = response.RootElement.GetProperty("status").GetString() ?? "";
                var message = response.RootElement.GetProperty("message").GetString() ?? "";
                onProgress(id, status, message);
            }
        }
    }

    private async Task SendAsync(object request)
    {
        var json = JsonSerializer.Serialize(request);
        await _stdin.WriteLineAsync(json);
        await _stdin.FlushAsync();
    }

    private async Task<JsonDocument?> ReadResponseAsync()
    {
        var line = await _stdout.ReadLineAsync();
        return line != null ? JsonDocument.Parse(line) : null;
    }

    public void Dispose()
    {
        _process.Kill(entireProcessTree: true);
        _process.Dispose();
        _stdin.Dispose();
        _stdout.Dispose();
    }
}
