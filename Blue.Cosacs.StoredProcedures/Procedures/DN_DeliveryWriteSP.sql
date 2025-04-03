SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_DeliveryWriteSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_DeliveryWriteSP]
GO

CREATE PROCEDURE 	dbo.DN_DeliveryWriteSP
-- ================================================
-- Project      : CoSACS .NET
-- File Name    : DN_DeliveryWriteSP.prc
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Write Delivery Items
-- Date         : ??
--
-- This procedure will write items to the delivery table
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 09/05/11  jec Change to use ItemID fro RI
-- ================================================
	-- Add Parametere here
			@origbr smallint,
			@acctno varchar(12),
			@agrmtno int,
			@datedel datetime,
			@delorcol char(1),
			--@itemno varchar(8),
			@itemId int,		-- RI 09/05/11
			@stocklocn smallint,
			@quantity float,
			--@retitemno varchar(8),
			@retItemId int,
			@retstocklocn smallint,
			@retval float,
			@buffno int,
			@buffbranchno smallint,
			@datetrans datetime,
			@branchno smallint,
			@transrefno int,
			@transvalue money,
			@runno int,
			@contractno varchar(10),
			@notifiedby int,
			@ftnotes varchar(4),
			--@parentItemNo	VARCHAR(8),
			@parentItemId int,		-- RI 09/05/11
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	Declare @ItemNo VARCHAR(18) = '',
			@ParentItemNo VARCHAR(18) = ''		-- RI 09/05/11
	
	Select @ItemNo = IUPC from stockInfo s where ID = @itemID
	Select @ParentItemNo = ISNULL((SELECT IUPC from stockInfo s where ID=@ParentItemID),'') 

	-- 67732 RD 13/03/06 Added to ensure that return item code for repo are alwasy in uppercase
	-- uat363 rdb add ParentItemNo as part of primarykey
	------ #17290 - why is this code here ?? - removed 
	----UPDATE	DELIVERY 
	----SET	OrigBr 		= @origbr,
	----	DelOrColl	= @delorcol,
	----	Quantity	= @quantity,
	----	RetItemNo	= '',
	----	RetItemID	= ISNULL(@retItemId, 0),
	----	RetStockLocn = @retstocklocn,
	----	RetVal		= @retval,
	----	contractno	= @contractno,
 ----     ftnotes =@ftnotes
	----WHERE	AcctNo		= @acctno
	----AND	AgrmtNo		= @agrmtno
	------AND	ItemNo		= @itemno
	----AND	ItemID		= @itemID			-- RI 09/05/11
	----AND	StockLocn 	= @stocklocn
	----AND	BuffNo		= @buffno
	----AND	contractno 	= @contractno
	------AND parentItemNo = @parentItemNo
	----AND	parentItemID		= @parentItemID			-- RI 09/05/11
	
	--IF(@@rowcount=0 and @@error = 0)
	BEGIN
		SET @datedel = CONVERT(SMALLDATETIME, CONVERT(VARCHAR(10), @datedel, 105), 105)

		INSERT	INTO	delivery
			(origbr, acctno, agrmtno, datedel, delorcoll, itemno, 
			stocklocn, quantity, retitemno, RetItemID, retstocklocn, retval, 
			buffno, buffbranchno, datetrans, branchno, transrefno,
			transvalue, runno, contractno, notifiedby,      ftnotes, parentItemNo,ItemID,ParentItemID)
		VALUES	(@origbr, @acctno, @agrmtno, @datedel, @delorcol, @itemno, 
			@stocklocn, @quantity, '', ISNULL(@retItemId, 0), @retstocklocn, @retval, 
			@buffno, @buffbranchno, @datetrans, @branchno, @transrefno,
			@transvalue, @runno, @contractno, @notifiedby, @ftnotes, @parentItemNo,@ItemID,@ParentItemID
)
	END

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

-- End End End End End End End End End End End End End End End End End End End End End End End End End End 