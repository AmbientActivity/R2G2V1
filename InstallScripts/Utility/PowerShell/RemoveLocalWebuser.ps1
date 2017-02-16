Try
{
    Write-Host -ForegroundColor yellow "`n--- Reemove Local webuser ---`n"

    $computername = $env:COMPUTERNAME
    $ADSIComp = [adsi]"WinNT://$computername"
    $webuser = 'webuser'
    $isfound = $false

    $isfound = $false
    Write-Host "Removing profile..." -NoNewline
    Get-WmiObject Win32_UserProfile | where localpath -like "*$webuser*" | foreach {
        if ($_) {
            $isfound = $true
            $_.Delete()
        }             
    }

    If ($isfound) {
        Write-Host "done."
    } Else {
        Write-Host "nothing to remove."
    }

    Write-Host "Removing user..." -NoNewline
    $ADSIComp.Children | where {$_.SchemaClassName -eq 'user'}  | % {$_.name[0].ToString()} | where {$_.ToString() -eq $webuser} | foreach {
        if ($_) {       
            $ADSIComp.Delete('User', $webuser)
            $isfound = $true
        }
    }
    
    If ($isfound) {
        Write-Host "done."
    } Else {
        Write-Host "nothing to remove."
    }
}
Catch
{
    throw $_.Exception.Message
}