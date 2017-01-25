 Try
{
    Write-Host -ForegroundColor yellow "`n--- Services ---`n"

    Write-Host "Installing State Machine Service..." -NoNewline

    $svcPath = "C:\Deployments\Services"

    Invoke-Command -ScriptBlock { C:\WINDOWS\Microsoft.NET\Framework\v4.0.30319\InstallUtil.exe /install "$svcPath\StateMachineService\1.0.0.0\Keebee.AAT.StateMachineService.exe"} | Out-Null
    Write-Host "done."

    Write-Host "Installing Phidget Service..." -NoNewline
    Invoke-Command -ScriptBlock { C:\WINDOWS\Microsoft.NET\Framework\v4.0.30319\InstallUtil.exe /install "$svcPath\PhidgetService\1.0.0.0\Keebee.AAT.PhidgetService.exe"} | Out-Null
    Write-Host "done."

    Write-Host "Installing Video Capture Service..." -NoNewline
    Invoke-Command -ScriptBlock { C:\WINDOWS\Microsoft.NET\Framework\v4.0.30319\InstallUtil.exe /install "$svcPath\VideoCaptureService\1.0.0.0\Keebee.AAT.VideoCaptureService.exe"} | Out-Null
    Write-Host "done."

    Write-Host "Installing Bluetooth Beacon Watcher Service..." -NoNewline
    Invoke-Command -ScriptBlock { C:\WINDOWS\Microsoft.NET\Framework\v4.0.30319\InstallUtil.exe /install "$svcPath\BluetoothBeaconWatcherService\1.0.0.0\Keebee.AAT.BluetoothBeaconWatcherService.exe"} | Out-Null
    Write-Host "done."

    Write-Host "Installing Keep IIS Alive Service..." -NoNewline
    Invoke-Command -ScriptBlock { C:\WINDOWS\Microsoft.NET\Framework\v4.0.30319\InstallUtil.exe /install "$svcPath\KeepIISAliveService\1.0.0.0\Keebee.AAT.KeepIISAliveService.exe"} | Out-Null
    Write-Host "done."
}
Catch
{
    throw $_.Exception.Message
}