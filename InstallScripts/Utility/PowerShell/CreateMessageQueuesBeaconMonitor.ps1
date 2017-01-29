Try
{
    Write-Host -ForegroundColor yellow "`n--- Beacon Monitor Message Queues ---`n"

    [Reflection.Assembly]::LoadWithPArtialName(“System.Messaging”) | Out-Null
    $msmq = [System.Messaging.MessageQueue]

    function CreateMessageQueue
    {
        $name = $args[0]
        $qname = “.\private$\$name"
        If (($msmq::Exists($qname))) {
            $qObject = $msmq::Delete($qname)
        }
        $qObject = $msmq::Create($qname)
        $qObject.SetPermissions("Everyone", [System.Messaging.MessageQueueAccessRights]::FullControl, [System.Messaging.AccessControlEntryType]::Allow)
        $qObject.SetPermissions("ANONYMOUS LOGON", [System.Messaging.MessageQueueAccessRights]::FullControl, [System.Messaging.AccessControlEntryType]::Allow)
    }

    # Beaon Monitor Message Queues
    write-host "Creating Beacon Monitor message queues..." -NoNewline
    CreateMessageQueue "Beacon-Monitor"
    CreateMessageQueue "Beacon-Monitor-Resident"
    CreateMessageQueue "Beacon-Monitor-State"
    write-host "done."
}
Catch
{
    throw $_.Exception.Message
}
