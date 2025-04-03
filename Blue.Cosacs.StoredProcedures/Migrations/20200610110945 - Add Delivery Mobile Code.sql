-- Date      By					Description
-- ----      --					-----------
-- 10/6/2020 Snehalata Tilekar Address Standardization CR2019 - 025

DECLARE @origbr int

-- Populate CountryCode
SELECT TOP 1 @origbr = origbr FROM country WITH(NOLOCK)

IF NOT EXISTS (SELECT * FROM [code] WITH(NOLOCK) WHERE codedescript='Delivery Mobile' and category='CT1')
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
			,'DM'
			,'Delivery Mobile'
			,'L'
			,0
			,0
			,NULL
			,NULL); 

END