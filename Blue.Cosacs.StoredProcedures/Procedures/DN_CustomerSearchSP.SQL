SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_CustomerSearchSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_CustomerSearchSP]
GO
CREATE procedure	dbo.DN_CustomerSearchSP
-- ================================================
-- Project      : CoSACS .NET
-- File Name    : DN_CustomerSearchSP.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Customer Search 
-- Author       : ??
-- Date         : ??
--
-- This procedure will retrieve details in the customer search screen.
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 29/03/10 jec UAT52 Error occurs when selecting row.
-- 16/08/10 jec CR1084 Allow search by Address and any Phone number 
-- 30/09/10 ip  UAT46 - UAT5.4 - Duplicate rows returned due to missing conditions when joining onto custtel
-- 20/10/10 jec UAT85 - check for nulls
-- 26/01/12 jec #9541 Time Out error in customer search screen.
--				remove single quote from any search field - causes error in dynamic sql
-- 22/01/14  ip #17083 Service Request charge account table changed from SR_ChargeAcct to ServiceChargeAcct
-- ================================================
	-- Add the parameters for the stored procedure here
			@custid varchar(20),
			@first varchar(30),
			@last varchar(60),
			@limit int,
			@settled smallint,
			@exact int,
			@storetype varchar(2),
			@address VARCHAR(60),		-- CR1084
			@phone VARCHAR(20),			-- CR1084
			@return int OUTPUT

