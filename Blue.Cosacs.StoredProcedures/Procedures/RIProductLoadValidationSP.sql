IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[RIProductLoadValidationSP]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[RIProductLoadValidationSP]
GO

SET ANSI_NULLS OFF
GO

SET QUOTED_IDENTIFIER OFF
GO


CREATE PROCEDURE [dbo].[RIProductLoadValidationSP]
-- ================================================
-- Project      : CoSACS .NET
-- File Name    : RIProductLoadValidationSP.prc
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : RI Interface - Deliveries & Returns
-- Date         : 06 May 2011
--
-- This procedure will validate the Product file imported from RI.
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 26/07/11 jec  Do not reject entire file if only errors in individual records
-- ================================================
	--@fileName varchar(40) OUT,
	--@path varchar(500) OUT,
	@interface varchar(20),
	@runno INT,
	@Repo BIT,
	@rerun BIT,
	@return INT output		
As
	set @return=0
	Declare @ImportTable VARCHAR(50)
	
	set @ImportTable=case when @repo=0 then 'RItemp_RawProductImport' 
					else 'RItemp_RawProductImportRepo' end
					
	select column_name,data_type,character_maximum_length,ordinal_position 
	into #dataValidation
	from information_schema.Columns
	where table_name='RItemp_RawProductload'
	order by ordinal_position
	
	declare @maxcols INT,@col INT,@sqlcommand SQLText,@column VARCHAR(30),@errors int	
	select @maxcols=MAX(ordinal_position) from #dataValidation
	-- Validate Column widths
	set @col=0
	while @col<@maxcols
	begin
	set @col=@col+1
		select @column=column_name from #dataValidation where ordinal_position=@col
		set @sqlcommand='insert into RItemp_RawProductImportError select ' + ''''+ @column +''''+
		','+ ''''+ 'Max Column Width exceeded'+ ''''+ ','+@column + ','+CAST(@repo as CHAR(1))+',l.*' +
		' from '+ @ImportTable + ' l, #dataValidation where len('+@column+') > character_maximum_length and '+
		''''+ @column +'''' + '= column_name' 
		--select @sqlcommand 
		exec sp_executesql @sqlcommand
	end
	
	-- If errors - report
	if exists(select top 1 * from RItemp_RawProductImportError where repo=@repo)
		BEGIN
			
			INSERT INTO Interfaceerror(interface, runno,errordate,severity,errortext)   
			select @interface, @runno, getdate(),'E', 
			'ABC file: Column: ' + ColumnName + ', Error: ' + ErrorDescr + ', ErrorData: ' + ErrorData
			from RItemp_RawProductImportError 
			
			--Declare @ErrorMsg VARCHAR(100)
			--set @ErrorMsg = 'Product File has error - Interface ' + @interface + ' Terminated'
			--RAISERROR (@ErrorMsg ,16,1)  

		END
	
	-- Copy imported data to Load tables
		Begin			
			if @Repo=0
			Begin
				Insert into RItemp_RawProductload 
				select * from RItemp_RawProductImport I
				Where not exists(select * from RItemp_RawProductImportError E where I.ItemIUPC=E.ItemIUPC and repo=@repo)
			End
			else
			Begin
				Insert into RItemp_RawProductloadRepo 
				select * from RItemp_RawProductImportRepo I
				Where not exists(select * from RItemp_RawProductImportError E where I.ItemIUPC=E.ItemIUPC and repo=@repo)
			End
		End

GO

-- end end end end end end end end end end end end end end end end end end end end end end end
