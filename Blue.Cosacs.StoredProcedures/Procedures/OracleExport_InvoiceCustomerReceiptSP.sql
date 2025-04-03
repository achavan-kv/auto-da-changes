SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS OFF
GO

IF EXISTS (SELECT * FROM SysObjects
           WHERE id = object_id('[dbo].OracleExport_InvoiceCustomerReceiptSP') and OBJECTPROPERTY(id, 'IsProcedure') = 1)

BEGIN
    DROP PROCEDURE OracleExport_InvoiceCustomerReceiptSP
END
GO
CREATE PROCEDURE dbo.OracleExport_InvoiceCustomerReceiptSP 

-- ================================================
-- Project      : CoSACS .NET
-- File Name    : OracleExport_InvoiceCustomerReceiptSP.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Export Account Receivables, Customer and Receipts
-- Author       : John Croft
-- Date         : 17 July 2008
--
-- This procedure will process Deliveries, Customers and Receipts to Oracle Export tables.
-- The procedure is run as part of the Cosacs to FACT EOD job.
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 08/08/08 jec  Modify calc of unit Price/Line value to cater for Non stocks
-- 11/08/08 jec  Convert Customer ID to uppercase.
-- 12/08/08 jec  Changes re create external invoice no.
--               Add customers for which there is only a receipt.
--               Change GL date to be actual date i.e EOD date
-- 15/08/08 jec  Create "Credit Memo" if only credit values on invoice and not Collection or Repo.
-- 18/08/08 jec  Further changes re Credit/Debit Memo 
-- 19/08/08 jec  Use delivery transvalue not lineitem price
-- 21/08/08 jec  Set null values to zero
-- 21/08/08 jec  isnull(l.taxamt)
-- 03/09/08 jec  Left outer join to stockitem and 'Not known' description
-- 15/10/08 jec  Only calc values net of tax for current runno (Mauritius)
-- 29/10/08 jec  Add Delivery date and RETITEMNO to Invoice
-- 03/11/08 jec  70367 Procedure only required for Mauritius and Madagascar.
-- 11/11/08 jec  70425 Receipt No duplicating for XFR
-- 24/11/08 jec  70504 set RetItemno= '.' instead of blank 
-- 26/11/08 jec  70514 generate unique receipt numbers.
-- 04/12/08 jec  70514 mod to generate unique receipt numbers.
-- 11/08/09 jec  create new invoice on change of delivery date (for reconcilliation with New XML)
-- 16/11/09 aa   removing old data to improve performance and also excluding tax from export
-- ===============================================================================================
	-- Add the parameters for the stored procedure here
		@RunnoFact int,
		@runnoUPDSMRY int,
		@return int output

as
	set @return=0		--initialise return code
	return -- returning as Mauritius don't require this any more with realtime interface.  
GO 
--set nocount on 
--DELETE FROM OracleReceiptTranRef WHERE datetrans < DATEADD(month, -1, GETDATE())
----IF not EXISTS (SELECT table_name FROM INFORMATION_SCHEMA.TABLES
----	   WHERE  table_schema = 'dbo' and table_name = 'OracleAccountReceivables')

--Declare @countrycode CHAR(1)
--set @countrycode=(select countrycode from CountryMaintenance where CodeName='countrycode')

----if @countrycode in('M','C')		-- 70367 jec 03/11/08 
--IF (SELECT value FROM countrymaintenance
--      WHERE codeNAME = 'OracleLineExport') != 'N' -- Stephen chong 23/07/09 71473
--Begin
--		-- Only for Mauritius and Madagascar!!

