Try
{
    Write-Host -ForegroundColor yellow "`n--- Message Queues ---`n"

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

    # Phidget Message Queue
    write-host "Removing Phidget message queue..." -NoNewline
    RemoveMessageQueue "Phidget"
    write-host "done."

    # Video Capture Message Queue
    write-host "Removing Video Capture message queue..." -NoNewline
    RemoveMessageQueue "Video-Capture"
    RemoveMessageQueue "Video-Capture-State"
    write-host "done."

    # Bluetooth Beacon Watcher Message Queues
    write-host "Removing Bluetooth Beacon Watcher message queues..." -NoNewline
    RemoveMessageQueue "Bluetooth-Beacon-Watcher"
    RemoveMessageQueue "Bluetooth-Beacon-Watcher-Reload"
    write-host "done."

    # Response Message Queue
    write-host "Removing Response message queue..." -NoNewline
    RemoveMessageQueue "Response"
    write-host "done."

    # Display Message Queues (to alert the Services when the Display App is running)
    write-host "Removing Display message queues..." -NoNewline
    RemoveMessageQueue "Display-SMS"
    RemoveMessageQueue "Display-Phidget"
    RemoveMessageQueue "Display-Video-Capture"
    RemoveMessageQueue "Display-Bluetooth-Beacon-Watcher"
    write-host "done."

    # Config Message Queues (to alert the State Machine Service when to repload a newly activated configuration)
    write-host "Removing Config message queues..." -NoNewline
    $name = "Config-SMS"
    RemoveMessageQueue "Config-SMS"
    RemoveMessageQueue "Config-Phidget"
    write-host "done."

    # Phidget Continuous Radio Message Queue (sends constant phidget values to the radio user control)
    write-host "Removing Continuous Radio message queue..." -NoNewline
    RemoveMessageQueue "Phidget-Continuous-Radio"
    write-host "done."

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
