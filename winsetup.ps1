$repo = "https://github.com/datbuiquoc035/winsetup/releases/latest/download"
$dir = "$env:TEMP\winsetup"
New-Item -ItemType Directory -Force -Path $dir | Out-Null

Write-Host "Downloading WinSetup..." -ForegroundColor Cyan
Invoke-WebRequest -Uri "$repo/Winsetup.App.exe" -OutFile "$dir\Winsetup.App.exe"
Invoke-WebRequest -Uri "$repo/winsetup-core.exe" -OutFile "$dir\winsetup-core.exe"

Push-Location $dir
Write-Host "Launching WinSetup..." -ForegroundColor Cyan
& ".\Winsetup.App.exe"
Pop-Location

Remove-Item -Recurse -Force $dir
