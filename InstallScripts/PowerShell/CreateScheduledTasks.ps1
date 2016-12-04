Try
{
   
    Write-Host "Creating scheduled tasks..." -NoNewline

    $service = new-object -ComObject("Schedule.Service")
    $service.Connect()
    $rootFolder = $service.GetFolder("\")


    # automated event log export
    $TaskName = "R2G2 - Automated Event Log Export"
    $TaskDescr = "R2G2 - Automated event log export routine"
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


    # video capture file cleanup (delete all 0KB files)
    $TaskName = "R2G2 - Video Capture File Cleanup"
    $TaskDescr = "R2G2 - Video Capture file cleanup routine"
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

    Write-Host "done.`n"
}
Catch
{
    Write-Host -ForegroundColor red $_.Exception.Message
}