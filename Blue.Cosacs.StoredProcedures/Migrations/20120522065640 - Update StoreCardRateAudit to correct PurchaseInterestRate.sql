-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
-- Previously the rates on this table were being rounded when saved to this table due to the column being decimal (18,0)
-- this has been corrected, but we still need to correct the rates on the audit table for the latest entries inserted.

update StoreCardRateAudit
set PurchaseInterestRate = scrd.PurchaseInterestRate
from StoreCardRateDetails scrd
where StoreCardRateAudit.id = scrd.ParentID
and StoreCardRateAudit.AppScoreFrom = scrd.AppScoreFrom
and StoreCardRateAudit.AppScoreTo = scrd.AppScoreTo
and [$Action] = 'I'
and [$CreatedOn] = (select max([$CreatedOn])
						from StoreCardRateAudit scra
						where scra.id = StoreCardRateAudit.id
						and scra.AppScoreFrom = StoreCardRateAudit.AppScoreFrom
						and scra.AppScoreTo = StoreCardRateAudit.AppScoreTo
						and scra.[$Action] = StoreCardRateAudit.[$Action])