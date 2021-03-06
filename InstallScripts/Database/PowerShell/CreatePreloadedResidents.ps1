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
                    # get rid of Thumbs.db
                    get-childitem $pathSqlProfiles\$residentId -include *.db -recurse | foreach ($_) {remove-item $_.fullname}
                    Write-Host "done."

                    # create sql statement
                    $sql = "DECLARE @pathProfiles varchar(100)`r`n" +
                           "SET @pathProfiles = FileTableRootPath() + '\$pathProfiles\'" + "`r`n" +
                           "DECLARE @mediaPathType varchar(100)" + "`r`n" +
                           "DECLARE @allowedExts varchar(100)"

                    Write-Host "Creating Resident $residentId..."-NoNewline
                    $sql += "`r`n`r`n--- ResidentId $residentId ---`r`n`r`n" +

                    "IF NOT EXISTS (SELECT * FROM Residents WHERE Id = $residentId)`r`n" +
                    "BEGIN`r`n" +
                        "SET IDENTITY_INSERT [dbo].[Residents] ON`r`n" +
                        "INSERT [dbo].[Residents] ([Id], [FirstName], [LastName], [Gender], [GameDifficultyLevel], [AllowVideoCapturing], [DateCreated], [DateUpdated], [ProfilePicture]) " +
                        "VALUES($residentId, 'Resident $residentId', null, 'F', 1, 1, GetDate(), GetDate(), CONVERT(VARBINARY(max), '0x', 1))`r`n" +
                        "SET IDENTITY_INSERT [dbo].[Residents] OFF`r`n" +
            
                        "`r`n--- Activity 1 - ResponseType 'SlideShow' ---`r`n" +
                        # images/general
                        "SELECT @mediaPathType = [Path], @allowedExts = '''' + REPLACE(AllowedExts, ', ', ''', ''') + '''' FROM MediaPathTypes WHERE Id = 3 `r`n" +
                        "INSERT INTO ResidentMediaFiles (IsLinked, ResidentId, ResponseTypeId, MediaPathTypeId, StreamId, DateAdded)`r`n" +
                        "SELECT 0, $residentId, 1, 3, StreamId, GETDATE() FROM MediaFiles WHERE [Path] = @pathProfiles + " +
                        "'$residentId\images\general\' AND (@allowedExts) LIKE '%' + [FileType] + '%'`r`n" +

                        # images/personal
                        "INSERT INTO ResidentMediaFiles (IsLinked, ResidentId, ResponseTypeId, MediaPathTypeId, StreamId, DateAdded)`r`n" +
                        "SELECT 0, $residentId, 1, 4, StreamId, GETDATE() FROM MediaFiles WHERE [Path] = @pathProfiles + " +
                        "'$residentId\images\personal\' AND (@allowedExts) LIKE '%' + [FileType] + '%'`r`n" +

                        "`r`n--- Activity 2 - ResponseType 'MatchingGame' ---`r`n" +
                        # activities\matching-game\shapes\
                        "SELECT @mediaPathType = [Path], @allowedExts = '''' + REPLACE(AllowedExts, ', ', ''', ''') + '''' FROM MediaPathTypes WHERE Id = 7 `r`n" +
                        "INSERT INTO ResidentMediaFiles (IsLinked, ResidentId, ResponseTypeId, MediaPathTypeId, StreamId, DateAdded)`r`n" + 
                        "SELECT 0, $residentId, 2, 7, StreamId, GETDATE() FROM MediaFiles WHERE [Path] = @pathProfiles + " +
                        "'$residentId\activities\matching-game\shapes\' AND (@allowedExts) LIKE '%' + [FileType] + '%'`r`n" +

                        # activities\matching-game\sounds\
                        "SELECT @mediaPathType = [Path], @allowedExts = '''' + REPLACE(AllowedExts, ', ', ''', ''') + '''' FROM MediaPathTypes WHERE Id = 8 `r`n" +
                        "INSERT INTO ResidentMediaFiles (IsLinked, ResidentId, ResponseTypeId, MediaPathTypeId, StreamId, DateAdded)`r`n" +
                        "SELECT 0, $residentId, 2, 8, StreamId, GETDATE() FROM MediaFiles WHERE [Path] = @pathProfiles + " +
                        "'$residentId\activities\matching-game\sounds\' AND (@allowedExts) LIKE '%' + [FileType] + '%'`r`n" +

                        "`r`n--- Activity 5 - ResponseType 'Radio' ---`r`n" +
                        # audio\music\
                        "SELECT @mediaPathType = [Path], @allowedExts = '''' + REPLACE(AllowedExts, ', ', ''', ''') + '''' FROM MediaPathTypes WHERE Id = 1 `r`n" +
                        "INSERT INTO ResidentMediaFiles (IsLinked, ResidentId, ResponseTypeId, MediaPathTypeId, StreamId, DateAdded)`r`n" + 
                        "SELECT 0, $residentId, 5, 1, StreamId, GETDATE() FROM MediaFiles WHERE [Path] = @pathProfiles + " +
                        "'$residentId\audio\music\' AND (@allowedExts) LIKE '%' + [FileType] + '%'`r`n" +

                        # audio\radio-shows\
                        "SELECT @mediaPathType = [Path], @allowedExts = '''' + REPLACE(AllowedExts, ', ', ''', ''') + '''' FROM MediaPathTypes WHERE Id = 2 `r`n" +
                        "INSERT INTO ResidentMediaFiles (IsLinked, ResidentId, ResponseTypeId, MediaPathTypeId, StreamId, DateAdded)`r`n" +
                        "SELECT 0, $residentId, 5, 2, StreamId, GETDATE() FROM MediaFiles WHERE [Path] = @pathProfiles + " +
                        "'$residentId\audio\radio-shows\' AND (@allowedExts) LIKE '%' + [FileType] + '%'`r`n" +

                        "`r`n--- Activity 6 - ResponseType 'Television' ---`n" +
                        # videos\tv-shows\
                        "SELECT @mediaPathType = [Path], @allowedExts = '''' + REPLACE(AllowedExts, ', ', ''', ''') + '''' FROM MediaPathTypes WHERE Id = 5 `r`n" +
                        "INSERT INTO ResidentMediaFiles (IsLinked, ResidentId, ResponseTypeId, MediaPathTypeId, StreamId, DateAdded)`r`n" +
                        "SELECT 0, $residentId, 6, 5, StreamId, GETDATE() FROM MediaFiles WHERE [Path] = @pathProfiles + " +
                        "'$residentId\videos\tv-shows\' AND (@allowedExts) LIKE '%' + [FileType] + '%'`r`n" +

                        # videos\home-movies\
                        "SELECT @mediaPathType = [Path], @allowedExts = '''' + REPLACE(AllowedExts, ', ', ''', ''') + '''' FROM MediaPathTypes WHERE Id = 6 `r`n" +
                        "INSERT INTO ResidentMediaFiles (IsLinked, ResidentId, ResponseTypeId, MediaPathTypeId, StreamId, DateAdded)`r`n" +
                        "SELECT 0, $residentId, 6, 6, StreamId, GETDATE() FROM MediaFiles WHERE [Path] = @pathProfiles + " +
                        "'$residentId\videos\home-movies\' AND (@allowedExts) LIKE '%' + [FileType] + '%'`r`n" +
                    "END"

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