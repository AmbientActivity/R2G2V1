@ECHO OFF

cd C:\Windows\Microsoft.NET\Framework\v4.0.30319

echo Uninstalling State Machine Service...
echo ---------------------------------------------------
\debug
echo Uninstalling RFID Reader Service...
echo ---------------------------------------------------
installutil /u C:\Users\%USERNAME%\Source\Repos\R2G2V1\Service\Keebee.AAT.RFIDReaderService\bin\Debug\Keebee.AAT.RFIDReaderService.exe
echo ---------------------------------------------------
echo Done.
echo Uninstalling Phidget Service...
echo ---------------------------------------------------
installutil /u C:\Users\%USERNAME%\Source\Repos\R2G2V1\Service\Keebee.AAT.PhidgetService\bin\Debug\Keebee.AAT.PhidgetService.exe
echo ---------------------------------------------------
echo Done.
installutil /u C:\Users\%USERNAME%\Source\Repos\R2G2V1\Service\Keebee.AAT.StateMachineService\bin\Debug\Keebee.AAT.StateMachineService.exe
echo ---------------------------------------------------
echo Done.
pause
