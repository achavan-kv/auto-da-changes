if exists (select * from dbo.sysobjects where id = object_id('[dbo].[VE_UpdateParentSKUMaster]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
BEGIN
	DROP PROCEDURE [dbo].[VE_UpdateParentSKUMaster]
END
GO

Create PROCEDURE [dbo].[VE_UpdateParentSKUMaster] 
	@XmlString XML 
	,@Message VARCHAR(MAX) OUTPUT
	,@StatusCode INT OUTPUT  
AS 
BEGIN
BEGIN TRY
	SET NOCOUNT ON;   
	SET @Message = '';
	SET @StatusCode = 0    
	BEGIN TRANSACTION
	DECLARE @externalProductID VARCHAR(20) 
	
	SELECT  @externalProductID =n.x.value('externalProductID[1]' ,'int')
	FROM @XmlString.nodes('/ParentSKUUpdate') n(x)
	
		update Merchandising.Product
		set 
		POSDescription = n.x.value('description[1]' ,'[varchar](240)'),
		VendorUPC = n.x.value('upc[1]' ,'[varchar](60)'),
		PrimaryVendorId = n.x.value('externalVendorID[1]' ,'int')
		FROM @XmlString.nodes('/ParentSKUUpdate') n(x) 
		WHERE Merchandising.Product.Id = @externalProductID
		
		update  Merchandising.RetailPrice 
		set RegularPrice = n.x.value('Retail[1]' ,'decimal(15,4)')
	    FROM @XmlString.nodes('/ParentSKUUpdate/Branches/Branch') n(x) 
		INNER JOIN [Merchandising].[Location] l ON l.SalesId = n.x.value('BranchNo[1]' ,'varchar(100)')
		INNER JOIN [Merchandising].[RetailPrice] mrp ON mrp.LocationId = l.Id
		WHERE mrp.ProductId = @externalProductID		

		update Merchandising.ProductStockLevel
		set StockAvailable = n.x.value('Quantity[1]' ,'int')
		FROM @XmlString.nodes('/ParentSKUUpdate/Branches/Branch') n(x) 
		INNER JOIN [Merchandising].[Location] l ON l.SalesId = n.x.value('BranchNo[1]' ,'varchar(100)')
		INNER JOIN [Merchandising].[ProductStockLevel] psl ON psl.LocationId = l.Id
		WHERE psl.ProductId = @externalProductID		

	IF (@@error != 0)
		BEGIN
			ROLLBACK
			print'Transaction rolled back'
			SET @StatusCode = 500;		
		END
		ELSE
		BEGIN
			SET @StatusCode = 200;		
			SET @Message = 'Customer information updated succesfully.'		
			print CONVERT(VARCHAR(3), @StatusCode)
			COMMIT
			print'Transaction committed'
		END 
	
	END TRY  
		BEGIN CATCH  
			IF (@@error > 0)
		SET @StatusCode = 500;		
		SET @Message = ERROR_Message()
       ROLLBACK TRAN  
	END CATCH
END