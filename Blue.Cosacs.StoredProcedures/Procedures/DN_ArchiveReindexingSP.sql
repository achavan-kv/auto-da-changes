SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO

IF EXISTS (SELECT * FROM SysObjects
           WHERE Name = 'DN_ArchiveReindexingSP' AND Type = 'P')
BEGIN
    DROP PROCEDURE DN_ArchiveReindexingSP
END
GO

CREATE PROCEDURE DN_ArchiveReindexingSP

--------------------------------------------------------------------------------
--
-- Project      : CoSACS .NET
-- File Name    : DN_ArchiveReindexingSP.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Archiving 
-- Author       : Rupal Desai
-- Date         : 10 May 2006
--
-- Change Control
-- --------------
-- Date      	By  Description
-- ----      	--  -----------
--


--declare
      @return             int OUTPUT
AS

BEGIN
	DECLARE @table_name varchar(32)

	SET 	@return = 0			--initialise return code

	DECLARE table_cursor CURSOR 
	   FOR 
	   SELECT top 10 table_name FROM information_schema.tables 
	   WHERE table_name not like 'sys%' and table_name not like 'ii%'
	   and table_type = 'BASE TABLE' and table_name not like 'temp%'
	   AND table_schema = 'dbo' -- prevent error on archiving UAT issue. 
	   and not exists (select * from Table_reindex t where reindex_date > dateadd (month, -2, getdate()) and t.table_name = information_schema.tables.table_name)

	   OPEN table_cursor

	   FETCH NEXT FROM table_cursor INTO @table_name
	
	   WHILE (@@fetch_status <> -1)
	   BEGIN
	   IF (@@fetch_status <> -2)
	   	begin
			DBCC DBREINDEX (@TABLE_NAME)
			update table_reindex set reindex_date = getdate() where table_name =@table_name
	         if @@rowcount = 0
	         begin
			insert into table_reindex (table_name,reindex_date) values (@table_name,getdate())
	         end
	   END
	      FETCH NEXT FROM table_cursor INTO @table_name

	   END
	
	   CLOSE table_cursor
	   DEALLOCATE table_cursor

	
	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END
	
END
GO

GRANT EXECUTE ON DN_ArchiveReindexingSP TO PUBLIC
GO
SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO
