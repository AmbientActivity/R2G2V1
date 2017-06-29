Try
{
    Write-Host -ForegroundColor yellow "`n--- Thumbnails ---`n"

    $exePath = "C:\Deployments\Install\Thumbnail\1.0.0.0\Keebee.AAT.GenerateThumbnails.exe"

    write-host "Creating thumbnails (this may take a few minutes)..." -NoNewline

    Start-Process -FilePath $exePath -NoNewWindow -Wait

    write-host "done."
}
Catch
{
    throw $_.Exception.Message
}