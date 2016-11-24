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


    # resident 1
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


    # resident 2
    # general images
    $sourcePath = $sourceProfilePath + "2\images\*"
    $destPath = $destProfilePath + "2\images\general\"
    Copy-Item $sourcePath $destPath -recurse -Force

    # general images
    $sourcePath = $sourceProfilePath + "2\pictures\*"
    $destPath = $destProfilePath + "2\images\personal\"
    Copy-Item $sourcePath $destPath -recurse -Force

    # music
    $sourcePath = $sourceProfilePath + "2\music\*"
    $destPath = $destProfilePath + "2\audio\music\"
    Copy-Item $sourcePath $destPath -recurse -Force

    # videos
    $sourcePath = $sourceProfilePath + "2\videos\*"
    $destPath = $destProfilePath + "2\videos\tv-shows\"
    Copy-Item $sourcePath $destPath -recurse -Force

    # matching game shapes
    $sourcePath = $sourceProfilePath + "2\shapes\*"
    $destPath = $destProfilePath + "2\activities\matching-game\shapes\"
    Copy-Item $sourcePath $destPath -recurse -Force

    # matching game sounds
    $sourcePath = $sourceProfilePath + "2\sounds\*"
    $destPath = $destProfilePath + "2\activities\matching-game\sounds\"
    Copy-Item $sourcePath $destPath -recurse -Force


    # resident 3
    # general images
    $sourcePath = $sourceProfilePath + "3\images\*"
    $destPath = $destProfilePath + "3\images\general\"
    Copy-Item $sourcePath $destPath -recurse -Force

    # general images
    $sourcePath = $sourceProfilePath + "3\pictures\*"
    $destPath = $destProfilePath + "3\images\personal\"
    Copy-Item $sourcePath $destPath -recurse -Force

    # music
    $sourcePath = $sourceProfilePath + "3\music\*"
    $destPath = $destProfilePath + "3\audio\music\"
    Copy-Item $sourcePath $destPath -recurse -Force

    # videos
    $sourcePath = $sourceProfilePath + "3\videos\*"
    $destPath = $destProfilePath + "3\videos\tv-shows\"
    Copy-Item $sourcePath $destPath -recurse -Force

    # matching game shapes
    $sourcePath = $sourceProfilePath + "3\shapes\*"
    $destPath = $destProfilePath + "3\activities\matching-game\shapes\"
    Copy-Item $sourcePath $destPath -recurse -Force

    # matching game sounds
    $sourcePath = $sourceProfilePath + "3\sounds\*"
    $destPath = $destProfilePath + "3\activities\matching-game\sounds\"
    Copy-Item $sourcePath $destPath -recurse -Force


    # resident 4
    # general images
    $sourcePath = $sourceProfilePath + "4\images\*"
    $destPath = $destProfilePath + "4\images\general\"
    Copy-Item $sourcePath $destPath -recurse -Force

    # general images
    $sourcePath = $sourceProfilePath + "4\pictures\*"
    $destPath = $destProfilePath + "4\images\personal\"
    Copy-Item $sourcePath $destPath -recurse -Force

    # music
    $sourcePath = $sourceProfilePath + "4\music\*"
    $destPath = $destProfilePath + "4\audio\music\"
    Copy-Item $sourcePath $destPath -recurse -Force

    # videos
    $sourcePath = $sourceProfilePath + "4\videos\*"
    $destPath = $destProfilePath + "4\videos\tv-shows\"
    Copy-Item $sourcePath $destPath -recurse -Force

    # matching game shapes
    $sourcePath = $sourceProfilePath + "4\shapes\*"
    $destPath = $destProfilePath + "4\activities\matching-game\shapes\"
    Copy-Item $sourcePath $destPath -recurse -Force

    # matching game sounds
    $sourcePath = $sourceProfilePath + "4\sounds\*"
    $destPath = $destProfilePath + "4\activities\matching-game\sounds\"
    Copy-Item $sourcePath $destPath -recurse -Force


    # resident 5
    # general images
    $sourcePath = $sourceProfilePath + "5\images\*"
    $destPath = $destProfilePath + "5\images\general\"
    Copy-Item $sourcePath $destPath -recurse -Force

    # general images
    $sourcePath = $sourceProfilePath + "5\pictures\*"
    $destPath = $destProfilePath + "5\images\personal\"
    Copy-Item $sourcePath $destPath -recurse -Force

    # music
    $sourcePath = $sourceProfilePath + "5\music\*"
    $destPath = $destProfilePath + "5\audio\music\"
    Copy-Item $sourcePath $destPath -recurse -Force

    # videos
    $sourcePath = $sourceProfilePath + "5\videos\*"
    $destPath = $destProfilePath + "5\videos\tv-shows\"
    Copy-Item $sourcePath $destPath -recurse -Force

    # matching game shapes
    $sourcePath = $sourceProfilePath + "5\shapes\*"
    $destPath = $destProfilePath + "5\activities\matching-game\shapes\"
    Copy-Item $sourcePath $destPath -recurse -Force

    # matching game sounds
    $sourcePath = $sourceProfilePath + "5\sounds\*"
    $destPath = $destProfilePath + "5\activities\matching-game\sounds\"
    Copy-Item $sourcePath $destPath -recurse -Force


    # resident 6
    # general images
    $sourcePath = $sourceProfilePath + "6\images\*"
    $destPath = $destProfilePath + "6\images\general\"
    Copy-Item $sourcePath $destPath -recurse -Force

    # general images
    $sourcePath = $sourceProfilePath + "6\pictures\*"
    $destPath = $destProfilePath + "6\images\personal\"
    Copy-Item $sourcePath $destPath -recurse -Force

    # music
    $sourcePath = $sourceProfilePath + "6\music\*"
    $destPath = $destProfilePath + "6\audio\music\"
    Copy-Item $sourcePath $destPath -recurse -Force

    # videos
    $sourcePath = $sourceProfilePath + "6\videos\*"
    $destPath = $destProfilePath + "6\videos\tv-shows\"
    Copy-Item $sourcePath $destPath -recurse -Force

    # matching game shapes
    $sourcePath = $sourceProfilePath + "6\shapes\*"
    $destPath = $destProfilePath + "6\activities\matching-game\shapes\"
    Copy-Item $sourcePath $destPath -recurse -Force

    # matching game sounds
    $sourcePath = $sourceProfilePath + "6\sounds\*"
    $destPath = $destProfilePath + "6\activities\matching-game\sounds\"
    Copy-Item $sourcePath $destPath -recurse -Force

    Write-Host "done.”
}
Catch
{
    Write-Host -ForegroundColor red $_.Exception.Message
}