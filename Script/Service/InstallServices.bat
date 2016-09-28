@ECHO OFF

cd C:\Windows\Microsoft.NET\Framework\v4.0.30319

echo Installing State Machine Service...
echo ---------------------------------------------------
\debug
installutil C:\Users\%USERNAME%\Source\Repos\R2G2V1\Service\Keebee.AAT.StateMachineService\bin\Debug\Keebee.AAT.StateMachineService.exe
echo ---------------------------------------------------
echo Done.
echo Installing Phidget Service...
echo ---------------------------------------------------
installutil C:\Users\%USERNAME%\Source\Repos\R2G2V1\Service\Keebee.AAT.PhidgetService\bin\Debug\Keebee.AAT.PhidgetService.exe
echo ---------------------------------------------------
echo Done.
echo Installing Rfid Reader Service...
echo ---------------------------------------------------
installutil C:\Users\%USERNAME%\Source\Repos\R2G2V1\Service\Keebee.AAT.RfidReaderService\bin\Debug\Keebee.AAT.RfidReaderService.exe
echo ---------------------------------------------------
echo Done.
pause
