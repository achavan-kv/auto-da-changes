IF EXISTS (SELECT * FROM sysobjects WHERE NAME = 'BrokerQueryPopulateServiceInsurance')
DROP PROCEDURE BrokerQueryPopulateServiceInsurance
GO
CREATE PROCEDURE BrokerQueryPopulateServiceInsurance @mindelrunno INT , @maxdelrunno INT,@runno INT
AS 
declare @status INT, @insuranceitemno VARCHAR(10),@adminitemno VARCHAR(10)
-- This procedure is used to populate the service insurance and tax details for the broker query. It repeats some logic contained in the BrokerExtractSP Procedure
-- but to get out details of accounts. 
SELECT @insuranceitemno= Value FROM CountryMaintenance WHERE NAME LIKE 'Insurance charge Item Number'
SELECT @adminitemno=Value FROM CountryMaintenance WHERE codename LIKE 'adminitemno'
IF @insuranceitemno = '' 
	SET @insuranceitemno = 'DT'
	
IF @adminitemno = ''	
   SET @adminitemno = 'DT'
DECLARE @countrycode CHAR(1)
SELECT @countrycode = countrycode FROM country
SET @status = 0
-- broker insurance hp bhi
BEGIN 
		insert into #totals  
		(interfaceaccount , 
		acctno , branchno, 
		transtypecode ,
		transvalue,datetrans,transrefno ,category, itemno )

		SELECT 
		    right(interfaceaccount,4),	
		    d.ACCTNO,	
            CONVERT(INT,LEFT(d.acctno,3)),
		    t.transtypecode,		
		    MIN(ISNULL(InsuranceWeight, insurance)),		
            MIN(d.datetrans),
            MIN(d.transrefno),
            0,  
            @insuranceitemno
		FROM transtype t, ##creditaccts c,delivery d
		WHERE t.transtypecode ='BHI'
		    AND d.acctno= c.acctno 
            AND d.runno BETWEEN @mindelrunno AND @maxdelrunno
		    AND (insurance <>0 or InsuranceWeight <> 0)
		    AND d.BrokerExRunNo = @runno
		GROUP BY right(interfaceaccount,4),	
		         d.ACCTNO,	
                 CONVERT(INT,LEFT(d.acctno,3)),
		         t.transtypecode
		IF @@ERROR = 0
		BEGIN
			PRINT 'done stage 1 -bhi'
		END
		ELSE
		BEGIN
			PRINT 'error at stage 1-bhi '
		END
			
--- now for the balancing service charge has been reduced above by removing admin and insurance
		insert into #totals  
		(interfaceaccount , 
		acctno , branchno, 
		transtypecode ,
		transvalue,datetrans,transrefno ,category, itemno )

		SELECT 
		right(interfacebalancing,4),		
		d.acctno,	CONVERT(INT,LEFT(d.acctno,3)),	-- Lets just do the branch for the account numbers for now???
		t.transtypecode,		
		MIN(-ISNULL(InsuranceWeight, insurance)),		MIN(d.datetrans),MIN(d.transrefno),0, @insuranceitemno
		FROM transtype t, ##creditaccts c,delivery d
		WHERE t.transtypecode ='BHI'
		AND d.acctno= c.acctno AND d.runno BETWEEN @mindelrunno AND @maxdelrunno
		AND (insurance <>0 or InsuranceWeight <> 0)
		AND d.BrokerExRunNo = @runno 
		GROUP BY d.acctno,d.itemno,CONVERT(INT,LEFT(d.acctno,3)),t.transtypecode,right(interfacebalancing,4)
		
		IF @@ERROR = 0
		BEGIN
			PRINT 'done stage 1 -bhi'
		END
		ELSE
		BEGIN
			PRINT 'error at stage 1-bhi '
		END
					
END 
-- broker admin hp bha
BEGIN 
		insert into #totals  
		(interfaceaccount , 
		acctno , branchno, 
		transtypecode ,
		transvalue,datetrans,transrefno ,category, itemno )

		SELECT 
		right(interfaceaccount,4),		
		d.acctno,	CONVERT(INT,LEFT(d.acctno,3)),	-- Lets just do the branch for the account numbers for now???
		t.transtypecode,
		MIN(administration),			MIN(d.datetrans),MIN(d.transrefno),0, @adminitemno
		FROM transtype t, ##creditaccts c, delivery d 
		WHERE t.transtypecode ='BHA' AND administration <>0
		AND d.acctno = c.acctno AND d.itemno =@adminitemno
		AND d.runno BETWEEN @mindelrunno AND @maxdelrunno 
		AND d.BrokerExRunNo = @runno 
		GROUP BY d.acctno,d.itemno,CONVERT(INT,LEFT(d.acctno,3)),t.transtypecode,right(interfaceaccount,4)
		IF @@ERROR = 0
		BEGIN
			PRINT 'done stage 1 -bha'
		END
		ELSE
		BEGIN
			PRINT 'error at stage 1-bha '
		END
			
