Try
{
    # setup
    $rootDrive = "C:"
    $pathDeployments = "$rootDrive\Deployments\"
    $pathVersion = "1.0.0.0\"

    # service paths
    $pathPhidgetServiceExe = $pathDeployments + "Services\PhidgetService\" + $pathVersion + "Keebee.AAT.PhidgetService.exe"
    $pathVideoCaptureServiceExe = $pathDeployments + "Services\VideoCaptureService\" + $pathVersion + "Keebee.AAT.VideoCaptureService.exe"
    $pathBluetoothBeaconServiceExe = $pathDeployments + "Services\BluetoothBeaconWatcherService\" + $pathVersion + "Keebee.AAT.BluetoothBeaconWatcherService.exe"
    $pathStateMachineServiceExe = $pathDeployments + "Services\StateMachineService\" + $pathVersion + "Keebee.AAT.StateMachineService.exe"
    $pathKeepIISAliveServiceExe = $pathDeployments + "Services\KeepIISAliveService\" + $pathVersion + "Keebee.AAT.KeepIISAliveService.exe"

    Write-Host -ForegroundColor yellow "`n--- Services ---`n"

    Write-Host "Uninstalling Phidget Service..." -NoNewline
    Invoke-Command -ScriptBlock { C:\WINDOWS\Microsoft.NET\Framework\v4.0.30319\InstallUtil.exe /uninstall $pathPhidgetServiceExe } | Out-Null
    Stop-Process -ProcessName Keebee.AAT.PhidgetService* -Force
    Write-Host "done."

    Write-Host "Uninstalling Video Capture Service..." -NoNewline
    Invoke-Command -ScriptBlock { C:\WINDOWS\Microsoft.NET\Framework\v4.0.30319\InstallUtil.exe /uninstall $pathVideoCaptureServiceExe } | Out-Null
    Stop-Process -ProcessName Keebee.AAT.VideoCaptureService* -Force
    Write-Host "done."

    Write-Host "Uninstalling Bluetooth Beacon Watcher Service..." -NoNewline
    Invoke-Command -ScriptBlock { C:\WINDOWS\Microsoft.NET\Framework\v4.0.30319\InstallUtil.exe /uninstall $pathBluetoothBeaconServiceExe } | Out-Null
    Stop-Process -ProcessName Keebee.AAT.BluetoothBeaconWatcherService* -Force
    Write-Host "done."

    Write-Host "Uninstalling State Machine Service..." -NoNewline
    Invoke-Command -ScriptBlock { C:\WINDOWS\Microsoft.NET\Framework\v4.0.30319\InstallUtil.exe /uninstall $pathStateMachineServiceExe } | Out-Null
    Stop-Process -ProcessName Keebee.AAT.StateMachineService* -Force
    Write-Host "done."

    Write-Host "Uninstalling Keep IIS Alive Service..." -NoNewline
    Invoke-Command -ScriptBlock { C:\WINDOWS\Microsoft.NET\Framework\v4.0.30319\InstallUtil.exe /uninstall $pathKeepIISAliveServiceExe } | Out-Null
    Stop-Process -ProcessName Keebee.AAT.KeepIISAliveService* -Force
    Write-Host "done."
}
Catch
{
    throw $_.Exception.Message
}