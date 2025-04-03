-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

declare @itemcount int

;WITH WarrantySaleQty as 
(
	select ws.CustomerAccount, ws.AgreementNumber, ws.ItemId, ws.StockLocation, count(*) as WSCount 
			from warranty.warrantysale ws
			inner join lineitem l on ws.customeraccount = l.acctno
			and ws.AgreementNumber = l.agrmtno
			and ws.ItemId = l.ItemID
			and ws.StockLocation = l.stocklocn
			where ws.WarrantyContractNo = 'Man001'
			and l.quantity !=0
			group by ws.CustomerAccount, ws.AgreementNumber, ws.ItemId, ws.StockLocation

)

select 
	l.acctno, l.agrmtno, l.quantity, l.ItemID, l.stocklocn, wsq.WSCount, 0 as warrantySaleId
into 
	#items
from 
	lineitem l inner join WarrantySaleQty wsq on l.acctno = wsq.CustomerAccount
	and l.agrmtno = wsq.AgreementNumber
	and l.ItemID = wsq.ItemId
	and l.stocklocn = wsq.StockLocation
where 
	l.quantity >=1
	and wsq.WSCount > l.quantity



set @itemcount = @@ROWCOUNT
select @itemcount


while (@itemcount !=0)
begin

	;WITH WarrantySaleQty as 
	(
		select ws.CustomerAccount, ws.AgreementNumber, ws.ItemId, ws.StockLocation, count(*) as WSCount 
				from warranty.warrantysale ws
				inner join lineitem l on ws.customeraccount = l.acctno
				and ws.AgreementNumber = l.agrmtno
				and ws.ItemId = l.ItemID
				and ws.StockLocation = l.stocklocn
				where ws.WarrantyContractNo = 'Man001'
				and l.quantity !=0
				group by ws.CustomerAccount, ws.AgreementNumber, ws.ItemId, ws.StockLocation

	)

	select 
		l.acctno, l.agrmtno, l.quantity, l.ItemID, l.stocklocn, wsq.WSCount, 0 as warrantySaleId
	into 
		#itemstodelete
	from 
		lineitem l inner join WarrantySaleQty wsq on l.acctno = wsq.CustomerAccount
		and l.agrmtno = wsq.AgreementNumber
		and l.ItemID = wsq.ItemId
		and l.stocklocn = wsq.StockLocation
	where 
		l.quantity >=1
		and wsq.WSCount > l.quantity

	set @itemcount = @@ROWCOUNT

	update 
		#itemstodelete
	set 
		warrantySaleId = (select max(ws.id)
								from warranty.WarrantySale ws
								where ws.CustomerAccount = #itemstodelete.acctno
								and ws.AgreementNumber = #itemstodelete.agrmtno
								and ws.ItemId = #itemstodelete.ItemID
								and ws.StockLocation = #itemstodelete.stocklocn
								and ws.WarrantyContractNo = 'Man001')


		delete from 
			warranty.WarrantySale
		where exists(select * from #itemstodelete i
						where i.warrantysaleid = warranty.WarrantySale.Id)


		drop table #itemstodelete

end


