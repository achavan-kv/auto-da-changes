-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here


declare @SCPCent decimal

select @SCPCent = value from CountryMaintenance where codename = 'StorecardPercent'

Update customer
set StoreCardLimit = null,
	StoreCardAvailable = null
where not exists(select * from custacct ca where customer.custid=ca.custid and substring(ca.acctno,4,1)='9' and ca.hldorjnt = 'H') or
StoreCardApproved = 0


Update Customer
Set StoreCardLimit = RFCreditLimit * @SCPCent/100,
	StoreCardAvailable = RFCreditLimit * @SCPCent/100
where exists (select * from custacct ca where customer.custid=ca.custid and substring(ca.acctno,4,1)='9' and ca.hldorjnt = 'H')
and (StoreCardLimit is null or StoreCardLimit = 0)


