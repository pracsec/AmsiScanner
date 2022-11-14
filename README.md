# AmsiScanner

Scan files with the Anti-Malware Scan Interface (AMSI) and minimize malicious text files and PowerShell scripts to see what the Antivirus is flagging as malicious.

**Features:**
- Scan binary and text files with the installed AV to see if they are malicious.
- Find the smallest subset of text that is still flagged as malicious by the installed AV.
- Find the smallest subset of PowerShell tokens that are still flagged as malicious by the installed AV.
- Encrypt files to mitigate accidental exection and detection by the installed AV.
- Read files encrypted with this utility.

# Dependencies

- Windows 10+
- .NET 6.0

# AmsiScanner.exe Usage

## Scan a File

Any type of file can be scanned with AmsiScanner; however, the type of text encoding may cause issues with some Antivirus engines. By default, Unicode encoding is used for text like files.

### Syntax

```
Description:
  Use the Anti-Malware Scan Interface (AMSI) to determine if the installed AV finds the file malicious.

Usage:
  AmsiScanner scan [<path>] [options]

Arguments:
  <path>  Path to the file to analyze. The file may be a text file, zip file containing a single text file, or a file encrypted by crypt.exe.

Options:
  -?, -h, --help  Show help and usage information

Run the following command to scan one of the encrypted sample files:
```

### Example

```
.\AmsiScanner.exe scan .\samples\00e05857a7e441a0ac139e5d92df642d.bin
```

You should receive the following output with Windows Defender.

```
Detected
```

## Find an Antivirus Signature in a Text File

Find the 1-minimal smallest string from the original file that still flags as malicious by the AV.

### Syntax

```
Description:
  Find the 1-minimal smallest set of characters identified by the installed AV as malicious.

Usage:
  AmsiScanner sigfind char [<path>] [options]

Arguments:
  <path>  Path to the file to analyze. The file may be a text file, zip file containing a single text file, or a file encrypted by crypt.exe.

Options:
  -q, --quiet     Suppresses verbose output.
  -?, -h, --help  Show help and usage information
```

### Example

```
.\AmsiScanner.exe sigfind char .\samples\0f1af4e2b55a4446b16bdf5cb6061dd1.bin
```

You should receive the following output with Windows Defender.

