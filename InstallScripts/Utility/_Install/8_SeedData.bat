@ECHO OFF

PowerShell -NoProfile -ExecutionPolicy Unrestricted -Command "& 'C:\Deployments\Install\Database\PowerShell\SeedData.ps1'"
echo.

PAUSE