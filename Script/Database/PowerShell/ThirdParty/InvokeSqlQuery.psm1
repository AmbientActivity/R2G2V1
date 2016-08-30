function GetSqlBatchesFromFile($file)
{
$buffer = new-object System.Text.StringBuilder

switch -regex -file $file {

"^\s*GO[\s\d]*$" { $buffer.ToString(); $buffer.Length = 0;}
default { $temp = $buffer.AppendLine($_);} 

}
$buffer.ToString();
}

function GetSqlBatchesFromString($string)
{
$buffer = new-object System.Text.StringBuilder

$string -Split "`n" | foreach {

switch -regex ($_) {

"^\s*GO[\s\d]*$" { $buffer.ToString(); $buffer.Length = 0;}
default { $temp = $buffer.AppendLine($_);} 

}
}
$buffer.ToString();
}

function WriteConnectionEvents($currentEventID)
{
Get-Event | % { 
if ($_.SourceIdentifier -eq $currentEventID)
{
    $CurrentEventIdentifier = $_.EventIdentifier;
    
    $info = $_.SourceEventArgs
    
    Write-Verbose "Event:$info"
    
    Remove-Event -EventIdentifier $CurrentEventIdentifier
}
}
}


function ExecuteBatch($cnn, $batch, [Hashtable] $Parameters, $IncludeRecordSetIndex, $IncludeRecordsCount, $ExecutionTimeout, [System.Data.SqlClient.SqlTransaction] $SqlTransaction)
{
    if ([string]::IsNullOrEmpty($batch) -eq $false)
    {
        WriteConnectionEvents $eventID;
    
        Write-Verbose "Begin Batch -------------------------------------"
        Write-Verbose -Message "Batch:   $batch"
        Write-Verbose -Message "Timeout: $ExecutionTimeout"
        $cmd = $cnn.CreateCommand();
        $cmd.CommandText = $batch;
        $cmd.CommandTimeout = $ExecutionTimeout;
        $cmd.Transaction = $SqlTransaction;
                
        
        
        $confirmMessage = $batch;
        
        if (($Parameters -ne $null) -and ($Parameters.Count -ne 0))
        {
            $confirmMessage = $confirmMessage+"`nParameters:`n";
            $Parameters.Keys | foreach { 
            $parameterName = $_;
            $parameterValue = $Parameters[$_];
        
            Write-Verbose -Message "Parameter: $parameterName=$parameterValue"
            $confirmMessage = $confirmMessage+"$parameterName=$parameterValue`n";
            
            $pardummy = $cmd.Parameters.AddWithValue($parameterName, $parameterValue);
            
            }
        }

        $confirmTarget = "["+$cnn.DataSource+"].["+$cnn.Database+"]";

        if ($psCmdlet.shouldProcess($confirmTarget,$confirmMessage.Trim()))
        {

            $reader = $cmd.ExecuteReader()
                do 
                {
                    $recordsetIndex++;
                    [int] $rec = 0;

                    while ($reader.Read()) {
                        $rec++;
                        $record = New-object PSObject

                        for ($i = 0; $i -lt $reader.FieldCount; $i++)
                        {
                            $name = $reader.GetName($i);
                            if ([string]::IsNullOrEmpty($name))
                            {
                                $name = "Column#" + $i.ToString();
                            }
                            
                            $val = $reader.GetValue($i);
                            
                            Add-Member -MemberType NoteProperty -InputObject $record -Name $name -Value $val 
                        }
                        
                        if ($IncludeRecordSetIndex)
                        {
                            Add-Member -MemberType NoteProperty -InputObject $record -Name "RecordSetIndex" -Value $recordsetIndex
                        }

                        $record
                    }
                    WriteConnectionEvents $eventID;
                                                    

                    if ($IncludeRecordsCount)
                    {
                    "("+$rec.ToString()+" row(s) affected)"
                    }
                           
                }
                while ($reader.NextResult());
                $reader.Close();
            }
        
    
        WriteConnectionEvents $eventID
    
        Write-Verbose "End Batch -------------------------------------"
    }
}

