-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

CREATE TABLE [Admin].[Notification]
(
	Id INT IDENTITY PRIMARY KEY,
	FromUserId INT NOT NULL FOREIGN KEY REFERENCES [Admin].[User](Id),
	ToUserId INT NOT NULL FOREIGN KEY REFERENCES [Admin].[User](Id),
	NotificationType INT NOT NULL,
	Subject VARCHAR(100) NOT NULL,
	Body VARCHAR(200) NOT NULL,
	ComplexMessage VARCHAR(300) SPARSE NULL,
	DateReceived DATETIME NOT NULL 
)