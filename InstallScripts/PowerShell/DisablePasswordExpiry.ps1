Try
{
    Write-Host -ForegroundColor yellow "`n--- Password Expiry ---`n`n"

    Write-Host "Disabling password expiry..." -NoNewline

    Invoke-Command -ScriptBlock { wmic path Win32_UserAccount set PasswordExpires=False } | Out-Null

    Write-Host "done."
}
Catch
{
    Write-Host -ForegroundColor red $_.Exception.Message
}