```
[*] Iteration: 0 Steps: 2 StepSize: 69432 Length: 138863 Delta: function Invoke-ReflectivePEInjection  {  <#  .SYNOPSIS    This script has two modes. It can reflectively load a DLL/EXE in to the PowerShell process,   or i...
[*] Iteration: 1 Steps: 2 StepSize: 34716 Length: 69432 Delta: function Invoke-ReflectivePEInjection  {  <#  .SYNOPSIS    This script has two modes. It can reflectively load a DLL/EXE in to the PowerShell process,   or it...
[*] Iteration: 2 Steps: 4 StepSize: 17358 Length: 69432 Delta: function Invoke-ReflectivePEInjection  {  <#  .SYNOPSIS    This script has two modes. It can reflectively load a DLL/EXE in to the PowerShell process,   or it...
[*] Iteration: 3 Steps: 3 StepSize: 17358 Length: 52074 Delta: (64) | Out-Null    ($TypeBuilder.DefineField('Subsystem', $SubSystemType, 'Public')).SetOffset(68) | Out-Null    ($TypeBuilder.DefineField('DllCharacteristics...
[*] Iteration: 4 Steps: 2 StepSize: 8679 Length: 17358 Delta: Hex = "0x{0:X$($ValueSize)}" -f [Int64]$Value #Passing a IntPtr to this doesn't work well. Cast to Int64 first.            return $Hex      }         Function ...
[*] Iteration: 5 Steps: 4 StepSize: 4340 Length: 17358 Delta: Hex = "0x{0:X$($ValueSize)}" -f [Int64]$Value #Passing a IntPtr to this doesn't work well. Cast to Int64 first.            return $Hex      }         Function ...
[*] Iteration: 6 Steps: 3 StepSize: 4340 Length: 13018 Delta:     Function Enable-SeDebugPrivilege   {    Param(    [Parameter(Position = 1, Mandatory = $true)]    [System.Object]    $Win32Functions,        [Parameter(Pos...
[*] Iteration: 7 Steps: 6 StepSize: 2170 Length: 13018 Delta:     Function Enable-SeDebugPrivilege   {    Param(    [Parameter(Position = 1, Mandatory = $true)]    [System.Object]    $Win32Functions,        [Parameter(Pos...
[*] Iteration: 8 Steps: 5 StepSize: 2170 Length: 10848 Delta: teropServices.Marshal]::PtrToStructure($PLuid, [Type]$Win32Types.LUID)    $TokenPrivileges.Privileges.Attributes = $Win32Constants.SE_PRIVILEGE_ENABLED    [Sys...
[*] Iteration: 9 Steps: 4 StepSize: 2170 Length: 8680 Delta: teropServices.Marshal]::PtrToStructure($PLuid, [Type]$Win32Types.LUID)    $TokenPrivileges.Privileges.Attributes = $Win32Constants.SE_PRIVILEGE_ENABLED    [Syst...
[*] Iteration: 10 Steps: 3 StepSize: 2170 Length: 6510 Delta: teropServices.Marshal]::PtrToStructure($PLuid, [Type]$Win32Types.LUID)    $TokenPrivileges.Privileges.Attributes = $Win32Constants.SE_PRIVILEGE_ENABLED    [Sys...
[*] Iteration: 11 Steps: 2 StepSize: 2170 Length: 4340 Delta: teropServices.Marshal]::PtrToStructure($PLuid, [Type]$Win32Types.LUID)    $TokenPrivileges.Privileges.Attributes = $Win32Constants.SE_PRIVILEGE_ENABLED    [Sys...
[*] Iteration: 12 Steps: 4 StepSize: 1085 Length: 4340 Delta: teropServices.Marshal]::PtrToStructure($PLuid, [Type]$Win32Types.LUID)    $TokenPrivileges.Privileges.Attributes = $Win32Constants.SE_PRIVILEGE_ENABLED    [Sys...
[*] Iteration: 13 Steps: 3 StepSize: 1085 Length: 3255 Delta: teropServices.Marshal]::PtrToStructure($PLuid, [Type]$Win32Types.LUID)    $TokenPrivileges.Privileges.Attributes = $Win32Constants.SE_PRIVILEGE_ENABLED    [Sys...
[*] Iteration: 14 Steps: 2 StepSize: 1085 Length: 2170 Delta:  [IntPtr]    $ArgumentPtr = [IntPtr]::Zero,        [Parameter(Position = 4, Mandatory = $true)]    [System.Object]    $Win32Functions    )        [IntPtr]$Remo...
[*] Iteration: 15 Steps: 4 StepSize: 543 Length: 2170 Delta:  [IntPtr]    $ArgumentPtr = [IntPtr]::Zero,        [Parameter(Position = 4, Mandatory = $true)]    [System.Object]    $Win32Functions    )        [IntPtr]$Remot...
[*] Iteration: 16 Steps: 3 StepSize: 543 Length: 1627 Delta: dHandle, 0x1FFFFF, [IntPtr]::Zero, $ProcessHandle, $StartAddress, $ArgumentPtr, $false, 0, 0xffff, 0xffff, [IntPtr]::Zero)     $LastError = [System.Runtime.Inte...
[*] Iteration: 17 Steps: 2 StepSize: 543 Length: 1086 Delta: dHandle, 0x1FFFFF, [IntPtr]::Zero, $ProcessHandle, $StartAddress, $ArgumentPtr, $false, 0, 0xffff, 0xffff, [IntPtr]::Zero)     $LastError = [System.Runtime.Inte...
[*] Iteration: 18 Steps: 4 StepSize: 272 Length: 1086 Delta: dHandle, 0x1FFFFF, [IntPtr]::Zero, $ProcessHandle, $StartAddress, $ArgumentPtr, $false, 0, 0xffff, 0xffff, [IntPtr]::Zero)     $LastError = [System.Runtime.Inte...
[*] Iteration: 19 Steps: 3 StepSize: 272 Length: 816 Delta: dHandle, 0x1FFFFF, [IntPtr]::Zero, $ProcessHandle, $StartAddress, $ArgumentPtr, $false, 0, 0xffff, 0xffff, [IntPtr]::Zero)     $LastError = [System.Runtime.Inter...
[*] Iteration: 20 Steps: 2 StepSize: 272 Length: 544 Delta: r in NtCreateThreadEx. Return value: $RetVal. LastError: $LastError"     }    }    #XP/Win8    else    {     #Write-Verbose "Windows XP/8 detected, using CreateR...
[*] Iteration: 21 Steps: 4 StepSize: 136 Length: 544 Delta: r in NtCreateThreadEx. Return value: $RetVal. LastError: $LastError"     }    }    #XP/Win8    else    {     #Write-Verbose "Windows XP/8 detected, using CreateR...
[*] Iteration: 22 Steps: 3 StepSize: 136 Length: 408 Delta: 8 detected, using CreateRemoteThread. Address of thread: $StartAddress"     $RemoteThreadHandle = $Win32Functions.CreateRemoteThread.Invoke($ProcessHandle, [IntP...
[*] Iteration: 23 Steps: 2 StepSize: 136 Length: 272 Delta: 8 detected, using CreateRemoteThread. Address of thread: $StartAddress"     $RemoteThreadHandle = $Win32Functions.CreateRemoteThread.Invoke($ProcessHandle, [IntP...
[*] Iteration: 24 Steps: 4 StepSize: 68 Length: 272 Delta: 8 detected, using CreateRemoteThread. Address of thread: $StartAddress"     $RemoteThreadHandle = $Win32Functions.CreateRemoteThread.Invoke($ProcessHandle, [IntPt...
[*] Iteration: 25 Steps: 8 StepSize: 34 Length: 272 Delta: 8 detected, using CreateRemoteThread. Address of thread: $StartAddress"     $RemoteThreadHandle = $Win32Functions.CreateRemoteThread.Invoke($ProcessHandle, [IntPt...
[*] Iteration: 26 Steps: 7 StepSize: 34 Length: 238 Delta: ad. Address of thread: $StartAddress"     $RemoteThreadHandle = $Win32Functions.CreateRemoteThread.Invoke($ProcessHandle, [IntPtr]::Zero, [UIntPtr][UInt64]0xFFFF,...
[*] Iteration: 27 Steps: 6 StepSize: 34 Length: 204 Delta: ss"     $RemoteThreadHandle = $Win32Functions.CreateRemoteThread.Invoke($ProcessHandle, [IntPtr]::Zero, [UIntPtr][UInt64]0xFFFF, $StartAddress, $ArgumentPtr, 0, [...
[*] Iteration: 28 Steps: 5 StepSize: 34 Length: 170 Delta: 32Functions.CreateRemoteThread.Invoke($ProcessHandle, [IntPtr]::Zero, [UIntPtr][UInt64]0xFFFF, $StartAddress, $ArgumentPtr, 0, [IntPtr]::Zero)    }        if ($Re...
[*] Iteration: 29 Steps: 4 StepSize: 34 Length: 136 Delta: 32Functions.CreateRemoteThread.Invoke($ProcessHandle, [IntPtr]::Zero, [UIntPtr][UInt64]0xFFFF, $StartAddress, $ArgumentPtr, 0, [IntPtr]:
[*] Iteration: 30 Steps: 8 StepSize: 17 Length: 136 Delta: 32Functions.CreateRemoteThread.Invoke($ProcessHandle, [IntPtr]::Zero, [UIntPtr][UInt64]0xFFFF, $StartAddress, $ArgumentPtr, 0, [IntPtr]:
[*] Iteration: 31 Steps: 16 StepSize: 9 Length: 136 Delta: 32Functions.CreateRemoteThread.Invoke($ProcessHandle, [IntPtr]::Zero, [UIntPtr][UInt64]0xFFFF, $StartAddress, $ArgumentPtr, 0, [IntPtr]:
[*] Iteration: 32 Steps: 15 StepSize: 9 Length: 127 Delta: 32Functions.CreateRemoteThread.Invoke($ProcessHandle, [IntPtr]::Zero, [UIntPtr][UInt64]0xFFFF, $StartAddress, $ArgumentPtr, 0,:
[*] Iteration: 33 Steps: 14 StepSize: 9 Length: 126 Delta: 32Functions.CreateRemoteThread.Invoke($ProcessHandle, [IntPtr]::Zero, [UIntPtr][UInt64]0xFFFF, $StartAddress, $ArgumentPtr, 0,
[*] Iteration: 34 Steps: 13 StepSize: 9 Length: 117 Delta: ns.CreateRemoteThread.Invoke($ProcessHandle, [IntPtr]::Zero, [UIntPtr][UInt64]0xFFFF, $StartAddress, $ArgumentPtr, 0,
[*] Iteration: 35 Steps: 26 StepSize: 5 Length: 117 Delta: ns.CreateRemoteThread.Invoke($ProcessHandle, [IntPtr]::Zero, [UIntPtr][UInt64]0xFFFF, $StartAddress, $ArgumentPtr, 0,
[*] Iteration: 36 Steps: 25 StepSize: 5 Length: 112 Delta: ns.CreateRemoteThread.Invoke($ssHandle, [IntPtr]::Zero, [UIntPtr][UInt64]0xFFFF, $StartAddress, $ArgumentPtr, 0,
[*] Iteration: 37 Steps: 24 StepSize: 5 Length: 107 Delta: ns.CreateRemoteThread.Invoke($dle, [IntPtr]::Zero, [UIntPtr][UInt64]0xFFFF, $StartAddress, $ArgumentPtr, 0,
[*] Iteration: 38 Steps: 48 StepSize: 3 Length: 107 Delta: ns.CreateRemoteThread.Invoke($dle, [IntPtr]::Zero, [UIntPtr][UInt64]0xFFFF, $StartAddress, $ArgumentPtr, 0,
[*] Iteration: 39 Steps: 47 StepSize: 3 Length: 104 Delta: ns.CreateRemoteThread.Invoke($, [IntPtr]::Zero, [UIntPtr][UInt64]0xFFFF, $StartAddress, $ArgumentPtr, 0,
[*] Iteration: 40 Steps: 94 StepSize: 2 Length: 104 Delta: ns.CreateRemoteThread.Invoke($, [IntPtr]::Zero, [UIntPtr][UInt64]0xFFFF, $StartAddress, $ArgumentPtr, 0,
[*] Iteration: 41 Steps: 93 StepSize: 2 Length: 102 Delta: .CreateRemoteThread.Invoke($, [IntPtr]::Zero, [UIntPtr][UInt64]0xFFFF, $StartAddress, $ArgumentPtr, 0,
[*] Iteration: 42 Steps: 102 StepSize: 1 Length: 102 Delta: .CreateRemoteThread.Invoke($, [IntPtr]::Zero, [UIntPtr][UInt64]0xFFFF, $StartAddress, $ArgumentPtr, 0,
[*] Iteration: 43 Steps: 101 StepSize: 1 Length: 101 Delta: .CreateRemoteThread.Invoke($,[IntPtr]::Zero, [UIntPtr][UInt64]0xFFFF, $StartAddress, $ArgumentPtr, 0,
[*] Iteration: 44 Steps: 100 StepSize: 1 Length: 100 Delta: .CreateRemoteThread.Invoke($,[IntPtr]::Zero,[UIntPtr][UInt64]0xFFFF, $StartAddress, $ArgumentPtr, 0,
[*] Iteration: 45 Steps: 99 StepSize: 1 Length: 99 Delta: .CreateRemoteThread.Invoke($,[IntPtr]::Zero,[UIntPtr][UInt64]0xFFFF,$StartAddress, $ArgumentPtr, 0,
[*] Iteration: 46 Steps: 98 StepSize: 1 Length: 98 Delta: .CreateRemoteThread.Invoke($,[IntPtr]::Zero,[UIntPtr][UInt64]0xFFFF,$StartAddress,$ArgumentPtr, 0,
[*] Iteration: 47 Steps: 97 StepSize: 1 Length: 97 Delta: .CreateRemoteThread.Invoke($,[IntPtr]::Zero,[UIntPtr][UInt64]0xFFFF,$StartAddress,$ArgumentPtr,0,
[*] Iteration: 48 Steps: 96 StepSize: 1 Length: 96 Delta: .CreateRemoteThread.Invoke($,[IntPtr]::Zero,[UIntPtr][UInt64]0xFFFF,$StartAddress,$ArgumentPtr,0
[*] Signature finding complete.
[*] Time: 5 seconds 10 milliseconds
[*] AmsiCalls: 532
[*] Characters: 96
[*] Reduced String:
.CreateRemoteThread.Invoke($,[IntPtr]::Zero,[UIntPtr][UInt64]0xFFFF,$StartAddress,$ArgumentPtr,0
```

The program was able to reduce a 138K character script to just the 96 characters that are being flagged as malicious by the installed AV in approximately 5 seconds. Note that if any character was removed from this string, it would no longer flag as malicious.

## Find an Antivirus Signature in a PowerShell Script

Find the 1-minimal smallest set of PowerShell tokens from the original script that still flags as malicious by the AV.

### Syntax

```
Description:
  Find the 1-minimal smallest set of PowerShell tokens identified by the installed AV as malicious.

Usage:
  AmsiScanner sigfind token [<path>] [options]

Arguments:
  <path>  Path to the file to analyze. The file may be a text file, zip file containing a single text file, or a file encrypted by crypt.exe.

Options:
  -q, --quiet     Suppresses verbose output.
  -?, -h, --help  Show help and usage information
```

### Example

```
.\AmsiScanner.exe sigfind token .\samples\0f1af4e2b55a4446b16bdf5cb6061dd1.bin
```

You should receive the following output with Windows Defender.

```
[*] Iteration: 0 Steps: 2 StepSize: 12776 Length: 25552 Delta: function Invoke-ReflectivePEInjection  {  <#  .SYNOPSIS    This script has two modes. It can reflectively load a DLL/EXE in to the PowerShell process,   or it...
[*] Iteration: 1 Steps: 2 StepSize: 6388 Length: 12776 Delta: function Invoke-ReflectivePEInjection  {  <#  .SYNOPSIS    This script has two modes. It can reflectively load a DLL/EXE in to the PowerShell process,   or it ...
[*] Iteration: 2 Steps: 4 StepSize: 3194 Length: 12776 Delta: function Invoke-ReflectivePEInjection  {  <#  .SYNOPSIS    This script has two modes. It can reflectively load a DLL/EXE in to the PowerShell process,   or it ...
[*] Iteration: 3 Steps: 2 StepSize: 1597 Length: 3194 Delta: VirtualAlloc -Value $VirtualAlloc        $VirtualAllocExAddr = Get-ProcAddress kernel32.dll VirtualAllocEx    $VirtualAllocExDelegate = Get-DelegateType @([IntP...
[*] Iteration: 4 Steps: 2 StepSize: 799 Length: 1597 Delta: VirtualAlloc -Value $VirtualAlloc        $VirtualAllocExAddr = Get-ProcAddress kernel32.dll VirtualAllocEx    $VirtualAllocExDelegate = Get-DelegateType @([IntPt...
[*] Iteration: 5 Steps: 2 StepSize: 400 Length: 799 Delta: VirtualAlloc -Value $VirtualAlloc        $VirtualAllocExAddr = Get-ProcAddress kernel32.dll VirtualAllocEx    $VirtualAllocExDelegate = Get-DelegateType @([IntPtr...
[*] Iteration: 6 Steps: 4 StepSize: 200 Length: 799 Delta: VirtualAlloc -Value $VirtualAlloc        $VirtualAllocExAddr = Get-ProcAddress kernel32.dll VirtualAllocEx    $VirtualAllocExDelegate = Get-DelegateType @([IntPtr...
[*] Iteration: 7 Steps: 3 StepSize: 200 Length: 599 Delta: Add-Member -MemberType NoteProperty -Name memset -Value $memset        $LoadLibraryAddr = Get-ProcAddress kernel32.dll LoadLibraryA    $LoadLibraryDelegate = Get-...
[*] Iteration: 8 Steps: 2 StepSize: 100 Length: 199 Delta: $Win32Functions | Add-Member NoteProperty -Name VirtualProtect -Value $VirtualProtect        $GetModuleHandleAddr = Get-ProcAddress kernel32.dll GetModuleHandleA ...
[*] Iteration: 9 Steps: 4 StepSize: 50 Length: 199 Delta: $Win32Functions | Add-Member NoteProperty -Name VirtualProtect -Value $VirtualProtect        $GetModuleHandleAddr = Get-ProcAddress kernel32.dll GetModuleHandleA  ...
[*] Iteration: 10 Steps: 3 StepSize: 50 Length: 150 Delta: $Win32Functions | Add-Member NoteProperty -Name VirtualProtect -Value $VirtualProtect        $GetModuleHandleAddr = Get-ProcAddress kernel32.dll GetModuleHandleA ...
[*] Iteration: 11 Steps: 2 StepSize: 50 Length: 100 Delta: $Win32Functions | Add-Member NoteProperty -Name VirtualProtect -Value $VirtualProtect        $GetModuleHandleAddr = Get-ProcAddress kernel32.dll GetModuleHandleA ...
[*] Iteration: 12 Steps: 2 StepSize: 25 Length: 50 Delta: $Win32Functions | Add-Member NoteProperty -Name VirtualProtect -Value $VirtualProtect        $GetModuleHandleAddr = Get-ProcAddress kernel32.dll GetModuleHandleA  ...
[*] Iteration: 13 Steps: 2 StepSize: 13 Length: 25 Delta: $Win32Functions | Add-Member NoteProperty -Name VirtualProtect -Value $VirtualProtect        $GetModuleHandleAddr = Get-ProcAddress
[*] Iteration: 14 Steps: 4 StepSize: 7 Length: 25 Delta: $Win32Functions | Add-Member NoteProperty -Name VirtualProtect -Value $VirtualProtect        $GetModuleHandleAddr = Get-ProcAddress
[*] Iteration: 15 Steps: 3 StepSize: 7 Length: 21 Delta: $Win32Functions | Add-Member NoteProperty -Name VirtualProtect -Value $VirtualProtect        $GetModuleHandleAddr
[*] Iteration: 16 Steps: 6 StepSize: 4 Length: 21 Delta: $Win32Functions | Add-Member NoteProperty -Name VirtualProtect -Value $VirtualProtect        $GetModuleHandleAddr
[*] Iteration: 17 Steps: 5 StepSize: 4 Length: 17 Delta: Add-Member NoteProperty -Name VirtualProtect -Value $VirtualProtect        $GetModuleHandleAddr
[*] Iteration: 18 Steps: 4 StepSize: 4 Length: 16 Delta: Add-Member NoteProperty -Name VirtualProtect -Value $VirtualProtect        $GetModuleHandleAddr
[*] Iteration: 19 Steps: 3 StepSize: 4 Length: 12 Delta: Add-Member NoteProperty -Name VirtualProtect -Value $VirtualProtect
[*] Iteration: 20 Steps: 6 StepSize: 2 Length: 12 Delta: Add-Member NoteProperty -Name VirtualProtect -Value $VirtualProtect
[*] Iteration: 21 Steps: 12 StepSize: 1 Length: 12 Delta: Add-Member NoteProperty -Name VirtualProtect -Value $VirtualProtect
[*] Iteration: 22 Steps: 11 StepSize: 1 Length: 11 Delta: Add-Member NoteProperty -Name VirtualProtect -Value$VirtualProtect
[*] Iteration: 23 Steps: 10 StepSize: 1 Length: 10 Delta: Add-Member NoteProperty -Name VirtualProtect-Value$VirtualProtect
[*] Iteration: 24 Steps: 9 StepSize: 1 Length: 9 Delta: Add-Member NoteProperty -NameVirtualProtect-Value$VirtualProtect
[*] Iteration: 25 Steps: 8 StepSize: 1 Length: 8 Delta: Add-Member NoteProperty-NameVirtualProtect-Value$VirtualProtect
[*] Iteration: 26 Steps: 7 StepSize: 1 Length: 7 Delta: Add-MemberNoteProperty-NameVirtualProtect-Value$VirtualProtect
[*] Iteration: 27 Steps: 6 StepSize: 1 Length: 6 Delta: Add-MemberNoteProperty-NameVirtualProtect-Value$VirtualProtect
[*] Signature finding complete.
[*] Time: 1 second 895 milliseconds
[*] AmsiCalls: 80
[*] Tokens: 6
[*] Characters: 62
[*] Reduced String:
Add-MemberNoteProperty-NameVirtualProtect-Value$VirtualProtect

[*] Reduced Tokens:
Index: 0 Type: Command Start: 41129 Content: Add-Member
Index: 1 Type: CommandArgument Start: 41140 Content: NoteProperty
Index: 2 Type: CommandParameter Start: 41153 Content: -Name
Index: 3 Type: CommandArgument Start: 41159 Content: VirtualProtect
Index: 4 Type: CommandParameter Start: 41174 Content: -Value
Index: 5 Type: Variable Start: 41181 Content: $VirtualProtect
```