function New-SqlConnection(
[string] $Server = ".", 
[string] $Database = "master", 
[System.Management.Automation.PSCredential] $Credential,
[int] $ConnectionTimeout = 15
)
{
    $ci = "Data Source=$Server;Initial Catalog=$Database;Connect Timeout=$ConnectionTimeout;"

    if ($Credential -eq $null)    {
        $ci = $ci + "Integrated Security=true;";
    }
    else    {
        $ci = $ci + "Integrated Security=false;User ID="+$Credential.UserName+";Password="+$Credential.Password.ToString()+";"
    }
    
    

    $cnn = new-object System.Data.SqlClient.SqlConnection;
    $cnn.ConnectionString = $ci;
    $cnn.Open();
    $cnn;
    
<#
.DESCRIPTION
    Instantiates and Opens SqlConnection object

.PARAMETER Server
	Server name
.PARAMETER Database
	Database name (optional. default value ‘master’)
.PARAMETER Credential
	Credentials for Sql Authentication. If not specified, Windows Authentication will be used
.PARAMETER ConnectionTimeout
    Connection timeout
.EXAMPLE    
    $connection =  New-SqlConnection -Server "."

.EXAMPLE    
    $connection =  New-SqlConnection -Server "." -Database "MyDatabase"

.OUTPUTS
    Connected SqlConnection ovject

.LINK
    http://powershell4sql.codeplex.com
#>    
}



