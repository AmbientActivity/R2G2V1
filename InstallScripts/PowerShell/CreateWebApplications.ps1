Try
{
    Import-Module WebAdministration

    $siteName = 'Default Web Site'
    $userName = 'webuser'
    $password = 'R2G2u$er'
    $administratorAppName = 'Keebee.AAT.Administrator'

    # set default app pool credentials
    Set-ItemProperty iis:\AppPools\DefaultAppPool -Name processModel -Value @{userName=$userName;password=$password;identitytype=3}

    # set default app pool recycle schedule to '12AM daily'
    Set-ItemProperty iis:\AppPools\DefaultAppPool -Name Recycling.periodicRestart.schedule -Value @{value="00:00"}
    Set-ItemProperty iis:\AppPools\DefaultAppPool -Name Recycling.periodicRestart.time -Value "0"

    # create the web applications
    New-WebApplication -Name "Keebee.AAT.DataAccess" -Site $siteName -PhysicalPath "C:\Deployments\Web\Data\1.0.0.0" -ApplicationPool "DefaultAppPool" -Force
    New-WebApplication -Name "Keebee.AAT.Operations" -Site $siteName -PhysicalPath "C:\Deployments\Web\API\1.0.0.0" -ApplicationPool "DefaultAppPool" -Force
    New-WebApplication -Name  $administratorAppName -Site $siteName -PhysicalPath 'C:\Deployments\Web\Administrator\1.0.0.0' -ApplicationPool "DefaultAppPool" -Force

    # set pass-through authentication (connect as...) for 'Keebee.AAT.Administrator' to 'webuser'
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
}
Catch
{
    Write-Host -ForegroundColor red $_.Exception.Message
}
