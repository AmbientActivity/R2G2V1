@ECHO OFF

PowerShell -NoProfile -ExecutionPolicy Unrestricted -Command "& 'C:\Deployments\Install\Utility\EventLog\Database\PurgeEventLogs.ps1'
echo.

PAUSE