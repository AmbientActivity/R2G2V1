Write-Host "Copying Flash builds...” -NoNewline
$pathSwf = "C:\Users\$env:USERNAME\Source\Repos\R2G2V1\Flash\Builds\*"
$pathDisplayDebug = "C:\Users\$env:USERNAME\Source\Repos\R2G2V1\UI\Keebee.AAT.Display\bin\Debug\"
Copy-Item $pathSwf $pathDisplayDebug -recurse -Force

$pathDisplayRelease = "C:\Users\$env:USERNAME\Source\Repos\R2G2V1\UI\Keebee.AAT.Display\bin\Release\"
Copy-Item $pathSwf $pathDisplayRelease -recurse -Force
Write-Host "done.”