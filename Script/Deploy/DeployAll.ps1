# setup
$destPath = "C:\Deployments\"
$versionPath = "1.0.0.0\"

# service paths
$stateMachinePath = "Services\StateMachineService\"
$rfidReaderPath = "Services\RfidReaderService\"
$phidgetPath = "Services\PhidgetService\"
$videoCapturePath = "Services\VideoCaptureService\"
$keepIISAlivePath = "Services\KeepIISAliveService\"

# web paths
$dataPath = "Web\Data\"
$apiPath = "Web\API\"
$administratorPath = "Web\Administrator\"

# ui paths
$displayReleasePath = "UI\Display\"
$displayDebugPath = "UI\Display\"
$simulatorPath = "UI\Simulator\"

# scheduled tasks
$scheduledTasksPath = "ScheduledTasks\"
$eventLogExportPath = "EventLogExporter\"

# scripts
$scriptDatabasePath = "Scripts\Database\"
$scriptEventLogPath = "Scripts\EventLogSource\"
$scriptMessageQueuePath = "Scripts\MessageQueue\"
$scriptServicePath = "Scripts\Service\"

# media
$profilesPath = "Media\Profiles\0\"
$exportsPath = "Media\Exports\EventLog\"
$publicLibrarySource = "\\" + $env:COMPUTERNAME + "\SQLEXPRESS\KeebeeAATFilestream\Media\Profiles\0\*"

# source code
$sourceCode = "C:\Users\" + $env:USERNAME + "\Source\Repos\R2G2V1\"
$solutionFile = "Keebee.AAT.sln"

Write-Host -foregroundcolor yellow "`nDeploying R2G2...”

# stop all services
Write-Host "`n------------------”
Write-Host "Uninstall Services”
Write-Host "------------------`n”

Get-Module SeriveUtilities | Out-Null

Write-Host "Uninstalling Phidget Service...” -NoNewline
$svcName = "PhidgetService"
If (Get-Service $svcName -ErrorAction SilentlyContinue)
{
    
    Uninstall-Service -Name $svcName
    while((Get-Service $svcName -ErrorAction SilentlyContinue)) {}
}
Write-Host "done.”

Write-Host "Uninstalling Rfid Reader Service...” -NoNewline
$svcName = "RfidReaderService"
If (Get-Service $svcName -ErrorAction SilentlyContinue)
{
    Uninstall-Service -Name $svcName
    while((Get-Service $svcName -ErrorAction SilentlyContinue)) {}
}
Write-Host "done.”

Write-Host "Uninstalling Video Capture Service...” -NoNewline
$svcName = "VideoCaptureService"
If (Get-Service $svcName -ErrorAction SilentlyContinue)
{
    Uninstall-Service -Name $svcName
    while((Get-Service $svcName -ErrorAction SilentlyContinue)) {}
}
Write-Host "done.”

Write-Host "Uninstalling State Machine Service...” -NoNewline
$svcName = "StateMachineService"
If (Get-Service $svcName -ErrorAction SilentlyContinue)
{
    Uninstall-Service -Name $svcName
    while((Get-Service $svcName -ErrorAction SilentlyContinue)) {}
}
Write-Host "done.”

Write-Host "Uninstalling Keep IIS Alive Service...” -NoNewline
$svcName = "KeepIISAliveService"
If (Get-Service $svcName -ErrorAction SilentlyContinue)
{
    Uninstall-Service -Name $svcName
    while((Get-Service $svcName -ErrorAction SilentlyContinue)) {}
}
Write-Host "done.”

# build the solution
Write-Host "`n`n--------------”
Write-Host "Build Solution”
Write-Host "--------------`n”

Get-Module R2G2Build | Out-Null
Get-Module R2G2Build | Out-Null

# build debug
$buildResult = Build-VisualStudioSolution -SourceCodePath $sourceCode -SolutionFile $solutionFile -BuildLogFile "R2G2BuildDebug.log" -Configuration "Debug" -CleanFirst;

If (!$buildResult)
{
    exit
}

# build release
$buildResult = Build-VisualStudioSolution -SourceCodePath $sourceCode -SolutionFile $solutionFile -BuildLogFile "R2G2BuildRelease.log" -Configuration "Release" -CleanFirst;

If (!$buildResult)
{
    exit
}

# restart IIS
Write-Host "`n`n-----------”
Write-Host "Restart IIS”
Write-Host "-----------”
invoke-command -scriptblock {iisreset}

# delpoy components
Write-Host "`n`n-----------------”
Write-Host "Deploy Components”
Write-Host "-----------------”
# -------------------- ROOT --------------------

# create the root directory
If(!(test-path $destPath))
{
    New-Item -ItemType Directory -Force -Path $destPath
}

