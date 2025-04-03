-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Payments].[ExchangeRate]') AND type in (N'U'))
DROP TABLE [Payments].[ExchangeRate]
GO

CREATE TABLE [Payments].[ExchangeRate](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CurrencyCode] [char](3) NOT NULL,
	[Rate] [dbo].[BlueAmount] NOT NULL,
	[DateFrom] [date] NOT NULL,
	[CreatedOn] [smalldatetime] NOT NULL,
	[CreatedBy] [int] NOT NULL,
 CONSTRAINT [PK_ExchangeRate] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


ALTER TABLE [Payments].[ExchangeRate]  WITH CHECK ADD CONSTRAINT [FK_ExchangeRate_User] FOREIGN KEY([CreatedBy])
REFERENCES [Admin].[User] ([Id])
GO

ALTER TABLE [Payments].[ExchangeRate] CHECK CONSTRAINT [FK_ExchangeRate_User]
GO

IF EXISTS(SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Sales].[ExchangeRate]') AND type in (N'U'))
BEGIN

    INSERT INTO [Payments].[ExchangeRate] (CurrencyCode, Rate, DateFrom, CreatedOn, CreatedBy)
    SELECT 
        s.CurrencyCode, s.Rate, s.DateFrom, s.CreatedOn, s.CreatedBy
    FROM
        [Sales].[ExchangeRate] s

    DROP TABLE [Sales].[ExchangeRate]
END
GO


IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Payments].[CurrencyCodes]') AND type in (N'U'))
DROP TABLE [Payments].[CurrencyCodes]
GO

