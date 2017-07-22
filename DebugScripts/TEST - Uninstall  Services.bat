@ECHO OFF

cd C:\Windows\Microsoft.NET\Framework\v4.0.30319

echo ---------------------------------------------------
echo Uninstalling Keep IIS Alive Service...
installutil /u C:\Users\%USERNAME%\Source\Repos\R2G2V1\Service\Keebee.AAT.KeepIISAliveService\bin\Debug\Keebee.AAT.KeepIISAliveService.exe
echo ---------------------------------------------------
echo Done.

REM echo.
REM echo ---------------------------------------------------
REM echo Installing Bluetooth Beacon Watcher Service...
REM echo ---------------------------------------------------
REM installutil /u C:\Users\%USERNAME%\Source\Repos\R2G2V1\Service\Keebee.AAT.BluetoothBeaconWatcherService\bin\Debug\Keebee.AAT.BluetoothBeaconWatcherService.exe
REM echo Done.

echo ---------------------------------------------------
echo Uninstalling Phidget Service...
installutil /u C:\Users\%USERNAME%\Source\Repos\R2G2V1\Service\Keebee.AAT.PhidgetService\bin\Debug\Keebee.AAT.PhidgetService.exe
echo ---------------------------------------------------
echo Done.

REM echo ---------------------------------------------------
REM echo Uninstalling Video Capture Service...
REM installutil /u C:\Users\%USERNAME%\Source\Repos\R2G2V1\Service\Keebee.AAT.VideoCaptureService\bin\Debug\Keebee.AAT.VideoCaptureService.exe
REM echo ---------------------------------------------------
REM echo Done.

echo ---------------------------------------------------
echo Uninstalling State Machine Service...
installutil /u C:\Users\%USERNAME%\Source\Repos\R2G2V1\Service\Keebee.AAT.StateMachineService\bin\Debug\Keebee.AAT.StateMachineService.exe
echo ---------------------------------------------------
echo Done.

pause