$server = $env:COMPUTERNAME + "\SQLEXPRESS"
$database = "KeebeeAAT"
$path = "C:\Users\$env:USERNAME\Source\Repos\R2G2V1\DebugScripts\Database\"

Try
{
    Write-Host -ForegroundColor yellow "`n--- John's Test Configuration ---`n`n"

    # check if the database exists
    $query = Invoke-SqlQuery -Query "SELECT COUNT(*) AS DatabaseCount FROM master.sys.databases WHERE name = N'$database'" -Server $server -Database "master"
    $databaseCount = $query.DatabaseCount

    # if the database doesn't exist, don't attempt anything
    if ($databaseCount -eq 0) {
        Write-Host -ForegroundColor yellow "`nR2G2 database does not exist.`n"
    } 
    else
    {
        Write-Host "Seeding John's configuration...” -NoNewline
        $queryFile = $path + "SeedConfigurationJohn.sql"
        Invoke-SqlQuery -File $queryFile -Server $server -Database $database
        Write-Host "done.`n”
    }
}
Catch
{
    Write-Host -ForegroundColor red $_.Exception.Message
}