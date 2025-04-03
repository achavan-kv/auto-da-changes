SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS OFF
GO

if exists (select * from dbo.sysobjects
where id = object_id('[dbo].[DN_SRGetCustomerSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_SRGetCustomerSP]
GO


CREATE PROCEDURE dbo.DN_SRGetCustomerSP
--------------------------------------------------------------------------------
--
-- Project      : CoSACS .NET
-- File Name    : DN_SRGetCustomerSP.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Get the Service Customer Details
-- Author       : ??
-- Date         : ??
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 25/11/10  jec CR1030 Check for different custid when searching by Cash &Go InvoiceNo 
--						as may have been changed from "PAID & TAKEN"
-- 25/05/11  IP  5.13 - #3736 (LW73645) - Previously error was received when saving a Stock Repair when closing. This was due to the large number of
--				 records that were being returned from SR_Customer for the COURTS custid (Stock Repair). Only need to return 1 record in this case.
--------------------------------------------------------------------------------
    @CustId             VARCHAR(20),
	@ServiceBranchNo	SMALLINT,
	@ServiceUniqueId	INTEGER,
    @AcctNo             CHAR(12),
    @InvoiceNo          INTEGER,
    @BranchNo           SMALLINT,
    @Return             INTEGER OUTPUT

AS DECLARE
    @RowCount INTEGER,
    @SRcustid VARCHAR(20),    --CR1030 jec
    @Arrears  MONEY

    SET NOCOUNT ON
    SET @RowCount = 0
    SET @Arrears = 0
    SET @Return = 0

    -- A Service Request customer can be loaded by any of:
    -- . Customer Id
    -- . Service Request Number
    -- . Courts Account Number
    -- . Cash & Go Invoice Number

    IF (@CustId = "")
    BEGIN
        IF (@ServiceUniqueId > 0)
        BEGIN
            -- Find the customer from the Service Request Number
            -- Make sure the account number is for this Service Request
            SELECT @CustId = CustId,
                   @AcctNo = AcctNo
            FROM   SR_ServiceRequest
            WHERE  ServiceBranchNo = @ServiceBranchNo
            AND    ServiceRequestNo = @ServiceUniqueId

            SET @RowCount = @@ROWCOUNT
        END


        IF (@RowCount = 0 AND @AcctNo != "" AND @AcctNo != '000000000000')
        BEGIN
           -- Find the customer from the Account Number
            SELECT @CustId = CustId
            FROM   CustAcct ca
            WHERE  AcctNo = @AcctNo
            AND    HldOrJnt = 'H'

            SET @RowCount = @@ROWCOUNT
        END

        IF (@RowCount = 0 AND @BranchNo > 0 AND @InvoiceNo > 0)
        BEGIN
            -- Find the customer from the Invoice Number
            -- Make sure the account number is for this invoice
            SELECT @CustId = ca.CustId,
                   @AcctNo = ca.AcctNo
            FROM   Agreement ag, CustAcct ca
            WHERE  ag.AgrmtNo = @InvoiceNo
            AND    LEFT(ag.AcctNo,3) = CONVERT(VARCHAR,@BranchNo)
            AND    ca.AcctNo = ag.AcctNo
            AND    ca.HldOrJnt = 'H'
            
            -- CR1030 jec 25/11/10
            -- try to find if an SR has already been created for this invoice number
            if @InvoiceNo>1		-- Cash&Go
            Begin
				select @SRCustid=s.custid
				from SR_ServiceRequest s
				where s.InvoiceNo=@InvoiceNo and s.AcctNo=@AcctNo
				-- if custid different use one from SR
				If @SRCustid is not null 
					set @custid=@SRCustid
			End
        END

        -- Get the arrears for the account
        SELECT @Arrears = Arrears
        FROM   Acct
        WHERE  AcctNo = @AcctNo
    END

	--Should return all customer details from both SR_Customer and Customer so that old and new details can be displayed at the front end

    IF (LTRIM(@CustId) != '')
    BEGIN
        -- Load from the snapshot if a snaphot has been saved
        --IF EXISTS (SELECT 1 FROM SR_Customer WHERE CustId = @CustId)
        --BEGIN
            IF(@CustId='COURTS')	--IP - 25/05/11 - 5.13 #3736 - Only return 1 record from SR_Customer if Stock Repair. This prevents thousands of records from being returned unecessarily.
			BEGIN
				SELECT top 1 ServiceRequestNo,CustId,
						Title,
						FirstName,
						LastName,
						Arrears,
						Address1 AS cusaddr1,
						Address2 AS cusaddr2,
						Address3 AS cusaddr3,
						AddressPC AS cuspocode,
						Directions,
						TelHome AS hometel,
						TelWork AS worktel,
						TelMobile AS mobileno
				FROM  SR_Customer
				WHERE CustId = @CustId
            END
            ELSE
            BEGIN
				SELECT ServiceRequestNo,CustId,
						Title,
						FirstName,
						LastName,
						Arrears,
						Address1 AS cusaddr1,
						Address2 AS cusaddr2,
						Address3 AS cusaddr3,
						AddressPC AS cuspocode,
						Directions,
						TelHome AS hometel,
						TelWork AS worktel,
						TelMobile AS mobileno
				FROM  SR_Customer
				WHERE CustId = @CustId
            END
        --END
        --ELSE
        --BEGIN
            SELECT c.CustId,
                   c.Title,
                   c.FirstName,
                   c.Name AS LastName,
                   @Arrears AS Arrears,
                   ca.cusaddr1,
                   ca.cusaddr2,
                   ca.cusaddr3,
                   ca.cuspocode,
                   ca.Notes AS Directions,
                   ct1.DialCode + ' ' + ct1.TelNo AS hometel,
                   ct2.DialCode + ' ' + ct2.TelNo AS worktel,
                   ct3.DialCode + ' ' + ct3.TelNo AS mobileno
            FROM Customer c
            LEFT OUTER JOIN CustAddress ca
            ON   ca.CustId = @Custid AND ca.AddType = 'H' AND ISNULL(ca.DateMoved,'') = ''
            LEFT OUTER JOIN CustTel ct1
            ON   ct1.CustId = @CustId AND ct1.TelLocn = 'H' AND ISNULL(ct1.DateDiscon,'') = ''
            LEFT OUTER JOIN CustTel ct2
            ON   ct2.CustId = @CustId AND ct2.TelLocn = 'W' AND ISNULL(ct2.DateDiscon,'') = ''
            LEFT OUTER JOIN CustTel ct3
            ON   ct3.CustId = @CustId AND ct3.TelLocn = 'M' AND ISNULL(ct3.DateDiscon,'') = ''
            WHERE c.CustId = @CustId
        --END
    END

    SET @Return = @@error

    SET NOCOUNT OFF
    RETURN @Return
GO
SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO

-- End End End End End End End End End End End End End End End End End End End End End End
