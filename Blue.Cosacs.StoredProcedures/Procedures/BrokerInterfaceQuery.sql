-- Question we are going to get the totals by branch - do we then want the totals within each branch... 
-- we should really restrict by branch -- if you can do so then you should 
--SELECT * FROM interface_financial order by runno desc
IF EXISTS (SELECT * FROM sysobjects WHERE NAME = 'BrokerInterfaceQuery')
DROP PROCEDURE BrokerInterfaceQuery
GO 
CREATE PROCEDURE  BrokerInterfaceQuery @type CHAR(1) -- 'I' for insert, 'D' for select (details), 'T' for totals
,@code VARCHAR(6)  -- Broker interface code... 
, @runno INT   -- Summary interface runno stored in interfacecontrol table as BrokerX
,@balance CHAR(1), -- 'Y' for balancing account  -- 'N ' for normal
@branchno SMALLINT -- branch for the xxxx
AS  
/* BrokerInterfaceQuery is a procedure which allows you to either save or retrieve data for a particular Broker interface and code... 
*/
SET NOCOUNT ON 
DECLARE @accounttypes VARCHAR(3),@codecatCategory1 VARCHAR(6),
@codecatCategory1Not CHAR(3),@excludeRepos CHAR(1),
@codecatCategory2 VARCHAR(6),@codecatCategory2Not CHAR(3),
@excludeInsAdmin CHAR(1),@itemno VARCHAR(12),@statement SQLText,
@countrycode CHAR(1), @finintstart INT , @fintintend INT ,
@delintstart INT , @delintend INT , @catBalanceAc BIT , @vbranch VARCHAR(6), @brokerExRunno INT


SELECT --@finintstart = finintstart,
@fintintend = finintend,
@delintend= DelIntEnd,
@delintstart = DelIntStart,
@brokerExRunno = BrokerRunno
 FROM brokerExtracthist 
 
WHERE finintend = @runno
SELECT @countrycode = countrycode FROM country 

SELECT 
	@accounttypes = accounttypes,
	@codecatCategory1 = codecatCategory1,
	@codecatCategory1Not = codecatCategory1Not,
	@excludeRepos = excludeRepos,
	@codecatCategory2 = codecatCategory2,
	@codecatCategory2Not = codecatCategory2Not,
	@excludeInsAdmin = excludeInsAdmin,
	@itemno = itemno,
	@catBalanceAc = CatBalanceAc 
FROM BrokerOracleSetup
WHERE code = @code

--SELECT * FROM BrokerOracleSetup
IF @codecatCategory1Not = 'N'
	SET @codecatCategory1Not = 'Not'
IF @codecatCategory2Not  = 'N'
	SET @codecatCategory2Not   = 'Not'
	
SET @statement = '' 
--SELECT '@' + column_name + ' = ' + column_name + ',' FROM INFORMATION_SCHEMA.columns WHERE table_name ='BrokerOracleSetup' 
IF @type = 'I' -- insert
  SET @statement =  'insert into #BrokerData ' 

IF @type = 'D'
  SET @statement =  'insert into #totals  (interfaceaccount , acctno , branchno, transtypecode ,
	transvalue,datetrans,transrefno ,category, itemno )' 

SET @statement = @statement + ' SELECT ' 
  
IF @type ='I'  
  SET @statement = @statement	+ 
		 '''' + CONVERT(VARCHAR,@countryCode) + '''' +  ',' 
		+ CONVERT(VARCHAR,@runno) + ','
		
IF @balance !='Y' OR @catBalanceAc =0
  SET @statement = @statement 
		+ ' right(interfaceaccount,4) AS InterfaceAccount '  + ',' 
ELSE -- balancing and uses category....
	SET @statement = @statement 
		+ ' LEFT(t.interfacebalancing,2) + CONVERT(VARCHAR,S.category) AS InterfaceAccount' + ','
		
  -- issue is can I do this??? can you do a select???		
  IF @TYPE  in ('I' ,'T')
	SET @statement = @statement + ' CONVERT(INT,LEFT(acctno,3)) AS Branchno , ' 
  ELSE -- select
	SET @statement = @statement + ' acctno, CONVERT(INT,LEFT(acctno,3)) AS Branchno , ' 
	
