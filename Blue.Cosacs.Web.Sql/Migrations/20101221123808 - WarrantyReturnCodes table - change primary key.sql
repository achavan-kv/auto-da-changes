-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

ALTER TABLE WarrantyReturnCodes DROP CONSTRAINT pk_WarrantyReturnCodes

ALTER TABLE WarrantyReturnCodesAudit DROP CONSTRAINT pk_WarrantyReturnCodesAudit

go 

ALTER TABLE WarrantyReturnCodes ADD  CONSTRAINT pk_WarrantyReturnCodes PRIMARY KEY CLUSTERED 
(
	ProductType ASC,
	MonthSinceDelivery ASC,
	WarrantyMonths ASC,
	ManufacturerMonths asc
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]

ALTER TABLE WarrantyReturnCodesAudit ADD  CONSTRAINT pk_WarrantyReturnCodesAudit PRIMARY KEY CLUSTERED 
(
	ProductType ASC,
	MonthSinceDelivery ASC,
	WarrantyMonths ASC,
	ManufacturerMonths asc,
	DateChange asc
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
