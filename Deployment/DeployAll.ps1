# setup
$rootDrive = "C:"
$pathDeployments = "$rootDrive\Deployments\"
$pathVersion = "1.0.0.0\"

# service paths
$pathPhidgetServiceExe = $pathDeployments + "Services\PhidgetService\" + $pathVersion + "Keebee.AAT.PhidgetService.exe"
$pathStateMachineServiceExe = $pathDeployments + "Services\StateMachineService\" + $pathVersion + "Keebee.AAT.StateMachineService.exe"
$pathBluetoothBeaconServiceExe = $pathDeployments + "Services\BluetoothBeaconWatcherService\" + $pathVersion + "Keebee.AAT.BluetoothBeaconWatcherService.exe"
$pathVideoCaptureServiceExe = $pathDeployments + "Services\VideoCaptureService\" + $pathVersion + "Keebee.AAT.VideoCaptureService.exe"
$pathKeepIISAliveServiceExe = $pathDeployments + "Services\KeepIISAliveService\" + $pathVersion + "Keebee.AAT.KeepIISAliveService.exe"

$pathServicesRoot = $pathDeployments + "Services\"
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
$pathBeaconMonitor = "UI\BeaconMonitor\"
$pathFlashBuilds = "Flash\Builds\"

# scheduled tasks
$pathScheduledTasks = "ScheduledTasks\"
$pathEventLogExporter = "EventLogExporter\"
$pathVideoCaptureFileCleanup = "VideoCaptureFileCleanup\"
$pathBackup = "Backup\"

# thumbnail generator
$pathThumbnailGenerator = $pathDeployments + "Install\Assembly\ThumbnailGenerator\"

# video converter
$pathVideoConverter = $pathDeployments + "Install\Assembly\VideoConverter\"

# install scripts
$pathInstallRoot = $pathDeployments + "Install\"

# documentation paths
$pathDocumentation = "Install\Documentation\"

# source code
$pathSourceCode = "$rootDrive\Users\$env:USERNAME\Source\Repos\R2G2V1\"
$filenameVSSolution = "Keebee.AAT.sln"

# media
$pathSharedLibrary = "Media\SharedLibrary\"
$pathProfiles = "Media\Profiles\"
$pathPublicProfile = "0\"
$pathSampleResidentProfile = "1\"
$pathExportEventLog = "Media\Exports\EventLog\"

$pathSourcePublicProfile = "$pathSourceCode\Media\Profiles\$pathPublicProfile*"
$pathSourceSampleResidentProfile = "$pathSourceCode\Media\Profiles\$pathSampleResidentProfile*"
$pathSourceSharedLibrary = "$pathSourceCode\Media\SharedLibrary\*"

