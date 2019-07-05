 Try
{
    # setup
    $rootDrive = "C:"
    $pathDeployments = "$rootDrive\Deployments\"
    $pathVersion = "1.0.0.0\"

    # service paths
    $pathStateMachineServiceExe = $pathDeployments + "Services\StateMachineService\" + $pathVersion + "Keebee.AAT.StateMachineService.exe"
    $pathPhidgetServiceExe = $pathDeployments + "Services\PhidgetService\" + $pathVersion + "Keebee.AAT.PhidgetService.exe"
    $pathKeepIISAliveServiceExe = $pathDeployments + "Services\KeepIISAliveService\" + $pathVersion + "Keebee.AAT.KeepIISAliveService.exe"

    Write-Host -ForegroundColor yellow "`n--- Services ---`n"

    Write-Host "Installing State Machine Service..." -NoNewline
    Invoke-Command -ScriptBlock { C:\WINDOWS\Microsoft.NET\Framework\v4.0.30319\InstallUtil.exe /install $pathStateMachineServiceExe } | Out-Null
    Write-Host "done."

    Write-Host "Installing Phidget Service..." -NoNewline
    Invoke-Command -ScriptBlock { C:\WINDOWS\Microsoft.NET\Framework\v4.0.30319\InstallUtil.exe /install $pathPhidgetServiceExe } | Out-Null
    Write-Host "done."

    Write-Host "Installing Keep IIS Alive Service..." -NoNewline
    Invoke-Command -ScriptBlock { C:\WINDOWS\Microsoft.NET\Framework\v4.0.30319\InstallUtil.exe /install $pathKeepIISAliveServiceExe } | Out-Null
    Write-Host "done."
}
Catch
{
    throw $_.Exception.Message
}