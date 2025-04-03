
/****** Object:  StoredProcedure [dbo].[DN_SRReportServiceFailureSP]    Script Date: 10/25/2006 09:12:10 ******/
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID('[dbo].[DN_SRReportServiceFailureSP]') AND OBJECTPROPERTY(id,'IsProcedure') = 1)
DROP PROCEDURE [dbo].[DN_SRReportServiceFailureSP]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--exec DN_SRReportServiceFailureSP '2007-04-05', '2007-04-12','ALL',null, 0,0
-- =============================================
-- Author:		Peter Chong
-- Create date: 24-Oct-2006
-- Description:	Service failure report 
-- Modified by: J.Hemans
-- Modified date: 07/10/2008
-- Modified Description: CR 949/958 Item Description, Model No and Total % Failure added to report. Also ability to filter on product and specific dates.

-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 22/07/11  IP  RI - #4381 - RI System Integration
-- =============================================
CREATE PROCEDURE DN_SRReportServiceFailureSP  --'2007-01-01', '2008-01-01', 0
	@Mindate datetime,
	@MaxDate datetime,
	@fault CHAR(4),  --UAT 453
	--@product VARCHAR(8) = NULL,     -- CR 949/958
	@product VARCHAR(18) = NULL,	--IP - 22/07/11 - RI - #4381 -- CR 949/958
	@quarters BIT,                  -- CR 949/958
	@return int output	
