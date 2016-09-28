@ECHO OFF

cd C:\Windows\Microsoft.NET\Framework\v4.0.30319

echo Uninstalling Keep IIS Alive Service...
echo ---------------------------------------------------
\debug
echo Uninstalling RFID Reader Service...
echo ---------------------------------------------------
installutil /u C:\%USERNAME%\John\Source\Repos\R2G2V1\Service\Keebee.AAT.KeepIISAliveService\bin\Debug\Keebee.AAT.KeepIISAliveService.exe
echo ---------------------------------------------------
echo Done.
pause
