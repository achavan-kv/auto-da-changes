
if exists (select * from sysobjects where name = 'Trig_StockQuantity_InsertUpdate')
drop trigger Trig_StockQuantity_InsertUpdate
GO

CREATE Trigger Trig_StockQuantity_InsertUpdate ON StockQuantity
For UPDATE, INSERT
-- ================================================
-- Project      : CoSACS .NET
-- File Name    : Trig_StockQuantity_InsertUpdate.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Insert Update trigger for auditing
-- Date         : ??
--
-- This trigger will insert tockQuantity changes into the audit table
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 12/04/11  jec Add ID column
-- 31/08/11  ip  RI - #4960 - RI System Integration
-- ================================================
AS

INSERT INTO StockQuantityAuditCosacs(ItemNo, StockLocn, QtyChange, DateChange,ID)
	Select I.ItemNo, I.StockLocn, (I.QtyAvailable - IsNull(D.QtyAvailable, 0)), GetDate(),I.ID
	From INSERTED I
	--LEFT JOIN DELETED D ON I.ItemNo = D.ItemNo and I.StockLocn = D.StockLocn
	LEFT JOIN DELETED D ON I.ID = D.ID and I.StockLocn = D.StockLocn										--IP - 31/08/11 
	Where I.LastOperationSource != 'ORACLE' and I.QtyAvailable != IsNull(D.QtyAvailable, 0)
	
--Delete records older than 6 months from the table but only for this itemno /stocklocn for performance /locking reasons
DELETE FROM StockQuantityAuditCosacs
WHERE DateChange < DATEADD(m, -6, GETDATE()) AND EXISTS 
--(SELECT * FROM inserted i WHERE i.itemno= StockQuantityAuditCosacs.itemno
(SELECT * FROM inserted i WHERE i.ID= StockQuantityAuditCosacs.ID											--IP - 31/08/11 
AND i.stocklocn = StockQuantityAuditCosacs.stocklocn )
GO 