AS
BEGIN
	SET NOCOUNT ON;
	
	DECLARE @Q1Date datetime,
			@Q2Date datetime,
			@Q3Date datetime,
			@Q4Date datetime,
			@MinYear		varchar(4),
			@MinYearPlus1	varchar(4),
	        @noOfDays       INT
	
	SET @MinYear		= cast(year(@Mindate) AS VARCHAR(4))
	SET @MinYearPlus1	= cast(year(@Mindate) + 1 AS VARCHAR(4))
	
	--TODO calc these dates based on the min and max date
	SELECT @Q1Date = case when DateDiff(q, '01-04-1900', @MinDate) % 4 IN (0,1) then @MinYear + '-04-01' else @MinYearPlus1 + '-04-01' end
	SELECT @Q2Date = case when DateDiff(q, '01-04-1900', @MinDate) % 4 IN (0,1,2) then @MinYear + '-07-01' else @MinYearPlus1 + '-07-01' end
	SELECT @Q3Date = case when DateDiff(q, '01-04-1900', @MinDate) % 4 IN (0,1,2,3) then @MinYear + '-10-01' else @MinYearPlus1 + '-10-01' end
	SELECT @Q4Date = case when DateDiff(q, '01-04-1900', @MinDate) % 4 = 0 then @MinYear + '-01-01' else @MinYearPlus1 + '-01-01' end
    	
	IF @Q2Date NOT BETWEEN @Mindate AND GetDate()
		SET @Q2Date = NULL
	IF @Q3Date NOT BETWEEN @Mindate AND GetDate()
		SET @Q3Date = NULL
	IF @Q4Date NOT BETWEEN @Mindate AND GetDate()
		SET @Q4Date = NULL
	
	IF @MaxDate > GetDate()
		SET @MaxDate = GetDate()
    
    --If specific dates have been selected as opposed to Quarters then need to calculate number of days between dates
	SET @noOfDays = DATEDIFF(DAY,@Mindate,@MaxDate)

	--71895 RM 07-01-10 need to use temp table instead of table variable
	--create table #delivery (ItemNo VARCHAR(8),ItemDescription VARCHAR(50),AcctNo CHAR(12),quantity float,datedel datetime,stocklocn smallint,agrmtno int)  
	create table #delivery (ItemNo VARCHAR(18),ItemDescription VARCHAR(50),AcctNo CHAR(12),quantity float,datedel datetime,stocklocn smallint,agrmtno int, itemID int)  --IP - 22/07/11 - RI - #4381
	
 
	IF @fault = 'ALL'
	BEGIN
		
	INSERT INTO #delivery
	--SELECT DISTINCT D.ItemNo,S.Description,D.AcctNo,	D.quantity, D.datedel, D.stocklocn, D.agrmtno
	SELECT DISTINCT SI.IUPC as ItemNo,S.Description,D.AcctNo,	D.quantity, D.datedel, D.stocklocn, D.agrmtno, D.ItemID --IP - 22/07/11 - RI - #4381
	--FROM Delivery D INNER JOIN SR_ServiceRequest S ON D.itemno = S.ProductCode
	FROM Delivery D INNER JOIN SR_ServiceRequest S ON D.ItemID = S.ItemID				--IP - 22/07/11 - RI - #4381
	LEFT JOIN StockInfo SI ON D.ItemID = SI.ID											--IP - 22/07/11 - RI - #4381
	WHERE  EXISTS(					--Only need to show items for which there is a service request
			SELECT * 
			FROM 
				SR_ServiceRequest S JOIN 
				SR_Resolution R ON S.servicerequestno = R.servicerequestno 
			WHERE
				Status = 'C' AND -- only pick up closed sr's 
				--ProductCode = D.ItemNo AND
				ItemID = D.ItemID AND													--IP - 22/07/11 - RI - #4381   
				ServiceType = 'C' AND  --Courts accounts only
				PurchaseDate < @MaxDate	AND 
				PurchaseDate  >= @MinDate AND
				R.Resolution NOT IN ('CAB', 'NFF') -- Exclude no fault found and abandoned calls
				) 
		AND D.DateDel < @MaxDate
		AND D.DateDel  >= @MinDate
		AND D.DelOrColl <> 'C'
		--AND (@product IS NULL OR D.itemno = @product)
		AND (@product IS NULL OR SI.IUPC = @product)									--IP - 22/07/11 - RI - #4381

	END
	ELSE
	BEGIN
	INSERT INTO #delivery
	--SELECT DISTINCT D.ItemNo,S.Description, D.AcctNo,	D.quantity, D.datedel, D.stocklocn, D.agrmtno
	SELECT DISTINCT SI.IUPC as ItemNo,S.Description, D.AcctNo,	D.quantity, D.datedel, D.stocklocn, D.agrmtno, D.ItemID			--IP - 22/07/11 - RI - #4381
	--FROM Delivery D INNER JOIN SR_ServiceRequest S ON D.itemno = S.ProductCode
	FROM Delivery D INNER JOIN SR_ServiceRequest S ON D.ItemID = S.ItemID				--IP - 22/07/11 - RI - #4381
	LEFT JOIN StockInfo SI ON D.ItemID = SI.ID											--IP - 22/07/11 - RI - #4381
	WHERE  EXISTS(					--Only need to show items for which there is a service request
			SELECT * 
			FROM 
				SR_ServiceRequest S JOIN 
				SR_Resolution R ON S.servicerequestno = R.servicerequestno 
			WHERE
				Status = 'C' AND -- only pick up closed sr's 
				--ProductCode = D.ItemNo AND 
				ItemID = D.ItemID AND													--IP - 22/07/11 - RI - #4381 
				ServiceType = 'C' AND  --Courts accounts only
				PurchaseDate < @MaxDate	AND 
				PurchaseDate  >= @MinDate AND
				R.Resolution NOT IN ('CAB', 'NFF') -- Exclude no fault found and abandoned calls
				AND R.Fault = @fault) 
		AND D.DateDel < @MaxDate
		AND D.DateDel  >= @MinDate
		AND D.DelOrColl <> 'C'
		--AND (@product IS NULL OR D.itemno = @product)
		AND (@product IS NULL OR SI.IUPC = @product)									--IP - 22/07/11 - RI - #4381 
	END
	

	UPDATE 	DT SET Quantity = Quantity - (
		SELECT isnull(Sum(QuantityBefore - QuantityAfter),0) 
		FROM LineItemAudit 
		WHERE Acctno = DT.Acctno AND 
		--ItemNo = DT.ItemNo	AND 
		ItemID = DT.ItemID	AND															--IP - 22/07/11 - RI - #4381 
		agrmtno = DT.agrmtno	AND
		stocklocn = DT.stocklocn AND
		source = 'GRTCancel')
	FROM #delivery DT

				
	SELECT 
		D.ItemNo [ProductCode]
		,D.ItemDescription [Product Description]
		
		, sum(d.quantity) [TotalSold]
		, dbo.fn_CalcTotalItemReturnPercentage(D.ItemNo, 365,  @Mindate, sum(case when DateDiff(day, @Mindate,D.DateDel ) < 366 then d.quantity else 0 end  ),@noOfDays,@quarters)	[Total % Failures] 
		, sum(case when DateDiff(q, '01-04-1900', D.DateDel ) % 4 = 1 then d.quantity else 0 end  ) [SalesQ1]
		--DateDiff(q,'01-04-1900',D.DateDel) gives the number of quarters since 01-04-1900. The mod of this will therefore give which quarter the DateDel was in.
			, dbo.fn_CalcItemReturnPercentage(D.ItemNo, 90,  @Q1Date, sum(case when DateDiff(q, '01-04-1900',D.DateDel ) % 4 = 1 then d.quantity else 0 end  ),@noOfDays,@quarters)	[90DaysQ1] 
			, dbo.fn_CalcItemReturnPercentage(D.ItemNo, 180, @Q1Date, sum(case when DateDiff(q, '01-04-1900',D.DateDel ) % 4 = 1 then d.quantity else 0 end  ),@noOfDays,@quarters)	[180DaysQ1] 
			, dbo.fn_CalcItemReturnPercentage(D.ItemNo, 270, @Q1Date, sum(case when DateDiff(q, '01-04-1900',D.DateDel ) % 4 = 1 then d.quantity else 0 end  ),@noOfDays,@quarters)	[270DaysQ1] 
			, dbo.fn_CalcItemReturnPercentage(D.ItemNo, 365, @Q1Date, sum(case when DateDiff(q, '01-04-1900',D.DateDel ) % 4 = 1 then d.quantity else 0 end  ),@noOfDays,@quarters)	[365DaysQ1] 
		, sum(case when DateDiff(q, '01-04-1900',D.DateDel ) % 4 = 2 then d.quantity else 0 end  ) [SalesQ2]
			, dbo.fn_CalcItemReturnPercentage(D.ItemNo, 90,  @Q2Date, sum(case when DateDiff(q, '01-04-1900',D.DateDel ) % 4 = 2 then d.quantity else 0 end  ),@noOfDays,@quarters)	[90DaysQ2] 
			, dbo.fn_CalcItemReturnPercentage(D.ItemNo, 180, @Q2Date, sum(case when DateDiff(q, '01-04-1900',D.DateDel ) % 4 = 2 then d.quantity else 0 end  ),@noOfDays,@quarters)	[180DaysQ2] 
			, dbo.fn_CalcItemReturnPercentage(D.ItemNo, 270, @Q2Date, sum(case when DateDiff(q, '01-04-1900',D.DateDel ) % 4 = 2 then d.quantity else 0 end  ),@noOfDays,@quarters)	[270DaysQ2] 
			, dbo.fn_CalcItemReturnPercentage(D.ItemNo, 365, @Q2Date, sum(case when DateDiff(q, '01-04-1900',D.DateDel ) % 4 = 2 then d.quantity else 0 end  ),@noOfDays,@quarters)	[365DaysQ2] 
		, sum(case when DateDiff(q, '01-04-1900',D.DateDel) % 4 = 3 then d.quantity else 0 end  ) [SalesQ3]
			, dbo.fn_CalcItemReturnPercentage(D.ItemNo, 90,  @Q3Date, sum(case when DateDiff(q, '01-04-1900',D.DateDel ) % 4 = 3 then d.quantity else 0 end  ),@noOfDays,@quarters)	[90DaysQ3] 
			, dbo.fn_CalcItemReturnPercentage(D.ItemNo, 180, @Q3Date, sum(case when DateDiff(q, '01-04-1900',D.DateDel ) % 4 = 3 then d.quantity else 0 end  ),@noOfDays,@quarters)	[180DaysQ3] 
			, dbo.fn_CalcItemReturnPercentage(D.ItemNo, 270, @Q3Date, sum(case when DateDiff(q, '01-04-1900',D.DateDel ) % 4 = 3 then d.quantity else 0 end  ),@noOfDays,@quarters)	[270DaysQ3] 
			, dbo.fn_CalcItemReturnPercentage(D.ItemNo, 365, @Q3Date, sum(case when DateDiff(q, '01-04-1900',D.DateDel ) % 4 = 3 then d.quantity else 0 end  ),@noOfDays,@quarters)	[365DaysQ3] 
		, sum(case when DateDiff(q, '01-04-1900',D.DateDel ) % 4 = 0 then d.quantity else 0 end  ) [SalesQ4]
			, dbo.fn_CalcItemReturnPercentage(D.ItemNo, 90,  @Q4Date, sum(case when DateDiff(q, '01-04-1900',D.DateDel ) % 4 = 0 then d.quantity else 0 end  ),@noOfDays,@quarters)	[90DaysQ4] 
			, dbo.fn_CalcItemReturnPercentage(D.ItemNo, 180, @Q4Date, sum(case when DateDiff(q, '01-04-1900',D.DateDel ) % 4 = 0 then d.quantity else 0 end  ),@noOfDays,@quarters)	[180DaysQ4] 
			, dbo.fn_CalcItemReturnPercentage(D.ItemNo, 270, @Q4Date, sum(case when DateDiff(q, '01-04-1900',D.DateDel ) % 4 = 0 then d.quantity else 0 end  ),@noOfDays,@quarters)	[270DaysQ4] 
			, dbo.fn_CalcItemReturnPercentage(D.ItemNo, 365, @Q4Date, sum(case when DateDiff(q, '01-04-1900',D.DateDel ) % 4 = 0 then d.quantity else 0 end  ),@noOfDays,@quarters)	[365DaysQ4] 
	FROM #delivery D  
			
	WHERE EXISTS(					--Only need to show items for which there is a service request
			SELECT * 
			FROM SR_ServiceRequest 
			WHERE 
				--ProductCode = D.ItemNo AND 
				ItemID = D.ItemID AND														--IP - 22/07/11 - RI - #4381
				Description = D.ItemDescription AND   
				ServiceType = 'C' AND  --Courts accounts only
				PurchaseDate < @MaxDate	AND 
				PurchaseDate  >= @MinDate) 
		AND D.DateDel < @MaxDate
		AND D.DateDel  >= @MinDate
		--AND D.DelOrColl <> 'C' --exclude delivery records where the item has been returned
--		--and D.Itemno = '101027'
	GROUP BY D.ItemNo,D.ItemDescription
	ORDER BY D.ItemNo



	--DROP TABLE #Delivery
	SET @return = @@error
END

