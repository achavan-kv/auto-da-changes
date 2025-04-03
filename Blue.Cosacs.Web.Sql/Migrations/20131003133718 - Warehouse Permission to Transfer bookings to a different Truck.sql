-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
-- Related to issue: #15228 - CR15170

if not exists(select * from admin.Permission where id = 1431)
begin

	
	insert into Admin.Permission
			( Id, Name, CategoryId, Description )
	values  ( 1431, -- Id - int
			  'Transfer picked items to a different Truck', -- Name - varchar(100)
			  14, -- CategoryId - int
			  'Allows users to transfer picked items to a different Truck'  -- Description - varchar(300)
			  )
          

end
