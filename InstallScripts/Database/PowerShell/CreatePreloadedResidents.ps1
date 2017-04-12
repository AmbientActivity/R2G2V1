$server = $env:COMPUTERNAME + "\SQLEXPRESS"
$database = "KeebeeAAT"

$pathSqlProfiles = "\\$env:COMPUTERNAME\SQLEXPRESS\KeebeeAATFilestream\Media\Profiles\"
$pathDeployments = "C:\Deployments\"
$pathProfiles = "Media\Profiles"

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
        $residentIds = Get-ChildItem "$pathDeployments\$pathProfiles" | Where {$_.Mode -match "d"} | Sort @{Expression = {[int]$_.Name}}

        #copy media to sql server
        foreach($residentId in $residentIds)
        {
            If ($residentId.ToString() -eq "0") { continue }

            $query = Invoke-SqlQuery -Query "SELECT COUNT(*) AS ResidentCount FROM Residents WHERE Id = $residentId" -Server $server -Database $database

            If ($query.ResidentCount -eq 0) {
                If (Test-Path "$pathDeployments\$pathProfiles\$residentId") {
                    $profilesExist = $true

                    Write-Host "Transferring Profile $residentId..." -NoNewline  
                    Copy-Item "$pathDeployments\$pathProfiles\$residentId" $pathSqlProfiles -Recurse -Force
                    Write-Host "done."

                    # create sql statement
                    $sql = "DECLARE @pathProfiles varchar(max)`r`n" +
                           "SET @pathProfiles = FileTableRootPath() + '\$pathProfiles\'"

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
                        "SELECT 0, $residentId, 1, 3, StreamId FROM MediaFiles WHERE [Path] = @pathProfiles + " +
                        "'$residentId\images\general\' AND [FileType] IN ('jpg', 'jpeg', 'png', 'bmp', 'gif')`r`n" + 
                        "INSERT INTO ResidentMediaFiles (IsLinked, ResidentId, ResponseTypeId, MediaPathTypeId, StreamId)`r`n" +
                        "SELECT 0, $residentId, 1, 4, StreamId FROM MediaFiles WHERE [Path] = @pathProfiles + " +
                        "'$residentId\images\personal\' AND [FileType] IN ('jpg', 'jpeg', 'png', 'bmp', 'gif')" +

                        "`r`n`r`n--- Activity 2 - ResponseType 'MatchingGame' ---`r`n" + 
                        "INSERT INTO ResidentMediaFiles (IsLinked, ResidentId, ResponseTypeId, MediaPathTypeId, StreamId)`r`n" + 
                        "SELECT 0, $residentId, 2, 7, StreamId FROM MediaFiles WHERE [Path] = @pathProfiles + " +
                        "'$residentId\activities\matching-game\shapes\' AND [FileType] = 'png'`r`n" + 
                        "INSERT INTO ResidentMediaFiles (IsLinked, ResidentId, ResponseTypeId, MediaPathTypeId, StreamId)`r`n" +
                        "SELECT 0, $residentId, 2, 8, StreamId FROM MediaFiles WHERE [Path] = @pathProfiles + " +
                        "'$residentId\activities\matching-game\sounds\' AND [FileType] = 'mp3'" +

                        "`r`n`r`n--- Activity 5 - ResponseType 'Radio' ---`r`n" +
                        "INSERT INTO ResidentMediaFiles (IsLinked, ResidentId, ResponseTypeId, MediaPathTypeId, StreamId)`r`n" + 
                        "SELECT 0, $residentId, 5, 1, StreamId FROM MediaFiles WHERE [Path] = @pathProfiles + " +
                        "'$residentId\audio\music\' AND [FileType] = 'mp3'`r`n" +
                        "INSERT INTO ResidentMediaFiles (IsLinked, ResidentId, ResponseTypeId, MediaPathTypeId, StreamId)`r`n" +
                        "SELECT 0, $residentId, 5, 2, StreamId FROM MediaFiles WHERE [Path] = @pathProfiles + " +
                        "'$residentId\audio\radio-shows\' AND [FileType] = 'mp3'" +

                        "`r`n`r`n--- Activity 5 - ResponseType 'Television' ---`n" +
                        "INSERT INTO ResidentMediaFiles (IsLinked, ResidentId, ResponseTypeId, MediaPathTypeId, StreamId)`r`n" +
                        "SELECT 0, $residentId, 6, 5, StreamId FROM MediaFiles WHERE [Path] = @pathProfiles + " +
                        "'$residentId\videos\tv-shows\' AND [FileType] = 'mp4'`r`n" +
                        "INSERT INTO ResidentMediaFiles (IsLinked, ResidentId, ResponseTypeId, MediaPathTypeId, StreamId)`r`n" +
                        "SELECT 0, $residentId, 6, 6, StreamId FROM MediaFiles WHERE [Path] = @pathProfiles + " +
                        "'$residentId\videos\home-movies\' AND [FileType] = 'mp4'`r`n" +
                    "END"

                    #Write-Host `n$sql
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