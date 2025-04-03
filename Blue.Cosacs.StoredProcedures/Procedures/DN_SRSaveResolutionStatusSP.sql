--ALTER TABLE SR_Resolution ADD ReplacementStatus CHAR(1)

/****** Object:  StoredProcedure [dbo].[DN_SRSaveResolutionStatus]    Script Date: 01/23/2007 11:36:33 ******/
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID('[dbo].[DN_SRSaveResolutionStatusSP]') AND OBJECTPROPERTY(id,'IsProcedure') = 1)
DROP PROCEDURE [dbo].DN_SRSaveResolutionStatusSP
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Peter Chong
-- Create date: 23/Jan/2007
-- Description:	Updates the status replacement status for an account for an item and account where replacement is 'Y'
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 22/07/11  IP  RI - System Integration. Join using ItemID
-- =============================================
CREATE PROCEDURE DN_SRSaveResolutionStatusSP
	@AccountNo			char(12),
	--@ItemNo				varchar(8),
	@ItemID				int,							--IP - 22/07/11 - RI
	@StockLocn			smallint,
	@ReplacementStatus	char(1),
	@Return				int OUTPUT
AS

BEGIN
	
	SET NOCOUNT ON;
	SET @Return = 0

	DECLARE	@CurrentStatus char(1)

	SELECT	@CurrentStatus = ISNULL(R.ReplacementStatus, '')
	FROM	SR_Resolution R INNER JOIN 
			SR_ServiceRequest SR ON SR.ServiceRequestNo =  R.ServiceRequestNo 
	WHERE	SR.Acctno = @AccountNo
	--AND		SR.ProductCode = @ItemNo
	AND		SR.ItemID = @ItemID						--IP - 22/07/11 - RI
	AND		SR.StockLocn = @StockLocn
	AND		R.Replacement = 'Y' 

	IF @CurrentStatus != 'D'
	BEGIN
		UPDATE R 
		SET ReplacementStatus = @ReplacementStatus
		FROM 
			SR_ServiceRequest SR INNER JOIN
			SR_Resolution R ON SR.ServiceRequestNo =  R.ServiceRequestNo 
		WHERE 
			SR.Acctno = @AccountNo AND
			--SR.ProductCode = @ItemNo AND
			SR.ItemID = @ItemID AND					--IP - 22/07/11 - RI
			SR.StockLocn = @StockLocn AND
			R.Replacement = 'Y' -- Only update items where the replacement indicator is set to 'Y'
	END

	SET @Return = @@error
	
END
GO





