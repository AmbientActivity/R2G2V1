﻿$server = $env:COMPUTERNAME + "\SQLEXPRESS"
$database = "KeebeeAAT"
$path = "C:\Deployments\Install\Database\SQL Server\"
$url = "http://localhost/Keebee.AAT.Operations/api/Configs"

Try
{
    Write-Host -ForegroundColor yellow "`n--- Database Tables ---`n"

    # check if the database exists
    $query = Invoke-SqlQuery -Query "SELECT COUNT(*) AS DatabaseCount FROM master.sys.databases WHERE name = N'$database'" -Server $server -Database "master"
    $databaseCount = $query.DatabaseCount

    # if the database doesn't exist, don't attempt anything
    if ($databaseCount -eq 0) {
        Write-Host -ForegroundColor yellow "`nR2G2 database does not exist.`n"
    } 
    else {

        $query = Invoke-SqlQuery -Query "SELECT ISNULL(OBJECT_ID('Configs', 'U'), 0) AS CONFIG_ID" -Server $server -Database $database
        $configId = $query.CONFIG_ID

        If ($configId -gt 0)
        {
            Write-Host "`nDropping existing tables...” -NoNewline
            $queryFile = $path + "DropTables.sql"
            Invoke-SqlQuery -File $queryFile -Server $server -Database $database
            Write-Host "done.`n”
        }

        # restart IIS
        Write-Host "Restarting IIS...” -NoNewline
        invoke-command -scriptblock {iisreset} | Out-Null
        Write-Host "done.”

        Write-Host "Creating tables...” -NoNewline
        $webclient = New-Object System.Net.WebClient
        $result = $webclient.DownloadString($url)
        Write-Host "done."
    }
}
Catch
{
    Write-Host -ForegroundColor red $_.Exception.Message
}