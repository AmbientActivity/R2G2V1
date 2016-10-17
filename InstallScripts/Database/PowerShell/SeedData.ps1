$server = $env:COMPUTERNAME + "\SQLEXPRESS"
$database = "KeebeeAAT"
$path = "C:\Deployments\Install\Database\SQL Server\"


# check if the database exists
$query = Invoke-SqlQuery -Query "SELECT COUNT(*) AS DatabaseCount FROM master.sys.databases WHERE name = N'$database'" -Server $server -Database "master"
$databaseCount = $query.DatabaseCount

# if the database doesn't exist, don't attempt anything
if ($databaseCount -eq 0) {
    Write-Host -ForegroundColor yellow "`nR2G2 database does not exist.`n"
} 
else
{
    Try
    {
        # check if there are any configurations
        $query = Invoke-SqlQuery -Query "SELECT COUNT(*) AS ConfigCount FROM Configs" -Server $server -Database $database
        $configCount = $query.ConfigCount

        # if there is already data, don't rerun
        if ($configCount -gt 0) {
            Write-Host "`nData has already been seeded."
        } 
        else {
            $mediaDestination = "\\" + $env:COMPUTERNAME + "\SQLEXPRESS\KeebeeAATFilestream\Media\"
            $mediaProfiles = $mediaDestination + "\Profiles\*"
            $mediaExports = $mediaDestination + "\Exports\*"

            Write-Host "`nTransfering media...” -NoNewline

            If(test-path $mediaProfiles)
            {
                Remove-Item $mediaProfiles -recurse -Force
            }

            If(test-path $mediaExports)
            {
                Remove-Item $mediaExports -recurse -Force
            }

            Copy-Item C:\Deployments\Media\* $mediaDestination -recurse -Force
            Write-Host "done.`n”

            Write-Host "Creating MediaFiles view...” -NoNewline
            $queryFile = $path + "CreateMediaFilesView.sql"
            Invoke-SqlQuery -File $queryFile -Server $server -Database $database
            Write-Host "done.`n”

            Write-Host "Seeding configuration data...” -NoNewline
            $queryFile = $path + "SeedConfigurationData.sql"
            Invoke-SqlQuery -File $queryFile -Server $server -Database $database
            Write-Host "done.`n”

            Write-Host "Seeding public library...” -NoNewline
            $queryFile = $path + "SeedPublicLibrary.sql"
            Invoke-SqlQuery -File $queryFile -Server $server -Database $database
            Write-Host "done.`n”

            Write-Host "Data seeded successfully!`n”
        }
    }
    Catch
    {
        Write-Host -ForegroundColor red $_.Exception.Message
    }
}