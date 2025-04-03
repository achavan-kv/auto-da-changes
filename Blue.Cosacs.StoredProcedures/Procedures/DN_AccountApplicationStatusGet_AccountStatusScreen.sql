SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_AccountApplicationStatusGet_AccountStatusScreen]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_AccountApplicationStatusGet_AccountStatusScreen]
GO


--------------------------------------------------------------------------------
--
-- Project      : CoSACS .NET
-- File Name    : DN_AccountApplicationStatusGet_AccountStatusScreen.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Returns application status, used by stored procedure: DN_AccountStatusget.sql
-- Author       : Ilyas Parker
-- Date         : 16th May 2008
--
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 16/05/08  IP  Stored Proc creation
--------------------------------------------------------------------------------


/****** Object:  StoredProcedure [dbo].[DN_AccountApplicationStatusGet_AccountStatusScreen]    Script Date: 11/14/2007 15:30:56 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO




CREATE procedure [dbo].[DN_AccountApplicationStatusGet_AccountStatusScreen] 
@acctno varchar (12),
						       @return integer output
AS 
 SET NOCOUNT ON
	SET @return = 0

   	CREATE TABLE #applicationstatus (StatusCode varchar (4), StatusDescription varchar (128))

	DECLARE @accttype char (1)
	DECLARE @propresult char(1)
	DECLARE @currstatus char(1)
	DECLARE @reflresult char(1)
	DECLARE @instalpredel char(1)
	DECLARE @holdprop char(1)
	DECLARE @dateprop datetime
	DECLARE @delqty float
	DECLARE @quantity float
	DECLARE @cancount int
	DECLARE @linecount int
	DECLARE @paycount int
	DECLARE @continue int
	DECLARE @schedcount int
	DECLARE @delcount int
	DECLARE @agrmtno int
	DECLARE @instamount money
	DECLARE @amountpaid money
	DECLARE @price money
	DECLARE @deposit money
	DECLARE @termstype varchar(2)
	DECLARE @paymethod varchar(1)
	DECLARE @custid varchar(20)
	DECLARE @itemId int
	DECLARE @cheqdays int
	DECLARE @revcount int
	DECLARE @stocklocn int

	SET @accttype = ''
	SET @termstype = ''
	SET @paymethod = ''
	SET @custid = ''
	SET @propresult = ''
	SET @currstatus = ''
	SET @reflresult = ''
	SET @instalpredel = ''
	SET @holdprop = ''
	SET @itemId = 0
	SET @cancount = 0
	SET @instamount = 0
	SET @amountpaid = 0
	SET @linecount = 0
	SET @paycount = 0
	SET @quantity = 0
	SET @continue = 0
	SET @schedcount = 0
	SET @price = 0
	SET @delcount = 0
	SET @deposit = 0
	SET @agrmtno = 0
	SET @cheqdays = 0
	SET @revcount = 0
	SET	@stocklocn = 0

   SET NOCOUNT ON	
	SELECT	@accttype = A.accttype,
			@currstatus = A.currstatus,
			@termstype = A.termstype,
			@paymethod = AG.paymethod,
			@deposit = AG.deposit,
			@holdprop =  AG.holdprop,
			@custid = CA.custid
	FROM		acct A INNER JOIN
			agreement AG ON A.acctno = AG.acctno INNER JOIN
			custacct CA ON A.acctno = CA.acctno
	WHERE	A.acctno = @acctNo
	AND 		CA.hldorjnt = 'H'


	SELECT	top 1 @propresult = propresult,
		     	 @dateprop	  = dateprop	 	
	FROM		proposal
	WHERE	acctno = @acctno
	AND		custid = @custid
	ORDER BY 	dateprop desc

--Check if Details Required

	BEGIN
	        	insert into #applicationstatus  
	        	(
	            		StatusCode,
	            		StatusDescription
	        	)
	        	select	code,
	               	codedescript
	        	from   	code, proposalflag
		where  	proposalflag.acctno = @acctno
                          and        code.code =proposalflag.checktype
		and	category like 'PH%'
                          and datecleared is null
	END
	--Check if Details Required
	IF(@currstatus = '0')
	BEGIN
	        	insert into #applicationstatus  
	        	(
	            		StatusCode,
	            		StatusDescription
	        	)
	        	select	code,
	               	codedescript
	        	from   	code
		where  	code = 'REQ'
		and	category = 'APS'
	END

	--Check if Awaiting Sanction
	IF(@propresult = '' AND @acctType != 'S' AND @acctType != 'C')
	BEGIN
	        	insert into #applicationstatus  
	        	(
	            		StatusCode,
	            		StatusDescription
	        	)
	        	select	code,
	               	codedescript
	        	from   	code
		where  	code = 'SAN'
		and	category = 'APS'
	END

	--Check if Awaiting Referal
	IF(@propresult = 'R' AND @acctType != 'S' AND @acctType != 'C')
	BEGIN
	        	insert into #applicationstatus  
	        	(
	            		StatusCode,
	            		StatusDescription
	        	)
	        	select	code,
	               	codedescript
	        	from   	code
		where  	code = 'REF'
		and	category = 'APS'
	END

	--Check if Credit Refused
	IF(@propresult = 'X' AND @acctType != 'S' AND @acctType != 'C')
	BEGIN
	        	insert into #applicationstatus  
	        	(
	            		StatusCode,
	            		StatusDescription
	        	)
	        	select	code,
	               	codedescript
	        	from   	code
		where  	code = 'CRF'
		and	category = 'APS'
	END

	IF(@propresult = 'D' AND @acctType != 'S' AND @acctType != 'C')
	BEGIN
		SELECT	@reflresult = ReflResult
		FROM		REFERRAL
		WHERE	custid		= @custid
		AND		dateprop	= @dateprop;
		
		IF(@reflresult = 'D')
		BEGIN
			insert into #applicationstatus  
		        	(
		            		StatusCode,
		            		StatusDescription
		        	)
		        	select	code,

		               	codedescript
		        	from   	code
			where  	code = 'CRF'
			and	category = 'APS'
		END
	END

	SELECT	@instalpredel = instalpredel
	FROM 		termstype t, country c
	WHERE 	t.countrycode  = c.countrycode
	AND 		termstype    = @termstype

	IF(@instalpredel = 'Y')
	BEGIN
		SELECT TOP 1	@instamount = instalamount
		FROM 		INSTALPLAN
		WHERE 	acctno = @acctno
		ORDER BY 	datefirst desc

		IF(@instamount > 0)
			SET @deposit = @instamount
	END

	SELECT	@amountpaid = isnull(sum(transvalue),0)
	FROM 		fintrans
	WHERE	acctno = @acctno 
	AND	(transtypecode = 'PAY' 	or
		 transtypecode = 'COR' or
		 transtypecode = 'REF' or
		 transtypecode = 'RET' or
		 transtypecode = 'SCX' or
		 transtypecode = 'REB' or
		 (transtypecode = 'XFR'))

	select @cheqdays = cheqdays
	from country

	--Checking to see if a cheque payment has been made in the last 5 days
	SELECT	@paycount = COUNT(*) 
	FROM 		fintrans
	WHERE	acctno = @acctno 
	AND 		datetrans + @cheqdays > getdate()
	AND		(paymethod%10) = 2 

	--Check if Deposit/Instal pre-del not paid
	IF(0 - @amountpaid < @deposit AND @acctType != 'S' AND @acctType != 'C')
	BEGIN
		insert into #applicationstatus  
	        	(
	            		StatusCode,
	            		StatusDescription
	        	)
	        	select	code,
	               	codedescript
	        	from   	code
		where  	code = 'INP'
		and	category = 'APS'
	END

	--Check if Awaiting Cheque Clearance
	IF(@amountpaid >= @deposit AND @currstatus = 'U' AND @acctType != 'S' AND @acctType != 'C' AND @paycount > 0)
	BEGIN
	        	insert into #applicationstatus  
	        	(
	            		StatusCode,
	            		StatusDescription
	        	)
	        	select	code,
	               	codedescript
	        	from   	code
		where  	code = 'CHQ'
		and	category = 'APS'
	END

	--Check if Awaiting D.A
	IF(@holdprop = 'Y' AND @acctType != 'S')
	BEGIN
	        	insert into #applicationstatus  
	        	(
	            		StatusCode,
	            		StatusDescription
	        	)
	        	select	code,
	               	codedescript
	        	from   	code
		where  	code = 'ADA'
		and	category = 'APS'
	END

	--Check if Lineitems exist
        	SELECT	@linecount = COUNT(*)
       	 FROM 		LINEITEM L, STOCKITEM S
        	WHERE 	L.AcctNo    = @acctno
        	AND    		L.ItemId    = S.ItemId
        	AND    		L.StockLocn = S.StockLocn
        	AND    		iskit = 0

	IF(@linecount = 0 )
	BEGIN
	        	insert into #applicationstatus  
	        	(
	            		StatusCode,
	            		StatusDescription
	        	)
	        	select	code,
	               	codedescript
	        	from   	code
		where  	code = 'NOL'
		and	category = 'APS'
	END

	--Using a cursor to loop through Lineitems to see if any items are Awaiting Delivery Scheduling	
	DECLARE items_cursor CURSOR FOR 
	SELECT L.quantity, L.delqty, L.price, L.agrmtno, L.itemId, L.stocklocn
        	FROM 	LINEITEM L, STOCKITEM S
        	where 	L.AcctNo    = @acctno
        	AND    	L.ItemId   = S.ItemId
        	AND    	L.StockLocn = S.StockLocn
        	AND    	iskit = 0 and s.category not in (12,82) and s.itemtype !='N'
	
	OPEN items_cursor
	
	FETCH NEXT FROM items_cursor 
	INTO @quantity, @delqty, @price, @agrmtno, @itemId, @stocklocn
	
	WHILE @@FETCH_STATUS = 0 AND @continue = 0
	BEGIN
		-- uat51 86 rdb 14/11/07 reossesed accounts showing up with
		-- 'SCH' status, check delivery taable for a repossesion
		if not exists (select 1 from delivery where delorcoll = 'R' and acctno = @acctno and itemId = @itemId)
		begin
		IF(@quantity > @delqty AND @price != 0)
		BEGIN
		        	insert into #applicationstatus  
		        	(
		            		StatusCode,
		            		StatusDescription
		       	)
		        	select	code,
		               	codedescript
		       	 from   	code
			where  	code = 'SCH'
			and	category = 'APS'
			SET @continue = 1;
		END
		end
		
		IF EXISTS (	SELECT	1
					FROM	schedule
					WHERE	acctno = @acctno
					AND		agrmtno = @agrmtno
					AND		itemId = @itemId
					AND		stocklocn = @stocklocn
					AND		delorcoll = 'C')
		BEGIN
			insert into #applicationstatus  
			(
				StatusCode,
				StatusDescription
			)
		    select	code,
		            codedescript
		    from   	code
			where  	code = 'COL'
			and		category = 'APS'
			SET @continue = 1;
		END
		
		FETCH NEXT FROM items_cursor 

		INTO @quantity, @delqty, @price, @agrmtno, @itemId, @stocklocn
	END
   	CLOSE items_cursor
   	
   	--Using a cursor to check if any warranty renewals are awaiting delivery	
	DECLARE renewal_cursor CURSOR FOR 
	SELECT L.quantity, L.delqty, L.price, L.agrmtno, L.itemId
        	FROM 	LINEITEM L, STOCKITEM S, WARRANTYRENEWALPURCHASE W
        	where 	L.AcctNo    = @acctno
        	AND    	L.AcctNo    = W.acctno
        	AND    	L.ItemId    = W.ItemId
        	AND    	L.StockLocn = W.StockLocn
        	AND    	L.ItemId    = S.ItemId
        	AND    	L.StockLocn = S.StockLocn
        	AND    	S.IUPC != 'DT'
	
	OPEN renewal_cursor
	
	FETCH NEXT FROM renewal_cursor 
	INTO @quantity, @delqty, @price, @agrmtno, @itemId
	
	WHILE @@FETCH_STATUS = 0 AND @continue = 0
	BEGIN
		IF(@quantity > @delqty AND @price != 0)
		BEGIN
		        	insert into #applicationstatus  
		        	(
		            		StatusCode,
		            		StatusDescription
		       	)
		        	select	code,
		               	codedescript
		       	 from   	code
			where  	code = 'WRD'
			and	category = 'APS'
			SET @continue = 1;
		END
		
		FETCH NEXT FROM renewal_cursor 

		INTO @quantity, @delqty, @price, @agrmtno, @itemId
	END
   	CLOSE renewal_cursor
   	--DEALLOCATE renewal_cursor


	--Using a cursor to loop through Lineitems to see if Goods Delivered
	SET @continue = 0
	OPEN items_cursor
	
	FETCH NEXT FROM items_cursor 
	INTO @quantity, @delqty, @price, @agrmtno, @itemId, @stocklocn
	
	WHILE @@FETCH_STATUS = 0 AND @continue = 0
	BEGIN
		SELECT	@delcount = COUNT(*)
		FROM 		DELIVERY
		WHERE 	itemId = @itemId
		AND		acctno = @acctno
		AND		agrmtno = @agrmtno

		IF(@delcount > 0)
		BEGIN
		       	 insert into #applicationstatus  
		        	(
		            		StatusCode,
		            		StatusDescription
		       	)
		        	select	code,
		               	codedescript
		       	 from   	code
			where  	code = 'DEL'
			and	category = 'APS'
			SET @continue = 1
		END
		FETCH NEXT FROM items_cursor 
		INTO @quantity, @delqty, @price, @agrmtno, @itemId, @stocklocn
	END
   	CLOSE items_cursor
   	DEALLOCATE items_cursor
   	
   	OPEN renewal_cursor
	FETCH NEXT FROM renewal_cursor 
	INTO @quantity, @delqty, @price, @agrmtno, @itemId
	
	WHILE @@FETCH_STATUS = 0 AND @continue = 0
	BEGIN
		SELECT	@delcount = COUNT(*)
		FROM 		DELIVERY
		WHERE 	itemId = @itemId
		AND		acctno = @acctno
		AND		agrmtno = @agrmtno

		IF(@delcount > 0)
		BEGIN
			delete from #applicationstatus where StatusCode = 'WRD'
		    
		    insert into #applicationstatus  
		    (
				StatusCode,
		        StatusDescription
		    )
		    select	code,
		            codedescript
		    from   	code
			where  	code = 'DEL'
			and	category = 'APS'
			SET @continue = 1
		END
		FETCH NEXT FROM renewal_cursor 
		INTO @quantity, @delqty, @price, @agrmtno, @itemId
	END
   	CLOSE renewal_cursor
   	DEALLOCATE renewal_cursor

   SELECT	@schedcount = isnull(COUNT(*),0)
       	 FROM 		SCHEDULE S ,LINEITEM L
       	 WHERE 	L.acctno    = @acctno 
         AND L.ITEMID = S.ITEMID AND L.STOCKLOCN = S.STOCKLOCN and l.acctno = s.acctno
         and  picklistnumber =0
	--Check if Deliveries Scheduled
	IF(@schedcount > 0)
	BEGIN
	        	insert into #applicationstatus  
	       	 (
	            		StatusCode,
	           		 StatusDescription
	       	 )
	        	select	code,
	               	codedescript
	       	 from   	code
		where  	code = 'DAD'
		and	category = 'APS'
	END

	SELECT	@schedcount = isnull(COUNT(*),0)
    FROM 	SCHEDULE S ,LINEITEM L
    WHERE 	L.acctno    = @acctno 
    AND		L.ITEMID = S.ITEMID 
    AND		L.STOCKLOCN = S.STOCKLOCN 
    AND		l.acctno = s.acctno
    AND		L.deliveryprocess = 'S'
    AND		S.loadno =0 and picklistnumber !=0

	--Check if Deliveries Scheduled
	IF(@schedcount > 0)
	BEGIN
      	insert into #applicationstatus  
		(
      		StatusCode,
	        StatusDescription
	    )
	    select	code,
	           	codedescript
	    from   	code
		where  	code = 'PLP'
		and	category = 'APS'
	END
	
	SELECT	@schedcount = isnull(COUNT(*),0)
    FROM 	SCHEDULE S ,LINEITEM L
    WHERE 	L.acctno    = @acctno 
    AND		L.ITEMID = S.ITEMID 
    AND		L.STOCKLOCN = S.STOCKLOCN 
    AND		l.acctno = s.acctno
    AND		L.deliveryprocess = 'S'
    AND		s.transchedno !=0

	--Check if Deliveries Scheduled
	IF(@schedcount > 0)
	BEGIN
      	insert into #applicationstatus  
		(
      		StatusCode,
	        StatusDescription
	    )
	    select	code,
	           	codedescript
	    from   	code
		where  	code = 'TPP'
		and	category = 'APS'
	END

	SELECT	@schedcount = isnull(COUNT(*),0)
    FROM 	SCHEDULE S ,LINEITEM L
    WHERE 	L.acctno    = @acctno 
    AND		L.ITEMID = S.ITEMID 
    AND		L.STOCKLOCN = S.STOCKLOCN 
    AND		l.acctno = s.acctno
    AND		L.deliveryprocess = 'S'
    AND		S.loadno !=0

	--Check if Deliveries Scheduled
	IF(@schedcount > 0)
	BEGIN
		insert into #applicationstatus  
	    (
			StatusCode,
	        StatusDescription
		)
		select	code,
	            codedescript
		from   	code
		where  	code = 'DCH'
		and	category = 'APS'
	END

	SELECT	@schedcount = isnull(COUNT(*),0)
    FROM 	SCHEDULE S ,LINEITEM L
    WHERE 	L.acctno    = @acctno 
    AND		L.ITEMID = S.ITEMID AND L.STOCKLOCN = S.STOCKLOCN and l.acctno = s.acctno
    AND		L.deliveryprocess= 'I'
	
	--Check if Deliveries Scheduled
	IF(@schedcount > 0)
	BEGIN
		insert into #applicationstatus  
	    (
			StatusCode,
	        StatusDescription
	    )
		select	code,
	           	codedescript
	    from   	code
		where  	code = 'AWP'
		and		category = 'APS'
	END

	--IP - 28/04/09 - CR929 & 974 - Application Status 'RFL' (Removing From Load).
	--When a Delivery Note has been removed from a load.

	SELECT	 @schedcount = isnull(count(*),0)
	FROM	 lineitem l inner join schedule s
	ON		 l.acctno = s.acctno
	AND		 l.itemId = s.itemId
	AND		 l.stocklocn = s.stocklocn
	INNER JOIN scheduleremoval sr
	ON		 s.acctno = sr.acctno
	AND		 s.itemId = sr.itemId
	AND		 s.stocklocn = sr.stocklocn
	WHERE	 l.deliveryprocess = 'S'
	AND		 s.picklistnumber = 0
	AND		 s.loadno = 0
	AND		 l.acctno = @acctno
		
	IF(@schedcount > 0)
	BEGIN	
		insert into #applicationstatus  
	    (
			StatusCode,
	        StatusDescription
	    )
		select	code,
	           	codedescript
	    from   	code
		where  	code = 'RFL'
		and		category = 'APS'
	END

	--IP - 28/04/09 - CR929 & 974 - Application Status 'DNR' (Delivery Note Rescheduled/Reloaded).
	--When a Delivery Note previously removed has been rescheduled/reloaded.

	SELECT	 @schedcount = isnull(count(*),0)
	FROM	 lineitem l inner join schedule s
	ON		 l.acctno = s.acctno
	AND		 l.itemId = s.itemId
	AND		 l.stocklocn = s.stocklocn
	INNER JOIN scheduleremoval sr
	ON		 s.acctno = sr.acctno
	AND		 s.itemId = sr.itemId
	AND		 s.stocklocn = sr.stocklocn
	WHERE	 l.deliveryprocess = 'S'
	AND		 s.loadno != 0
	AND		 l.acctno = @acctno
		
	IF(@schedcount > 0)
	BEGIN	
		insert into #applicationstatus  
	    (
			StatusCode,
	        StatusDescription
	    )
		select	code,
	           	codedescript
	    from   	code
		where  	code = 'DNR'
		and		category = 'APS'
	END

	SELECT	@revcount = COUNT(*)
	FROM	reverse_cancellation
	WHERE	acctno = @acctno
	
	IF(@revcount > 0)
	BEGIN
		insert into #applicationstatus  
	        	(
	            		StatusCode,
	            		StatusDescription
	        	)
	        	select	code,
	             		codedescript
	        	from   	code
	       	where  	code = 'ROP'
	       	and	category = 'APS'
	END




	SELECT	@cancount = ISNULL( COUNT(*),0)
	FROM		CANCELLATION
	WHERE	acctno = @acctno
		
	--Check if account cancelled
	IF(@cancount > 0 AND @revcount=0)
	BEGIN

       delete from #applicationstatus
		insert into #applicationstatus  
	        	(
	            		StatusCode,
	            		StatusDescription
	        	)
	        	select	code,
	             		codedescript
	        	from   	code
	       	where  	code = 'CAN'
	       	and	category = 'APS'
	END

	--Check if cancellation has been reversed

    --Table created by stored procedure DN_AccountStatusget.sql, which then uses this table.
	INSERT INTO #StatusA (Acctno, Status)
		 SELECT @acctno, StatusDescription
		 FROM #applicationstatus

	--Drop temp table
	DROP TABLE #applicationstatus

	SET @return = @@error



