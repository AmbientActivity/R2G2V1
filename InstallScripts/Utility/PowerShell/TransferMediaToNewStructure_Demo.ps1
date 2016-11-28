# export path
$sourceExportPath = "\\" + $env:COMPUTERNAME + "\SQLEXPRESS\KeebeeAATFilestream\Media\Exports\*"
$destExportPath = "C:\Deployments\Media\Exports\"

# media paths
$sourceProfilePath = "\\" + $env:COMPUTERNAME + "\SQLEXPRESS\KeebeeAATFilestream\Media\Profiles\"
$destProfilePath = "C:\Deployments\Media\Profiles\"

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
    # -------------------- TRANSFER MEDIA --------------------

    Write-Host "Transferring Media to New Folder Structure...” -NoNewline

    # exports
    Copy-Item $sourceExportPath $destExportPath -recurse -Force

    # public profile
    # images
    $sourcePath = $sourceProfilePath + "0\images\*"
    $destPath = $destProfilePath + "0\images\general\"
    Copy-Item $sourcePath $destPath -recurse -Force

    # music
    $sourcePath = $sourceProfilePath + "0\music\*"
    $destPath = $destProfilePath + "0\audio\music\"
    Copy-Item $sourcePath $destPath -recurse -Force

    # videos
    $sourcePath = $sourceProfilePath + "0\videos\*"
    $destPath = $destProfilePath + "0\videos\system\"
    Copy-Item $sourcePath $destPath -recurse -Force

    # matching game shapes
    $sourcePath = $sourceProfilePath + "0\shapes\*"
    $destPath = $destProfilePath + "0\activities\matching-game\shapes\"
    Copy-Item $sourcePath $destPath -recurse -Force

    # matching game sounds
    $sourcePath = $sourceProfilePath + "0\sounds\*"
    $destPath = $destProfilePath + "0\activities\matching-game\sounds\"
    Copy-Item $sourcePath $destPath -recurse -Force


    # demo resident
    # general images
    $sourcePath = $sourceProfilePath + "1\images\*"
    $destPath = $destProfilePath + "1\images\general\"
    Copy-Item $sourcePath $destPath -recurse -Force

    # general images
    $sourcePath = $sourceProfilePath + "1\pictures\*"
    $destPath = $destProfilePath + "1\images\personal\"
    Copy-Item $sourcePath $destPath -recurse -Force

    # music
    $sourcePath = $sourceProfilePath + "1\music\*"
    $destPath = $destProfilePath + "1\audio\music\"
    Copy-Item $sourcePath $destPath -recurse -Force

    # videos
    $sourcePath = $sourceProfilePath + "1\videos\*"
    $destPath = $destProfilePath + "1\videos\tv-shows\"
    Copy-Item $sourcePath $destPath -recurse -Force

    # matching game shapes
    $sourcePath = $sourceProfilePath + "1\shapes\*"
    $destPath = $destProfilePath + "1\activities\matching-game\shapes\"
    Copy-Item $sourcePath $destPath -recurse -Force

    # matching game sounds
    $sourcePath = $sourceProfilePath + "1\sounds\*"
    $destPath = $destProfilePath + "1\activities\matching-game\sounds\"
    Copy-Item $sourcePath $destPath -recurse -Force

    Write-Host "done.”
}
Catch
{
    Write-Host -ForegroundColor red $_.Exception.Message
}