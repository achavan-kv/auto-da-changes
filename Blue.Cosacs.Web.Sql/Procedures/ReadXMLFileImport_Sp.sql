 if exists ( select *  from  dbo.sysobjects   where  id = object_id('[ReadXMLFileImport]')    and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure  [ReadXMLFileImport]
go
Create PROCEDURE [dbo].[ReadXMLFileImport]
AS
BEGIN 
  IF OBJECT_ID('tempdb..#FileList') IS NOT NULL    DROP TABLE #FileList
--Folder path where files are present
Declare @SourceFolder VARCHAR(100)
SET @SourceFolder='D:\Ashley\AckXMLRead\'

  CREATE TABLE #FileList (
    Id int identity(1,1),
    FileName nvarchar(255),
    Depth smallint,
    FileFlag bit)

--Load the file names from a folder to a table
   INSERT INTO #FileList (FileName,Depth,FileFlag) 
   EXEC xp_dirtree @SourceFolder, 10, 1

   --Use Cursor to loop throught files
   --Select * From #FileList
Declare @FileName VARCHAR(500)
Print 'Print File Name'

   DECLARE Cur CURSOR FOR
  SELECT FileName from #FileList
  where fileflag=1

OPEN Cur
FETCH Next FROM Cur INTO @FileName
WHILE @@FETCH_STATUS = 0
  BEGIN
  DECLARE @InsertSQL NVARCHAR(MAX)=NULL
  --Prepare SQL Statement for insert
  SET @InsertSQL=
  'INSERT INTO dbo.XMLFilesTable(FileName, LoadedDateTime,XMLData)
SELECT '''+@FileName+''',getdate(),Convert(XML,BulkColumn ) As BulkColumn
FROM Openrowset( Bulk '''+@SourceFolder+@FileName+''', Single_Blob) as Image'

--Print and Execute SQL Insert Statement to load file
Print @InsertSQL
EXEC(@InsertSQL)

     FETCH Next FROM Cur  INTO @FileName   

END
CLOSE Cur
DEALLOCATE Cur

 end 