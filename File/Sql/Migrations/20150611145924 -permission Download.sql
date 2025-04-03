-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here


if not exists (select * from Admin.PermissionCategory 
where Id = 29)
BEGIN
	INSERT INTO Admin.PermissionCategory
	VALUES(29, 'File')
END

INSERT INTO Admin.Permission
        ( Id, Name, CategoryId, Description )
VALUES  ( 
				2903,
				'Download File',
				29,
				'Allow users to download files'
          )

Go


-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
