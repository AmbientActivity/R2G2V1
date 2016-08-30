@ECHO OFF

cd C:\Windows\Microsoft.NET\Framework\v4.0.30319

echo Installing State Machine Service...
echo ---------------------------------------------------
\debug
installutil C:\Users\%USERNAME%\Source\Repos\keebee\Service\Keebee.AAT.StateMachineService\bin\Debug\Keebee.AAT.StateMachineService.exe
echo ---------------------------------------------------
echo Done.
echo Installing Phidget Service...
echo ---------------------------------------------------
installutil C:\Users\%USERNAME%\Source\Repos\keebee\Service\Keebee.AAT.PhidgetService\bin\Debug\Keebee.AAT.PhidgetService.exe
echo ---------------------------------------------------
echo Done.
echo Installing RFID Reader Service...
echo ---------------------------------------------------
installutil C:\Users\%USERNAME%\Source\Repos\keebee\Service\Keebee.AAT.RFIDReaderService\bin\Debug\Keebee.AAT.RFIDReaderService.exe
echo ---------------------------------------------------
echo Done.
pause
