IF EXISTS (SELECT * FROM syscolumns
           WHERE name = 'ResolutionSupplierToChargeAccount'
           AND OBJECT_NAME(id)  = 'Request')
BEGIN
	ALTER TABLE Service.Request
	DROP COLUMN ResolutionSupplierToChargeAccount
END
GO

IF EXISTS (SELECT * FROM sys.objects so
           INNER JOIN sys.schemas ss ON so.schema_id = ss.schema_id
           WHERE so.type = 'U'
           AND so.NAME = 'Charge'
           AND ss.name = 'Service')
BEGIN
	drop TABLE Service.Charge
END
GO
GO

CREATE TABLE Service.Charge
(
	Id INT IDENTITY(1,1),
	RequestId INT NOT NULL,
	Type VARCHAR(50) NOT NULL,
	Account VARCHAR(12) NULL,
	Tax MONEY NULL,
	[Value] MONEY NOT NULL
 CONSTRAINT [PK_ServiceCharge] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
)

ALTER TABLE Service.Charge
ADD CONSTRAINT FK_ServiceCharge_ServiceRequest 
FOREIGN KEY	(requestId) REFERENCES Service.Request (Id)
ON DELETE CASCADE
GO
