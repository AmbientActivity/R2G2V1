Write-Host "Copying Flash builds...” -NoNewline

$pathSwf = "C:\Users\$env:USERNAME\Source\Repos\R2G2V1\Flash\Builds\*"
$pathDisplayBin = "C:\Users\$env:USERNAME\Source\Repos\R2G2V1\UI\Keebee.AAT.Display\bin"

Copy-Item $pathSwf "$pathDisplayBin\Debug\" -recurse -Force
Copy-Item $pathSwf "$pathDisplayBin\Release\" -recurse -Force

Write-Host "done.”