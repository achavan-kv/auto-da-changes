SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_CustomerUpdateCustIDSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_CustomerUpdateCustIDSP]
GO

CREATE PROCEDURE 	dbo.DN_CustomerUpdateCustIDSP
-- ================================================
-- Project      : CoSACS .NET
-- File Name    : DN_CustomerUpdateCustIDSP.PRC
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : ?
-- Author       : ?
-- Date         : ?
--
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 05/03/12  IP  #9687 - LW74186 - The new customer id was not being updated for tables
--				 where the column name is CustomerID
-- ================================================
			@newcustid varchar(20),
			@oldcustid varchar(20),
			@user int	=0,
			@return int OUTPUT

AS

DECLARE @status int
set @status = 0
set @return = 0
declare @table_name varchar (64), @statement sqltext

DECLARE table_cursor CURSOR 
  	FOR SELECT t.table_name
   from information_schema.columns c,information_schema.tables t
   where c.column_name = 'custid'
   and  t.table_name not in ('customer','prevname','customer_changeid')
   and t.table_name not like 'temp_%'
	and t.table_name = c.table_Name
   and t.table_type = 'BASE TABLE'
   and t.table_schema = 'dbo'
   OPEN table_cursor
   FETCH NEXT FROM table_cursor INTO @table_name

   WHILE (@@fetch_status <> -1)
   BEGIN
	   IF (@@fetch_status <> -2)
   	begin
				set @statement=' update ' +  @table_name + ' set custid = ' + '''' +  @newcustid  + '''' +  
                   ' where  custid='  + '''' + @oldcustid + ''''
		           
				exec sp_executesql @statement
				if  @@error !=0
				BEGIN
					SET @return = @@error
					break
				END

   	
	   END
      FETCH NEXT FROM table_cursor INTO @table_name

   END

   CLOSE table_cursor
   deallocate table_cursor

   --IP - 05/03/12 - #9687 - LW74186
   if @return = 0
   begin
		
		update CustomerPhotos
		set CustomerID = @newcustid
		where CustomerID = @oldcustid
		
		update CustomerSignatures
		set CustomerID = @newcustid
		where CustomerID = @oldcustid
		
		update SR_CustomerInteraction
		set CustomerID = @newcustid
		where CustomerID = @oldcustid
   end
   
   if @return = 0 and @user != 0
   begin
	   insert into CustomerIdchanged(newcustid ,oldcustid  , datechange ,	   empeenochange )
		values (@newcustid,@oldcustid, getdate(),@user)		
    	IF (@@error != 0)
   	BEGIN
	   	SET @return = @@error
   	END
   end

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

