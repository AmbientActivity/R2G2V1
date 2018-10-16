# setup
$rootDrive = "C:"
$pathDeployments = "$rootDrive\Deployments\"
$pathVersion = "1.0.0.0\"
$pathStateMachine = "$pathDeployments\Services\StateMachineService\"
$pathAdministator = "$pathDeployments\Web\Administrator\"
$pathSQLInstallScripts = "$pathDeployments\Install\Database\SQL Server\"
$pathFiles = "$pathDeployments\Updates\1\Files"
$pathAdministratorFiles = "$pathFiles\Administrator"
$pathStateMachineFiles = "$pathFiles\StateMachineService"
$pathSQLInstallScriptFiles = "$pathFiles\InstallScripts\Database"

# service paths
$pathPhidgetServiceExe = $pathDeployments + "Services\PhidgetService\" + $pathVersion + "Keebee.AAT.PhidgetService.exe"
$pathStateMachineServiceExe = $pathDeployments + "Services\StateMachineService\" + $pathVersion + "Keebee.AAT.StateMachineService.exe"
$pathBluetoothBeaconServiceExe = $pathDeployments + "Services\BluetoothBeaconWatcherService\" + $pathVersion + "Keebee.AAT.BluetoothBeaconWatcherService.exe"
$pathVideoCaptureServiceExe = $pathDeployments + "Services\VideoCaptureService\" + $pathVersion + "Keebee.AAT.VideoCaptureService.exe"
$pathKeepIISAliveServiceExe = $pathDeployments + "Services\KeepIISAliveService\" + $pathVersion + "Keebee.AAT.KeepIISAliveService.exe"

$server = $env:COMPUTERNAME + "\SQLEXPRESS"
$database = "KeebeeAAT"

$path = "C:\Deployments\Install\Utility\PhidgetStyleType\Database\"

$videoCaptureIsInstalled = $False
$beaconServiceIsInstalled = $False

