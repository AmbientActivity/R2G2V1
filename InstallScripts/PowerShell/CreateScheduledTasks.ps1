Try
{
   
    Write-Host "Creating scheduled tasks..." -NoNewline

    $TaskName = "Automated R2G2 Event Log Export"
    $TaskDescr = "Automated R2G2 event log export routine"
    $TaskCommand = "C:\Deployments\ScheduledTasks\EventLogExporter\1.0.0.0\Keebee.AAT.EventLogExporter.exe"
 
    $service = new-object -ComObject("Schedule.Service")
    $service.Connect()
    $rootFolder = $service.GetFolder("\")
 
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

    Write-Host "done.`n"
}
Catch
{
    Write-Host -ForegroundColor red $_.Exception.Message
}