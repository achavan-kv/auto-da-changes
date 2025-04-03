-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
ALTER TABLE SalesManagement.Call
	ADD Source Tinyint NOT NULL
GO

CREATE NONCLUSTERED INDEX ix_SalesManagementCall_Source ON SalesManagement.Call
(
	Source
)
