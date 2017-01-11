@ECHO OFF

cd C:\Windows\Microsoft.NET\Framework\v4.0.30319

echo ---------------------------------------------------
echo Installing State Machine Service...
echo ---------------------------------------------------
installutil C:\Deployments\Services\StateMachineService\1.0.0.0\Keebee.AAT.StateMachineService.exe
echo Done.

echo.
echo ---------------------------------------------------
echo Installing Phidget Service...
echo ---------------------------------------------------
installutil C:\Deployments\Services\PhidgetService\1.0.0.0\Keebee.AAT.PhidgetService.exe
echo Done.

echo.
echo ---------------------------------------------------
echo Installing Video Capture Service...
echo ---------------------------------------------------
installutil C:\Deployments\Services\VideoCaptureService\1.0.0.0\Keebee.AAT.VideoCaptureService.exe
echo Done.

rem echo.
rem echo ---------------------------------------------------
rem echo Installing Rfid Reader Service...
rem echo ---------------------------------------------------
rem installutil C:\Deployments\Services\RfidReaderService\1.0.0.0\Keebee.AAT.RfidReaderService.exe
rem echo Done.

echo.
echo ---------------------------------------------------
echo Installing Bluetooth Beacon Watcher Service...
echo ---------------------------------------------------
installutil C:\Deployments\Services\BluetoothBeaconWatcherService\1.0.0.0\Keebee.AAT.BluetoothBeaconWatcherService.exe
echo Done.

echo.
echo ---------------------------------------------------
echo Installing Keep IIS Alive Service...
echo ---------------------------------------------------
installutil C:\Deployments\Services\KeepIISAliveService\1.0.0.0\Keebee.AAT.KeepIISAliveService.exe
echo Done.

pause
