
if exists (select * FROM sysobjects where  name ='dn_DeliveriesandOrdersRemoveafterExport')
drop procedure dn_DeliveriesandOrdersRemoveafterExport 
go
create procedure [dbo].[dn_DeliveriesandOrdersRemoveafterExport] @runno int, @return int out
--------------------------------------------------------------------------------
--
-- Project      : CoSACS .NET
-- File Name    : dn_DeliveriesandOrdersRemoveafterExport.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        :
-- Author       : ??
-- Date         : ??
--
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 30/06/11  IP  CR1212 - RI - #3987 - RI Integration system changes.
--------------------------------------------------------------------------------
as
set @return = 0

--Cashloan amortization CR  to push details for broker at EOD on instal due dates
BEGIN TRAN
	BEGIN TRY
		EXEC [dbo].[CLAmortizationInsertBrokerDataDetailsSP]
		IF(@@trancount>0)
			COMMIT
END TRY
BEGIN CATCH 
	IF(@@trancount>0)
			ROLLBACK
END CATCH

BEGIN TRAN
update CLANewPaymentDetails
		set runno =@runno
		where runno = 0

update delivery set runno =@runno
from lastfactexport f
where f.type = 'D'
and f.acctno = delivery.acctno and f.ItemID = delivery.ItemID				--IP - 30/06/11 - CR1212 - RI - #3987
--and f.acctno = delivery.acctno and f.itemno = delivery.itemno
and f.contractno = delivery.contractno and delivery.runno = 0
and f.agrmtno = delivery.agrmtno

update delivery set runno =@runno
from lastfactexport f
where f.type = 'D'
--and f.acctno = delivery.acctno and f.originalitem = delivery.itemno -- also doing for the original item if different returned item
and f.acctno = delivery.acctno and f.OriginalItemItemID= delivery.ItemID -- also doing for the original item if different returned item --IP - 30/06/11 - CR1212 - RI - #3987
and f.contractno = delivery.contractno and delivery.runno = 0
and f.agrmtno = delivery.agrmtno
set @return = @@error

if @return = 0
-- add tos dont get interfaced but need to be stamped anyway
update delivery set runno =@runno										--IP - 30/06/11 - CR1212 - RI - #3987 - Replaces code below.
from stockinfo si
where delivery.ItemID = si.ID
and runno = 0 and si.iupc in ('adddr','addcr')	

--update delivery set runno =@runno where runno = 0 and itemno in ('adddr','addcr')

set @return = @@error
if @return = 0
begin
delete from facttrans where exists (select * from 
lastfactexport f
where f.acctno =facttrans.acctno and f.agrmtno =facttrans.agrmtno
--and f.itemno =facttrans.itemno and f.tccode =facttrans.tccode
and f.ItemID =facttrans.ItemID and f.tccode =facttrans.tccode
and f.trandate = facttrans.trandate)
set @return = @@error
end
COMMIT
return @return

