@ECHO OFF

cd C:\Windows\Microsoft.NET\Framework\v4.0.30319

echo ---------------------------------------------------
echo Uninstalling Keep IIS Alive Service...
echo ---------------------------------------------------
installutil /u C:\Deployments\Services\KeepIISAliveService\1.0.0.0\Keebee.AAT.KeepIISAliveService.exe
echo Done.

echo ---------------------------------------------------
echo Uninstalling RFID Reader Service...
echo ---------------------------------------------------
installutil /u C:\Deployments\Services\RfidReaderService\1.0.0.0\Keebee.AAT.RfidReaderService.exe
echo Done.

echo ---------------------------------------------------
echo Uninstalling Beacon Reader Service...
echo ---------------------------------------------------
installutil /u C:\Deployments\Services\BeaconReaderService\1.0.0.0\Keebee.AAT.BeaconReaderService.exe
echo Done.

echo ---------------------------------------------------
echo Uninstalling Phidget Service...
echo ---------------------------------------------------
installutil /u C:\Deployments\Services\PhidgetService\1.0.0.0\Keebee.AAT.PhidgetService.exe
echo Done.

echo ---------------------------------------------------
echo Uninstalling Video Capture Service...
echo ---------------------------------------------------
installutil /u C:\Deployments\Services\VideoCaptureService\1.0.0.0\Keebee.AAT.VideoCaptureService.exe
echo Done.

echo ---------------------------------------------------
echo Uninstalling State Machine Service...
echo ---------------------------------------------------
installutil /u C:\Deployments\Services\StateMachineService\1.0.0.0\Keebee.AAT.StateMachineService.exe
echo Done.

pause