---- select Delivery Info
--insert into OracleAccountReceivables	
--select 
--		-- Header 
--		case
--			when SUBSTRING(d.acctno,4,1) <'4' then 'Credit'
--			else 'Cash'
--		end as TranType,
--		Case
--			when d.delorcoll='D' then 'Invoice' 
--			when d.delorcoll='C' then 'Credit Memo' 
--			when d.delorcoll='R' then 'Repossession'		-- Repossession
--		end as TranClass,
--		d.datetrans as TranDate,GETDATE() as GLDate,ag.empeenosale,
----		case 
----			when d.buffbranchno !=0 then CAST(buffbranchno as CHAR(3)) + CAST(buffno as varchar(7)) 
----			else CAST(buffno as varchar(10))
----		end 
--		ISNULL(ExtInvoice,'') as InvoiceReference,
--		Case 
--			when d.delorcoll!='D' then CAST(d.delorcoll as varchar(10))
--			else CAST(' ' as varchar(10))
--		end as CredInvRef,
--		case 
--			when SUBSTRING(d.acctno,4,1) >='4' then 0
--			else i.instalno 
--		end as PayTerm,
--		cp.empeename as SalesPerson,
--		UPPER(ca.custid) as CustomerID,d.acctno,ISNULL(ba.AddressID,0) as BillAdrRef,
--		ISNULL(sa.AddressID,0) as ShipAdrRef,
--		-- Line
--		d.itemno,CAST(REPLACE(isnull(s.itemdescr1,'Not Known'),',',' ') as varchar(25)) as LineDescription,'Unit' as UOM,
--		d.quantity,
--		case
--			when l.itemtype='N' and l.quantity!=0 then ROUND(d.transvalue-(/*l.taxamt*/ 0/l.quantity),2) 	-- non stock (l.price could be 0) Exclusive of tax
--			when d.quantity!=0 then ROUND((d.transvalue-(ISNULL(/*l.taxamt*/ 0,0)/d.quantity))/d.quantity,2) 	-- Exclusive of tax
--			else d.transvalue-ISNULL(/*l.taxamt*/ 0,0)		-- 11/08/08	
--		end as UnitPrice,
--		case
--			when l.itemtype='N' and l.quantity!=0 then ROUND(d.transvalue/*-l.taxamt*/ /l.quantity,2) 	-- non stock (l.price could be 0) Exclusive of tax
--			when d.quantity!=0 then ROUND((d.transvalue-(ISNULL(/*l.taxamt*/ 0,0)/d.quantity)),2)   -- Exclusive of tax
--			else d.transvalue-ISNULL(/*l.taxamt*/ 0,0)
--		end as LineValue,
--		case	
--			--when ct.agrmttaxtype='I' then 'N'		-- Tax Inclusive??
--			when l.taxamt=0 then 'N'		-- No Tax
--			else 'Y'
--		end as TaxFlag,
--		cm.value as TaxCode,ct.TaxRate,ISNULL(d.InvoiceLineNo,0) as LineRef,

--		'AcctCode??' as AccountCode,
--		d.acctno as kacctno,d.agrmtno as kagrmtno,d.itemno as kitemno,d.stocklocn as kstocklocn,
--		d.contractno as kcontractno,d.runno,SUBSTRING(d.acctno,1,3) as BranchNo, d.transrefno as ktransrefno,
--		d.datedel as DelDate,
--		RetItemNo =case when d.delorcoll='R' then d.Retitemno else '.' end -- jec 24/11/08 70504
		
--From Delivery d INNER JOIN custacct ca on d.acctno = ca.acctno and hldorjnt='H'
--				INNER JOIN agreement ag on ag.acctno=d.acctno and ag.agrmtno=d.agrmtno
--				left outer join instalplan i on ag.acctno = i.acctno and ag.agrmtno = i.agrmtno
--				LEFT outer JOIN lineitem l on l.acctno = d.acctno and d.agrmtno = l.agrmtno and d.itemno=l.itemno -- item may not exist on lineitem ???
--							and d.stocklocn=l.stocklocn and l.contractno=d.contractno
--				left outer join custaddress ba on ca.custid=ba.custid and ba.addtype='H' and ba.datemoved is null -- Home address				
--				left outer join custaddress sa on ca.custid=sa.custid and sa.addtype=l.deliveryaddress and sa.datemoved is null-- Delivery address
--				left outer join courtsperson cp on cp.empeeno=ag.empeenosale
--				LEFT outer JOIN stockitem s on d.itemno=s.itemno and d.stocklocn=s.stocklocn,
--				country ct,CountryMaintenance cm

--Where runno = @runnoFACT --  equal last Fact Run
--	and cm.CodeName='taxname'
--	and d.itemno!='RB'		-- rebates are in receipts
	
---- set null unitprice and linevalue to zero (quick fix)
--update OracleAccountReceivables set unitprice=0 where unitprice is null
--update OracleAccountReceivables set linevalue=0 where linevalue is null
	
---- Now update Tran Class for Credit/Debit Memo
--IF (@@error = 0)
--	BEGIN
		
--		Update OracleAccountReceivables
--			set TranClass= case
--					when LineValue<0 and itemdescr1 not like '%disco%'	-- not a discount
--									then 'Credit Memo'
--					when LineValue>0 and itemdescr1 like '%disco%'		-- is a discount
--									then 'Debit Memo'
--						else TranClass
--						end 
--		from OracleAccountReceivables ar INNER JOIN stockitem s on ar.kitemno=s.itemno 
--							and ar.kstocklocn=s.stocklocn
--		Where TranClass ='Invoice'
	
