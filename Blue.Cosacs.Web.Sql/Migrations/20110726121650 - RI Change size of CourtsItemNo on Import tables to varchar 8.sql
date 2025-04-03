-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

Alter TABLE RItemp_RawProductload alter column ItemNo VARCHAR(8)

Alter TABLE RItemp_RawProductloadRepo alter column ItemNo VARCHAR(8)

