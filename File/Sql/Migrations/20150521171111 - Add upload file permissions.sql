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
				2900,
				'Upload File',
				29,
				'Allow users to upload files'
          )

Go

INSERT INTO Admin.Permission
        ( Id, Name, CategoryId, Description )
VALUES  ( 
				2901,
				'Delete Upload File',
				29,
				'Allow the user to delete an uploaded file'
          )
