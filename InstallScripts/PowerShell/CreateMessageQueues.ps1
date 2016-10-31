[Reflection.Assembly]::LoadWithPArtialName(“System.Messaging”)
$msmq = [System.Messaging.MessageQueue]

# Phidget Message Queue
write-host "Creating Phidget message queue..."
$name = "Phidget"
$qname = “.\private$\” + $name
If (($msmq::Exists($qname))) {
    $qObject = $msmq::Delete($qname)
}
$qObject = $msmq::Create($qname) 
$qObject.SetPermissions("Everyone", [System.Messaging.MessageQueueAccessRights]::FullControl, [System.Messaging.AccessControlEntryType]::Allow)
$qObject.SetPermissions("ANONYMOUS LOGON", [System.Messaging.MessageQueueAccessRights]::FullControl, [System.Messaging.AccessControlEntryType]::Allow)
write-host -foregroundcolor green "done."


# Video Capture Message Queue
write-host "Creating Video Capture message queue..."
$name = "Video-Capture"
$qname = “.\private$\” + $name
If (($msmq::Exists($qname))) {
    $qObject = $msmq::Delete($qname)
}
$qObject = $msmq::Create($qname) 
$qObject.SetPermissions("Everyone", [System.Messaging.MessageQueueAccessRights]::FullControl, [System.Messaging.AccessControlEntryType]::Allow)
$qObject.SetPermissions("ANONYMOUS LOGON", [System.Messaging.MessageQueueAccessRights]::FullControl, [System.Messaging.AccessControlEntryType]::Allow)
write-host -foregroundcolor green "done."

# RFID Message Queue
write-host "Creating RFID Reader message queue..."
$name = "RFID"
$qname = “.\private$\” + $name
If (($msmq::Exists($qname))) {
    $qObject = $msmq::Delete($qname)
}
$qObject = $msmq::Create($qname) 
$qObject.SetPermissions("Everyone", [System.Messaging.MessageQueueAccessRights]::FullControl, [System.Messaging.AccessControlEntryType]::Allow)
$qObject.SetPermissions("ANONYMOUS LOGON", [System.Messaging.MessageQueueAccessRights]::FullControl, [System.Messaging.AccessControlEntryType]::Allow)
write-host -foregroundcolor green "done."

# Response Message Queue
write-host "Creating Response message queue..."
$name = "Response"
$qname = “.\private$\” + $name
If (($msmq::Exists($qname))) {
    $qObject = $msmq::Delete($qname)
}
$qObject = $msmq::Create($qname) 
$qObject.SetPermissions("Everyone", [System.Messaging.MessageQueueAccessRights]::FullControl, [System.Messaging.AccessControlEntryType]::Allow)
$qObject.SetPermissions("ANONYMOUS LOGON", [System.Messaging.MessageQueueAccessRights]::FullControl, [System.Messaging.AccessControlEntryType]::Allow)
write-host -foregroundcolor green "done."

# Display Message Queue (to alert the State Machine Service when the Display App is running)
write-host "Creating Display message queues..."
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
write-host -foregroundcolor green "done."

# Config Message Queue (to alert the State Machine Service when to repload a newly activated configuration)
write-host "Creating Config message queues..."
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
write-host -foregroundcolor green "done."

$qObject = $msmq::Create($qname) 
$qObject.SetPermissions("Everyone", [System.Messaging.MessageQueueAccessRights]::FullControl, [System.Messaging.AccessControlEntryType]::Allow)
$qObject.SetPermissions("ANONYMOUS LOGON", [System.Messaging.MessageQueueAccessRights]::FullControl, [System.Messaging.AccessControlEntryType]::Allow)

# Phidget Continuous Message Queue (sends all phidget values to the display)
$name = "Phidget-Continuous"
$qname = “.\private$\” + $name
$qname = “.\private$\” + $name
If (($msmq::Exists($qname))) {
    $qObject = $msmq::Delete($qname)
}

$qObject = $msmq::Create($qname) 
$qObject.SetPermissions("Everyone", [System.Messaging.MessageQueueAccessRights]::FullControl, [System.Messaging.AccessControlEntryType]::Allow)
$qObject.SetPermissions("ANONYMOUS LOGON", [System.Messaging.MessageQueueAccessRights]::FullControl, [System.Messaging.AccessControlEntryType]::Allow)
write-host -foregroundcolor green "done."