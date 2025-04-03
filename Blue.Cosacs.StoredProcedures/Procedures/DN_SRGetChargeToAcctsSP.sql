SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS OFF
GO

if exists (select * from dbo.sysobjects
where id = object_id('[dbo].[DN_SRGetChargeToAcctsSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_SRGetChargeToAcctsSP]
GO


CREATE PROCEDURE dbo.DN_SRGetChargeToAcctsSP
    @ServiceRequestNo   	INTEGER,
    @InternalAcctNo     	CHAR(12) OUTPUT,
    @WarrantyAcctNo     	CHAR(12) OUTPUT,
    @SupplierAcctNo     	CHAR(12) OUTPUT,
    @SupplierSpecialAcctNo	CHAR(12) OUTPUT,
    @DelivererAcctNo    	CHAR(12) OUTPUT,
    @CustomerAcctNo     	CHAR(12) OUTPUT,
    @SupplierId         	VARCHAR(12) OUTPUT,
    @DelivererId        	VARCHAR(12) OUTPUT,
    @CustomerId         	VARCHAR(20) OUTPUT,
    @DepositAmount      	MONEY OUTPUT,
    @DepositPaid        	MONEY OUTPUT,
    @Return             	INTEGER OUTPUT

AS
    SET NOCOUNT ON
    SET @Return = 0

    -- Charge To payment accounts are linked to SRs via SR_ChargeAcct
    
    -- Get all of the Charge To accounts already linked
    
    SELECT @InternalAcctNo = s.AcctNo
    FROM   SR_ChargeAcct s
    WHERE  s.ServiceRequestNo = @ServiceRequestNo
    AND    s.ChargeType = 'I'

    SELECT @WarrantyAcctNo = s.AcctNo
    FROM   SR_ChargeAcct s
    WHERE  s.ServiceRequestNo = @ServiceRequestNo
    AND    s.ChargeType = 'W'

    SELECT @SupplierAcctNo = s.AcctNo
    FROM   SR_ChargeAcct s
    WHERE  s.ServiceRequestNo = @ServiceRequestNo
    AND    s.ChargeType = 'S'

    SELECT @DelivererAcctNo = s.AcctNo
    FROM   SR_ChargeAcct s
    WHERE  s.ServiceRequestNo = @ServiceRequestNo
    AND    s.ChargeType = 'D'

    SELECT @CustomerAcctNo = s.AcctNo
    FROM   SR_ChargeAcct s
    WHERE  s.ServiceRequestNo = @ServiceRequestNo
    AND    s.ChargeType = 'C'

    -- Get the 'Customer' ids
    
    SELECT @SupplierId = c.Code,
           @SupplierSpecialAcctNo = c.Reference
    FROM   SR_Resolution r, Code c
    WHERE  r.ServiceRequestNo = @ServiceRequestNo
    AND    c.Category = 'SRSUPPLIER'
    AND    c.Codedescript = r.ChargeToMake

    SELECT @DelivererId = r.Deliverer
    FROM   SR_Resolution r
    WHERE  r.ServiceRequestNo = @ServiceRequestNo
    
    -- For the customer also return the deposit paid and required
    
    SELECT @CustomerId = CustId,
           @DepositAmount = s.DepositAmount
    FROM   SR_ServiceRequest s
    WHERE  s.ServiceRequestNo = @ServiceRequestNo

    SELECT @DepositPaid = -SUM(f.TransValue)
    FROM   FinTrans f
    WHERE  f.AcctNo = @CustomerAcctNo
    AND    f.TransTypeCode = 'PAY'

    SET @Return = @@error

    SET NOCOUNT OFF
    RETURN @Return
GO
SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO
