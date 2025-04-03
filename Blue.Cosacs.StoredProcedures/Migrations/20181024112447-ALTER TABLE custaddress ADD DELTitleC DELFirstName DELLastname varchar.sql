-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE  Table_Name = 'custaddress' AND  Column_Name = 'DELTitleC' AND  Column_Name = 'DELFirstName' AND 
 Column_Name = 'DELLastname')
BEGIN
	ALTER TABLE custaddress ADD DELTitleC varchar(50),DELFirstName varchar(50),DELLastname varchar(50)
END

