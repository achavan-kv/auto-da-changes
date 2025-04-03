SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_RemoveLoadFromContentsSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_RemoveLoadFromContentsSP]
GO

CREATE PROCEDURE 	dbo.DN_RemoveLoadFromContentsSP
--------------------------------------------------------------------------------
--
-- Project      : CoSACS .NET
-- File Name    : DN_RemoveLoadFromContentsSP.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Remove Load From Contents
-- Description	: Remove a Load
-- Author       : ??
-- Date         : ??
--
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 31/03/10 jec CR1072 Add Original Buff no to scheduleremoval
--------------------------------------------------------------------------------

    -- Parameters
			@dateDel datetime,
			@stocklocn smallint, 
			@buffNo int,
			@loadNo smallint,
			@piEmpeeNo int,
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

    -- Save an audit record for the Schedule removal
    INSERT INTO ScheduleRemoval
        (AcctNo, AgrmtNo, ItemNo, ItemID, StockLocn, Quantity, Price,
         DeliveryArea, BuffNo, LoadNo, DateRemoved, RemovedBy, Type,OrigBuffno)
    SELECT  s.AcctNo, s.AgrmtNo, '', s.ItemID, s.StockLocn, s.Quantity, l.Price,
            l.DeliveryArea, s.BuffNo, s.LoadNo, GETDATE(), @piEmpeeNo,
            null,s.OrigBuffno	-- CR1072 jec 31/03/10
    FROM    Schedule s, LineItem l
    WHERE   @StockLocn = (CASE WHEN ISNULL(s.RetStockLocn,0) = 0 THEN s.StockLocn ELSE s.RetStockLocn END)
    AND     s.BuffNo     = @BuffNo
    AND     s.LoadNo     = @loadNo
    AND     l.AcctNo     = s.Acctno
    AND     l.AgrmtNo    = s.AgrmtNo
    AND     l.ItemID     = s.ItemID
    AND     l.StockLocn  = s.StockLocn
    AND     l.Iskit      = 0

    -- DSR 3/11/05 - UAT 220
    -- Reset the Load No to zero on the Schedule
    -- and increment the Undelivered Flag to A, B, C ... Z
	UPDATE  schedule 
       SET  loadno = 0,
            UndeliveredFlag = CASE LTRIM(UndeliveredFlag)
                                  WHEN '' THEN 'A'
                                  WHEN 'Z' THEN 'Z'
                                  ELSE CHAR(ASCII(UndeliveredFlag) + 1)
                              END
	 WHERE  datedelplan = @dateDel
	   AND  @StockLocn = (CASE WHEN ISNULL(retstocklocn,0) = 0 THEN stocklocn ELSE retstocklocn END)
	   AND  buffno = @buffNo
	   AND  loadno = @loadNo

	DELETE  FROM deliveryload
	 WHERE  datedel = @dateDel
	   AND  buffbranchno = @StockLocn
	   AND  buffno = @buffNo
	   AND  loadno = @loadNo

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
