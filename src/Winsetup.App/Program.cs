using Winsetup.App.Services;

var backendPath = Path.Combine(AppContext.BaseDirectory, "winsetup-core.exe");
using var client = new IpcClient(backendPath);

var items = await client.GetCatalogAsync();
Console.WriteLine($"Loaded {items.Count} apps\n");

while (true)
{
    Console.WriteLine("Available apps:");
    for (int i = 0; i < items.Count; i++)
        Console.WriteLine($"  [{i + 1}] {items[i].Name,-30} {items[i].Description}");

    Console.Write("\nEnter numbers to install (e.g. 1,3,5) or 'q' to quit: ");
    var input = Console.ReadLine();
    if (input?.ToLower() is "q" or "") break;

    var ids = input?.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
        .Select(s => int.TryParse(s, out var n) && n > 0 && n <= items.Count ? items[n - 1].Id : null)
        .Where(id => id != null)
        .Cast<string>()
        .ToList();

    if (ids == null || ids.Count == 0) { Console.WriteLine("No valid selections.\n"); continue; }

    Console.WriteLine("Installing...");
    await client.InstallAsync(ids, (id, status, message) =>
    {
        var item = items.FirstOrDefault(i => i.Id == id);
        var name = item?.Name ?? id;
        Console.WriteLine($"  [{status,-12}] {name,-30} {message}");
    });
    Console.WriteLine();
}
