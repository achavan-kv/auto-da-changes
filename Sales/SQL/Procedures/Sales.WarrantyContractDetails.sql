IF OBJECT_ID('[Sales].[WarrantyContractDetails]') IS NOT NULL
	DROP PROCEDURE [Sales].[WarrantyContractDetails]
GO 

CREATE PROCEDURE [Sales].[WarrantyContractDetails] 
	@agreementNo int,
	@contractNo varchar(10)
AS
BEGIN
	SET NOCOUNT ON;


	DECLARE
		@accountNo varchar(12), 
		@agrmttaxtype varchar(1)

	SELECT @agrmttaxtype = agrmttaxtype
	  FROM country

	SELECT @accountNo = a.acctno FROM custacct a
	WHERE a.custid LIKE '%PAID & TAKEN%' AND a.acctno LIKE CAST(origbr AS varchar) + '5%'

	SELECT 
		I.itemno AS ItemNo, 
		W.itemno AS WarrantyNo, 
		O.BranchNo, 
		B.branchname AS BranchName, 
		O.CreatedBy AS EmployeeNo, 
		O.CreatedOn AS AgreementDate, 
		I.[Description] AS ItemDescription,
		ISNULL( W.WarrantyLengthMonths, 0) AS WarrantyLength, 
		CASE
				   WHEN @agrmttaxtype <> 'F' AND 
						I.quantity > 0 THEN I.price + I.TaxAmount / I.Quantity
					   ELSE I.Price
				   END AS ItemPrice, 
		CASE
					   WHEN @agrmttaxtype <> 'F' AND 
							W.quantity > 0 THEN W.price + W.TaxAmount / W.Quantity
						   ELSE W.Price
					   END AS WarrantyPrice, 
		--W.datereqdel AS DateReqDel, 
		W.[Description] AS WarrantyDescription, 
		CP.FullName AS EmployeeName,
		@accountNo AS AccountNo,
		C.Title AS CustomerTitle,
		C.FirstName AS CustomerFirstName,
		C.LastName AS CustomerLastName,
		C.AddressLine1 AS CustomerAddressLine1,
		C.AddressLine2 AS CustomerAddressLine2,
		C.AddressLine3 AS CustomerAddressLine3,
		C.PostCode AS CustomerPostCode,
		C.MobilePhone AS CustomerMobilePhone,
		C.HomePhone AS CustomerHomePhone
	FROM Sales.[Order] O 
	INNER JOIN [Sales].[OrderItem] I 
		ON O.Id = I.OrderId
	INNER JOIN [Sales].[OrderItem] W
		ON I.Id = W.ParentID
	INNER JOIN branch B
		ON O.BranchNo = B.branchno
	INNER JOIN Admin.[User] CP
		ON O.CreatedBy = CP.id
	LEFT OUTER JOIN [Sales].[OrderCustomer] C 
		ON O.Id = C.OrderId
	WHERE O.Id = @agreementNo AND W.WarrantyContractNo = @contractNo

END
GO