--- now for the balancing service charge has been reduced above by removing admin and insurance
		insert into #totals  
		(interfaceaccount , 
		acctno , branchno, 
		transtypecode ,
		transvalue,
		datetrans,transrefno ,category, itemno )

		SELECT 
		right(interfacebalancing,4),		
		d.acctno,	CONVERT(INT,LEFT(d.acctno,3)),	t.transtypecode,
		MIN(-administration),			
		MIN(d.datetrans),MIN(d.transrefno),0, @adminitemno
		FROM transtype t, ##creditaccts,delivery d 
		WHERE t.transtypecode ='BHA' AND administration <>0
		AND d.acctno = ##creditaccts.acctno AND d.itemno =@adminitemno
		AND d.runno BETWEEN @mindelrunno AND @maxdelrunno 
		AND d.BrokerExRunNo = @runno 
		GROUP BY d.acctno,d.itemno,CONVERT(INT,LEFT(d.acctno,3)),t.transtypecode,right(interfacebalancing,4)
		IF @@ERROR = 0
		BEGIN
			PRINT 'done stage 2 -bha'
		END
		ELSE
		BEGIN
			PRINT 'error at stage 2-bha '
		END
END 	
-- broker service charge hp take out ins + admin charge bhs 
-- '
BEGIN 
		INSERT INTO #totals  
		(interfaceaccount , 
		acctno , branchno, 
		transtypecode ,
		transvalue,
		datetrans,transrefno ,category, itemno )

		SELECT 
		right(interfaceaccount,4),		
		d.acctno,	CONVERT(INT,LEFT(d.acctno,3)),	t.transtypecode,
		MIN(reducedservice),		
		MIN(d.datetrans),MIN(d.transrefno),0, 'DT'
		FROM transtype t, ##creditaccts c, delivery d 
		WHERE t.transtypecode ='BHS' AND d.itemno = 'dt'
		AND d.acctno= c.acctno AND d.runno BETWEEN @mindelrunno AND @maxdelrunno
		AND d.BrokerExRunNo = @runno 
		GROUP BY d.acctno,d.itemno,CONVERT(INT,LEFT(d.acctno,3)),t.transtypecode,right(interfaceaccount,4)
		IF @@ERROR = 0
		BEGIN
			PRINT 'done stage 1 -bhs'
		END
		ELSE
		BEGIN
			PRINT 'error at stage 1-bhs '
		END
			
--- now for the balancing service charge has been reduced above by removing admin and insurance
		INSERT INTO #totals  
		(interfaceaccount , 
		acctno , branchno, 
		transtypecode ,
		transvalue,
		datetrans,transrefno ,category, itemno )

		SELECT 
		right(interfacebalancing,4),		d.acctno,	CONVERT(INT,LEFT(d.acctno,3)),	t.transtypecode,
		MIN(-reducedservice),		-- doing min as totals are summed up.
		MIN(d.datetrans),MIN(d.transrefno),0, 'DT'
		FROM transtype t, ##creditaccts c, delivery d 
		WHERE t.transtypecode ='BHS' AND d.itemno = 'dt'
		AND d.acctno= c.acctno AND d.runno BETWEEN @mindelrunno AND @maxdelrunno
		AND d.BrokerExRunNo = @runno 
		GROUP BY d.acctno,d.itemno,CONVERT(INT,LEFT(d.acctno,3)),t.transtypecode,right(interfacebalancing,4)
		IF @@ERROR = 0
		BEGIN
			PRINT 'done stage 1 -bhs'
		END
		ELSE
		BEGIN
			PRINT 'error at stage 1-bhs '
		END
END 			

DECLARE  @agreementtaxtype CHAR(1)
SELECT @agreementtaxtype = agrmttaxtype FROM country 

