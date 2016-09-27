@ECHO OFF

cd C:\Windows\Microsoft.NET\Framework\v4.0.30319

echo Uninstalling State Machine Service...
echo ---------------------------------------------------
\debug
echo Uninstalling RFID Reader Service...
echo ---------------------------------------------------
installutil /u C:\Deployments\Services\KeepIISAliveService\1.0.0.0\Keebee.AAT.KeepIISAliveService.exe
echo ---------------------------------------------------
echo Done.
pause
