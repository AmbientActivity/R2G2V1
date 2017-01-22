Try
{
    Write-Host -ForegroundColor yellow "`n--- Web Applications ---`n"

    Import-Module WebAdministration

    $siteName = "Default Web Site"

    # remove administrator
    Write-Host "Removing Keebee.AAT.Administrator..." -NoNewline
    $administratorApp = "Keebee.AAT.Administrator"
    $path = "IIS:\Sites\$siteName\$administratorApp"
    if (Test-Path $path) 
    { 
        Remove-WebApplication -Name $administratorApp -Site $siteName
    }
    Write-Host "done." 

    # remove api
    Write-Host "Removing Keebee.AAT.Operations..." -NoNewline
    $operationsApp = "Keebee.AAT.Operations"
    $path = "IIS:\Sites\$siteName\$operationsApp"
    if (Test-Path $path) 
    { 
        Remove-WebApplication -Name $operationsApp -Site $siteName
    }
    Write-Host "done." 

    # remove data access
    Write-Host "Removing Keebee.AAT.DataAccess..." -NoNewline
    $dataAccessApp = "Keebee.AAT.DataAccess"
    $path = "IIS:\Sites\$siteName\$dataAccessApp"
    if (Test-Path $path) 
    { 
        Remove-WebApplication -Name $dataAccessApp -Site $siteName
    }
    Write-Host "done.`n" 

    # clear app pool credentials
    Write-Host "Reverting DefaultAppPool to default settings..." -NoNewline
    Set-ItemProperty iis:\AppPools\DefaultAppPool -Name processModel -Value @{identitytype=4}

    # clear recycling times
    Clear-ItemProperty IIS:\AppPools\DefaultAppPool -Name Recycling.periodicRestart.schedule
    Clear-ItemProperty IIS:\AppPools\DefaultAppPool -Name Recycling.periodicRestart.time
    Write-Host "done."
}
Catch
{
    Write-Host -ForegroundColor red $_.Exception.Message
}