# -------------------- SERVICES --------------------
# state machine service
Write-Host "`nDeploying Services...” -NoNewline
$path = $destPath + $stateMachinePath + $versionPath
If(test-path $path)
{
    Remove-Item $path -recurse -Force
}
New-Item -ItemType Directory -Force -Path $path | Out-Null
Copy-Item C:\Users\$env:USERNAME\Source\Repos\R2G2V1\Service\Keebee.AAT.StateMachineService\bin\Release\* $path -recurse -Force 

# phidget service
$path = $destPath + $phidgetPath + $versionPath
If(test-path $path)
{
    Remove-Item $path -recurse -Force
}
New-Item -ItemType Directory -Force -Path $path | Out-Null
Copy-Item C:\Users\$env:USERNAME\Source\Repos\R2G2V1\Service\Keebee.AAT.PhidgetService\bin\Release\* $path -recurse -Force

# rfid reader service
$path = $destPath + $rfidReaderPath + $versionPath
If(test-path $path)
{
    Remove-Item $path -recurse -Force
}
New-Item -ItemType Directory -Force -Path $path | Out-Null
Copy-Item C:\Users\$env:USERNAME\Source\Repos\R2G2V1\Service\Keebee.AAT.RfidReaderService\bin\Release\* $path -recurse -Force

# video capture service
$path = $destPath + $videoCapturePath + $versionPath
If(test-path $path)
{
    Remove-Item $path -recurse -Force
}
New-Item -ItemType Directory -Force -Path $path | Out-Null
Copy-Item C:\Users\$env:USERNAME\Source\Repos\R2G2V1\Service\Keebee.AAT.VideoCaptureService\bin\Release\* $path -recurse -Force

# keep iis alive service
$path = $destPath + $keepIISAlivePath + $versionPath
If(test-path $path)
{
    Remove-Item $path -recurse -Force
}
New-Item -ItemType Directory -Force -Path $path | Out-Null
Copy-Item C:\Users\$env:USERNAME\Source\Repos\R2G2V1\Service\Keebee.AAT.KeepIISAliveService\bin\Release\* $path -recurse -Force
Write-Host "done.”


# -------------------- UI --------------------
# display
Write-Host "Deploying UI Components...” -NoNewline
$path = $destPath + $displayReleasePath + $versionPath + "Release\"
If(test-path $path)
{
    Remove-Item $path -recurse -Force
}
New-Item -ItemType Directory -Force -Path $path | Out-Null
Copy-Item C:\Users\$env:USERNAME\Source\Repos\R2G2V1\UI\Keebee.AAT.Display\bin\Release\* $path -recurse -Force

$path = $destPath + $displayDebugPath + $versionPath + "Debug\"
If(test-path $path)
{
    Remove-Item $path -recurse -Force
}
New-Item -ItemType Directory -Force -Path $path | Out-Null
Copy-Item C:\Users\$env:USERNAME\Source\Repos\R2G2V1\UI\Keebee.AAT.Display\bin\Debug\* $path -recurse -Force

# simulator
$path = $destPath + $simulatorPath + $versionPath
If(test-path $path)
{
    Remove-Item $path -recurse -Force
}
New-Item -ItemType Directory -Force -Path $path | Out-Null
Copy-Item C:\Users\$env:USERNAME\Source\Repos\R2G2V1\UI\Keebee.AAT.Simulator\bin\Release\* $path -recurse -Force
Write-Host "done.”


# -------------------- SCHEDULED TASKS --------------------

# scheduled tasks root
$path = $destPath + $scheduledTasksPath
If(test-path $path)
{
    Remove-Item $path -recurse -Force
}
New-Item -ItemType Directory -Force -Path $path | Out-Null

# event log exporter
Write-Host "Deploying Scheduled Tasks...” -NoNewline
$path = $destPath + $scheduledTasksPath + $eventLogExportPath + $versionPath
If(test-path $path)
{
    Remove-Item $path -recurse -Force
}
New-Item -ItemType Directory -Force -Path $path | Out-Null
Copy-Item C:\Users\$env:USERNAME\Source\Repos\R2G2V1\ScheduledTasks\Keebee.AAT.EventLogExporter\bin\Release\* $path -recurse -Force
Write-Host "done.”


# -------------------- WEB --------------------
# data access
Write-Host "Deploying Web Components...” -NoNewline
$path = $destPath + $dataPath + $versionPath
If(test-path $path)
{
    Remove-Item $path -recurse -Force
}
New-Item -ItemType Directory -Force -Path $path | Out-Null
Copy-Item C:\Users\$env:USERNAME\Source\Repos\R2G2V1\Data\Keebee.AAT.DataAccess\* $path -recurse -Force

