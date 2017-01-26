$pathProfiles = "C:\Deployments\Media\Profiles"
$pathsql = "C:\Deployments\Install\Database\SQL Server"
$sqlFilename = "$pathsql\CreatePreloadedResidents.sql"
$sql = ""

Try
{
    "DECLARE @pathProfile varchar(max)" | Out-File $sqlFilename
    "SET @pathProfile = '$pathSqlProfiles'`r`n" | Out-File $sqlFilename -Append

    #read the folder names to get residentIds
    $residentIds = Get-ChildItem $pathProfiles | Where-Object {$_.Mode -match "d"}
    
    # insert residents
    "SET IDENTITY_INSERT [dbo].[Residents] ON" | Out-File $sqlFilename -Append
    foreach($residentId in $residentIds)
    {
        If ($residentId.ToString() -eq "0") { continue }

        "IF NOT EXISTS (SELECT * FROM Residents WHERE Id = $residentId)`n" +
        "INSERT [dbo].[Residents] ([Id], [FirstName], [LastName], [Gender], [GameDifficultyLevel], [AllowVideoCapturing], [DateCreated], [DateUpdated]) " +
        "VALUES($residentId, 'Resident $residentId', '', 'F', 1, 1, GetDate(), GetDate())" | Out-File $sqlFilename -Append
    }
    "SET IDENTITY_INSERT [dbo].[Residents] OFF" | Out-File $sqlFilename -Append

    # insert resident media
    foreach($residentId in $residentIds)
    {
        If ($residentId.ToString() -eq "0") { continue }

        $sql = "`r`n--- ResidentId $residentId ---`r`n`r`n" +
        "IF NOT EXISTS (SELECT * FROM Residents WHERE Id = $residentId)`n" +
        "BEGIN" +
        "--- Activity 1 - ResponseType 'SlideShow' ---`r`n" +
        "INSERT INTO ResidentMediaFiles (IsPublic, ResidentId, ResponseTypeId, MediaPathTypeId, StreamId)`r`n" +
        "SELECT 0, $residentId, 1, 3, StreamId FROM MediaFiles WHERE [Path] = @pathProfile + " +
        "'$residentId\images\general\' AND [FileType] IN ('jpg', 'jpeg', 'png', 'bmp', 'gif')`r`n" + 
        "INSERT INTO ResidentMediaFiles (IsPublic, ResidentId, ResponseTypeId, MediaPathTypeId, StreamId)`r`n" +
        "SELECT 0, $residentId, 1, 4, StreamId FROM MediaFiles WHERE [Path] = @pathProfile + " +
        "'$residentId\images\personal\' AND [FileType] IN ('jpg', 'jpeg', 'png', 'bmp', 'gif')" +

        "`r`n`r`n--- Activity 2 - ResponseType 'MatchingGame' ---`r`n" + 
        "INSERT INTO ResidentMediaFiles (IsPublic, ResidentId, ResponseTypeId, MediaPathTypeId, StreamId)`r`n" + 
        "SELECT 0, $residentId, 2, 8, StreamId FROM MediaFiles WHERE [Path] = @pathProfile + " +
        "'$residentId\activities\matching-game\shapes\' AND [FileType] = 'png'`r`n" + 
        "INSERT INTO ResidentMediaFiles (IsPublic, ResidentId, ResponseTypeId, MediaPathTypeId, StreamId)`r`n" +
        "SELECT 0, $residentId, 2, 9, StreamId FROM MediaFiles WHERE [Path] = @pathProfile + " +
        "'$residentId\activities\matching-game\sounds\' AND [FileType] = 'mp3'" +

        "`r`n`r`n--- Activity 5 - ResponseType 'Radio' ---`r`n" +
        "INSERT INTO ResidentMediaFiles (IsPublic, ResidentId, ResponseTypeId, MediaPathTypeId, StreamId)`r`n" + 
        "SELECT 0, $residentId, 5, 1, StreamId FROM MediaFiles WHERE [Path] = @pathProfile + " +
        "'$residentId\audio\music\' AND [FileType] = 'mp3'`r`n" +
        "INSERT INTO ResidentMediaFiles (IsPublic, ResidentId, ResponseTypeId, MediaPathTypeId, StreamId)`r`n" +
        "SELECT 0, $residentId, 5, 2, StreamId FROM MediaFiles WHERE [Path] = @pathProfile + " +
        "'$residentId\audio\radio-shows\' AND [FileType] = 'mp3'" +

        "`r`n`r`n--- Activity 5 - ResponseType 'Television' ---`n" +
        "INSERT INTO ResidentMediaFiles (IsPublic, ResidentId, ResponseTypeId, MediaPathTypeId, StreamId)`r`n" +
        "SELECT 0, $residentId, 6, 5, StreamId FROM MediaFiles WHERE [Path] = @pathProfile + " +
            "'$residentId\videos\tv-shows\' AND [FileType] = 'mp4'`r`n" +
        "INSERT INTO ResidentMediaFiles (IsPublic, ResidentId, ResponseTypeId, MediaPathTypeId, StreamId)`r`n" +
        "SELECT 0, $residentId, 6, 6, StreamId FROM MediaFiles WHERE [Path] = @pathProfile + " +
        "'$residentId\videos\home-movies\' AND [FileType] = 'mp4'" +
        "END"

        $sql | Out-File $sqlFilename -Append
    }
}
Catch
{
    throw $_.Exception.Message
}