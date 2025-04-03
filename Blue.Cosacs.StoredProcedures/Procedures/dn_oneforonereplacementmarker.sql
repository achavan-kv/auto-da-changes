

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[dn_oneforonereplacementmarker]')
           and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[dn_oneforonereplacementmarker]
GO

CREATE PROCEDURE dn_oneforonereplacementmarker
-- ================================================
-- Project      : CoSACS .NET
-- File Name    : dn_oneforonereplacementmarker.prc
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        :
-- Author       : ?
-- Date         : ?
--
-- This procedure will create the interface file for Delivery Transfers.
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 22/06/11  IP   CR1212 - RI - #3987 - RI System Integration changes - Join on ItemID
-- ================================================
AS
SET NOCOUNT ON 
DECLARE @status     INTEGER
DECLARE @IRPeriod1  INTEGER
DECLARE @IRPeriod2  INTEGER
DECLARE @IRPeriod3  INTEGER

SELECT @IRPeriod1 = Value FROM CountryMaintenance WHERE CodeName = 'irperiod1'
SELECT @IRPeriod2 = Value FROM CountryMaintenance WHERE CodeName = 'irperiod2'
SELECT @IRPeriod3 = Value FROM CountryMaintenance WHERE CodeName = 'irperiod3'

UPDATE Delivery SET replacementmarker = '' WHERE runno = 0

--print 'instant replacement collections in Period ONE'

UPDATE Delivery SET ReplacementMarker = 'CW'
--WHERE EXISTS (SELECT * FROM stockitem 
--              WHERE stockitem.itemno = delivery.itemno
--              --and stockitem.stocklocn = delivery.stocklocn
--              and stockitem.category in (select distinct code from code where category = 'WAR')) 
WHERE EXISTS(SELECT * FROM StockInfo si
			 WHERE si.ID = delivery.ItemID
			 and si.category in (select distinct code from code where category = 'WAR'))	--IP - 22/06/11 - CR1212 - RI - #3987
			 
and transvalue < 0
and acctno like '___5%' 
and EXISTS (select * from delivery d2, warrantyband wb       -- original delivery
            where d2.acctno     = delivery.acctno
            and d2.agrmtno      = delivery.agrmtno
            and d2.contractno   = delivery.contractno
            --and d2.itemno       = delivery.itemno
            and d2.ItemID		= delivery.ItemID											--IP - 22/06/11 - CR1212 - RI - #3987
            and d2.transvalue   > 0
            --and wb.waritemno    = delivery.itemno  
            and wb.ItemID		= delivery.ItemID											--IP - 22/06/11 - CR1212 - RI - #3987			          
            and DATEADD(Day, @IRPeriod1, DATEADD(Month, @IRPeriod2, d2.datedel)) > delivery.datedel
            and DATEADD(Year, wb.WarrantyLength, d2.datedel) > delivery.datedel)
and delivery.runno = 0

--print 'instant replacement collections in Period TWO'

UPDATE Delivery SET ReplacementMarker = 'CT'
--WHERE EXISTS (SELECT * FROM stockitem 
--              WHERE stockitem.itemno = delivery.itemno
--              --and stockitem.stocklocn = delivery.stocklocn
--              and stockitem.category in (select distinct code from code where category = 'WAR')) 
WHERE EXISTS (SELECT * FROM StockInfo si													--IP - 22/06/11 - CR1212 - RI - #3987
              WHERE si.ID = delivery.ItemID
              --and stockitem.stocklocn = delivery.stocklocn
              and si.category in (select distinct code from code where category = 'WAR')) 
and transvalue < 0
and acctno like '___5%' 
and EXISTS (select * from delivery d2, warrantyband wb       -- original delivery
            where d2.acctno     = delivery.acctno
            and d2.agrmtno      = delivery.agrmtno
            and d2.contractno   = delivery.contractno
            --and d2.itemno       = delivery.itemno
            and d2.ItemID       = delivery.ItemID
            and d2.transvalue   > 0
            --and wb.waritemno    = delivery.itemno
            and wb.ItemID       = delivery.ItemID											--IP - 22/06/11 - CR1212 - RI - #3987
            and DATEADD(Day, @IRPeriod1, DATEADD(Month, @IRPeriod2, d2.datedel)) <= delivery.datedel
            and DATEADD(Day, @IRPeriod1, DATEADD(Month, @IRPeriod2+@IRPeriod3, d2.datedel)) > delivery.datedel
            and DATEADD(Year, wb.WarrantyLength, d2.datedel) > delivery.datedel)
