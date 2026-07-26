# WinSetup

Batch installer for essential Windows apps — powered by a .NET frontend and Rust backend.

## Quick Start

Run this in **PowerShell as Administrator**:

```powershell
$url = "https://github.com/datbuiquoc035/winsetup/releases/latest/download/WinSetup.exe"
$tmp = "$env:TEMP\WinSetup.exe"
Invoke-WebRequest -Uri $url -OutFile $tmp
Start-Process -FilePath $tmp -Wait
Remove-Item $tmp
```

## Features

- **App catalog** with 20+ essential apps across Browsers, Dev Tools, Utilities, Communication, and Media
- **Search/filter** to find apps quickly
- **Batch install** with real-time progress per app
- **Silent installs** via winget (no clicking through installers)
- **Rollback on failure** — one failure doesn't stop the rest

## Architecture

```
src/
├── Winsetup.App/          # .NET WinUI 3 frontend (MVVM)
│   ├── ViewModels/
│   ├── Services/
│   └── Models/
└── winsetup-core/         # Rust backend (tokio + serde)
    └── src/
        ├── main.rs         # IPC loop over stdin/stdout
        ├── ipc.rs          # JSON message protocol
        ├── manifest.rs     # Catalog parser
        ├── installer.rs    # Install orchestrator
        └── winget.rs       # winget wrapper
```

The frontend spawns the Rust binary as a child process and communicates over stdin/stdout with JSON messages.

## Development

Prerequisites: .NET 8 SDK, Rust toolchain with `x86_64-pc-windows-msvc` target.

```bash
# Build Rust backend
cd src/winsetup-core
cargo build --release

# Build .NET frontend
cd src/Winsetup.App
dotnet publish -c Release -o ../../publish

# Copy Rust binary alongside the .NET app
cp src/winsetup-core/target/release/winsetup-core.exe publish/
cp catalog.json publish/
```

## License

MIT
