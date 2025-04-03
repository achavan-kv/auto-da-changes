GO
IF EXISTS (	SELECT	1 
			FROM	sys.objects 
			WHERE	object_id = OBJECT_ID(N'[dbo].[CalculateFirstInstalmentDate]') 
					AND type in (N'P', N'PC'))
BEGIN
	DROP PROCEDURE [dbo].[CalculateFirstInstalmentDate]
END


GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[CalculateFirstInstalmentDate]		
@AcctNo VARCHAR(20),
@DateFirst DATE OUTPUT 
-- ================================================
-- Project      : CoSACS .NET
-- File Name    : CalculateFirstInstalmentDate
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : CalculateFirstInstalmentDate
-- Author       : ??
-- Date         : 10/7/2020
-- Version		: 004
-- Created for CR10.7
-- This procedure calculates the first instalment Date
-- 
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 15/07/2020 Rahul Sonawane 10.7 Feature CR - Calculate the First Instalment Paymnet date
-- 15/12/2020 Rahul Sonawane Changed the logic for calculate first payment date.
-- 13/01/2021 Rahul Sonawane Changed the logic for calculate firstpayment date for minimum due day after delivery
-- 29/01/2021 Rahul Sonawane Changed the logic for calculate firstpayment difference between Delivery date and customer Preferred day is equal to the set parameter
-- ================================================					
AS
	DECLARE @DayAfterBooking INT;
	DECLARE @DelayDelivery INT;
	DECLARE @DiffDelPay INT;
	DECLARE @MinDueDay INT;
	DECLARE @DateFirstInstalment DATE;
	DECLARE @Code INT = 0 

BEGIN

	SET NOCOUNT ON;

	SELECT @Code = code FROM code WHERE codedescript = 'Instalments' AND category = 'CMC'

	SET @DayAfterBooking=(SELECT CAST(ISNULL(Value,0) AS INT) FROM CountryMaintenance
								WHERE CodeName='NoOfDaysAfterBookFirstPayDate' AND ParameterCategory=@Code);

	SET @DelayDelivery=(SELECT CAST(ISNULL(Value,0) AS INT) FROM CountryMaintenance
								WHERE CodeName='MaxDaysDelayDelAfterBook' AND ParameterCategory=@Code);

	SET @DiffDelPay=(SELECT CAST(ISNULL(Value,0) AS INT) FROM CountryMaintenance
								WHERE CodeName='MinDiffBtwDelDateAndPayDate' AND ParameterCategory=@Code);

	SET @MinDueDay=(SELECT CAST(ISNULL(Value,0) AS INT) FROM CountryMaintenance
								WHERE CodeName='minperiod' AND ParameterCategory=@Code);

	SELECT @DateFirst=(
		CASE 

		--When account save first time set the first instalment date as (booking date + NoOfDaysAfterBookFirstPayDate) before delivery
			WHEN agr.datedel='1900-01-01 00:00:00.000'
			THEN DATEADD(Day, @DayAfterBooking, ac.dateacctopen)
		----------------------------------------------END--------------------------------------------------------------

		--When account save first time set the first instalment date as (booking date + Minimum due day) AFTER delivery
			WHEN (ip.prefInstalmentDay-DAY(agr.datedel))>@DiffDelPay AND ip.prefInstalmentDay>0
			THEN DATEADD(Day, ip.prefInstalmentDay, DATEADD(month, DATEDIFF(month, 0, ac.dateacctopen), -1))
			WHEN (ip.prefInstalmentDay-DAY(agr.datedel))<=@DiffDelPay AND ip.prefInstalmentDay>0
			THEN
				CASE WHEN(DAY(agr.datedel)<ip.prefInstalmentDay)
				THEN DATEADD(MONTH, 1,DATEADD(Day, ip.prefInstalmentDay, DATEADD(month, DATEDIFF(month, 0, agr.datedel), -1)))
				ELSE
					DATEADD(MONTH, 1,DATEADD(Day, ip.prefInstalmentDay, DATEADD(month, DATEDIFF(month, 0, agr.datedel)+1, -1)))
				END
		----------------------------------------------END--------------------------------------------------------------	
		
		--When delivery delay after "MaxDaysDelayDelAfterBook" then (delivery date + Minimum due day) AFTER delivery
			WHEN DATEDIFF(day,ac.dateacctopen,agr.datedel)>@DelayDelivery AND ip.prefInstalmentDay<=0
			THEN DATEADD(Day, @MinDueDay, agr.datedel)
			ELSE
				DATEADD(Day, @MinDueDay, agr.datedel)
		----------------------------------------------END-------------------------------------------------------------			 
		END )
	FROM instalplan ip 
	INNER JOIN acct ac ON ip.acctno=ac.acctno
	INNER JOIN agreement agr ON ip.acctno=agr.acctno
	WHERE ip.acctno=@AcctNo
END