--	END

--IF (@@error = 0)
--	BEGIN
---- Update InvoiceLineNo

--Declare @PrevAcct varchar(12),
--		@PrevAgrmtno int,
--		@InvLineNo int,
--		@acctno char(12),
--		@itemno varchar(8),
--		@Previtemno varchar(8),
--		@agrmtno int,
--		@transrefno int,
--		@stocklocn smallint,
--		@contractno varchar(10),
--		@TranClass varchar(12),
--		@PrevTranClass varchar(12),
--		@PrevDateDel datetime,		-- jec 11/08/09
--		@DateDel datetime,		-- jec 11/08/09
--		@InvCount int,
--		@InvoiceReference varchar(10),
--		@PrevBranch CHAR(3),
--		@BranchNo CHAR(3),
--		@InvoiceNumber int,
--		@LineValue money,
--		@FirstLineValue money,
--		@NewReceiptNo bigint,		-- 70514 jec
--		@OldReceiptNo varchar(19),	-- 70514 jec
--		@ReceiptRunNo int,			-- 70514 jec
--		@OracleReceiptNo varchar(19),	-- 70514 jec
--		@ReceiptDate datetime		-- 70514 jec
		
	
--set @PrevAcct=''
--set @PrevAgrmtno=0
--set @InvCount=0
--set @PrevBranch=''
--set @PrevTranClass=''
--set @Previtemno=''
--set @FirstLineValue=0
--set @PrevDateDel='1900-01-01'		-- jec 11/08/09

--Declare Line cursor for
--	select d.acctno,d.agrmtno,d.itemno,stocklocn,contractno,TranClass,d.transrefno,LineValue,
----		case 
----			when d.buffbranchno !=0 then CAST(buffbranchno as CHAR(3)) + CAST(buffno as varchar(7)) 
----			else CAST(buffno as varchar(10))
----		end as InvoiceReference,
--		SUBSTRING(d.acctno,1,3) as BranchNo,	-- branch from account no.
--		datedel			-- jec 11/08/09	
--		from delivery d INNER JOIN OracleAccountReceivables ar on kacctno = d.acctno 
--				and d.agrmtno = kagrmtno and d.itemno=kitemno 
--							and d.stocklocn=kstocklocn and kcontractno=d.contractno
--							and ktransrefno=d.transrefno
--	Where d.runno = @runnoFACT --  equal last Fact Run
--		and ExtInvoice is null	-- not already updated
--	Order by d.acctno,d.agrmtno,d.datedel,TranClass desc, LineValue desc    -- jec 11/08/09 deldate order -- so invoice before credit memo
----		,case 
----			when d.buffbranchno !=0 then CAST(buffbranchno as CHAR(3)) + CAST(buffno as varchar(7)) 
----			else CAST(buffno as varchar(10))
----		end

--Open Line
--Fetch from Line into @acctno,@agrmtno,@itemno,@stocklocn,@contractno,@TranClass,@transrefno,
--				@LineValue,@BranchNo,@DateDel			-- jec 11/08/09
--	WHILE	@@fetch_status = 0
--	BEGIN

--	if @PrevBranch!=@BranchNo		-- Branch changes
--	BEGIN
--		-- update hi invoice no for prev branch
--		update branch
--			set hiExtInvoiceNo=hiExtInvoiceNo+@InvCount 
--		where branchno=@PrevBranch
--		-- get start invoice no for current branch
--		set @InvoiceNumber=(select hiExtInvoiceNo from branch where branchno=@BranchNo)
--		set @InvCount=0
--		set @PrevBranch=@BranchNo
--	END		

--	if @PrevAcct!=@acctno		-- Account changes
--		or @PrevAgrmtno!=@agrmtno	-- Agreemtnt no changes
--		or @PrevTranClass!=@TranClass	-- Class changes
--		or (@Previtemno!=@itemno and @TranClass!='Invoice')	-- item changes and class not invoice
--		or @PrevDateDel!=@DateDel			-- Delivery date changes jec 11/08/09
--	BEGIN
--		set @PrevAcct=@acctno
--		set @PrevAgrmtno=@agrmtno
--		set @PrevTranClass=@TranClass
--		set @Previtemno=@itemno
--		set @PrevDateDel=@DateDel			-- jec 11/08/09		
--		set @InvLineNo=0				-- reset line no
--		set @InvCount=@InvCount+1
--		set @FirstLineValue=0			-- 		
--	END		
		
