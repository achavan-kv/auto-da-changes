CREATE TABLE Service.Payment
	(
	Id int NOT NULL IDENTITY (1, 1),
	PayMethod int NOT NULL,
	CustomerId varchar(20) NOT NULL,
	Amount money NOT NULL,
	EmpeeNo int NOT NULL,
	RequestId int NOT NULL,
	Bank varchar(6) NULL,
	ChequeNumber varchar(16) NULL,
	BankAccountNumber varchar(20) NULL,
	ChargeType varchar(30) NULL,
	CardType varchar(20) NULL,
	CardNumber int NULL
	)  ON [PRIMARY]
GO
ALTER TABLE Service.Payment ADD CONSTRAINT
	PK_ServicePaymentId PRIMARY KEY CLUSTERED 
	(
	Id
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO


CREATE NONCLUSTERED INDEX IX_ServicePayment_RequestNumber
    ON Service.Payment (RequestId); 
GO