[Reflection.Assembly]::LoadWithPArtialName(“System.Messaging”)
$msmq = [System.Messaging.MessageQueue]

# Phidget Monitor Message Queue (for testing - to monitor the phidget sensor change events)
write-host "Creating Phidget Monitor message queues..."
$name = "Phidget-Monitor"
$qname = “.\private$\” + $name
$qname = “.\private$\” + $name
If (($msmq::Exists($qname))) {
    $qObject = $msmq::Delete($qname)
}
$qObject = $msmq::Create($qname) 
$qObject.SetPermissions("Everyone", [System.Messaging.MessageQueueAccessRights]::FullControl, [System.Messaging.AccessControlEntryType]::Allow)
$qObject.SetPermissions("ANONYMOUS LOGON", [System.Messaging.MessageQueueAccessRights]::FullControl, [System.Messaging.AccessControlEntryType]::Allow)

# Phidget Monitor State Message Queue (for testing - to monitor the phidget sensor change events)
$name = "Phidget-Monitor-State"
$qname = “.\private$\” + $name
$qname = “.\private$\” + $name
If (($msmq::Exists($qname))) {
    $qObject = $msmq::Delete($qname)
}
$qObject = $msmq::Create($qname) 
$qObject.SetPermissions("Everyone", [System.Messaging.MessageQueueAccessRights]::FullControl, [System.Messaging.AccessControlEntryType]::Allow)
$qObject.SetPermissions("ANONYMOUS LOGON", [System.Messaging.MessageQueueAccessRights]::FullControl, [System.Messaging.AccessControlEntryType]::Allow)
write-host -foregroundcolor green "done."

write-host -foregroundcolor green "All queues created successfully.”