--	set @InvLineNo=@InvLineNo+1
--	if @InvLineNo=1
--		set @FirstLineValue=@LineValue
	
--	set @InvoiceReference=SUBSTRING(@acctno,1,3) + CAST(@InvoiceNumber+@InvCount as varchar(7))
--	-- Update Account receivables
--	Update OracleAccountReceivables
--		set LineRef=@InvLineNo, InvoiceReference=@InvoiceReference
----		,
----			TranClass=case
----						when @FirstLineValue<0 and TranClass ='Invoice' then 'Credit Memo'
----						else TranClass
----						end
--	from OracleAccountReceivables ar INNER JOIN branch b on ar.BranchNo=b.branchno	
--		where kacctno=@acctno and kagrmtno=@agrmtno and kitemno=@itemno 
--				and kstocklocn=@stocklocn and kcontractno=@contractno
--				and ktransrefno=@transrefno and TranClass=@TranClass 
--				--and InvoiceReference=@InvoiceReference
	
--	-- Update Delivery table 
--	Update Delivery
--		set InvoiceLineNo=@InvLineNo,ExtInvoice=@InvoiceReference
--		where acctno=@acctno and agrmtno=@agrmtno and itemno=@itemno 
--				and stocklocn=@stocklocn and contractno=@contractno 
--				and transrefno=@transrefno
--				and runno=@runnoFACT
----				and case 
----					when buffbranchno !=0 then CAST(buffbranchno as CHAR(3)) + CAST(buffno as varchar(7)) 
----					else CAST(buffno as varchar(10))
----				end=@InvoiceReference	

--	-- get next row
--	fetch next from Line into @acctno,@agrmtno,@itemno,@stocklocn,@contractno,@TranClass,@transrefno,
--					@LineValue,@BranchNo,@DateDel			-- jec 11/08/09

--	End

--	-- update hi invoice no for last branch
--	update branch
--		set hiExtInvoiceNo=hiExtInvoiceNo+@InvCount 
--	where branchno=@PrevBranch

--	close Line
--	Deallocate Line

--	END

--IF (@@error = 0)
--	BEGIN
---- Update Credit Invoice Ref for cancellation/Returns
--update OracleAccountReceivables 
--		set CredInvRef = 
--		case 
--			when ExtInvoice is null and d.buffbranchno !=0 then CAST(buffbranchno as CHAR(3)) + CAST(buffno as varchar(7))
--			when ExtInvoice is null and d.buffbranchno =0 then CAST(buffno as varchar(10))
--			else ExtInvoice
--		end 

--	from delivery d INNER JOIN OracleAccountReceivables ar on ar.acctno=d.acctno 
--		and ar.kagrmtno=d.agrmtno and ar.itemno=d.itemno and ar.kstocklocn=d.stocklocn
--		and ar.kcontractno=d.contractno and ar.ktransrefno!=d.transrefno
--		and d.delorcoll='D' and d.datetrans<=ar.trandate 
--	 and TranClass!= 'Invoice'
--		--and d.runno=ar.runno

--	END

---- Update taxrate and taxcode for Mauritius (as taxrate in Cosacs is 0)
--if @countrycode='M'
--Begin
---- 
--update OracleAccountReceivables
--	set taxrate=15, taxflag='Y', Taxcode='VAT'
---- Set specific categories to zero tax rate
--update OracleAccountReceivables
--SET taxrate = 0, taxflag='N'
--	from OracleAccountReceivables o INNER JOIN stockitem s on o.kitemno=s.itemno 
--							and o.kstocklocn=s.stocklocn
--    WHERE  unitpricehp = 0
--		AND category not in (14, 84, 36, 37, 38, 46, 47, 48, 86, 87, 88)

---- Reduce unit price and Line value by calculated tax value
--/*update OracleAccountReceivables
--	set UnitPrice=round(UnitPrice*100/(100+TaxRate 0),2), LineValue=ROUND(LineValue*100/(100+TaxRate 0),2)
--		where runno = @runnoFACT --  equal last Fact Run	-- jec 15/10/08
--	*/

--End


