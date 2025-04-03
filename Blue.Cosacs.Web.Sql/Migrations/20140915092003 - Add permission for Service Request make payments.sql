-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here


IF NOT EXISTS(select * from Admin.[Permission]
				where Id = 1641 
				and CategoryId = 16)

BEGIN
	INSERT INTO Admin.Permission
        ( Id, Name, CategoryId, Description )
VALUES  ( 1641,
          'Service Request - Make Payments',
          16,
          'Allow user to make payments on Service Requests.'
          )
END
