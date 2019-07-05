$server = $env:COMPUTERNAME + "\SQLEXPRESS"
$database = "KeebeeAAT"

$path = "C:\Deployments\Install\Utility\Resident\Database\"

Try
{
    Write-Host -ForegroundColor yellow "`n--- Reseed Resident Id ---`n"

    # check if the database exists
    $query = Invoke-SqlQuery -Query "SELECT COUNT(*) AS DatabaseCount FROM master.sys.databases WHERE name = N'$database'" -Server $server -Database "master"
    $databaseCount = $query.DatabaseCount

    # if the database doesn't exist, don't attempt anything
    if ($databaseCount -eq 0) {
        Write-Host -ForegroundColor yellow "`nR2G2 database does not exist.`n"
    }
    else
    {
        Write-Host "Reseeding..." -NoNewline
        $queryFile = $path + "ReseedIdentity.sql"
        Invoke-SqlQuery -File $queryFile -Server $server -Database $database
        Write-Host "done."
    }
}
Catch
{
    throw $_.Exception.Message
}