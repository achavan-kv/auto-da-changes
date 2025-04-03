SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS OFF
GO

IF EXISTS (SELECT * FROM SysObjects
           WHERE id = object_id('[dbo].OMig_CustomerInterface') and OBJECTPROPERTY(id, 'IsProcedure') = 1)

BEGIN
    DROP PROCEDURE OMig_CustomerInterface
END
GO

CREATE PROCEDURE dbo.OMig_CustomerInterface 

-- ================================================
-- Project      : CoSACS .NET
-- File Name    : OMig_CustomerInterface.prc
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Migrate Customer Details
-- Author       : John Croft
-- Date         : 15 July 2008
--
-- This procedure will process create csv file for the Customer Interface into Oracle
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 11/08/08 jec Convert Customer ID to uppercase.
-- 12/08/08 jec Trim Custid and convert to uppercase
-- 12/08/08 jec change joihn on customer and use custacct custid in select
-- ================================================
	-- Add the parameters for the stored procedure here

	@date datetime
as

--set @date =DATEADD(d,1,@date) -- Add 1 day to ensure all transaction for date entered are included


IF EXISTS (SELECT table_name FROM INFORMATION_SCHEMA.TABLES
	   WHERE  table_schema = 'dbo' and table_name = 'CustDelAdr')
	drop table CustDelAdr

-- Get delivery address and value for stock items
select acctno,REPLACE(deliveryaddress,' ','H') as deliveryaddress,SUM(ordval) as ordval,' ' as thisadr
into CustDelAdr
from lineitem l INNER JOIN stockitem s on l.itemno = s.itemno and l.stocklocn = s.stocklocn
where s.itemtype='S'
Group by acctno,deliveryaddress
order by acctno,deliveryaddress

-- add addresses for accounts not selected above
insert into CustDelAdr 
select acctno,'H',0,'Y'
from acct a where not exists(select * from CustDelAdr c where a.acctno=c.acctno)
and (a.outstbal !=0
	or exists(select * from fintrans f where a.acctno=f.acctno and datetrans>=@date) )
-- select delivery address with highest value as Deliverd address
update CustDelAdr
	set thisadr='Y'
from CustDelAdr da
where ordval=(select MAX(ordval) from CustDelAdr da1 where da1.acctno=da.acctno Group by da1.acctno)

-- Select details
IF EXISTS (SELECT table_name FROM INFORMATION_SCHEMA.TABLES
	   WHERE  table_schema = 'dbo' and table_name = 'CustomerInterface')
	drop table CustomerInterface

Select CAST(REPLACE(c.firstname +' '+ c.name,',',' ') as varchar(90)) as CustomerName,RTRIM(LTRIM(UPPER(ca.custid))) as custid,a.acctno,'Person' as CustType1,
		CAST(REPLACE(Title,',',' ') as varchar(25))as Title,CAST(REPLACE(c.Firstname,',',' ') as varchar(30)) as Firstname,CAST(REPLACE(c.name,',',' ') as varchar(60))as name,'N/A' as CustType2,
		'N/A' as CustClass, CAST(REPLACE(REPLACE(CAST(ISNULL(cc.code,'N/A') as varchar(10)),'N/A','Individual'),'STAF','Employee') as varchar(10))as CustCat,CAST(REPLACE(h.Telno,',',' ') as varchar(30))as HomeTelno,CAST(REPLACE(ba.Email,' ','') as varchar(60)) as Email,ca.custid as Passport,0 as empeeno,
		CAST(REPLACE(ba.cusaddr1,',',' ') as varchar(50))  as BillAddr1,CAST(REPLACE(ba.cusaddr2,',',' ') as varchar(50)) as BillAddr2,CAST(REPLACE(ba.cusaddr3,',',' ') as varchar(50)) as BillAddr3,
		CAST(' ' as varchar(50)) as BillCity, CAST(REPLACE(ba.cuspocode,',',' ') as varchar(10)) as BillPostCode,cm.value as BillCountry,
		ba.AddressID as BillAdrRef,
		CAST(REPLACE(sa.cusaddr1,',',' ') as varchar(50)) as ShipAddr1,CAST(REPLACE(sa.cusaddr2,',',' ') as varchar(50)) as ShipAddr2,CAST(REPLACE(sa.cusaddr3,',',' ') as varchar(50)) as ShipAddr3,
		CAST(' ' as varchar(50)) as ShipCity, CAST(REPLACE(sa.cuspocode,',',' ') as varchar(10))as ShipPostCode,cm.value as ShipCountry,
		sa.AddressID as ShipAdrRef,
		CAST(REPLACE(m.Telno,',',' ') as varchar(30))as mobileTelno,CAST(REPLACE(w.Telno,',',' ') as varchar(30))as WorkTelno

