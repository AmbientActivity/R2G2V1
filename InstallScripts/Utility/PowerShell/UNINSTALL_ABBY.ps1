Try
{
    Write-Host -ForegroundColor green "`nUninstalling ABBY..."

    $installPathUtility = "C:\Deployments\Install\Utility\PowerShell"
    $installPathData = "C:\Deployments\Install\Database\PowerShell"

    invoke-expression -Command $installPathUtility\UninstallServices.ps1
    invoke-expression -Command $installPathData\DropDatabase.ps1
    invoke-expression -Command $installPathUtility\RemoveWebApplications.ps1
    invoke-expression -Command $installPathUtility\RemoveLocalWebuser.ps1
    invoke-expression -Command $installPathUtility\RemoveMessageQueues.ps1
    invoke-expression -Command $installPathUtility\RemoveEventLogSources.ps1
    invoke-expression -Command $installPathUtility\RemoveScheduledTasks.ps1
    invoke-expression -Command $installPathUtility\RemoveVideoCaptures.ps1

    Write-Host -ForegroundColor green "`nUninstall complete.`n"
}
Catch
{
    Write-Host -ForegroundColor red $_.Exception.Message
    Write-Host -ForegroundColor yellow "`nUninstall aborted.`n"
}