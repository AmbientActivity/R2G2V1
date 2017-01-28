Try
{
    Write-Host -ForegroundColor yellow "`n--- Event Log Sources ---`n"
 
    $logNames = Get-EventLog -List | Select LogDisplayName | Where-Object {$_.LogDisplayName -like "R2G2*"}

    foreach($logName in $logNames)
    {
        $source = $logName.LogDisplayName
    
        write-host "Removing event log source $source..." -NoNewline
        if ([System.Diagnostics.EventLog]::SourceExists($source) -eq $true) 
        {   
            Remove-EventLog -LogName $source
        }
        write-host "done."
    }
 }
 Catch
 {
    throw $_.Exception.Message
 }