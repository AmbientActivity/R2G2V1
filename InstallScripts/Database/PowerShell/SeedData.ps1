Try
{
    $database = "KeebeeAAT"
    $server = $env:COMPUTERNAME + "\SQLEXPRESS"

    $mediaDestination = "\\$server\KeebeeAATFilestream\Media\"
    $pathDeployments = "C:\Deployments"
    $pathSqlScript = "$pathDeployments\Install\Database\SQL Server\"
    $pathPublicLibrary = "Profiles\0"
    
    Write-Host -ForegroundColor yellow "`n--- Seed ---`n"

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
        $query = Invoke-SqlQuery -Query "SELECT COUNT(*) AS ConfigCount FROM Configs" -Server $server -Database $database
        $configCount = $query.ConfigCount

        # if there is already data, don't rerun
        if ($configCount -gt 0) {
            Write-Host "Data has already been seeded."
        } 
        else {
            
            $mediaProfiles = $mediaDestination + "\Profiles\*"
            If(test-path $mediaProfiles)
            {
                Remove-Item $mediaProfiles -recurse -Force
            }

            $mediaExports = $mediaDestination + "\Exports\*"
            If(test-path $mediaExports)
            {
                Remove-Item $mediaExports -recurse -Force
            }

            Write-Host "Transferring startup nmedia...” -NoNewline
            Copy-Item "$pathDeployments\Media\$pathPublicLibrary" "$mediaDestination\$pathPublicLibrary" -recurse -Force
            Copy-Item "$pathDeployments\Media\Exports" "$mediaDestination\Exports" -recurse -Force
            Write-Host "done.”

            Write-Host "Seeding configuration data...” -NoNewline
            $queryFile = $pathSqlScript + "CreateMediaFilesView.sql"
            Invoke-SqlQuery -File $queryFile -Server $server -Database $database

            $queryFile = $pathSqlScript + "SeedConfigurationData.sql"
            Invoke-SqlQuery -File $queryFile -Server $server -Database $database
            Write-Host "done.”

            Write-Host "Seeding public library...” -NoNewline
            $queryFile = $pathSqlScript + "SeedPublicLibrary.sql"
            Invoke-SqlQuery -File $queryFile -Server $server -Database $database
            Write-Host "done.”
        }
    }
}
Catch
{
    throw $_.Exception.Message
}