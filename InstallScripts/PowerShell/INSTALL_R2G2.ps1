Try
{
    Write-Host -ForegroundColor green "`nInstalling R2G2...`n"

    $installPath = "C:\Deployments\Install\PowerShell"
    $installPathData = "C:\Deployments\Install\Database\PowerShell"

    invoke-expression -Command $installPath\CreateEventLogSources.ps1
    invoke-expression -Command $installPath\CreateMessageQueues.ps1
    invoke-expression -Command $installPath\CreateScheduledTasks.ps1
    invoke-expression -Command $installPath\CreateLocalWebuser.ps1
    invoke-expression -Command $installPath\CreateWebApplications.ps1
    invoke-expression -Command $installPathData\CreateDatabase.ps1
    invoke-expression -Command $installPathData\DropAndCreateTables.ps1
    invoke-expression -Command $installPathData\SeedData.ps1
    invoke-expression -Command $installPath\InstallServices.ps1

    Write-Host -ForegroundColor green "`nInstallation complete.`n"
    Write-Host -ForegroundColor DarkYellow  "The application will launch after the system is rebooted.`n"

    $confirmation = Read-Host "Reboot now (Y/N)?"
    if ($confirmation -eq 'Y')
    {
        Restart-Computer
    }
}
Catch
{
    Write-Host -ForegroundColor red $_.Exception.Message
    Write-Host -ForegroundColor yellow "`nInstallation aborted.`n"
}