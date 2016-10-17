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

# install
$installRoot = "Install\"
$installDatabasePath = "Install\Database\"
$installPowerShellPath = "Install\PowerShell\"
$installUtilityPath = "Install\Utility\"

# media
$profilesPath = "Media\Profiles\0\"
$exportsPath = "Media\Exports\EventLog\"
$publicLibrarySource = "\\" + $env:COMPUTERNAME + "\SQLEXPRESS\KeebeeAATFilestream\Media\Profiles\0\*"

# documentation paths
$documentationPath = "Install\Documentation\"

# source code
$sourceCode = "C:\Users\" + $env:USERNAME + "\Source\Repos\R2G2V1\"
$solutionFile = "Keebee.AAT.sln"

Try
{
    Write-Host -foregroundcolor yellow "`nDeploying R2G2...”

    # stop all services
    Write-Host "`n------------------”
    Write-Host "Uninstall Services”
    Write-Host "------------------`n”

    # register ServiceUtilities powershell module
    $path = "C:\Users\" + $env:USERNAME + "\Documents\WindowsPowerShell\Modules\ServiceUtilities\"
    If(!(test-path $path))
    {
        Write-Host "Registering Module ServiceUtilities...” -NoNewline
        New-Item -ItemType Directory -Force -Path $path | Out-Null
        Copy-Item C:\Users\$env:USERNAME\Source\Repos\R2G2V1\Deployment\Modules\ServiceUtilities\* $path -recurse -Force
        Write-Host "done.`n”
    }

    Get-Module SeriveUtilities | Out-Null

    Write-Host "Uninstalling Phidget Service...” -NoNewline
    $svcName = "PhidgetService"
    If (Get-Service $svcName -ErrorAction SilentlyContinue)
    {
    
        Uninstall-Service -Name $svcName
    }
    Write-Host "done.”

    Write-Host "Uninstalling Rfid Reader Service...” -NoNewline
    $svcName = "RfidReaderService"
    If (Get-Service $svcName -ErrorAction SilentlyContinue)
    {
        Uninstall-Service -Name $svcName
    }
    Write-Host "done.”

    Write-Host "Uninstalling Video Capture Service...” -NoNewline
    $svcName = "VideoCaptureService"
    If (Get-Service $svcName -ErrorAction SilentlyContinue)
    {
        Uninstall-Service -Name $svcName
    }
    Write-Host "done.”

    Write-Host "Uninstalling State Machine Service...” -NoNewline
    $svcName = "StateMachineService"
    If (Get-Service $svcName -ErrorAction SilentlyContinue)
    {
        Uninstall-Service -Name $svcName
    }
    Write-Host "done.”

    Write-Host "Uninstalling Keep IIS Alive Service...” -NoNewline
    $svcName = "KeepIISAliveService"
    If (Get-Service $svcName -ErrorAction SilentlyContinue)
    {
        Uninstall-Service -Name $svcName
    }
    Write-Host "done.”


    # build the solution
    Write-Host "`n`n--------------”
    Write-Host "Build Solution”
    Write-Host "--------------`n”

    # register Build-VisualStudioSolution powershell module
    $path = "C:\Users\" + $env:USERNAME + "\Documents\WindowsPowerShell\Modules\Build-VisualStudioSolution\"
    If(!(test-path $path))
    {
        Write-Host "Registering Module Build-VisualStudioSolution...” -NoNewline
        New-Item -ItemType Directory -Force -Path $path | Out-Null
        Copy-Item C:\Users\$env:USERNAME\Source\Repos\R2G2V1\Deployment\Modules\Build-VisualStudioSolution\* $path -recurse -Force
        Write-Host "done.`n”
    }

    Get-Module Build-VisualStudioSolution

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

    # -------------------- UI --------------------
    # display
    Write-Host "`nDeploying UI Components...” -NoNewline
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


    # -------------------- INSTALL SCRIPTS --------------------
    # install
    Write-Host "Deploying Install Scripts...” -NoNewline
    $path = $destPath + $installRoot
    If(test-path $path)
    {
        Remove-Item $path -recurse -Force
    }
    New-Item -ItemType Directory -Force -Path $path | Out-Null
    Copy-Item C:\Users\$env:USERNAME\Source\Repos\R2G2V1\InstallScripts\* $path -recurse -Force

    $path = $destPath + $installDatabasePath
    If(test-path $path)
    {
        Remove-Item $path -recurse -Force
    }
    New-Item -ItemType Directory -Force -Path $path | Out-Null
    Copy-Item C:\Users\$env:USERNAME\Source\Repos\R2G2V1\InstallScripts\Database\* $path -recurse -Force

    $path = $destPath + $installPowerShellPath
    If(!(test-path $path))
    {
        New-Item -ItemType Directory -Force -Path $path | Out-Null
    }
    Copy-Item C:\Users\$env:USERNAME\Source\Repos\R2G2V1\InstallScripts\PowerShell\* $path -recurse -Force

    $path = $destPath + $installUtilityPath
    If(test-path $path)
    {
        Remove-Item $path -recurse -Force
    }
    New-Item -ItemType Directory -Force -Path $path | Out-Null
    Copy-Item C:\Users\$env:USERNAME\Source\Repos\R2G2V1\InstallScripts\Utility\* $path -recurse -Force

    Write-Host "done.”


    # -------------------- SERVICES --------------------

    # wait until they are done unstalling
    while((Get-Service $svcName -ErrorAction SilentlyContinue)) {}
    while((Get-Service $svcName -ErrorAction SilentlyContinue)) {}
    while((Get-Service $svcName -ErrorAction SilentlyContinue)) {}
    while((Get-Service $svcName -ErrorAction SilentlyContinue)) {}
    while((Get-Service $svcName -ErrorAction SilentlyContinue)) {}

    # state machine service
    Write-Host "Deploying Services...” -NoNewline
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

    # documentation
    Write-Host "`nDeploying setup documentation...” -NoNewline
    $path = $destPath + $documentationPath
    If(test-path $path)
    {
        Remove-Item $path -recurse -Force
    }
    New-Item -ItemType Directory -Force -Path $path | Out-Null
    Copy-Item C:\Users\$env:USERNAME\Source\Repos\R2G2V1\Documentation\Setup\PostWindowsInstallationSetup.docx $path -recurse -Force
    Write-Host "done.”

    Write-Host -foregroundcolor green "`nR2G2 successfully deployed.`n”
}
Catch
{
    Write-Host -ForegroundColor red $_.Exception.Message
}