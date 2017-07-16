Write-Host "Installing 3rd party assemblies...” -NoNewline

# NReco Video Converter
$pathNReco = "C:\Users\$env:USERNAME\Source\Repos\R2G2V1\3rdParty\NReco\"
$pathThumbnailBin = "C:\Users\$env:USERNAME\Source\Repos\R2G2V1\Shared\Keebee.AAT.ThumbnailGeneration\bin\"
$pathVideoBin = "C:\Users\$env:USERNAME\Source\Repos\R2G2V1\Shared\Keebee.AAT.VideoConversion\bin\"

If (!(test-path "$pathThumbnailBin\Debug"))
{
    New-Item -ItemType Directory -Force -Path "$pathThumbnailBin\Debug" | Out-Null
}
Copy-Item "$pathNReco\NReco.VideoConverter.dll" "$pathThumbnailBin\Debug" -recurse -Force

If (!(test-path "$pathThumbnailBin\Release"))
{
    New-Item -ItemType Directory -Force -Path "$pathThumbnailBin\Release" | Out-Null
}
Copy-Item "$pathNReco\NReco.VideoConverter.dll" "$pathThumbnailBin\Release" -recurse -Force


If (!(test-path "$pathVideoBin\Debug"))
{
    New-Item -ItemType Directory -Force -Path "$pathVideoBin\Debug" | Out-Null
}
Copy-Item "$pathNReco\NReco.VideoConverter.dll" "$pathVideoBin\Debug" -recurse -Force
Copy-Item "$pathNReco\NReco.VideoConverter.dll" "$pathVideoBin\Release" -recurse -Force

# NReco Video Info
If (!(test-path "$pathVideoBin\Release"))
{
    New-Item -ItemType Directory -Force -Path "$pathVideoBin\Debug" | Out-Null
}
Copy-Item "$pathNReco\NReco.VideoInfo.dll" "$pathVideoBin\Debug" -recurse -Force
Copy-Item "$pathNReco\NReco.VideoInfo.dll" "$pathVideoBin\Release" -recurse -Force

Write-Host "done.”