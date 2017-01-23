@ECHO OFF

PowerShell -NoProfile -ExecutionPolicy Unrestricted -Command "& 'C:\Deployments\Install\Utility\PowerShell\CreateMessageQueuesDev.ps1'"
echo.

PAUSE