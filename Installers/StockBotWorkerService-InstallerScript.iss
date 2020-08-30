; -- 64BitTwoArch.iss --
; Demonstrates how to install a program built for two different
; architectures (x86 and x64) using a single installer: on a "x86"
; edition of Windows the x86 version of the program will be
; installed but on a "x64" edition of Windows the x64 version will
; be installed.

; SEE THE DOCUMENTATION FOR DETAILS ON CREATING .ISS SCRIPT FILES!

[Setup]
AppName=StockBotWorkerService
AppVersion=1.0
DefaultDirName={autopf}\StockBotWorkerService
DefaultGroupName=StockBotWorkerService
UninstallDisplayIcon={app}\StockBotWorkerService.exe
WizardStyle=modern
Compression=lzma2
SolidCompression=yes
OutputDir=userdocs:Inno Setup Examples Output
; "ArchitecturesInstallIn64BitMode=x64" requests that the install be
; done in "64-bit mode" on x64, meaning it should use the native
; 64-bit Program Files directory and the 64-bit view of the registry.
; On all other architectures it will install in "32-bit mode".
ArchitecturesInstallIn64BitMode=x64
; Note: We don't set ProcessorsAllowed because we want this
; installation to run on all architectures (including Itanium,
; since it's capable of running 32-bit code too).


[Files]
Source: "C:\Users\cesarjaramillo\source\repos\ChatWebApp\StockBotWorkerService\bin\Release\netcoreapp3.1\publish\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs

[Icons]
Name: "{group}\StockBotWorkerService"; Filename: "{app}\StockBotWorkerService.exe"

[run]
Filename: {app}\nssm.exe; Parameters: "install StockBotWorkerService ""C:\Program Files\StockBotWorkerService\StockBotWorkerService.exe""" ; Flags: runhidden
Filename: {app}\nssm.exe; Parameters: "set StockBotWorkerService Description ""Bot to get stock quotes""" ; Flags: runhidden
Filename: {app}\nssm.exe; Parameters: "start StockBotWorkerService" ; Flags: runhidden

[UninstallRun]
Filename: {app}\nssm.exe; Parameters: "stop StockBotWorkerService" ; Flags: runhidden
Filename: {app}\nssm.exe; Parameters: "remove StockBotWorkerService confirm" ; Flags: runhidden
