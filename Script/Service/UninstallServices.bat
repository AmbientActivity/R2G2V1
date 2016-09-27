@ECHO OFF

cd C:\Windows\Microsoft.NET\Framework\v4.0.30319

echo Uninstalling State Machine Service...
echo ---------------------------------------------------
\debug
echo Uninstalling RFID Reader Service...
echo ---------------------------------------------------
installutil /u C:\Deployments\Services\RFIDReaderService\1.0.0.0\Keebee.AAT.RfidReaderService.exe
echo ---------------------------------------------------
echo Done.
echo Uninstalling Phidget Service...
echo ---------------------------------------------------
installutil /u C:\Deployments\Services\PhidgetService\1.0.0.0\Keebee.AAT.PhidgetService.exe
echo ---------------------------------------------------
echo Done.
installutil /u C:\Deployments\Services\StateMachineService\1.0.0.0\Keebee.AAT.StateMachineService.exe
echo ---------------------------------------------------
echo Done.
pause