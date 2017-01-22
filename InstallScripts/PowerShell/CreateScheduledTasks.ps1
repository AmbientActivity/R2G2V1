Try
{
    Write-Host -ForegroundColor yellow "`n--- Scheduled Tasks ---`n"

    Write-Host "Creating Event Log Export task..." -NoNewline
    $service = new-object -ComObject("Schedule.Service")
    $service.Connect()
    $rootFolder = $service.GetFolder("\")

    # event log export
    $TaskName = "R2G2 - Event Log Export"
    $TaskDescr = "Executes the event log export routine which creates an Excel file containing one day's worth of activities."
    $TaskCommand = "C:\Deployments\ScheduledTasks\EventLogExporter\1.0.0.0\Keebee.AAT.EventLogExporter.exe"
 
    $TaskDefinition = $service.NewTask(0) 
    $TaskDefinition.RegistrationInfo.Description = "$TaskDescr"
    $TaskDefinition.Settings.Enabled = $true
    $TaskDefinition.Settings.AllowDemandStart = $true
 
    $triggers = $TaskDefinition.Triggers
    $trigger = $triggers.Create(2) # Creates a "Daily" trigger
    $TaskStartTime = [datetime]::ParseExact("01:00","hh:mm", $null)
    $trigger.StartBoundary = $TaskStartTime.ToString("yyyy-MM-dd'T'HH:mm:ss")
    $trigger.Enabled = $true

    $Action = $TaskDefinition.Actions.Create(0)
    $action.Path = "$TaskCommand"
    $action.Arguments = "$TaskArg"

    $rootFolder.RegisterTaskDefinition("$TaskName", $TaskDefinition, 6, "System", $null, 5) | Out-Null
    Write-Host "done."

    # backup
    Write-Host "Creating Backup task..." -NoNewline
    $TaskName = "R2G2 - Backup"
    $TaskDescr = "Performs a full backup of the deployment folders and resident media " +
                 "and creates additional database scripts for restoring the data back to its original state." 
    $TaskCommand = "C:\Deployments\ScheduledTasks\Backup\1.0.0.0\Keebee.AAT.Backup.exe"
 
    $TaskDefinition = $service.NewTask(0) 
    $TaskDefinition.RegistrationInfo.Description = "$TaskDescr"
    $TaskDefinition.Settings.Enabled = $true
    $TaskDefinition.Settings.AllowDemandStart = $true
 
    $triggers = $TaskDefinition.Triggers
    $trigger = $triggers.Create(2) # Creates a "Daily" trigger
    $TaskStartTime = [datetime]::ParseExact("03:30","hh:mm", $null)
    $trigger.StartBoundary = $TaskStartTime.ToString("yyyy-MM-dd'T'HH:mm:ss")
    $trigger.Enabled = $true

    $Action = $TaskDefinition.Actions.Create(0)
    $action.Path = "$TaskCommand"
    $action.Arguments = "$TaskArg"

    $rootFolder.RegisterTaskDefinition("$TaskName", $TaskDefinition, 6, "System", $null, 5) | Out-Null
    Write-Host "done."

    # video capture file cleanup (delete all 0KB files)
    Write-Host "Creating Video Capture Cleanup task..." -NoNewline
    $TaskName = "R2G2 - Video Capture File Cleanup"
    $TaskDescr = "Finds and deletes all 0KB video capture files."
    $TaskCommand = "C:\Deployments\ScheduledTasks\VideoCaptureFileCleanup\1.0.0.0\Keebee.AAT.VideoCaptureFileCleanup.exe"
 
    $TaskDefinition = $service.NewTask(0) 
    $TaskDefinition.RegistrationInfo.Description = "$TaskDescr"
    $TaskDefinition.Settings.Enabled = $true
    $TaskDefinition.Settings.AllowDemandStart = $true
 
    $triggers = $TaskDefinition.Triggers
    $trigger = $triggers.Create(2) # Creates a "Daily" trigger
    $TaskStartTime = [datetime]::ParseExact("03:00","hh:mm", $null)
    $trigger.StartBoundary = $TaskStartTime.ToString("yyyy-MM-dd'T'HH:mm:ss")
    $trigger.Enabled = $true

    $Action = $TaskDefinition.Actions.Create(0)
    $action.Path = "$TaskCommand"
    $action.Arguments = "$TaskArg"

    $rootFolder.RegisterTaskDefinition("$TaskName", $TaskDefinition, 6, "System", $null, 5) | Out-Null
    Write-Host "done."
}
Catch
{
    Write-Host -ForegroundColor red $_.Exception.Message
}