AS
	
	DECLARE @sqlText SQLText, @limit1 INT
	/* this will be the basis for a dynamic sql statement. The 
	   where clause will be built up depending on the parameters
	   passed in
	*/
	-- remove any quotes #9541
	set @custid= REPLACE(@custid,'''','')
	set @first= REPLACE(@first,'''','')
	set @last= REPLACE(@last,'''','')
	set @address= REPLACE(@address,'''','')
	set @phone= REPLACE(@phone,'''','')
	
	--IP - 13/03/09 - (70849) - Added 'Holdprop'
	--IP - 30/09/10 - UAT(46) UAT5.4 - Check for current telephone number tellocn = 'H' and datediscon is null
	SET	@sqlText = '	INSERT 
				INTO 	#customera 
					(custid, title, firstname, name,acctno,hldorjnt, accttype,dateacctopen, outstbal, arrears, agrmttotal, currstatus, alias,   agrmtno, privilegeclub, branchno, archived, bdw, holdprop,cusaddr1,cusaddr2,cusaddr3,telno,postcode)
				SELECT TOP ' + CONVERT(VARCHAR,@limit *3) + -- we are doing twice the limit as some of these will be shrunk but don't want to write too much to db or query will be too slow
				'   C.custid, C.title, C.firstname,	C.name,	CA.acctno,CA.hldorjnt,  '''', null,0,0,0, '''',C.alias, 1, 0, 0, 0, 0, '''',
						cusaddr1,cusaddr2,cusaddr3,max(LTRIM(RTRIM(t.dialcode))+replace(telno,''-'','''')),' +  -- adding max to allow to search on any phone - see group by later
						' cuspocode as postcode
				FROM   	customer C LEFT JOIN custacct CA ON CA.custid = C.custid 
					    left outer join custaddress ad ON C.custid = ad.custid AND ad.addtype = ''H'' and ad.datemoved is null
						left outer join custtel t ON C.custid = t.custid ' + --AND t.tellocn = ''H'' removing so users can search on mobile phones
						' and t.datediscon is null	'
						--inner join acct a on ca.acctno=a.acctno 
				--WHERE	isnull(CA.hldorjnt, ''H'') = ''H'' --remove as unable to view non-main holder customers
				+ ' WHERE C.storetype LIKE ' + '''' + @storetype + '''' +
				  -- ' and (' + '''' + @sett + '''' + ' = ''1'' or a.currstatus !=''S'')
				' AND 1 = 1' --leave this as a dummy clause as needed for building up where clause below
		
	--SET @limit1 = @limit * 10	--set intial limit higher as may be mulitple rows for account (distinct is used)
	
	--SET	ROWCOUNT  @limit1 
	-- remove any hyphens
	SET @phone = REPLACE(@phone,'-','')
	
	IF @exact = 0
	BEGIN
		IF @custid !=''
			SET @custid =@custid +'%'
		IF @first !='' 
		  SET @first ='"*' + @first +'*"' -- contains needs this'
		IF @last !=''
			SET @last =@last +'%'
			-- Search address and phone number  CR1084
		IF @address !=''		
		 	SET @address = '"*' + @address +'*"' --contains needs this'
		IF @phone !=''
			SET @phone = @phone +'%'
	END

	SET NOCOUNT ON 
	
	SET 	@return = 0			--initialise return code
	
	CREATE TABLE 	#customera 
			(custid varchar (20) , 
			title varchar (25),
			firstname varchar (30), 
			name varchar (60), 
			acctno char (12), 
			hldorjnt char(1), 
			accttype varchar(1),
	   		dateacctopen DateTime, 
	   		outstbal money , 
	   		arrears money , 
	   		agrmttotal money, 
	   		currstatus char(1), 
			alias varchar (25), 
			agrmtno int, 
			cashprice money, 
		privilegeclub smallint,
		branchno smallint,
		archived bit,--IP - 30/01/09 - CR971 
		bdw bit,		--IP - 09/02/09 - CR971 
		holdprop char(1),
		cusaddr1 VARCHAR(50),cusaddr2 VARCHAR(50),cusaddr3 VARCHAR(50),telno VARCHAR(40), postcode VARCHAR(10)
	)
	
	IF(@exact = 1)
	BEGIN
		IF @custid !='' 	/* exact customer ID */
		BEGIN
			SET	@sqlText = @sqlText + ' AND C.custid = ''' + @custid + '''' 
		END 
		ELSE 			/* exact names */
		BEGIN
			IF(@first != '')
			BEGIN
	--			SET	@sqlText = @sqlText + ' AND C.firstname = ''' + @first + ''''
				SET	@sqlText = @sqlText + ' AND freetext ((firstname), ' + '''' + @first + ''''  + ' ) '
	
			END

			IF(@last != '')
			BEGIN
				SET	@sqlText = @sqlText + ' AND C.name = ''' + @last + ''''			
			END
			-- Address lines  CR1084
			DECLARE @ftext VARCHAR(12) -- either contains or freetext - freetext required if space in there. 
			IF @address != ''
			BEGIN
				IF CHARINDEX(' ', @address) = 0
					SET @ftext = 'CONTAINS'
				ELSE
					SET @ftext = 'FREETEXT '	 
				SET @sqlText = @sqlText + ' AND ' + @ftext + ' ((Cusaddr1,Cusaddr2,Cusaddr3),' + '''' + @address + '''' + ') '  
			END
	
			-- Phone Number (any)  CR1084
			
			IF @phone != ''	
			BEGIN
				IF CHARINDEX(' ', @phone ) = 0
					SET @Ftext = 'CONTAINS'
				ELSE
					SET @ftext = 'FREETEXT '	 
				SET @sqlText = @sqlText +   ' AND ' + @ftext + ' ((telno), ' + '''' + @phone + ''''  + ') ' 
		
			END				
			--		IF(@phone != '')
			--BEGIN
			--	SET	@sqlText = @sqlText + ' AND ((replace(t.telno,''-'','''') = ''' + @phone + ''' ) 
			--		or  LTRIM(RTRIM(t.dialcode))+ISNULL(REPLACE(t.telno,''-'',''''),'''') = ''' + @phone + ''' )'					
			--END
		END
	END
	ELSE
	BEGIN	/* not exact match */

		IF(@first != '')
		BEGIN
			IF CHARINDEX(' ', @first) = 0
					SET @ftext = 'CONTAINS'
				ELSE
					SET @ftext = 'FREETEXT '	
			--SET	@sqlText = @sqlText + ' AND C.firstname like ''' + @first + ''''
			SET	@sqlText = @sqlText +  ' AND ' + @ftext + ' ((firstname), ' + '''' + @first + ''''  + ' ) '
	
		END

		IF(@last != '')
		BEGIN
	--		SET	@sqlText = @sqlText + ' AND C.name like ''' + @last + ''''		
			SET	@sqlText = @sqlText + '  AND name like ' + '''' + @last + ''''   
		END

		IF(@custid != '')
		BEGIN
			SET	@sqlText = @sqlText + ' AND C.custid like ''' + @custid + ''''
		END
		
		-- Address lines  CR1084
		IF(@address != '')
		BEGIN
		
			IF CHARINDEX(' ', @address) = 0
					SET @ftext = 'CONTAINS'
				ELSE
					SET @ftext = 'FREETEXT '
			--SET	@sqlText = @sqlText + ' AND (ad.cusaddr1 like ''' + @address + ''' or ad.cusaddr2 like ''' + @address + ''' or ad.cusaddr3 like ''' + @address + ''')'			
			SET	@sqlText = @sqlText + ' AND  ' + @ftext + ' ((Cusaddr1,Cusaddr2,Cusaddr3), ' + '''' + @address + ''''  + ' ) '
		END
		-- Phone Number (any)  CR1084
		IF(@phone != '')
		BEGIN
			SET	@sqlText = @sqlText + ' AND ((replace(t.telno,''-'','''') like ''' + @phone + ''' ) 
					or  LTRIM(RTRIM(t.dialcode))+ISNULL(REPLACE(t.telno,''-'',''''),'''') like ''' + @phone + ''' )'				
		END
	END

    SET @sqlText =@sqlText + 
    ' GROUP BY  C.custid, C.title, C.firstname,	C.name,	CA.acctno,CA.hldorjnt,c.alias,
	 ad.cusaddr1,ad.cusaddr2,ad.cusaddr3,ad.cuspocode'

	EXEC sp_executesql @sqlText
    IF DB_NAME()!='Cosacs'
		PRINT @sqltext
	CREATE CLUSTERED INDEX cserwer ON #customera(acctno)

	IF @settled =0 -- remove customers who don't have open accounts
		DELETE FROM #customera WHERE NOT EXISTS (SELECT * FROM acct a 
		JOIN custacct ca ON a.acctno = ca.acctno
		WHERE a.currstatus !='S' AND ca.custid =#customera.custid)
	
		--IP - 30/01/09 - CR971 - Tag the account as an archived account.
	UPDATE #customera
	SET archived = 1
	WHERE EXISTS (select * from cosacs_archive.dbo.agreement_archive ag
					where ag.acctno = #customera.acctno)
	
	--IP - 09/02/09 - CR971 - Update 'bdw' to true if the bdwbalance on the account > 0
	UPDATE #customera
	SET bdw = 1
	WHERE EXISTS(select * from acct a where a.acctno = #customera.acctno
					and a.bdwbalance > 0)
	
	UPDATE 	#customera 
	SET  	dateacctopen=a.dateacctopen,
		arrears = a.arrears,
		agrmttotal = a.agrmttotal, 
		outstbal = a.outstbal,
		currstatus = a.currstatus,
		accttype = at.accttype, 
		agrmtno = ag.agrmtno,
			cashprice = ag.cashprice,
			holdprop = ag.holdprop,
			branchno = a.branchno
	FROM 	acct a, accttype at, agreement ag
	WHERE 	a.acctno = #customera.acctno
	AND 	a.accttype = at.genaccttype 
	AND 	a.acctno = ag.acctno
	
	--IP - 02/02/09 - CR971 - Update for 'Archived' accounts
	--requires join on cosacs_archive.dbo.agreement_archive
		UPDATE 	#customera
		SET  	dateacctopen=a.dateacctopen,
			arrears = a.arrears,
			agrmttotal = a.agrmttotal, 
			outstbal = a.outstbal,
			currstatus = a.currstatus,
			accttype = at.accttype, 
			agrmtno = ag.agrmtno,
				cashprice = ag.cashprice,
				branchno = a.branchno
		FROM 	acct a, accttype at, cosacs_archive.dbo.agreement_archive ag
		WHERE 	a.acctno = #customera.acctno
		AND 	a.accttype = at.genaccttype 
		AND 	a.acctno = ag.acctno
		AND		#customera.archived = 1
		
	-- CR1084 row limit here so final select is limited not initial select
	SET	ROWCOUNT  @limit		
	
		-- agrmttotal for Store card accounts set to maximum spend
	IF EXISTS (SELECT * FROM #customera WHERE accttype = 'T')
		UPDATE #customera SET agrmttotal = ISNULL(c.StoreCardLimit,0)
		FROM customer c, countrymaintenance m , proposal p
		WHERE c.custid = #customera.custid 
		AND m.codename = 'StorecardPercent'
		AND #customera.accttype = 'T'
		and p.custid= c.custid and p.propresult = 'A' -- accepted 
		AND p.acctno= #customera.acctno 

	SELECT  DISTINCT c.custid as 'Customer ID', 
		isnull(title, '') as 'Title',
		isnull(firstname,'') as 'First Name', 
		isnull(name,'') as 'Last Name', 
		isnull(C.acctno,'') as 'AccountNumber',
		isnull(hldorjnt,'') as 'HldOrJnt',		-- 67782 
		CASE WHEN L.loyaltyAcct IS NOT NULL THEN 'HCC' 
		     WHEN SR.acctno IS NOT NULL THEN 'SRC'
		     ELSE isnull(C.accttype,N'') END as 'Type',  
		--isnull(accttype,'') as 'Type',		-- 29/03/10 jec UAT52
		isnull(dateacctopen,'') as 'Date Opened', 			
		isnull(outstbal,0) as 'Outstanding Balance', 
		isnull(arrears,0) as 'Arrears',
		isnull(agrmttotal,0) as 'Agreement Total',
		isnull(currstatus,'') as 'Status',
		isnull(alias,'') as 'Alias'	,
		-1 as 'addto',		--dummy column which will be populated later
		-1 as 'DeliveredIndicator',
		-1 as 'reversible',
		isnull(agrmtno, 1) as agrmtno,
			isnull(cashprice, 0) as cashprice,
			branchno,
			archived,
			bdw as 'Bdw',
		isnull(holdprop,'') as holdprop,
		loyalty.MemberNo AS LoyaltyMemNo,
		ISNULL(cusaddr1,'') as cusaddr1,  ISNULL(cusaddr2,'') as cusaddr2,ISNULL(cusaddr3,'') as cusaddr3,	--UAT85 jec
		ISNULL(postcode,'') AS postcode,		--UAT85 jec
		ISNULL(telno,'') as telno,		--UAT85 jec
		CASE	
			WHEN EXISTS(SELECT 1 FROM dbo.Installation WHERE AcctNo = c.acctno) THEN CONVERT(BIT, 1)
			ELSE CONVERT(BIT, 0)
		END AS [HasInatallations]	
	FROM	#customera c
    LEFT OUTER JOIN loyalty ON Loyalty.Custid = c.Custid
                            AND Loyalty.StatusAcct = 1
    LEFT OUTER JOIN Loyalty L ON C.acctno = L.LoyaltyAcct
    LEFT OUTER JOIN ServiceChargeAcct SR ON SR.AcctNo = C.acctno			--#17083
	WHERE 	(@settled = 1 or currstatus !='S')
	order by [Date Opened] desc 	

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END
	SET	ROWCOUNT  0
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO
-- End End End End End End End End End End End End End End End End End End End End End End End End End End 

