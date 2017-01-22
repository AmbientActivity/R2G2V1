Try
{
    Write-Host -ForegroundColor green "`nUninstalling R2G2...`n"

    Write-Host "Uninstalling Phidget Service..." -NoNewline
    Invoke-Command -ScriptBlock { C:\WINDOWS\Microsoft.NET\Framework\v4.0.30319\InstallUtil.exe /uninstall "C:\Deployments\Services\PhidgetService\1.0.0.0\Keebee.AAT.PhidgetService.exe"} | Out-Null
    Write-Host "done."

    Write-Host "Uninstalling Video Capture Service..." -NoNewline
    Invoke-Command -ScriptBlock { C:\WINDOWS\Microsoft.NET\Framework\v4.0.30319\InstallUtil.exe /uninstall "C:\Deployments\Services\VideoCaptureService\1.0.0.0\Keebee.AAT.VideoCaptureService.exe"} | Out-Null
    Write-Host "done."

    Write-Host "Uninstalling Bluetooth Beacon Watcher Service..." -NoNewline
    Invoke-Command -ScriptBlock { C:\WINDOWS\Microsoft.NET\Framework\v4.0.30319\InstallUtil.exe /uninstall "C:\Deployments\Services\BluetoothBeaconWatcherService\1.0.0.0\Keebee.AAT.BluetoothBeaconWatcherService.exe"} | Out-Null
    Write-Host "done."

    Write-Host "Uninstalling State Machine Service..." -NoNewline
    Invoke-Command -ScriptBlock { C:\WINDOWS\Microsoft.NET\Framework\v4.0.30319\InstallUtil.exe /uninstall "C:\Deployments\Services\StateMachineService\1.0.0.0\Keebee.AAT.StateMachineService.exe"} | Out-Null
    Write-Host "done."

    Write-Host "Uninstalling Keep IIS Alive Service..." -NoNewline
    Invoke-Command -ScriptBlock { C:\WINDOWS\Microsoft.NET\Framework\v4.0.30319\InstallUtil.exe /uninstall "C:\Deployments\Services\KeepIISAliveService\1.0.0.0\Keebee.AAT.KeepIISAliveService.exe"} | Out-Null
    Write-Host "done."
}
Catch
{
    Write-Host -ForegroundColor red $_.Exception.Message
}