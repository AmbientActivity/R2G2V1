﻿$server = $env:COMPUTERNAME + "\SQLEXPRESS"
$database = "KeebeeAAT"
$path = "C:\Deployments\Install\Database\SQL Server\"


# check if the database exists
$query = Invoke-SqlQuery -Query "SELECT COUNT(*) AS DatabaseCount FROM master.sys.databases WHERE name = N'$database'" -Server $server -Database "master"
$databaseCount = $query.DatabaseCount

# if the database doesn't exist, don't attempt anything
if ($databaseCount -eq 0) {
    Write-Host -ForegroundColor yellow "`nR2G2 database does not exist.`n"
} 
else
{
    Try
    {
        Write-Host "Applying Update 1...” -NoNewline
        $queryFile = $path + "ApplyUpdate_1.sql"
        Invoke-SqlQuery -File $queryFile -Server $server -Database $database
        Write-Host "done.`n”

        Write-Host "Update 1 applied successfully!`n”
    }
    Catch
    {
        Write-Host -ForegroundColor red $_.Exception.Message
    }
}