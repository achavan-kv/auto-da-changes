SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO

if exists (select * from dbo.sysobjects
where id = object_id('[dbo].[DN_AccountsSearchSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_AccountsSearchSP]
GO

CREATE procedure dbo.DN_AccountsSearchSP
-- ================================================
-- Project      : CoSACS .NET
-- File Name    : DN_AccountsSearchSP.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Customer Search 
-- Author       : ??
-- Date         : ??
--
-- This procedure will retrieve details in the account search screen.
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 16/08/10 jec CR1084 Allow search by any Phone number
-- 01/11/11 jec #8489 CR1232 remove ability to revise cash loan accounts  
-- 22/11/11 AA   Performance improvement Full text index use for last name where more than 3 characters long
-- 22/01/14 ip  #17083 Service Request charge account table changed from SR_ChargeAcct to ServiceChargeAcct
-- ================================================
	-- Add the parameters for the stored procedure here
    @accountNo          varchar(50),
    @custId             varchar(50),
    @firstName          varchar(50),
    @lastName           varchar(50),
    @address            varchar(50),
    @postCode           varchar(50),    
    @accountStatus      varchar(50),
    @limit              INT,
    @exact              INT,
    @storetype			varchar(2),
    @accountExists      INT OUTPUT,
    @accountType        CHAR(1) OUTPUT,
    @phoneNumber		VARCHAR(20),			-- CR1084
    @return             INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON
    SET  @return = 0            --initialise return code
    SET  @accountExists = 0
    DECLARE @limit1 INT
    DECLARE @sqlText SQLText, @fnameclause varchar(300), @lnameclause VARCHAR(300),
    @addressclause VARCHAR(300), @pcodeclause VARCHAR(300),@phoneclause VARCHAR(300),
    @columns  VARCHAR(1500) , @NL varchar(64), @groupby VARCHAR(1000)
    SET @fnameclause = '' SET @lnameclause ='' SET  @addressclause = ''
    SET @pcodeclause = '' SET @phoneclause =''
    set @nl = '
    '
    SET @columns = ' INSERT INTO #search '
		+ 'SELECT  TOP (' + convert(varchar,@LIMIT*3) + ') C.custid        as custid,'	--CR1084 AA putting in limit to prevent too much insert activity if someone enters single character 
		+ '		C.title         AS Title,'
		+ '		C.firstname     AS firstname,'
		+ '		C.name          AS name,'
		+ '		C.dateborn      AS DOB ,'
				--CR1084  -- revamp select  
		+ '		a.cusaddr1, a.cusaddr2,a.cusaddr3,a.cuspocode,'		
		+	'	case when ' + '''' + @phoneNumber + '''' + ' ='''' then '''' ' 
		+  ' else max(LTRIM(RTRIM(t.dialcode) + REPLACE(t.telno,''-'',''''))) end as phone  '  +
		+ ' , '''' ' + 
		'   FROM    customer C INNER JOIN custaddress a on c.custid = a.custid and a.addtype=''H''
							LEFT OUTER JOIN custtel t on c.custid=t.custid '
		+		' WHERE 1=1 '  + @nl
		
	SET @groupby = ' GROUP BY  c.custid,c.title,c.firstname,c.name,c.dateborn,a.cusaddr1,a.cusaddr2,a.cusaddr3,a.cuspocode '
	
    if @exact = 0
	begin
		if @firstname !='' AND DATALENGTH(@firstname) > 3
			set @firstname = '"*' + @firstname + '*"'
		ELSE 
			SET @firstname =@firstname + '%'
	    if @address !=''
			set @address='"*' + @address + '*"'
		if @lastname !='' AND DATALENGTH(@lastname) >3
			set @lastname = '"*' + @lastname + '*"'
		ELSE 
			SET @lastname =@lastname + '%'
	    
	end 

	DECLARE @ftext VARCHAR(12) -- determines whether to use full text phrase contains or freetext. If space need to use Freetext.	
   	IF @firstname !=''
   	BEGIN 
   		IF DATALENGTH(@firstname) > 3
   		BEGIN
   				IF CHARINDEX(' ', @firstname) = 0 -- check whether space in name -- if so have to use freetext instead of contains
					SET @ftext = 'CONTAINS'
				ELSE
					SET @ftext = 'FREETEXT '	
			SET @fnameclause =  ' AND ' + @ftext + ' ((firstname),' + '''' + @firstname + '''' + ') ' + @nl
		END 
		ELSE 
		BEGIN
			SET @fnameclause =  ' AND firstname like ' + ''''  + @firstname + ''''
		END
	END
	IF @lastname !='' 
	BEGIN 
		IF DATALENGTH(@lastname) >3
		BEGIN
			 IF CHARINDEX(' ', @lastname) = 0 -- check whether space in name -- if so have to use freetext instead of contains
					SET @ftext = 'CONTAINS'
				ELSE
					SET @ftext = 'FREETEXT '	
			SET @Lnameclause =  ' AND ' + @ftext + ' ((name),' + '''' + @lastname + '''' + ') ' + @nl
		END 
		ELSE
		BEGIN
		 	
			IF @exact = 1 
				SET @Lnameclause = ' AND name = '  + '''' + @lastname + ''''
			ELSE
				SET @Lnameclause = ' AND name like '  + '''' + @lastname + '%' + ''''
		END
	END 	

	IF @address != ''
	BEGIN
		IF CHARINDEX(' ', @address) = 0
			SET @ftext = 'CONTAINS'
		ELSE
			SET @ftext = 'FREETEXT '	 
		SET @addressclause = ' AND ' + @ftext + ' ((Cusaddr1,Cusaddr2,Cusaddr3),' + '''' + @address + '''' + ') ' + @nl
		--and (c.name=@lastname OR @lastname ='')
    END 			
	IF @postcode != ''	
	BEGIN
		IF CHARINDEX(' ', @postcode) = 0
			SET @Ftext = 'CONTAINS'
		ELSE
			SET @ftext = 'FREETEXT '	 
		SET @pcodeclause =  ' AND ' + @ftext + ' ((cuspocode), ' + '''' + @postcode + ''''  + ') ' + @nl
		
	END				
	    	--and ((a.cusaddr1=@address or a.cusaddr2=@address or a.cusaddr3=@address) and @address!='')
			--and (a.cuspocode=@postcode OR @postcode='')
			
	IF @phoneNumber != ''	
	BEGIN
		IF CHARINDEX(' ', @phoneNumber ) = 0
			SET @Ftext = 'CONTAINS'
		ELSE
			SET @ftext = 'FREETEXT '	 
		SET @phoneclause =  ' AND ' + @ftext + ' ((telno), ' + '''' + @phonenumber + ''''  + ') ' + @nl
		
	END				
	--IF @phoneNumber !=''
	--	SET @phoneclause =  ' and ((ISNULL(REPLACE(t.telno,''-'',''''),'''')=' + '''' + @phoneNumber + '''' +
	--			'	or LTRIM(RTRIM(t.dialcode))+ISNULL(REPLACE(t.telno,''-'',''''),'''')= ' + '''' + @phoneNumber + '''' + ' )) '
				--OR ' + @phoneNumber + '=' + '''' + '''' + ') '  + @nl
	--SET @limit1 = @limit * 5	--set intial limit higher as may be mulitple rows for account (distinct is used)
	
	--SET	ROWCOUNT  @limit1 
    -- AA only load up settled account if user has checked settled account
    -- box to prevent revise agreement of settled account

	 SET @sqltext = @sqltext + @fnameclause + @Lnameclause + @addressclause + @pcodeclause + @phoneclause


    IF EXISTS (SELECT acctno FROM acct
               WHERE  acctno = @accountNo
               AND    @accountno != '000000000000'
               AND    (@accountStatus != 'S' or currstatus != 'S') )
    AND NOT EXISTS (SELECT acctno
                    FROM   custacct
                    WHERE  acctno = @accountNo
                    AND    @accountno != '000000000000'
                    AND    custacct.hldorjnt != 'H')
    BEGIN
        SET @accountExists = 1
        -- returning account type to prevent future unnecessary database call
        SET @accountType = (SELECT AT.accttype FROM acct A INNER JOIN accttype AT	
	                        ON	A.accttype = AT.genaccttype WHERE acctno = @accountno)
    END
    ELSE
    BEGIN
        CREATE TABLE #search
           (custid      varchar(20),
            title       varchar(25),
            firstname   varchar(30),
            name        varchar(60),
            DOB         datetime,
            cusaddr1    varchar(50),
            cusaddr2    varchar(50),	--CR1084
            cusaddr3    varchar(50),	--CR1084            
            cuspocode   varchar(10),
            phone       varchar(30),
            email       varchar(60))

        IF (@accountNo = '000000000000')   -- Search on CUSTOMER
        BEGIN
            IF(@exact = 1)
            BEGIN
                IF @custid != ''    -- exact match on Customer ID
                BEGIN
                    SET  @sqltext = @columns + ' AND   C.custid = ' + '''' + @custId + '''' 
					 + @fnameclause + @Lnameclause + @addressclause + @pcodeclause + @phoneclause
					 + ' AND a.datemoved is null AND t.datediscon IS NULL ' +
					 + ' AND C.storetype LIKE ' + '''' + @storetype + ''''
                END
                ELSE	-- IF @firstName != ''      -- exact match on name   CR1084
                BEGIN
                     SET  @sqltext = @columns + 
                      + @fnameclause + @Lnameclause + @addressclause + @pcodeclause + @phoneclause
					 + ' AND a.datemoved is null  AND t.datediscon IS NULL ' +
					 + ' AND C.storetype LIKE ' + '''' + @storetype + ''''
                END
            END
            ELSE                -- NOT exact match on Customer ID
            BEGIN
                SET @custid = @custid + '%'
                SET @postCode = @postCode + '%'			--CR1084
                SET @phoneNumber = @phoneNumber + '%'	--CR1084

                IF @custid != '%'       -- like search on custid and name
                BEGIN
                    SET  @sqltext = @columns + ' AND   C.custid LIKE ' + '''' + @custId + '''' 
					 + @fnameclause + @Lnameclause + @addressclause + @pcodeclause + @phoneclause
					 + ' AND a.datemoved is NULL  AND t.datediscon IS NULL  ' +
					 + ' AND C.storetype LIKE ' + '''' + @storetype + ''''
                END
                ELSE                    -- like search on other fields
                BEGIN
                                       SET  @sqltext = @columns +
					 + @fnameclause + @Lnameclause + @addressclause + @pcodeclause + @phoneclause
					 + ' AND a.datemoved is NULL  AND t.datediscon IS NULL  ' +
					 + ' AND C.storetype LIKE ' + '''' + @storetype + ''''
                END
            END
			
			SET @sqltext = @sqltext + @groupby
			
			EXEC sp_executesql @sqltext
			PRINT @sqltext 
            UPDATE  #search
            SET     cusaddr1    = ca.cusaddr1,
                    cuspocode   = ca.cuspocode
            FROM    custaddress ca
            WHERE   ca.custid = #search.custid
            AND     (@address = '' OR CA.cusaddr1 LIKE @address + '%' )
            AND     (@postCode = '' OR CA.cuspocode LIKE @postCode + '%' )
            AND     ca.addtype= 'H' and ca.datemoved is null

			if @phoneNumber='' or @phoneNumber='%'		--CR1084 only update if not search by phone number
			begin
				UPDATE  #search
				SET     phone = LTRIM(RTRIM(ct.dialcode)) + REPLACE(ct.telno,'-','')			--CR1084
				FROM    custtel ct
				WHERE   #search.custid = CT.custid
				AND     ct.tellocn = 'H'
				AND     ct.datediscon is null
				
				-- use Mobile if home doesn't exists
				UPDATE  #search
				SET     phone = LTRIM(RTRIM(ct.dialcode)) + REPLACE(ct.telno,'-','')
				FROM    custtel ct
				WHERE   #search.custid = CT.custid
				AND     ct.tellocn = 'M'
				AND     ct.datediscon is null
				and not exists(select * from custtel ct2 where #search.custid = CT2.custid and ct2.tellocn='H')
            end
			
			--SET ROWCOUNT @limit			--CR1084 set row limit on final select removing using top (n)
			
            SELECT TOP (@limit) #search.custid  AS [Customer ID],		--CR1084
                    acct.acctno         AS [Account Number],
                    acct.currstatus     AS Status,
                    #search.title       AS Title,
                    #search.firstname   AS [First Name],
                    #search.name        AS [Last Name],
                    #search.DOB         AS DOB,
                    CASE WHEN Loyalty.LoyaltyAcct IS NOT NULL THEN 'HCC' 
			        WHEN SR.AcctNo IS NOT NULL THEN 'SRC' 
			        ELSE RTRIM(accttype.accttype) END AS [Account Type], --IP - 19/04/10 - UAT(58) UAT5.2 -- AA trimming   
                    --accttype.accttype   AS [Account Type],
                    #search.cusaddr1    AS Address,
                    #search.cuspocode   AS [Post Code],
                    #search.phone       AS Phone,
                    #search.Email       AS Email,
                    custacct.hldorjnt   AS HldOrJnt,
                    Loyalty.MemberNo AS HCmemberno,
                    CASE	
						WHEN EXISTS(SELECT 1 FROM dbo.Installation WHERE AcctNo = acct.acctno) THEN CONVERT(BIT, 1)
						ELSE CONVERT(BIT, 0)
					END AS [Has Inatallations],
					t.IsLoan					-- #8489		    
            FROM    #search
            INNER JOIN custacct ON #search.custid = custacct.custid
            INNER JOIN acct ON custacct.acctno = acct.acctno
            INNER JOIN accttype ON accttype.genaccttype = acct.accttype
            INNER JOIN TermsTypeTable t on acct.TermsType=t.TermsType		-- #8489
            LEFT OUTER JOIN Loyalty ON Loyalty.Custid = custacct.custid     
                              AND Loyalty.StatusAcct = 1   
            LEFT OUTER JOIN ServiceChargeAcct SR ON SR.AcctNo = acct.acctno	--#17083
            WHERE   (NOT (acct.currstatus LIKE @accountStatus))
            -- FR68132 return joint accounts for this customer as well
            -- AND     custacct.hldorjnt = 'H'

        END
        ELSE            -- Search on ACCOUNT NUMBER
        BEGIN

            INSERT INTO #search
            SELECT  C.custid        as custid,
                    C.title         AS Title,
                    C.firstname     AS firstname,
                    C.name          AS name,
                    C.dateborn      AS DOB , 
                    '', '',
                    '', '', '', ''
            FROM    customer C, custacct ca
            WHERE   ca.acctno = @accountNo
            AND     ca.custid = C.custid
            AND		C.storetype LIKE @storetype

            UPDATE  #search
            SET     cusaddr1 = ca.cusaddr1,
                    cuspocode = ca.cuspocode
            FROM    custaddress ca
            WHERE   ca.custid = #search.custid
            AND     (@address = '' OR CA.cusaddr1 LIKE @address + '%' )
            AND     (@postCode = '' OR CA.cuspocode LIKE @postCode + '%' )
            AND     ca.addtype = 'H' and ca.datemoved is null

			if @phoneNumber='' or @phoneNumber='%'		--CR1084 only update if not search by phone number
			begin
				UPDATE  #search
				SET     phone = LTRIM(RTRIM(ct.dialcode)) + REPLACE(ct.telno,'-','')			--CR1084
				FROM    custtel ct
				WHERE   #search.custid = CT.custid
				AND     ct.tellocn = 'H'
				AND     ct.datediscon is null
				-- use Mobile if home doesn't exists
				UPDATE  #search
				SET     phone = LTRIM(RTRIM(ct.dialcode)) + REPLACE(ct.telno,'-','')
				FROM    custtel ct
				WHERE   #search.custid = CT.custid
				AND     ct.tellocn = 'M'
				AND     ct.datediscon is null
				and not exists(select * from custtel ct2 where #search.custid = CT2.custid and ct2.tellocn='H')
            end
            
		--SET ROWCOUNT @limit			--CR1084 set row limit on final select
            SELECT  TOP (@limit) #search.custid   AS [Customer ID],			--CR1084
                    acct.acctno         AS [Account Number],
                    acct.currstatus     AS Status,
                    #search.title       AS Title,
                    #search.firstname   AS [First Name],
                    #search.name        AS [Last Name],
                    #search.DOB         AS DOB,
                    CASE WHEN Loyalty.LoyaltyAcct IS NOT NULL THEN 'HCC' 
			        WHEN SR.AcctNo IS NOT NULL THEN 'SRC' 
			        ELSE RTRIM(accttype.accttype) END AS [Account Type], --IP - 19/04/10 - UAT(58) UAT5.2 -- AA trimming        
                    --accttype.accttype   AS [Account Type],
                    #search.cusaddr1    AS Address,
                    #search.cuspocode   AS [Post Code],
                    #search.phone       AS Phone,
                    #search.Email       AS Email,
                    custacct.hldorjnt   AS HldOrJnt,
                    Loyalty.MemberNo AS HCmemberno,
                    CASE	
						WHEN EXISTS(SELECT 1 FROM dbo.Installation WHERE AcctNo = acct.acctno) THEN CONVERT(BIT, 1)
						ELSE CONVERT(BIT, 0)
					END AS [Has Inatallations],
					t.IsLoan					-- #8489
            FROM    #search
            INNER JOIN custacct ON #search.custid = custacct.custid
            INNER JOIN acct ON custacct.acctno = acct.acctno
            INNER JOIN TermsTypeTable t on acct.TermsType=t.TermsType		-- #8489
            INNER JOIN accttype ON accttype.genaccttype = acct.accttype			
            LEFT OUTER JOIN Loyalty ON Loyalty.Custid = custacct.custid     
                              AND Loyalty.StatusAcct = 1
            LEFT OUTER JOIN Loyalty L ON L.LoyaltyAcct = acct.acctno   
            LEFT OUTER JOIN ServiceChargeAcct SR ON SR.AcctNo = acct.acctno  --#17083
            WHERE   (NOT (acct.currstatus LIKE @accountStatus))
            AND     custacct.acctno = @accountNo

        END
    END

    SET NOCOUNT OFF
    SET ROWCOUNT 0
    SET @return = @@error
END
GO

-- End End End End End End End End End End End End End End End End End End End End End End End End End End 