and delivery.runno = 0

--print 'instant replacement collections in Period THREE up to warranty expiry'

UPDATE Delivery SET ReplacementMarker = 'AT'
--WHERE EXISTS (SELECT * FROM stockitem 
--              WHERE stockitem.itemno = delivery.itemno
--              --and stockitem.stocklocn = delivery.stocklocn
--              and stockitem.category in (select distinct code from code where category = 'WAR')) 
WHERE EXISTS (SELECT * FROM StockInfo si													--IP - 22/06/11 - CR1212 - RI - #3987
              WHERE si.ID = delivery.ItemID
              --and stockitem.stocklocn = delivery.stocklocn
              and si.category in (select distinct code from code where category = 'WAR')) 
and transvalue < 0
and acctno like '___5%' 
and EXISTS (select * from delivery d2, warrantyband wb       -- original delivery
            where d2.acctno     = delivery.acctno
            and d2.agrmtno      = delivery.agrmtno
            and d2.contractno   = delivery.contractno
            --and d2.itemno       = delivery.itemno
            and d2.ItemID       = delivery.ItemID											--IP - 22/06/11 - CR1212 - RI - #3987
            and d2.transvalue   > 0 
            --and wb.waritemno    = delivery.itemno
            and wb.ItemID		= delivery.ItemID											--IP - 22/06/11 - CR1212 - RI - #3987
            and DATEADD(Day, @IRPeriod1, DATEADD(Month, @IRPeriod2+@IRPeriod3, d2.datedel)) <= delivery.datedel
            and DATEADD(Year, wb.WarrantyLength, d2.datedel) > delivery.datedel)
and delivery.runno = 0

-- stockitems collected for instant replacement should be marked

update d set ReplacementMarker = w.ReplacementMarker
from delivery w, delivery d, lineitem lw
where d.runno = 0 
--and w.ReplacementMarker = 'CW'
and d.transvalue < 0
and lw.acctno = d.acctno
--and (lw.parentitemno = d.itemno)
and (lw.ParentItemID = d.ItemID)															--IP - 22/06/11 - CR1212 - RI - #3987
--and lw.parentlocation = d.stocklocn
and lw.agrmtno = d.agrmtno
and w.acctno = lw.acctno
--and w.itemno = lw.itemno
and w.ItemID = lw.ItemID																	--IP - 22/06/11 - CR1212 - RI - #3987
and w.agrmtno = lw.agrmtno
--and w.stocklocn = lw.stocklocn
and w.runno = 0
and d.acctno like '___5%' 
--and exists (select * from stockitem
--            where d.itemno = stockitem.itemno and stockitem.itemtype = 'S')
and exists (select * from StockInfo si
            where d.ItemID = si.ID and si.itemtype = 'S')									--IP - 22/06/11 - CR1212 - RI - #3987
and w.replacementmarker is not Null
and w.replacementmarker != ''

update d set ReplacementMarker = replacementmarker + isnull (p.reason, '')
from delivery d, productfaults p, lineitem l
where replacementmarker is not null and replacementmarker != ''
and d.acctno = p.acctno and d.agrmtno = p.agrmtno
and d.acctno = l.acctno and d.agrmtno = l.agrmtno
--and (d.itemno = l.parentitemno or d.itemno = l.itemno)  -- collected stock item number = parent of warranty
and (d.ItemID = l.ParentItemID or d.ItemID = l.ItemID)  -- collected stock item number = parent of warranty		--IP - 22/06/11 - CR1212 - RI - #3987
and d.runno = 0
--and datalength(d.replacementmarker) <= 2

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