-- broker tax
   IF @agreementtaxtype ='E'
   BEGIN
		insert into #totals  
		(interfaceaccount , acctno , branchno, transtypecode ,
		transvalue,datetrans,transrefno ,category, itemno )

		SELECT 
		right(interfaceaccount,4),	
		d.acctno ,	CONVERT(INT,LEFT(acctno,3)),	-- Lets just do the branch for the account numbers for now???
		t.transtypecode,		d.transvalue,d.datetrans,d.transrefno, s.category,d.itemno 
		FROM transtype t, delivery d 
		LEFT JOIN 
        (
            SELECT itemno,stocklocn,unitpricecash,unitpricehp,category,CASE WHEN agrmttaxtype = 'E' THEN 0 ELSE stockitem.taxrate END AS taxrate
            FROM stockitem, country
        ) s ON d.itemno = s.itemno AND d.stocklocn = s.stocklocn
		WHERE runno between @minDelRunno AND @maxDelRunno
		AND t.transtypecode ='BTX'  AND d.itemno ='STAX'
		AND d.BrokerExRunNo = @runno 
		IF @@ERROR = 0
		BEGIN
			PRINT 'done stage 1 -tax'
		END
		ELSE
		BEGIN
			PRINT 'error at stage 1-tax '
		END
			
		insert into #totals  
		(interfaceaccount , acctno , branchno, transtypecode ,
		transvalue,datetrans,transrefno ,category, itemno )

		SELECT 
		LEFT(t.interfacebalancing,4),
		d.acctno ,	CONVERT(INT,LEFT(acctno,3)),	-- Lets just do the branch for the account numbers for now???
		t.transtypecode,-d.transvalue,d.datetrans,d.transrefno, s.category,d.itemno 
		FROM transtype t, delivery d 
		LEFT JOIN 
        (
            SELECT itemno,stocklocn,unitpricecash,unitpricehp,category,CASE WHEN agrmttaxtype = 'E' THEN 0 ELSE stockitem.taxrate END AS taxrate
            FROM stockitem, country
        ) s ON d.itemno = s.itemno AND d.stocklocn = s.stocklocn
		WHERE runno between @minDelRunno AND @maxDelRunno
		AND t.transtypecode ='BTX'  AND d.itemno ='STAX'
		AND d.BrokerExRunNo = @runno 
		
		IF @@ERROR = 0
		BEGIN
			PRINT 'done stage 2 -tax'
		END
		ELSE
		BEGIN
			PRINT 'error at stage 2-tax '
		END
			
    END
    ELSE -- Need to extract tax from the items deliveryed 
    BEGIN
		
		insert into #totals  
		(interfaceaccount , acctno , branchno, transtypecode ,
		transvalue,
		datetrans,transrefno ,category, itemno )

		SELECT 
		
		right(interfaceaccount,4),	
		d.acctno ,	CONVERT(INT,LEFT(acctno,3)),	-- Lets just do the branch for the account numbers for now???
		t.transtypecode,ROUND(transvalue-(transvalue*100)/(100+ s.taxrate),2),
		d.datetrans,		d.transrefno, s.category,d.itemno 
		
		FROM transtype t, delivery d 
		LEFT JOIN 
        (
            SELECT itemno,stocklocn,unitpricecash,unitpricehp,category,CASE WHEN agrmttaxtype = 'E' THEN 0 ELSE stockitem.taxrate END AS taxrate
            FROM stockitem, country
        ) s ON d.itemno = s.itemno AND d.stocklocn = s.stocklocn
		WHERE runno between @minDelRunno AND @maxDelRunno
		AND t.transtypecode ='BTX'  
		AND d.BrokerExRunNo = @runno 
		
		IF @@ERROR = 0
		BEGIN
			PRINT 'done stage 1 -btx'
		END
		ELSE
		BEGIN
			PRINT 'error at stage 1-btx '
		END
		
		
		insert into #totals  
		(interfaceaccount , acctno , branchno, transtypecode ,
		transvalue,datetrans,transrefno ,category, itemno )

		SELECT 
		
		left(interfacebalancing,4),	
		d.acctno ,	CONVERT(INT,LEFT(acctno,3)),	-- Lets just do the branch for the account numbers for now???
		t.transtypecode,-(ROUND(transvalue-(transvalue*100)/(100+ s.taxrate),2)),
		d.datetrans,		d.transrefno, s.category,d.itemno 
		FROM transtype t, delivery d 
		LEFT JOIN 
        (
            SELECT itemno,stocklocn,unitpricecash,unitpricehp,category,CASE WHEN agrmttaxtype = 'E' THEN 0 ELSE stockitem.taxrate END AS taxrate
            FROM stockitem, country
        ) s ON d.itemno = s.itemno AND d.stocklocn = s.stocklocn
		WHERE runno between @minDelRunno AND @maxDelRunno
		AND t.transtypecode ='BTX' 
	    AND d.BrokerExRunNo = @runno 
		IF @@ERROR = 0
		BEGIN
			PRINT 'done stage 2 -btx'
		END
		ELSE
		BEGIN
			PRINT 'error at stage 2-btx '
		END
			
	END
GO