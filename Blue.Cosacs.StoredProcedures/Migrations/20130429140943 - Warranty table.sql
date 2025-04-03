sp_rename 'Warranty.ProductWarranty','Warranty'
GO

sp_rename 'Warranty.ProductWarrantyTags','WarrantyTags'
GO


ALTER TABLE Warranty.Warranty
ADD Number VARCHAR(20) NOT NULL,
    Decription VARCHAR(100) NOT NULL,
    [Length] SMALLINT NOT NULL,
    TaxRate DECIMAL(3,2) NOT NULL,
	Free BIT NOT NULL,
	Deleted BIT NOT NULL
GO


