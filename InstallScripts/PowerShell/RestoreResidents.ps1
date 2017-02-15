$server = $env:COMPUTERNAME + "\SQLEXPRESS"
$database = "KeebeeAAT"
$pathSqlScripts = "C:\Deployments\Install\Database\SQL Server\"

$pathSqlProfiles = "\\$env:COMPUTERNAME\SQLEXPRESS\KeebeeAATFilestream\Media\Profiles\"
$pathDeployments = "C:\Deployments\"
$pathProfiles = "Media\Profiles"

Try
{
    Write-Host -ForegroundColor yellow "`n--- Restore Residents ---`n"

    # check if the database exists
    $query = Invoke-SqlQuery -Query "SELECT COUNT(*) AS DatabaseCount FROM master.sys.databases WHERE name = N'$database'" -Server $server -Database "master"
    $databaseCount = $query.DatabaseCount

    # if the database doesn't exist, don't attempt anything
    if ($databaseCount -eq 0) {
        Write-Host -ForegroundColor yellow "`nR2G2 database does not exist.`n"
    }
    else
    {
        # check if there are any configurations
        $query = Invoke-SqlQuery -Query "SELECT COUNT(*) AS FileCount FROM ResidentMediaFiles" -Server $server -Database $database
        $fileCount = $query.FileCount

        # if there is already data, don't rerun
        if ($fileCount -gt 0) {
            Write-Host "Resident Profiles have already been seeded."
        } 
        else {
            #read folder names (residentIds)
            $residentIds = Get-ChildItem "$pathDeployments\$pathProfiles" | Where-Object {$_.Mode -match "d"}

            #copy media to sql server
            foreach($residentId in $residentIds)
            {
                If ($residentId.ToString() -eq "0") { continue }

                Write-Host "Transferring Profile $residentId..." -NoNewline  
                Copy-Item "$pathDeployments\$pathProfiles\$residentId" $pathSqlProfiles -Recurse -Force
                Write-Host "done."
            }

            Write-Host "Restoring Residents..."-NoNewline
            $queryFile = $pathSqlScripts + "RestoreResidents.sql"
            Invoke-SqlQuery -File $queryFile -Server $server -Database $database
            Write-Host "done."
        }
    }
}
Catch
{
    throw $_.Exception.Message
}