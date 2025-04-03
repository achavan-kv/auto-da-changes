SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_UpdateStockItemTransSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_UpdateStockItemTransSP]
GO


CREATE procedure DN_UpdateStockItemTransSP @itemno varchar(8), @descr1 varchar(25), @descr2 varchar(40), @return int output as

	-- update translation
	update StockItemTrans
	set descr1 = @descr1,
            descr2 = @descr2
        where itemno = @itemno

	select @return = @@error

	if (@return = 0)
	begin
		update StockItem 
		set itemdescr1 = @descr1,
		    itemdescr2 = @descr2
		where itemno = @itemno

		select @return = @@error
	end
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