--IF (@@error = 0)
--	BEGIN
---- Receipts
----IF EXISTS (SELECT table_name FROM INFORMATION_SCHEMA.TABLES
----	   WHERE  table_schema = 'dbo' and table_name = 'Receipts')
----	drop table Receipts
--Insert into OracleReceipts
--Select  CAST(f.branchNo as CHAR(3)) + case	
--	-- make receipt no std 10 digits
----Select  case
--				when LEN(CAST(f.transrefno as varchar(10))) =7 then CAST(f.transrefno as varchar(10))
--				when LEN(CAST(f.transrefno as varchar(10))) =6 then '0' + CAST(f.transrefno as varchar(10))
--				when LEN(CAST(f.transrefno as varchar(10))) =5 then '00' + CAST(f.transrefno as varchar(10))
--				when LEN(CAST(f.transrefno as varchar(10))) =4 then '000' + CAST(f.transrefno as varchar(10))
--				when LEN(CAST(f.transrefno as varchar(10))) =3 then '0000' + CAST(f.transrefno as varchar(10))
--				when LEN(CAST(f.transrefno as varchar(10))) =2 then '00000' + CAST(f.transrefno as varchar(10))
--				when LEN(CAST(f.transrefno as varchar(10))) =1 then '000000' + CAST(f.transrefno as varchar(10))	
--				else CAST(f.transrefno as varchar(10))
--				end as receiptNo,
--		f.datetrans as ReceiptDate,'R' as CurrencyCode, UPPER(ca.custid) as custid,f.acctno,ROUND(f.transvalue,2) as ReceiptAmount,
--		CAST(0 as varchar(10)) as InvoiceReference,
--		f.transvalue as AppliedAmount,f.empeeno as CosacsUser,
--		ISNULL(Paymethod,0) as Paymethod,f.transTypeCode as TranType,f.ChequeNo as Chq_CredCard,b.bankname,f.runno,null

--From fintrans f INNER JOIN custacct ca on f.acctno=ca.acctno and hldorjnt='H'
--				LEFT OUTER JOIN bank b on f.bankcode=b.bankcode

--where  transtypecode not in('DEL','GRT','REP','RDL')	-- Deliveries/Returns/Reposs/ReDeliver
			
--		and runno = @runnoUPDSMRY -- equal last UPDSMRY Run	 NOTE: FACT and UPDSMRY runnos should be aligned	
--	END

---- Now generate unique Receipt number and save in OracleReceiptTranRef xref table
--IF (@@error = 0)
--	BEGIN
	
--	set @NewReceiptNo=(select HiExtReceiptNo from country)
--	-- as the routine can be run for previous runno's, need to join to OracleReceiptTranRef to see if the receipt number has been allocated previously
--	-- i.e. a previous selection and this is a re-run
--	if exists (select r.acctno, r.receiptNo, r.ReceiptDate, t.OraclereceiptNo 
--			from OracleReceipts r LEFT OUTER JOIN OracleReceiptTranRef t 
--					on r.acctno=t.acctno and r.receiptno=t.CosacsReceiptNo and r.ReceiptDate=t.datetrans
--			where isnull(right(t.OraclereceiptNo,10),0)  not between '1000000000' and '2147483647') -- highest possible for integer )
--	begin 
--		Declare Recpt cursor for
--			select r.acctno, r.receiptNo, r.ReceiptDate, t.OraclereceiptNo 
--				from OracleReceipts r LEFT OUTER JOIN OracleReceiptTranRef t 
--						on r.acctno=t.acctno and r.receiptno=t.CosacsReceiptNo and r.ReceiptDate=t.datetrans
--				where isnull(right(t.OraclereceiptNo,10),0)  not between '1000000000' and '2147483647' -- highest possible for integer 
		
--		open Recpt
--		Fetch Recpt into @acctno,@OldReceiptNo,@ReceiptDate,@OracleReceiptNo
			
--		WHILE	@@fetch_status = 0
--		BEGIN

--		if @OracleReceiptNo is null		-- new receiptno not previously allocated 
--		Begin
--			set @NewReceiptNo=@NewReceiptNo+1
--			set @OracleReceiptNo=left(@OldReceiptNo,3) + cast(@NewReceiptNo as varchar(19))		
--			if @@ERROR !=0
--			    print 'here1'
--			insert into OracleReceiptTranRef (acctno,OracleReceiptNo,CosacsReceiptNo,DateTrans)
--			select	@acctno,@OracleReceiptNo,@OldReceiptNo,@ReceiptDate
--			IF @@ERROR !=0 
--		BEGIN
--			PRINT 'error2 Oracle' + CONVERT(VARCHAR,DATALENGTH ( @oraclereceiptno) )
--			+ ' old ' + CONVERT(VARCHAR,DATALENGTH( @oldreceiptno))
--		END
--		End
		
--		Fetch next from Recpt into @acctno,@OldReceiptNo,@ReceiptDate,@OracleReceiptNo
		
--		End
		
