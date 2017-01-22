Try
{
    Write-Host -ForegroundColor green "`nUninstalling R2G2...`n"

    invoke-expression -Command C:\Deployments\Install\Utility\PowerShell\UninstallServices.ps1
    invoke-expression -Command C:\Deployments\Install\Database\PowerShell\DropDatabase.ps1
    invoke-expression -Command C:\Deployments\Install\Utility\PowerShell\RemoveWebApplications.ps1
    invoke-expression -Command C:\Deployments\Install\Utility\PowerShell\RemoveLocalWebuser.ps1
    invoke-expression -Command C:\Deployments\Install\Utility\PowerShell\RemoveMessageQueues.ps1
    invoke-expression -Command C:\Deployments\Install\Utility\PowerShell\RemoveEventLogSources.ps1
    invoke-expression -Command C:\Deployments\Install\Utility\PowerShell\RemoveScheduledTasks.ps1

    Write-Host -ForegroundColor green "`nR2G2 successfully uninstalled.`n"
}
Catch
{
    Write-Host -ForegroundColor red $_.Exception.Message
}