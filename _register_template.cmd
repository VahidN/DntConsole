SET DntConsolePath=%~dp0
IF "%DntConsolePath:~-1%"=="\" SET DntConsolePath=%DntConsolePath:~0,-1%
dotnet new uninstall %DntConsolePath%
dotnet new install %DntConsolePath%
dotnet new list
rem dotnet new DntConsole -n MyNewProj
pause