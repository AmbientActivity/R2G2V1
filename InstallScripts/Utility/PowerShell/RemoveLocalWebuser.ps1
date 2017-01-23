Try
{
  Write-Host -ForegroundColor yellow "`n--- Local webuser ---`n"

  Write-Host "Removing local webuser..." -NoNewline

  $computername = $env:COMPUTERNAME
  $ADSIComp = [adsi]"WinNT://$computername"
  $ADSIComp.Delete('User','webuser') 

  Write-Host "done." 
}
Catch
{
    Write-Host -ForegroundColor red $_.Exception.Message
}