@ECHO OFF

cd C:\Windows\Microsoft.NET\Framework\v4.0.30319

echo Installing Keep IIS Alive Service...
echo ---------------------------------------------------
\debug
installutil C:\Users\%USERNAME%\Source\Repos\R2G2V1\Service\Keebee.AAT.KeepIISAliveService\bin\Debug\Keebee.AAT.KeepIISAliveService.exe
echo ---------------------------------------------------
echo Done.
pause