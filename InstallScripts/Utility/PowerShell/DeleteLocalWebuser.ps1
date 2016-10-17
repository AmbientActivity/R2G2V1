Try
{
  Write-Host "Deleting local webuser..." -NoNewline

  $computername = $env:COMPUTERNAME
  $ADSIComp = [adsi]"WinNT://$computername"
  $ADSIComp.Delete('User','webuser') 

  Write-Host "done.`n" 
}
Catch
{
    Write-Host -ForegroundColor red $_.Exception.Message
}