The program was able to reduce a 25K token script to just the 6 tokens that are being flagged as malicious by the installed AV in approximately 5 seconds. Note that if any token was removed from this string, it would no longer flag as malicious.

It is also worth noting that the string found with this method is different than that found by the character minimization method indicating that there are likely multiple AV signatures for this one script.

# PowerShell Cmdlet Usage

# Importing the Module

Before you can call the cmdlets provided by this project, you first need to import the module with the following command:

```powershell
Import-Module AmsiScanner.Common.dll
```

## Scan a File

Any type of file can be scanned with AmsiScanner; however, the type of text encoding may cause issues with some Antivirus engines. By default, Unicode encoding is used for text like files.

### Syntax

```
NAME
    Invoke-AmsiScan

SYNTAX
    Invoke-AmsiScan [-File] <FileInfo>  [<CommonParameters>]

    Invoke-AmsiScan [-Text] <string>  [<CommonParameters>]

    Invoke-AmsiScan [-Bytes] <byte[]>  [<CommonParameters>]
```

### Example - Scan a File

```
Invoke-AmisScan .\samples\00e05857a7e441a0ac139e5d92df642d.bin
```

You should receive the following output with Windows Defender.

```
Detected
```

### Example - Scan a String Object

```
$text = [AmsiScanner.Common.Utility]::ReadAllText("C:\Users\helpdesk\Desktop\Workspace\repos\amsiscanner\src\release\samples\00e05857a7e441a0ac139e5d92df642d.bin")
Invoke-AmsiScan $text
```

