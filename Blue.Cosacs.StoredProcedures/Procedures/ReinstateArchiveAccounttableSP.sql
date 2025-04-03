-- This gives the ability for the user to reinstate the archived account for each table

-- ============================================================================================
-- Author:		Alex Ayscough
-- Modified by: Ilyas Parker
-- Modified date: 03/02/2009
-- Description:	The procedure will Un-archive all the data held in the archive 
--				tables and insert the data back into the live tables for an 
--				account. The un-archive option will be selected from the
--				'Customer Search' screen.		
-- ============================================================================================

IF EXISTS(SELECT * FROM sysobjects WHERE NAME ='ReinstateArchiveAccounttableSP')
DROP PROCEDURE ReinstateArchiveAccounttableSP
go
CREATE PROCEDURE ReinstateArchiveAccounttableSP 
				 @archive_table VARCHAR(32),
				 @acctno CHAR(12)
AS

DECLARE @table_name VARCHAR(32),
		@column VARCHAR(32),
		@statement SQLText,
		@continueCheck SQLText,
		@continue INT,
		@counter INT,
		@total INT,
		@selectstatement SQLText,
		@FINALSTATEMENT SQLText,
		@newline VARCHAR(22),
		@param Nvarchar(50)

-- removing the _archive from the table name 
SET @table_name = left(@archive_table,DATALENGTH(@archive_table)-8)

-- only archive if this archive table exists
--PRINT @archive_table
--IF NOT EXISTS(SELECT * FROM cosacs_archive.information_schema.tables
--WHERE table_name = @archive_table and exists(select count(*) from cosacs_archive.information_schema.tables
--									))
--BEGIN
----   print 'No ' + @archive_table + ' found'
--	RETURN
--END
--   PRINT 'doing ' + @table_name

--If the archive table does not exist or there is no data to un-archive in the archive table for the selected
--account, then we do not want to continue to un-archive data from this table.
set @continueCheck = 'IF NOT EXISTS(SELECT * FROM cosacs_archive.information_schema.tables
					WHERE table_name = ' + ''''+ @archive_table + ''''+
				  ' ) OR NOT EXISTS (SELECT COUNT(*) FROM cosacs_archive.dbo.' + @archive_table +
				  ' WHERE acctno = ' + '''' + @acctno+ '''' +  ' HAVING count(*) > 0 ) 
				  BEGIN
					
					SELECT @continue = 0
				  END
				  ELSE
					SELECT @continue = 1'
				  

--Need to pass in an OUTPUT parameter to check whether we can continue to un-archive data from the table.
SELECT @param = '@continue int OUTPUT'

EXEC sp_executesql @continueCheck, @param, @continue = @continue OUTPUT

IF(@continue = 0)
BEGIN
	return
END
ELSE
PRINT  'doing ' + @table_name

--IP - If the table is 'Delivery_archive' need to update the quantity to 1 due to a check constraint on the delivery
--table which says quantity <> 0
IF(@archive_table = 'Delivery_archive')
BEGIN
	UPDATE cosacs_archive.dbo.delivery_archive
	SET quantity = 1
	WHERE acctno = @acctno
	AND quantity = 0
END
--IP - Need to update the source on the 'Fintrans_archive' table as there is a check constraint on the 
--fintrans table which says this should either be 'COSACS' or 'COASTER'.
IF(@archive_table = 'Fintrans_archive')
BEGIN
	UPDATE cosacs_archive.dbo.fintrans_archive
	SET source = 'COSACS'
	WHERE acctno = @acctno
	AND (source in ('', 'AS400')
	OR source like '%COSACS%')
END

SET @counter =0
--select the total number of columns on the archive table.
--SELECT @total = COUNT(*) FROM cosacs_archive.information_schema.columns WHERE table_name = @archive_table 
--Select the total number of columns from the Live database table.
SELECT @total = COUNT(*) FROM information_schema.columns WHERE table_name = @table_name 

SET @newline = ' '

DECLARE column_cursor CURSOR FAST_FORWARD READ_ONLY FOR
--for each column in the archive table
--SELECT c1.column_name --archive table column
--FROM  cosacs_archive.information_schema.columns c1
--JOIN cosacs_archive.information_schema.tables t1
--ON t1.table_name = c1.table_name
--JOIN information_schema.columns c2 
--ON c2.column_name = c1.column_name 
--JOIN information_schema.tables t2 
--ON t2.table_name = c2.table_name
--WHERE c1.table_name = @archive_table
--AND c2.table_name = @table_name
--AND t2.table_type = 'BASE TABLE'
--AND t1.table_type = 'BASE TABLE'

