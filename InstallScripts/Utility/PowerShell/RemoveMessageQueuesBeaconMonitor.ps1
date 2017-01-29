Try
{
    Write-Host -ForegroundColor yellow "`n--- Beacon Monitor Message Queues ---`n"

    [Reflection.Assembly]::LoadWithPArtialName(“System.Messaging”) | Out-Null
    $msmq = [System.Messaging.MessageQueue]

    function RemoveMessageQueue
    {
        $name = $args[0]
        $qname = “.\private$\$name"
        If (($msmq::Exists($qname))) {
            $qObject = $msmq::Delete($qname)
        }
    }

    # Beacon Monitor message queus
    write-host "Removing Beacon Monitor message queues..." -NoNewline
    RemoveMessageQueue "Beacon-Monitor"
    RemoveMessageQueue "Beacon-Monitor-Resident"
    RemoveMessageQueue "Beacon-Monitor-State"
    write-host "done."
}
Catch
{
    throw $_.Exception.Message
}
