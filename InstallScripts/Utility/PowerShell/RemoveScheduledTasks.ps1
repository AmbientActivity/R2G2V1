Try
{
    Write-Host -ForegroundColor yellow "`n--- Scheduled Tasks ---`n"

    function RemoveScheduledTask
    {
        $task_name = $args[0]
        $exists = Get-ScheduledTask | Where-Object {$_.TaskName -like "$task_name" }
        if ($exists) {
            Unregister-ScheduledTask -TaskName $task_name -Confirm:$false
        }
    }

    # event log export
    Write-Host "Removing Event Log Export task..." -NoNewline
    RemoveScheduledTask "R2G2 - Event Log Export"
    Write-Host "done."

    # backup
    Write-Host "Removing Backup task..." -NoNewline
     RemoveScheduledTask "R2G2 - Backup"
    Write-Host "done."

    # video capture cleanup
    Write-Host "Removing File Cleanup task..." -NoNewline
    RemoveScheduledTask "R2G2 - File Cleanup"
    Write-Host "done."

    # display launch
    Write-Host "Removing Display Launch task..." -NoNewline
    RemoveScheduledTask "R2G2 - Display Launch"
    Write-Host "done."
}
Catch
{
    throw $_.Exception.Message
}