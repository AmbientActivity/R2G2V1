@ECHO OFF

PowerShell -NoProfile -ExecutionPolicy Unrestricted -Command "& 'C:\Deployments\Install\Database\PowerShell\DropAndCreateTables.ps1'"
echo.

pause