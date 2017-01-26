$server = $env:COMPUTERNAME + "\SQLEXPRESS"
$database = "KeebeeAAT"

$pathSqlProfiles = "\\$env:COMPUTERNAME\SQLEXPRESS\KeebeeAATFilestream\Media\Profiles\"
$pathProfiles = "C:\Deployments\Media\Profiles"
$pathSql = "C:\Deployments\Install\Database\SQL Server\"
$queryFile = "$pathSql\CreatePreloadedResidents.sql"

$pathPowerShell = "C:\Deployments\Install\Utility\PowerShell"
$queryBuilder = "$pathPowerShell\Build-CreatePreloadedResidentsSql.ps1"

Try
{
    # check if the database exists
    $query = Invoke-SqlQuery -Query "SELECT COUNT(*) AS DatabaseCount FROM master.sys.databases WHERE name = N'$database'" -Server $server -Database "master"
    $databaseCount = $query.DatabaseCount

    Write-Host -ForegroundColor Yellow "`n--- Create Preloaded Residents ---`n"

    # if the database doesn't exist, don't attempt anything
    if ($databaseCount -eq 0) {
        Write-Host -ForegroundColor yellow "`nR2G2 database does not exist.`n"
    }
    else
    {
        #read the folder names to get residentIds
        $residentIds = Get-ChildItem $pathProfiles | Where-Object {$_.Mode -match "d"}

        Write-Host "Transferring media..." -NoNewline
        #copy the media to sql
        foreach($residentId in $residentIds)
        {
            If ($residentId.ToString() -eq "0") { continue }
            Copy-Item "$pathProfiles\$residentId" $pathSqlProfiles -Recurse -Force
        }
        Write-Host "done."

        Write-Host "Building database script..."-NoNewline
        Invoke-Expression -Command $queryBuilder
        Write-Host "done."

        Write-Host "Creating residents..."-NoNewline
        Invoke-SqlQuery -File $queryFile -Server $server -Database $database
        Write-Host "done."

        Remove-Item $queryFile -Force

        Write-Host -ForegroundColor Cyan "`nNote: Must set relevant FirstName, LastName and Gender through the Administrator Interface."
    }
}
Catch
{
    Write-Host  -ForegroundColor red $_.Exception.Message
}