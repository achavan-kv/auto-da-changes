-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

CREATE TABLE [Admin].[EmployeeUnavailability]
(
    Id INT IDENTITY(1, 1) NOT NULL CONSTRAINT PK_Admin_EmployeeUnavailability PRIMARY KEY CLUSTERED,
    UserId INT NOT NULL CONSTRAINT FK_Admin_EmployeeUnavailability_Admin_User FOREIGN KEY REFERENCES [Admin].[User](Id),
    DateFrom DATE NOT NULL,
    DateTo DATE NOT NULL,
    CreatedBy INT NOT NULL CONSTRAINT FK_Admin_EmployeeUnavailability_2_Admin_User FOREIGN KEY REFERENCES [Admin].[User](Id),
    CreatedOn SMALLDATETIME NOT NULL,
    Approved BIT,
    Cancelled BIT NOT NULL
)