@ECHO OFF
PowerShell -NoProfile -ExecutionPolicy Unrestricted -Command "& 'C:\Deployments\Install\PowerShell\CreateLocalWebuser.ps1'"
C:\Windows\system32\net localgroup Administrators webuser /add
PAUSE