
IF EXISTS (SELECT 1 
            FROM   sysobjects 
            WHERE  id = object_id(N'[dbo].[DN_GetInvoiceNumberAndVersionByAcctnoSP]') 
                   and OBJECTPROPERTY(id, N'IsProcedure') = 1 )
BEGIN
    DROP PROCEDURE [dbo].[DN_GetInvoiceNumberAndVersionByAcctnoSP]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Ritesh Joge
-- Create date: 29/07/2020
-- Description:	Below sp return  latest invoice number and its version
--	        by account number
-- =============================================
CREATE PROCEDURE dbo.DN_GetInvoiceNumberAndVersionByAcctnoSP 	
	@acctno VARCHAR(12) NULL,
	@return INT OUTPUT
AS
BEGIN	
	SET NOCOUNT ON;

   SELECT TOP 1 IIF([AgreementInvNoVersion] IS NULL
					,''
					, CONCAT(SUBSTRING([AgreementInvNoVersion], 1, 3)
							,'-'
							,SUBSTRING([AgreementInvNoVersion], 4, LEN([AgreementInvNoVersion])),'-',ISNULL([InvoiceVersion],'')))
   FROM [dbo].[invoiceDetails] WHERE  [acctno] = @acctno 
   ORDER BY InvoiceVersion DESC

END
GO