into CustomerInterface

	From custacct ca LEFT outer JOIN Customer c on c.custid = ca.custid
		INNER JOIN acct a on ca.acctno = a.acctno
		INNER JOIN CustDelAdr da on a.acctno=da.acctno and thisadr='Y'
		left outer JOIN custaddress ba on c.custid=ba.custid and ba.addtype='H' -- Home address
				and (ba.datemoved is null or c.custid='Paid & Taken')	-- P&T may not have datemoved set correctly										
		left outer join custaddress sa on c.custid=sa.custid and sa.addtype=da.deliveryaddress and sa.datemoved is null-- Delivery address
		LEFT OUTER JOIN custtel h on ca.custid=h.custid	and h.tellocn='H' and h.datediscon is null	-- Home tel
		LEFT OUTER JOIN custtel m on ca.custid=m.custid	and m.tellocn='M' and m.datediscon is null	-- Mobile tel
		LEFT OUTER JOIN custtel w on ca.custid=w.custid and w.tellocn='W' and w.datediscon is null		-- Work tel
		LEFT OUTER JOIN custcatcode cc on ca.custid=cc.custid and cc.code='STAF',
		CountryMaintenance cm

	where cm.CodeName='countryname'
		and (a.outstbal!=0 -- changed to match AccountBalance script --50	-- greater 50 Rs-- !=0
			or exists(select * from fintrans f where a.acctno=f.acctno and datetrans>=@date) )
		and hldorjnt='H'
	

-- Set City (from 2nd or 3rd line of address)
Update CustomerInterface
		set Billcity=case
			when BillAddr3!='' then BillAddr3
			when BillAddr2!='' then BillAddr2
			End,
			Shipcity=case
			when ShipAddr3!='' then ShipAddr3
			when ShipAddr2!='' then ShipAddr2
			End
-- Clear Address line containing City 
Update CustomerInterface
		set BillAddr3=case
			when BillAddr3=Billcity then ''
			else BillAddr3				
			End,
			BillAddr2=case
			when BillAddr2=Billcity then ''
			else BillAddr2			
			End,
			ShipAddr3=case
			when ShipAddr3=Shipcity then ''	
			else ShipAddr3			
			End,
			ShipAddr2=case
			when ShipAddr2=Shipcity then ''	
			else ShipAddr2		
			End

-- Set empeeno who created Customer (from earliest agreement)
--Update CustomerInterface
--	set empeeno=empeenosale 
--from CustomerInterface i INNER JOIN custacct ca on i.custid=ca.custid and hldorjnt='H'
--	 INNER JOIN agreement ag on ca.acctno = ag.acctno
--where ag.dateagrmt=(select MIN(ag1.dateagrmt) from agreement ag1 
--					INNER JOIN custacct ca1 on ag1.acctno=ca1.acctno
--					where ca.custid=ca1.custid
--					Group by ca1.custid)

-- Set empeeno who created Customer
Update CustomerInterface
	set empeeno=empeenosale 
from CustomerInterface i INNER JOIN agreement ag on i.acctno = ag.acctno

------ Check for Deleted/Inactive employees where active exists
--select distinct i.empeeno as origempeeno,c.empeeno as delempeeno,0 as actempeeno,c.fullname as empeename
--into #cp
--from CustomerInterface i 
--INNER JOIN Admin.[User] c on i.empeeno=c.Id
--where locked = 1

---- get active employee no with same name
--update #cp
--set actempeeno=empeeno
--from courtsperson c, #cp where c.empeename=#cp.empeename and empeetype!='Z'
-- Update deleted empeeno to active empeeno
update i
	set empeeno= 0
from CustomerInterface i 
INNER JOIN Admin.[User] c on i.empeeno=c.Id
Where locked = 1

-- export csv file

-- convert empty columns to null prior to BCP    
exec empty_to_null CustomerInterface  

declare @path varchar(200)

set @path = '"c:\program files\microsoft sql server\80\tools\binn\BCP" ' + db_name()+'..CustomerInterface' + ' out ' +
'd:\users\default\CustomerInterface.csv ' + '-c -t, -q -T'

exec master.dbo.xp_cmdshell @path



-- End End End End End End End End End End End End End End End End End End End End End End End End 


