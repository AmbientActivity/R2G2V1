Try
{
    Import-Module WebAdministration

    Set-ItemProperty iis:\AppPools\DefaultAppPool -Name processModel -Value @{userName="webuser";password='R2G2u$er';identitytype=3}

    New-WebApplication -Name "Keebee.AAT.DataAccess" -Site 'Default Web Site' -PhysicalPath "C:\Deployments\Web\Data\1.0.0.0" -ApplicationPool "DefaultAppPool" -Force
    New-WebApplication -Name "Keebee.AAT.Operations" -Site 'Default Web Site' -PhysicalPath "C:\Deployments\Web\API\1.0.0.0" -ApplicationPool "DefaultAppPool" -Force
    New-WebApplication -Name "Keebee.AAT.Administrator" -Site 'Default Web Site' -PhysicalPath "C:\Deployments\Web\Administrator\1.0.0.0" -ApplicationPool "DefaultAppPool" -Force
}
Catch
{
    Write-Host -ForegroundColor red $_.Exception.Message
}
