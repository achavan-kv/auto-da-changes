SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_GetStockItemTransSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_GetStockItemTransSP]
GO


CREATE procedure DN_GetStockItemTransSP @itemno varchar(8) = '', @descr1 varchar(25) = '',
                                        @descr1_en varchar(25) = '', @descr2 varchar(40) = '',
                                        @descr2_en varchar(40) = '', @return int OUTPUT as

	set rowcount 250

	-- build wildcards
	select @descr1_en = '%'+@descr1_en+N'%'
	select @descr2_en = '%'+@descr2_en+N'%'

	if (len(@itemno) = 0 and len(@descr1) = 0 and len(@descr2) = 0)
	begin
		-- everything where descr1 and descr2 are null
	        select itemno, descr1_en, descr1, descr2_en, descr2
	        from StockItemTrans
	        where isnull(descr1,'') = @descr1
	        and isnull(descr1_en,'') like @descr1_en
	        and isnull(descr2,'') = @descr2
	        and isnull(descr2_en,'') like @descr2_en
	        order by itemno
 	end
	else
	begin
		select @itemno = '%'+@itemno+N'%'
		select @descr1 = '%'+@descr1+N'%'
		select @descr2 = '%'+@descr2+N'%'

		-- everything where descr1 is null and descr2 like...
	        select itemno, descr1_en, descr1, descr2_en, descr2
	        from StockItemTrans
	        where isnull(itemno,'') like @itemno
	        and isnull(descr1,'') like @descr1
	        and isnull(descr1_en,'') like @descr1_en
	        and isnull(descr2,'') like @descr2
	        and isnull(descr2_en,'') like @descr2_en
	        order by itemno
 	end
	
	select @return = @@error

	set rowcount 0

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

