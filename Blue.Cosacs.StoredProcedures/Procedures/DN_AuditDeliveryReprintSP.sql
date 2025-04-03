SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects
where id = object_id('[dbo].[DN_AuditDeliveryReprintSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_AuditDeliveryReprintSP]
GO

CREATE PROCEDURE  dbo.DN_AuditDeliveryReprintSP
    @accountno   char(12),
    @agrmtno     int,
    @itemId      int,
    @stocklocn   smallint,
    @buffno      int,
    @printedby   int,
    @return      int OUTPUT

AS DECLARE

    @DeliveryPrinted   CHAR(1)
    
BEGIN
    SET @return = 0

    -- Check whether the line item has been printed before
	SELECT @DeliveryPrinted = DeliveryPrinted
	FROM   LineItem
	WHERE  AcctNo = @accountno
	AND    AgrmtNo = @agrmtno
	AND    ItemID = @itemId
	AND    StockLocn = @stocklocn
	
	IF (@DeliveryPrinted = 'Y')
	BEGIN
	    -- Audit the reprint of this delivery item
	    INSERT INTO DeliveryReprint
	        (AcctNo, AgrmtNo, ItemNo, ItemID, StockLocn, BuffNo, DatePrinted, PrintedBy)
	    VALUES
	        (@accountno, @agrmtno, '', @itemId, @stocklocn, @buffno, GETDATE(), @printedby)
	END
	ELSE
	BEGIN
	    -- The first print of this delvery item
	    UPDATE LineItem SET DeliveryPrinted = 'Y'
	    WHERE  AcctNo = @accountno
	    AND    AgrmtNo = @agrmtno
	  	AND    ItemID = @itemId
	    AND    StockLocn = @stocklocn
	END

    SET @return = @@error
END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

