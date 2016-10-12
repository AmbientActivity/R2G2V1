@ECHO OFF
PowerShell -NoProfile -ExecutionPolicy Unrestricted -Command "& 'C:\Deployments\Scripts\Windows\CreateLocalWebuser.ps1'"
C:\Windows\system32\net localgroup Administrators webuser /add
PAUSE