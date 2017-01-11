@ECHO OFF

cd C:\Windows\Microsoft.NET\Framework\v4.0.30319

echo.
echo ---------------------------------------------------
echo Uninstalling Keep IIS Alive Service...
echo ---------------------------------------------------
installutil /u C:\Deployments\Services\KeepIISAliveService\1.0.0.0\Keebee.AAT.KeepIISAliveService.exe
echo Done.

rem echo.
rem echo ---------------------------------------------------
rem echo Uninstalling RFID Reader Service...
rem echo ---------------------------------------------------
rem installutil /u C:\Deployments\Services\RfidReaderService\1.0.0.0\Keebee.AAT.RfidReaderService.exe
rem echo Done.

echo.
echo ---------------------------------------------------
echo Uninstalling Bluetooth Beacon Watcher Service...
echo ---------------------------------------------------
installutil /u C:\Deployments\Services\BluetoothBeaconWatcherService\1.0.0.0\Keebee.AAT.BluetoothBeaconWatcherService.exe
echo Done.

echo.
echo ---------------------------------------------------
echo Uninstalling Phidget Service...
echo ---------------------------------------------------
installutil /u C:\Deployments\Services\PhidgetService\1.0.0.0\Keebee.AAT.PhidgetService.exe
echo Done.

echo.
echo ---------------------------------------------------
echo Uninstalling Video Capture Service...
echo ---------------------------------------------------
installutil /u C:\Deployments\Services\VideoCaptureService\1.0.0.0\Keebee.AAT.VideoCaptureService.exe
echo Done.

echo.
echo ---------------------------------------------------
echo Uninstalling State Machine Service...
echo ---------------------------------------------------
installutil /u C:\Deployments\Services\StateMachineService\1.0.0.0\Keebee.AAT.StateMachineService.exe
echo Done.

pause