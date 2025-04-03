IF NOT EXISTS (SELECT 1 FROM admin.permission WHERE Id = 800)
BEGIN
	Insert into admin.permission  values (800,'Reprint Invoice',8,'To Reprint Invoice Details',0)
END

IF NOT EXISTS (SELECT 1 FROM [dbo].[Control] WHERE TaskId = 69 and control='RePrintInvoice')
BEGIN
	Insert into [dbo].[Control] values (69,'MainForm','RePrintInvoice',1,1,'menuTransaction')
END