function Invoke-SqlQuery
{
[CmdletBinding(SupportsTransactions=$true, DefaultParameterSetName = "NewConnection", SupportsShouldProcess=$true)]
param
(
[Parameter(ValueFromPipeline=$true, ValueFromPipelineByPropertyName=$true, Position=0)]
[string] $Query, 
[Parameter(ValueFromPipeline=$true, ValueFromPipelineByPropertyName=$true)]
[Alias("FullName")]
[string] $File,
[Hashtable] $Parameters = $null,
[Parameter(ParameterSetName = "NewConnection")][string] $Server = ".", 
[Parameter(ParameterSetName = "NewConnection")][string] $Database = "master", 
[Parameter(ParameterSetName = "NewConnection")][System.Management.Automation.PSCredential] $Credential,
[Parameter(ParameterSetName = "ExistingConnection")][System.Data.SqlClient.SqlConnection] $Connection,
[Parameter(ParameterSetName = "ExistingConnection")][System.Data.SqlClient.SqlTransaction] $SqlTransaction = $null,
[switch] $IncludeRecordSetIndex,
[switch] $IncludeRecordsCount,
[int] $ConnectionTimeout = 15,
[int] $ExecutionTimeout = 0
)
begin {

    $eventID = $null;

    if ($Connection -eq $null)
    {
        $cnn = New-SqlConnection $Server $Database $Credential $ConnectionTimeout
    }
    else
    {
        if ($Connection.State -ne [System.Data.ConnectionState]::Open)
        {
            $Connection.Open();
        }
        $cnn = $Connection;
    }
    
    $eventID = "Connection.Messages."+[datetime]::Now.Ticks;
    Register-ObjectEvent -inputObject $cnn -eventName InfoMessage -sourceIdentifier $eventID
}
process{
    $recordsetIndex = -1;

    if (-not [string]::IsNullOrEmpty($File))
    {
        if (-not [System.IO.Path]::IsPathRooted($File))
        {
            $File = Join-Path -Path $pwd -ChildPath $File
        }

        [System.IO.FileInfo] $dataFile = new-object System.IO.FileInfo $File
        
        if ($dataFile.Exists) 
        {
            Write-Verbose $dataFile
            GetSqlBatchesFromFile $dataFile | % { . ExecuteBatch $cnn $_ $Parameters $IncludeRecordSetIndex $IncludeRecordsCount $ExecutionTimeout $SqlTransaction}
        }
    }

    if ([string]::IsNullOrEmpty($Query) -eq $false)
    {
        GetSqlBatchesFromString $Query | % { . ExecuteBatch $cnn $_ $Parameters $IncludeRecordSetIndex $IncludeRecordsCount $ExecutionTimeout $SqlTransaction}
    }

}
end {

    if ($eventID -ne $null)
    {
        WriteConnectionEvents $eventID
        Unregister-Event -SourceIdentifier $eventID
    }

    if ($Connection -eq $null)
    {
        if ($cnn -ne $null)
        {
            $cnn.Close()
        }
    }
}

<#
.DESCRIPTION
    Executes T-SQL Query on the target server. 
    This command accepts either query string or query file or both. 
    You can pass either –Server (with optional –Credential and/or –Database) or –Connection object.
    To create connection object, you can use New-SqlConnection command.
    Invoke-SqlQuery will execute specified query or queries, using GO as query separator and return strongly types results.
    Invoke-SqlQuery supports parameterized queries. In this case you should pass parameters as a dictionary using –Parameters parameter. 


.PARAMETER  Query
    T-SQL query text. You can pass multiple queries, separated by GO statement, like in sqlcmd and Sql Server Management Studio.
.PARAMETER  File
    File (usually .sql) containing one or more T-SQL queries

.PARAMETER  Parameters
    Parameters for parameterized query
.PARAMETER Server
	Server name
.PARAMETER Database
	Database name (optional. default value ‘master’)
.PARAMETER Credential
	Credentials for Sql Authentication. If not specified, Windows Authentication will be used
.PARAMETER Connection
	SqlConnection object. You can Create one using New-SqlConnection command	.PARAMETER SqlTRansaction
	Sql Tranaction object.PARAMETER ConnectionTimeout
    Connection timeout
.PARAMETER ExecutionTimeout
    Execution timeout
.PARAMETER IncludeRecordSetIndex 
	Include recordset index for every record. In case of multiple recordsets, that will help to distinguish between them
.PARAMETER IncludeRecordsCount
    Returns number of records at the end of recordset 
.PARAMETER UseTransaction
	Included in Powershell Transaction
.EXAMPLE    
    Invoke-SqlQuery -Query "select * from sys.objects where name = 'sysrowsets'" -Server "."
    name                : sysrowsets
    object_id           : 5
    principal_id        :
    schema_id           : 4
    parent_object_id    : 0
    type                : S
    type_desc           : SYSTEM_TABLE
    create_date         : 7/9/2008 4:19:59 PM
    modify_date         : 7/9/2008 4:19:59 PM
    is_ms_shipped       : True
    is_published        : False
    is_schema_published : False

.EXAMPLE    
    Invoke-SqlQuery -IncludeRecordsCount -Query "select * from sys.objects where name = 'sysrowsets'" -Server "." 
    name                : sysrowsets
    object_id           : 5
    principal_id        :
    schema_id           : 4
    parent_object_id    : 0
    type                : S
    type_desc           : SYSTEM_TABLE
    create_date         : 7/9/2008 4:19:59 PM
    modify_date         : 7/9/2008 4:19:59 PM
    is_ms_shipped       : True
    is_published        : False
    is_schema_published : False
    (1 row(s) affected)

.EXAMPLE    
    Invoke-SqlQuery -Query "select name, type from sys.objects where name = 'sysrowsets'; select 1;" -Server "." -IncludeRecordSetIndex | Format-List
    name           : sysrowsets
    type           : S
    RecordSetIndex : 0
    Column#0       : 1
    RecordSetIndex : 1

.EXAMPLE    
    Invoke-SqlQuery -Query "select name from sys.objects where name = @Name" -Server "." -Parameter (@{Name='sysrowsets'})
    Name
    ----
    sysrowsets

.OUTPUTS
    Strongly-typed recordsets

.LINK
    http://powershell4sql.codeplex.com

#>
}



