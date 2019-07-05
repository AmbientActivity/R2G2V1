Try
{
    Write-Host -ForegroundColor yellow "`n--- Local webuser ---`n"

    Write-Host "Creating local webuser..." -NoNewline

    $computername = $env:COMPUTERNAME
    $ADSIComp = [adsi]"WinNT://$computername"
    $webuser = 'webuser'

    $ADSIComp.Children | where {$_.SchemaClassName -eq 'user'}  | % {$_.name[0].ToString()} | where {$_.ToString() -like $webuser} | foreach {
        if ($_) {
            Write-Host "already exists."
            exit
        }
    }

    $newUser = $ADSIComp.Create('User', $webuser) 
    $password = 'ABBYu$er'
    $newUser.SetPassword(($password))
    $newUser.SetInfo()

    $newUser.Description  ='ABBY webuser account'
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