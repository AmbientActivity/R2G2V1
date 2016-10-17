function Build-VisualStudioSolution            
{            
    param            
    (            
        [parameter(Mandatory=$false)]            
        [ValidateNotNullOrEmpty()]             
        [String] $SourceCodePath = "C:\Users\John\Source\Repos\R2G2V1\",            
            
        [parameter(Mandatory=$false)]            
        [ValidateNotNullOrEmpty()]             
        [String] $SolutionFile,            
                    
        [parameter(Mandatory=$false)]            
        [ValidateNotNullOrEmpty()]             
        [String] $Configuration = "Debug",            
                    
        [parameter(Mandatory=$false)]            
        [ValidateNotNullOrEmpty()]             
        [Boolean] $AutoLaunchBuildLog = $false,            
            
        [parameter(Mandatory=$false)]            
        [ValidateNotNullOrEmpty()]             
        [Switch] $MsBuildHelp,            
                    
        [parameter(Mandatory=$false)]            
        [ValidateNotNullOrEmpty()]             
        [Switch] $CleanFirst,            
                    
        [ValidateNotNullOrEmpty()]             
        [string] $BuildLogFile,            
               
	[ValidateNotNullOrEmpty()]                  
        [string] $BuildLogOutputPath = $env:userprofile + "\Desktop\"            
    )            
                
    process            
    {            
        # Local Variables            
        $MsBuild = "C:\Program Files (x86)\MSBuild\14.0\Bin\MSBuild.exe";            
                
        # Caller requested MsBuild Help?            
        if($MsBuildHelp)            
        {            
                $BuildArgs = @{            
                    FilePath = $MsBuild            
                    ArgumentList = "/help"            
                    Wait = $true            
                    RedirectStandardOutput = "C:\MsBuildHelp.txt"            
                }            
            
                # Get the help info and show            
                Start-Process @BuildArgs            
                Start-Process -verb Open "C:\MsBuildHelp.txt";            
        }            
        else            
        {            
            # Local Variables            
            $SlnFilePath = $SourceCodePath + $SolutionFile;            
            $SlnFileParts = $SolutionFile.Split("\");            
            $SlnFileName = $SlnFileParts[$SlnFileParts.Length - 1];            
            $BuildLog = $BuildLogOutputPath + $BuildLogFile            
            $bOk = $true;            
            $success = $false;
                        
            try            
            {            
                # Clear first?            
                if($CleanFirst)            
                {            
                    # Display Progress
                    #Write-Progress -Id 20275 -Activity $SlnFileName  -Status "Cleaning..." -PercentComplete 10;           
                    Write-Host "Cleaning" $Configuration"..." -NoNewline            
                            
                    $BuildArgs = @{            
                        FilePath = $MsBuild            
                        ArgumentList = $SlnFilePath, "/t:clean", ("/p:Configuration=" + $Configuration), "/v:minimal"            
                        RedirectStandardOutput = $BuildLog            
                        Wait = $true            
                        WindowStyle = "Hidden"            
                    }            
            
                    # Start the build            
                    Start-Process @BuildArgs #| Out-String -stream -width 1024 > $BuildLog
                                
                    # Display Progress            
                    #Write-Progress -Id 20275 -Activity $SlnFileName  -Status "Done cleaning." -PercentComplete 50;
                    Write-Host "done."            
                }            
            
                # Display Progress
                #Write-Progress  -Id 20275 -Activity $SlnFileName  -Status "Building..." -PercentComplete 60;         
                Write-Host "Building" $Configuration"..." -NoNewline
                            
                # Prepare the Args for the actual build            
                $BuildArgs = @{            
                    FilePath = $MsBuild            
                    ArgumentList = $SlnFilePath, "/t:rebuild", ("/p:Configuration=" + $Configuration), "/v:minimal"            
                    RedirectStandardOutput = $BuildLog           
                    Wait = $true            
                    WindowStyle = "Hidden"
                }            
            
                # Start the build            
                Start-Process @BuildArgs #| Out-String -stream -width 1024 > $BuildLog          
                            
                # Display Progress            
                #Write-Progress -Id 20275 -Activity $SlnFileName  -Status "Done building." -PercentComplete 100;            
            }            
            catch            
            {            
                $bOk = $false;            
                Write-Error ("Unexpect error occured while building " + $SlnFileParts[$SlnFileParts.Length - 1] + ": " + $_.Message);            
            }            
                        
            # All good so far?            
            if($bOk)            
            {            
                #Show projects which where built in the solution            
                #Select-String -Path $BuildLog -Pattern "Done building project" -SimpleMatch            
                            
                # Show if build succeeded or failed...            
                #$successes = Select-String -Path $BuildLog -Pattern "Build succeeded." -SimpleMatch            
                $failures = Select-String -Path $BuildLog -Pattern "error" -SimpleMatch            
                            
                if($failures -ne $null)            
                {        
                    Write-Host -foregroundcolor red "failed.";    
                    Write-Host ("`nPlease check the build log $BuildLog for details.`n");
                    $success = $false;        
                }            
                else
                {
                    Write-Host -foregroundcolor green "succeeded.";
                    $success = $true;    
                }
                            
                # Show the build log...            
                if($AutoLaunchBuildLog)            
                {            
                    Start-Process -verb "Open" $BuildLog;  
                }            
            }
            
            Return $success;          
        }            
    }            
                
    <#
        .SYNOPSIS
        Executes the v2.0.50727\MSBuild.exe tool against the specified Visual Studio solution file.
        
        .Description
        
        .PARAMETER SourceCodePath
        The source code root directory. $SolutionFile can be relative to this directory. 
        
        .PARAMETER SolutionFile
        The relative path and filename of the Visual Studio solution file.
        
        .PARAMETER Configuration
        The project configuration to build within the solution file. Default is "Debug".
        
        .PARAMETER AutoLaunchBuildLog
        If true, the build log will be launched into the default viewer. Default is false.
        
        .PARAMETER MsBuildHelp
        If set, this function will run MsBuild requesting the help listing.
        
        .PARAMETER CleanFirst
        If set, this switch will cause the function to first run MsBuild as a "clean" operation, before executing the build.
        
        .PARAMETER BuildLogFile
        The name of the file which will contain the build log after the build completes.
        
        .PARAMETER BuildLogOutputPath
        The full path to the output folder where build log files will be placed. Defaults to the current user's desktop.
        
        .EXAMPLE
        
        .LINK
        http://stackoverflow.com/questions/2560652/why-does-powershell-fail-to-build-my-net-solutions-file-is-being-used-by-anot
        http://geekswithblogs.net/dwdii
        
        .NOTES
        Name:   Build-VisualStudioSolution
        Author: Daniel Dittenhafer
    #>                
}