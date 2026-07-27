$url = "https://github.com/datbuiquoc035/winsetup/releases/latest/download/winsetup.zip"
$dir = "$env:TEMP\winsetup"
$zip = "$env:TEMP\winsetup.zip"

New-Item -ItemType Directory -Force -Path $dir | Out-Null

Write-Host "Downloading WinSetup..." -ForegroundColor Cyan
Invoke-WebRequest -Uri $url -OutFile $zip

Write-Host "Extracting..." -ForegroundColor Cyan
Expand-Archive -Path $zip -DestinationPath $dir -Force

Write-Host "Launching WinSetup..." -ForegroundColor Cyan
Push-Location $dir
& ".\Winsetup.App.exe"
Pop-Location

Remove-Item -Recurse -Force $dir, $zip
