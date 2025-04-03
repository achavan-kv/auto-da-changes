-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
-- improve performance when logging in -- otherwise believe insert without index = table scan	
IF NOT EXISTS (SELECT * FROM sysindexes WHERE NAME ='iX_courtspersonaudit')
BEGIN
  CREATE CLUSTERED INDEX iX_courtspersonaudit ON Courtspersonaudit( newEmpeeno,DateChange)
END 
	
DELETE FROM Courtspersonaudit WHERE datechange < DATEADD(MONTH,-6,GETDATE()	)
 
GO