Try
{
    # check if the database exists
    $query = Invoke-SqlQuery -Query "SELECT COUNT(*) AS DatabaseCount FROM master.sys.databases WHERE name = N'$database'" -Server $server -Database "master"
    $databaseCount = $query.DatabaseCount

    # if the database doesn't exist, don't attempt anything
    if ($databaseCount -eq 0) {
        Write-Host -ForegroundColor yellow "`nR2G2 database does not exist.`n"
    }
    else
    {
        Write-Host -ForegroundColor green "`n--- Install Update 1 ---`n”

        # -------------------- UNINSTALL SERVICES --------------------
    
        Write-Host -ForegroundColor yellow "--- Uninstall Services ---`n”

        Write-Host "Uninstalling Phidget Service..." -NoNewline
        If(test-path $pathPhidgetServiceExe)
        { 
            Invoke-Command -ScriptBlock { C:\WINDOWS\Microsoft.NET\Framework\v4.0.30319\InstallUtil.exe /uninstall $pathPhidgetServiceExe} | Out-Null
            Stop-Process -ProcessName Keebee.AAT.PhidgetService* -Force
        }
        Write-Host "done."

        If (Get-Service "VideoCaptureService" -ErrorAction SilentlyContinue)
        {
            Write-Host "Uninstalling Video Capture Service..." -NoNewline
            $videoCaptureIsInstalled = $True
            If(test-path $pathVideoCaptureServiceExe)
            { 
                Invoke-Command -ScriptBlock { C:\WINDOWS\Microsoft.NET\Framework\v4.0.30319\InstallUtil.exe /uninstall $pathVideoCaptureServiceExe} | Out-Null
                Stop-Process -ProcessName Keebee.AAT.VideoCaptureService* -Force
            }
            Write-Host "done."
        }
        
        If (Get-Service "BluetoothBeaconWatcherService" -ErrorAction SilentlyContinue)
        {
            Write-Host "Uninstalling Bluetooth Beacon Watcher Service..." -NoNewline
            $beaconServiceIsInstalled = $True
            If(test-path $pathBluetoothBeaconServiceExe)
            {
                Invoke-Command -ScriptBlock { C:\WINDOWS\Microsoft.NET\Framework\v4.0.30319\InstallUtil.exe /uninstall $pathBluetoothBeaconServiceExe} | Out-Null
                Stop-Process -ProcessName Keebee.AAT.BluetoothBeaconWatcherService* -Force
            }
            Write-Host "done."
        }

        Write-Host "Uninstalling State Machine Service..." -NoNewline
        If(test-path $pathStateMachineServiceExe)
        { 
            Invoke-Command -ScriptBlock { C:\WINDOWS\Microsoft.NET\Framework\v4.0.30319\InstallUtil.exe /uninstall $pathStateMachineServiceExe } | Out-Null
            Stop-Process -ProcessName Keebee.AAT.StateMachineService* -Force
        }
        Write-Host "done."

        Write-Host "Uninstalling Keep IIS Alive Service..." -NoNewline
        If(test-path $pathKeepIISAliveServiceExe)
        { 
            Invoke-Command -ScriptBlock { C:\WINDOWS\Microsoft.NET\Framework\v4.0.30319\InstallUtil.exe /uninstall $pathKeepIISAliveServiceExe} | Out-Null
            Stop-Process -ProcessName Keebee.AAT.KeepIISAliveService* -Force
        }
        Write-Host "done."

        Write-Host -ForegroundColor yellow "`n--- Apply Updates ---`n”

        Write-Host "Adding PhidgetStyleType for 'Non-rotational'..." -NoNewline
        $queryFile = $path + "AddPhidgetStyleType.sql"
        Invoke-SqlQuery -File $queryFile -Server $server -Database $database
        Write-Host "done."

        Write-Host "Copying files..." -NoNewline

        # state machine service
        $path = $pathStateMachine + $pathVersion
        $source = $pathStateMachineFiles
        Copy-Item $source\* $path -Force

        # administrator
        $path = $pathAdministator + $pathVersion + "\Scripts\PhidgetConfig"
        $source = $pathAdministratorFiles + "\Scripts\PhidgetConfig"
        Copy-Item $source\* $path -Force

        $path = $pathAdministator + $pathVersion
        $source = $pathAdministratorFiles
        Copy-Item $source\bin\* $path\bin -Force

        $path = $pathAdministator + $pathVersion + "\Scripts\PhidgetConfig"
        $source = $pathAdministratorFiles + "\Scripts\PhidgetConfig"
        Copy-Item $source\* $path -Force

        # sql server install scripts
        Copy-Item $pathSQLInstallScriptFiles\* $pathSQLInstallScripts -Force

        Write-Host "done."

        Write-Host -ForegroundColor yellow "`n--- Install Services ---`n"

        Write-Host "Installing State Machine Service..." -NoNewline
        Invoke-Command -ScriptBlock { C:\WINDOWS\Microsoft.NET\Framework\v4.0.30319\InstallUtil.exe /install $pathStateMachineServiceExe } | Out-Null
        Write-Host "done."

        Write-Host "Installing Phidget Service..." -NoNewline
        Invoke-Command -ScriptBlock { C:\WINDOWS\Microsoft.NET\Framework\v4.0.30319\InstallUtil.exe /install $pathPhidgetServiceExe } | Out-Null
        Write-Host "done."

        Write-Host "Installing Keep IIS Alive Service..." -NoNewline
        Invoke-Command -ScriptBlock { C:\WINDOWS\Microsoft.NET\Framework\v4.0.30319\InstallUtil.exe /install $pathKeepIISAliveServiceExe } | Out-Null
        Write-Host "done."

        if ($beaconServiceIsInstalled)
        {
            Write-Host "Installing Bluetooth Beacon Service..." -NoNewline
            Invoke-Command -ScriptBlock { C:\WINDOWS\Microsoft.NET\Framework\v4.0.30319\InstallUtil.exe /install $pathBluetoothBeaconServiceExe } | Out-Null
            Write-Host "done."
        }

        if ($videoCaptureIsInstalled)
        {
            Write-Host "Installing Video Capture Service..." -NoNewline
            Invoke-Command -ScriptBlock { C:\WINDOWS\Microsoft.NET\Framework\v4.0.30319\InstallUtil.exe /install $pathVideoCaptureServiceExe } | Out-Null
            Write-Host "done."
        }
    }
}
Catch
{
    throw $_.Exception.Message
}