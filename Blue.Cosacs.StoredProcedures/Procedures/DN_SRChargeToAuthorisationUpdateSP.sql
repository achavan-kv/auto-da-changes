SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_SRChargeToAuthorisationUpdateSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_SRChargeToAuthorisationUpdateSP]
GO
-- =============================================
-- Author:		Jez Hemans
-- Create date: 27/11/2007
-- Description:	Updates the table SR_ChargeToAuthorisation
-- =============================================
CREATE PROCEDURE  [dbo].[DN_SRChargeToAuthorisationUpdateSP]
	@defaultChargeTo VARCHAR(10),
	@chargeToCustomer CHAR(1),
	@chargeToAIG CHAR(1),
	@chargeToSupplier CHAR(1),
	@chargeToInternal CHAR(1),
	@chargeToDeliverer CHAR(1),
	@return INT OUTPUT
AS
BEGIN
	
    SET NOCOUNT ON
    SET @Return = 0
    
    UPDATE SR_ChargeToAuthorisation
    SET ChargeToCustomer = @chargeToCustomer,ChargeToAIG = @chargeToAIG,ChargeToSupplier = @chargeToSupplier,ChargeToInternal = @chargeToInternal,ChargeToDeliverer = @chargeToDeliverer
    WHERE DefaultChargeTo = @defaultChargeTo
    
END

GO
