$server = $env:COMPUTERNAME + "\SQLEXPRESS"
$database = "KeebeeAAT"
$path = "C:\Deployments\Scripts\Database\SQL Server\"

# check if there are any profiles
$query = Invoke-SqlQuery -Query "SELECT COUNT(*) AS ConfigCount FROM Configs" -Server $server -Database $database
$profileCount = $query.ProfileCount

# if there's already data, don't rerun
if ($profileCount -gt 0) {
    Write-Host "`nData has already been seeded."
} 
else {
    Write-Host "Creating MediaFiles view...” -NoNewline
    $queryFile = $path + "6_CreateMediaFilesView.sql"
    Invoke-SqlQuery -File $queryFile -Server $server -Database $database
    Write-Host "done.`n”

    Write-Host "Seeding configuration data...” -NoNewline
    $queryFile = $path + "7_SeedConfigurationData.sql"
    Invoke-SqlQuery -File $queryFile -Server $server -Database $database
    Write-Host "done.`n”

    Write-Host "Seeding public library...” -NoNewline
    $queryFile = $path + "8_SeedPublicLibrary.sql"
    Invoke-SqlQuery -File $queryFile -Server $server -Database $database
    Write-Host "done.`n”

    Write-Host "Data seeded successfully!`n”
}