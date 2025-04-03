CREATE TABLE Service.Holiday
(
	Id INT IDENTITY(1,1),
	UserId INT NOT NULL,
	[Date] SMALLDATETIME NOT NULL,
	Approved BIT NOT NULL
)
GO

ALTER TABLE Service.Holiday
ADD CONSTRAINT PK_Holiday PRIMARY KEY (Id)
GO

ALTER TABLE Service.Holiday
ADD CONSTRAINT FK_Holiday_Technician FOREIGN KEY (UserId) REFERENCES Service.Technician (UserId)
GO

CREATE NONCLUSTERED INDEX IX_UserId
    ON Service.Holiday (UserId)
    INCLUDE ([Date])
GO
