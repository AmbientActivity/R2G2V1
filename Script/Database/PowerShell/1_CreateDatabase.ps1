$server = $env:COMPUTERNAME + "\SQLEXPRESS"
$database = "KeebeeAAT"
$path = "C:\Deployments\Scripts\Database\SQL Server\"

# check if the database already exists
$query = Invoke-SqlQuery -Query "SELECT COUNT(*) AS DatabaseCount FROM master.sys.databases WHERE name = N'$database'" -Server $server -Database "master"
$databaseCount = $query.DatabaseCount

# if the database already exists, don't recreate it
if ($databaseCount -gt 0) {
    Write-Host -ForegroundColor yellow "`nR2G2 database has already been created.`n"
} 
else {
    Try
    {
        Write-Host "`nEnabling Filestream...” -NoNewline
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

        Write-Host "Creatng webuser user...” -NoNewline
        $queryFile = $path + "5_CreateWebuserUser.sql"
        Invoke-SqlQuery -File $queryFile -Server $server -Database $database
        Write-Host "done.`n”

        Write-Host -ForegroundColor green "Database created successfully.`n”
    }
    Catch
    {
        Write-Host -ForegroundColor red "One or more errors occurred.`n”
    }
}