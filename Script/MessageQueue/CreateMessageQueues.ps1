[Reflection.Assembly]::LoadWithPArtialName(System.Messaging)
$msmq = [System.Messaging.MessageQueue]

# Phidget Message Queue
$name = "Phidget"
$qname = .\private$\ + $name
If (($msmq::Exists($qname))) {
    $qObject = $msmq::Delete($qname)
}
$qObject = $msmq::Create($qname) 
$qObject.SetPermissions("Everyone", [System.Messaging.MessageQueueAccessRights]::FullControl, [System.Messaging.AccessControlEntryType]::Allow)
$qObject.SetPermissions("ANONYMOUS LOGON", [System.Messaging.MessageQueueAccessRights]::FullControl, [System.Messaging.AccessControlEntryType]::Allow)

# Video Message Queue
$name = "Video"
$qname = .\private$\ + $name
If (($msmq::Exists($qname))) {
    $qObject = $msmq::Delete($qname)
}
$qObject = $msmq::Create($qname) 
$qObject.SetPermissions("Everyone", [System.Messaging.MessageQueueAccessRights]::FullControl, [System.Messaging.AccessControlEntryType]::Allow)
$qObject.SetPermissions("ANONYMOUS LOGON", [System.Messaging.MessageQueueAccessRights]::FullControl, [System.Messaging.AccessControlEntryType]::Allow)


# RFID Message Queue
$name = "RFID"
$qname = .\private$\ + $name
If (($msmq::Exists($qname))) {
    $qObject = $msmq::Delete($qname)
}
$qObject = $msmq::Create($qname) 
$qObject.SetPermissions("Everyone", [System.Messaging.MessageQueueAccessRights]::FullControl, [System.Messaging.AccessControlEntryType]::Allow)
$qObject.SetPermissions("ANONYMOUS LOGON", [System.Messaging.MessageQueueAccessRights]::FullControl, [System.Messaging.AccessControlEntryType]::Allow)

# Response Message Queue
$name = "Response"
$qname = .\private$\ + $name
If (($msmq::Exists($qname))) {
    $qObject = $msmq::Delete($qname)
}
$qObject = $msmq::Create($qname) 
$qObject.SetPermissions("Everyone", [System.Messaging.MessageQueueAccessRights]::FullControl, [System.Messaging.AccessControlEntryType]::Allow)
$qObject.SetPermissions("ANONYMOUS LOGON", [System.Messaging.MessageQueueAccessRights]::FullControl, [System.Messaging.AccessControlEntryType]::Allow)


# Display Message Queue (to alert the State Machine Service when the Display App is running)
$name = "Display"
$qname = .\private$\ + $name
$qname = .\private$\ + $name
If (($msmq::Exists($qname))) {
    $qObject = $msmq::Delete($qname)
}

$qObject = $msmq::Create($qname) 
$qObject.SetPermissions("Everyone", [System.Messaging.MessageQueueAccessRights]::FullControl, [System.Messaging.AccessControlEntryType]::Allow)
$qObject.SetPermissions("ANONYMOUS LOGON", [System.Messaging.MessageQueueAccessRights]::FullControl, [System.Messaging.AccessControlEntryType]::Allow)

# Config Message Queue (to alert the State Machine Service when to repload a newly activated configuration)
$name = "Config"
$qname = .\private$\ + $name
$qname = .\private$\ + $name
If (($msmq::Exists($qname))) {
    $qObject = $msmq::Delete($qname)
}

$qObject = $msmq::Create($qname) 
$qObject.SetPermissions("Everyone", [System.Messaging.MessageQueueAccessRights]::FullControl, [System.Messaging.AccessControlEntryType]::Allow)
$qObject.SetPermissions("ANONYMOUS LOGON", [System.Messaging.MessageQueueAccessRights]::FullControl, [System.Messaging.AccessControlEntryType]::Allow)

# Phidget Monitor Message Queue (for testing - to monitor the phidget sensor change events)
$name = "Phidget-Monitor"
$qname = .\private$\ + $name
$qname = .\private$\ + $name
If (($msmq::Exists($qname))) {
    $qObject = $msmq::Delete($qname)
}

$qObject = $msmq::Create($qname) 
$qObject.SetPermissions("Everyone", [System.Messaging.MessageQueueAccessRights]::FullControl, [System.Messaging.AccessControlEntryType]::Allow)
$qObject.SetPermissions("ANONYMOUS LOGON", [System.Messaging.MessageQueueAccessRights]::FullControl, [System.Messaging.AccessControlEntryType]::Allow)

# Phidget Monitor Message Queue (for testing - to monitor the phidget sensor change events)
$name = "Phidget-Monitor-State"
$qname = .\private$\ + $name
$qname = .\private$\ + $name
If (($msmq::Exists($qname))) {
    $qObject = $msmq::Delete($qname)
}

$qObject = $msmq::Create($qname) 
$qObject.SetPermissions("Everyone", [System.Messaging.MessageQueueAccessRights]::FullControl, [System.Messaging.AccessControlEntryType]::Allow)
$qObject.SetPermissions("ANONYMOUS LOGON", [System.Messaging.MessageQueueAccessRights]::FullControl, [System.Messaging.AccessControlEntryType]::Allow)


# RFID Reader Monitor Message Queue (for testing - to monitor the rfid read events)
$name = "RFID-Monitor"
$qname = .\private$\ + $name
$qname = .\private$\ + $name
If (($msmq::Exists($qname))) {
    $qObject = $msmq::Delete($qname)
}

$qObject = $msmq::Create($qname) 
$qObject.SetPermissions("Everyone", [System.Messaging.MessageQueueAccessRights]::FullControl, [System.Messaging.AccessControlEntryType]::Allow)
$qObject.SetPermissions("ANONYMOUS LOGON", [System.Messaging.MessageQueueAccessRights]::FullControl, [System.Messaging.AccessControlEntryType]::Allow)

# RFID Reader Monitor Message Queue (for testing - to monitor the rfid read events)
$name = "RFID-Monitor-State"
$qname = .\private$\ + $name
$qname = .\private$\ + $name
If (($msmq::Exists($qname))) {
    $qObject = $msmq::Delete($qname)
}

$qObject = $msmq::Create($qname) 
$qObject.SetPermissions("Everyone", [System.Messaging.MessageQueueAccessRights]::FullControl, [System.Messaging.AccessControlEntryType]::Allow)
$qObject.SetPermissions("ANONYMOUS LOGON", [System.Messaging.MessageQueueAccessRights]::FullControl, [System.Messaging.AccessControlEntryType]::Allow)

Echo ("All queues created successfully.)