$server = $env:COMPUTERNAME + "\SQLEXPRESS"
$database = "KeebeeAAT"
$path = "C:\Users\" + $env:USERNAME + "\Source\Repos\R2G2V1\Script\Database\SQL Server\"
$url = "http://localhost/Keebee.AAT.Operations/api/Profiles"

Write-Host "Dropping tables...” -NoNewline
$queryFile = $path + "DropAll.sql"
Invoke-SqlQuery -File $queryFile -Server $server -Database $database
Write-Host "complete.`n”

# restart IIS
Write-Host "IIS Restart”
Write-Host "-----------”
invoke-command -scriptblock {iisreset}

Write-Host "`nIn a moment your browser will open and create the tables...”
Start-Sleep -s 3
Start $url