--		close Recpt
--		Deallocate Recpt	
--	end 	
--		-- update hi Receipt no
--	update Country set HiExtReceiptNo=@NewReceiptNo
	 
--	-- Update OracleReceipts with new unique receipt no
--	Update OracleReceipts
--			set OracleReceiptNo= t.OracleReceiptNo
--	From OracleReceiptTranRef t INNER JOIN OracleReceipts r 
--		on t.acctno=r.acctno and r.receiptno=t.CosacsReceiptNo and r.ReceiptDate=t.datetrans

--	END
	

--IF (@@error = 0)
--	BEGIN
---- Get max invoice ref (ExtInvoice or buffbranch + buffno)
----drop table #temp
--select distinct d.acctno,MAX(case 
--					when ExtInvoice is null then CAST(buffbranchno as CHAR(3)) + CAST(buffno as varchar(7)) 
--					else ExtInvoice
--				end) as InvoiceReference
--into #temp
--from OracleReceipts r LEFT outer JOIN delivery d on r.acctno=d.acctno

--Group by d.acctno
---- Update with Invoice reference

--	END

--IF (@@error = 0)
--	BEGIN
--	Update OracleReceipts
--			set InvoiceReference=ISNULL(t.InvoiceReference,0)
--	From #temp t INNER JOIN OracleReceipts r on t.acctno=r.acctno

--	END

	
---- Duplicate Receipt nos  (70425) (caused by error in fix script. should not occur normally)
--IF (@@error = 0)
--	BEGIN
	
--	select receiptno,COUNT(*) as nbr into #dups from OracleReceipts
--		Group by receiptno
--		Having count(*)>1	
		
--	Update OracleReceipts
--			set ReceiptNo=r.ReceiptNo+'A'
--	From OracleReceipts r INNER JOIN #dups d on r.receiptNo=d.receiptno
--	Where TranType='XFR' and ReceiptAmount<0

--	END


---- Customers 
--IF (@@error = 0)
--	BEGIN

----IF EXISTS (SELECT table_name FROM INFORMATION_SCHEMA.TABLES
----	   WHERE  table_schema = 'dbo' and table_name = 'CustDelAdr')
----	drop table CustDelAdr

--	-- Get delivery address and value for stock items
--	select ar.acctno,REPLACE(deliveryaddress,' ','H') as deliveryaddress,SUM(ordval) as ordval,' ' as thisadr
--	into #CustDelAdr
--	from lineitem l INNER JOIN OracleAccountReceivables ar on l.acctno = ar.acctno and l.itemno = ar.itemno
--	INNER JOIN stockitem s on l.itemno = s.itemno and l.stocklocn = s.stocklocn
--	--where s.itemtype='S'
--	Group by ar.acctno,deliveryaddress
--	order by ar.acctno,deliveryaddress

--	END

