
--Snehalata Tilekar Address Standardization CR2019
-- 10/6/2020 Snehalata Tilekar Address Standardization CR2019 - 025 -INSERT Multiple delivery mobile  
DECLARE @origbr int

-- Populate CountryCode
SELECT TOP 1 @origbr = origbr FROM country WITH(NOLOCK)

IF NOT EXISTS (SELECT 1 FROM [code] WITH(NOLOCK) WHERE codedescript='Delivery 1 Mobile' and category='CT1')
BEGIN
INSERT INTO [dbo].[code]
           ([origbr]
           ,[category]
           ,[code]
           ,[codedescript]
           ,[statusflag]
           ,[sortorder]
           ,[reference]
           ,[additional]
           ,[Additional2])
	VALUES ( @origbr 
			,'CT1'
			,'D1M'
			,'Delivery 1 Mobile'
			,'L'
			,0
			,0
			,NULL
			,NULL); 
			END



IF NOT EXISTS (SELECT 1 FROM [code] WITH(NOLOCK) WHERE codedescript='Delivery 2 Mobile' and category='CT1')
BEGIN
INSERT INTO [dbo].[code]
           ([origbr]
           ,[category]
           ,[code]
           ,[codedescript]
           ,[statusflag]
           ,[sortorder]
           ,[reference]
           ,[additional]
           ,[Additional2])
	VALUES ( @origbr 
			,'CT1'
			,'D2M'
			,'Delivery 2 Mobile'
			,'L'
			,0
			,0
			,NULL
			,NULL); 
			END


IF NOT EXISTS (SELECT 1 FROM [code] WITH(NOLOCK) WHERE codedescript='Delivery 3 Mobile' and category='CT1')
BEGIN

INSERT INTO [dbo].[code]
           ([origbr]
           ,[category]
           ,[code]
           ,[codedescript]
           ,[statusflag]
           ,[sortorder]
           ,[reference]
           ,[additional]
           ,[Additional2])
	VALUES ( @origbr 
			,'CT1'
			,'D3M'
			,'Delivery 3 Mobile'
			,'L'
			,0
			,0
			,NULL
			,NULL);
			END 

GO


