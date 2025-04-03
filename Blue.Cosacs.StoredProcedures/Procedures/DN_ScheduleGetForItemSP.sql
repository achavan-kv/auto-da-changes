SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_ScheduleGetForItemSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_ScheduleGetForItemSP]
GO

CREATE PROCEDURE 	dbo.DN_ScheduleGetForItemSP
-- =============================================
-- Author:		??
-- Create date: ??
-- Description:	Retrieve Scheduled items
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 04/03/10  IP/jec CR1072 Malaysia merge
-- ============================================= 
			@acctNo varchar(12),
			@agreementNo int,
			--@itemNo varchar(8),
			@itemID int,				--IP/NM - 18/05/11 -CR1212 - #3627 
			@location smallint,
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	SELECT	stocklocn as Branch,
			buffno as Number,
			quantity as 'Item Qty'
	FROM		schedule
	WHERE	acctno = @acctNo
	AND		agrmtno = @agreementNo
	--AND		itemno = @itemNo
	AND		ItemID = @itemID
	AND		stocklocn = @location
	and		DHLDNNo is null		-- IP/jec 04/03/10 Malaysia 3PL - only select records not in transit

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
