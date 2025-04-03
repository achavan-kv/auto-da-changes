if exists (select * from dbo.sysobjects where id = object_id('[dbo].[VE_DeleteOldLogFiles]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
BEGIN
	DROP PROCEDURE [dbo].[VE_DeleteOldLogFiles]
END
GO

CREATE PROCEDURE [dbo].[VE_DeleteOldLogFiles] (
 @BackupFolderLocation VARCHAR(30)
,@FilesSuffix VARCHAR(3)
,@DaysToDelete SMALLINT
)
AS
BEGIN

 DELETE FROM VE_TaskSchedular WHERE CreatedDate<=(GETDATE()-90);

 DECLARE @delCommand VARCHAR(400)

 IF UPPER (@FilesSuffix) IN ('log') 
 BEGIN
  SET @delCommand = CONCAT('FORFILES /p ' ,
    @BackupFolderLocation,
    ' /s /m '  ,
    '*.'   , 
    @FilesSuffix ,
    ' /d '  ,
    '-'   , 
    ltrim(Str(@DaysToDelete)),
    ' /c ' ,
    '"'  ,
    'CMD /C del /Q /F @FILE',
    '"')

  PRINT @delCommand
  EXEC sys.xp_cmdshell @delCommand
 END
END