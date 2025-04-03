/*****************************************************************************************************************
** Author	: I.Parker
** Date		: 10th Novemver 2011
** Version	: 1.0
** Name		: tr_lineitembfcollection_Delete.sql
** Details	: Reverses discounts linked to an item if the item was collected in the Goods Return screen
**			  and subsequently the collection is cancelled through the Cancel Collection Notes screen.
**			  Deferred Terms is also reversed, and depending on the Countries Agreement Tax Type Deferred Terms
**			  on the tax is reversed and tax reversed.
** Modified	:
** Who	When	  Description
** ---  ----      -----------
** IP	10/11/11  Creation
******************************************************************************************************************/
--IF EXISTS(SELECT name FROM sysobjects WHERE name = 'tr_lineitembfcollection_Delete' 
--  	  		AND type = 'TR')

--BEGIN
--	DROP TRIGGER tr_lineitembfcollection_Delete
--END
--GO
--CREATE TRIGGER tr_lineitembfcollection_Delete
--ON  lineitembfcollection 
--FOR delete
--AS

--DECLARE @acctno varchar(12), @agrmtno int, @itemID int, @rowCount int, @dtItemID int, @staxItemID int, @scPcent float, @accttype varchar(1), @agrTaxType varchar(1)

--SELECT @acctno = acctno, @agrmtno = agrmtno, @itemID = itemID 
--FROM deleted 


--create table #discounts
--(
--	ItemID int,
--	ItemNo varchar (18),
--	StockLocn int,
--	Qty int,
--	OrdVal money,
--	TaxAmt money,
--	Empeeno int,
--	SCPcent float,
--	Term int,
--	DtItemID int,
--	DtVal money,
--	DtValTax money,
--	DtTot money,
--	RowID int IDENTITY(1,1)
--)

--set @dtItemID = (select ID from stockinfo where iupc = 'DT')
--set @staxItemID = (select ID from stockinfo where iupc = 'STAX')
--set @accttype = (select accttype from acct where acctno = @acctno) 
--set @agrTaxType = (select value from CountryMaintenance where codename = 'agrmttaxtype')

--insert into #discounts
--select l.ItemID, si.IUPC, la.StockLocn, la.QuantityBefore, la.ValueBefore,la.TaxAmtBefore, la.Empeenochange,0,ip.instalno, @dtItemID, 0,0,0
--from lineitem l inner join stockinfo si on l.itemID = si.ID
--inner join code c on si.category = c.code 
--inner join lineitemaudit la on la.acctno = l.acctno and la.ItemID = l.ItemID
--left join instalplan ip on l.acctno = ip.acctno
--where l.acctno = @acctno
--and l.ParentItemID = @itemID
--and l.ordval = 0
--and c.category = 'PCDIS'
--and la.datechange = (select max(datechange) 
--						from lineitemaudit la2
--						where la2.acctno = la.acctno
--						and la2.ItemID = la.ItemID)
--and si.Repossesseditem = 0
--and exists( select * from delivery d
--				where d.acctno = l.acctno
--				and d.ItemID = l.ItemID
--				and d.StockLocn = l.StockLocn
--				and d.Delorcoll = 'D')


--set @rowCount = @@ROWCOUNT

----Retrieve the Service Percent used at the time the account was opened to calculate the Deferred Terms to be reversed on the discounts being reversed.
--update #discounts
--set SCPcent = isnull((select i.intrate from intratehistory i inner join acct a on i.termstype = a.termstype
--					inner join instalplan ip on ip.acctno = a.acctno
--					where a.acctno = @acctno
--					and i.band = ip.scoringband
--					and a.dateacctopen between i.datefrom and case when i.dateto = '1900-01-01' then getdate() else i.dateto end),0)


--IF(@rowCount > 0)
--BEGIN

--	--Update ordval of the discount to what it was prior to cancellation
--	update lineitem
--	set Ordval = d.OrdVal,
--		Quantity = d.Qty,
--		DelQty = d.Qty,
--		TaxAmt = d.TaxAmt
--	from #discounts d
--	where lineitem.acctno = @acctno
--	and lineitem.ItemID = d.ItemID
--	and lineitem.StockLocn = d.StockLocn
	
--	--Reverse the collection of the discount
--	insert into delivery(origbr, acctno, agrmtno, datedel, delorcoll, itemno, stocklocn, quantity, retitemno, retstocklocn, retval, 
--	buffno, buffbranchno, datetrans, branchno, transrefno, transvalue, runno, contractno, ReplacementMarker, NotifiedBy, ftnotes, 
--	InvoiceLineNo, ExtInvoice, ParentItemNo, ItemID, ParentItemID, RetItemID)
--	select 0, @acctno, @agrmtno, getdate(), 'D', d.ItemNo, d.StockLocn, d.Qty, '', 0, 0,
--	b.hibuffno + d.RowID, d.StockLocn, getdate(), d.StockLocn, b.hirefno + d.RowID, d.OrdVal, 0, '', '',d.Empeeno , 'CCN',
--	null, null, si.IUPC, d.ItemID, @itemID,0
--	from #discounts d
--	inner join branch b on d.StockLocn = b.branchno
--	inner join stockinfo si on si.ID = @itemID
--	where si.Repossesseditem = 0
	
--	--For credit accounts we need to reverse the Deferred Terms
--	IF(@accttype != 'C')
--	BEGIN
	
