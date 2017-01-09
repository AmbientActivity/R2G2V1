@ECHO OFF

cd C:\Windows\Microsoft.NET\Framework\v4.0.30319

echo ---------------------------------------------------
echo Installing State Machine Service...
echo ---------------------------------------------------
installutil C:\Deployments\Services\StateMachineService\1.0.0.0\Keebee.AAT.StateMachineService.exe
echo Done.

echo ---------------------------------------------------
echo Installing Phidget Service...
echo ---------------------------------------------------
installutil C:\Deployments\Services\PhidgetService\1.0.0.0\Keebee.AAT.PhidgetService.exe
echo Done.

echo ---------------------------------------------------
echo Installing Video Capture Service...
echo ---------------------------------------------------
installutil C:\Deployments\Services\VideoCaptureService\1.0.0.0\Keebee.AAT.VideoCaptureService.exe
echo Done.

echo ---------------------------------------------------
echo Installing Rfid Reader Service...
echo ---------------------------------------------------
installutil C:\Deployments\Services\RfidReaderService\1.0.0.0\Keebee.AAT.RfidReaderService.exe
echo Done.

echo ---------------------------------------------------
echo Installing Beacon Reader Service...
echo ---------------------------------------------------
installutil C:\Deployments\Services\BeaconReaderService\1.0.0.0\Keebee.AAT.BeaconReaderService.exe
echo Done.

echo ---------------------------------------------------
echo Installing Keep IIS Alive Service...
echo ---------------------------------------------------
installutil C:\Deployments\Services\KeepIISAliveService\1.0.0.0\Keebee.AAT.KeepIISAliveService.exe
echo Done.

pause
