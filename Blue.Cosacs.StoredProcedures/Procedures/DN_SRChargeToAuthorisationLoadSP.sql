/****** Object:  StoredProcedure [dbo].[DN_SRChargeToAuthorisationLoadSP]    Script Date: 11/27/2007 13:57:29 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF EXISTS(SELECT * FROM dbo.sysobjects where id = object_id('[dbo].[DN_SRChargeToAuthorisationLoadSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
DROP PROCEDURE [dbo].[DN_SRChargeToAuthorisationLoadSP]
GO

-- =============================================
-- Author:		Jez Hemans
-- Create date: 27/11/2007
-- Description:	Returns contents of the table SR_ChargeToAuthorisation
-- =============================================
CREATE PROCEDURE [dbo].[DN_SRChargeToAuthorisationLoadSP] 
-- =============================================
-- Author:		????
-- Create date: ??
-- Title:	DN_SRChargeToAuthorisationLoadSP
--
--	This procedure will retrieve details of Charge To Authorisation
-- 
-- Change Control
-----------------
-- 16/11/10 jec CR1030 - return ChargeToAIG as ChargeToEW
-- =============================================
	-- Add the parameters for the function here	
AS
BEGIN
    SET NOCOUNT ON
	
    SELECT DefaultChargeTo,ChargeToCustomer,ChargeToAIG as ChargeToEW,ChargeToSupplier,
			ChargeToInternal,ChargeToDeliverer
    FROM SR_ChargeToAuthorisation
    ORDER BY CASE 
    WHEN DefaultChargeTo = 'Customer' THEN REPLACE(DefaultChargeTo,'Customer','AA') 
    WHEN DefaultChargeTo = 'Supplier' THEN REPLACE(DefaultChargeTo,'Supplier','AJ') 
    WHEN DefaultChargeTo = 'Internal' THEN REPLACE(DefaultChargeTo,'Internal','AK') 
    ELSE DefaultChargeTo END
END

GO

-- End End End End End End End End End End End End End End End End End End End End End End End End End End
