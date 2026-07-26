$url = "https://github.com/datbuiquoc035/winsetup/releases/latest/download/WinSetup.exe"
$tmp = "$env:TEMP\WinSetup.exe"

Write-Host "Downloading WinSetup..." -ForegroundColor Cyan
Invoke-WebRequest -Uri $url -OutFile $tmp

Write-Host "Launching WinSetup..." -ForegroundColor Cyan
Start-Process -FilePath $tmp -Wait

Remove-Item $tmp