You should receive the following output with Windows Defender.

```
Detected
```

## Find an Antivirus Signature in a Text File

Find the 1-minimal smallest string from the original file that still flags as malicious by the AV.

### Syntax

```
NAME
    Invoke-Sigfind

SYNTAX
    Invoke-Sigfind [-File] <FileInfo> [-Type {Char | Token}] [-MinimumDeltaLength <int>]  [<CommonParameters>]

    Invoke-Sigfind [-Text] <string> [-Type {Char | Token}] [-MinimumDeltaLength <int>]  [<CommonParameters>]
```

### Example

```
Invoke-Sigfind .\samples\0f1af4e2b55a4446b16bdf5cb6061dd1.bin -Verbose
```

You should receive the following output with Windows Defender.

```
VERBOSE: Iteration: 0 Steps: 2 StepSize: 69432 Length: 138863 Delta: function Invoke-ReflectivePEInjection  {  <#  .SYNOPSIS    This script ...
VERBOSE: Iteration: 1 Steps: 2 StepSize: 34716 Length: 69432 Delta: function Invoke-ReflectivePEInjection  {  <#  .SYNOPSIS    This script h...
VERBOSE: Iteration: 2 Steps: 4 StepSize: 17358 Length: 69432 Delta: function Invoke-ReflectivePEInjection  {  <#  .SYNOPSIS    This script h...
VERBOSE: Iteration: 3 Steps: 3 StepSize: 17358 Length: 52074 Delta: (64) | Out-Null    ($TypeBuilder.DefineField('Subsystem', $SubSystemType...
VERBOSE: Iteration: 4 Steps: 2 StepSize: 8679 Length: 17358 Delta: Hex = "0x{0:X$($ValueSize)}" -f [Int64]$Value #Passing a IntPtr to this d...
VERBOSE: Iteration: 5 Steps: 4 StepSize: 4340 Length: 17358 Delta: Hex = "0x{0:X$($ValueSize)}" -f [Int64]$Value #Passing a IntPtr to this d...
VERBOSE: Iteration: 6 Steps: 3 StepSize: 4340 Length: 13018 Delta:     Function Enable-SeDebugPrivilege   {    Param(    [Parameter(Position...
VERBOSE: Iteration: 7 Steps: 6 StepSize: 2170 Length: 13018 Delta:     Function Enable-SeDebugPrivilege   {    Param(    [Parameter(Position...
VERBOSE: Iteration: 8 Steps: 5 StepSize: 2170 Length: 10848 Delta: teropServices.Marshal]::PtrToStructure($PLuid, [Type]$Win32Types.LUID)   ...
VERBOSE: Iteration: 9 Steps: 4 StepSize: 2170 Length: 8680 Delta: teropServices.Marshal]::PtrToStructure($PLuid, [Type]$Win32Types.LUID)    ...
VERBOSE: Iteration: 10 Steps: 3 StepSize: 2170 Length: 6510 Delta: teropServices.Marshal]::PtrToStructure($PLuid, [Type]$Win32Types.LUID)   ...
VERBOSE: Iteration: 11 Steps: 2 StepSize: 2170 Length: 4340 Delta: teropServices.Marshal]::PtrToStructure($PLuid, [Type]$Win32Types.LUID)   ...
VERBOSE: Iteration: 12 Steps: 4 StepSize: 1085 Length: 4340 Delta: teropServices.Marshal]::PtrToStructure($PLuid, [Type]$Win32Types.LUID)   ...
VERBOSE: Iteration: 13 Steps: 3 StepSize: 1085 Length: 3255 Delta: teropServices.Marshal]::PtrToStructure($PLuid, [Type]$Win32Types.LUID)   ...
VERBOSE: Iteration: 14 Steps: 2 StepSize: 1085 Length: 2170 Delta:  [IntPtr]    $ArgumentPtr = [IntPtr]::Zero,        [Parameter(Position = ...
VERBOSE: Iteration: 15 Steps: 4 StepSize: 543 Length: 2170 Delta:  [IntPtr]    $ArgumentPtr = [IntPtr]::Zero,        [Parameter(Position = 4...
VERBOSE: Iteration: 16 Steps: 3 StepSize: 543 Length: 1627 Delta: dHandle, 0x1FFFFF, [IntPtr]::Zero, $ProcessHandle, $StartAddress, $Argumen...
VERBOSE: Iteration: 17 Steps: 2 StepSize: 543 Length: 1086 Delta: dHandle, 0x1FFFFF, [IntPtr]::Zero, $ProcessHandle, $StartAddress, $Argumen...
VERBOSE: Iteration: 18 Steps: 4 StepSize: 272 Length: 1086 Delta: dHandle, 0x1FFFFF, [IntPtr]::Zero, $ProcessHandle, $StartAddress, $Argumen...
VERBOSE: Iteration: 19 Steps: 3 StepSize: 272 Length: 816 Delta: dHandle, 0x1FFFFF, [IntPtr]::Zero, $ProcessHandle, $StartAddress, $Argument...
VERBOSE: Iteration: 20 Steps: 2 StepSize: 272 Length: 544 Delta: r in NtCreateThreadEx. Return value: $RetVal. LastError: $LastError"     } ...
VERBOSE: Iteration: 21 Steps: 4 StepSize: 136 Length: 544 Delta: r in NtCreateThreadEx. Return value: $RetVal. LastError: $LastError"     } ...
VERBOSE: Iteration: 22 Steps: 3 StepSize: 136 Length: 408 Delta: 8 detected, using CreateRemoteThread. Address of thread: $StartAddress"    ...
VERBOSE: Iteration: 23 Steps: 2 StepSize: 136 Length: 272 Delta: 8 detected, using CreateRemoteThread. Address of thread: $StartAddress"    ...
VERBOSE: Iteration: 24 Steps: 4 StepSize: 68 Length: 272 Delta: 8 detected, using CreateRemoteThread. Address of thread: $StartAddress"     ...
VERBOSE: Iteration: 25 Steps: 8 StepSize: 34 Length: 272 Delta: 8 detected, using CreateRemoteThread. Address of thread: $StartAddress"     ...
VERBOSE: Iteration: 26 Steps: 7 StepSize: 34 Length: 238 Delta: ad. Address of thread: $StartAddress"     $RemoteThreadHandle = $Win32Functi...
VERBOSE: Iteration: 27 Steps: 6 StepSize: 34 Length: 204 Delta: ss"     $RemoteThreadHandle = $Win32Functions.CreateRemoteThread.Invoke($Pro...
VERBOSE: Iteration: 28 Steps: 5 StepSize: 34 Length: 170 Delta: 32Functions.CreateRemoteThread.Invoke($ProcessHandle, [IntPtr]::Zero, [UIntP...
VERBOSE: Iteration: 29 Steps: 4 StepSize: 34 Length: 136 Delta: 32Functions.CreateRemoteThread.Invoke($ProcessHandle, [IntPtr]::Zero, [UIntP...
VERBOSE: Iteration: 30 Steps: 8 StepSize: 17 Length: 136 Delta: 32Functions.CreateRemoteThread.Invoke($ProcessHandle, [IntPtr]::Zero, [UIntP...
VERBOSE: Iteration: 31 Steps: 16 StepSize: 9 Length: 136 Delta: 32Functions.CreateRemoteThread.Invoke($ProcessHandle, [IntPtr]::Zero, [UIntP...
VERBOSE: Iteration: 32 Steps: 15 StepSize: 9 Length: 127 Delta: 32Functions.CreateRemoteThread.Invoke($ProcessHandle, [IntPtr]::Zero, [UIntP...
VERBOSE: Iteration: 33 Steps: 14 StepSize: 9 Length: 126 Delta: 32Functions.CreateRemoteThread.Invoke($ProcessHandle, [IntPtr]::Zero, [UIntP...
VERBOSE: Iteration: 34 Steps: 13 StepSize: 9 Length: 117 Delta: ns.CreateRemoteThread.Invoke($ProcessHandle, [IntPtr]::Zero, [UIntPtr][UInt6...
VERBOSE: Iteration: 35 Steps: 26 StepSize: 5 Length: 117 Delta: ns.CreateRemoteThread.Invoke($ProcessHandle, [IntPtr]::Zero, [UIntPtr][UInt6...
VERBOSE: Iteration: 36 Steps: 25 StepSize: 5 Length: 112 Delta: ns.CreateRemoteThread.Invoke($ssHandle, [IntPtr]::Zero, [UIntPtr][UInt64]0xF...
VERBOSE: Iteration: 37 Steps: 24 StepSize: 5 Length: 107 Delta: ns.CreateRemoteThread.Invoke($dle, [IntPtr]::Zero, [UIntPtr][UInt64]0xFFFF, ...
VERBOSE: Iteration: 38 Steps: 48 StepSize: 3 Length: 107 Delta: ns.CreateRemoteThread.Invoke($dle, [IntPtr]::Zero, [UIntPtr][UInt64]0xFFFF, ...
VERBOSE: Iteration: 39 Steps: 47 StepSize: 3 Length: 104 Delta: ns.CreateRemoteThread.Invoke($, [IntPtr]::Zero, [UIntPtr][UInt64]0xFFFF, $St...
VERBOSE: Iteration: 40 Steps: 94 StepSize: 2 Length: 104 Delta: ns.CreateRemoteThread.Invoke($, [IntPtr]::Zero, [UIntPtr][UInt64]0xFFFF, $St...
VERBOSE: Iteration: 41 Steps: 93 StepSize: 2 Length: 102 Delta: .CreateRemoteThread.Invoke($, [IntPtr]::Zero, [UIntPtr][UInt64]0xFFFF, $Star...
VERBOSE: Iteration: 42 Steps: 102 StepSize: 1 Length: 102 Delta: .CreateRemoteThread.Invoke($, [IntPtr]::Zero, [UIntPtr][UInt64]0xFFFF, $Sta...
VERBOSE: Iteration: 43 Steps: 101 StepSize: 1 Length: 101 Delta: .CreateRemoteThread.Invoke($,[IntPtr]::Zero, [UIntPtr][UInt64]0xFFFF, $Star...
VERBOSE: Iteration: 44 Steps: 100 StepSize: 1 Length: 100 Delta: .CreateRemoteThread.Invoke($,[IntPtr]::Zero,[UIntPtr][UInt64]0xFFFF, $Start...
VERBOSE: Iteration: 45 Steps: 99 StepSize: 1 Length: 99 Delta: .CreateRemoteThread.Invoke($,[IntPtr]::Zero,[UIntPtr][UInt64]0xFFFF,$StartAdd...
VERBOSE: Iteration: 46 Steps: 98 StepSize: 1 Length: 98 Delta: .CreateRemoteThread.Invoke($,[IntPtr]::Zero,[UIntPtr][UInt64]0xFFFF,$StartAdd...
VERBOSE: Iteration: 47 Steps: 97 StepSize: 1 Length: 97 Delta: .CreateRemoteThread.Invoke($,[IntPtr]::Zero,[UIntPtr][UInt64]0xFFFF,$StartAdd...
VERBOSE: Iteration: 48 Steps: 96 StepSize: 1 Length: 96 Delta: .CreateRemoteThread.Invoke($,[IntPtr]::Zero,[UIntPtr][UInt64]0xFFFF,$StartAdd...
VERBOSE: Signature finding complete.
VERBOSE: Time: 4 seconds 770 milliseconds
VERBOSE: AmsiCalls: 532
VERBOSE: Characters: 96
VERBOSE: Reduced String:
.CreateRemoteThread.Invoke($,[IntPtr]::Zero,[UIntPtr][UInt64]0xFFFF,$StartAddress,$ArgumentPtr,0
```

