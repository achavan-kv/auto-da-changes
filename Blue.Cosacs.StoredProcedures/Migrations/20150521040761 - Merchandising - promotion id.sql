-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here


-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
IF(EXISTS (SELECT 1 FROM sys.tables t WHERE t.name = 'temp_promoload'))
BEGIN
	IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE Name = N'PromotionId' AND Object_ID = OBJECT_ID(N'temp_promoload')) 
	BEGIN
		TRUNCATE TABLE temp_promoload

		ALTER TABLE temp_promoload ADD PromotionId INT NOT NULL
	END
END

IF(EXISTS (SELECT 1 FROM sys.tables t WHERE t.name = 'temp_rawpromoload'))
BEGIN
	IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE Name = N'PromotionId' AND Object_ID = OBJECT_ID(N'temp_rawpromoload')) 
	BEGIN
		TRUNCATE TABLE temp_rawpromoload

		ALTER TABLE temp_rawpromoload ADD PromotionId INT NOT NULL
	END
END


IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE Name = N'PromotionId' AND Object_ID = OBJECT_ID(N'promoprice')) 
BEGIN
	ALTER TABLE promoprice ADD PromotionId INT NULL  
END
GO


UPDATE promoprice SET PromotionId = -1 where PromotionId is null
GO


 IF COLUMNPROPERTY(OBJECT_ID('promoprice', 'U'), 'PromotionId', 'AllowsNull') = 1
 BEGIN
        ALTER TABLE promoprice ALTER COLUMN PromotionId INT NOT NULL
 END
 GO


 
