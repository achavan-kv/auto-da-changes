SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID('[dbo].[DN_SUCBGetFinancialTotalsSP]') AND OBJECTPROPERTY(id, 'IsProcedure') = 1)
DROP PROCEDURE [dbo].[DN_SUCBGetFinancialTotalsSP]
GO

CREATE PROCEDURE dbo.DN_SUCBGetFinancialTotalsSP
			@runno INT,
			@return INT OUTPUT

AS
   SET NOCOUNT ON 
	SET 	@return = 0

	DECLARE @RunDate DATETIME,
			@MinDate DATETIME,
			@MaxDate DATETIME
	
	SET @RunDate = (SELECT datestart FROM interfacecontrol WHERE runno = @runno
					 AND interface = 'UPDSMRY')
	SET @MinDate = (SELECT MIN(datechange) FROM transtypeaudit)
	SET @MaxDate = (SELECT MAX(datechange) FROM transtypeaudit)

	IF(@RunDate < @MinDate)
	BEGIN
		print '@RunDate < @MinDate'
		SELECT CONVERT(VARCHAR(5),f.branchno) AS Branchno, 
			   f.transtypecode AS Transtypecode,
			   SUM(f.transvalue) AS transvalue,
               CONVERT(VARCHAR(12),'') AS [type]
		INTO #itotals       
		FROM interface_financial f
        LEFT OUTER JOIN transtypeaudit ta
        ON f.transtypecode = ta.transtypecode
		    AND RIGHT(f.interfaceaccount,4) = ta.interfaceaccountold
            AND ta.datechange=(SELECT MIN(datechange) 
                               FROM transtypeaudit t2 
                               WHERE t2.datechange > @RunDate 
                                  AND t2.transtypecode = ta.transtypecode)
        LEFT OUTER JOIN transtype t 
        ON f.transtypecode = t.transtypecode
            AND RIGHT(f.Interfaceaccount, 4) = t.interfaceaccount
		WHERE runno = @runno
		    AND COALESCE(ta.isdepositold, t.isdeposit, 1) = 0
		GROUP BY f.branchno, 
                 f.transtypecode
	  UNION 
		SELECT CONVERT(VARCHAR(5),f.branchno) AS Branchno, 
			   f.transtypecode,
			   SUM(f.transvalue) AS transvalue,
               ''
		FROM interface_financial f
        LEFT OUTER JOIN transtypeaudit ta
        ON f.transtypecode = ta.transtypecode
		    AND RIGHT(f.interfaceaccount,4) = ta.interfacesecaccountold
            AND ta.datechange = (SELECT MIN(datechange) 
                                 FROM transtypeaudit t2 
                                 WHERE t2.datechange > @RunDate 
                                    AND t2.transtypecode = ta.transtypecode)
        LEFT OUTER JOIN transtype t
        ON f.transtypecode = t.transtypecode
            AND RIGHT(f.Interfaceaccount, 4) = t.interfacesecaccount
		WHERE runno = @runno
		    AND COALESCE(ta.isdepositold, t.isdeposit, 1) = 0
		GROUP BY f.branchno, 
                 f.transtypecode
		ORDER BY  branchno, 
                  transtypecode

		UPDATE #itotals SET [type] = 'ADJUSTMENT' -- default
		UPDATE #itotals SET [type] = 'RECEIPT' WHERE transtypecode IN ('COR','PA1','PA2','PA4','PAY','REF','DDE','DDN','DDR','DPY','RET') 
		UPDATE #itotals SET [type] = 'DELIVERY' WHERE transtypecode IN ('ADD','GRT','DEL','REB','RPO','REP') 
        UPDATE #itotals SET [type] = 'DELIVERY BX' WHERE EXISTS (SELECT * FROM brokeroraclesetup X WHERE X.code = #itotals.transtypecode)

		SELECT branchno, 
               transtypecode, 
               transvalue, 
               [type] AS [Description]
		FROM #itotals
		ORDER BY [type] , 
                 branchno,
                 transtypecode		

	END

    ELSE IF(@RunDate BETWEEN @MinDate AND @MaxDate)
	BEGIN
		PRINT '@RunDate between @MinDate and @MaxDate'
		SELECT CONVERT(VARCHAR(5),f.branchno) AS Branchno, 
			   f.transtypecode AS Transtypecode,
			   SUM(f.transvalue) AS transvalue,
               CONVERT(VARCHAR(12),'') AS [Type]
	    INTO #itotals2       
		FROM interface_financial f
        LEFT OUTER JOIN transtypeaudit ta
        ON f.transtypecode = ta.transtypecode
		    AND RIGHT(f.interfaceaccount,4) = ta.interfaceaccountnew
            AND ta.datechange = (SELECT MAX(datechange) 
                                 FROM transtypeaudit t2 
                                 WHERE t2.datechange < @RunDate 
                                     AND t2.transtypecode = ta.transtypecode)
        LEFT OUTER JOIN transtype t
        ON f.transtypecode = t.transtypecode
            AND RIGHT(f.Interfaceaccount, 4) = t.interfaceaccount
		WHERE runno = @runno
		    AND COALESCE(ta.isdepositnew, t.isdeposit, 1) = 0
		GROUP BY f.branchno, 
                 f.transtypecode
		HAVING MAX(ta.datechange) < @RunDate 
	  UNION 
		SELECT CONVERT(VARCHAR(5),f.branchno) AS Branchno, 
			   f.transtypecode,
			   SUM(f.transvalue) AS transvalue,
               ''
		FROM interface_financial f
        LEFT OUTER JOIN transtypeaudit ta
        ON f.transtypecode = ta.transtypecode
		    AND RIGHT(f.interfaceaccount,4) = ta.interfacesecaccountnew
            AND ta.datechange = (SELECT MAX(datechange) 
                                 FROM transtypeaudit t2 
                                 WHERE t2.datechange < @RunDate 
                                     AND t2.transtypecode = ta.transtypecode) 
        LEFT OUTER JOIN transtype t
        ON f.transtypecode = t.transtypecode
            AND RIGHT(f.Interfaceaccount, 4) = t.interfacesecaccount
		WHERE runno = @runno
		    AND COALESCE(ta.isdepositnew, t.isdeposit, 1) = 0
		GROUP BY f.branchno, 
                 f.transtypecode
		ORDER BY branchno, 
                 transtypecode

		UPDATE #itotals2 SET [Type] = 'ADJUSTMENT' -- default
		UPDATE #itotals2 SET [Type] = 'RECEIPT' WHERE transtypecode IN ('COR','PA1','PA2','PA4','PAY','REF','DDE','DDN','DDR','DPY','RET') 
		UPDATE #itotals2 SET [Type] = 'DELIVERY' WHERE transtypecode IN ('ADD','GRT','DEL','REB','RPO','REP') 
		UPDATE #itotals2 SET [Type] = 'DELIVERY BX' WHERE EXISTS (SELECT * FROM brokeroraclesetup X WHERE X.code = #itotals2.transtypecode)

		SELECT branchno,
               transtypecode,
               transvalue, 
               [Type] AS Description
		FROM #itotals2
		ORDER BY [Type],
                 branchno,
                 transtypecode
	END
	ELSE
	BEGIN
		PRINT 'current values'
		SELECT CONVERT(VARCHAR(5),f.branchno) AS Branchno, 
			   f.transtypecode AS Transtypecode,
			   SUM(f.transvalue) AS transvalue,
          CONVERT(VARCHAR(12),'') AS [type]
		INTO #itotals3       
		FROM interface_financial f
        LEFT OUTER JOIN transtype t
        ON f.transtypecode = t.transtypecode
		    AND RIGHT(f.interfaceaccount,4) = t.interfaceaccount
		WHERE runno = @runno
		    AND ISNULL(isdeposit, 1) = 0
		GROUP BY f.branchno, 
                 f.transtypecode
	  UNION 
		SELECT CONVERT(VARCHAR(5),f.branchno) AS Branchno, 
			   f.transtypecode,
			   SUM(f.transvalue) AS transvalue,
               ''
		FROM interface_financial f
        LEFT OUTER JOIN transtype t
        ON f.transtypecode = t.transtypecode
		    AND RIGHT(f.interfaceaccount,4) =t.interfacesecaccount
		WHERE runno = @runno
		    AND ISNULL(isdeposit, 1) = 0
		GROUP BY f.branchno, 
                 f.transtypecode
		ORDER BY branchno, 
                 transtypecode

		UPDATE #itotals3 SET [type] = 'ADJUSTMENT' -- default
		UPDATE #itotals3 SET [type] = 'RECEIPT' WHERE transtypecode IN ('COR','PA1','PA2','PA4','PAY','REF','DDE','DDN','DDR','DPY','RET') 
		UPDATE #itotals3 SET [type] = 'DELIVERY' WHERE transtypecode IN ('ADD','GRT','DEL','REB','RPO','REP') 
		UPDATE #itotals3 SET [type] = 'DELIVERY BX' WHERE EXISTS (SELECT * FROM brokeroraclesetup X WHERE X.code = #itotals3.transtypecode)
        
		SELECT branchno,
               transtypecode,
               transvalue, 
               [type] AS [Description]
		FROM #itotals3
		ORDER BY [type],
                 branchno,
                 transtypecode
	END
  
	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO
