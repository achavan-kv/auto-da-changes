-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

  UPDATE Merchandising.ClassMapping 
  SET LegacyCode = 5
  WHERE ClassCode = '62A'