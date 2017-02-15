@ECHO OFF

PowerShell -NoProfile -ExecutionPolicy Unrestricted -Command "& 'C:\Deployments\Install\Database\PowerShell\CreatePreloadedResidents.ps1'"
echo.

PAUSE