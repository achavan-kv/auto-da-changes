-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here


CREATE TABLE [dbo].[WarrantyReturnCodesAudit](
	[ProductType] [char](1) NOT NULL,
	[MonthSinceDelivery] [smallint] NOT NULL,
	[ReturnCode] [varchar](10) NOT NULL,
	[refundpercentfromAIG] [float] NOT NULL,	
	[WarrantyMonths] [int] NOT NULL,
	[ManufacturerMonths] [int] NOT NULL,
	[DateChange] [datetime] not null,
 CONSTRAINT [pk_WarrantyReturnCodesAudit] PRIMARY KEY CLUSTERED 
(
	[ProductType] ASC,
	[MonthSinceDelivery] ASC,
	[WarrantyMonths] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE WarrantyReturnCodes DROP CONSTRAINT pk_WarrantyReturnCodes

Alter TABLE WarrantyReturnCodes drop column warrantylength

Alter TABLE WarrantyReturnCodes add DateChange datetime not null default '1900-01-01'

go 

ALTER TABLE WarrantyReturnCodes ADD  CONSTRAINT pk_WarrantyReturnCodes PRIMARY KEY CLUSTERED 
(
	[ProductType] ASC,
	[MonthSinceDelivery] ASC,
	[WarrantyMonths] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO


Insert into WarrantyReturnCodesAudit(ProductType, MonthSinceDelivery, ReturnCode,
			refundpercentfromAIG, WarrantyMonths, ManufacturerMonths, DateChange)
select ProductType, MonthSinceDelivery, ReturnCode, 
		refundpercentfromAIG, WarrantyMonths, ManufacturerMonths, DateChange
From WarrantyReturnCodes

go
