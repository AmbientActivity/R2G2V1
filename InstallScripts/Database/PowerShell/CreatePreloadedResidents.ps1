$server = $env:COMPUTERNAME + "\SQLEXPRESS"
$database = "KeebeeAAT"

$pathSqlProfiles = "\\$env:COMPUTERNAME\SQLEXPRESS\KeebeeAATFilestream\Media\Profiles\"
$pathProfiles = "C:\Deployments\Media\Profiles"

Try
{
    Write-Host -ForegroundColor yellow "`n--- Preloaded Residents ---`n"

    # check if the database exists
    $query = Invoke-SqlQuery -Query "SELECT COUNT(*) AS DatabaseCount FROM master.sys.databases WHERE name = N'$database'" -Server $server -Database "master"
    $databaseCount = $query.DatabaseCount

    $profilesExist = $false

    # if the database doesn't exist, don't attempt anything
    if ($databaseCount -eq 0) {
        Write-Host -ForegroundColor yellow "`nR2G2 database does not exist.`n"
    }
    else
    {
        #read folder names (residentIds)
        $residentIds = Get-ChildItem $pathProfiles | Where-Object {$_.Mode -match "d"}

        #copy media to sql server
        foreach($residentId in $residentIds)
        {
            If ($residentId.ToString() -eq "0") { continue }
            If ($residentId.ToString() -eq "common") { continue }

            $query = Invoke-SqlQuery -Query "SELECT COUNT(*) AS ResidentCount FROM Residents WHERE Id = $residentId" -Server $server -Database $database

            If ($query.ResidentCount -eq 0) {
                If (Test-Path "$pathProfiles\$residentId") {
                    $profilesExist = $true

                    Write-Host "Transferring Profile $residentId..." -NoNewline  
                    Copy-Item "$pathProfiles\$residentId" $pathSqlProfiles -Recurse -Force
                    Write-Host "done."

                    # create sql statement
                    $sql = "DECLARE @pathProfile varchar(max)`r`n" +
                           "SET @pathProfile = '$pathSqlProfiles'"

                    Write-Host "Creating Resident $residentId..."-NoNewline
                    $sql += "`r`n`r`n--- ResidentId $residentId ---`r`n`r`n" +

                    "IF NOT EXISTS (SELECT * FROM Residents WHERE Id = $residentId)`r`n" +
                    "BEGIN`r`n" +
                        "SET IDENTITY_INSERT [dbo].[Residents] ON`r`n" +
                        "INSERT [dbo].[Residents] ([Id], [FirstName], [LastName], [Gender], [GameDifficultyLevel], [AllowVideoCapturing], [DateCreated], [DateUpdated]) " +
                        "VALUES($residentId, 'Resident $residentId', null, 'F', 1, 1, GetDate(), GetDate())`r`n" +
                        "SET IDENTITY_INSERT [dbo].[Residents] OFF`r`n`r`n" +
            
                        "--- Activity 1 - ResponseType 'SlideShow' ---`r`n" +
                        "INSERT INTO ResidentMediaFiles (IsLinked, ResidentId, ResponseTypeId, MediaPathTypeId, StreamId)`r`n" +
                        "SELECT 0, $residentId, 1, 3, StreamId FROM MediaFiles WHERE [Path] = @pathProfile + " +
                        "'$residentId\images\general\' AND [FileType] IN ('jpg', 'jpeg', 'png', 'bmp', 'gif')`r`n" + 
                        "INSERT INTO ResidentMediaFiles (IsLinked, ResidentId, ResponseTypeId, MediaPathTypeId, StreamId)`r`n" +
                        "SELECT 0, $residentId, 1, 4, StreamId FROM MediaFiles WHERE [Path] = @pathProfile + " +
                        "'$residentId\images\personal\' AND [FileType] IN ('jpg', 'jpeg', 'png', 'bmp', 'gif')" +

                        "`r`n`r`n--- Activity 2 - ResponseType 'MatchingGame' ---`r`n" + 
                        "INSERT INTO ResidentMediaFiles (IsLinked, ResidentId, ResponseTypeId, MediaPathTypeId, StreamId)`r`n" + 
                        "SELECT 0, $residentId, 2, 7, StreamId FROM MediaFiles WHERE [Path] = @pathProfile + " +
                        "'$residentId\activities\matching-game\shapes\' AND [FileType] = 'png'`r`n" + 
                        "INSERT INTO ResidentMediaFiles (IsLinked, ResidentId, ResponseTypeId, MediaPathTypeId, StreamId)`r`n" +
                        "SELECT 0, $residentId, 2, 8, StreamId FROM MediaFiles WHERE [Path] = @pathProfile + " +
                        "'$residentId\activities\matching-game\sounds\' AND [FileType] = 'mp3'" +

                        "`r`n`r`n--- Activity 5 - ResponseType 'Radio' ---`r`n" +
                        "INSERT INTO ResidentMediaFiles (IsLinked, ResidentId, ResponseTypeId, MediaPathTypeId, StreamId)`r`n" + 
                        "SELECT 0, $residentId, 5, 1, StreamId FROM MediaFiles WHERE [Path] = @pathProfile + " +
                        "'$residentId\audio\music\' AND [FileType] = 'mp3'`r`n" +
                        "INSERT INTO ResidentMediaFiles (IsLinked, ResidentId, ResponseTypeId, MediaPathTypeId, StreamId)`r`n" +
                        "SELECT 0, $residentId, 5, 2, StreamId FROM MediaFiles WHERE [Path] = @pathProfile + " +
                        "'$residentId\audio\radio-shows\' AND [FileType] = 'mp3'" +

                        "`r`n`r`n--- Activity 5 - ResponseType 'Television' ---`n" +
                        "INSERT INTO ResidentMediaFiles (IsLinked, ResidentId, ResponseTypeId, MediaPathTypeId, StreamId)`r`n" +
                        "SELECT 0, $residentId, 6, 5, StreamId FROM MediaFiles WHERE [Path] = @pathProfile + " +
                        "'$residentId\videos\tv-shows\' AND [FileType] = 'mp4'`r`n" +
                        "INSERT INTO ResidentMediaFiles (IsLinked, ResidentId, ResponseTypeId, MediaPathTypeId, StreamId)`r`n" +
                        "SELECT 0, $residentId, 6, 6, StreamId FROM MediaFiles WHERE [Path] = @pathProfile + " +
                        "'$residentId\videos\home-movies\' AND [FileType] = 'mp4'`r`n" +
                    "END"

                    #Write-Host $sql
                    Invoke-SqlQuery -Query $sql -Server $server -Database $database
                    Write-Host "done.`n"
                }
            }
        }  
     
        If ($profilesExist) {
            Write-Host -ForegroundColor Cyan "Note: Must set relevant FirstName, LastName and Gender through the Administrator Interface."
        } else {
            Write-Host "None created."
        }  
    }
}
Catch
{
    Write-Host  -ForegroundColor red $_.Exception.Message
}