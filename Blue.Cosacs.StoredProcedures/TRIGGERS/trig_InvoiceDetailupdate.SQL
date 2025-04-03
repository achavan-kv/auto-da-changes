SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF  EXISTS (SELECT * FROM sys.triggers WHERE object_id = OBJECT_ID(N'[Sales].[trig_InvoiceDetailupdate]'))
DROP TRIGGER [Sales].[trig_InvoiceDetailupdate]
GO 

CREATE TRIGGER [Sales].[trig_InvoiceDetailupdate]
ON [Sales].[OrderItem] For INSERT
AS 
-- Script Name  : 6213425_Curacao
--
-- Created For  : Curacao
-- Created By	: Santosh Kanade
-- Created On	: 20/08/2019
-- Comment :  Implememted a case condition for itemno Eg. DEC,DFO etc... instead of Price use ManualDiscount
BEGIN
DECLARE 
	@OriginalOrderId int = 0,
	@AgreementInvoiceNumber varchar(14),
	@acctno varchar(12)='0',
	@ParentItemID int = 0

   select @OriginalOrderId=OriginalOrderId FROM [Sales].[Order] o
		INNER JOIN Inserted i on i.OrderId = o.id 
   --If original order id > o then this is from GRT screen 
   IF(@OriginalOrderId > 0)
   BEGIN
		----fetch account number based on parent item
		SELECT @acctno=acctno from [sales].[order] so
		INNER JOIN Agreement on agrmtno = so.id
		WHERE id in (@OriginalOrderId)

		 SELECT @AgreementInvoiceNumber= o.AgreementInvoiceNumber FROM [Sales].[Order] o
		 INNER JOIN Inserted i on i.OrderId = o.id
			--Insert data in Invoice Details table.
		SELECT @ParentItemID=id FROM merchandising.Product where SKU in(SELECT ParentItemNo FROM Inserted)
		
		
			INSERT INTO [dbo].[InvoiceDetails]
		([acctno],[agrmtno],[InvoiceVersion],[datedel],[itemno],[stocklocn],[quantity],[branchno],Price,taxamt,ItemID,ParentItemID,AgreementInvNoVersion,contractno,returnquantity,RetItemNo,RetVal)
		SELECT distinct @acctno,o.id,1,CreatedOn,ItemNo,BranchNo,Quantity,BranchNo, 
		--(Price+TaxAmount),
		case when ((ManualDiscount is not null or ManualDiscount <>0) and (ManualDiscountPercentage is not null or ManualDiscountPercentage<>0) and WarrantyTypeCode IS NULL) then (ManualDiscount+TaxAmount) else  (Price+TaxAmount) end as Price,
		(Quantity*TaxAmount),ProductItemId,@ParentItemID,@AgreementInvoiceNumber,WarrantyContractNo,-ReturnQuantity,ItemNo,Price
		
		FROM [Sales].[Order] o
		INNER JOIN Inserted i on i.OrderId = o.id 
   END
END

