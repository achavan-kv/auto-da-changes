-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

ALTER TABLE storecardRateAudit DROP CONSTRAINT PK_StoreCardRateAudit
ALTER TABLE storecardRateAudit DROP COLUMN [$AuditId]
ALTER TABLE storecardRateAudit ADD [$AuditId] INT identity
ALTER TABLE storecardRateAudit ADD CONSTRAINT  PK_StoreCardRateAudit PRIMARY KEY ([$AuditId])
ALTER TABLE storecardRateAudit DROP COLUMN Scorefrom
ALTER TABLE storecardRateAudit DROP COLUMN ScoreTo

ALTER TABLE storecardRateAudit ADD AppScorefrom smallint 
ALTER TABLE storecardRateAudit ADD AppScoreTo SMALLINT 
ALTER TABLE storecardRateAudit ADD BehaveScoreFrom smallint 
ALTER TABLE storecardRateAudit ADD BehaveScoreTo SMALLINT 
 