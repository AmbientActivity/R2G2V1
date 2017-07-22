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

REM echo.
REM echo ---------------------------------------------------
REM echo Installing Video Capture Service...
REM echo ---------------------------------------------------
REM installutil C:\Users\%USERNAME%\Source\Repos\R2G2V1\Service\Keebee.AAT.VideoCaptureService\bin\Debug\Keebee.AAT.VideoCaptureService.exe
REM echo Done.

REM echo.
REM echo ---------------------------------------------------
REM echo Installing Bluetooth Beacon Watcher Service...
REM echo ---------------------------------------------------
REM installutil C:\Users\%USERNAME%\Source\Repos\R2G2V1\Service\Keebee.AAT.BluetoothBeaconWatcherService\bin\Debug\Keebee.AAT.BluetoothBeaconWatcherService.exe
REM echo Done.

echo.
echo ---------------------------------------------------
echo Installing Keep IIS Alive Service...
echo ---------------------------------------------------
installutil C:\Users\%USERNAME%\Source\Repos\R2G2V1\Service\Keebee.AAT.KeepIISAliveService\bin\Debug\Keebee.AAT.KeepIISAliveService.exe
echo Done.

pause
