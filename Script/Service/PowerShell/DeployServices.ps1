$server = $env:COMPUTERNAME + "\SQLEXPRESS"
$database = "KeebeeAAT"
$path = "C:\Users\" + $env:USERNAME + "\Source\Repos\R2G2V1\Script\Database\SQL Server\"
$url = "http://localhost/Keebee.AAT.Operations/api/Configs"

# restart IIS
Write-Host "IIS Restart”
Write-Host "-----------”
invoke-command -scriptblock {iisreset}

Write-Host "`nDeploying Phidget Service...”
Copy-Item C:\Users\$env:USERNAME\Source\Repos\R2G2V1\Service\Keebee.AAT.PhidgetService\bin\Debug\*.* C:\Deployments\Services\PhidgetService\1.0.0.0\
Write-Host "Done.”

Write-Host "`nDeploying RFID Reader Service...”
Copy-Item C:\Users\$env:USERNAME\Source\Repos\R2G2V1\Service\Keebee.AAT.RfidReaderService\bin\Debug\*.* C:\Deployments\Services\RfidReaderService\1.0.0.0\
Write-Host "Done.”

Write-Host "`nDeploying State Machine Service...”
Copy-Item C:\Users\$env:USERNAME\Source\Repos\R2G2V1\Service\Keebee.AAT.StateMachineService\bin\Debug\*.* C:\Deployments\Services\StateMachineService\1.0.0.0\
Write-Host "Done.”

Write-Host "`nDeploying Video Capture Service...”
Copy-Item C:\Users\$env:USERNAME\Source\Repos\R2G2V1\Service\Keebee.AAT.VideoCaptureService\bin\Debug\*.* C:\Deployments\Services\VideoCaptureService\1.0.0.0\
Write-Host "Done.”

Write-Host "`nDeploying Keep IIS Alive Service...”
Copy-Item C:\Users\$env:USERNAME\Source\Repos\R2G2V1\Service\Keebee.AAT.KeepIISAliveService\bin\Debug\*.* C:\Deployments\Services\KeepIISAliveService\1.0.0.0\
Write-Host "Done.”