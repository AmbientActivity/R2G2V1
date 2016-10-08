$destPath = "C:\Deployments\"
$versionPath = "1.0.0.0\"
$stateMachinePath = "Services\StateMachineService\"
$rfidReaderPath = "Services\RfidReaderService\"
$phidgetPath = "Services\PhidgetService\"
$videoCapturePath = "Services\VideoCaptureService\"
$keepIISAlivePath = "Services\KeepIISAliveService\"
$dataPath = "Web\Data\"
$apiPath = "Web\API\"
$administratorPath = "Web\Administrator\"
$displayReleasePath = "UI\Display\"
$displayDebugPath = "UI\Display\"
$simulatorPath = "UI\Simulator\"
$scriptDatabasePath = "Scripts\Database\"
$scriptEventLogPath = "Scripts\EventLogSource\"
$scriptMessageQueuePath = "Scripts\MessageQueue\"
$scriptServicePath = "Scripts\Service\"

$profilesPath = "Media\Profiles\0\"
$exportsPath = "Media\Exports\EventLog\"
$publicLibrarySource = "\\" + $env:COMPUTERNAME + "\SQLEXPRESS\KeebeeAATFilestream\Media\Profiles\0\*"

# restart IIS
Write-Host "IIS Restart”
Write-Host "-----------”
invoke-command -scriptblock {iisreset}

# create the root directory
If(!(test-path $destPath))
{
    New-Item -ItemType Directory -Force -Path $destPath
}

# state machine service
Write-Host -foregroundcolor green "`nDeploying State Machine Service...”
$path = $destPath + $stateMachinePath + $versionPath
If(test-path $path)
{
    Remove-Item $path -recurse -Force
}
New-Item -ItemType Directory -Force -Path $path
Copy-Item C:\Users\$env:USERNAME\Source\Repos\R2G2V1\Service\Keebee.AAT.StateMachineService\bin\Release\* $path -recurse -Force
Write-Host -foregroundcolor green "Done.”

# phidget service
Write-Host -foregroundcolor green "`nDeploying Phidget Service...”
$path = $destPath + $phidgetPath + $versionPath
If(test-path $path)
{
    Remove-Item $path -recurse -Force
}
New-Item -ItemType Directory -Force -Path $path
Copy-Item C:\Users\$env:USERNAME\Source\Repos\R2G2V1\Service\Keebee.AAT.PhidgetService\bin\Release\* $path -recurse -Force
Write-Host -foregroundcolor green "Done.”

# rfid reader service
Write-Host -foregroundcolor green "`nDeploying RFID Reader Service...”
$path = $destPath + $rfidReaderPath + $versionPath
If(test-path $path)
{
    Remove-Item $path -recurse -Force
}
New-Item -ItemType Directory -Force -Path $path
Copy-Item C:\Users\$env:USERNAME\Source\Repos\R2G2V1\Service\Keebee.AAT.RfidReaderService\bin\Release\* $path -recurse -Force
Write-Host -foregroundcolor green "Done.”

# video capture service
Write-Host -foregroundcolor green "`nDeploying Video Capture Service...”
$path = $destPath + $videoCapturePath + $versionPath
If(test-path $path)
{
    Remove-Item $path -recurse -Force
}
New-Item -ItemType Directory -Force -Path $path
Copy-Item C:\Users\$env:USERNAME\Source\Repos\R2G2V1\Service\Keebee.AAT.VideoCaptureService\bin\Release\* $path -recurse -Force
Write-Host -foregroundcolor green "Done.”

# keep iis alive service
Write-Host -foregroundcolor green "`nDeploying Keep IIS Alive Service...”
$path = $destPath + $keepIISAlivePath + $versionPath
If(test-path $path)
{
    Remove-Item $path -recurse -Force
}
New-Item -ItemType Directory -Force -Path $path
Copy-Item C:\Users\$env:USERNAME\Source\Repos\R2G2V1\Service\Keebee.AAT.KeepIISAliveService\bin\Release\* $path -recurse -Force
Write-Host -foregroundcolor green "Done.”


# display
Write-Host -foregroundcolor green "`nDeploying Display...”
$path = $destPath + $displayReleasePath + $versionPath + "Release\"
If(test-path $path)
{
    Remove-Item $path -recurse -Force
}
New-Item -ItemType Directory -Force -Path $path
Copy-Item C:\Users\$env:USERNAME\Source\Repos\R2G2V1\UI\Keebee.AAT.Display\bin\Release\* $path -recurse -Force

