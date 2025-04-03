SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects
where id = object_id('[dbo].[DN_AuditReprintSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_AuditReprintSP]
GO

CREATE PROCEDURE  dbo.DN_AuditReprintSP
    @accountno   char(12),
    @agrmtno     int,
    @doctype     char(1),
    @printedby   int,
    @return      int OUTPUT

AS DECLARE

    @AgreementPrinted   CHAR(1),
    @TaxInvoicePrinted  CHAR(1),
    @WarrantyPrinted    CHAR(1)
    
BEGIN
    SET @return = 0

    -- Check whether the documents have been printed before
	SELECT @AgreementPrinted = AgreementPrinted,
	       @TaxInvoicePrinted = TaxInvoicePrinted,
	       @WarrantyPrinted = WarrantyPrinted
	FROM   Agreement
	WHERE  AcctNo = @accountno
	AND    AgrmtNo = @agrmtno
	
	IF (@doctype = 'A' AND @AgreementPrinted = 'Y')
	OR (@doctype = 'T' AND @TaxInvoicePrinted = 'Y')
	OR (@doctype = 'W' AND @WarrantyPrinted = 'Y')
	BEGIN
	    -- Audit the reprint of this document
	    INSERT INTO DocumentReprint
	        (AcctNo, AgrmtNo, DocType, DatePrinted, PrintedBy)
	    VALUES
	        (@accountno, @agrmtno, @doctype, GETDATE(), @printedby)
	END
	ELSE IF (@doctype = 'A')
	BEGIN
	    -- The first print of the Agreement
	    UPDATE Agreement SET AgreementPrinted = 'Y'
	    WHERE  AcctNo = @accountno
	    AND    AgrmtNo = @agrmtno
	END
	ELSE IF (@doctype = 'T')
	BEGIN
	    -- The first print of the Tax Invoice
	    UPDATE Agreement SET TaxInvoicePrinted = 'Y'
	    WHERE  AcctNo = @accountno
	    AND    AgrmtNo = @agrmtno
	END
	ELSE IF (@doctype = 'W')
	BEGIN
	    -- The first print of the Warranty
	    UPDATE Agreement SET WarrantyPrinted = 'Y'
	    WHERE  AcctNo = @accountno
	    AND    AgrmtNo = @agrmtno
	END

    SET @return = @@error
END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

