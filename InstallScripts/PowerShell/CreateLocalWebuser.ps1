Try
{
    Write-Host -ForegroundColor yellow "`n--- Local webuser ---`n"

    Write-Host "Creating local webuser..." -NoNewline

    $computername = $env:COMPUTERNAME
    $ADSIComp = [adsi]"WinNT://$computername"

    $username = 'webuser'
    $newUser = $ADSIComp.Create('User', $username) 
    $password = 'R2G2u$er'
    $newUser.SetPassword(($password))
    $newUser.SetInfo()

    $newUser.Description  ='webuser account'
    $newUser.SetInfo()

    Invoke-Command -ScriptBlock { C:\Windows\system32\net localgroup Administrators webuser /add } | Out-Null

    Write-Host "done."

    # disable password expiry
    Write-Host "Disabling password expiry..." -NoNewline
    Invoke-Command -ScriptBlock { wmic path Win32_UserAccount set PasswordExpires=False } | Out-Null
    Write-Host "done."
}
Catch
{
    throw $_.Exception.Message
}