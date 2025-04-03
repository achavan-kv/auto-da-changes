SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS OFF
GO

if exists (select * from dbo.sysobjects
where id = object_id('[dbo].[DN_SRSaveSRCustomerSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_SRSaveSRCustomerSP]
GO


CREATE PROCEDURE dbo.DN_SRSaveSRCustomerSP
    @ServiceRequestNo       INTEGER,
    @Return                 INTEGER OUTPUT

AS 

DECLARE @Arrears    MONEY

    SET NOCOUNT ON
    SET @Return = 0


    -- Get the current arrears if this SR is for a Courts Account
    SELECT @Arrears = Arrears
    FROM   SR_ServiceRequest s, Acct a
    WHERE  s.ServiceRequestNo = @ServiceRequestNo
    AND    s.ServiceType = 'C'              -- Courts Account
    AND    a.AcctNo = s.AcctNo


    -- Save a snapshot of the Customer Details for this SR
    
    IF NOT EXISTS(SELECT * FROM SR_Customer WHERE ServiceRequestNo = @ServiceRequestNo)
    BEGIN
    INSERT INTO SR_Customer
       (ServiceRequestNo,
        CustId,
        Title,
        FirstName,
        LastName,
        Arrears,
        Address1,
        Address2,
        Address3,
        AddressPC,
        Directions,
        TelHome,
        TelWork,
        TelMobile)
    SELECT
        @ServiceRequestNo,
        c.CustId,
        c.Title,
        c.FirstName,
        c.Name,
        ISNULL(@Arrears,0),
        ISNULL(ca.CusAddr1,''),
        ISNULL(ca.CusAddr2,''),
        ISNULL(ca.CusAddr3,''),
        ISNULL(ca.CusPoCode,''),
        ISNULL(ca.Notes,''),
        ISNULL(ct1.DialCode + ' ' + ct1.TelNo,'') AS HomeTel,
        ISNULL(ct2.DialCode + ' ' + ct2.TelNo,'') AS WorkTel,
        ISNULL(ct3.DialCode + ' ' + ct3.TelNo,'') AS MobileNo
    FROM SR_ServiceRequest s, Customer c
    LEFT OUTER JOIN CustAddress ca
    ON   ca.CustId = c.custid AND ca.AddType = 'H' AND ISNULL(ca.DateMoved,'') = ''
    LEFT OUTER JOIN CustTel ct1
    ON   ct1.CustId = c.custid AND ct1.TelLocn = 'H' AND ISNULL(ct1.DateDiscon,'') = ''
    LEFT OUTER JOIN CustTel ct2
    ON   ct2.CustId = c.custid AND ct2.TelLocn = 'W' AND ISNULL(ct2.DateDiscon,'') = ''
    LEFT OUTER JOIN CustTel ct3
    ON   ct3.CustId = c.custid AND ct3.TelLocn = 'M' AND ISNULL(ct3.DateDiscon,'') = ''
    WHERE s.ServiceRequestNo = @ServiceRequestNo
    AND   c.CustId = s.CustId

	END
    
        UPDATE SR_ServiceRequest SET Updated = 0 WHERE ServiceRequestNo = @ServiceRequestNo 

    SET @Return = @@error

    SET NOCOUNT OFF
    RETURN @Return
GO
SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO

