@ECHO OFF

cd C:\Windows\Microsoft.NET\Framework\v4.0.30319

echo Installing State Machine Service...
echo ---------------------------------------------------
installutil C:\Deployments\Services\StateMachineService\1.0.0.0\Keebee.AAT.StateMachineService.exe
echo ---------------------------------------------------
echo Done.

echo Installing Phidget Service...
echo ---------------------------------------------------
installutil C:\Deployments\Services\PhidgetService\1.0.0.0\Keebee.AAT.PhidgetService.exe
echo ---------------------------------------------------
echo Done.

echo Installing Video Capture Service...
echo ---------------------------------------------------
installutil C:\Deployments\Services\VideoCaptureService\1.0.0.0\Keebee.AAT.VideoCaptureService.exe
echo ---------------------------------------------------
echo Done.

echo Installing Rfid Reader Service...
echo ---------------------------------------------------
installutil C:\Deployments\Services\RfidReaderService\1.0.0.0\Keebee.AAT.RfidReaderService.exe
echo ---------------------------------------------------
echo Done.

echo Installing Keep IIS Alive Service...
echo ---------------------------------------------------
installutil C:\Deployments\Services\KeepIISAliveService\1.0.0.0\Keebee.AAT.KeepIISAliveService.exe
echo ---------------------------------------------------
echo Done.

pause