--  IF @TYPE in ('I' ,'T',)
	SET @statement = @statement + '	t.transtypecode,'
	DECLARE @minus CHAR(1)
	
  IF @balance = 'Y'	
	SET @minus= '-'
  ELSE 
	SET @minus =''	
  IF @type in ('I' ,'T')
	SET @statement = @statement + 
		' SUM(ROUND((' + @minus + 'transvalue*100)/(100+ s.taxrate),2)) as transvalue, '
   ELSE -- getting items out rather than totals...
	SET @statement = @statement	+ 	
		' ROUND(('+ @minus + 'transvalue*100)/(100+ s.taxrate),2) as transvalue, d.datetrans, d.transrefno, '

  IF @type ='I' 
    	SET @statement = @statement + 
	'''' + '''' + 	',' 
	
  BEGIN
  	SET @statement = @statement + 
	' s.category, d.itemno ' +
	' FROM transtype t, delivery d ' +
	 ' LEFT JOIN
     (
        SELECT itemno,stocklocn,unitpricecash,unitpricehp,category,CASE WHEN agrmttaxtype = ''E'' THEN 0 ELSE stockitem.taxrate END AS taxrate
        FROM stockitem, country
     ) s ON d.itemno = s.itemno AND d.stocklocn = s.stocklocn ' +
	' WHERE runno = ' + CONVERT(VARCHAR,@delintstart)  + ' AND BrokerExRunno = ' + CONVERT(VARCHAR, @BrokerExRunno)
	+ ' AND t.transtypecode =' + '''' + @code  + ''''
  END	

  IF @accounttypes ='H'
 	SET @statement = @statement + ' AND D.acctno LIKE ' + '''' + CONVERT(VARCHAR,@branchno) + '0%'' ' 
  ELSE if @accounttypes ='C'
   	SET @statement = @statement + ' AND D.acctno NOT LIKE ''___0%'' ' + ' AND D.acctno LIKE ' + '''' + CONVERT(VARCHAR,@branchno) + '%' + '''' 
 	
  IF @codecatCategory1 !=	''
  BEGIN
   SET @statement  = @statement 	
  	+ ' AND category ' + @codecatCategory1Not +
	' IN  (SELECT reference FROM code c WHERE c.category = ' + '''' + @codecatCategory1 + '''' + ')'
  END	
		
  IF @codecatCategory2 !=''	
  BEGIN
   SET @statement  = @statement 	
  	+ ' AND category ' + @codecatCategory2Not +
	' IN  (SELECT reference FROM code c WHERE c.category = ' + '''' + @codecatCategory2 + ''''  + ')'
  END
	
	--	'(39,98,90,12,82,36,37,38,46,47,48,49,86,87,88,96) '
	IF @excluderepos ='Y'
	BEGIN
		SET @statement = @statement + 
			' AND d.delorcoll !=''R''  ' 	
	END
	DECLARE @adminitemno VARCHAR(10), @insuranceitemno VARCHAR(10)
	SELECT	@insuranceitemno = Value FROM CountryMaintenance WHERE CodeNAME LIKE 'insitemno%'
	SELECT	@adminitemno = Value FROM CountryMaintenance WHERE CodeName LIKE 'adminitemno'
	
	IF @excludeinsadmin ='Y' -- exclude all strange non -stocks
	BEGIN
		SET @statement = @statement + 
		'AND d.itemno NOT IN ( ' + '''' + @adminitemno + '''' + ',' + '''' + @insuranceitemno + '''' + ',' 
		+ '''' + 'RB'  + ''''  + ',' +  '''' + 'DT' + '''' + ')'
	END
	
    SET @statement = @statement +   
  'AND d.itemno NOT IN ( ' + '''' + 'RB'  + ''''  + ',' +  '''' + 'DT' + '''' + ')'     
	
  IF(@code = 'REP')
    SET @statement = @statement + 'AND d.delorcoll = ' + '''' + 'R' + ''''
  ELSE
    SET @statement = @statement + 'AND d.delorcoll != ' + '''' + 'R' + ''''

	IF @itemno ='T' -- tax item 
		SET @statement = @statement + ' and d.itemno= ' + '''' + 'STAX' + '''' 
	ELSE 
	    SET @statement = @statement + ' and d.itemno != ' + '''' + 'STAX' + '''' 
	
IF @type in ('I' ,'T')
BEGIN
	IF @balance !='Y' OR @catBalanceAc =0
	  SET @statement = @statement 
			+ ' right(interfaceaccount,4)'  + ',' 
	ELSE 
		SET @statement = @statement 
			+ ' LEFT(t.interfacebalancing,2) + CONVERT(VARCHAR,S.category) ' + ','
END		

IF @type in ('I' ,'T')
  SET @statement = @statement + 
	' CONVERT(INT,LEFT(acctno,3)),	t.transtypecode, s.category ' 


	EXEC sp_executesql @statement 	
	IF @@ERROR != 0 OR @code = 'BHP'
		PRINT @statement 
GO 

-- Broker Extract Changes CR1036

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.tables WHERE table_name ='BrokerExtractHist')
CREATE TABLE BrokerExtractHist
(BrokerRunno INT,
FinIntStart INT , 
FinIntEnd INT ,
DelIntStart INT,
DelIntEnd INT)

GO 
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.tables WHERE table_name ='BrokerOracleSetup')
CREATE TABLE BrokerOracleSetup 
( code VARCHAR(6) NOT NULL PRIMARY KEY,
accounttypes VARCHAR(3),
codecatCategory1 VARCHAR(6),
codecatCategory1Not CHAR(3),
excludeRepos CHAR(1),
codecatCategory2 VARCHAR(6),
codecatCategory2Not CHAR(3),
excludeInsAdmin CHAR(1),
itemno VARCHAR(8),
CatBalanceAc BIT 
)

IF NOT EXISTS (SELECT * FROM BrokerOracleSetup WHERE CODE = 'bhp')
INSERT INTO BrokerOracleSetup (
	code,	accounttypes,	codecatCategory1,	
	codecatCategory1Not,	excludeRepos,	codecatCategory2,	
	codecatCategory2Not,	excludeInsAdmin,	itemno,CatBalanceAc
) VALUES ( 
	'BHP',	 'H',	'DIS',
	'NOT',	'Y',	 'WAR',
	'NOT',	'Y',	'' , 1) 
--HP warranties 
IF NOT EXISTS (SELECT * FROM BrokerOracleSetup WHERE CODE = 'bhw')
INSERT INTO BrokerOracleSetup (
	code,	accounttypes,	codecatCategory1,	
	codecatCategory1Not,	excludeRepos,	codecatCategory2,	
	codecatCategory2Not,	excludeInsAdmin,	itemno,CatBalanceAc
) VALUES ( 
	'BHW',	 'H',	'DIS',
	'NOT',	'Y',	 'WAR',
	'',	'Y',	'' , 1) 
-- insurance HP 
IF NOT EXISTS (SELECT * FROM BrokerOracleSetup WHERE CODE = 'bhi')
INSERT INTO BrokerOracleSetup (
	code,	accounttypes,	codecatCategory1,	
	codecatCategory1Not,	excludeRepos,	codecatCategory2,	
	codecatCategory2Not,	excludeInsAdmin,	itemno,CatBalanceAc
) VALUES ( 
	'BHI',	 'H',	'DIS',
	'NOT',	'Y',	 'WAR',
	'NOT',	'N',	'I' ,1) 
--ADMIN item 
IF NOT EXISTS (SELECT * FROM BrokerOracleSetup WHERE CODE = 'bha')
INSERT INTO BrokerOracleSetup (
	code,	accounttypes,	codecatCategory1,	
	codecatCategory1Not,	excludeRepos,	codecatCategory2,	
	codecatCategory2Not,	excludeInsAdmin,	itemno,CatBalanceAc
) VALUES ( 
	'BHA',	 'H',	'DIS',
	'NOT',	'Y',	 'WAR',
	'NOT',	'N',	'A' ,1) 
-- deferred terms
IF NOT EXISTS (SELECT * FROM BrokerOracleSetup WHERE CODE = 'bhs')
INSERT INTO BrokerOracleSetup (
	code,	accounttypes,	codecatCategory1,	
	codecatCategory1Not,	excludeRepos,	codecatCategory2,	
	codecatCategory2Not,	excludeInsAdmin,	itemno,CatBalanceAc
) VALUES ( 
	'BHS',	 'H',	'DIS',
	'NOT',	'Y',	 'WAR',
	'NOT',	'Y',	'DT' ,1) 
-- 
SELECT * FROM CODE WHERE CATEGORY = 'WAR'
IF NOT EXISTS (SELECT * FROM CODECAT WHERE category = 'WAR')
INSERT INTO codecat (
	origbr,	category,	catdescript,
	codelgth,	forcenum,	forcenumdesc,usermaint
) VALUES ( 
	/* origbr - smallint */ 0,	/* category - varchar(12) */ 'WAR',	/* catdescript - nvarchar(64) */ N'Warranty Categories',
	/* codelgth - int */ 3,	/* forcenum - char(1) */ 'N',	/* forcenumdesc - char(1) */ 'N',
	/* usermaint - char(1) */ 'Y' ) 

IF NOT EXISTS (SELECT * FROM code WHERE category = 'WAR')
BEGIN
	INSERT INTO code (
		origbr,		category,		code,
		codedescript,		statusflag,		sortorder,
		reference	) VALUES ( 
		/* origbr - smallint */ 0,		/* category - varchar(12) */ 'WAR',		/* code - varchar(12) */ '12',
		/* codedescript - nvarchar(64) */ N'Electrical Warranties',		/* statusflag - char(1) */ 'L',		/* sortorder - smallint */ 0,
		/* reference - varchar(12) */ '12' ) 
		
	INSERT INTO code (
		origbr,		category,		code,
		codedescript,		statusflag,		sortorder,
		reference	) VALUES ( 
		/* origbr - smallint */ 0,		/* category - varchar(12) */ 'WAR',		/* code - varchar(12) */ '82',
		/* codedescript - nvarchar(64) */ N'Furniture Warranties',		/* statusflag - char(1) */ 'L',		/* sortorder - smallint */ 0,
		/* reference - varchar(12) */ '82' ) 
END

	
	
IF NOT EXISTS (SELECT * FROM BrokerOracleSetup WHERE CODE = 'btx')
INSERT INTO BrokerOracleSetup (
	code,	accounttypes,	codecatCategory1,	
	codecatCategory1Not,	excludeRepos,	codecatCategory2,	
	codecatCategory2Not,	excludeInsAdmin,	itemno,	CatBalanceAc
	
) VALUES ( 
	'BTX',	 'A',	'',
	'',	'Y',	 '',
	'',	'N',	'T' ,1) 

IF NOT EXISTS (SELECT * FROM BrokerOracleSetup WHERE CODE = 'btx')
INSERT INTO BrokerOracleSetup (
	code,	accounttypes,	codecatCategory1,	
	codecatCategory1Not,	excludeRepos,	codecatCategory2,	
	codecatCategory2Not,	excludeInsAdmin,	itemno,CatBalanceAc
) VALUES ( 
	'BTX',	 'A',	'',
	'',	'Y',	 '',
	'',	'N',	'T' ,1) 
-- hp discounts
IF NOT EXISTS (SELECT * FROM BrokerOracleSetup WHERE CODE = 'BHC')
INSERT INTO BrokerOracleSetup (
	code,	accounttypes,	codecatCategory1,	
	codecatCategory1Not,	excludeRepos,	codecatCategory2,	
	codecatCategory2Not,	excludeInsAdmin,	itemno,CatBalanceAc
) VALUES ( 
	'BHC',	 'H',	'DIS',
	'',	'Y',	 '',
	'',	'Y',	'',1  ) 
-- 	cash discounts

IF NOT EXISTS (SELECT * FROM BrokerOracleSetup WHERE CODE = 'BCC')
INSERT INTO BrokerOracleSetup (
	code,	accounttypes,	codecatCategory1,	
	codecatCategory1Not,	excludeRepos,	codecatCategory2,	
	codecatCategory2Not,	excludeInsAdmin,	itemno,CatBalanceAc
) VALUES ( 
	'BCC',	 'C',	'DIS',
	'',	'Y',	 '',
	'',	'Y',	'' ,1) 
-- cash account deliveries
IF NOT EXISTS (SELECT * FROM BrokerOracleSetup WHERE CODE = 'bcp')
INSERT INTO BrokerOracleSetup (
	code,	accounttypes,	codecatCategory1,	
	codecatCategory1Not,	excludeRepos,	codecatCategory2,	
	codecatCategory2Not,	excludeInsAdmin,	itemno,CatBalanceAc
) VALUES ( 
	'BCP',	 'C',	'DIS',
	'NOT',	'Y',	 'WAR',
	'NOT',	'Y',	'' ,1) 

-- cash account warranties
IF NOT EXISTS (SELECT * FROM BrokerOracleSetup WHERE CODE = 'bcw')
INSERT INTO BrokerOracleSetup (
	code,	accounttypes,	codecatCategory1,	
	codecatCategory1Not,	excludeRepos,	codecatCategory2,	
	codecatCategory2Not,	excludeInsAdmin,	itemno,CatBalanceAc
) VALUES ( 
	'BCW',	 'C',	'DIS',
	'NOT',	'Y',	 'WAR',
	'',	'Y',	'' , 1) 

IF NOT EXISTS (SELECT * FROM BrokerOracleSetup WHERE CODE = 'REP')
INSERT INTO BrokerOracleSetup (
	code,	accounttypes,	codecatCategory1,	
	codecatCategory1Not,	excludeRepos,	codecatCategory2,	
	codecatCategory2Not,	excludeInsAdmin,	itemno,CatBalanceAc
) VALUES ( 
	'REP',	 'H',	'',
	'',	'R',	 '',
	'',	'N',	'' ,1) 
/*
IF NOT EXISTS (SELECT * FROM BrokerOracleSetup WHERE CODE = 'REB')
INSERT INTO BrokerOracleSetup (
	code,	accounttypes,	codecatCategory1,	
	codecatCategory1Not,	excludeRepos,	codecatCategory2,	
	codecatCategory2Not,	excludeInsAdmin,	itemno,CatBalanceAc
) VALUES ( 
	'REB',	 'H',	'',
	'',	'R',	 '',
	'',	'N',	'RB' ,1 ) 				*/
	
GO 

IF NOT EXISTS (SELECT * FROM BrokerExtractHist )
AND EXISTS (SELECT * FROM interfacecontrol WHERE interface = 'BrokerX')
BEGIN
	

declare @runno char(12),@counter INT ,@datestart DATETIME, @prevdatestart DATETIME, @datefinish DATETIME, @prevdatefinish DATETIME,
@FinIntStart INT ,	@FinIntEnd SMALLINT, @DelIntStart INT,@DelIntEnd INT 

SET @counter = 1
declare BEX_cursor CURSOR FAST_FORWARD READ_ONLY FOR
SELECT runno, datestart, datefinish 
FROM interfacecontrol 
WHERE interface = 'BrokerX' order by runno 
OPEN BEX_cursor
FETCH NEXT FROM BEX_cursor INTO @runno,@datestart, @datefinish
WHILE @@FETCH_STATUS = 0
BEGIN
		IF @counter != 1
		BEGIN
			SELECT @FinIntStart= MIN(runno) FROM interfacecontrol WHERE interface ='updsmry'
		    AND datestart BETWEEN @prevdatefinish AND @datefinish 
		    
		    SELECT @FinIntEnd = MAX(runno)  FROM interfacecontrol WHERE 
		    interface ='updsmry'
		    AND datestart BETWEEN @prevdatefinish AND @datefinish 
		    
		    SELECT @DelIntStart= MIN(runno) FROM interfacecontrol WHERE interface ='COS FACT'
		    AND datestart BETWEEN @prevdatefinish AND @datefinish 
		    
		    SELECT @DelIntEnd = MAX(runno)  FROM interfacecontrol WHERE 
		    interface ='COS FACT'
		    AND datestart BETWEEN @prevdatefinish AND @datefinish 
		    

			INSERT INTO BrokerExtractHist (
				BrokerRunno,
				FinIntStart,
				FinIntEnd,
				DelIntStart,
				DelIntEnd
			) VALUES ( 
				/* BrokerRunno - int */ @runno,
				/* FinIntStart - int */ @FinIntStart,
				/* FinIntEnd - int */ @FinIntEnd,
				/* DelIntStart - int */ @DelIntStart,
				/* DelIntEnd - int */ @DelIntEnd) 
			SET @prevdatefinish = @datefinish
			SET @prevdatestart = @datestart 
		END 
		SET @counter = @counter + 1 
FETCH NEXT FROM BEX_cursor INTO @runno,@datestart,@datefinish


END

CLOSE BEX_cursor
DEALLOCATE BEX_cursor

END