Try
{
    Write-Host -foregroundcolor green "`nDeploying R2G2...`n”

    # stop all services
    Write-Host -ForegroundColor yellow "--- Uninstall Services ---`n”

    Write-Host "Uninstalling Phidget Service..." -NoNewline
    If(test-path $pathPhidgetServiceExe)
    { 
        Invoke-Command -ScriptBlock { C:\WINDOWS\Microsoft.NET\Framework\v4.0.30319\InstallUtil.exe /uninstall $pathPhidgetServiceExe} | Out-Null
    }
    Write-Host "done."

    Write-Host "Uninstalling Video Capture Service..." -NoNewline
    If(test-path $pathVideoCaptureServiceExe)
    { 
        Invoke-Command -ScriptBlock { C:\WINDOWS\Microsoft.NET\Framework\v4.0.30319\InstallUtil.exe /uninstall $pathVideoCaptureServiceExe} | Out-Null
    }
    Write-Host "done."

    Write-Host "Uninstalling Bluetooth Beacon Watcher Service..." -NoNewline
    If(test-path $pathBluetoothBeaconServiceExe)
    { 
        Invoke-Command -ScriptBlock { C:\WINDOWS\Microsoft.NET\Framework\v4.0.30319\InstallUtil.exe /uninstall $pathBluetoothBeaconServiceExe} | Out-Null
    }
    Write-Host "done."

    Write-Host "Uninstalling State Machine Service..." -NoNewline
    If(test-path $pathStateMachineServiceExe)
    { 
        Invoke-Command -ScriptBlock { C:\WINDOWS\Microsoft.NET\Framework\v4.0.30319\InstallUtil.exe /uninstall $pathStateMachineServiceExe } | Out-Null
    }
    Write-Host "done."

    Write-Host "Uninstalling Keep IIS Alive Service..." -NoNewline
    If(test-path $pathKeepIISAliveServiceExe)
    { 
        Invoke-Command -ScriptBlock { C:\WINDOWS\Microsoft.NET\Framework\v4.0.30319\InstallUtil.exe /uninstall $pathKeepIISAliveServiceExe} | Out-Null
    }
    Write-Host "done."

    # build the solution
    Write-Host -ForegroundColor yellow "`n--- Build Solution ---`n”

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
    Copy-Item C:\Users\$env:USERNAME\Source\Repos\R2G2V1\Flash\Builds\* $path -recurse -Force

    $path = $pathDeployments + $pathDisplayDebug + $pathVersion + "Debug\"
    If(test-path $path)
    {
        Remove-Item $path -recurse -Force
    }
    New-Item -ItemType Directory -Force -Path $path | Out-Null
    Copy-Item C:\Users\$env:USERNAME\Source\Repos\R2G2V1\UI\Keebee.AAT.Display\bin\Debug\* $path -recurse -Force
    Copy-Item C:\Users\$env:USERNAME\Source\Repos\R2G2V1\Flash\Builds\* $path -recurse -Force

    # simulator
    $path = $pathDeployments + $pathSimulator + $pathVersion
    If(test-path $path)
    {
        Remove-Item $path -recurse -Force
    }
    New-Item -ItemType Directory -Force -Path $path | Out-Null
    Copy-Item C:\Users\$env:USERNAME\Source\Repos\R2G2V1\UI\Keebee.AAT.Simulator\bin\Release\* $path -recurse -Force

    # beacon monitor
    $path = $pathDeployments + $pathBeaconMonitor + $pathVersion
    If(test-path $path)
    {
        Remove-Item $path -recurse -Force
    }
    New-Item -ItemType Directory -Force -Path $path | Out-Null
    Copy-Item C:\Users\$env:USERNAME\Source\Repos\R2G2V1\UI\Keebee.AAT.BeaconMonitor\bin\Release\* $path -recurse -Force

    Write-Host "done.”


    # -------------------- WEB --------------------
    # data access
    Write-Host "Deploying Web Components...” -NoNewline
    
    $path = $pathDeployments + $pathDataAccess + $pathVersion
    If(test-path $path)
    {
        Remove-Item $path -recurse -Force
    }
    New-Item -ItemType Directory -Force -Path $path\bin | Out-Null

    $source = "C:\Users\$env:USERNAME\Source\Repos\R2G2V1\Data\Keebee.AAT.DataAccess"
    Copy-Item $source\bin\* $path\bin -Recurse -Exclude "*.pdb", "*.xml" -Force
    Copy-Item $source\* $path -Recurse -Exclude "*.cs", "*.csproj", "*.user", "packages.config", "Web.*.config", "App_Start", "Controllers","Models", "Properties", "obj" -Force

    # api
    $path = $pathDeployments + $pathAPI + $pathVersion
    If(test-path $path)
    {
        Remove-Item $path -recurse -Force
    }
    New-Item -ItemType Directory -Force -Path $path\bin | Out-Null

    $source = "C:\Users\$env:USERNAME\Source\Repos\R2G2V1\API\Keebee.AAT.Operations"
    Copy-Item $source\bin\* $path\bin -Recurse -Exclude "*.pdb", "*.xml", "*.exe" -Force
    Copy-Item $source\* $path -Recurse -Exclude "*.cs", "*.csproj", "*.user", "apiapp.json", "packages.config", "Web.*.config", "App_Start", "Controllers","Helpers", "Metadata", "Properties", "obj" -Force

    # administrator
    $path = $pathDeployments + $pathAdministator + $pathVersion
    If(test-path $path)
    {
        Remove-Item $path -recurse -Force
    }
    New-Item -ItemType Directory -Force -Path $path\bin | Out-Null

    $source = "C:\Users\$env:USERNAME\Source\Repos\R2G2V1\WebUI\Keebee.AAT.Administrator"
    Copy-Item $source\bin\* $path\bin -Recurse -Exclude "*.pdb", "*.xml", "*ffmpeg.exe" -Force
    Copy-Item $source\* $path -Recurse -Exclude "*.cs", "*.csproj", "*.user", "packages.config", "Project_Readme.html", "Web.*.config", "App_Start", "Controllers", "Extensions", "Properties", "ViewModels", "obj", "bin" -Force

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

    # resident profile
    Write-Host "Deploying Sample Resident Profile...” -NoNewline
    $path = $pathDeployments + $pathProfiles + $pathSampleResidentProfile
    If(test-path $path)
    {
        Remove-Item $path -recurse -Force
    }
    $path = $pathDeployments + $pathProfiles + $pathSampleResidentProfile
    New-Item -ItemType Directory -Force -Path $path | Out-Null
    Copy-Item $pathSourceSampleResidentProfile $path -recurse -Force
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
    $path = $pathDeployments + $pathScheduledTasks + $pathVideoCaptureFileCleanup + $pathVersion
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
    If(test-path $pathInstallRoot)
    {
        Remove-Item  $pathInstallRoot -recurse -Force
    }
    New-Item -ItemType Directory -Force -Path $pathInstallRoot | Out-Null
    Copy-Item C:\Users\$env:USERNAME\Source\Repos\R2G2V1\InstallScripts\* $pathInstallRoot -recurse -Force

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
    $path = $pathDeployments + $pathPhidget + $pathVersion
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

    # -------------------- Thumbnail Generation --------------------

    # thumbnails
    Write-Host "Deploying Thumbnail Generator...” -NoNewline

    # thumbnail installation path
    $path = $pathThumbnailGenerator + $pathVersion
    If(test-path $path)
    {
        Remove-Item $path -recurse -Force
    }
    New-Item -ItemType Directory -Force -Path $path | Out-Null
    Copy-Item C:\Users\$env:USERNAME\Source\Repos\R2G2V1\Install\Keebee.AAT.GenerateThumbnails\bin\Release\* $path -recurse -Force

    Write-Host "done.”

        # -------------------- Video Conversion --------------------

    # videos
    Write-Host "Deploying Video Converter...” -NoNewline

    # thumbnail installation path
    $path = $pathVideoConverter + $pathVersion
    If(test-path $path)
    {
        Remove-Item $path -recurse -Force
    }
    New-Item -ItemType Directory -Force -Path $path | Out-Null
    Copy-Item C:\Users\$env:USERNAME\Source\Repos\R2G2V1\Install\Keebee.AAT.ConvertVideos\bin\Debug\* $path -recurse -Force

    Write-Host "done.”

    # -------------------- Documentation --------------------

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


    # cleanup garbage
    get-childitem $pathDeployments -include *.pdb -recurse | foreach ($_) {remove-item $_.fullname}
    get-childitem $pathDeployments -include *.vshost.* -recurse | foreach ($_) {remove-item $_.fullname}
    get-childitem $pathDeployments -include *TemporaryGeneratedFile_* -recurse | foreach ($_) {remove-item $_.fullname}
    get-childitem $pathDeployments -include *.gitignore -recurse | foreach ($_) {remove-item $_.fullname}

    Write-Host -foregroundcolor green "`nDeployment complete.`n”
}
Catch
{
    Write-Host -ForegroundColor red $_.Exception.Message
}