Try
{
    Write-Host "Creating local webuser..." -NoNewline

    $computername = $env:COMPUTERNAME
    $ADSIComp = [adsi]"WinNT://$computername"

    $username = 'webuser'
    $newUser = $ADSIComp.Create('User', $username) 
    $password = 'R2G2u$er'
    $newUser.SetPassword(($password))
    $newUser.SetInfo()

    $newUser.Description  ='Test webuser account'
    $newUser.SetInfo()

    Write-Host "done."
}
Catch
{
    Write-Host -ForegroundColor red $_.Exception.Message
}