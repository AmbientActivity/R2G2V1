@ECHO OFF

cd C:\Windows\Microsoft.NET\Framework\v4.0.30319

echo Installing Video Capture Service...
echo ---------------------------------------------------
\debug
installutil C:\Users\%USERNAME%\Source\Repos\R2G2V1\Service\Keebee.AAT.VideoCaptureService\bin\Debug\Keebee.AAT.VideoCaptureService.exe
echo ---------------------------------------------------
echo Done.
pause