$path = $destPath + $displayDebugPath + $versionPath + "Debug\"
If(test-path $path)
{
    Remove-Item $path -recurse -Force
}
New-Item -ItemType Directory -Force -Path $path
Copy-Item C:\Users\$env:USERNAME\Source\Repos\R2G2V1\UI\Keebee.AAT.Display\bin\Debug\* $path -recurse -Force

Write-Host -foregroundcolor green "Done.”

# simulator
Write-Host -foregroundcolor green "`nDeploying Simulator...”
$path = $destPath + $simulatorPath + $versionPath
If(test-path $path)
{
    Remove-Item $path -recurse -Force
}
New-Item -ItemType Directory -Force -Path $path
Copy-Item C:\Users\$env:USERNAME\Source\Repos\R2G2V1\UI\Keebee.AAT.Simulator\bin\Release\* $path -recurse -Force
Write-Host -foregroundcolor green "Done.”

# data access
Write-Host -foregroundcolor green "`nDeploying Data Access...”
$path = $destPath + $dataPath + $versionPath
If(test-path $path)
{
    Remove-Item $path -recurse -Force
}
New-Item -ItemType Directory -Force -Path $path
Copy-Item C:\Users\$env:USERNAME\Source\Repos\R2G2V1\Data\Keebee.AAT.DataAccess\* $path -recurse -Force
Write-Host -foregroundcolor green "Done.”


# api
Write-Host -foregroundcolor green "`nDeploying Operations API...”
$path = $destPath + $apiPath + $versionPath
If(test-path $path)
{
    Remove-Item $path -recurse -Force
}
New-Item -ItemType Directory -Force -Path $path
Copy-Item C:\Users\$env:USERNAME\Source\Repos\R2G2V1\API\Keebee.AAT.Operations\* $path -recurse -Force
Write-Host -foregroundcolor green "Done.”


# administrator
Write-Host -foregroundcolor green "`nDeploying Administrator Interface...”
$path = $destPath + $administratorPath + $versionPath
If(test-path $path)
{
    Remove-Item $path -recurse -Force
}
New-Item -ItemType Directory -Force -Path $path
Copy-Item C:\Users\$env:USERNAME\Source\Repos\R2G2V1\WebUI\Keebee.AAT.Administrator\* $path -recurse -Force
Write-Host -foregroundcolor green "Done.”


# scripts
Write-Host -foregroundcolor green "`nDeploying Scripts...”
$path = $destPath + $scriptDatabasePath
If(test-path $path)
{
    Remove-Item $path -recurse -Force
}
New-Item -ItemType Directory -Force -Path $path
Copy-Item C:\Users\$env:USERNAME\Source\Repos\R2G2V1\Script\Database\* $path -recurse -Force

$path = $destPath + $scriptEventLogPath
If(!(test-path $path))
{
    New-Item -ItemType Directory -Force -Path $path
}
Copy-Item C:\Users\$env:USERNAME\Source\Repos\R2G2V1\Script\EventLogSource\* $path -recurse -Force

$path = $destPath + $scriptMessageQueuePath
If(test-path $path)
{
    Remove-Item $path -recurse -Force
}
New-Item -ItemType Directory -Force -Path $path
Copy-Item C:\Users\$env:USERNAME\Source\Repos\R2G2V1\Script\MessageQueue\CreateMessageQueues.ps1 $path -recurse -Force

$path = $destPath + $scriptServicePath
If(test-path $path)
{
    Remove-Item $path -recurse -Force
}
New-Item -ItemType Directory -Force -Path $path
Copy-Item C:\Users\$env:USERNAME\Source\Repos\R2G2V1\Script\Service\* $path -recurse -Force

Write-Host -foregroundcolor green "Done.”


# media
Write-Host -foregroundcolor green "`nDeploying media...”

$path = $destPath + $exportsPath
If(test-path $path)
{
    Remove-Item $path -recurse -Force
}
New-Item -ItemType Directory -Force -Path $path

$path = $destPath + $profilesPath
If(test-path $path)
{
    Remove-Item $path -recurse -Force
}
New-Item -ItemType Directory -Force -Path $path
Copy-Item $publicLibrarySource $path -recurse -Force

Write-Host -foregroundcolor green "`nR2G2 has successfully been deployed.`n”