--		update #discounts
--		set DtVal = #discounts.OrdVal * (#discounts.SCPcent/100) * #discounts.Term/12
		
--		--Calculate the Deferred Terms on the tax portion of the discount (only if tax is exsclusive)
--		IF(@agrTaxType = 'E')
--		BEGIN
--			update #discounts
--			set DtValTax = #discounts.TaxAmt * (#discounts.SCPcent/100) * #discounts.Term/12
--		END
		
--		--Total Deferred Terms to reverse
--		update #discounts
--		set DtTot = #discounts.DtVal + #discounts.DtValTax
		
--		insert into delivery(origbr, acctno, agrmtno, datedel, delorcoll, itemno, stocklocn, quantity, retitemno, retstocklocn, retval, 
--		buffno, buffbranchno, datetrans, branchno, transrefno, transvalue, runno, contractno, ReplacementMarker, NotifiedBy, ftnotes, 
--		InvoiceLineNo, ExtInvoice, ParentItemNo, ItemID, ParentItemID, RetItemID)
--		select 0, @acctno, @agrmtno, getdate(), 'D', 'DT', d.StockLocn, 1, '', 0, 0,
--		b.hibuffno + d.RowID, d.StockLocn, getdate(), d.StockLocn, b.hirefno + d.RowID, d.DtTot , 0, '', '',d.Empeeno , 'CCN',
--		null, null, '', d.DtItemID, 0,0
--		from #discounts d
--		inner join branch b on d.StockLocn = b.branchno
		
--		--Update the order value and price of the DT back to what it was prior to cancellation
--		update lineitem 
--		set ordval = lineitem.ordval +  (select sum(d.DtTot) from #discounts d),
--		price = price + (select sum(d.DtTot) from #discounts d)
--		from #discounts d
--		where lineitem.acctno = @acctno
--		and lineitem.ItemID = d.DtItemID
		
--		--update agreement total with the total of dt being reversed
--		update agreement 
--		set agrmttotal = agrmttotal + (select sum(d.DtTot)from #discounts d),
--			servicechg = servicechg + (select sum(d.DtTot)from #discounts d)
--		where agreement.acctno = @acctno
--		and agrmtno = @agrmtno
	
--		update acct 
--		set agrmttotal = agrmttotal + (select sum(d.DtTot)from #discounts d)
--		where acct.acctno = @acctno
		
--	END
		
		
--		IF(@agrTaxType = 'E')
--		BEGIN 
--			--Inserting a reversal on STAX for each discount if the discount has a tax amount. This needs to be done as at the point when collection notes 
--			--are cancelled the TaxAmt column is not updated for the discount and the code therefore posts a collection for the tax as it thinks there is now a difference in the tax
--			insert into delivery(origbr, acctno, agrmtno, datedel, delorcoll, itemno, stocklocn, quantity, retitemno, retstocklocn, retval, 
--			buffno, buffbranchno, datetrans, branchno, transrefno, transvalue, runno, contractno, ReplacementMarker, NotifiedBy, ftnotes, 
--			InvoiceLineNo, ExtInvoice, ParentItemNo, ItemID, ParentItemID, RetItemID)
--			select 0, @acctno, @agrmtno, getdate(), 'D', 'STAX', d.StockLocn, 1, '', 0, 0,
--			b.hibuffno + d.RowID, d.StockLocn, getdate(), d.StockLocn, b.hirefno + d.RowID, d.TaxAmt , 0, '', '',d.Empeeno , 'CCN',
--			null, null, '', @staxItemID, 0,0
--			from #discounts d
--			inner join branch b on d.StockLocn = b.branchno
--			where d.TaxAmt != 0

--			update lineitem 
--			set OrdVal = (select sum(TaxAmt) from lineitem l
--							where l.acctno = lineitem.acctno
--							and l.ItemID != @staxItemID),
--				Price = (select sum(TaxAmt) from lineitem l
--							where l.acctno = lineitem.acctno
--							and l.ItemID != @staxItemID)
--			where lineitem.acctno = @acctno
--			and lineitem.ItemID = @staxItemID
			
--			update agreement
--			set agrmttotal = agrmttotal + (select sum(d.TaxAmt) from #discounts d),
--				cashprice = cashprice + (select sum(d.TaxAmt) from #discounts d)
--			where agreement.acctno = @acctno
--			and agrmtno = @agrmtno
			
--			update acct
--			set agrmttotal = agrmttotal + (select sum(d.TaxAmt) from #discounts d)
--			where acct.acctno = @acctno
			 
			
--		END
		
--		update branch 
--		set hirefno = hirefno + @rowCount,
--			hibuffno = hibuffno + @rowCount
--		from #discounts d
--		where branch.branchno = d.StockLocn
			
		
--		--Update agreement totals back to what they were prior to cancelling
--		update agreement 
--		set agrmttotal = agrmttotal + (select sum(d.OrdVal)from #discounts d),
--			cashprice = cashprice + (select sum(d.OrdVal)from #discounts d)
--		where agreement.acctno = @acctno
--		and agrmtno = @agrmtno
		
--		update acct 
--		set agrmttotal = agrmttotal + (select sum(d.OrdVal)from #discounts d)
--		where acct.acctno = @acctno
		
--		--Update the outstanding balance
--		update acct 
--		set outstbal = (select sum(transvalue) from fintrans
--							where fintrans.acctno = acct.acctno) 
--		where acct.acctno = @acctno
	
--END

