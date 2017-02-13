# setup
$destPath = "C:\Deployments\"
$versionPath = "1.0.0.0\"

# service paths
$stateMachinePath = "Services\StateMachineService\"
$bluetoothBeaconWatcherPath = "Services\BluetoothBeaconWatcherService\"
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
$videoCaptureCleanupPath = "VideoCaptureFileCleanup\"
$backupPath = "Backup\"

# install
$installRoot = "Install\"
$installDatabasePath = "Install\Database\"
$installPowerShellPath = "Install\PowerShell\"
$installUtilityPath = "Install\Utility\"

# media
$sharedLibraryPath = "Media\SharedLibrary\"
$profilesPublicPath = "Media\Profiles\0\"
$exportsPath = "Media\Exports\EventLog\"
$publicProfileSource = "\\$env:COMPUTERNAME\SQLEXPRESS\KeebeeAATFilestream\Media\Profiles\0\*"
$sharedLibrarySource = "\\$env:COMPUTERNAME\SQLEXPRESS\KeebeeAATFilestream\Media\SharedLibrary\*"

# documentation paths
$documentationPath = "Install\Documentation\"

# source code
$sourceCode = "C:\Users\$env:USERNAME\Source\Repos\R2G2V1\"
$solutionFile = "Keebee.AAT.sln"

# services
$servicesRoot = "C:\Deployments\Services"

Try
{
    Write-Host -foregroundcolor green "`nDeploying R2G2...`n”

    # stop all services
    Write-Host -ForegroundColor yellow "--- Uninstall Services ---`n”

    Write-Host "Uninstalling Phidget Service..." -NoNewline
    Invoke-Command -ScriptBlock { C:\WINDOWS\Microsoft.NET\Framework\v4.0.30319\InstallUtil.exe /uninstall "$servicesRoot\PhidgetService\1.0.0.0\Keebee.AAT.PhidgetService.exe"} | Out-Null
    Write-Host "done."

    Write-Host "Uninstalling Video Capture Service..." -NoNewline
    Invoke-Command -ScriptBlock { C:\WINDOWS\Microsoft.NET\Framework\v4.0.30319\InstallUtil.exe /uninstall "$servicesRoot\VideoCaptureService\1.0.0.0\Keebee.AAT.VideoCaptureService.exe"} | Out-Null
    Write-Host "done."

    Write-Host "Uninstalling Bluetooth Beacon Watcher Service..." -NoNewline
    Invoke-Command -ScriptBlock { C:\WINDOWS\Microsoft.NET\Framework\v4.0.30319\InstallUtil.exe /uninstall "$servicesRoot\BluetoothBeaconWatcherService\1.0.0.0\Keebee.AAT.BluetoothBeaconWatcherService.exe"} | Out-Null
    Write-Host "done."

    Write-Host "Uninstalling State Machine Service..." -NoNewline
    Invoke-Command -ScriptBlock { C:\WINDOWS\Microsoft.NET\Framework\v4.0.30319\InstallUtil.exe /uninstall "$servicesRoot\StateMachineService\1.0.0.0\Keebee.AAT.StateMachineService.exe"} | Out-Null
    Write-Host "done."

    Write-Host "Uninstalling Keep IIS Alive Service..." -NoNewline
    Invoke-Command -ScriptBlock { C:\WINDOWS\Microsoft.NET\Framework\v4.0.30319\InstallUtil.exe /uninstall "$servicesRoot\KeepIISAliveService\1.0.0.0\Keebee.AAT.KeepIISAliveService.exe"} | Out-Null
    Write-Host "done."


    # build the solution
    Write-Host -ForegroundColor yellow "`n--- Build Solution ---`n”

    # Write-Host "--- Functionality temporarily removed ---`n” -NoNewline

    # register Build-VisualStudioSolution powershell module
    $path = "C:\Users\$env:USERNAME\Documents\WindowsPowerShell\Modules\Build-VisualStudioSolution\"
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
    Write-Host -ForegroundColor yellow "`n--- Deploy Components ---`n”
    # -------------------- ROOT --------------------

    # create the root directory
    If(!(test-path $destPath))
    {
        New-Item -ItemType Directory -Force -Path $destPath | Out-Null
    }

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
    Write-Host "Deploying Export folders...” -NoNewline
    $path = $destPath + $exportsPath
    If(test-path $path)
    {
        Remove-Item $path -recurse -Force
    }
    New-Item -ItemType Directory -Force -Path $path | Out-Null
    Write-Host "done.”

    # shared library
    Write-Host "Deploying Shared Library...” -NoNewline
    $path = $destPath + $sharedLibraryPath
    If(test-path $path)
    {
        Remove-Item $path -recurse -Force
    } 
    New-Item -ItemType Directory -Force -Path $path | Out-Null
    Copy-Item $sharedLibrarySource $path -recurse -Force
    Write-Host "done.”

    # public profile
    Write-Host "Deploying Public Profile...” -NoNewline
    $path = $destPath + $profilesPublicPath
    If(test-path $path)
    {
        Remove-Item $path -recurse -Force
    }
    $path = $destPath + $profilesPublicPath
    New-Item -ItemType Directory -Force -Path $path | Out-Null
    Copy-Item $publicProfileSource $path -recurse -Force

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

    # video capture file cleanup
    $path = $destPath + $scheduledTasksPath + $videoCaptureCleanupPath + $versionPath
    If(test-path $path)
    {
        Remove-Item $path -recurse -Force
    }
    New-Item -ItemType Directory -Force -Path $path | Out-Null
    Copy-Item C:\Users\$env:USERNAME\Source\Repos\R2G2V1\ScheduledTasks\Keebee.AAT.VideoCaptureFileCleanup\bin\Release\* $path -recurse -Force

    # backup
    $path = $destPath + $scheduledTasksPath + $backupPath + $versionPath
    If(test-path $path)
    {
        Remove-Item $path -recurse -Force
    }
    New-Item -ItemType Directory -Force -Path $path | Out-Null
    Copy-Item C:\Users\$env:USERNAME\Source\Repos\R2G2V1\ScheduledTasks\Keebee.AAT.Backup\bin\Release\* $path -recurse -Force

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

    # bluetooth beacon watcher service
    $path = $destPath + $bluetoothBeaconWatcherPath + $versionPath
    If(test-path $path)
    {
        Remove-Item $path -recurse -Force
    }
    New-Item -ItemType Directory -Force -Path $path | Out-Null
    Copy-Item C:\Users\$env:USERNAME\Source\Repos\R2G2V1\Service\Keebee.AAT.BluetoothBeaconWatcherService\bin\Release\* $path -recurse -Force

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
    Write-Host "Deploying Setup Documentation...” -NoNewline
    $path = $destPath + $documentationPath
    If(test-path $path)
    {
        Remove-Item $path -recurse -Force
    }
    New-Item -ItemType Directory -Force -Path $path | Out-Null
    Copy-Item C:\Users\$env:USERNAME\Source\Repos\R2G2V1\Documentation\Setup\PostWindowsInstallationSetup.docx $path -recurse -Force

    Write-Host "done.”

    Write-Host -foregroundcolor green "`nDeployment complete.`n”
}
Catch
{
    Write-Host -ForegroundColor red $_.Exception.Message
}