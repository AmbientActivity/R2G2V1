@ECHO OFF

cd C:\Windows\Microsoft.NET\Framework\v4.0.30319

echo Uninstalling State Machine Service...
echo ---------------------------------------------------
\debug
installutil /u C:\Users\%USERNAME%\Source\Repos\keebee\Service\Keebee.AAT.StateMachineService\bin\Debug\Keebee.AAT.StateMachineService.exe
echo ---------------------------------------------------
echo Done.
echo Uninstalling Phidget Service...
echo ---------------------------------------------------
installutil /u C:\Users\%USERNAME%\Source\Repos\keebee\Service\Keebee.AAT.PhidgetService\bin\Debug\Keebee.AAT.PhidgetService.exe
echo ---------------------------------------------------
echo Done.
echo Uninstalling RFID Reader Service...
echo ---------------------------------------------------
installutil /u C:\Users\%USERNAME%\Source\Repos\keebee\Service\Keebee.AAT.RFIDReaderService\bin\Debug\Keebee.AAT.RFIDReaderService.exe
echo ---------------------------------------------------
echo Done.
pause
