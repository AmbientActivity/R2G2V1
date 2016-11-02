[Reflection.Assembly]::LoadWithPArtialName(“System.Messaging”)
$msmq = [System.Messaging.MessageQueue]

# Phidget Continuous Radio Message Queue (sends constant phidget values to the radio user control)
$name = "Phidget-Continuous-Radio"
$qname = “.\private$\” + $name
$qname = “.\private$\” + $name
If (($msmq::Exists($qname))) {
    $qObject = $msmq::Delete($qname)
}

$qObject = $msmq::Create($qname) 
$qObject.SetPermissions("Everyone", [System.Messaging.MessageQueueAccessRights]::FullControl, [System.Messaging.AccessControlEntryType]::Allow)
$qObject.SetPermissions("ANONYMOUS LOGON", [System.Messaging.MessageQueueAccessRights]::FullControl, [System.Messaging.AccessControlEntryType]::Allow)
write-host -foregroundcolor green "done."

# Phidget Continuous Television Message Queue (sends constant phidget values to the video user control)
$name = "Phidget-Continuous-Television"
$qname = “.\private$\” + $name
$qname = “.\private$\” + $name
If (($msmq::Exists($qname))) {
    $qObject = $msmq::Delete($qname)
}

$qObject = $msmq::Create($qname) 
$qObject.SetPermissions("Everyone", [System.Messaging.MessageQueueAccessRights]::FullControl, [System.Messaging.AccessControlEntryType]::Allow)
$qObject.SetPermissions("ANONYMOUS LOGON", [System.Messaging.MessageQueueAccessRights]::FullControl, [System.Messaging.AccessControlEntryType]::Allow)
write-host -foregroundcolor green "done."