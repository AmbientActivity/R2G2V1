Try
{
    Write-Host -ForegroundColor yellow "`n--- Batch Event Log Export ---`n"

    $path = "C:\Users\John\Source\Repos\R2G2V1\Shared\Keebee.AAT.Exporting\bin\Release\"
    $targetDLL = "Keebee.AAT.Exporting.dll"

    #Load all .NET binaries in the folder
    Get-ChildItem -recurse $path | Where-Object {($_.Extension -EQ ".dll") -or ($_.Extension -eq ".exe")} | ForEach-Object { $AssemblyName=$_.FullName; Try {[Reflection.Assembly]::LoadFile($AssemblyName)} Catch{ "***ERROR*** Not .NET assembly: " + $AssemblyName}} | Out-Null
    $exporter = New-Object "Keebee.AAT.Exporting.EventLogExporter"

    $endDate = Get-Date
    #$startDate = [datetime]"09/01/2017"

    $inputDate = Read-Host -Prompt 'Enter start date (MM/DD/YYYY)'
    $startDate = [datetime]$inputDate

    Write-Host "`nExporting..." -NoNewline

    while ($startDate -le $endDate) {
        $exporter.ExportAndSave($startDate.ToString("MM/dd/yyyy"))
        $startDate = $startDate.AddDays(1)
    }

    Write-Host "done.”

}
Catch
{
    throw $_.Exception.Message
}