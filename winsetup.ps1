$url = "https://github.com/datbuiquoc035/winsetup/releases/latest/download/Winsetup.App.exe"
$tmp = "$env:TEMP\Winsetup.App.exe"

Write-Host "Downloading WinSetup..." -ForegroundColor Cyan
Invoke-WebRequest -Uri $url -OutFile $tmp

Write-Host "Launching WinSetup..." -ForegroundColor Cyan
Start-Process -FilePath $tmp -Wait -WindowStyle Normal

Remove-Item $tmp
