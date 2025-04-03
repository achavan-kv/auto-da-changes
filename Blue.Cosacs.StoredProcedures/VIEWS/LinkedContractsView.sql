/*
Contracts to print for Ready Assist items

*/

IF EXISTS (SELECT * FROM sys.objects so
		   INNER JOIN sys.schemas ss ON so.schema_id = ss.schema_id
		   WHERE so.type = 'V'
		   AND so.NAME = 'LinkedContractsView'
		   AND ss.name = 'dbo')
DROP VIEW  dbo.[LinkedContractsView]
GO


CREATE VIEW  dbo.[LinkedContractsView]
AS
	

	select l.acctno, l.ID, si.IUPC as itemno, l.ordval, co.reference as ContractLength, lc.Contract, c.title + ' ' + c.firstname + ' ' + c.name as CustomerName, 
		cad.cusaddr1 as AddressLine1, cad.cusaddr2 as AddressLine2, cad.cusaddr3 as AddressLine3, cad.cuspocode as PostCode,
		case when datepart(day, getdate()) in (1, 21, 31) then cast(datepart(day, getdate()) as varchar(3)) + 'st'
		 when datepart(day, getdate()) in (2, 22) then cast(datepart(day, getdate()) as varchar(3)) +'nd' 
		 when datepart(day, getdate()) in (3, 23) then cast(datepart(day, getdate()) as varchar(3)) +'rd'
		 else cast(datepart(day, getdate()) as varchar(3)) +'th' end as AgreementDay,
		 DATENAME(month, getdate()) as AgreementMonth,
		 datepart(year, getdate()) as AgreementYear,
		 --case when datepart(day, ra.RAContractDate) in (1, 21, 31) then cast(datepart(day, ra.RAContractDate) as varchar(3)) + 'st'
		 --when datepart(day, ra.RAContractDate) in (2, 22) then cast(datepart(day, ra.RAContractDate) as varchar(3)) +'nd' 
		 --when datepart(day, ra.RAContractDate) in (3, 23) then cast(datepart(day, ra.RAContractDate) as varchar(3)) +'rd'
		 --else cast(datepart(day, ra.RAContractDate) as varchar(3)) +'th' end as ContractDay,
		 --DATENAME(month, getdate()) as ContractMonth,
		 --datepart(year, getdate()) as ContractYear,
		 case when ((l.ordval)%1) > 0 then dbo.IntegerToWordsFN(floor(l.ordval)) + ' Dollars and ' + RIGHT(cast(((l.ordval)%1)  as varchar(6)),2) + ' Cents '  --#14305
		 else dbo.IntegerToWordsFN(floor(l.ordval)) + ' Dollars' end as moneyString,
		 --dbo.IntegerToWordsFN(floor(l.ordval)) + ' Dollars ' + case when ((l.ordval)%1) > 0 
			--then  ' and ' + RIGHT(cast(((l.ordval)%1)  as varchar(6)),2) + ' Cents ' end as moneyString, 
			'$'+ cast(l.ordval as varchar(9)) as moneyValue 															
	from lineitem l inner join custacct ca on l.acctno = ca.acctno and ca.hldorjnt = 'H'
	--inner join ReadyAssistDetails ra on l.acctno = ra.acctno and l.agrmtno = ra.agrmtno				--#18630
	inner join customer c on ca.custid = c.custid
	inner join custaddress cad on c.custid = cad.custid and cad.addtype = 'H' and cad.datemoved is null
	inner join stockinfo si on si.ID = l.ItemID
	inner join code co on si.IUPC = co.code
	inner join sales.LinkedContracts lc on si.IUPC = lc.ItemNo
	where co.category = 'RDYAST'
	union
		select l.acctno, l.ID, si.IUPC as itemno, l.ordval, 0 as ContractLength, lc.Contract, c.title + ' ' + c.firstname + ' ' + c.name as CustomerName, 
		cad.cusaddr1 as AddressLine1, cad.cusaddr2 as AddressLine2, cad.cusaddr3 as AddressLine3, cad.cuspocode as PostCode,
		 case when datepart(day, getdate()) in (1, 21, 31) then cast(datepart(day, getdate()) as varchar(3)) +'st'
		 when datepart(day, getdate()) in (2, 22) then cast(datepart(day, getdate()) as varchar(3)) +'nd' 
		 when datepart(day, getdate()) in (3, 23) then cast(datepart(day, getdate()) as varchar(3)) +'rd'
		 else cast(datepart(day, getdate()) as varchar(3)) +'th' end as AgreementDay,
		  DATENAME(month, getdate()) as AgreementMonth,
		   datepart(year, getdate()) as AgreementYear,
		--case when datepart(day, ra.RAContractDate) in (1, 21, 31) then cast(datepart(day, ra.RAContractDate) as varchar(3)) + 'st'
		-- when datepart(day, ra.RAContractDate) in (2, 22) then cast(datepart(day, ra.RAContractDate) as varchar(3)) +'nd' 
		-- when datepart(day, ra.RAContractDate) in (3, 23) then cast(datepart(day, ra.RAContractDate) as varchar(3)) +'rd'
		-- else cast(datepart(day, ra.RAContractDate) as varchar(3)) +'th' end as ContractDay,
		-- DATENAME(month, getdate()) as ContractMonth,
		-- datepart(year, getdate()) as ContractYear,
		 case when ((l.ordval)%1) > 0 then dbo.IntegerToWordsFN(floor(l.ordval)) + ' Dollars and ' + RIGHT(cast(((l.ordval)%1)  as varchar(6)),2) + ' Cents ' --#14305
		 else dbo.IntegerToWordsFN(floor(l.ordval)) + ' Dollars' end as moneyString,
		 -- dbo.IntegerToWordsFN(floor(l.ordval)) + ' Dollars ' + case when ((l.ordval)%1) > 0 
		 --then  ' and ' + RIGHT(cast(((l.ordval)%1)  as varchar(6)),2) + ' Cents '  end as moneyString, 
			'$'+ cast(l.ordval as varchar(9)) as moneyValue
	from lineitem l inner join custacct ca on l.acctno = ca.acctno and ca.hldorjnt = 'H'
	--inner join ReadyAssistDetails ra on l.acctno = ra.acctno and l.agrmtno = ra.agrmtno
	inner join customer c on ca.custid = c.custid
	inner join custaddress cad on c.custid = cad.custid and cad.addtype = 'H' and cad.datemoved is null
	inner join StockInfo si on l.ItemID = si.ID
	inner join sales.LinkedContracts lc on si.category = lc.Category
	where not exists(select * from code c
						where c.category = 'RDYAST'
						and c.code = si.IUPC)


GO

