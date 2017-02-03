Try
{
    Write-Host -ForegroundColor yellow "`n--- Local webuser ---`n"

    $computername = $env:COMPUTERNAME
    $ADSIComp = [adsi]"WinNT://$computername"
    $webuser = 'webuser'
    $isfound = $false

    Write-Host "Removing user..." -NoNewline
    $ADSIComp.Children | where {$_.SchemaClassName -eq 'user'}  | % {$_.name[0].ToString()} | where {$_.ToString() -like 'webuser'} | foreach {
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

    $isfound = $false
    Write-Host "Removing profile..." -NoNewline
    #-filter "localpath='C:\\Users\\$webuser'"
    Get-WmiObject Win32_UserProfile | where localpath -like '*webuser*' | foreach {
        #Write-Host $_.localpath
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
}
Catch
{
    throw $_.Exception.Message
}