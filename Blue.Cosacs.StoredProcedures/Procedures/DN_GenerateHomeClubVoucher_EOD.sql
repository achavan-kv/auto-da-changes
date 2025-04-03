SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO

---------------------------------------------------------------------------------------------------

IF exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[DN_GenerateHomeClubVoucher_EOD]') and objectproperty(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[DN_GenerateHomeClubVoucher_EOD]
GO

------------------------------------------------------------------------------------
-- Author : NM
-- CR 1017 - Home Club
-- 21/08/2009
-- 
-- PERF OPT ==> Any statement added for performance optimization like reducing the number of rows
--				 retruned by the query etc will be followed by the comment (PERF OPT)
-- ---------------------------------------------------------------------------------

CREATE PROCEDURE [dbo].[DN_GenerateHomeClubVoucher_EOD] 	
	@return Int OUTPUT
AS


SET NOCOUNT ON; --PERF OPT
SET @return = 0


-----------------------------------------------------------------------------------
DECLARE @runNo Int, @runDateFrom DateTime , @runDateTo DateTime

SELECT @runNo = ISNULL(MAX(RunNo),0) + 1 FROM  InterfaceControl
WHERE  Interface = 'HomeClub' 

SELECT @runDateFrom = DATEDIFF(DD, 0, IsNull(Max(DateStart), '1900-01-01')) FROM InterfaceControl 
WHERE Interface = 'HomeClub' and Result = 'P'

SET @runDateTo = DATEDIFF(DD, 0, GETDATE()) -- DateDiff to strip off time value

INSERT INTO InterfaceControl (Interface, RunNo, DateStart, Result)
VALUES ('HomeClub', @runNo, GETDATE(), 'F')

IF @runDateFrom > @runDateTo
	SET @runDateFrom = @runDateTo
ELSE IF @runDateFrom < @runDateTo
	SET @runDateFrom = DATEADD(DD, 1, @runDateFrom)
-----------------------------------------------------------------------------------


-----------------------------------------------------------------------------------
DECLARE @expiredStatus SmallInt, @currentStatus SmallInt, @unpaidStatus SmallInt
DECLARE @activeStatus SmallInt, @suspendedStatus SmallInt, @reviewStatus SmallInt
DECLARE @cparamHSS SmallInt, @cparamHSO SmallInt, @cparamCS SmallInt, 
		@cparamVoucherPeriod Int, @cparamMonthsAfterDelivery SmallInt, @lastRunWeekNo SmallInt -- Country Parameters

SELECT @expiredStatus = CONVERT(SMALLINT,Reference) FROM Code WHERE Category = 'HCA' and CodeDescript = 'Expired'
SELECT @currentStatus = CONVERT(SMALLINT,Reference) FROM Code WHERE Category = 'HCA' and CodeDescript = 'Current'
SELECT @unpaidStatus = CONVERT(SMALLINT,Reference) FROM Code WHERE Category = 'HCA' and CodeDescript = 'Unpaid'

SELECT @activeStatus = CONVERT(SMALLINT,Reference) FROM Code WHERE Category = 'HCR' and CodeDescript = 'Active'
SELECT @suspendedStatus = CONVERT(SMALLINT,Reference) FROM Code WHERE Category = 'HCR' and CodeDescript = 'Suspended'
SELECT @reviewStatus = CONVERT(SMALLINT,Reference) FROM Code WHERE Category = 'HCR' and CodeDescript = 'Review'

SELECT @cparamHSS = Value FROM CountryMaintenance WHERE CodeName = 'LoyaltyHSS' 
SELECT @cparamHSO = Value FROM CountryMaintenance WHERE CodeName = 'LoyaltyHSO' 
SELECT @cparamCS = Value FROM CountryMaintenance WHERE CodeName = 'LoyaltyCS'
SELECT @cparamVoucherPeriod = Value FROM CountryMaintenance WHERE CodeName = 'LoyaltyVoucherPeriod'
SELECT @cparamMonthsAfterDelivery = Value FROM CountryMaintenance WHERE CodeName = 'LoyaltyMonthsAfterDelivery'
SELECT @lastRunWeekNo = Value FROM CountryMaintenance WHERE CodeName = 'lastchargesweekno'
-----------------------------------------------------------------------------------


-- Membership Status Change -------------------------------------------------------
UPDATE Loyalty 
SET StatusAcct = @expiredStatus 
WHERE StatusAcct = @currentStatus and DATEDIFF(DD, 0, EndDate) <= @runDateTo -- UAT 45
-----------------------------------------------------------------------------------


-----------------------------------------------------------------------------------
SELECT DISTINCT L.CustID
INTO #IneligibleCust
FROM Loyalty L
INNER JOIN CustAcct CA ON CA.CustID = L.CustID
			and (L.StatusVoucher = @activeStatus or L.StatusVoucher = @suspendedStatus) --PERF OPT
INNER JOIN Acct AC ON AC.AcctNo = CA.AcctNo and (AC.AcctType = 'R' or AC.AcctType = 'O') 
Where ( AC.CurrStatus = 'S' and ISNUMERIC(AC.HighstStatus) = 1 and  
		CONVERT(SmallInt,AC.HighstStatus) >= @cparamHSS  and 
		AC.HighstStatus != '9'	-- 9 for Staff
	  )
		or
	  ( AC.CurrStatus != 'S' and ISNUMERIC(AC.HighstStatus) = 1 and  
		CONVERT(SmallInt,AC.HighstStatus) >= @cparamHSO and
		AC.HighstStatus != '9'
	  )
		or
	  ( ISNUMERIC(AC.CurrStatus) = 1 and  
		CONVERT(SmallInt,AC.CurrStatus) >= @cparamCS and
		AC.CurrStatus != '9'
	  )
-----------------------------------------------------------------------------------


-- Voucher Status - From Active to Suspended --------------------------------------
UPDATE Loyalty
SET StatusVoucher = @suspendedStatus
WHERE StatusVoucher = @activeStatus and
	CustID IN (Select CustID From #IneligibleCust)		 
-----------------------------------------------------------------------------------


-- Voucher Status - From Suspended to Active --------------------------------------
UPDATE Loyalty
SET StatusVoucher = @activeStatus
WHERE StatusVoucher = @suspendedStatus and
	CustID NOT IN (Select CustID From #IneligibleCust)		 
-----------------------------------------------------------------------------------


-- Selecting Qualifying Customer Accounts -----------------------------------------
SELECT L.CustID, L.MemberNo, AC.AcctNo, IP.InstalNo as TermsLength, L.StartDate,
		CONVERT(SmallInt, 0) as DiscountTerm, -- Will be updated below  
		CONVERT(Money, 0) as ServiceChargeTotal, -- Will be updated below  
		CONVERT(Money, 0) as VoucherValue -- Will be updated below  
INTO #AccountsQualified
FROM Loyalty L
INNER JOIN CustAcct CA ON CA.CustID = L.CustID and L.StatusAcct != @unpaidStatus and 
					(L.StatusVoucher = @activeStatus or L.StatusVoucher = @reviewStatus)
INNER JOIN Acct AC ON AC.AcctNo = CA.AcctNo and (AC.AcctType = 'R' or AC.AcctType = 'O') and
					DATEDIFF(DD, 0, AC.DateAcctOpen) between DATEDIFF(DD, 0, L.StartDate) and DATEDIFF(DD, 0, L.EndDate)
INNER JOIN InstalPlan IP on IP.AcctNo = AC.AcctNo and
					DATEDIFF(DD, 0, DATEADD(MM,(@cparamMonthsAfterDelivery - 1),IP.DateFirst)) between @runDateFrom and @runDateTo -- 1 year from 75% delivery threshold date
INNER JOIN Agreement AG ON AG.AcctNo = AC.AcctNo AND AG.DateDel IS NOT NULL AND CONVERT(DATETIME,AG.DateDel) != '1900-01-01'
WHERE L.MemberType IN (SELECT Code FROM Code WHERE Category = 'HCM' AND Reference = '1') -- Free Membership
		OR NOT EXISTS (SELECT 1 From FinTrans Where AcctNo = L.LoyaltyAcct AND TransTypeCode = 'GRT' AND 
							DateTrans BETWEEN DATEADD(second, -2, L.EndDate) AND DATEADD(second, 2, L.EndDate))  
-----------------------------------------------------------------------------------				


-- Updating Discount Term, Service Charge Total------------------------------------
UPDATE #AccountsQualified
SET DiscountTerm = IsNull((Select MAX(SortOrder) From Code Where Category = 'HCV' and SortOrder <= AQ.TermsLength), NULL),
	ServiceChargeTotal = IsNull((Select SUM(DEL.TransValue) From Delivery DEL Where DEL.AcctNo = AQ.AcctNo and DEL.ItemNo = 'DT'), 0)
FROM #AccountsQualified AQ
-----------------------------------------------------------------------------------


-- Updating Voucher Value ---------------------------------------------------------
--
-- C.Reference ==> DiscountPercentage
--
UPDATE #AccountsQualified
SET VoucherValue = AQ.ServiceChargeTotal * CONVERT(Float, IsNull(C.Reference, 0)) / 100
FROM #AccountsQualified AQ
INNER JOIN Code C on  C.Category = 'HCV' and AQ.DiscountTerm = C.SortOrder
-----------------------------------------------------------------------------------


-- Inserting values into LoyaltyVoucher table -------------------------------------
-- VoucherDate & RunNo will be updated again after exporting the CSV file --

IF (SELECT countrycode FROM country) = 'Y' --Special rounding for Mal only. 
BEGIN
	INSERT INTO LoyaltyVoucher (MemberNo, CustID, AcctNoGen, VoucherValue, VoucherDate, RunNo)
	SELECT AQ.MemberNo, AQ.CustID, AQ.AcctNo, ROUND(AQ.VoucherValue,0), @runDateTo, 0 
	FROM #AccountsQualified AQ
	WHERE AQ.VoucherValue > 0 AND AQ.Startdate = (Select MAX(Startdate) From #AccountsQualified WHERE AcctNo = AQ.AcctNo) AND
	NOT EXISTS (Select MemberNo From LoyaltyVoucher LV_TEMP Where LV_TEMP.AcctNoGen = AQ.AcctNo)
END
ELSE
BEGIN
	INSERT INTO LoyaltyVoucher (MemberNo, CustID, AcctNoGen, VoucherValue, VoucherDate, RunNo)
	SELECT AQ.MemberNo, AQ.CustID, AQ.AcctNo, AQ.VoucherValue, @runDateTo, 0 
	FROM #AccountsQualified AQ
	WHERE AQ.VoucherValue > 0 AND AQ.Startdate = (Select MAX(Startdate) From #AccountsQualified WHERE AcctNo = AQ.AcctNo) AND
	NOT EXISTS (Select MemberNo From LoyaltyVoucher LV_TEMP Where LV_TEMP.AcctNoGen = AQ.AcctNo)
END
-----------------------------------------------------------------------------------


-- Populating HomeClubCSVExport temp table ----------------------------------------
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[HomeClubCSVExport]') AND type in (N'U'))
	DROP TABLE HomeClubCSVExport
 
-- Header Names --
SELECT 'BranchNo' AS BranchNo, 'BranchName' AS BranchName, 'MemberNo' AS MemberNo, 'MemberStatus' AS MemberStatus, 'CustID' AS CustID, 
		'Title' AS Title, 'CustomerName' AS CustomerName, 'CusAddr1' AS CusAddr1, 'CusAddr2' AS CusAddr2, 'CusAddr3' AS CusAddr3, 'CusPostCode' AS CusPostCode, 
		'HomeTelNo' AS HomeTelNo, 'WorkTelNo' AS WorkTelNo, 'AcctNo' AS AcctNo, 'TransactionValue' AS TransactionValue, 'VoucherRef' AS VoucherRef, 
		'VoucherValue' AS VoucherValue, 'DateIssued' AS DateIssued, 'DateValid' AS DateValid, 'ExecutionDate' AS ExecutionDate, 'WeekRange' AS WeekRange
INTO HomeClubCSVExport
UNION ALL 
SELECT	-- Actual Values
CONVERT(VARCHAR, B.BranchNo) AS BranchNo,
REPLACE(ISNULL(B.BranchName, ''), ',', ' ') AS BranchName,
LV.MemberNo AS MemberNo,
CD.CodeDescript AS MemberStatus,
REPLACE(CU.CustID, ',', ' ') as CustID,  
REPLACE(IsNull(CU.Title, ''), ',', ' ') AS Title,
REPLACE(IsNull(CU.FirstName, '') + ' ' + IsNull(CU.Name, '') , ',', ' ') as CustomerName,  
REPLACE(REPLACE(REPLACE(REPLACE(IsNull(ADR_H.CusAddr1, ''), ',', ' '), '\t', ' '), '\n', ' '), '\0', ' ')  AS CusAddr1, 
REPLACE(REPLACE(REPLACE(REPLACE(IsNull(ADR_H.CusAddr2, ''), ',', ' '), '\t', ' '), '\n', ' '), '\0', ' ')  AS CusAddr2, 
REPLACE(REPLACE(REPLACE(REPLACE(IsNull(ADR_H.CusAddr3, ''), ',', ' '), '\t', ' '), '\n', ' '), '\0', ' ')  AS CusAddr3, 
REPLACE(IsNull(ADR_H.CusPoCode, ''), ',', ' ') AS CusPostCode,
REPLACE(ISNULL(TEL_H.TelNo, ''), ',', ' ') AS HomeTelNo,
REPLACE(ISNULL(TEL_W.TelNo, ''), ',', ' ') AS WorkTelNo,
REPLACE(LV.AcctNoGen, ',', ' ') as AcctNo, 
REPLACE(CONVERT(VARCHAR, AG.AgrmtTotal), ',', ' ') as TransactionValue,  
RIGHT('00000000' + Convert(VARCHAR, LV.VoucherRef), 8) as VoucherRef,
REPLACE(CONVERT(VARCHAR, ROUND(LV.VoucherValue, 0)), ',', ' ') as VoucherValue, 
REPLACE(CONVERT(VARCHAR, @runDateTo,103), ',', ' ') as DateIssued,  
REPLACE(Convert(VARCHAR, DATEADD(day, @cparamVoucherPeriod, @runDateTo), 103),',', ' ') as DateValid,
REPLACE(CONVERT(VARCHAR, GETDATE(),103), ',', ' ') as ExecutionDate,
CONVERT(VARCHAR, @lastRunWeekNo) AS WeekRange
FROM LoyaltyVoucher LV 
INNER JOIN Agreement AG ON AG.AcctNo = LV.AcctNoGen AND AG.AgrmtNo = (Select Max(AgrmtNo) From Agreement Where AcctNo = LV.AcctNoGen)
INNER JOIN Loyalty L ON L.MemberNo = LV.MemberNo AND  L.CustID = LV.CustID
INNER JOIN CUSTOMER CU on CU.CustID = LV.CustID AND LV.RunNo = 0  
LEFT JOIN BRANCH B ON B.BranchNo = CONVERT(SMALLINT, SUBSTRING(LV.AcctNoGen, 1, 3))
INNER JOIN CODE CD ON CD.Category = 'HCA' AND CONVERT(INT,CD.Reference) = L.StatusAcct
LEFT JOIN CustAddress ADR_H on CU.CustID = ADR_H.CustID and ADR_H.AddType = 'H' and ADR_H.DateMoved is NULL and 
		ADR_H.DateIn in (Select Max(DateIn) From CustAddress Where CustID = CU.CustID and AddType = 'H' and DateMoved is NULL) -- To always make sure only one address is selected, though redundant  
LEFT JOIN CustTel TEL_H on CU.CustID = TEL_H.CustID and TEL_H.TelLocn = 'H' and TEL_H.DateDiscon is NULL and 
		TEL_H.DateTelAdd in (Select Max(DateTelAdd) From CustTel Where CustID = CU.CustID and TelLocn = 'H' and DateDiscon is NULL) -- To always make sure only one record is selected, though redundant  
LEFT JOIN CustTel TEL_W on CU.CustID = TEL_W.CustID and TEL_W.TelLocn = 'W' and TEL_W.DateDiscon is NULL and 
		TEL_W.DateTelAdd in (Select Max(DateTelAdd) From CustTel Where CustID = CU.CustID and TelLocn = 'W' and DateDiscon is NULL) -- To always make sure only one record is selected, though redundant  
-----------------------------------------------------------------------------------  
  
IF EXISTS (Select VoucherRef From HomeClubCSVExport Where VoucherRef != 'VoucherRef')
BEGIN
	-- Exporting as a CSV file --------------------------------------------------------
	DECLARE @path varchar(100), @fileName varchar(40), @bcpCommand  varchar(400), @returnCode Int

	SET @path = 'D:\HomeClub\Vouchers'
	-- filename format : voucher_yyyyddmm_hh-mm-ss.csv
	SET @fileName =  'voucher_' + Convert(Varchar(8),GETDATE(),112) + '_' + REPLACE(Convert(Varchar(8), GETDATE(), 108), ':', '-') + '.csv'

	SET @bcpCommand = 'mkdir "' + @path + '"' 
	EXEC master.dbo.xp_cmdshell @bcpCommand -- Creating folder

	SET @bcpCommand = 'BCP " select * from ' + DB_NAME() + '.dbo.HomeClubCSVExport "  queryout ' +  @path + '\' + @fileName + ' -c -t, -q -Usa -P'  
	EXEC @returnCode = master.dbo.xp_cmdshell @bcpCommand -- Exporting as CSV
	-----------------------------------------------------------------------------------

	-----------------------------------------------------------------------------------
	IF @returnCode = 0
	BEGIN
		UPDATE InterfaceControl
		SET DateFinish = GETDATE(), Result = 'P', [FileName] = @fileName
		WHERE Interface = 'HomeClub' and RunNo = @runNo
		
		UPDATE LoyaltyVoucher
		SET VoucherDate = @runDateTo, RunNo = @runNo
		WHERE RunNo = 0
	END
	ELSE
	BEGIN
		UPDATE InterfaceControl
		SET DateFinish = GETDATE(),Result = 'F'
		WHERE Interface = 'HomeClub' and RunNo = @runNo
	END
	-----------------------------------------------------------------------------------
END
ELSE
BEGIN
	-----------------------------------------------------------------------------------
	UPDATE InterfaceControl
	SET DateFinish = GETDATE(), Result = 'P'
	WHERE Interface = 'HomeClub' and RunNo = @runNo -- No records found to generate any vouchers
	-----------------------------------------------------------------------------------
END

SET @return = @@ERROR

RETURN @return

