SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_OracleInteg_GetARDetail]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_OracleInteg_GetARDetail]
GO

--Author : Mohamed Nasmi
CREATE PROCEDURE [dbo].[DN_OracleInteg_GetARDetail] 
	@runNo int,
	@isRerun bit,
	@return int output
		
AS
	SET @return = 0		-- initialise return code

-----------------------------------------------------------------------------------------------------

SELECT  
--- Header ---------------------------------------------------------------
Case
	When SUBSTRING(OE.AcctNo,4,1) < '4' Then 'Credit'
	Else 'Cash'
End as TranType,

Case
	When DEL.DelOrColl = 'D' Then 'Invoice' 
	When DEL.DelOrColl = 'C' Then 'Credit Memo' 
	When DEL.DelOrColl = 'R' Then 'Repossession'	-- Repossession
End as TranClass,

DEL.DateTrans as TranDate, GETDATE() as GLDate, Ag.EmpeeNoSale,

--Case 
--	When DEL.BuffBranchNo != 0 Then CAST(DEL.BuffBranchNo as CHAR(3)) + CAST(DEL.BuffNo as varchar(7)) 
--	Else CAST(DEL.BuffNo as varchar(10))
--End ,

ISNULL(DEL.ExtInvoice,'') as InvoiceReference,

Case 
	When DEL.DelOrColl != 'D' then CAST(DEL.DelOrColl as varchar(10))
	Else CAST(' ' as varchar(10))
End as CredInvRef,

Case 
	When SUBSTRING(OE.AcctNo,4,1) >= '4' Then 0
	Else IP.InstalNo 
End as PayTerm,

EMP.FullName as SalesPerson, UPPER(CA.CustId) as CustomerID, OE.AcctNo, 
ISNULL(BA.AddressID,0) as BillAdrRef,
ISNULL(SA.AddressID,0) as ShipAdrRef,
---------------------------------------------------------------------------
		
-- Line -------------------------------------------------------------------
OE.ItemNo, CAST(REPLACE(ISNULL(SI.ItemDescr1,'Not Known'),',',' ') as varchar(25)) as LineDescription,
'Unit' as UOM, DEL.Quantity,

Case
	When SI.Category IN (Select DISTINCT Reference from Code where Category = 'dis') Then 'DISCOUNT'
	When OE.ItemNo = 'DT' Then 'DT'
	Else ''
End as ItemType,

Case
	When LI.ItemType = 'N' and LI.Quantity != 0 Then ROUND(DEL.TransValue -(LI.TaxAmt/LI.Quantity),2) 	-- non stock (l.price could be 0) Exclusive of tax
	When DEL.Quantity != 0 Then ROUND((DEL.TransValue - (ISNULL(LI.TaxAmt,0)/DEL.Quantity))/DEL.Quantity,2) 	-- Exclusive of tax
	Else DEL.TransValue - ISNULL(LI.TaxAmt,0)		-- 11/08/08	
End as UnitPrice,

Case
	When LI.ItemType = 'N' and LI.Quantity != 0 Then ROUND(DEL.TransValue - (LI.TaxAmt/LI.Quantity),2) 	-- non stock (LI.price could be 0) Exclusive of tax
	When DEL.Quantity != 0 Then ROUND((DEL.TransValue - (ISNULL(LI.TaxAmt,0)/DEL.Quantity)),2)   -- Exclusive of tax
	Else DEL.TransValue - ISNULL(LI.TaxAmt,0)
End as LineValue,

Case	
	--When CTRY.AgrmtTaxType = 'I' Then 'N'		-- Tax Inclusive??
	When LI.TaxAmt = 0 Then 'N'		-- No Tax
	Else 'Y'
End as TaxFlag,

CM.Value as TaxCode, CTRY.TaxRate, ISNULL(DEL.InvoiceLineNo,0) as LineRef, 'AcctCode??' as AccountCode,
OE.AcctNo as KAcctNo, OE.AgrmtNo as KAgrmtNo, OE.ItemNo as KItemNo, OE.StockLocn as KStockLocn,
OE.ContractNo as KContractNo, OE.RunNo, SUBSTRING(OE.AcctNo,1,3) as BranchNo, DEL.TransRefNo as KTransRefNo,
DEL.DateDel as DelDate,

