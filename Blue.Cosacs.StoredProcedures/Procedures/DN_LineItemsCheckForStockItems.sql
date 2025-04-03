SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_LineItemsCheckForStockItems]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_LineItemsCheckForStockItems]
GO


CREATE PROCEDURE 	dbo.DN_LineItemsCheckForStockItems
-- ================================================
-- Project      : CoSACS .NET
-- File Name    : DN_LineItemsCheckForStockItems.PRC
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        :  
-- Author       : ??
-- Date         : ??
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 16/05/11 ip  RI Integration changes - CR1212 - #3627 - Changed joins between stockitem and lineitem to use ItemID rather than ItemNo
-- ================================================
			@acctNo varchar(12),
			@stockCount int OUT,
			@affinityCount int OUT,
			@return int OUTPUT
AS

	SET 	@return = 0			--initialise return code
	SET 	@stockCount = 0		--initialie stock count
	SET 	@affinityCount = 0	--initialie stock count

    	/*check whether warranty available for this item and price but not if cash and go*/
	if @acctno not like '___5%' -- don't do this for cash and go
	begin
	        SELECT 	@stockCount = ISNULL(count(*),0) 
        	FROM 	lineitem 
		WHERE	acctno = @acctNo
		AND 	itemtype = 'S'

		IF (@@error != 0)
		BEGIN
			SET @return = @@error
		END

		If @return = 0 
		BEGIN
	        	SELECT 	@affinityCount = ISNULL(count(*),0) 
	        	FROM 	lineitem l, stockitem s
			WHERE	l.acctno = @acctNo
			--AND 	l.itemno = s.itemno
			AND l.ItemID = s.ItemID			--IP - 16/05/11 - CR1212 - #3627
			AND	l.stocklocn = s.stocklocn
			AND     (s.category = 11 OR s.category BETWEEN 51 AND 59) 
		END 

		IF (@@error != 0)
		BEGIN
			SET @return = @@error
		END
          end
          else
	  begin -- cash and go
		set @stockcount = 1
                set @affinitycount =0
	  end

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO