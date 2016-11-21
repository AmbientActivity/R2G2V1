# root
$deployPath = "C:\Deployments\"

# media
$profilesPath = "Media\Profiles\new\"

$activitiesGameShapesPath = "activities\matching-game\shapes\"
$activitiesGameSoundsPath = "activities\matching-game\sounds\"
$audioMusicPath = "audio\music\"
$audioRadioShowsPath = "audio\radio-shows\"
$imagesGeneralPath = "images\general\"
$imagesPersonalPath = "images\personal\"
$videosSystemPath = "videos\system\"
$videosTVShowsPath = "videos\tv-shows\"
$videosHomeMoviesPath = "videos\home-movies\"

Try
{
    # -------------------- MEDIA FOLDER STRUCTURE --------------------

    Write-Host "Deploying Media Folders...” -NoNewline

    $destPath = $deployPath + $profilesPath


    If(test-path $destPath)
    {
        Remove-Item $destPath -recurse -Force
    }

    New-Item -ItemType Directory -Force -Path $destPath | Out-Null

    $path = $destPath + $activitiesGameShapesPath
    New-Item -ItemType Directory -Force -Path $path | Out-Null

    $path = $destPath + $activitiesGameSoundsPath
    New-Item -ItemType Directory -Force -Path $path | Out-Null

    $path = $destPath + $audioMusicPath
    New-Item -ItemType Directory -Force -Path $path | Out-Null

    $path = $destPath + $audioRadioShowsPath
    New-Item -ItemType Directory -Force -Path $path | Out-Null

    $path = $destPath + $imagesGeneralPath
    New-Item -ItemType Directory -Force -Path $path | Out-Null

    $path = $destPath + $imagesPersonalPath
    New-Item -ItemType Directory -Force -Path $path | Out-Null

    $path = $destPath + $videosTVShowsPath
    New-Item -ItemType Directory -Force -Path $path | Out-Null

    $path = $destPath + $videosHomeMoviesPath
    New-Item -ItemType Directory -Force -Path $path | Out-Null

    Write-Host "done.”
}
Catch
{
    Write-Host -ForegroundColor red $_.Exception.Message
}