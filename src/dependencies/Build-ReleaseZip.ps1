function Create-Directory {
    param(
        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
        [string]$Path
    )

    if(![System.IO.Directory]::Exists($Path)) {
        [void][System.IO.Directory]::CreateDirectory($Path);
        while(![System.IO.Directory]::Exists($Path)) {
            Start-Sleep -Milliseconds 5;
        }
    }
}

#Setup paths
$current = New-Object System.IO.DirectoryInfo($PSScriptRoot);
$root = $current.Parent;
$binsrc = [System.IO.Path]::Combine($root.FullName, "AmsiScanner", "bin", "release", "net6.0");
$release = [System.IO.Path]::Combine($root.FullName, "release");
$samples_src = [System.IO.Path]::Combine($current.FullName, "samples");
$samples_dst = [System.IO.Path]::Combine($release, "samples");
$zip = [System.IO.Path]::Combine($root.FullName, "release.zip");

#Delete old contents
Remove-Item $release -Recurse -Force -ErrorAction SilentlyContinue
Remove-Item $zip -Recurse -Force -ErrorAction SilentlyContinue

#Ensure folders exist
Create-Directory $release
Create-Directory $samples_dst

#Copy binaries over
$files = @(
	"$($root.FullName)\AmsiScanner\bin\Release\net6.0\AmsiScanner.Common.dll",
	"$($root.FullName)\AmsiScanner\bin\Release\net6.0\AmsiScanner.dll",
	"$($root.FullName)\AmsiScanner\bin\Release\net6.0\AmsiScanner.exe",
    "$($root.FullName)\AmsiScanner\bin\Release\net6.0\AmsiScanner.runtimeconfig.json",
	"$($root.FullName)\AmsiScanner\bin\Release\net6.0\Microsoft.ApplicationInsights.dll",
	"$($root.FullName)\AmsiScanner\bin\Release\net6.0\Microsoft.PowerShell.MarkdownRender.dll",
	"$($root.FullName)\AmsiScanner\bin\Release\net6.0\Newtonsoft.Json.dll",
	"$($root.FullName)\AmsiScanner\bin\Release\net6.0\runtimes\win\lib\net6.0\Microsoft.PowerShell.Commands.Diagnostics.dll",
	"$($root.FullName)\AmsiScanner\bin\Release\net6.0\runtimes\win\lib\net6.0\Microsoft.PowerShell.Commands.Management.dll",
	"$($root.FullName)\AmsiScanner\bin\Release\net6.0\runtimes\win\lib\net6.0\Microsoft.PowerShell.Commands.Utility.dll",
	"$($root.FullName)\AmsiScanner\bin\Release\net6.0\runtimes\win\lib\net6.0\Microsoft.PowerShell.ConsoleHost.dll",
	"$($root.FullName)\AmsiScanner\bin\Release\net6.0\runtimes\win\lib\net6.0\Microsoft.PowerShell.CoreCLR.Eventing.dll",
	"$($root.FullName)\AmsiScanner\bin\Release\net6.0\runtimes\win\lib\net6.0\Microsoft.PowerShell.Security.dll",
	"$($root.FullName)\AmsiScanner\bin\Release\net6.0\runtimes\win\lib\net6.0\Microsoft.WSMan.Management.dll",
	"$($root.FullName)\AmsiScanner\bin\Release\net6.0\runtimes\win\lib\net6.0\Microsoft.WSMan.Runtime.dll",
	"$($root.FullName)\AmsiScanner\bin\Release\net6.0\runtimes\win\lib\net6.0\System.DirectoryServices.dll",
	"$($root.FullName)\AmsiScanner\bin\Release\net6.0\runtimes\win\lib\net6.0\System.Management.Automation.dll",
	"$($root.FullName)\AmsiScanner\bin\Release\net6.0\runtimes\win\lib\net6.0\System.Management.dll",
	"$($root.FullName)\AmsiScanner\bin\Release\net6.0\runtimes\win10-x64\lib\netstandard1.6\Microsoft.Management.Infrastructure.dll",
	"$($root.FullName)\AmsiScanner\bin\Release\net6.0\System.CommandLine.dll",
	"$($root.FullName)\AmsiScanner\bin\Release\net6.0\System.CommandLine.NamingConventionBinder.dll"
);

foreach($file in $files) {
    Copy-Item $file $release -Force;
}
#Copy samples over
$files = gci $samples_src -File
foreach($file in $files) {
    Copy-Item $file.FullName $samples_dst
}

#Compress the release folder and save it
Get-ChildItem -Path $release | Compress-Archive -DestinationPath $zip -CompressionLevel Optimal -Force;