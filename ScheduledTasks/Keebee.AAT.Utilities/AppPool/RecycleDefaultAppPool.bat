@ECHO OFF
cd C:\Windows\System32\inetsrv
appcmd recycle apppool /apppool.name:DefaultAppPool
pause