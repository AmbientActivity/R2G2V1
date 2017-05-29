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

    # display launch
    Write-Host "Removing Display Launch task..." -NoNewline
    RemoveScheduledTask "R2G2 - Display Launch"
    Write-Host "done."

    # event log export
    Write-Host "Removing Event Log Export task..." -NoNewline
    RemoveScheduledTask "R2G2 - Event Log Export"
    Write-Host "done."

    # video capture file cleanup
    Write-Host "Removing Video Capture File Cleanup task..." -NoNewline
    RemoveScheduledTask "R2G2 - Video Capture File Cleanup"
    Write-Host "done."

    # backup
    Write-Host "Removing Backup task..." -NoNewline
     RemoveScheduledTask "R2G2 - Backup"
    Write-Host "done."

    # system restart
    Write-Host "Removing System Restart task..." -NoNewline
    RemoveScheduledTask "R2G2 - System Restart"
    Write-Host "done."
}
Catch
{
    throw $_.Exception.Message
}