SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS OFF
GO

if exists (select * from dbo.sysobjects
where id = object_id('[dbo].[DN_SRCustomerUpdateSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_SRCustomerUpdateSP]
GO


CREATE PROCEDURE dbo.DN_SRCustomerUpdateSP
    @ServiceRequestNo       INTEGER,
    @custID                 VARCHAR(20),
    @title                  VARCHAR(25),
    @firstName              VARCHAR(30),
    @lastName               VARCHAR(60),
    @arrears                MONEY,
    @address1               VARCHAR(50),
    @address2               VARCHAR(50),
    @address3               VARCHAR(50), --UAT 722
    @postCode               VARCHAR(10),
    @directions             VARCHAR(2000),
    @telHome                VARCHAR(30),
    @telWork                VARCHAR(30),
    @telMobile              VARCHAR(30),   
    @Return                 INTEGER OUTPUT

AS
   
    SET NOCOUNT ON
    SET @Return = 0

       IF NOT EXISTS (SELECT 1 FROM SR_Customer WHERE ServiceRequestNo = @ServiceRequestNo)
       BEGIN
	   	    INSERT INTO SR_Customer (
		   	ServiceRequestNo,
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
		   	VALUES ( 
		    @ServiceRequestNo,
		    @custID,
		   	@title,
            @firstName,
            @lastName,
            @arrears,
            @address1,
            @address2,
            @address3,
            @postCode,
            @directions,
            @telHome,
            @telWork,
            @telMobile) 
	   END
	   ELSE
	   BEGIN
    UPDATE SR_Customer
    SET Title = @title,
        FirstName = @firstName,
        LastName = @lastName,
        Arrears = @arrears,
        Address1 = @address1,
        Address2 = @address2,
        Address3 = @address3,
        AddressPC = @postCode,
        Directions = @directions,
        TelHome = @telHome,
        TelWork = @telWork,
        TelMobile = @telMobile
       WHERE ServiceRequestNo = @ServiceRequestNo AND CustId = @custID
	   END

        UPDATE SR_ServiceRequest SET Updated = 1 WHERE ServiceRequestNo = @ServiceRequestNo

    SET @Return = @@error

    SET NOCOUNT OFF
    RETURN @Return
GO
SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO

