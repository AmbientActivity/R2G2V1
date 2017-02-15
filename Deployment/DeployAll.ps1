# setup
$pathDeployments = "C:\Deployments\"
$pathVersion = "1.0.0.0\"

# service paths
$pathServicesRoot = $pathDeployments + "Services"
$pathStateMachine = "Services\StateMachineService\"
$pathBluetoothBeacon = "Services\BluetoothBeaconWatcherService\"
$pathPhidget= "Services\PhidgetService\"
$pathVideoCapture = "Services\VideoCaptureService\"
$pathKeepIISAlive = "Services\KeepIISAliveService\"

# web paths
$pathDataAccess = "Web\Data\"
$pathAPI = "Web\API\"
$pathAdministator = "Web\Administrator\"

# ui paths
$pathDisplayRelease = "UI\Display\"
$pathDisplayDebug = "UI\Display\"
$pathSimulator = "UI\Simulator\"

# scheduled tasks
$pathScheduledTasks = "ScheduledTasks\"
$pathEventLogExporter = "EventLogExporter\"
$pathVideoCaptureCleanup = "VideoCaptureFileCleanup\"
$pathBackup = "Backup\"

# install scripts
$pathInstallRoot = "Install\"
$pathDatabaseInstall = "Install\Database\"
$pathInstallPowerShell = "Install\PowerShell\"
$pathInstallUtility = "Install\Utility\"

# media
$pathSharedLibrary = "Media\SharedLibrary\"
$pathProfiles = "Media\Profiles\"
$pathPublicProfile = "0\"
$pathExportEventLog = "Media\Exports\EventLog\"
$pathSourcePublicProfile = "\\$env:COMPUTERNAME\SQLEXPRESS\KeebeeAATFilestream\Media\Profiles\0\*"
$pathSourceSharedLibrary = "\\$env:COMPUTERNAME\SQLEXPRESS\KeebeeAATFilestream\Media\SharedLibrary\*"

# documentation paths
$pathDocumentation = "Install\Documentation\"

# source code
$pathSourceCode = "C:\Users\$env:USERNAME\Source\Repos\R2G2V1\"
$filenameVSSolution = "Keebee.AAT.sln"


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
    $buildResult = Build-VisualStudioSolution -SourceCodePath $pathSourceCode -SolutionFile $filenameVSSolution -BuildLogFile "R2G2BuildDebug.log" -Configuration "Debug" -CleanFirst;

    If (!$buildResult)
    {
       exit
    }

    # build release
    $buildResult = Build-VisualStudioSolution -SourceCodePath $pathSourceCode -SolutionFile $filenameVSSolution -BuildLogFile "R2G2BuildRelease.log" -Configuration "Release" -CleanFirst;

    If (!$buildResult)
    {
        exit
    }

    # delpoy components
    Write-Host -ForegroundColor yellow "`n--- Deploy Components ---`n”
    # -------------------- ROOT --------------------

    # create the root directory
    If(!(test-path $pathDeployments))
    {
        New-Item -ItemType Directory -Force -Path $pathDeployments | Out-Null
    }

    # -------------------- UI --------------------
    # display
    Write-Host "Deploying UI Components...” -NoNewline
    $path = $pathDeployments + $pathDisplayRelease + $pathVersion + "Release\"
    If(test-path $path)
    {
        Remove-Item $path -recurse -Force
    }
    New-Item -ItemType Directory -Force -Path $path | Out-Null
    Copy-Item C:\Users\$env:USERNAME\Source\Repos\R2G2V1\UI\Keebee.AAT.Display\bin\Release\* $path -recurse -Force

    $path = $pathDeployments + $pathDisplayDebug + $pathVersion + "Debug\"
    If(test-path $path)
    {
        Remove-Item $path -recurse -Force
    }
    New-Item -ItemType Directory -Force -Path $path | Out-Null
    Copy-Item C:\Users\$env:USERNAME\Source\Repos\R2G2V1\UI\Keebee.AAT.Display\bin\Debug\* $path -recurse -Force

    # simulator
    $path = $pathDeployments + $pathSimulator + $pathVersion
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
    $path = $pathDeployments + $pathDataAccess + $pathVersion
    If(test-path $path)
    {
        Remove-Item $path -recurse -Force
    }
    New-Item -ItemType Directory -Force -Path $path | Out-Null
    Copy-Item C:\Users\$env:USERNAME\Source\Repos\R2G2V1\Data\Keebee.AAT.DataAccess\* $path -recurse -Force

    # api
    $path = $pathDeployments + $pathAPI + $pathVersion
    If(test-path $path)
    {
        Remove-Item $path -recurse -Force
    }
    New-Item -ItemType Directory -Force -Path $path | Out-Null
    Copy-Item C:\Users\$env:USERNAME\Source\Repos\R2G2V1\API\Keebee.AAT.Operations\* $path -recurse -Force

    # administrator
    $path = $pathDeployments + $pathAdministator + $pathVersion
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
    $path = $pathDeployments + $pathExportEventLog
    If(test-path $path)
    {
        Remove-Item $path -recurse -Force
    }
    New-Item -ItemType Directory -Force -Path $path | Out-Null
    Write-Host "done.”

    # shared library
    Write-Host "Deploying Shared Library...” -NoNewline
    $path = $pathDeployments + $pathSharedLibrary
    If(test-path $path)
    {
        Remove-Item $path -recurse -Force
    } 
    New-Item -ItemType Directory -Force -Path $path | Out-Null
    Copy-Item $pathSourceSharedLibrary $path -recurse -Force
    Write-Host "done.”

    # clear out existing profiles
    $path = $pathDeployments + $pathProfiles
    If(test-path $path)
    {
        Remove-Item $path -recurse -Force
    }

    # public profile
    Write-Host "Deploying Public Profile...” -NoNewline
    $path = $pathDeployments + $pathProfiles + $pathPublicProfile
    If(test-path $path)
    {
        Remove-Item $path -recurse -Force
    }
    $path = $pathDeployments + $pathProfiles + $pathPublicProfile
    New-Item -ItemType Directory -Force -Path $path | Out-Null
    Copy-Item $pathSourcePublicProfile $path -recurse -Force
    Write-Host "done.”

    # -------------------- SCHEDULED TASKS --------------------

    # create scheduled tasks root
    $path = $pathDeployments + $pathScheduledTasks
    If(test-path $path)
    {
        Remove-Item $path -recurse -Force
    }
    New-Item -ItemType Directory -Force -Path $path | Out-Null

    # event log exporter
    Write-Host "Deploying Scheduled Tasks...” -NoNewline
    $path = $pathDeployments + $pathScheduledTasks + $pathEventLogExporter + $pathVersion
    If(test-path $path)
    {
        Remove-Item $path -recurse -Force
    }
    New-Item -ItemType Directory -Force -Path $path | Out-Null
    Copy-Item C:\Users\$env:USERNAME\Source\Repos\R2G2V1\ScheduledTasks\Keebee.AAT.EventLogExporter\bin\Release\* $path -recurse -Force

    # video capture file cleanup
    $path = $pathDeployments + $pathScheduledTasks + $pathVideoCaptureCleanup + $pathVersion
    If(test-path $path)
    {
        Remove-Item $path -recurse -Force
    }
    New-Item -ItemType Directory -Force -Path $path | Out-Null
    Copy-Item C:\Users\$env:USERNAME\Source\Repos\R2G2V1\ScheduledTasks\Keebee.AAT.VideoCaptureFileCleanup\bin\Release\* $path -recurse -Force

    # backup
    $path = $pathDeployments + $pathScheduledTasks + $pathBackup + $pathVersion
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

    # root
    $path = $pathDeployments + $pathInstallRoot
    If(test-path $path)
    {
        Remove-Item $path -recurse -Force
    }
    New-Item -ItemType Directory -Force -Path $path | Out-Null
    Copy-Item C:\Users\$env:USERNAME\Source\Repos\R2G2V1\InstallScripts\* $path -recurse -Force

    $path = $pathDeployments + $pathInstallRoot + $pathDatabaseInstall
    If(test-path $path)
    {
        Remove-Item $path -recurse -Force
    }
    New-Item -ItemType Directory -Force -Path $path | Out-Null
    Copy-Item C:\Users\$env:USERNAME\Source\Repos\R2G2V1\InstallScripts\Database\* $path -recurse -Force

    $path = $pathDeployments + $pathInstallRoot + $pathInstallPowerShell
    If(!(test-path $path))
    {
        New-Item -ItemType Directory -Force -Path $path | Out-Null
    }
    Copy-Item C:\Users\$env:USERNAME\Source\Repos\R2G2V1\InstallScripts\PowerShell\* $path -recurse -Force

    $path = $pathDeployments + $pathInstallRoot + $pathInstallUtility
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
    $path = $pathDeployments + $pathStateMachine + $pathVersion
    If(test-path $path)
    {
        Remove-Item $path -recurse -Force
    }
    New-Item -ItemType Directory -Force -Path $path | Out-Null
    Copy-Item C:\Users\$env:USERNAME\Source\Repos\R2G2V1\Service\Keebee.AAT.StateMachineService\bin\Release\* $path -recurse -Force 

    # phidget service
    $path = $pathDeployments + $pathPhidget+ $pathVersion
    If(test-path $path)
    {
        Remove-Item $path -recurse -Force
    }
    New-Item -ItemType Directory -Force -Path $path | Out-Null
    Copy-Item C:\Users\$env:USERNAME\Source\Repos\R2G2V1\Service\Keebee.AAT.PhidgetService\bin\Release\* $path -recurse -Force

    # bluetooth beacon watcher service
    $path = $pathDeployments + $pathBluetoothBeacon + $pathVersion
    If(test-path $path)
    {
        Remove-Item $path -recurse -Force
    }
    New-Item -ItemType Directory -Force -Path $path | Out-Null
    Copy-Item C:\Users\$env:USERNAME\Source\Repos\R2G2V1\Service\Keebee.AAT.BluetoothBeaconWatcherService\bin\Release\* $path -recurse -Force

    # video capture service
    $path = $pathDeployments + $pathVideoCapture + $pathVersion
    If(test-path $path)
    {
        Remove-Item $path -recurse -Force
    }
    New-Item -ItemType Directory -Force -Path $path | Out-Null
    Copy-Item C:\Users\$env:USERNAME\Source\Repos\R2G2V1\Service\Keebee.AAT.VideoCaptureService\bin\Release\* $path -recurse -Force

    # keep iis alive service
    $path = $pathDeployments + $pathKeepIISAlive + $pathVersion
    If(test-path $path)
    {
        Remove-Item $path -recurse -Force
    }
    New-Item -ItemType Directory -Force -Path $path | Out-Null
    Copy-Item C:\Users\$env:USERNAME\Source\Repos\R2G2V1\Service\Keebee.AAT.KeepIISAliveService\bin\Release\* $path -recurse -Force
    Write-Host "done.”

    # documentation
    Write-Host "Deploying Setup Documentation...” -NoNewline
    $path = $pathDeployments + $pathDocumentation
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