Try
{
    Write-Host -ForegroundColor yellow "`n--- Video Capture ---`n"

    $videoCapturesRoot = "C:\VideoCaptures"

    Write-Host "Removing Video Captures..." -NoNewline
    If(test-path $videoCapturesRoot)
    {
        Remove-Item $videoCapturesRoot -recurse -Force
    }
    Write-Host "done."
}
Catch
{
    throw $_.Exception.Message
}