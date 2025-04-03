-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

--Authorisation is now required when selling out of stock associated items in the New Sales Order screen.
--Therefore need to add control entry required to determine if user has the user right to sell out of stock items.

declare @taskID int

if exists(select * from task where taskname = 'New Sales Order - Authorise Selling Out Of Stock Products')
BEGIN

	select @taskID = taskid from task where taskname = 'New Sales Order - Authorise Selling Out Of Stock Products'
	
	if not exists(select * from [control] where taskid = @taskID 
					and screen = 'RelatedProducts')
	begin
			
			insert into [control] (TaskID, Screen, [Control], Visible, [Enabled], ParentMenu)
			select @taskID, 'RelatedProducts', 'lAuthorise', 0, 1, ''
	end
	

END