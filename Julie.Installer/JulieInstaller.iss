[Setup]
AppName=Julie
AppVersion=1.0.0
DefaultDirName={pf}\Julie
DefaultGroupName=Julie
OutputDir=Output
OutputBaseFilename=JulieInstaller
Compression=lzma
SolidCompression=yes

[Files]
Source: "..\Julie\bin\Release\net8.0\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
Name: "{group}\Julie"; Filename: "{app}\Julie.exe"
Name: "{group}\Uninstall Julie"; Filename: "{uninstallexe}"

[Run]
Filename: "{app}\Julie.exe"; Description: "Starte Julie"; Flags: nowait postinstall skipifsilent