Case 
	When DEL.DelOrColl = 'R' Then DEL.RetItemNo 
	Else '.' 
End as RetItemNo -- jec 24/11/08 70504
---------------------------------------------------------------------------

INTO #TempOracleAR  -- Oracle Account Receivables
FROM LineItemOracleExport OE 	

INNER JOIN Delivery DEL on OE.ItemNo = DEL.ItemNo and OE.AcctNo = DEL.AcctNo and OE.AgrmtNo = DEL.AgrmtNo 
			and OE.StockLocn = DEL.StockLocn and OE.ContractNo = DEL.ContractNo and OE.BuffNo = DEL.BuffNo
LEFT JOIN CustAcct CA on CA.HldorJnt = 'H' and OE.AcctNo = CA.AcctNo 
LEFT JOIN Agreement AG on OE.AcctNo = AG.AcctNo and OE.AgrmtNo = AG.AgrmtNo
LEFT JOIN InstalPlan IP on OE.AcctNo = IP.AcctNo and OE.AgrmtNo = IP.Agrmtno
LEFT JOIN LineItem LI on OE.ItemNo = LI.ItemNo and OE.AcctNo = LI.AcctNo and OE.AgrmtNo = LI.AgrmtNo 
			and OE.StockLocn = LI.StockLocn and OE.ContractNo = LI.ContractNo 
LEFT JOIN CustAddress BA on CA.CustId = BA.CustId and BA.AddType = 'H' and BA.DateMoved is NULL -- Home address				
LEFT JOIN CustAddress SA on CA.CustId = SA.CustId and SA.AddType = LI.DeliveryAddress and SA.DateMoved is NULL -- Delivery address
LEFT JOIN Admin.[User] EMP on AG.EmpeeNoSale = EMP.Id
LEFT JOIN StockItem SI on OE.ItemNo = SI.ItemNo and OE.StockLocn = SI.StockLocn,
Country CTRY, CountryMaintenance CM

WHERE OE.RunNo = @runNo and OE.Type = 'D' and CM.CodeName = 'taxname' 
		and DEL.ItemNo != 'RB'	-- rebates are in receipts
		
-----------------------------------------------------------------------------------------------------		


UPDATE #TempOracleAR SET UnitPrice = 0 where UnitPrice is NULL
UPDATE #TempOracleAR SET LineValue = 0 where LineValue is NULL

UPDATE #TempOracleAR
SET TranClass = Case
					When LineValue < 0 and ItemType != 'DISCOUNT' Then 'Credit Memo'	-- not a discount
					When LineValue > 0 and ItemType = 'DISCOUNT' Then 'Debit Memo'	-- is a discount
					Else TranClass
				End 
WHERE TranClass = 'Invoice'

-- this is unnecessary Oracle can handle negative values for discounts
/*UPDATE #TempOracleAR
SET Quantity =	ABS(Quantity) * -1 
WHERE ( (LineValue < 0 and ItemType != 'DISCOUNT') 
		 or (LineValue > 0 and ItemType = 'DISCOUNT') )
*/
---------------------------------------------------------------------------
-- Correcting the UnitPrice for DTs and Discounts -------------------------
UPDATE #TempOracleAR
SET UnitPrice = ABS(UnitPrice)
WHERE ItemType = 'DT' 

-- this is unnecessary Oracle can handle negative values for discounts
/*UPDATE #TempOracleAR
SET UnitPrice = ABS(UnitPrice) * -1
WHERE ItemType = 'DISCOUNT'*/
---------------------------------------------------------------------------


