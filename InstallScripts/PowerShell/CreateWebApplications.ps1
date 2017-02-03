Try
{
    Write-Host -ForegroundColor yellow "`n--- Web Applications ---`n"

    Import-Module WebAdministration

    $siteName = 'Default Web Site'
    $userName = 'webuser'
    $password = 'R2G2u$er'
    $administratorAppName = 'Keebee.AAT.Administrator'

    # create the web applications
    Write-Host "Creating DataAccess..." -NoNewline
    New-WebApplication -Name "Keebee.AAT.DataAccess" -Site $siteName -PhysicalPath "C:\Deployments\Web\Data\1.0.0.0" -ApplicationPool "DefaultAppPool" -Force | Out-Null
    Write-Host "done."

    Write-Host "Creating Operations API..." -NoNewline
    New-WebApplication -Name "Keebee.AAT.Operations" -Site $siteName -PhysicalPath "C:\Deployments\Web\API\1.0.0.0" -ApplicationPool "DefaultAppPool" -Force | Out-Null
    Write-Host "done."

    Write-Host "Creating Adminstrator Interface..." -NoNewline
    New-WebApplication -Name $administratorAppName -Site $siteName -PhysicalPath 'C:\Deployments\Web\Administrator\1.0.0.0' -ApplicationPool "DefaultAppPool" -Force | Out-Null
    Write-Host "done.`n"

    # set default app pool credentials
    Write-Host "Setting DefaultAppPool Identity..." -NoNewline
    Set-ItemProperty iis:\AppPools\DefaultAppPool -Name processModel -Value @{userName=$userName;password=$password;identitytype=3}
    Write-Host "done."

    # set default app pool recycle schedule to '12AM daily'
    Write-Host "Setting DefaultAppPool recycling schedule..." -NoNewline
    Set-ItemProperty iis:\AppPools\DefaultAppPool -Name Recycling.periodicRestart.schedule -Value @{value="00:00"}
    Set-ItemProperty iis:\AppPools\DefaultAppPool -Name Recycling.periodicRestart.time -Value "0"
    Write-Host "done."

    # set pass-through authentication (connect as...) for 'Keebee.AAT.Administrator' to 'webuser'
    Write-Host "Setting Administrator pass-through authentication..." -NoNewline
    $apps = Get-WebApplication
    ForEach($app in $apps)
    {
        $xpath = ($app | Select -Property "ItemXPath").ItemXPath
        $fullPath = "$xpath/virtualDirectory[@path='/']" 
        $fullPath = $fullPath.Substring(1)

        if ($fullPath -like '*' + $administratorAppName + '*')
        {
            Set-WebConfigurationProperty $fullPath -Name "username" -Value $userName
            Set-WebConfigurationProperty $fullPath -Name "password" -Value $password
        }
    }

    # start iis if not already running
    $status = (Get-Service "W3SVC").Status
    if ($status -eq "Stopped") {
        Start-Service "W3SVC"
    }

    Write-Host "done."
}
Catch
{
    throw $_.Exception.Message
}
