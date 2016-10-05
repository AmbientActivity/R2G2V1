@ECHO OFF

cd C:\Windows\Microsoft.NET\Framework\v4.0.30319

echo Uninstalling RFID Reader Service...
echo ---------------------------------------------------
installutil /u C:\Users\%USERNAME%\Source\Repos\R2G2V1\Service\Keebee.AAT.RfidReaderService\bin\Debug\Keebee.AAT.RfidReaderService.exe
echo ---------------------------------------------------
echo Done.

echo Uninstalling Phidget Service...
echo ---------------------------------------------------
installutil /u C:\Users\%USERNAME%\Source\Repos\R2G2V1\Service\Keebee.AAT.PhidgetService\bin\Debug\Keebee.AAT.PhidgetService.exe
echo ---------------------------------------------------
echo Done.

echo Uninstalling Video Capture Service...
echo ---------------------------------------------------
installutil /u C:\Users\%USERNAME%\Source\Repos\R2G2V1\Service\Keebee.AAT.VideoCaptureService\bin\Debug\Keebee.AAT.VideoCaptureService.exe
echo ---------------------------------------------------
echo Done.

echo Uninstalling Keep IIS Alive Service...
echo ---------------------------------------------------
installutil /u C:\Users\%USERNAME%\Source\Repos\R2G2V1\Service\Keebee.AAT.KeepIISAliveService\bin\Debug\Keebee.AAT.KeepIISAliveService.exe
echo ---------------------------------------------------
echo Done.

echo Uninstalling State Machine Service...
echo ---------------------------------------------------
installutil /u C:\Users\%USERNAME%\Source\Repos\R2G2V1\Service\Keebee.AAT.StateMachineService\bin\Debug\Keebee.AAT.StateMachineService.exe
echo ---------------------------------------------------
echo Done.

pause