--IF (@@error = 0)
--	BEGIN
--	-- select delivery address with highest value as Deliverd address
--	update #CustDelAdr
--		set thisadr='Y'
--	from #CustDelAdr da
--	where ordval=(select MAX(ordval) from #CustDelAdr da1 where da1.acctno=da.acctno Group by da1.acctno)

--	END

--IF (@@error = 0)
--	BEGIN
---- Select details
--IF EXISTS (SELECT table_name FROM INFORMATION_SCHEMA.TABLES
--	   WHERE  table_schema = 'dbo' and table_name = 'OracleCustomers')
--	truncate table OracleCustomers

--Insert into OracleCustomers
--Select distinct CAST(REPLACE(c.firstname +' '+ c.name,',',' ') as varchar(90)) as CustomerName,RTRIM(LTRIM(UPPER(ca.custid))) as custid,a.acctno,'Person' as CustType1,
--		CAST(REPLACE(Title,',',' ') as varchar(25))as Title,CAST(REPLACE(c.Firstname,',',' ') as varchar(30)) as Firstname,CAST(REPLACE(c.name,',',' ') as varchar(60))as name,'N/A' as CustType2,
--		'N/A' as CustClass, CAST(REPLACE(REPLACE(CAST(ISNULL(cc.code,'N/A') as varchar(10)),'N/A','Individual'),'STAF','Employee') as varchar(10))as CustCat,
--		CAST(REPLACE(h.Telno,',',' ') as varchar(30))as HomeTelno,CAST(REPLACE(ba.Email,' ',' ') as varchar(60)) as Email,RTRIM(LTRIM(UPPER(ca.custid))) as Passport,0 as empeeno,
--		CAST(REPLACE(ba.cusaddr1,',',' ') as varchar(50))  as BillAddr1,CAST(REPLACE(ba.cusaddr2,',',' ') as varchar(50)) as BillAddr2,CAST(REPLACE(ba.cusaddr3,',',' ') as varchar(50)) as BillAddr3,
--		CAST(' ' as varchar(50)) as BillCity, CAST(REPLACE(ba.cuspocode,',',' ') as varchar(10)) as BillPostCode,cm.value as BillCountry,
--		ISNULL(ba.AddressID,0) as BillAdrRef,
--		CAST(REPLACE(sa.cusaddr1,',',' ') as varchar(50)) as ShipAddr1,CAST(REPLACE(sa.cusaddr2,',',' ') as varchar(50)) as ShipAddr2,CAST(REPLACE(sa.cusaddr3,',',' ') as varchar(50)) as ShipAddr3,
--		CAST(' ' as varchar(50)) as ShipCity, CAST(REPLACE(sa.cuspocode,',',' ') as varchar(10))as ShipPostCode,cm.value as ShipCountry,
--		ISNULL(sa.AddressID,0) as ShipAdrRef,
--		CAST(REPLACE(m.Telno,',',' ') as varchar(30))as mobileTelno,CAST(REPLACE(w.Telno,',',' ') as varchar(30))as WorkTelno,ar.runno


--	From custacct ca LEFT outer JOIN Customer c on c.custid = ca.custid
--		INNER JOIN acct a on ca.acctno = a.acctno
--		INNER JOIN OracleAccountReceivables ar on ar.CustomerID=ca.custid		-- Deliveries
--		INNER JOIN #CustDelAdr da on a.acctno=da.acctno and thisadr='Y'
--		left outer JOIN custaddress ba on c.custid=ba.custid and ba.addtype='H'  -- Home address -- change to loj as might not be Home address
--					and (ba.datemoved is null or c.custid='Paid & Taken')	-- P&T may not have datemoved set correctly										
--		left outer join custaddress sa on c.custid=sa.custid and sa.addtype=da.deliveryaddress and sa.datemoved is null-- Delivery address
--		LEFT OUTER JOIN custtel h on ca.custid=h.custid	and h.tellocn='H' and h.datediscon is null	-- Home tel
--		LEFT OUTER JOIN custtel m on ca.custid=m.custid	and m.tellocn='M' and m.datediscon is null	-- Mobile tel
--		LEFT OUTER JOIN custtel w on ca.custid=w.custid and w.tellocn='W' and w.datediscon is null		-- Work tel
--		LEFT OUTER JOIN custcatcode cc on c.custid=cc.custid and cc.code='STAF',
--		CountryMaintenance cm

--	where cm.CodeName='countryname' and hldorjnt='H'
	
	
---- add new customers that have only a financial transaction
---- add addresses for accounts not selected above
--insert into #CustDelAdr 
--select distinct acctno,'H',0,'Y'
--from OracleReceipts a where not exists(select * from #CustDelAdr c where a.acctno=c.acctno)

--Insert into OracleCustomers
--Select distinct CAST(REPLACE(c.firstname +' '+ c.name,',',' ') as varchar(90)) as CustomerName,RTRIM(LTRIM(UPPER(ca.custid))) as custid,a.acctno,'Person' as CustType1,
--		CAST(REPLACE(Title,',',' ') as varchar(25))as Title,CAST(REPLACE(c.Firstname,',',' ') as varchar(30)) as Firstname,CAST(REPLACE(c.name,',',' ') as varchar(60))as name,'N/A' as CustType2,
--		'N/A' as CustClass, CAST(REPLACE(REPLACE(CAST(ISNULL(cc.code,'N/A') as varchar(10)),'N/A','Individual'),'STAF','Employee') as varchar(10))as CustCat,
--		CAST(REPLACE(h.Telno,',',' ') as varchar(30))as HomeTelno,CAST(REPLACE(ba.Email,' ',' ') as varchar(60)) as Email,RTRIM(LTRIM(UPPER(ca.custid))) as Passport,0 as empeeno,
--		CAST(REPLACE(ba.cusaddr1,',',' ') as varchar(50))  as BillAddr1,CAST(REPLACE(ba.cusaddr2,',',' ') as varchar(50)) as BillAddr2,CAST(REPLACE(ba.cusaddr3,',',' ') as varchar(50)) as BillAddr3,
--		CAST(' ' as varchar(50)) as BillCity, CAST(REPLACE(ba.cuspocode,',',' ') as varchar(10)) as BillPostCode,cm.value as BillCountry,
--		ISNULL(ba.AddressID,0) as BillAdrRef,
--		CAST(REPLACE(sa.cusaddr1,',',' ') as varchar(50)) as ShipAddr1,CAST(REPLACE(sa.cusaddr2,',',' ') as varchar(50)) as ShipAddr2,CAST(REPLACE(sa.cusaddr3,',',' ') as varchar(50)) as ShipAddr3,
--		CAST(' ' as varchar(50)) as ShipCity, CAST(REPLACE(sa.cuspocode,',',' ') as varchar(10))as ShipPostCode,cm.value as ShipCountry,
--		ISNULL(sa.AddressID,0) as ShipAdrRef,
--		CAST(REPLACE(m.Telno,',',' ') as varchar(30))as mobileTelno,CAST(REPLACE(w.Telno,',',' ') as varchar(30))as WorkTelno,r.runno


--	From custacct ca LEFT outer JOIN Customer c on c.custid = ca.custid
--		INNER JOIN acct a on ca.acctno = a.acctno
--		INNER JOIN OracleReceipts r on r.custid=ca.custid		-- Receipts
--		INNER JOIN #CustDelAdr da on a.acctno=da.acctno and thisadr='Y'
--		left outer JOIN custaddress ba on c.custid=ba.custid and ba.addtype='H'  -- Home address -- change to loj as might not be Home address
--					and (ba.datemoved is null or c.custid='Paid & Taken')	-- P&T may not have datemoved set correctly										
--		left outer join custaddress sa on c.custid=sa.custid and sa.addtype=da.deliveryaddress and sa.datemoved is null-- Delivery address
--		LEFT OUTER JOIN custtel h on ca.custid=h.custid	and h.tellocn='H' and h.datediscon is null	-- Home tel
--		LEFT OUTER JOIN custtel m on ca.custid=m.custid	and m.tellocn='M' and m.datediscon is null	-- Mobile tel
--		LEFT OUTER JOIN custtel w on ca.custid=w.custid and w.tellocn='W' and w.datediscon is null		-- Work tel
--		LEFT OUTER JOIN custcatcode cc on c.custid=cc.custid and cc.code='STAF',
--		CountryMaintenance cm

--	where cm.CodeName='countryname' and hldorjnt='H'
--		and not exists(select * from OracleCustomers oc where oc.custid=r.custid and oc.acctno=r.acctno)
	
--	END


--IF (@@error = 0)
--	BEGIN	

---- Set City (from 2nd or 3rd line of address)
--Update OracleCustomers
--		set Billcity=case
--			when BillAddr3!='' then BillAddr3
--			when BillAddr2!='' then BillAddr2
--			End,
--			Shipcity=case
--			when ShipAddr3!='' then ShipAddr3
--			when ShipAddr2!='' then ShipAddr2
--			End
---- Clear Address line containing City 
--Update OracleCustomers
--		set BillAddr3=case
--			when BillAddr3=Billcity then '.'
--			when BillAddr3='' then '.'
--			else BillAddr3				
--			End,
--			BillAddr2=case
--			when BillAddr2=Billcity then '.'
--			else BillAddr2			
--			End,
--			ShipAddr3=case
--			when ShipAddr3=Shipcity then '.'	
--			else ShipAddr3			
--			End,
--			ShipAddr2=case
--			when ShipAddr2=Shipcity then '.'	
--			else ShipAddr2		
--			End,
--			Email=case
--			when Email='' then '.'
--			else Email
--			End,			
--			WorkTelno=case
--			when ISNULL(WorkTelno,'')='' then '.'
--			else WorkTelno
--			End,
--			mobileTelno=case
--			when ISNULL(mobileTelno,'')='' then '.'
--			else mobileTelno
--			End

--	END

--IF (@@error = 0)
--	BEGIN

---- Set empeeno who created Customer (from earliest agreement)
----Update OracleCustomers
----	set empeeno=empeenosale 
------from OracleCustomers i INNER JOIN custacct ca on i.custid=ca.custid and hldorjnt='H'
----	 INNER JOIN agreement ag on ca.acctno = ag.acctno
----where ag.dateagrmt=(select MIN(ag1.dateagrmt) from agreement ag1 
----					INNER JOIN custacct ca1 on ag1.acctno=ca1.acctno
----					where ca.custid=ca1.custid
----					Group by ca1.custid)
--Update OracleCustomers
--	set empeeno=empeenosale 
--from OracleCustomers i INNER JOIN agreement ag on i.acctno = ag.acctno

--	END
	

--End

--IF (@@error != 0)
--	BEGIN
--		SET @return = @@error
--	END
GO 

