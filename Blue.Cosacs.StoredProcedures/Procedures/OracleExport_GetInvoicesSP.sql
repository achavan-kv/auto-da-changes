SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS OFF
GO

IF EXISTS (SELECT * FROM SysObjects
           WHERE id = object_id('[dbo].OracleExport_GetInvoicesSP') and OBJECTPROPERTY(id, 'IsProcedure') = 1)

BEGIN
    DROP PROCEDURE OracleExport_GetInvoicesSP
END
GO

CREATE PROCEDURE dbo.OracleExport_GetInvoicesSP 

-- ================================================
-- Project      : CoSACS .NET
-- File Name    : OracleExport_GetInvoicesSP.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Get Invoice data
-- Author       : John Croft
-- Date         : 21 July 2008
--
-- This procedure will get the Invoice data previously created in OracleExport_InvoiceCustomerReceiptSP
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 
-- ================================================
	-- Add the parameters for the stored procedure here
		@return int output

as
	set @return=0		--initialise return code

Select * from OracleAccountReceivables
	Order by acctno,InvoiceReference


-- End End End End End End End End End End End End End End End End End End End End End End End 


					