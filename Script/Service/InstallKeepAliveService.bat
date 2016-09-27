@ECHO OFF

cd C:\Windows\Microsoft.NET\Framework\v4.0.30319

echo Installing State Machine Service...
echo ---------------------------------------------------
\debug
installutil C:\Deployments\Services\KeepIISAliveService\1.0.0.0\Keebee.AAT.KeepIISAliveService.exe
echo ---------------------------------------------------
echo Done.
pause