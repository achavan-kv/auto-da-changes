CREATE TABLE Warranty.WarrantyPrice
(
	Id int IDENTITY(1,1) NOT NULL,
	WarrantyId int NOT NULL,
	BranchType varchar(20),
	BranchNumber smallint,
	CostPrice money NOT NULL,
	RetailPrice money NOT NULL
)

ALTER TABLE Warranty.WarrantyPrice
ADD CONSTRAINT [PK_WarrantyPrice] PRIMARY KEY (Id)

ALTER TABLE Warranty.WarrantyPrice
ADD CONSTRAINT [FK_WarrantyPrice_Warranty]
FOREIGN KEY ([WarrantyId])
REFERENCES [Warranty].[Warranty] ([Id])
ON DELETE CASCADE

ALTER TABLE Warranty.WarrantyPrice
ADD CONSTRAINT [FK_WarrantyPrice_BranchNumber] FOREIGN KEY ([BranchNumber])
REFERENCES [dbo].[branch] ([branchno])
