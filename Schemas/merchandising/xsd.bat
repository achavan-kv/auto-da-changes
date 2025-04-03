
"%ProgramFiles(x86)%\Microsoft SDKs\Windows\v7.0A\Bin\xsd.exe" Cints.xsd /o:..\..\Blue.Cosacs.Messages\Merchandising /c /namespace:Blue.Cosacs.Messages.Merchandising.Cints



SETLOCAL ENABLEDELAYEDEXPANSION
set datetimef=%date:~-4%%date:~3,2%%date:~0,2%%time:~0,2%%time:~3,2%%time:~6,2%
FOR /R %%X IN (*.xsd) DO (
  set temp=%%~nX%%~xX
  echo update Hub.Queue set [Schema] = N'>> ..\..\Blue.Cosacs.Web.Sql\Migrations\"%datetimef% - Web Hub Schema Update.sql"
  type !temp! >> ..\..\Blue.Cosacs.Web.Sql\Migrations\"%datetimef% - Web Hub Schema Update.sql"
  echo ' WHERE SchemaSource = '!temp!' >> ..\..\Blue.Cosacs.Web.Sql\Migrations\"%datetimef% - Web Hub Schema Update.sql"
)