-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here


Create table Merchandising.Promotion
( 
Id int identity(1,1)
,Name varchar(100) 
,Fascia varchar(100)
,LocationId int 
,StartDate date
,EndDate date
,PromotionType varchar(100) 

,PRIMARY KEY (Id)
,CONSTRAINT FK_Promotion_Location FOREIGN KEY (LocationId) REFERENCES Merchandising.Location(Id)
)


Create Table Merchandising.PromotionDetail
(
Id int identity(1,1)
,PromotionId int
,ProductId int
,PriceType varchar(100)
,Price Money
,ValueDiscount Money
,PercentDiscount float

,PRIMARY KEY (Id)
,CONSTRAINT FK_PromotionDetail_Promotion FOREIGN KEY (PromotionId) REFERENCES Merchandising.Promotion(Id)
,CONSTRAINT FK_PromotionDetail_Product FOREIGN KEY (ProductId) REFERENCES Merchandising.Product(Id)
)


Create Table Merchandising.PromotionHierarchy
(
Id int identity(1,1)
,PromotionDetailId int
,LevelId int
,TagId int
PRIMARY KEY (Id)
,CONSTRAINT FK_PromotionHierarchy_HierarchyLevel FOREIGN KEY (LevelId) REFERENCES Merchandising.HierarchyLevel(Id)
,CONSTRAINT FK_PromotionHierarchy_HierarchyTag FOREIGN KEY (TagId) REFERENCES Merchandising.HierarchyTag(Id)
)