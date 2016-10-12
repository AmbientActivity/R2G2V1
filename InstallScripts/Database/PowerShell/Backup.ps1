$db = $svr.Databases['AdventureWorks']
$dbname = $db.Name
$dt = get-date -format yyyyMMddHHmmss
$dbbk = new-object ('Microsoft.SqlServer.Management.Smo.Backup')
$dbbk.Action = 'Database'
$dbbk.BackupSetDescription = "Full backup of " + $dbname
$dbbk.BackupSetName = $dbname + " Backup"
$dbbk.Database = $dbname
$dbbk.MediaDescription = "Disk"
$dbbk.Devices.AddDevice($bdir + "\" + $dbname + "_db_" + $dt + ".bak", 'File')
$dbbk.SqlBackup($svr)