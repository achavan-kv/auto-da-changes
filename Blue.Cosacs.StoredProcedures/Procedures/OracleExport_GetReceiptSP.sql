SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS OFF
GO

IF EXISTS (SELECT * FROM SysObjects
           WHERE id = object_id('[dbo].OracleExport_GetReceiptSP') and OBJECTPROPERTY(id, 'IsProcedure') = 1)

BEGIN
    DROP PROCEDURE OracleExport_GetReceiptSP
END
GO

CREATE PROCEDURE dbo.OracleExport_GetReceiptSP 

-- ================================================
-- Project      : CoSACS .NET
-- File Name    : OracleExport_GetReceiptSP.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Get Receipts data
-- Author       : John Croft
-- Date         : 21 July 2008
--
-- This procedure will get the Receipt date previously created in OracleExport_InvoiceCustomerReceiptSP
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

Select * from OracleReceipts


-- End End End End End End End End End End End End End End End End End End End End End End End 


					