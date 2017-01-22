Try
{
    Write-Host -ForegroundColor yellow "`n--- Message Queues ---`n"

    [Reflection.Assembly]::LoadWithPArtialName(“System.Messaging”) | Out-Null
    $msmq = [System.Messaging.MessageQueue]


    # Phidget Message Queue
    write-host "Deleting Phidget message queue..." -NoNewline
    $name = "Phidget"
    $qname = “.\private$\” + $name
    If (($msmq::Exists($qname))) {
        $qObject = $msmq::Delete($qname)
    }
    write-host "done."


    # Video Capture Message Queue
    write-host "Deleting Video Capture message queue..." -NoNewline
    $name = "Video-Capture"
    $qname = “.\private$\” + $name
    If (($msmq::Exists($qname))) {
        $qObject = $msmq::Delete($qname)
    }
    write-host "done."


    # Bluetooth Beacon Watcher Message Queue
    write-host "Deleting Bluetooth Beacon Watcher message queue..." -NoNewline
    $name = "Bluetooth-Beacon-Watcher"
    $qname = “.\private$\” + $name
    If (($msmq::Exists($qname))) {
        $qObject = $msmq::Delete($qname)
    }
    write-host "done."

    # Bluetooth Beacon Watcher Reload Message Queue
    write-host "Deleting Bluetooth Beacon Watcher Reload message queue..." -NoNewline
    $name = "Bluetooth-Beacon-Watcher-Reload"
    $qname = “.\private$\” + $name
    If (($msmq::Exists($qname))) {
        $qObject = $msmq::Delete($qname)
    }
    write-host "done."

    # Response Message Queue
    write-host "Deleting Response message queue..." -NoNewline
    $name = "Response"
    $qname = “.\private$\” + $name
    If (($msmq::Exists($qname))) {
        $qObject = $msmq::Delete($qname)
    }
    write-host "done."


    # Display Message Queue (to alert the State Machine Service when the Display App is running)
    write-host "Deleting Display message queues..." -NoNewline
    $name = "Display-SMS"
    $qname = “.\private$\” + $name
    $qname = “.\private$\” + $name
    If (($msmq::Exists($qname))) {
        $qObject = $msmq::Delete($qname)
    }


    # Display Message Queue (to alert the Phidget Service when the Display App is running)
    $name = "Display-Phidget"
    $qname = “.\private$\” + $name
    $qname = “.\private$\” + $name
    If (($msmq::Exists($qname))) {
        $qObject = $msmq::Delete($qname)
    }


    # Display Message Queue (to alert the Video Capture Service when the Display App is running)
    $name = "Display-Video-Capture"
    $qname = “.\private$\” + $name
    $qname = “.\private$\” + $name
    If (($msmq::Exists($qname))) {
        $qObject = $msmq::Delete($qname)
    }
    write-host "done."


    # Config Message Queue (to alert the State Machine Service when to repload a newly activated configuration)
    write-host "Deleting Config message queues..." -NoNewline
    $name = "Config-SMS"
    $qname = “.\private$\” + $name
    $qname = “.\private$\” + $name
    If (($msmq::Exists($qname))) {
        $qObject = $msmq::Delete($qname)
    }


    # Config Message Queue (to alert the State Machine Service when to repload a newly activated configuration)
    $name = "Config-Phidget"
    $qname = “.\private$\” + $name
    $qname = “.\private$\” + $name
    If (($msmq::Exists($qname))) {
        $qObject = $msmq::Delete($qname)
    }
    write-host "done."


    # Phidget Continuous Radio Message Queue (sends constant phidget values to the radio user control)
    write-host "Deleting Continuous Radio message queue..." -NoNewline
    $name = "Phidget-Continuous-Radio"
    $qname = “.\private$\” + $name
    $qname = “.\private$\” + $name
    If (($msmq::Exists($qname))) {
        $qObject = $msmq::Delete($qname)
    }
    write-host "done."
}
Catch
{
    Write-Host -ForegroundColor red $_.Exception.Message
}
