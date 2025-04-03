IF EXISTS (select * from dbo.sysobjects where id = object_id(N'[dbo].[empty_to_null]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	DROP PROCEDURE empty_to_null
go

CREATE PROCEDURE empty_to_null
-- ================================================
-- Project      : CoSACS .NET
-- File Name    : empty_to_null.prc
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Empty to Null
-- Author       : Simone Linder
-- Date         : May 2008
--
-- This procedure will convert empty fields (length 0) to null.
-- 
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 
-- ================================================
	-- Add the parameters for the stored procedure here

@TableName nvarchar(256)

AS 

BEGIN

set nocount on
DECLARE 
	
    @ColumnName nvarchar(128), 
      @SQL nvarchar(4000),
	@data_type varchar(50),
	@character_maximum_length varchar(20)

            SET @ColumnName = ''
            WHILE (@TableName IS NOT NULL) AND (@ColumnName IS NOT NULL)

            BEGIN
	-- get column
                  SET @ColumnName =
                  (
                        SELECT MIN(QUOTENAME(COLUMN_NAME))
                        FROM INFORMATION_SCHEMA.COLUMNS
                        WHERE             table_name = @TableName
                              AND   DATA_TYPE IN ('char', 'varchar', 'nchar', 'nvarchar')
                              AND   QUOTENAME(COLUMN_NAME) > @ColumnName
                  )
	-- get datatype
				  Set @data_type=
				  (
                        SELECT MIN(data_type)
                        FROM INFORMATION_SCHEMA.COLUMNS
                        WHERE             table_name = @TableName
                              AND   QUOTENAME(COLUMN_NAME) = @ColumnName
                  )
     -- get column length             
					Set @character_maximum_length=
					(
                        CAST((SELECT MIN(character_maximum_length)
                        FROM INFORMATION_SCHEMA.COLUMNS
                        WHERE             table_name = @TableName
                              AND   QUOTENAME(COLUMN_NAME) = @ColumnName) as varchar)
                  )

                  IF @ColumnName IS NOT NULL
                  BEGIN
	-- change column to allow nulls
						SET @SQL=   'alter table  ' + @TableName + 
                                    ' alter column ' + @ColumnName 
                                    + ' ' + @data_type + '(' + @character_maximum_length +') ' 

						EXEC (@SQL)
	-- set column to null if empty  (null and empty are not the same)
                        SET @SQL=   'UPDATE ' + @TableName + 
                                    ' SET ' + @ColumnName 
                                    + ' =  NULL' + 
                                    ' WHERE len(' + @ColumnName + ')=0'
                        EXEC (@SQL)

                  END
            END   
      END
GO

-- End End End End End End End End End End End End End End End End End End End End End End End 