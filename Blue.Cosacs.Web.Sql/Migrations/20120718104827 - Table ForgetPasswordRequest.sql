-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF OBJECT_ID('[Admin].ForgetPasswordRequest') IS NULL
BEGIN
print 'sadf'
	CREATE TABLE [Admin].ForgetPasswordRequest
	(
		id			Int IDENTITY (1, 1)	NOT NULL,
		UserLogin	VarChar(256)		NOT NULL,
		RequestDate SmallDatetime		NOT NULL,
		Token		VarChar(256)		NOT NULL
	)  ON [PRIMARY]
	
	
	ALTER TABLE Admin.ForgetPasswordRequest ADD CONSTRAINT	PK_ForgetPasswordRequest PRIMARY KEY CLUSTERED 
	(
		id
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) 

END

