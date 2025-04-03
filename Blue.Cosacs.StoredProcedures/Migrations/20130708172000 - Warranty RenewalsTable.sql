CREATE TABLE Warranty.Renewal
(
	Id INT Identity(1,1),
	WarrantyId INT NOT NULL,
	RenewalId INT NOT NULL
)
GO

ALTER TABLE Warranty.Renewal
ADD CONSTRAINT PK_WarrantyRenewal PRIMARY KEY (Id)

ALTER TABLE Warranty.Renewal
ADD CONSTRAINT FK_RenewalsWarrantyId_Warranty FOREIGN KEY (WarrantyID) 
REFERENCES Warranty.Warranty(id)

ALTER TABLE Warranty.Renewal
ADD CONSTRAINT FK_RenewalsRenewalId_Warranty FOREIGN KEY (RenewalId) 
REFERENCES Warranty.Warranty(id)

ALTER TABLE Warranty.Renewal
ADD UNIQUE (WarrantyId,RenewalId)
GO
