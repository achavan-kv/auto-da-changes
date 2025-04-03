SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS OFF
GO

IF EXISTS (SELECT * FROM SysObjects
           WHERE id = object_id('[dbo].OracleExport_TruncateSP') and OBJECTPROPERTY(id, 'IsProcedure') = 1)

BEGIN
    DROP PROCEDURE OracleExport_TruncateSP
END
GO

CREATE PROCEDURE dbo.OracleExport_TruncateSP 

-- ================================================
-- Project      : CoSACS .NET
-- File Name    : OracleExport_TruncateSP.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Get Customers data
-- Author       : John Croft
-- Date         : 21 July 2008
--
-- This procedure will trauncate the date created in OracleExport_InvoiceCustomerReceiptSP 
-- after a successfull export
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

	truncate table OracleAccountReceivables
	truncate table OracleCustomers
	truncate table OracleReceipts


-- End End End End End End End End End End End End End End End End End End End End End End End 