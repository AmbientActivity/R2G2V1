Try
{
    Write-Host -ForegroundColor yellow "`n--- Message Queues ---`n"

    [Reflection.Assembly]::LoadWithPArtialName(“System.Messaging”) | Out-Null
    $msmq = [System.Messaging.MessageQueue]

    # Phidget Message Queue
    write-host "Creating Phidget message queue..." -NoNewline
    $name = "Phidget"
    $qname = “.\private$\” + $name
    If (($msmq::Exists($qname))) {
        $qObject = $msmq::Delete($qname)
    }
    $qObject = $msmq::Create($qname)
    $qObject.SetPermissions("Everyone", [System.Messaging.MessageQueueAccessRights]::FullControl, [System.Messaging.AccessControlEntryType]::Allow)
    $qObject.SetPermissions("ANONYMOUS LOGON", [System.Messaging.MessageQueueAccessRights]::FullControl, [System.Messaging.AccessControlEntryType]::Allow)
    write-host "done."


    # Video Capture Message Queue
    write-host "Creating Video Capture message queue..." -NoNewline
    $name = "Video-Capture"
    $qname = “.\private$\” + $name
    If (($msmq::Exists($qname))) {
        $qObject = $msmq::Delete($qname)
    }
    $qObject = $msmq::Create($qname)
    $qObject.SetPermissions("Everyone", [System.Messaging.MessageQueueAccessRights]::FullControl, [System.Messaging.AccessControlEntryType]::Allow)
    $qObject.SetPermissions("ANONYMOUS LOGON", [System.Messaging.MessageQueueAccessRights]::FullControl, [System.Messaging.AccessControlEntryType]::Allow)
    write-host "done."


    # Bluetooth Beacon Watcher Message Queue
    write-host "Creating Bluetooth Beacon Watcher message queues..." -NoNewline
    $name = "Bluetooth-Beacon-Watcher"
    $qname = “.\private$\” + $name
    If (($msmq::Exists($qname))) {
        $qObject = $msmq::Delete($qname)
    }
    $qObject = $msmq::Create($qname) 
    $qObject.SetPermissions("Everyone", [System.Messaging.MessageQueueAccessRights]::FullControl, [System.Messaging.AccessControlEntryType]::Allow)
    $qObject.SetPermissions("ANONYMOUS LOGON", [System.Messaging.MessageQueueAccessRights]::FullControl, [System.Messaging.AccessControlEntryType]::Allow)

    # Bluetooth Beacon Watcher Reload Message Queue
    $name = "Bluetooth-Beacon-Watcher-Reload"
    $qname = “.\private$\” + $name
    If (($msmq::Exists($qname))) {
        $qObject = $msmq::Delete($qname)
    }
    $qObject = $msmq::Create($qname) 
    $qObject.SetPermissions("Everyone", [System.Messaging.MessageQueueAccessRights]::FullControl, [System.Messaging.AccessControlEntryType]::Allow)
    $qObject.SetPermissions("ANONYMOUS LOGON", [System.Messaging.MessageQueueAccessRights]::FullControl, [System.Messaging.AccessControlEntryType]::Allow)
    write-host "done."

    # Response Message Queue
    write-host "Creating Response message queue..." -NoNewline
    $name = "Response"
    $qname = “.\private$\” + $name
    If (($msmq::Exists($qname))) {
        $qObject = $msmq::Delete($qname)
    }
    $qObject = $msmq::Create($qname) 
    $qObject.SetPermissions("Everyone", [System.Messaging.MessageQueueAccessRights]::FullControl, [System.Messaging.AccessControlEntryType]::Allow)
    $qObject.SetPermissions("ANONYMOUS LOGON", [System.Messaging.MessageQueueAccessRights]::FullControl, [System.Messaging.AccessControlEntryType]::Allow)
    write-host "done."


    # Display Message Queue (to alert the State Machine Service when the Display App is running)
    write-host "Creating Display message queues..." -NoNewline
    $name = "Display-SMS"
    $qname = “.\private$\” + $name
    $qname = “.\private$\” + $name
    If (($msmq::Exists($qname))) {
        $qObject = $msmq::Delete($qname)
    }
    $qObject = $msmq::Create($qname) 
    $qObject.SetPermissions("Everyone", [System.Messaging.MessageQueueAccessRights]::FullControl, [System.Messaging.AccessControlEntryType]::Allow)
    $qObject.SetPermissions("ANONYMOUS LOGON", [System.Messaging.MessageQueueAccessRights]::FullControl, [System.Messaging.AccessControlEntryType]::Allow)


    # Display Message Queue (to alert the Phidget Service when the Display App is running)
    $name = "Display-Phidget"
    $qname = “.\private$\” + $name
    $qname = “.\private$\” + $name
    If (($msmq::Exists($qname))) {
        $qObject = $msmq::Delete($qname)
    }
    $qObject = $msmq::Create($qname) 
    $qObject.SetPermissions("Everyone", [System.Messaging.MessageQueueAccessRights]::FullControl, [System.Messaging.AccessControlEntryType]::Allow)
    $qObject.SetPermissions("ANONYMOUS LOGON", [System.Messaging.MessageQueueAccessRights]::FullControl, [System.Messaging.AccessControlEntryType]::Allow)


    # Display Message Queue (to alert the Video Capture Service when the Display App is running)
    $name = "Display-Video-Capture"
    $qname = “.\private$\” + $name
    $qname = “.\private$\” + $name
    If (($msmq::Exists($qname))) {
        $qObject = $msmq::Delete($qname)
    }
    $qObject = $msmq::Create($qname) 
    $qObject.SetPermissions("Everyone", [System.Messaging.MessageQueueAccessRights]::FullControl, [System.Messaging.AccessControlEntryType]::Allow)
    $qObject.SetPermissions("ANONYMOUS LOGON", [System.Messaging.MessageQueueAccessRights]::FullControl, [System.Messaging.AccessControlEntryType]::Allow)
    write-host "done."


    # Config Message Queue (to alert the State Machine Service when to repload a newly activated configuration)
    write-host "Creating Config message queues..." -NoNewline
    $name = "Config-SMS"
    $qname = “.\private$\” + $name
    $qname = “.\private$\” + $name
    If (($msmq::Exists($qname))) {
        $qObject = $msmq::Delete($qname)
    }
    $qObject = $msmq::Create($qname) 
    $qObject.SetPermissions("Everyone", [System.Messaging.MessageQueueAccessRights]::FullControl, [System.Messaging.AccessControlEntryType]::Allow)
    $qObject.SetPermissions("ANONYMOUS LOGON", [System.Messaging.MessageQueueAccessRights]::FullControl, [System.Messaging.AccessControlEntryType]::Allow)


    # Config Message Queue (to alert the State Machine Service when to repload a newly activated configuration)
    $name = "Config-Phidget"
    $qname = “.\private$\” + $name
    $qname = “.\private$\” + $name
    If (($msmq::Exists($qname))) {
        $qObject = $msmq::Delete($qname)
    }
    $qObject = $msmq::Create($qname) 
    $qObject.SetPermissions("Everyone", [System.Messaging.MessageQueueAccessRights]::FullControl, [System.Messaging.AccessControlEntryType]::Allow)
    $qObject.SetPermissions("ANONYMOUS LOGON", [System.Messaging.MessageQueueAccessRights]::FullControl, [System.Messaging.AccessControlEntryType]::Allow)
    write-host "done."


    # Phidget Continuous Radio Message Queue (sends constant phidget values to the radio user control)
    write-host "Creating Continuous Radio message queue..." -NoNewline
    $name = "Phidget-Continuous-Radio"
    $qname = “.\private$\” + $name
    $qname = “.\private$\” + $name
    If (($msmq::Exists($qname))) {
        $qObject = $msmq::Delete($qname)
    }
    $qObject = $msmq::Create($qname) 
    $qObject.SetPermissions("Everyone", [System.Messaging.MessageQueueAccessRights]::FullControl, [System.Messaging.AccessControlEntryType]::Allow)
    $qObject.SetPermissions("ANONYMOUS LOGON", [System.Messaging.MessageQueueAccessRights]::FullControl, [System.Messaging.AccessControlEntryType]::Allow)
    write-host "done."
}
Catch
{
    throw $_.Exception.Message
}
