-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here


ALTER TABLE Merchandising.CintOrder 
ADD PromotionId int null 

ALTER TABLE [Merchandising].CintOrder
WITH CHECK ADD CONSTRAINT FK_CintOrder_PromotionId_Promotion
FOREIGN KEY (PromotionId)
REFERENCES [Merchandising].Promotion(Id) 