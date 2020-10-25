Try
{
    Write-Host -ForegroundColor yellow "`n--- Scheduled Tasks ---`n"

    $service = new-object -ComObject("Schedule.Service")
    $service.Connect()
    $rootFolder = $service.GetFolder("\")
    $scheduledTasksPath = "C:\Deployments\ScheduledTasks"
    $displayPath = "C:\Deployments\UI\Display\1.0.0.0\Release\Keebee.AAT.Display.exe"

    function CreateScheduledTask
    {
        $task_name = $args[0]
        $description = $args[1]
        $command = $args[2]
        $start_time = $args[3]
        $arguments = $args[4]

        $get_task = Get-ScheduledTask $task_name -ErrorAction SilentlyContinue

        if (!$get_task) {
            $TaskDefinition = $service.NewTask(0) 
            $TaskDefinition.RegistrationInfo.Description = "$description"
            $TaskDefinition.Settings.Enabled = $true
            $TaskDefinition.Settings.AllowDemandStart = $true
 
            $triggers = $TaskDefinition.Triggers
            $trigger = $triggers.Create(2) # Creates a "Daily" trigger
            $TaskStartTime = [datetime]::ParseExact("$start_time", "hh:mm", $null)
            $trigger.StartBoundary = $TaskStartTime.ToString("yyyy-MM-dd'T'HH:mm:ss")
            $trigger.Enabled = $true
             
            $Action = $TaskDefinition.Actions.Create(0)
            $action.Path = "$command"
            $action.Arguments = "$arguments"

            $rootFolder.RegisterTaskDefinition("$task_name", $TaskDefinition, 6, "SYSTEM", $null, 5) | Out-Null
        }
    }

    # display (to launch upon user logon)
    Write-Host "Creating Display Launch task..." -NoNewline
    $task_name = "ABBY - Display Launch"
    $description = "Launches the main Display application on startup."
    $working_directory = "$displayPath"
    $execute = "$displayPath"
    $user = "$env:COMPUTERNAME\$env:USERNAME"

    $get_task = Get-ScheduledTask $task_name -ErrorAction SilentlyContinue
    if (!$get_task) {
        $action = New-ScheduledTaskAction -Execute $execute
        $trigger = New-ScheduledTaskTrigger -AtLogon -User $user
        Register-ScheduledTask -Action $action -Trigger $trigger -RunLevel Highest -TaskName $task_name -Description $description | Out-Null
    }
    Write-Host "done."

    # event log export
    Write-Host "Creating Event Log Export task..." -NoNewline
    $task_name = "ABBY - Event Log Export"
    $description = "Executes the event log export routine which creates an Excel file containing one day's worth of activities."
    $execute =  "$scheduledTasksPath\EventLogExporter\1.0.0.0\Keebee.AAT.EventLogExporter.exe"
    $user = "$env:COMPUTERNAME\$env:USERNAME"
    $start_time = "01:00"

    $get_task = Get-ScheduledTask $task_name -ErrorAction SilentlyContinue
    if (!$get_task) {
        $action = New-ScheduledTaskAction -Execute $execute
        $trigger = New-ScheduledTaskTrigger -Daily -AT "01:00"
        Register-ScheduledTask -Action $action -Trigger $trigger -RunLevel Highest -TaskName $task_name -Description $description | Out-Null
    }
    Write-Host "done."

    # video capture file cleanup
    Write-Host "Creating Video Capture File Cleanup task..." -NoNewline
    $task_name = "ABBY - Video Capture File Cleanup"
    $description = "Deletes all 0KB video capture files."
    $command = "$scheduledTasksPath\VideoCaptureFileCleanup\1.0.0.0\Keebee.AAT.VideoCaptureFileCleanup.exe"
    $start_time = "02:00"

    CreateScheduledTask $task_name $description $command $start_time
    Write-Host "done."

    # backup
    Write-Host "Creating Backup task..." -NoNewline
    $task_name = "ABBY - Backup"
    $description = "Performs a full backup of the deployment folders and resident media " +
                     "and creates additional database scripts for restoring the data back to its original state."
    $execute = "$scheduledTaskPath\Keebee.AAT.Backup.exe" 

    $get_task = Get-ScheduledTask $task_name -ErrorAction SilentlyContinue
    if (!$get_task) {
        $action = New-ScheduledTaskAction -Execute $execute
        $trigger = New-ScheduledTaskTrigger -Daily -AT "03:00"
        Register-ScheduledTask -Action $action -Trigger $trigger -RunLevel Highest -TaskName $task_name -Description $description | Out-Null
    }
    Write-Host "done."

    # system restart
    Write-Host "Creating System Restart task..." -NoNewline
    $task_name = "ABBY - System Restart"
    $description = "Performs a system restart."
    $command = "shutdown"
    $start_time = "04:00"
    $arguments = "/r"

    CreateScheduledTask $task_name $description $command $start_time $arguments

    Write-Host "done."
}
Catch
{
    throw $_.Exception.Message
}