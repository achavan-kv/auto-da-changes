SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_ScheduleAssignNewBufferNoSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_ScheduleAssignNewBufferNoSP]
GO





CREATE PROCEDURE 	dbo.DN_ScheduleAssignNewBufferNoSP
			@acctno varchar(12),
			@stocklocn smallint,
			@buffno int,
			@itemId int,
            @newBuffNo int,
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

    -- DSR 8/11/05 - UAT 220
    -- Update the Buff No on the Schedule
    -- The Undelivered Flag should not be used for a new buffno
    UPDATE  schedule 
    SET     buffno = @newBuffNo,
            UndeliveredFlag = ''
    WHERE  AcctNo       = @acctno
    AND    ItemId       = @itemId
    AND    StockLocn    = @stocklocn
    AND    BuffNo       = @buffno

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO