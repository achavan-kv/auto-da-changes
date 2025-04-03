-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here


IF NOT EXISTS(select * from code where category = 'EDC' and code = 'INSTCREDAPP')
BEGIN
	
	insert into code
	select null, 'EDC', 'INSTCREDAPP', 'Instant Credit Qualification', 'L', 0, 0, null, null

END


