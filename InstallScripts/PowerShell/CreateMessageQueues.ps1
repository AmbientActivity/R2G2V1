Try
{
    Write-Host -ForegroundColor yellow "`n--- Message Queues ---`n"

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

    # Phidget Message Queue
    write-host "Creating Phidget message queue..." -NoNewline
    CreateMessageQueue "Phidget"
    CreateMessageQueue "Phidget-Monitor"
    CreateMessageQueue "Phidget-Monitor-State"
    write-host "done."

    # Video Capture Message Queue
    write-host "Creating Video Capture message queue..." -NoNewline
    CreateMessageQueue "Video-Capture"
    CreateMessageQueue "Video-Capture-State"
    write-host "done."

    # Bluetooth Beacon Watcher Message Queues
    write-host "Creating Bluetooth Beacon Watcher message queues..." -NoNewline
    CreateMessageQueue "Bluetooth-Beacon-Watcher"
    CreateMessageQueue "Bluetooth-Beacon-Watcher-Reload"
    write-host "done."

    # Response Message Queue
    write-host "Creating Response message queue..." -NoNewline
    CreateMessageQueue "Response"
    write-host "done."

    # Display Message Queues (to alert the Services when the Display App is running)
    write-host "Creating Display message queues..." -NoNewline
    CreateMessageQueue "Display-SMS"
    CreateMessageQueue "Display-Phidget"
    CreateMessageQueue "Display-Video-Capture"
    CreateMessageQueue "Display-Bluetooth-Beacon-Watcher"
    write-host "done."

    # Config Message Queues (to alert the Services when to reload a newly activated configuration)
    write-host "Creating Config message queues..." -NoNewline
    CreateMessageQueue "Config-SMS"
    CreateMessageQueue "Config-Phidget"
    write-host "done."

    # Phidget Continuous Radio Message Queue (sends constant phidget values to the radio user control)
    write-host "Creating Continuous Radio message queue..." -NoNewline
    CreateMessageQueue "Phidget-Continuous-Radio"
    write-host "done."

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
