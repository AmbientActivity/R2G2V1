$server = $env:COMPUTERNAME + "\SQLEXPRESS"
$database = "KeebeeAAT"
$path = "C:\Users\" + $env:USERNAME + "\Source\Repos\R2G2V1\Script\Database\SQL Server\"

Write-Host "Enabling Filestream...” -NoNewline
$queryFile = $path + "1_EnableFilestream.sql"
Invoke-SqlQuery -File $queryFile -Server $server -Database "master"
Write-Host "done.`n”

Write-Host "Creating database...” -NoNewline
$queryFile = $path + "2_CreateDatabase.sql"
Invoke-SqlQuery -File $queryFile -Server $server -Database "master"
Write-Host "done.`n”

Write-Host "Creating File Table...” -NoNewline
$queryFile = $path + "3_CreateMediaFileTable.sql"
Invoke-SqlQuery -File $queryFile -Server $server -Database $database
Write-Host "done.`n”

Write-Host "Creatng keebee login...” -NoNewline
$queryFile = $path + "4_CreateKeebeeLogin.sql"
Invoke-SqlQuery -File $queryFile -Server $server -Database $database
Write-Host "done.`n”

Write-Host "Database created successfully!`n”