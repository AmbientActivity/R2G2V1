@ECHO OFF

PowerShell -NoProfile -ExecutionPolicy Unrestricted -Command "& 'C:\Deployments\Install\PowerShell\CreateEventLogSources.ps1'"
PowerShell -NoProfile -ExecutionPolicy Unrestricted -Command "& 'C:\Deployments\Install\PowerShell\CreateMessageQueues.ps1'"
PowerShell -NoProfile -ExecutionPolicy Unrestricted -Command "& 'C:\Deployments\Install\PowerShell\CreateScheduledTasks.ps1'"
PowerShell -NoProfile -ExecutionPolicy Unrestricted -Command "& 'C:\Deployments\Install\PowerShell\CreateLocalWebuser.ps1'"
PowerShell -NoProfile -ExecutionPolicy Unrestricted -Command "& 'C:\Deployments\Install\PowerShell\CreateWebApplications.ps1'"
PowerShell -NoProfile -ExecutionPolicy Unrestricted -Command "& 'C:\Deployments\Install\Database\PowerShell\CreateDatabase.ps1'"
PowerShell -NoProfile -ExecutionPolicy Unrestricted -Command "& 'C:\Deployments\Install\Database\PowerShell\DropAndCreateTables.ps1'"
PowerShell -NoProfile -ExecutionPolicy Unrestricted -Command "& 'C:\Deployments\Install\Database\PowerShell\SeedData.ps1'"
PowerShell -NoProfile -ExecutionPolicy Unrestricted -Command "& 'C:\Deployments\Install\PowerShell\InstallServices.ps1'"
echo.

PAUSE