@ECHO OFF

REM The following directory is for .NET 4.0
set DOTNETFX2=%SystemRoot%\Microsoft.NET\Framework\v4.0.30319
set PATH=%PATH%;%DOTNETFX2%

echo Installing IEPPAMS Win Service...
echo ---------------------------------------------------
set batdir=%~dp0
echo %batdir%
echo %batdir%Blue.Cosacs.MVESchedular.exe
"%batdir%Blue.Cosacs.MVESchedular.exe"
echo ---------------------------------------------------
pause
echo Done.