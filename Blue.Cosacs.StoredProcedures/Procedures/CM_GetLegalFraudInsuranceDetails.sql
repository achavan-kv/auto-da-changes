SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID('[dbo].[CM_GetLegalFraudInsuranceDetails]') AND OBJECTPROPERTY(id,'IsProcedure') = 1)
DROP PROCEDURE [dbo].[CM_GetLegalFraudInsuranceDetails]
GO

-- =============================================================================================
-- Author:		Nasmi Mohamed & Ilyas Parker 
-- Create date: 08/01/2009
-- Description:	Procedure that will return 'legal details', 'fraud details', or 'insurance details'
--				for an account.

-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 02/06/09 jec Add call to get Trace details	                       
-- =============================================================================================

CREATE PROCEDURE 	[dbo].[CM_GetLegalFraudInsuranceDetails]
			@acctno	char(12),
			@return int OUTPUT

AS

	SET @return = 0			--initialise return code
	
		
		--Select the 'Legal' details.
		SELECT acctno,
			   empeeno,
			   solicitorNo,
			   auctionProceeds,
			   auctionDate,
			   auctionAmount,
			   courtDeposit,
			   courtAmount,	
			   courtDate,
			   caseClosed,
			   mentionDate,
			   mentionCost,
			   paymentRemittance,
			   judgement,
			   legalAttachmentDate,
			   legalInitiatedDate,
			   defaultedDate,
			   userNotes
		FROM CMLegalDetail
		WHERE acctno = @acctno

		--Select the 'Fraud' details.
		SELECT acctno,
			   empeeno,	
			   fraudInitiatedDate,
			   isResolved,
			   userNotes
		FROM CMFraudDetail
		WHERE acctno = @acctno

		--Select 'Insurance' details.
		SELECT acctno,
			   empeeno,
			   initiatedDate,
			   fullOrPartClaim,
			   insAmount,
			   insType,	
			   isApproved,
			   userNotes
		FROM CMInsuranceDetail
		WHERE acctno = @acctno
		
		-- get Trace details
		
		exec CM_GetTraceDetails @acctno,@return 

	
	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END
	

Go

-- End End End End End End End End End End End End End End End End End End End End End End End End End End

