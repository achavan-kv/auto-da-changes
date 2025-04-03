SET ANSI_WARNINGS OFF

alter table Merchandising.Supplier alter column
	[Code]			[varchar](30)	NULL
alter table Merchandising.Supplier alter column
	[Name]			[varchar](240)	NULL
alter table Merchandising.Supplier alter column
	[Type]			[varchar](20)	NULL
alter table Merchandising.Supplier alter column
	[Status]		[int]			NOT NULL
alter table Merchandising.Supplier alter column
	[AddressLine1]	[varchar](240)	NULL
alter table Merchandising.Supplier alter column
	[AddressLine2]	[varchar](240)	NULL
alter table Merchandising.Supplier alter column
	[City]			[varchar](25)	NULL
alter table Merchandising.Supplier alter column
	[Country]		[varchar](2)	NULL
alter table Merchandising.Supplier alter column
	[PostCode]		[varchar](20)	NULL
alter table Merchandising.Supplier alter column
	[PaymentTerms]	[varchar](50)	NULL
alter table Merchandising.Supplier alter column
	[Contacts]		[varchar](max)	NULL
alter table Merchandising.Supplier alter column
	[OrderEmail]	[varchar](100)	NULL
alter table Merchandising.Supplier alter column
	[Tags]			[varchar](max)	NULL
alter table Merchandising.Supplier alter column
	[Currency]		[varchar](10)	NULL
alter table Merchandising.Supplier add
	[State]			[varchar](150)	NULL

SET ANSI_WARNINGS ON