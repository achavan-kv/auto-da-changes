  IF EXISTS (SELECT * FROM sys.objects  WHERE object_id = OBJECT_ID(N'[dbo].[DN_GetSalesOrderItemSP]') AND type IN (N'P', N'PC'))
  DROP PROCEDURE [dbo].[DN_GetSalesOrderItemSP]
GO 
create  PROCEDURE [dbo].[DN_GetSalesOrderItemSP]
	-- Add the parameters for the stored procedure here
@agreementNo int,
	@AgreementInvoiceNumber varchar(14),
	@return int OUTPUT
AS
BEGIN

	set @return = 0;

	 Declare @Icounter int ,@Price float,@ParentItemPrice float,@NumberOfRecords int,@ParentId varchar(15),@Addisionaltaxrate float,@countrycode varchar(10),
	 @itemNo varchar(18),@taxrate float
	 select row_number() OVER (ORDER BY o.ID) as rownumber ,
	 o.ID,
	 o.originalorderID,
	 o.TotalAmount, o.TotalTaxAmount, o.BranchNo as stocklocn, o.Agreementinvoicenumber
	,so.ItemTypeID,
	 so.ItemNo,
	 (CASE WHEN so.PosDescription = '' THEN so.Description ELSE so.PosDescription END) as Description, 
	so.Quantity 
	--,((so.Quantity)*( so.Price + so.TaxAmount))  as Price
	,  CASE WHEN (  so.ManualDiscountPercentage IS NOT NULL 
					AND ManualDiscount IS NOT NULL 
					AND so.ManualDiscountPercentage<>0 
					AND ManualDiscount<>0	)  
				THEN  ((so.Quantity)*( so.ManualDiscount + so.TaxAmount)) 
			ELSE ((so.Quantity)*( so.Price + so.TaxAmount)) 
		End as Price
	--,(CASE WHEN so.Price = 0 THEN Country    ELSE City END)
	,  so.TaxRate as TaxRate
	, (so.Quantity * so.TaxAmount) as TaxAmount
	, so.ProductItemID as ItemID
	, so.Returned
	, so.WarrantyLengthMonths, so.WarrantyEffectiveDate, so.WarrantyContractNo as ContractNo
	, so.WarrantyTypeCode, so.WarrantyLinkId
	, so.ParentId
	, so.id as Orderitemid
	--, (so.Price + so.TaxAmount) * so.Quantity as orderval
	, (CASE WHEN so.ItemTypeId != 5 THEN ((so.Price + so.TaxAmount) * so.Quantity) 
		--ELSE (o.IstaxFreeSale ? 0 : (so.ManualDiscount * so.Quantity) * taxRate * 0.01M)  END)	
		ELSE (CASE WHEN o.IsTaxFreeSale = 1 THEN (0 + (so.ManualDiscount * so.Quantity)) ELSE (((so.ManualDiscount * so.Quantity) * taxRate * 0.01) + (so.ManualDiscount * so.Quantity)) END)  
		END) as orderval
	,0 as AdditionalTaxRate
	, o.BranchNo as BranchNo
	, so.PosDescription
	INTO #SalesData
	FROM [Sales].[Order] o
	INNER JOIN [Sales].[Orderitem] so on so.OrderId = o.id 
	where AgreementInvoiceNumber = @AgreementInvoiceNumber


	 SET @Addisionaltaxrate = 0
	 SET @Icounter = 1
	 SELECT @NumberOfRecords= count(id) from  #SalesData
	 --Loop through each record and update table.
	 WHILE (@Icounter <= @NumberOfRecords)
	 BEGIN
		---Check price for selected item.--------------------
		SELECT @Price=Price,@ParentId=ParentId,@itemNo = itemNo FROM #SalesData WHERE rownumber= @Icounter
		------ if price is = 0 then get its parent item price-----
			IF (@Price = 0 and @ParentId IS NOT NULL)
			BEGIN
				SET @ParentItemPrice = NULL
				 ----Fetch price of parent id and update in temp data----------
				 select @ParentItemPrice=Price from #SalesData where Orderitemid= @ParentId
				 ------Update Price for child item as equal to parent price------------------
				 UPDATE #SalesData
				 SET orderval=@ParentItemPrice
				 WHERE rownumber= @Icounter
			END-- End of (@Price = 0 and @ParentId IS NOT NULL)

			-------------Calculation for additional Taxrate----------------------------------
			SELECT @countrycode = countrycode FROM country
			IF(@countrycode ='Q')
			BEGIN
				SET @Addisionaltaxrate = 0
				select @taxrate = ( Rate * 100) from [Merchandising].[TaxRate] where Name = 'OB' and productid is NULL and EffectiveDate < getdate()
				select @Addisionaltaxrate = Rate from [Merchandising].[TaxRate] where Name = 'LUX' and productid in (select id from Merchandising.Product where sku = @itemNo) and EffectiveDate < getdate()
				IF(@Addisionaltaxrate > 0)
				BEGIN
				  SET @Addisionaltaxrate = @Addisionaltaxrate
				END 
				UPDATE #SalesData
				SET AdditionalTaxRate = @Addisionaltaxrate
				WHERE rownumber= @Icounter
			END -- END OF (@countrycode ='Q')
			-----------------------------------------------------------------------------
			SET @Icounter = @Icounter + 1
	 END-- End of while

	 -----As price of single item saved in db we need to multiply with quantity.


	

	 SELECT
	 ID,
	 originalorderID,
	 TotalAmount, TotalTaxAmount, stocklocn, Agreementinvoicenumber
	,ItemTypeID,
	 ItemNo,
	 Description, 
	Quantity 
	,Price
	,TaxRate
	,TaxAmount
	,ItemID
	,Returned
	,WarrantyLengthMonths,WarrantyEffectiveDate,ContractNo
	,WarrantyTypeCode, WarrantyLinkId
	,ParentId
	,Orderitemid
	,orderval
	,AdditionalTaxRate
	, BranchNo
	, PosDescription
	FROM #SalesData

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END	


END
