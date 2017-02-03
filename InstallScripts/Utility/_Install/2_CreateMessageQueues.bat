@ECHO OFF

PowerShell -NoProfile -ExecutionPolicy Unrestricted -Command "& 'C:\Deployments\Install\PowerShell\CreateMessageQueues.ps1'"
echo.

PAUSE