---------------------------------------------------------------------------
-- Update InvoiceReference, InvoiceLineNo using a cursor ------------------
IF (@@error = 0 and @isRerun = 0) -- Only for initial run 
BEGIN

	DECLARE @PrevAcct varchar(12), @PrevAgrmtNo int, @InvLineNo int, @AcctNo char(12),
			@ItemNo varchar(8), @PrevItemNo varchar(8), @AgrmtNo int, @TransRefNo int,
			@StockLocn smallint, @ContractNo varchar(10), @BuffNo int, @TranClass varchar(12),
			@PrevTranClass varchar(12), @InvCount int, @InvoiceReference varchar(10),
			@PrevBranch CHAR(3), @BranchNo CHAR(3), @InvoiceNumber int, @LineValue money,
			@FirstLineValue money, 
			@NewReceiptNo int, @OldReceiptNo varchar(19), @ReceiptRunNo int, -- 70514 jec
			@OracleReceiptNo varchar(19), @ReceiptDate datetime	-- 70514 jec
			
	SET @PrevAcct = ''
	SET @PrevAgrmtNo = 0 
	SET @InvCount = 0
	SET @PrevBranch = '' 
	SET	@PrevTranClass = ''
	SET @PrevItemNo = ''
	SET @FirstLineValue = 0

	DECLARE Line CURSOR FOR
	SELECT DEL.AcctNo, DEL.AgrmtNo, DEL.ItemNo, DEL.StockLocn, DEL.ContractNo, DEL.BuffNo,
			AR.TranClass,DEL.TransRefNo, AR.LineValue, SUBSTRING(DEL.AcctNo,1,3) as BranchNo
	FROM Delivery DEL 
	INNER JOIN #TempOracleAR AR on DEL.AcctNo = AR.KAcctNo and DEL.AgrmtNo = AR.KAgrmtNo and 
				DEL.ItemNo = AR.KItemNo and DEL.StockLocn = AR.KStockLocn and 
				DEL.ContractNo = AR.KContractNo and DEL.TransRefNo = AR.KTransRefNo
	ORDER BY DEL.AcctNo, DEL.AgrmtNo, AR.TranClass desc, AR.LineValue desc -- so invoice before credit memo

	OPEN Line
	FETCH FROM Line INTO @AcctNo, @AgrmtNo, @ItemNo, @StockLocn, @ContractNo, @BuffNo, 
			@TranClass, @TransRefNo, @LineValue, @BranchNo

	WHILE (@@fetch_status = 0)
	BEGIN

		IF @PrevBranch != @BranchNo	-- Branch changes
		BEGIN
			-- update hi invoice no for prev branch
			Update Branch
			Set HiExtInvoiceNo = HiExtInvoiceNo + @InvCount 
			Where BranchNo = @PrevBranch
			-- get start invoice no for current branch
			Set @InvoiceNumber = (Select HiExtInvoiceNo From Branch Where BranchNo = @BranchNo)
			Set @InvCount = 0
			Set @PrevBranch = @BranchNo
		END		

		IF @PrevAcct != @AcctNo	-- Account changes
			or @PrevAgrmtNo != @AgrmtNo	-- Agreemtnt no changes
			or @PrevTranClass != @TranClass	-- Class changes
			or (@PrevItemNo != @ItemNo and @TranClass != 'Invoice')	-- item changes and class not invoice
		BEGIN
			Set @PrevAcct = @AcctNo
			Set @PrevAgrmtNo = @AgrmtNo
			Set @PrevTranClass = @TranClass
			Set @PrevItemNo = @ItemNo		
			Set @InvLineNo = 0				-- reset line no
			Set @InvCount = @InvCount+1
			Set @FirstLineValue = 0			-- 		
		END		
		
		Set @InvLineNo = @InvLineNo + 1
		IF @InvLineNo = 1
			Set @FirstLineValue = @LineValue
	
		Set @InvoiceReference = SUBSTRING(@AcctNo,1,3) + CAST(@InvoiceNumber + @InvCount as varchar(7))
		
		-- Update Account Receivable Temp --
		Update #TempOracleAR
		Set LineRef = @InvLineNo, InvoiceReference = @InvoiceReference
		From #TempOracleAR AR 
		INNER JOIN Branch B on AR.BranchNo = B.BranchNo	
		Where AR.KAcctNo = @AcctNo and AR.KAgrmtNo = @AgrmtNo and AR.KItemNo = @ItemNo and 
				AR.KStockLocn = @StockLocn and AR.KContractNo = @ContractNo and 
				AR.KTransRefNo = @TransRefNo and AR.TranClass = @TranClass --and AR.InvoiceReference = @InvoiceReference
	
		-- Update Delivery -- 
		Update Delivery
		Set InvoiceLineNo = @InvLineNo, ExtInvoice = @InvoiceReference
		Where AcctNo = @AcctNo and AgrmtNo = @AgrmtNo and ItemNo = @ItemNo and StockLocn = @StockLocn 
				and ContractNo = @ContractNo and TransRefNo = @TransRefNo and BuffNo = @BuffNo

		-- get next row
		FETCH NEXT FROM Line INTO @AcctNo, @AgrmtNo, @ItemNo, @StockLocn, @ContractNo, @BuffNo,
								@TranClass, @TransRefNo, @LineValue, @BranchNo
	END

	-- update hi invoice no for last branch --
	Update Branch
	Set HiExtInvoiceNo = HiExtInvoiceNo + @InvCount 
	Where BranchNo = @PrevBranch

	CLOSE Line
	DEALLOCATE Line
