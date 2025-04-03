IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE Table_Name = 'AshleyErrorLogs'
               AND TABLE_SCHEMA = 'Merchandising')
BEGIN
Create Table Merchandising.AshleyErrorLogs
(
	  Id int identity(1,2) primary key,
          FunctionName varchar (max),
          Error varchar (max),
         string  varchar (max),
         ErrorDate DateTime default(getdate())
)
END
GO
