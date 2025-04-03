-- Trigger for Audit of StockPrice table

go

IF EXISTS (SELECT * FROM sysobjects WHERE NAME= 'Trig_StockPrice_InsertUpdate')
DROP TRIGGER Trig_StockPrice_InsertUpdate
GO 

CREATE Trigger [dbo].[Trig_StockPrice_InsertUpdate] ON [dbo].[Stockprice]
For UPDATE, INSERT
-- ================================================
-- Project      : CoSACS .NET
-- File Name    : Trig_StockPrice_InsertUpdate.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Insert Update trigger for auditing
-- Date         : ??
--
-- This trigger will isert StockPrice changes into the audit table
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 12/04/11  jec Add ID column
-- ================================================
AS

INSERT INTO StockPriceAudit(ItemNo, BranchNo,CreditPrice, CashPrice, DutyFreePrice, CostPrice, Refcode,DateChange,ID)
	Select ItemNo, BranchNo,CreditPrice, CashPrice, DutyFreePrice, CostPrice, Refcode, GetDate(),ID
	From INSERTED I


-- End End End End End End End End End End End End End End End End End End End End End End End End	