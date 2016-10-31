[Reflection.Assembly]::LoadWithPArtialName(“System.Messaging”)
$msmq = [System.Messaging.MessageQueue]

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