$server = $env:COMPUTERNAME + "\SQLEXPRESS"
$database = "KeebeeAAT"
$exportFolder = "\\$server\KeebeeAATFilestream\Media\Exports\EventLog"

$path = "C:\Deployments\Install\Utility\EventLog\Database\"

Try
{
    Write-Host -ForegroundColor yellow "`n--- Purge Event Logs ---`n"

    # check if the database exists
    $query = Invoke-SqlQuery -Query "SELECT COUNT(*) AS DatabaseCount FROM master.sys.databases WHERE name = N'$database'" -Server $server -Database "master"
    $databaseCount = $query.DatabaseCount

    # if the database doesn't exist, don't attempt anything
    if ($databaseCount -eq 0) {
        Write-Host -ForegroundColor yellow "`nR2G2 database does not exist.`n"
    }
    else
    {
        Write-Host "Purging..." -NoNewline
        $queryFile = $path + "PurgeEventLogs.sql"
        Invoke-SqlQuery -File $queryFile -Server $server -Database $database

        $date = Get-Date
        $filename = "EventLog_" + $date.ToString("yyyy_MM_dd") + ".xls"
        $newFilename = "EventLog_" + $date.ToString("yyyy_MM_dd") + "_1.xls"

        if (Test-Path "$exportFolder\$filename")
        {
            Rename-Item -Path "$exportFolder\$filename" -NewName $newFilename
        }
        Write-Host "done."
    }
}
Catch
{
    throw $_.Exception.Message
}