[Setup]
AppName=Julie
AppVersion=1.0.0
DefaultDirName={pf}\Julie
DefaultGroupName=Julie
OutputDir=Output
OutputBaseFilename=JulieInstaller
Compression=lzma
SolidCompression=yes

SetupIconFile=..\Julie\Assets\logo_orange_transparent.ico

[Files]
Source: "..\Julie\bin\Release\net8.0\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs
Source: "..\Julie\Assets\logo_orange_transparent.ico"; DestDir: "{app}"; Flags: ignoreversion

[Icons]
Name: "{group}\Julie"; Filename: "{app}\Julie.exe"; IconFilename: "{app}\logo_orange_transparent.ico"
Name: "{group}\Uninstall Julie"; Filename: "{uninstallexe}"; IconFilename: "{app}\logo_orange_transparent.ico"

[Run]
Filename: "{app}\Julie.exe"; Description: "Starte Julie"; Flags: nowait postinstall skipifsilent

[Registry]
; Open With-Eintrag hinzufügen
Root: HKCU; Subkey: "Software\Microsoft\Windows\CurrentVersion\Explorer\FileExts\.log\OpenWithList"; \
    ValueType: string; ValueName: "j"; ValueData: "Julie.exe"; Flags: uninsdeletevalue
    
Root: HKCU; Subkey: "Software\Microsoft\Windows\CurrentVersion\Explorer\FileExts\.tlg\OpenWithList"; \
    ValueType: string; ValueName: "j"; ValueData: "Julie.exe"; Flags: uninsdeletevalue

; Befehl registrieren
Root: HKCU; Subkey: "Software\Classes\Applications\Julie.exe\shell\open\command"; \
    ValueType: string; ValueData: """{app}\Julie.exe"" ""%1"""; Flags: uninsdeletekey