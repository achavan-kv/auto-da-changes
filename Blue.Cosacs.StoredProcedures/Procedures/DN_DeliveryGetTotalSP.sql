SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_DeliveryGetTotalSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_DeliveryGetTotalSP]
GO


CREATE PROCEDURE 	dbo.DN_DeliveryGetTotalSP
--------------------------------------------------------------------------------
--
-- Project      : CoSACS .NET
-- File Name    : DN_DeliveryGetTotalSP.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        :
-- Author       : Ruth Mqueen
-- Date         : ??
--
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 28/06/11 IP   5.13 - LW73619 - #3751 - Added isnull check as delivered amount was returned as null.
-- 01/05/12 jec  #10000 Not able to collect the items after goods return.
--------------------------------------------------------------------------------
			@acctNo varchar(12),
			@delivered money OUT,
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code
	SET	@delivered = 0

	SELECT	@delivered = isnull(sum(transvalue), 0)
	FROM		delivery
	WHERE	acctno = @acctNo
	AND itemno !='RB' -- exclude rebates 74450  
 
	-- #15993 removed - Collection of warranties processed before collection of item  WarehouseDeliver.cs !!!!!
	----SELECT @delivered = @delivered + isnull(sum(COALESCE(retval * -1, ordval, 0)),0)	--IP - 28/06/11 - 5.13 - LW73619 - #3751	
	----from schedule s
	----inner join lineitem l
	----	on l.itemId = s.ItemID
	----	and l.acctno = s.acctno 
	----	and l.stocklocn = s.stocklocn
	----	and l.ParentItemID=s.ParentItemID		-- #10000
	----	and l.contractno=s.contractno			-- #10000
	----where s.acctno = @acctNo
	----	AND delorcoll = 'C'


	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

-- End End End End End End End End End End End End End End End End End End End End End End End End End End End