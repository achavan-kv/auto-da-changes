IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE Table_Name = 'XMLDataStore'
               AND TABLE_SCHEMA = 'Merchandising')
BEGIN
CREATE TABLE  [Merchandising].[XMLDataStore](
	[POId] [int] NULL,
	[CreationDate] [date] NULL,
	[AckDoumentID] [nvarchar](50) NULL,
	[itemDescriptionClassification] [varchar](500) NULL,
	[itemDescriptionQualifier] [varchar](500) NULL,
	[descriptionValue] [varchar](500) NULL,
	[ItemNumberBuyer] [varchar](50) NULL,
	[ItemNumberSeller] [varchar](50) NULL,
	[Quantity] [int] NULL,
	[ShiptoLocationArrialDate] [date] NULL,
	[Shipid] [varchar](50) NULL,
	[AckDiscount] [money] NULL,
	[DiscountDescription] [varchar](500) NULL,
	[AdditionalChargeAmount] [money] NULL,
	[ChargeDescription] [nchar](10) NULL,
	[ackprice] [money] NULL,
	[BuyerpartyIdentifierQualifierCode] [varchar](500) NULL,
	[BuyerpartyIdentifierCode] [varchar](500) NULL,
	[SellerpartyIdentifierQualifierCode] [varchar](500) NULL,
	[SellerpartyIdentifierCode] [varchar](500) NULL,
	[Addressline1] [varchar](400) NULL,
	[City] [varchar](400) NULL,
	[partyName] [varchar](500) NULL,
	[StatePr] [varchar](500) NULL,
	[Country] [varchar](200) NULL,
	[Postalcode] [int] NULL
) ON [PRIMARY]

END
GO