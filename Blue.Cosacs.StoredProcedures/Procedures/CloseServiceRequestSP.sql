IF EXISTS (
			SELECT 1
			FROM sys.procedures WITH (NOLOCK)
			WHERE NAME = 'CloseServiceRequestSP'
				AND type = 'P'
		)
	DROP PROCEDURE [dbo].[CloseServiceRequestSP]
GO

-- =============================================================================================================================
-- Procedure Name:	CloseServiceRequestSP
-- Author:			Ashwini Akula
-- Create Date:		12/02/2019
-- Description:		Version		Details
--					V1.0		Close service request when installation item (non stock) removed from sales order screen
-- =============================================================================================================================


CREATE PROCEDURE [dbo].[CloseServiceRequestSP]
@Acctno VARCHAR(12),
@ParentItemId INT,
@ItemId INT,
@return INT OUT
AS

BEGIN

	SET @return = 0

	UPDATE	SR  
	SET		SR.[State] = 'Closed',
			SR.[isClosed] = 1
	FROM	[service].[request] SR 
			INNER JOIN	dbo.lineitem L
			ON SR.account = L.acctno 
				AND SR.Branch = L.stocklocn
				AND SR.[state] != 'Closed' 
				AND SR.isClosed != 1
	WHERE	L.acctno = @Acctno
			AND (L.ParentItemID = @ParentItemId AND L.ItemId = @ItemId)
			AND SR.itemid = @ParentItemId
			AND SR.ItemAmount != L.ordval

END