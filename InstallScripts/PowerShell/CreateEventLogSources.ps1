Try
{
    Write-Host -ForegroundColor yellow "--- Event Log Sources ---`n"

    # eventSources is an array of comma delimited strings containing the Event Log followed by the event source.
    $eventSources = @(
        # windows services
        "ABBY State Machine Service,ABBYStateMachineService",
        "ABBY Phidget Service,ABBYPhidgetService",
        "ABBY Keep IIS Alive Service,ABBYKeepIISAliveService",
        "ABBY Video Capture Service,ABBYVideoCaptureService"
        "ABBY Bluetooth Beacon Watcher Service,ABBYBluetoothBeaconWatcherService"

        # Display app
        "ABBY Display,ABBYDisplay",

        # shared
        "ABBY Event Log,ABBYEventLog",

        #scheduled tasks
        "ABBY Automated Export,ABBYAutomatedExport"

        #admin interface
        "ABBY Administrator Interface,ABBYAdministratorInterface"
    )

    # loop through each event log, source pair to create the source on the specified log if it does not exist.
    foreach($logSource in $eventSources) {
        $log = $logSource.split(",")[0]
        $source = $logSource.split(",")[1]

        if ([System.Diagnostics.EventLog]::SourceExists($source) -eq $false) {
            write-host "Creating event log source $log..." -NoNewline
            [System.Diagnostics.EventLog]::CreateEventSource($source, $log)
            write-host "done."
        }
        else {
            write-host -foregroundcolor yellow "Warning: Event source $source already exists. Cannot create this source on Event log $log"
        }
    }
}
Catch
{
    throw $_.Exception.Message
}