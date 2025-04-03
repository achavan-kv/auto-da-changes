-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
-- Related to issue: #14479

delete from countrymaintenance where codename in ('OnlineSBandIntRate', 'OnlineTermsLength', 'OnlineTermsType')