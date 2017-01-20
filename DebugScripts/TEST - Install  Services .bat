@ECHO OFF

cd C:\Windows\Microsoft.NET\Framework\v4.0.30319

echo ---------------------------------------------------
echo Installing State Machine Service...
echo ---------------------------------------------------
installutil C:\Users\%USERNAME%\Source\Repos\R2G2V1\Service\Keebee.AAT.StateMachineService\bin\Debug\Keebee.AAT.StateMachineService.exe
echo Done.

echo.
echo ---------------------------------------------------
echo Installing Phidget Service...
echo ---------------------------------------------------
installutil C:\Users\%USERNAME%\Source\Repos\R2G2V1\Service\Keebee.AAT.PhidgetService\bin\Debug\Keebee.AAT.PhidgetService.exe
echo Done.

echo.
echo ---------------------------------------------------
echo Installing Video Capture Service...
echo ---------------------------------------------------
installutil C:\Users\%USERNAME%\Source\Repos\R2G2V1\Service\Keebee.AAT.VideoCaptureService\bin\Debug\Keebee.AAT.VideoCaptureService.exe
echo Done.

echo.
echo ---------------------------------------------------
echo Installing Bluetooth Beacon Watcher Service...
echo ---------------------------------------------------
installutil C:\Users\%USERNAME%\Source\Repos\R2G2V1\Service\Keebee.AAT.BluetoothBeaconWatcherService\bin\Debug\BluetoothBeaconWatcherService.exe
echo Done.

echo.
echo ---------------------------------------------------
echo Installing Keep IIS Alive Service...
echo ---------------------------------------------------
installutil C:\Users\%USERNAME%\Source\Repos\R2G2V1\Service\Keebee.AAT.KeepIISAliveService\bin\Debug\Keebee.AAT.KeepIISAliveService.exe
echo Done.

pause
