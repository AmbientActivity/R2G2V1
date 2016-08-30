$server = $env:COMPUTERNAME + "\SQLEXPRESS"
$database = "KeebeeAAT"
$path = "C:\Users\" + $env:USERNAME + "\Source\Repos\keebee\Script\Database\SQL Server\"

# check if there are any profiles
$query = Invoke-SqlQuery -Query "SELECT COUNT(*) AS ProfileCount FROM Profiles" -Server $server -Database $database
$profileCount = $query.ProfileCount

# if there's already data, don't rerun
if ($profileCount -gt 0) {
    Write-Host "`nData has already been seeded."
} 
else {
    Write-Host "Creating MediaFiles view...” -NoNewline
    $queryFile = $path + "5_CreateMediaFilesView.sql"
    Invoke-SqlQuery -File $queryFile -Server $server -Database $database
    Write-Host "done.`n”

    Write-Host "Seeding initial data...” -NoNewline
    $queryFile = $path + "6_SeedInitialData.sql"
    Invoke-SqlQuery -File $queryFile -Server $server -Database $database
    Write-Host "done.`n”

    Write-Host "Seeding response data...” -NoNewline
    $queryFile = $path + "7_SeedResponses.sql"
    Invoke-SqlQuery -File $queryFile -Server $server -Database $database
    Write-Host "done.`n”

    Write-Host "Data seeded successfully!`n”
}