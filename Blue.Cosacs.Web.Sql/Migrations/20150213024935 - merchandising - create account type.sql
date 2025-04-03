CREATE TABLE [Merchandising].TransactionType (
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](100) NOT NULL,
	[TransactionCode] [varchar](10) NULL,
	[DebitAccount] [varchar](30) NULL,
	[CreditAccount] [varchar](30) NULL,
	[SplitDebitByDepartment] [bit] NULL,
	[SplitCreditByDepartment] [bit] NULL,
 CONSTRAINT [PK_Merchandising_AccountType] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

insert into Merchandising.TransactionType
	([Name])
values 
	('Goods Received from Supplier (Foreign)'),
	('Goods Received from Supplier (Parts)'),
	('Goods Received from Supplier (Local)'),
	('Vendor Return (Foreign)'),
	('Vendor Return (Parts)'),
	('Vendor Return (Local)'),
	('Stock Transfer'),
	('Stock Adjustment'),
	('FYW Provisioning'),
	('Cost of Sale')