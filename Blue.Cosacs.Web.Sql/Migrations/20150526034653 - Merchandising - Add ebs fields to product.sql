-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
ALTER TABLE Merchandising.Product Add ProductAction char(1) NULL
ALTER TABLE Merchandising.Product Add CreatedById int NULL
ALTER TABLE Merchandising.Product ADD ExternalCreationDate DateTime NULL

ALTER TABLE [Merchandising].Product  WITH CHECK ADD  CONSTRAINT [FK_Merchandising_Product_CreatedById] FOREIGN KEY(CreatedById)
REFERENCES [Admin].[User] ([Id])


ALTER TABLE [Merchandising].Product CHECK CONSTRAINT [FK_Merchandising_Product_CreatedById]

