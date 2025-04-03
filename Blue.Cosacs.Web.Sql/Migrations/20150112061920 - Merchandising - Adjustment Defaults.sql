-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

insert into merchandising.StockAdjustmentPrimaryReason
(Name, DateDeleted)

Select 'Write off',	NULL union 
Select 'Count Adjustment',	NULL union 
Select 'Promotions',	NULL union 
Select 'Internal use',	NULL


insert into merchandising.StockAdjustmentSecondaryReason
( PrimaryReasonId,SecondaryReason,TransactionCode,DebitAccount
,CreditAccount,SplitDebitByDepartment,SplitCreditByDepartment
,DefaultForCountAdjustment,DateDeleted)

Select id, 'FYW Write Off','AFW','2930','1100',	0,	1,	0,	NULL 
from merchandising.StockAdjustmentPrimaryReason where name = 'Write off' union
Select id	,'EW Write Off','AEW',	'2910','1100',	0,	1,	0,	NULL
from merchandising.StockAdjustmentPrimaryReason where name = 'Write off' union
Select id	,'Insurance Claims','AIC','1403','1100',	0,	1,	0,	NULL
from merchandising.StockAdjustmentPrimaryReason where name = 'Write off' union
Select id	,'Merchandise losses NRV','AML','9700','1100',	1,	1,	0,	NULL
from merchandising.StockAdjustmentPrimaryReason where name = 'Write off' union
Select id	,'Missing inventory and Damaged Stock Losses','AMI','9800','1100',	1,	1,	0,	NULL
from merchandising.StockAdjustmentPrimaryReason where name = 'Write off' union
Select id	,'Supplier Warranty W/O','ASW','1392','1100',	0,	1,	0,	NULL
from merchandising.StockAdjustmentPrimaryReason where name = 'Write off' union


Select id	,'Inventory Adjustment Count','ACA','9800','1100',	1,	1,	1,	NULL
from merchandising.StockAdjustmentPrimaryReason where name = 'Count Adjustment' union

Select id	,'Advertising Exchange','AAE','9420','1100',	0,	1,	0,	NULL
from merchandising.StockAdjustmentPrimaryReason where name = 'Promotions' union
Select id	,'Charitable Donations','ACD','8310','1100',	0,	1,	0,	NULL
from merchandising.StockAdjustmentPrimaryReason where name = 'Promotions' union
Select id	,'Customer promotions Initiative','APR','9400','1100',	0	,1	,0	,NULL
from merchandising.StockAdjustmentPrimaryReason where name = 'Promotions' union
Select id	,'Employee gifts','AEG','7590','1100',	0,	1	,0	,NULL
from merchandising.StockAdjustmentPrimaryReason where name = 'Promotions' union

Select id	,'Fixed Assets','AFA','0201','1100',	0,	1	,0	,NULL
from merchandising.StockAdjustmentPrimaryReason where name = 'Internal Use' union
Select id	,'For store use','ASU','9260','1100',	0,	1	,0	,NULL
from merchandising.StockAdjustmentPrimaryReason where name = 'Internal Use' 

