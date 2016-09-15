# -----------------------------------------------------------------------------
 # CreateEventLogSources.ps1
 # AUTHOR: John Charlton
 # DATE: 09/14/2016

 # eventSources is an array of comma delimited strings containing the Event Log followed by the event source.

 $eventSources = @(
    # windows services
    "R2G2 State Machine Service, R2G2StateMachineService",
    "R2G2 Phidget Service, R2G2PhidgetService",
    "R2G2 Rfid Reader Service, R2G2RfidReaderService",

    # Display app
    "R2G2 Display, R2G2Display",

    # shared
    "R2G2 Message Queuing, R2G2MessageQueuing",
    "R2G2 Event Log, R2G2EventLog",

    #shceduled tasks
    "R2G2 Automated Export, R2G2AutomatedExport",

    #administration
    "R2G2 File Manager, R2G2FileManager"
 )
 
 # loop through each event log, source pair to create the source on the specified log if it does not exist.
 
 foreach($logSource in $eventSources) {
     $log = $logSource.split(",")[0]
     $source = $logSource.split(",")[1]

     if ([System.Diagnostics.EventLog]::SourceExists($source) -eq $false) {
        write-host "Creating event source $source on event log $log"
        [System.Diagnostics.EventLog]::CreateEventSource($source, $log)
        write-host -foregroundcolor green "Event source $source created"
     }
     else {
        write-host -foregroundcolor yellow "Warning: Event source $source already exists. Cannot create this source on Event log $log"
     }
 }