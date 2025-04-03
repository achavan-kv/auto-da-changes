

IF  NOT EXISTS (SELECT * FROM SYS.OBJECTS WHERE OBJECT_ID = OBJECT_ID('NonSaleableNonStocks') AND TYPE IN (N'U'))
BEGIN

	CREATE TABLE [dbo].[NonSaleableNonStocks]
	(
		[Id] INT PRIMARY KEY IDENTITY(1,1),
		[SKU] VARCHAR(18) NOT NULL,
		[Type] VARCHAR(8) NULL
	)

	INSERT INTO [dbo].[NonSaleableNonStocks] 
	SELECT	[SKU], [Type]
	FROM	Nonstocks.Nonstock 
	WHERE	[Type] LIKE '%discount%'

END