# api
$path = $destPath + $apiPath + $versionPath
If(test-path $path)
{
    Remove-Item $path -recurse -Force
}
New-Item -ItemType Directory -Force -Path $path | Out-Null
Copy-Item C:\Users\$env:USERNAME\Source\Repos\R2G2V1\API\Keebee.AAT.Operations\* $path -recurse -Force

# administrator
$path = $destPath + $administratorPath + $versionPath
If(test-path $path)
{
    Remove-Item $path -recurse -Force
}
New-Item -ItemType Directory -Force -Path $path | Out-Null
Copy-Item C:\Users\$env:USERNAME\Source\Repos\R2G2V1\WebUI\Keebee.AAT.Administrator\* $path -recurse -Force
Write-Host "done.”


# -------------------- SCRIPTS --------------------
# scripts
Write-Host "Deploying Startup Scripts...” -NoNewline
$path = $destPath + $scriptDatabasePath
If(test-path $path)
{
    Remove-Item $path -recurse -Force
}
New-Item -ItemType Directory -Force -Path $path | Out-Null
Copy-Item C:\Users\$env:USERNAME\Source\Repos\R2G2V1\Script\Database\* $path -recurse -Force

$path = $destPath + $scriptEventLogPath
If(!(test-path $path))
{
    New-Item -ItemType Directory -Force -Path $path | Out-Null
}
Copy-Item C:\Users\$env:USERNAME\Source\Repos\R2G2V1\Script\EventLogSource\* $path -recurse -Force

$path = $destPath + $scriptMessageQueuePath
If(test-path $path)
{
    Remove-Item $path -recurse -Force
}
New-Item -ItemType Directory -Force -Path $path | Out-Null
Copy-Item C:\Users\$env:USERNAME\Source\Repos\R2G2V1\Script\MessageQueue\CreateMessageQueues.ps1 $path -recurse -Force

$path = $destPath + $scriptServicePath
If(test-path $path)
{
    Remove-Item $path -recurse -Force
}
New-Item -ItemType Directory -Force -Path $path | Out-Null
Copy-Item C:\Users\$env:USERNAME\Source\Repos\R2G2V1\Script\Service\* $path -recurse -Force

Write-Host "done.”


# -------------------- MEDIA --------------------
# export folder
Write-Host "Deploying Media...” -NoNewline
$path = $destPath + $exportsPath
If(test-path $path)
{
    Remove-Item $path -recurse -Force
}
New-Item -ItemType Directory -Force -Path $path | Out-Null

# public library
$path = $destPath + $profilesPath
If(test-path $path)
{
    Remove-Item $path -recurse -Force
}
New-Item -ItemType Directory -Force -Path $path | Out-Null
Copy-Item $publicLibrarySource $path -recurse -Force
Write-Host "done.”

if ($false)
{
# start all services
Write-Host "`n`n-----------------”
Write-Host "Restart Services”
Write-Host "-----------------`n”

$svcName = "KeepIISAliveService"
if (Get-Service $svcName -ErrorAction SilentlyContinue)
{
    Write-Host "Restarting Keep IIS Alive Service...” -NoNewline
    $svc = Get-Service $svcName
    Start-Service $svc

    while($svc.status -ne 'Running')
    {
       Start-Sleep -Seconds 1
    }
    Write-Host "done.”
}

$svcName = "StateMachineService"
if (Get-Service $svcName -ErrorAction SilentlyContinue)
{
    Write-Host "Restarting State Machine Service...” -NoNewline
    $svc = Get-Service $svcName
    Start-Service $svc

    while($svc.status -ne 'Running')
    {
       Start-Sleep -Seconds 1
    }
    Write-Host "done.”
}

$svcName = "PhidgetService"
if (Get-Service $svcName -ErrorAction SilentlyContinue)
{
    Write-Host "Restarting Phidget Service...” -NoNewline
    $svc = Get-Service $svcName
    Start-Service $svc

    while($svc.status -ne 'Running')
    {
       Start-Sleep -Seconds 1
    }
    Write-Host "done.”
}

$svcName = "RfidReaderService"
if (Get-Service $svcName -ErrorAction SilentlyContinue)
{
    Write-Host "Restarting RFID Reader Service...” -NoNewline
    $svc = Get-Service $svcName
    Start-Service $svc

    while($svc.status -ne 'Running')
    {
       Start-Sleep -Seconds 1
    }
    Write-Host "done.”
}

$svcName = "VideoCaptureService"
if (Get-Service $svcName -ErrorAction SilentlyContinue)
{
    Write-Host "Restarting Video Capture Service...” -NoNewline
    $svc = Get-Service $svcName
    Start-Service $svc

    while($svc.status -ne 'Running')
    {
       Start-Sleep -Seconds 1
    }
    Write-Host "done.”
}
}

Write-Host -foregroundcolor green "`nR2G2 successfully deployed.`n”