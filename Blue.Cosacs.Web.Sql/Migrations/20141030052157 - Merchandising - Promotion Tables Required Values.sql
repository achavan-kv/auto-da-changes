-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

--UPDATE [Merchandising].[Promotion] 


ALTER TABLE [Merchandising].[Promotion] 
ALTER COLUMN Name varchar(100)  NOT NULL

ALTER TABLE [Merchandising].[Promotion] 
ALTER COLUMN StartDate date NOT NULL

ALTER TABLE [Merchandising].[Promotion] 
ALTER COLUMN EndDate date NOT NULL

ALTER TABLE [Merchandising].[Promotion] 
ALTER COLUMN PromotionType varchar(100) NOT NULL


ALTER TABLE [Merchandising].[PromotionDetail] 
ALTER COLUMN PriceType varchar(100) NOT NULL

ALTER TABLE Merchandising.PromotionHierarchy
ADD CONSTRAINT FK_PromotionHierarchy_PromotionDetail FOREIGN KEY (PromotionDetailId) REFERENCES Merchandising.PromotionDetail(Id)


