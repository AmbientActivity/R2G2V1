Write-Host -ForegroundColor yellow "`n--- Set Web Paths To Production ---`n"

Write-Host "Setting paths to production..." -NoNewline

Import-Module WebAdministration

Set-ItemProperty "IIS:\Sites\Default Web Site\Keebee.AAT.Administrator" -Name physicalPath -Value C:\Deployments\Web\Administrator\1.0.0.0

Set-ItemProperty "IIS:\Sites\Default Web Site\Keebee.AAT.DataAccess" -name physicalPath -value "C:\Deployments\Web\Data\1.0.0.0"

Set-ItemProperty "IIS:\Sites\Default Web Site\Keebee.AAT.Operations" -name physicalPath -value "C:\Deployments\Web\API\1.0.0.0"

Write-Host "done."