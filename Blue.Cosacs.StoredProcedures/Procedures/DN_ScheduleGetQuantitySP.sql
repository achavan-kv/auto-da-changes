SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_ScheduleGetQuantitySP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_ScheduleGetQuantitySP]
GO

CREATE PROCEDURE 	dbo.DN_ScheduleGetQuantitySP
-- =============================================
-- Author:		??
-- Create date: ??
-- Description:	Retrieve Scheduled item quantity
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 04/03/10  IP/jec CR1072 Malaysia merge
-- 17/05/11  IP	RI Integration changes - CR1212 - #3627 - Changed joins to join on ItemID	
-- ============================================= 
			@acctNo varchar(12),
			@agreementNo int,
			--@itemNo varchar(8), 
			@itemID int,			--IP - 17/05/11 - CR1212 - #3627
			@location smallint,
			@scheduled float OUT,
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code
	SET	@scheduled = 0

	SELECT	@scheduled = sum(quantity)
	FROM		schedule
	WHERE	acctno = @acctNo
	AND		agrmtno = @agreementNo
	--AND		itemno = @itemNo
	AND		ItemID = @itemID				--IP - 17/05/11 - CR1212 - #3627
	AND		stocklocn = @location
	--and		DHLDNNo is null		-- IP/jec 04/03/10 Malaysia 3PL - only select records not in transit

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END


GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

-- End End End End End End End End End End End End End End End End End End End End End End End End End