END


---------------------------------------------------------------------------
-- Update Credit Invoice Ref for Cancellation/Returns ---------------------
UPDATE #TempOracleAR 
SET CredInvRef = 
				Case 
					When DEL.ExtInvoice is NULL and DEL.BuffBranchNo != 0 
							Then CAST(DEL.BuffBranchNo as CHAR(3)) + CAST(DEL.BuffNo as varchar(7))
					When DEL.ExtInvoice is NULL and DEL.BuffBranchNo = 0 
							Then CAST(DEL.BuffNo as varchar(10))
					Else DEL.ExtInvoice
				End 
FROM Delivery DEL 
INNER JOIN #TempOracleAR AR on DEL.AcctNo = AR.AcctNo and DEL.AgrmtNo = AR.KAgrmtNo and DEL.ItemNo = AR.ItemNo 
				and DEL.StockLocn = AR.KStockLocn and DEL.ContractNo = AR.KContractNo and DEL.TransRefNo < AR.KTransRefNo 
				and DEL.DelOrColl = 'D' and AR.TranClass != 'Invoice'
---------------------------------------------------------------------------


---------------------------------------------------------------------------
-- Update TaxRate and Taxcode for Mauritius (as TaxRate in Cosacs is 0)----
DECLARE @CountryCode CHAR(1)
SET @CountryCode = (Select CountryCode From CountryMaintenance Where CodeName = 'countrycode')

IF @countrycode = 'M'
BEGIN
 
	UPDATE #TempOracleAR
	SET TaxRate = 15, TaxFlag = 'Y', Taxcode = 'VAT'
	
	-- Set specific categories to zero TaxRate
	UPDATE #TempOracleAR
	SET TaxRate = 0, TaxFlag = 'N'
	FROM #TempOracleAR AR 
	INNER JOIN StockItem SI on AR.KItemNo = SI.ItemNo and AR.KStockLocn = SI.StockLocn
	WHERE  SI.UnitPriceHP = 0 AND SI.Category not IN (14, 84, 36, 37, 38, 46, 47, 48, 86, 87, 88)

	-- Reduce unit price and Line value by calculated tax value
	UPDATE #TempOracleAR
	SET UnitPrice = ROUND(UnitPrice*100 /(100 + TaxRate),2), 
		LineValue = ROUND(LineValue*100 /(100 + TaxRate),2)
END
---------------------------------------------------------------------------

-- Final Select Statement -------------------------------------------------	
SELECT DISTINCT AcctNo, InvoiceReference FROM #TempOracleAR	 -- Header
SELECT * FROM #TempOracleAR
---------------------------------------------------------------------------

-----------------------------------------------------------------------------------------------------
        
SET @return = @@error
		
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO