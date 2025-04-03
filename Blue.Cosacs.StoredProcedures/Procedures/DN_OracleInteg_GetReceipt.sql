SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_OracleInteg_GetReceipt]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_OracleInteg_GetReceipt]
GO

--Author : Mohamed Nasmi
CREATE PROCEDURE [dbo].[DN_OracleInteg_GetReceipt] 
	@runNo int,
	@return int output
		
AS
	SET @return = 0		-- initialise return code

---------------------------------------------------------------------------------------------
SELECT 
-- To make ReceiptNo = BranchNo(3 Digits) + TransRefNo(7 digits left padded with 0s) --  
Case
	When LEN(CAST(ABS(F.TransRefNo) as varchar(10))) <= 7 Then
		LEFT(CAST(F.BranchNo as varchar(5)), 3) +  RIGHT('0000000' + CAST(ABS(F.TransRefNo) as varchar(10)), 7)
	Else  
		LEFT(CAST(F.BranchNo as varchar(5)), 3) + CAST(ABS(F.TransRefNo) as varchar(10))
End as ReceiptNo,

F.DateTrans as ReceiptDate, 'R' as CurrencyCode, UPPER(CA.CustId) as CustId,
F.AcctNo, ROUND(F.TransValue,2) as ReceiptAmount,
CAST(0 as varchar(10)) as InvoiceReference, F.TransValue as AppliedAmount,
F.EmpeeNo as CosacsUser, ISNULL(F.Paymethod,0) as Paymethod, 
F.TransTypeCode as TranType, F.ChequeNo as Chq_CredCard, B.BankName, F.RunNo, F.OracleReceiptNo

INTO #TempOracleReceipt
FROM FinTransOracleExport F
LEFT JOIN CustAcct CA on F.AcctNo = CA.AcctNo and HldorJnt = 'H'
LEFT JOIN Bank B on F.BankCode = B.BankCode
WHERE  F.RunNo = @runNo
---------------------------------------------------------------------------------------------	


---------------------------------------------------------------------------------------------
IF (@@error = 0)
BEGIN
	-- Get max invoice ref (ExtInvoice or buffbranch + buffno)
	SELECT DISTINCT 
	DEL.AcctNo,
	MAX(
		Case 
			When DEL.ExtInvoice is NULL Then CAST(DEL.BuffBranchNo as CHAR(3)) + CAST(DEL.BuffNo as varchar(7)) 
			Else DEL.ExtInvoice
		End
		) as InvoiceReference
	INTO #Temp
	FROM #TempOracleReceipt R 
	LEFT JOIN Delivery DEL on R.AcctNo = DEL.AcctNo
	Group By DEL.AcctNo
END

IF (@@error = 0)
BEGIN
	UPDATE #TempOracleReceipt
	SET InvoiceReference = ISNULL(TEMP.InvoiceReference, 0)
	FROM #Temp TEMP 
	INNER JOIN #TempOracleReceipt R ON TEMP.AcctNo = R.AcctNo
END
---------------------------------------------------------------------------------------------


---------------------------------------------------------------------------------------------	
-- Duplicate Receipt Nos (70425) - (Caused by error in fix script. Should not occur normally)
IF (@@error = 0)
BEGIN
	SELECT ReceiptNo, COUNT(*) as ReceiptsCount 
	INTO #DuplicateReceipts 
	FROM #TempOracleReceipt
	Group By ReceiptNo
	Having COUNT(*) > 1	
	
	UPDATE #TempOracleReceipt
	SET ReceiptNo = R.ReceiptNo + 'A'
	FROM #TempOracleReceipt R 
	INNER JOIN #DuplicateReceipts DUP ON R.ReceiptNo = DUP.ReceiptNo
	WHERE TranType = 'XFR' and ReceiptAmount < 0
END
---------------------------------------------------------------------------------------------


-- Final Select Statement -------------------------------------------------------------------		
SELECT * FROM #TempOracleReceipt
---------------------------------------------------------------------------------------------
        
SET @return = @@error
	
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO