-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here was displaying very narrow columns so these need to be set to null 
UPDATE dbo.codecat SET SortOrderHeaderText = NULL,referenceheadertext = NULL,additionalheadertext = NULL,
CodeHeaderText=NULL
WHERE CATEGORY IN('STPM','STPO','STQ')
