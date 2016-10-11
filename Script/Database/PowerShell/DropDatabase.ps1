$server = $env:COMPUTERNAME + "\SQLEXPRESS"
$database = "KeebeeAAT"
$path = "C:\Deployments\Scripts\Database\SQL Server\"

# check if the database already exists
$query = Invoke-SqlQuery -Query "SELECT COUNT(*) AS DatabaseCount FROM master.sys.databases WHERE name = N'$database'" -Server $server -Database "master"
$databaseCount = $query.DatabaseCount

# if there's already data, don't rerun
if ($databaseCount -eq 0) {
    Write-Host -ForegroundColor yellow "`nR2G2 database does not exist. Nothing dropped`n"
} 
else {
    Try
    {
        # restart IIS
        Write-Host "IIS Restart”
        Write-Host "-----------”
        invoke-command -scriptblock {iisreset}

        Write-Host "`nDropping users...” -NoNewline
        $queryFile = $path + "XXX_DropUsers.sql"
        Invoke-SqlQuery -File $queryFile -Server $server -Database "KeebeeAAT"
        Write-Host "done.”

        Write-Host "`nDropping database...” -NoNewline
        $queryFile = $path + "XXX_DropDatabase.sql"
        Invoke-SqlQuery -File $queryFile -Server $server -Database "master"
        Write-Host "done.`n”

        Write-Host -ForegroundColor green "Database dropped successfully.`n”
    }
    Catch
    {
        Write-Host -ForegroundColor red $_.Exception.Message
    }
}