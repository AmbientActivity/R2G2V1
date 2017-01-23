$server = $env:COMPUTERNAME + "\SQLEXPRESS"
$database = "KeebeeAAT"
$path = "C:\Deployments\Install\Database\SQL Server\"

Try
{
    Write-Host -ForegroundColor yellow "`n--- Database ---`n"

    # check if the database already exists
    $query = Invoke-SqlQuery -Query "SELECT COUNT(*) AS DatabaseCount FROM master.sys.databases WHERE name = N'$database'" -Server $server -Database "master"
    $databaseCount = $query.DatabaseCount

    # if there's already data, don't rerun
    if ($databaseCount -eq 0) {
        Write-Host -ForegroundColor yellow "`nR2G2 database does not exist. Nothing dropped`n"
    } 
    else {

        # restart IIS
        Write-Host "Restarting IIS...” -NoNewline
        invoke-command -scriptblock {iisreset} | Out-Null
        Write-Host "done.”

        Write-Host "Dropping users...” -NoNewline
        $queryFile = $path + "DropUsers.sql"
        Invoke-SqlQuery -File $queryFile -Server $server -Database "KeebeeAAT"
        Write-Host "done.”

        Write-Host "Dropping database...” -NoNewline
        $queryFile = $path + "DropDatabase.sql"
        Invoke-SqlQuery -File $queryFile -Server $server -Database "master"
        Write-Host "done.”

        # restart SQLEXPRESS
        Write-Host "Restarting SQLEXPRESS...” -NoNewline
        $SQLEXPRESS = 'MSSQL$SQLEXPRESS'
        Restart-Service -Force $SQLEXPRESS -WarningAction SilentlyContinue
        Write-Host "done.”
    }
}
Catch
{
    Write-Host -ForegroundColor red $_.Exception.Message
}