$server = $env:COMPUTERNAME + "\SQLEXPRESS"
$database = "KeebeeAAT"
$path = "C:\Deployments\Install\Database\SQL Server\"

# register InvokeSqlQuery powershell module
$invokeSqlQueryPath = "C:\Windows\System32\WindowsPowerShell\v1.0\Modules\InvokeSqlQuery\"
If(!(test-path $invokeSqlQueryPath))
{
    Write-Host "`nRegistering Module InvokeSqlQuery...” -NoNewline
    New-Item -ItemType Directory -Force -Path $invokeSqlQueryPath | Out-Null
    Copy-Item C:\Deployments\Install\Database\PowerShell\InvokeSqlQuery\*.ps* $invokeSqlQueryPath -recurse -Force
    Write-Host "done.”
}

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
        $queryFile = $path + "EnableFilestream.sql"
        Invoke-SqlQuery -File $queryFile -Server $server -Database "master"
        Write-Host "done.`n”

        Write-Host "Creating database...” -NoNewline
        $queryFile = $path + "CreateDatabase.sql"
        Invoke-SqlQuery -File $queryFile -Server $server -Database "master"
        Write-Host "done.`n”

        Write-Host "Creating File Table...” -NoNewline
        $queryFile = $path + "CreateMediaFileTable.sql"
        Invoke-SqlQuery -File $queryFile -Server $server -Database $database
        Write-Host "done.`n”

        Write-Host "Creatng keebee login...” -NoNewline
        $queryFile = $path + "CreateKeebeeLogin.sql"
        Invoke-SqlQuery -File $queryFile -Server $server -Database $database
        Write-Host "done.`n”

        Write-Host "Creatng webuser user...” -NoNewline
        $queryFile = $path + "CreateWebuserUser.sql"
        Invoke-SqlQuery -File $queryFile -Server $server -Database $database
        Write-Host "done.`n”

        Write-Host -ForegroundColor green "Database created successfully.`n”
    }
    Catch
    {
        Write-Host -ForegroundColor red $_.Exception.Message
    }
}