CREATE TABLE [Payments].[CurrencyCodes](
[Id] [int] IDENTITY(1,1) NOT NULL,
[CurrencyCode] char(3) NOT NULL,
[CurrencyName] [varchar](64) NOT NULL,		
CONSTRAINT [PK_Payments.CurrencyCodes] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

INSERT INTO Payments.[CurrencyCodes] (CurrencyCode, CurrencyName)  VALUES     ('AED','United Arab Emirates Dirham')
INSERT INTO Payments.[CurrencyCodes] (CurrencyCode, CurrencyName)  VALUES     ('AFN','Afghan Afghani')
INSERT INTO Payments.[CurrencyCodes] (CurrencyCode, CurrencyName)  VALUES     ('ALL','Albanian Lek')
INSERT INTO Payments.[CurrencyCodes] (CurrencyCode, CurrencyName)  VALUES     ('AMD','Armenian Dram')
INSERT INTO Payments.[CurrencyCodes] (CurrencyCode, CurrencyName)  VALUES     ('ANG','Netherlands Antillean Guilder')
INSERT INTO Payments.[CurrencyCodes] (CurrencyCode, CurrencyName)  VALUES     ('AOA','Angolan Kwanza')
INSERT INTO Payments.[CurrencyCodes] (CurrencyCode, CurrencyName)  VALUES     ('ARS','Argentine Peso')
INSERT INTO Payments.[CurrencyCodes] (CurrencyCode, CurrencyName)  VALUES     ('AUD','Australian Dollar')
INSERT INTO Payments.[CurrencyCodes] (CurrencyCode, CurrencyName)  VALUES     ('AWG','Aruban Florin')
INSERT INTO Payments.[CurrencyCodes] (CurrencyCode, CurrencyName)  VALUES     ('AZN','Azerbaijani Manat')
INSERT INTO Payments.[CurrencyCodes] (CurrencyCode, CurrencyName)  VALUES     ('BAM','Bosnia and Herzegovina Convertible Mark')
INSERT INTO Payments.[CurrencyCodes] (CurrencyCode, CurrencyName)  VALUES     ('BBD','Barbados Dollar')
INSERT INTO Payments.[CurrencyCodes] (CurrencyCode, CurrencyName)  VALUES     ('BDT','Bangladeshi Taka')
INSERT INTO Payments.[CurrencyCodes] (CurrencyCode, CurrencyName)  VALUES     ('BGN','Bulgarian Lev')
INSERT INTO Payments.[CurrencyCodes] (CurrencyCode, CurrencyName)  VALUES     ('BHD','Bahraini Dinar')
INSERT INTO Payments.[CurrencyCodes] (CurrencyCode, CurrencyName)  VALUES     ('BIF','Burundian Franc')
INSERT INTO Payments.[CurrencyCodes] (CurrencyCode, CurrencyName)  VALUES     ('BMD','Bermudian Dollar')
INSERT INTO Payments.[CurrencyCodes] (CurrencyCode, CurrencyName)  VALUES     ('BND','Brunei Dollar')
INSERT INTO Payments.[CurrencyCodes] (CurrencyCode, CurrencyName)  VALUES     ('BOB','Boliviano')
INSERT INTO Payments.[CurrencyCodes] (CurrencyCode, CurrencyName)  VALUES     ('BOV','Bolivian Mvdol Funds Code')
INSERT INTO Payments.[CurrencyCodes] (CurrencyCode, CurrencyName)  VALUES     ('BRL','Brazilian Real')
INSERT INTO Payments.[CurrencyCodes] (CurrencyCode, CurrencyName)  VALUES     ('BSD','Bahamian Dollar')
INSERT INTO Payments.[CurrencyCodes] (CurrencyCode, CurrencyName)  VALUES     ('BTN','Bhutanese ngultrum')
INSERT INTO Payments.[CurrencyCodes] (CurrencyCode, CurrencyName)  VALUES     ('BWP','Botswana Pula')
INSERT INTO Payments.[CurrencyCodes] (CurrencyCode, CurrencyName)  VALUES     ('BYR','Belarusian Ruble')
INSERT INTO Payments.[CurrencyCodes] (CurrencyCode, CurrencyName)  VALUES     ('BZD','Belize Dollar')
INSERT INTO Payments.[CurrencyCodes] (CurrencyCode, CurrencyName)  VALUES     ('CAD','Canadian Dollar')
INSERT INTO Payments.[CurrencyCodes] (CurrencyCode, CurrencyName)  VALUES     ('CDF','Congolese Franc')
INSERT INTO Payments.[CurrencyCodes] (CurrencyCode, CurrencyName)  VALUES     ('CHE','WIR Euro Complementary Currency')
INSERT INTO Payments.[CurrencyCodes] (CurrencyCode, CurrencyName)  VALUES     ('CHF','Swiss Franc')
INSERT INTO Payments.[CurrencyCodes] (CurrencyCode, CurrencyName)  VALUES     ('CHW','WIR Franc Complementary Currency')
INSERT INTO Payments.[CurrencyCodes] (CurrencyCode, CurrencyName)  VALUES     ('CLF','Unidad de Fomento Funds code')
INSERT INTO Payments.[CurrencyCodes] (CurrencyCode, CurrencyName)  VALUES     ('CLP','Chilean Peso')
INSERT INTO Payments.[CurrencyCodes] (CurrencyCode, CurrencyName)  VALUES     ('CNH','Chinese Yuan Traded in Hong Kong')
INSERT INTO Payments.[CurrencyCodes] (CurrencyCode, CurrencyName)  VALUES     ('CNY','Chinese Yuan')
INSERT INTO Payments.[CurrencyCodes] (CurrencyCode, CurrencyName)  VALUES     ('COP','Colombian Peso')
INSERT INTO Payments.[CurrencyCodes] (CurrencyCode, CurrencyName)  VALUES     ('COU','Unidad de Valor Real UVR Funds Code')
INSERT INTO Payments.[CurrencyCodes] (CurrencyCode, CurrencyName)  VALUES     ('CRC','Costa Rican Colon')
INSERT INTO Payments.[CurrencyCodes] (CurrencyCode, CurrencyName)  VALUES     ('CUC','Cuban Convertible Peso')
INSERT INTO Payments.[CurrencyCodes] (CurrencyCode, CurrencyName)  VALUES     ('CUP','Cuban Peso')
INSERT INTO Payments.[CurrencyCodes] (CurrencyCode, CurrencyName)  VALUES     ('CVE','Cape Verde Escudo')
INSERT INTO Payments.[CurrencyCodes] (CurrencyCode, CurrencyName)  VALUES     ('CZK','Czech Koruna')
INSERT INTO Payments.[CurrencyCodes] (CurrencyCode, CurrencyName)  VALUES     ('DJF','Djiboutian Franc')
INSERT INTO Payments.[CurrencyCodes] (CurrencyCode, CurrencyName)  VALUES     ('DKK','Danish Krone')
INSERT INTO Payments.[CurrencyCodes] (CurrencyCode, CurrencyName)  VALUES     ('DOP','Dominican Peso')
INSERT INTO Payments.[CurrencyCodes] (CurrencyCode, CurrencyName)  VALUES     ('DZD','Algerian Dinar')
INSERT INTO Payments.[CurrencyCodes] (CurrencyCode, CurrencyName)  VALUES     ('EGP','Egyptian Pound')
INSERT INTO Payments.[CurrencyCodes] (CurrencyCode, CurrencyName)  VALUES     ('ERN','Eritrean Nakfa')
INSERT INTO Payments.[CurrencyCodes] (CurrencyCode, CurrencyName)  VALUES     ('ETB','Ethiopian Birr')
INSERT INTO Payments.[CurrencyCodes] (CurrencyCode, CurrencyName)  VALUES     ('EUR','Euro')
INSERT INTO Payments.[CurrencyCodes] (CurrencyCode, CurrencyName)  VALUES     ('FJD','Fiji Dollar')
INSERT INTO Payments.[CurrencyCodes] (CurrencyCode, CurrencyName)  VALUES     ('FKP','Falkland Islands Pound')
INSERT INTO Payments.[CurrencyCodes] (CurrencyCode, CurrencyName)  VALUES     ('GBP','Pound Sterling')
INSERT INTO Payments.[CurrencyCodes] (CurrencyCode, CurrencyName)  VALUES     ('GEL','Georgian Lari')
INSERT INTO Payments.[CurrencyCodes] (CurrencyCode, CurrencyName)  VALUES     ('GHS','Ghanaian Cedi')
INSERT INTO Payments.[CurrencyCodes] (CurrencyCode, CurrencyName)  VALUES     ('GIP','Gibraltar Pound')
INSERT INTO Payments.[CurrencyCodes] (CurrencyCode, CurrencyName)  VALUES     ('GMD','Gambian Dalasi')
INSERT INTO Payments.[CurrencyCodes] (CurrencyCode, CurrencyName)  VALUES     ('GNF','Guinean Franc')
INSERT INTO Payments.[CurrencyCodes] (CurrencyCode, CurrencyName)  VALUES     ('GTQ','Guatemalan Quetzal')
INSERT INTO Payments.[CurrencyCodes] (CurrencyCode, CurrencyName)  VALUES     ('GYD','Guyanese Dollar')
INSERT INTO Payments.[CurrencyCodes] (CurrencyCode, CurrencyName)  VALUES     ('HKD','Hong Kong Dollar')
INSERT INTO Payments.[CurrencyCodes] (CurrencyCode, CurrencyName)  VALUES     ('HNL','Honduran Lempira')
INSERT INTO Payments.[CurrencyCodes] (CurrencyCode, CurrencyName)  VALUES     ('HRK','Croatian Kuna')
INSERT INTO Payments.[CurrencyCodes] (CurrencyCode, CurrencyName)  VALUES     ('HTG','Haitian Gourde')
INSERT INTO Payments.[CurrencyCodes] (CurrencyCode, CurrencyName)  VALUES     ('HUF','Hungarian Forint')
INSERT INTO Payments.[CurrencyCodes] (CurrencyCode, CurrencyName)  VALUES     ('IDR','Indonesian Rupiah')
INSERT INTO Payments.[CurrencyCodes] (CurrencyCode, CurrencyName)  VALUES     ('ILS','Israeli New Shekel')
INSERT INTO Payments.[CurrencyCodes] (CurrencyCode, CurrencyName)  VALUES     ('INR','Indian Rupee')
INSERT INTO Payments.[CurrencyCodes] (CurrencyCode, CurrencyName)  VALUES     ('IQD','Iraqi Dinar')
INSERT INTO Payments.[CurrencyCodes] (CurrencyCode, CurrencyName)  VALUES     ('IRR','Iranian Rial')
INSERT INTO Payments.[CurrencyCodes] (CurrencyCode, CurrencyName)  VALUES     ('ISK','Icelandic Króna')
INSERT INTO Payments.[CurrencyCodes] (CurrencyCode, CurrencyName)  VALUES     ('JMD','Jamaican Dollar')
INSERT INTO Payments.[CurrencyCodes] (CurrencyCode, CurrencyName)  VALUES     ('JOD','Jordanian Dinar')
INSERT INTO Payments.[CurrencyCodes] (CurrencyCode, CurrencyName)  VALUES     ('JPY','Japanese Yen')
INSERT INTO Payments.[CurrencyCodes] (CurrencyCode, CurrencyName)  VALUES     ('KES','Kenyan Shilling')
INSERT INTO Payments.[CurrencyCodes] (CurrencyCode, CurrencyName)  VALUES     ('KGS','Kyrgyzstani Som')
INSERT INTO Payments.[CurrencyCodes] (CurrencyCode, CurrencyName)  VALUES     ('KHR','Cambodian Riel')
INSERT INTO Payments.[CurrencyCodes] (CurrencyCode, CurrencyName)  VALUES     ('KMF','Comoro Franc')
INSERT INTO Payments.[CurrencyCodes] (CurrencyCode, CurrencyName)  VALUES     ('KPW','North Korean Won')
INSERT INTO Payments.[CurrencyCodes] (CurrencyCode, CurrencyName)  VALUES     ('KRW','South Korean Won')
INSERT INTO Payments.[CurrencyCodes] (CurrencyCode, CurrencyName)  VALUES     ('KWD','Kuwaiti Dinar')
INSERT INTO Payments.[CurrencyCodes] (CurrencyCode, CurrencyName)  VALUES     ('KYD','Cayman Islands Dollar')
INSERT INTO Payments.[CurrencyCodes] (CurrencyCode, CurrencyName)  VALUES     ('KZT','Kazakhstani Tenge')
INSERT INTO Payments.[CurrencyCodes] (CurrencyCode, CurrencyName)  VALUES     ('LAK','Lao Kip')
INSERT INTO Payments.[CurrencyCodes] (CurrencyCode, CurrencyName)  VALUES     ('LBP','Lebanese Pound')
INSERT INTO Payments.[CurrencyCodes] (CurrencyCode, CurrencyName)  VALUES     ('LKR','Sri Lankan Rupee')
INSERT INTO Payments.[CurrencyCodes] (CurrencyCode, CurrencyName)  VALUES     ('LRD','Liberian Dollar')
INSERT INTO Payments.[CurrencyCodes] (CurrencyCode, CurrencyName)  VALUES     ('LSL','Lesotho Loti')
INSERT INTO Payments.[CurrencyCodes] (CurrencyCode, CurrencyName)  VALUES     ('LTL','Lithuanian Litas')
INSERT INTO Payments.[CurrencyCodes] (CurrencyCode, CurrencyName)  VALUES     ('LYD','Libyan Dinar')
INSERT INTO Payments.[CurrencyCodes] (CurrencyCode, CurrencyName)  VALUES     ('MAD','Moroccan Dirham')
INSERT INTO Payments.[CurrencyCodes] (CurrencyCode, CurrencyName)  VALUES     ('MDL','Moldovan Leu')
INSERT INTO Payments.[CurrencyCodes] (CurrencyCode, CurrencyName)  VALUES     ('MGA','Malagasy Ariary')
INSERT INTO Payments.[CurrencyCodes] (CurrencyCode, CurrencyName)  VALUES     ('MKD','Macedonian Denar')
INSERT INTO Payments.[CurrencyCodes] (CurrencyCode, CurrencyName)  VALUES     ('MMK','Myanmar Kyat')
INSERT INTO Payments.[CurrencyCodes] (CurrencyCode, CurrencyName)  VALUES     ('MNT','Mongolian Tugrik')
INSERT INTO Payments.[CurrencyCodes] (CurrencyCode, CurrencyName)  VALUES     ('MOP','Macanese Pataca')
INSERT INTO Payments.[CurrencyCodes] (CurrencyCode, CurrencyName)  VALUES     ('MRO','Mauritanian Ouguiya')
INSERT INTO Payments.[CurrencyCodes] (CurrencyCode, CurrencyName)  VALUES     ('MUR','Mauritian Rupee')
INSERT INTO Payments.[CurrencyCodes] (CurrencyCode, CurrencyName)  VALUES     ('MVR','Maldivian Rufiyaa')
INSERT INTO Payments.[CurrencyCodes] (CurrencyCode, CurrencyName)  VALUES     ('MWK','Malawian Kwacha')
INSERT INTO Payments.[CurrencyCodes] (CurrencyCode, CurrencyName)  VALUES     ('MXN','Mexican Peso')
INSERT INTO Payments.[CurrencyCodes] (CurrencyCode, CurrencyName)  VALUES     ('MXV','Mexican Unidad de Inversion UDI Funds Code')
INSERT INTO Payments.[CurrencyCodes] (CurrencyCode, CurrencyName)  VALUES     ('MYR','Malaysian Ringgit')
INSERT INTO Payments.[CurrencyCodes] (CurrencyCode, CurrencyName)  VALUES     ('MZN','Mozambican Metical')
INSERT INTO Payments.[CurrencyCodes] (CurrencyCode, CurrencyName)  VALUES     ('NAD','Namibian Dollar')
INSERT INTO Payments.[CurrencyCodes] (CurrencyCode, CurrencyName)  VALUES     ('NGN','Nigerian Naira')
INSERT INTO Payments.[CurrencyCodes] (CurrencyCode, CurrencyName)  VALUES     ('NIO','Nicaraguan Cordoba')
INSERT INTO Payments.[CurrencyCodes] (CurrencyCode, CurrencyName)  VALUES     ('NOK','Norwegian Krone')
INSERT INTO Payments.[CurrencyCodes] (CurrencyCode, CurrencyName)  VALUES     ('NPR','Nepalese Rupee')
INSERT INTO Payments.[CurrencyCodes] (CurrencyCode, CurrencyName)  VALUES     ('NZD','New Zealand Dollar')
INSERT INTO Payments.[CurrencyCodes] (CurrencyCode, CurrencyName)  VALUES     ('OMR','Omani Rial')
INSERT INTO Payments.[CurrencyCodes] (CurrencyCode, CurrencyName)  VALUES     ('PAB','Panamanian Balboa')
INSERT INTO Payments.[CurrencyCodes] (CurrencyCode, CurrencyName)  VALUES     ('PEN','Peruvian Nuevo Sol')
INSERT INTO Payments.[CurrencyCodes] (CurrencyCode, CurrencyName)  VALUES     ('PGK','Papua New Guinean Kina')
INSERT INTO Payments.[CurrencyCodes] (CurrencyCode, CurrencyName)  VALUES     ('PHP','Philippine Peso')
INSERT INTO Payments.[CurrencyCodes] (CurrencyCode, CurrencyName)  VALUES     ('PKR','Pakistani Rupee')
INSERT INTO Payments.[CurrencyCodes] (CurrencyCode, CurrencyName)  VALUES     ('PLN','Polish Zioty')
INSERT INTO Payments.[CurrencyCodes] (CurrencyCode, CurrencyName)  VALUES     ('PYG','Paraguayan Guaraní')
INSERT INTO Payments.[CurrencyCodes] (CurrencyCode, CurrencyName)  VALUES     ('QAR','Qatari Riyal')
INSERT INTO Payments.[CurrencyCodes] (CurrencyCode, CurrencyName)  VALUES     ('RON','Romanian New Leu')
INSERT INTO Payments.[CurrencyCodes] (CurrencyCode, CurrencyName)  VALUES     ('RSD','Serbian Dinar')
INSERT INTO Payments.[CurrencyCodes] (CurrencyCode, CurrencyName)  VALUES     ('RUB','Russian Ruble')
INSERT INTO Payments.[CurrencyCodes] (CurrencyCode, CurrencyName)  VALUES     ('RWF','Rwandan Franc')
INSERT INTO Payments.[CurrencyCodes] (CurrencyCode, CurrencyName)  VALUES     ('SAR','Saudi Riyal')
INSERT INTO Payments.[CurrencyCodes] (CurrencyCode, CurrencyName)  VALUES     ('SBD','Solomon Islands Dollar')
INSERT INTO Payments.[CurrencyCodes] (CurrencyCode, CurrencyName)  VALUES     ('SCR','Seychelles Rupee')
INSERT INTO Payments.[CurrencyCodes] (CurrencyCode, CurrencyName)  VALUES     ('SDG','Sudanese Pound')
INSERT INTO Payments.[CurrencyCodes] (CurrencyCode, CurrencyName)  VALUES     ('SEK','Swedish Krona Kronor')
INSERT INTO Payments.[CurrencyCodes] (CurrencyCode, CurrencyName)  VALUES     ('SGD','Singapore Dollar')
INSERT INTO Payments.[CurrencyCodes] (CurrencyCode, CurrencyName)  VALUES     ('SHP','Saint Helena Pound')
INSERT INTO Payments.[CurrencyCodes] (CurrencyCode, CurrencyName)  VALUES     ('SLL','Sierra Leonean Leone')
INSERT INTO Payments.[CurrencyCodes] (CurrencyCode, CurrencyName)  VALUES     ('SOS','Somali Shilling')
INSERT INTO Payments.[CurrencyCodes] (CurrencyCode, CurrencyName)  VALUES     ('SRD','Surinamese Dollar')
INSERT INTO Payments.[CurrencyCodes] (CurrencyCode, CurrencyName)  VALUES     ('SSP','South Sudanese Pound')
INSERT INTO Payments.[CurrencyCodes] (CurrencyCode, CurrencyName)  VALUES     ('STD','Sao Tome and Principe Dobra')
INSERT INTO Payments.[CurrencyCodes] (CurrencyCode, CurrencyName)  VALUES     ('SYP','Syrian Pound')
INSERT INTO Payments.[CurrencyCodes] (CurrencyCode, CurrencyName)  VALUES     ('SZL','Swazi Lilangeni')
INSERT INTO Payments.[CurrencyCodes] (CurrencyCode, CurrencyName)  VALUES     ('THB','Thai Baht')
INSERT INTO Payments.[CurrencyCodes] (CurrencyCode, CurrencyName)  VALUES     ('TJS','Tajikistani Somoni')
INSERT INTO Payments.[CurrencyCodes] (CurrencyCode, CurrencyName)  VALUES     ('TMT','Turkmenistani Manat')
INSERT INTO Payments.[CurrencyCodes] (CurrencyCode, CurrencyName)  VALUES     ('TND','Tunisian Dinar')
INSERT INTO Payments.[CurrencyCodes] (CurrencyCode, CurrencyName)  VALUES     ('TOP','Tongan Paanga')
INSERT INTO Payments.[CurrencyCodes] (CurrencyCode, CurrencyName)  VALUES     ('TRY','Turkish Lira')
INSERT INTO Payments.[CurrencyCodes] (CurrencyCode, CurrencyName)  VALUES     ('TTD','Trinidad and Tobago Dollar')
INSERT INTO Payments.[CurrencyCodes] (CurrencyCode, CurrencyName)  VALUES     ('TWD','New Taiwan Dollar')
INSERT INTO Payments.[CurrencyCodes] (CurrencyCode, CurrencyName)  VALUES     ('TZS','Tanzanian Shilling')
INSERT INTO Payments.[CurrencyCodes] (CurrencyCode, CurrencyName)  VALUES     ('UAH','Ukrainian Hryvnia')
INSERT INTO Payments.[CurrencyCodes] (CurrencyCode, CurrencyName)  VALUES     ('UGX','Ugandan Shilling')
INSERT INTO Payments.[CurrencyCodes] (CurrencyCode, CurrencyName)  VALUES     ('USD','United States Dollar')
INSERT INTO Payments.[CurrencyCodes] (CurrencyCode, CurrencyName)  VALUES     ('USN','United States Dollar Nnext Day Funds Code')
INSERT INTO Payments.[CurrencyCodes] (CurrencyCode, CurrencyName)  VALUES     ('USS','United States Dollar Same Day Funds Code')
INSERT INTO Payments.[CurrencyCodes] (CurrencyCode, CurrencyName)  VALUES     ('UYI','Uruguay Peso en Unidades Indexadas Funds Code')
INSERT INTO Payments.[CurrencyCodes] (CurrencyCode, CurrencyName)  VALUES     ('UYU','Uruguayan Peso')
INSERT INTO Payments.[CurrencyCodes] (CurrencyCode, CurrencyName)  VALUES     ('UZS','Uzbekistan Som')
INSERT INTO Payments.[CurrencyCodes] (CurrencyCode, CurrencyName)  VALUES     ('VEF','Venezuelan Bolivar')
INSERT INTO Payments.[CurrencyCodes] (CurrencyCode, CurrencyName)  VALUES     ('VND','Vietnamese Dong')
INSERT INTO Payments.[CurrencyCodes] (CurrencyCode, CurrencyName)  VALUES     ('VUV','Vanuatu Vatu')
INSERT INTO Payments.[CurrencyCodes] (CurrencyCode, CurrencyName)  VALUES     ('WST','Samoan Tala')
INSERT INTO Payments.[CurrencyCodes] (CurrencyCode, CurrencyName)  VALUES     ('XAF','CFA Franc BEAC')
INSERT INTO Payments.[CurrencyCodes] (CurrencyCode, CurrencyName)  VALUES     ('XAG','Silver One Troy Ounce')
INSERT INTO Payments.[CurrencyCodes] (CurrencyCode, CurrencyName)  VALUES     ('XAU','Gold One Troy Ounce')
INSERT INTO Payments.[CurrencyCodes] (CurrencyCode, CurrencyName)  VALUES     ('XBA','European Composite Unit Bond Market Unit')
INSERT INTO Payments.[CurrencyCodes] (CurrencyCode, CurrencyName)  VALUES     ('XBB','European Monetary Unit Bond Market Unit')
INSERT INTO Payments.[CurrencyCodes] (CurrencyCode, CurrencyName)  VALUES     ('XBC','European Unit of Account 9 Bond Market Unit')
INSERT INTO Payments.[CurrencyCodes] (CurrencyCode, CurrencyName)  VALUES     ('XBD','European Unit of Account 17 Bond Market Unit')
INSERT INTO Payments.[CurrencyCodes] (CurrencyCode, CurrencyName)  VALUES     ('XCD','East Caribbean Dollar')
INSERT INTO Payments.[CurrencyCodes] (CurrencyCode, CurrencyName)  VALUES     ('XDR','Special Drawing Rights')
INSERT INTO Payments.[CurrencyCodes] (CurrencyCode, CurrencyName)  VALUES     ('XFU','UIC Franc Special Settlement Currency')
INSERT INTO Payments.[CurrencyCodes] (CurrencyCode, CurrencyName)  VALUES     ('XOF','CFA Franc BCEAO')
INSERT INTO Payments.[CurrencyCodes] (CurrencyCode, CurrencyName)  VALUES     ('XPD','Palladium One Troy Ounce')
INSERT INTO Payments.[CurrencyCodes] (CurrencyCode, CurrencyName)  VALUES     ('XPF','CFP Franc Pacifique')
INSERT INTO Payments.[CurrencyCodes] (CurrencyCode, CurrencyName)  VALUES     ('XPT','Platinum One Troy Ounce')
INSERT INTO Payments.[CurrencyCodes] (CurrencyCode, CurrencyName)  VALUES     ('XSU','SUCRE')
INSERT INTO Payments.[CurrencyCodes] (CurrencyCode, CurrencyName)  VALUES     ('XUA','ADB Unit of Account')
INSERT INTO Payments.[CurrencyCodes] (CurrencyCode, CurrencyName)  VALUES     ('YER','Yemeni Rial')
INSERT INTO Payments.[CurrencyCodes] (CurrencyCode, CurrencyName)  VALUES     ('ZAR','South African Rand')
INSERT INTO Payments.[CurrencyCodes] (CurrencyCode, CurrencyName)  VALUES     ('ZMW','Zambian Kwacha')
INSERT INTO Payments.[CurrencyCodes] (CurrencyCode, CurrencyName)  VALUES     ('ZWD','Zimbabwe Dollar')


IF EXISTS(SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Sales].[CurrencyCodes]') AND type in (N'U'))
BEGIN
    DROP TABLE [Sales].[CurrencyCodes]
END
GO



