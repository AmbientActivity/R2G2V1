Try
{
    Write-Host -ForegroundColor yellow "`n--- Event Log Sources ---`n"

    # eventSources is an array of comma delimited strings containing the Event Log followed by the event source.
    $eventSources = @(
        # windows services
        "R2G2 State Machine Service,R2G2StateMachineService",
        "R2G2 Phidget Service,R2G2PhidgetService",
        "R2G2 Keep IIS Alive Service,R2G2KeepIISAliveService",
        "R2G2 Video Capture Service,R2G2VideoCaptureService"
        "R2G2 Bluetooth Beacon Watcher Service,R2G2BluetoothBeaconWatcherService"

        # Display app
        "R2G2 Display,R2G2Display",

        # shared
        "R2G2 Event Log,R2G2EventLog",

        #scheduled tasks
        "R2G2 Automated Export,R2G2AutomatedExport"

        #admin interface
        "R2G2 Administrator Interface,R2G2AdministratorInterface"
     )
 
     # loop through each event log, source pair to delete the source on the specified log if it exists
 
     foreach($logSource in $eventSources) 
     {
         $log = $logSource.split(",")[0]
         $source = $logSource.split(",")[1]

         write-host "Removing event log source $log..." -NoNewline
         if ([System.Diagnostics.EventLog]::SourceExists($source) -eq $true) 
         {   
            Remove-EventLog -LogName $log
         }
         write-host "done."
     }
 }
 Catch
 {
    Write-Host -ForegroundColor red $_.Exception.Message
 }