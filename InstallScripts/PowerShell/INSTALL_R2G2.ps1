Try
{
    Write-Host -ForegroundColor greeb "`nInstalling R2G2...`n"

    invoke-expression -Command C:\Deployments\Install\PowerShell\CreateEventLogSources.ps1
    invoke-expression -Command C:\Deployments\Install\PowerShell\CreateMessageQueues.ps1
    invoke-expression -Command C:\Deployments\Install\PowerShell\CreateScheduledTasks.ps1
    invoke-expression -Command C:\Deployments\Install\PowerShell\CreateLocalWebuser.ps1
    invoke-expression -Command C:\Deployments\Install\PowerShell\CreateWebApplications.ps1
    invoke-expression -Command C:\Deployments\Install\Database\PowerShell\CreateDatabase.ps1
    invoke-expression -Command C:\Deployments\Install\Database\PowerShell\DropAndCreateTables.ps1
    invoke-expression -Command C:\Deployments\Install\Database\PowerShell\SeedData.ps1
    invoke-expression -Command C:\Deployments\Install\PowerShell\InstallServices.ps1

    Write-Host -ForegroundColor green "`nR2G2 successfully installed.`n"
}
Catch
{
    Write-Host -ForegroundColor red $_.Exception.Message
}