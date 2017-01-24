Try
{
    Write-Host -ForegroundColor yellow "`n--- Scheduled Tasks ---`n"

    # event log export
    Write-Host "Removing Event Log Export task..." -NoNewline
    $taskName = "R2G2 - Event Log Export"
    $taskExists = Get-ScheduledTask | Where-Object {$_.TaskName -like $taskName }
    if ($taskExists) {
        Unregister-ScheduledTask -TaskName $taskName -Confirm:$false
    }
    Write-Host "done."

    # backup
    Write-Host "Removing Backup task..." -NoNewline
    $taskName = "R2G2 - Backup"
    $taskExists = Get-ScheduledTask | Where-Object {$_.TaskName -like $taskName }
    if ($taskExists) {
        Unregister-ScheduledTask -TaskName $taskName -Confirm:$false
    }
    Write-Host "done."

    # video capture cleanup
    Write-Host "Removing Video Capture Cleanup task..." -NoNewline
    $taskName = "R2G2 - Video Capture File Cleanup"
    $taskExists = Get-ScheduledTask | Where-Object {$_.TaskName -like $taskName }
    if ($taskExists) {
        Unregister-ScheduledTask -TaskName $taskName -Confirm:$false
    }
    Write-Host "done."
}
Catch
{
    throw $_.Exception.Message
}