﻿Try
{
    $database = "KeebeeAAT"
    $server = $env:COMPUTERNAME + "\SQLEXPRESS"

    $mediaDestination = "\\$server\KeebeeAATFilestream\Media\"
    $pathDeployments = "C:\Deployments"
    $pathSqlScript = "$pathDeployments\Install\Database\SQL Server\"
    $pathProfilesPublic = "Profiles\0"

    Write-Host -ForegroundColor yellow "`n--- Restore Public Profile ---`n"

    # check if the database exists
    $query = Invoke-SqlQuery -Query "SELECT COUNT(*) AS DatabaseCount FROM master.sys.databases WHERE name = N'$database'" -Server $server -Database "master"
    $databaseCount = $query.DatabaseCount

    # if the database doesn't exist, don't attempt anything
    if ($databaseCount -eq 0) {
        Write-Host -ForegroundColor yellow "`nR2G2 database does not exist.`n"
    } 
    else
    {
        # check if there is already a public profile
        $query = Invoke-SqlQuery -Query "SELECT COUNT(*) AS FileCount FROM PublicMediaFiles" -Server $server -Database $database
        $fileCount = $query.FileCount

        # if there is already data, don't rerun
        if ($fileCount -gt 0) {
            Write-Host "Public Profile has already been restored."
        } 
        else {
            
            $mediaProfiles = $mediaDestination + "\Profiles\*"
            If(test-path $mediaProfiles)
            {
                Remove-Item $mediaProfiles -recurse -Force
            }

            Write-Host "Transferring Profile...” -NoNewline
            Copy-Item "$pathDeployments\Media\$pathProfilesPublic" $mediaDestination\$pathProfilesPublic -recurse -Force
            Write-Host "done.”

            Write-Host "Restoring Profile...” -NoNewline
            $queryFile = $pathSqlScript + "RestorePublicProfile.sql"
            Invoke-SqlQuery -File $queryFile -Server $server -Database $database
            Write-Host "done.”
        }
    }
}
Catch
{
    throw $_.Exception.Message
}