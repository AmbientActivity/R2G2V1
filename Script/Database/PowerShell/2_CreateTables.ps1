﻿$server = $env:COMPUTERNAME + "\SQLEXPRESS"
$database = "KeebeeAAT"
$path = "C:\Deployments\Scripts\Database\SQL Server\"
$url = "http://localhost/Keebee.AAT.Operations/api/Configs"

# check if the database exists
$query = Invoke-SqlQuery -Query "SELECT COUNT(*) AS DatabaseCount FROM master.sys.databases WHERE name = N'$database'" -Server $server -Database "master"
$databaseCount = $query.DatabaseCount

# if the database doesn't exist, don't attempt anything
if ($databaseCount -eq 0) {
    Write-Host -ForegroundColor yellow "`nR2G2 database does not exist.`n"
} 
else {
    Try
    {
        Write-Host "Dropping tables...” -NoNewline
        $queryFile = $path + "DropTables.sql"
        Invoke-SqlQuery -File $queryFile -Server $server -Database $database
        Write-Host "done.`n”

        # restart IIS
        Write-Host "IIS Restart”
        Write-Host "-----------”
        invoke-command -scriptblock {iisreset}

        Write-Host "`nIn a moment your browser will open and create the tables...”
        Start-Sleep -s 3
        Start $url
    }
    Catch
    {
        Write-Host -ForegroundColor red "`nOne or more errors occurred.`n"
    }
}