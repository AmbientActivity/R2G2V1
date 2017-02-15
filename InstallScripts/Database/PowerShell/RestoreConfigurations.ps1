$server = $env:COMPUTERNAME + "\SQLEXPRESS"
$database = "KeebeeAAT"
$path = "C:\Deployments\Install\Database\SQL Server\"

Try
{
    # check if the database exists
    $query = Invoke-SqlQuery -Query "SELECT COUNT(*) AS DatabaseCount FROM master.sys.databases WHERE name = N'$database'" -Server $server -Database "master"
    $databaseCount = $query.DatabaseCount

    # if the database doesn't exist, don't attempt anything
    if ($databaseCount -eq 0) {
        Write-Host -ForegroundColor yellow "`nR2G2 database does not exist.`n"
    }
    else
    {
        Write-Host "Restoring configurations..."-NoNewline
        $queryFile = $path + "RestoreConfigurations.sql"
        Invoke-SqlQuery -File $queryFile -Server $server -Database $database
        Write-Host "done."
    }
}
Catch
{
    throw $_.Exception.Message
}