@ECHO OFF

echo.
echo ---- Services --
echo.
PowerShell -NoProfile -ExecutionPolicy Unrestricted -Command "& 'C:\Deployments\Install\Utility\PowerShell\UninstallServices.ps1'"

echo.
echo ---- Database ---
echo.
PowerShell -NoProfile -ExecutionPolicy Unrestricted -Command "& 'C:\Deployments\Install\Database\PowerShell\DropDatabase.ps1'"

echo.
echo ---- Web Applications ---
echo.
PowerShell -NoProfile -ExecutionPolicy Unrestricted -Command "& 'C:\Deployments\Install\Utility\PowerShell\RemoveWebApplications.ps1'"

echo.
echo ---- Windows Webuser ---
echo.
PowerShell -NoProfile -ExecutionPolicy Unrestricted -Command "& 'C:\Deployments\Install\Utility\PowerShell\DeleteLocalWebuser.ps1'"

echo.
echo ---- Message Queues ---
echo.
PowerShell -NoProfile -ExecutionPolicy Unrestricted -Command "& 'C:\Deployments\Install\Utility\PowerShell\DeleteMessageQueues.ps1'"

echo.
echo ---- Event Log Sources ---
echo.
PowerShell -NoProfile -ExecutionPolicy Unrestricted -Command "& 'C:\Deployments\Install\Utility\PowerShell\RemoveEventLogSources.ps1'"

echo.
echo ---- Scheduled Tasks ---
echo.
PowerShell -NoProfile -ExecutionPolicy Unrestricted -Command "& 'C:\Deployments\Install\Utility\PowerShell\RemoveScheduledTasks.ps1'"
echo.

PAUSE