## Find an Antivirus Signature in a PowerShell Script

Find the 1-minimal smallest set of PowerShell tokens from the original script that still flags as malicious by the AV.

### Syntax

```
NAME
    Invoke-Sigfind

SYNTAX
    Invoke-Sigfind [-File] <FileInfo> [-Type {Char | Token}] [-MinimumDeltaLength <int>]  [<CommonParameters>]

    Invoke-Sigfind [-Text] <string> [-Type {Char | Token}] [-MinimumDeltaLength <int>]  [<CommonParameters>]
```

### Example

```
Invoke-Sigfind .\samples\0f1af4e2b55a4446b16bdf5cb6061dd1.bin -Type Token -Verbose
```

You should receive the following output with Windows Defender.

```
[*] Iteration: 0 Steps: 2 StepSize: 12776 Length: 25552 Delta: function Invoke-ReflectivePEInjection  {  <#  .SYNOPSIS    This script has two modes. It can reflectively load a DLL/EXE in to the PowerShell process,   or it...
[*] Iteration: 1 Steps: 2 StepSize: 6388 Length: 12776 Delta: function Invoke-ReflectivePEInjection  {  <#  .SYNOPSIS    This script has two modes. It can reflectively load a DLL/EXE in to the PowerShell process,   or it ...
[*] Iteration: 2 Steps: 4 StepSize: 3194 Length: 12776 Delta: function Invoke-ReflectivePEInjection  {  <#  .SYNOPSIS    This script has two modes. It can reflectively load a DLL/EXE in to the PowerShell process,   or it ...
[*] Iteration: 3 Steps: 2 StepSize: 1597 Length: 3194 Delta: VirtualAlloc -Value $VirtualAlloc        $VirtualAllocExAddr = Get-ProcAddress kernel32.dll VirtualAllocEx    $VirtualAllocExDelegate = Get-DelegateType @([IntP...
[*] Iteration: 4 Steps: 2 StepSize: 799 Length: 1597 Delta: VirtualAlloc -Value $VirtualAlloc        $VirtualAllocExAddr = Get-ProcAddress kernel32.dll VirtualAllocEx    $VirtualAllocExDelegate = Get-DelegateType @([IntPt...
[*] Iteration: 5 Steps: 2 StepSize: 400 Length: 799 Delta: VirtualAlloc -Value $VirtualAlloc        $VirtualAllocExAddr = Get-ProcAddress kernel32.dll VirtualAllocEx    $VirtualAllocExDelegate = Get-DelegateType @([IntPtr...
[*] Iteration: 6 Steps: 4 StepSize: 200 Length: 799 Delta: VirtualAlloc -Value $VirtualAlloc        $VirtualAllocExAddr = Get-ProcAddress kernel32.dll VirtualAllocEx    $VirtualAllocExDelegate = Get-DelegateType @([IntPtr...
[*] Iteration: 7 Steps: 3 StepSize: 200 Length: 599 Delta: Add-Member -MemberType NoteProperty -Name memset -Value $memset        $LoadLibraryAddr = Get-ProcAddress kernel32.dll LoadLibraryA    $LoadLibraryDelegate = Get-...
[*] Iteration: 8 Steps: 2 StepSize: 100 Length: 199 Delta: $Win32Functions | Add-Member NoteProperty -Name VirtualProtect -Value $VirtualProtect        $GetModuleHandleAddr = Get-ProcAddress kernel32.dll GetModuleHandleA ...
[*] Iteration: 9 Steps: 4 StepSize: 50 Length: 199 Delta: $Win32Functions | Add-Member NoteProperty -Name VirtualProtect -Value $VirtualProtect        $GetModuleHandleAddr = Get-ProcAddress kernel32.dll GetModuleHandleA  ...
[*] Iteration: 10 Steps: 3 StepSize: 50 Length: 150 Delta: $Win32Functions | Add-Member NoteProperty -Name VirtualProtect -Value $VirtualProtect        $GetModuleHandleAddr = Get-ProcAddress kernel32.dll GetModuleHandleA ...
[*] Iteration: 11 Steps: 2 StepSize: 50 Length: 100 Delta: $Win32Functions | Add-Member NoteProperty -Name VirtualProtect -Value $VirtualProtect        $GetModuleHandleAddr = Get-ProcAddress kernel32.dll GetModuleHandleA ...
[*] Iteration: 12 Steps: 2 StepSize: 25 Length: 50 Delta: $Win32Functions | Add-Member NoteProperty -Name VirtualProtect -Value $VirtualProtect        $GetModuleHandleAddr = Get-ProcAddress kernel32.dll GetModuleHandleA  ...
[*] Iteration: 13 Steps: 2 StepSize: 13 Length: 25 Delta: $Win32Functions | Add-Member NoteProperty -Name VirtualProtect -Value $VirtualProtect        $GetModuleHandleAddr = Get-ProcAddress
[*] Iteration: 14 Steps: 4 StepSize: 7 Length: 25 Delta: $Win32Functions | Add-Member NoteProperty -Name VirtualProtect -Value $VirtualProtect        $GetModuleHandleAddr = Get-ProcAddress
[*] Iteration: 15 Steps: 3 StepSize: 7 Length: 21 Delta: $Win32Functions | Add-Member NoteProperty -Name VirtualProtect -Value $VirtualProtect        $GetModuleHandleAddr
[*] Iteration: 16 Steps: 6 StepSize: 4 Length: 21 Delta: $Win32Functions | Add-Member NoteProperty -Name VirtualProtect -Value $VirtualProtect        $GetModuleHandleAddr
[*] Iteration: 17 Steps: 5 StepSize: 4 Length: 17 Delta: Add-Member NoteProperty -Name VirtualProtect -Value $VirtualProtect        $GetModuleHandleAddr
[*] Iteration: 18 Steps: 4 StepSize: 4 Length: 16 Delta: Add-Member NoteProperty -Name VirtualProtect -Value $VirtualProtect        $GetModuleHandleAddr
[*] Iteration: 19 Steps: 3 StepSize: 4 Length: 12 Delta: Add-Member NoteProperty -Name VirtualProtect -Value $VirtualProtect
[*] Iteration: 20 Steps: 6 StepSize: 2 Length: 12 Delta: Add-Member NoteProperty -Name VirtualProtect -Value $VirtualProtect
[*] Iteration: 21 Steps: 12 StepSize: 1 Length: 12 Delta: Add-Member NoteProperty -Name VirtualProtect -Value $VirtualProtect
[*] Iteration: 22 Steps: 11 StepSize: 1 Length: 11 Delta: Add-Member NoteProperty -Name VirtualProtect -Value$VirtualProtect
[*] Iteration: 23 Steps: 10 StepSize: 1 Length: 10 Delta: Add-Member NoteProperty -Name VirtualProtect-Value$VirtualProtect
[*] Iteration: 24 Steps: 9 StepSize: 1 Length: 9 Delta: Add-Member NoteProperty -NameVirtualProtect-Value$VirtualProtect
[*] Iteration: 25 Steps: 8 StepSize: 1 Length: 8 Delta: Add-Member NoteProperty-NameVirtualProtect-Value$VirtualProtect
[*] Iteration: 26 Steps: 7 StepSize: 1 Length: 7 Delta: Add-MemberNoteProperty-NameVirtualProtect-Value$VirtualProtect
[*] Iteration: 27 Steps: 6 StepSize: 1 Length: 6 Delta: Add-MemberNoteProperty-NameVirtualProtect-Value$VirtualProtect
[*] Signature finding complete.
[*] Time: 1 second 895 milliseconds
[*] AmsiCalls: 80
[*] Tokens: 6
[*] Characters: 62
[*] Reduced String:
ReducedString                                                  ReducedTokens
-------------                                                  -------------
Add-MemberNoteProperty-NameVirtualProtect-Value$VirtualProtect {AmsiScanner.Common.Token, AmsiScanner.Common.Token, AmsiScanner.Common.Token, Ams...
```

This cmdlet returns an object with the reduced string and tokens. The tokens array is nice because it gives you the exact location in the original string where each token was located, making it easier to find. In order to  isualize the data, I recommend assigning the output of Invoke-Sigfind to a variable and then printing the results like in the example below:

```
$result = Invoke-Sigfind .\samples\0f1af4e2b55a4446b16bdf5cb6061dd1.bin -Type Token -Verbose
$result.ReducedTokens | Select Start,TokenType,Contents | ft -a
```

You should get the following output with Windows Defender:

```
Start TokenType        Contents
----- ---------        --------
41129 Command          Add-Member
41140 CommandArgument  NoteProperty
41153 CommandParameter -Name
41159 CommandArgument  VirtualProtect
41174 CommandParameter -Value
41181 Variable         VirtualProtect
```