--IP - Select columns from the Live table as we are inserting into the Live table.
--Previously was selecting the columns from Cosacs_archive, which may not have the
--same number of columns as the live table.
SELECT c1.column_name 
FROM  information_schema.columns c1
JOIN information_schema.tables t1
ON t1.table_name = c1.table_name
WHERE c1.table_name = @table_name
AND t1.table_type = 'BASE TABLE'

--For each column
OPEN column_cursor
FETCH NEXT FROM column_cursor INTO @column

WHILE @@FETCH_STATUS = 0
BEGIN
   SET @counter =@counter + 1

   IF @counter =1 -- first row
   BEGIN
   	SET @statement =' insert into dbo.' + @table_name + '('   --Insert into live table e.g insert into agreement(acctno'
      SET @selectstatement = ' select '
   END
   
   SET @statement =@statement + @column   
	
   --IP - Check that the column being selected exists on Cosacs_archive table
   IF NOT EXISTS(SELECT * FROM cosacs_archive.information_schema.columns c
					WHERE c.column_name = @column and c.table_name = @archive_table)
	BEGIN
		
		--IP -- Need to update the 'ftnotes' column to 'XX' as the delivery trigger (trig_delivery)
		--was doubling the transvalue on fintrans if the fintrans record exists and we are inserting 
		--the delivery record. This is prevented by updating the 'ftnotes' to 'XX'.
		IF (@column = 'ftnotes' AND @archive_table = 'delivery_archive')
		BEGIN
			SET @column = '''XX'''
		END
		ELSE	
		BEGIN
			--Select the data type of the column from the Live database table
			DECLARE @type varchar(10)
			SET @type = (SELECT c.data_type FROM information_schema.columns c
							WHERE c.column_name = @column and c.table_name = @table_name)
			
			--As the column does not exist on the Cosacs_archive table
			--when selecting the value we have no column to select from 
			--therefore we must determine the data type of the column does
			--not exist on Cosacs_archive and in its place 
			--replace the value with one of the following determined by the
			--case statement.
			SET @column = CASE 
				 WHEN @type = 'varchar' then ' '''' '
				 WHEN @type = 'char' then ' '''' '
				 WHEN @type = 'int' then '0'                   
				 WHEN @type = 'money' then  '0'
				 WHEN @type = 'smallint' then '0'
				 WHEN @type = 'datetime' then '01/01/1900'
				 WHEN @type = 'bit' then '0'
				 WHEN @type = 'float' then '0'
				 WHEN @type = 'decimal' then '0'
				 END
		END
	END
	
   --IP - If the column exists on the archive table then this will be selected.
   SET @selectstatement = @selectstatement + @column
			
   IF @counter !=@total -- not last row(last column) so need comma
   BEGIN
		SET @statement = @statement + ',' + @newline			
   		SET @selectstatement = @selectstatement + ',' + @newline 
   END
   ELSE -- last row
   BEGIN
	--insert the record into the live database from the archive if it does not exist.
   		SET @statement =@statement + ')'  
		SET @selectstatement = @selectstatement + ' FROM cosacs_archive.dbo.' + @archive_table + 
      @newline + ' WHERE acctno = ' + '''' + @acctno + '''' +
      ' AND NOT EXISTS (SELECT * FROM ' + @table_name + ' T WHERE T.ACCTNO =' + '''' + @ACCTNO + '''' + ')'
   END

FETCH NEXT FROM column_cursor INTO @column

END

CLOSE column_cursor
DEALLOCATE column_cursor

   --Process the insert into the live table from the Cosacs_archive table.
   SET @FINALSTATEMENT = RTRIM(@statement) + @newline + RTRIM(@selectstatement )
   PRINT RTRIM(@FINALSTATEMENT) + @newline
   EXEC sp_executesql @FINALSTATEMENT

	
   --Finally delete the entries from the Cosacs_archive table for the account.
   SET @FINALSTATEMENT = ' DELETE FROM cosacs_archive.dbo.' + @archive_table +
   ' WHERE acctno=' + '''' + @acctno + ''''
   EXEC sp_executesql @FINALSTATEMENT

GO





