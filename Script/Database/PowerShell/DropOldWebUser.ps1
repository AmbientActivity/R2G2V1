$server = $env:COMPUTERNAME + "\SQLEXPRESS"
$database = "KeebeeAAT"
$path = "C:\Deployments\Scripts\Database\SQL Server\"

Write-Host "Removing/creatng webuser login...” -NoNewline
$queryFile = $path + "DropOldWebuser.sql"
Invoke-SqlQuery -File $queryFile -Server $server -Database $database
Write-Host "done.`n”

Write-Host "Webuser created successfully`n”