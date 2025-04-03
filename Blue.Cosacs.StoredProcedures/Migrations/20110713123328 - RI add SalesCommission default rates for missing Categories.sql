-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

insert into dbo.SalesCommissionRates (ItemText, CommissionType, Percentage, PercentageCash, Value, DateFrom, DateTo, EmpeenoChanged, ComBranchNo, 
				ItemId, RepoPercentage, RepoPercentageCash, RepoValue, RepoItemId
	
) 
select distinct category,'PC',0,0,0,convert(datetime,convert(varchar,getdate(),105),105) ,'2050-01-01',0,'All',0,0,0,0,0
from stockinfo s
where not exists(select * from dbo.SalesCommissionRates r where ItemText=CAST(s.category as VARCHAR) and CommissionType='PC')
and s.category!=0

