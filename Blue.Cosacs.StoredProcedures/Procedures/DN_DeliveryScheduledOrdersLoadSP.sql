--SET QUOTED_IDENTIFIER OFF 
--GO
--SET ANSI_NULLS ON 
--GO

--if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_DeliveryScheduledOrdersLoadSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
--drop procedure [dbo].[DN_DeliveryScheduledOrdersLoadSP]
--GO

--CREATE PROCEDURE dbo.DN_DeliveryScheduledOrdersLoadSP 
--				@orderfrom datetime, --  default this to 3 months ago
--				@orderto datetime, -- default this to today + 23:59
--				@deliveryarea VARCHAR(12),-- can be 'ALL'
--				@includedeliveries bit,-- 1 for bring back deliveries
--				@includecollections bit, -- 1 for bring back collections
--				@majorcategory VARCHAR(12), -- can be 'ALL'
--				@minorcategory VARCHAR(6), -- can be  'ALL'
--				@acctno VARCHAR(13), -- can be '' or '000000000000'
--				@user int, --current user
--				@branch int,  -- stock location for printing ??
--				@delnotebranch int,
--				@truckid VARCHAR(26),
--				@includeassembly bit,
--				@includenonassembly bit,
--				@TimeLocked DATETIME OUTPUT,  --
--				@return INT OUTPUT 
--AS
--    SELECT @return = 0

--	SELECT @TimeLocked = GetDate()

--	DECLARE @deltype1 char(1), @deltype2 char(1), @deltype3 char(1), @deltype4 char(1)
	
--	set @deltype1 = ''
--	set @deltype2 = ''
--	set @deltype3 = ''
--	set @deltype4 = ''

--	IF @deliveryarea ='ALL'
--		SET @deliveryarea ='%'
--	ELSE
--		SET @deliveryarea =@deliveryarea + '%'
		
--	IF @truckid = 'ALL'
--		SET @truckid ='%'
--	ELSE
--		SET @truckid = @truckid + '%'

--	SET @orderto = dateadd(hour,23,@orderto)
--	SET @orderto = dateadd(minute,59,@orderto)
--	SET @orderto = dateadd(second,59,@orderto)

--	-- restricting by category
--	-- Major categories:PCF furniture categories PCE electrical categories PCD delivery categories
--	-- Minor categories - if PCE or PCF 2 digit categories
--	-- Minor categories if PCD then itemno begins with this

--	DECLARE @mincategory smallint, @maxcategory smallint, @itemnolike varchar(10)
     
--	SET @mincategory = 0
--	SET @maxcategory = 1000
--	SET @itemnolike= '%'

--	IF @majorcategory = ''
--		SET @majorcategory = 'ALL'

--	IF @majorcategory = 'ALL'
--		SET @itemnolike= '%'

--	IF @majorcategory != 'PCD' AND @minorcategory != 'ALL'
--	BEGIN
--		SET @mincategory = @minorcategory 
--		SET @maxcategory = @minorcategory 
--	END

--	IF @majorcategory = 'PCD' AND @minorcategory != 'ALL'
--		SET @itemnolike = @minorcategory + '%'

--    IF @deliveryarea = ''
--       SET @deliveryarea = '%'
   
--    IF @includedeliveries = 1
--    BEGIN 
--		SET @deltype1 = 'D'
--		SET @deltype2 = ''
--    END
    
--	IF @includecollections = 1
--    BEGIN
--		SET @deltype3 = 'C'
--		SET @deltype4 = 'R'
--    END

--	IF (ISNULL(@acctno,'') = '' OR @acctno = '000000000000')
--		SELECT @acctno = '%'
--	ELSE
--		SELECT @acctno = @acctno + '%'
	
--	IF (@return = 0)
--	BEGIN	
--		if exists (select * from AccountLocking 
--				   WHERE	LockCount <= 0)
--		BEGIN
--			DELETE 
--			FROM	AccountLocking 
--			WHERE	LockCount = 0
--		END 
--		-- Lock accounts that have lineitems to be delivered AND that are not locked
--		INSERT INTO accountlocking (acctno, lockedby, lockedat, lockcount)
--		SELECT DISTINCT(l.acctno), @user, @TimeLocked, 1
--		FROM	lineitem l	
--				INNER JOIN schedule d ON l.acctno = d.acctno 
--									  AND l.ItemID = d.ItemID 
--									  AND l.stocklocn = d.stocklocn
--				INNER JOIN acct ON d.acctno = acct.acctno
--				INNER JOIN agreement g ON g.acctno = d.acctno 
--									   AND g.agrmtno =d.agrmtno
--				INNER JOIN stockitem s ON d.ItemID = s.ItemID 
--									   AND d.stocklocn = s.stocklocn
--				INNER JOIN deliveryload dl ON d.buffno = dl.buffno
--										  AND d.stocklocn = dl.buffbranchno
--										  AND d.loadno = dl.loadno
--				INNER JOIN transptsched t ON dl.branchno = t.branchno
--										  AND dl.datedel = t.datedel
--										  AND dl.loadno	= t.loadno		      	
--				LEFT OUTER JOIN accountlocking a ON l.acctno = a.acctno
--		WHERE	d.acctno LIKE @acctno
--        AND		D.quantity !=0
--		AND		(@branch = (CASE WHEN ISNULL(d.retstocklocn,0) = 0 THEN d.stocklocn ELSE d.retstocklocn END) OR @branch = -5)
--		AND		(l.delnotebranch = @delnotebranch OR @delnotebranch = -5)
--		AND		acct.currstatus not in ('0', 'U', 'S') 
--		AND		s.IUPC not in ('DT','STAX') 	/*will be removing all non-stock items later*/     
--		AND		(l.quantity != 0 OR (l.quantity = 0 AND d.quantity < 0))  
--		AND		l.Iskit = 0
--		AND		s.itemtype != 'N'
--  		AND		G.holdprop = 'N'
--		AND		NOT EXISTS (SELECT c.AcctNo FROM Cancellation c WHERE c.AcctNo = d.AcctNo)
--		AND		l.deliveryarea like @deliveryarea
--		AND		s.itemno like @itemnolike
--		AND		l.datereqdel BETWEEN @orderfrom AND @orderto
--		AND		d.delorcoll IN (@deltype1, @deltype2, @deltype3, @deltype4)
--		AND		NOT EXISTS (SELECT * FROM ACCOUNTLOCKING A WHERE A.LOCKEDBY = @USER AND A.ACCTNO = L.ACCTNO)
--		AND		l.deliveryprocess = 'S'
--		AND		t.truckid like @truckid
--		--AND		s.category NOT IN(12,82,36,37,38,39,46,47,48,86,87,88)
--		AND		s.category NOT IN(select distinct CODE from code where category in ('WAR', 'PCDIS'))
--		AND		d.dateprinted is null 
		
--		IF @@error = 0	
--		BEGIN
--			-- Fetch address type from lineitem table
--			-- TODO - Include Stock Status once CR556 has been done
--	        SELECT DISTINCT l.deliveryarea,
--							al.acctno, 
--							ISNULL(l.deliveryaddress, '') as addtype, 
--        					ca.custid, 
--							s.IUPC AS itemno, 
--							l.quantity, 
--	       					l.delqty, 
--							l.notes as itemnotes, 
--							CASE WHEN ISNULL(d.retstocklocn,0) = 0 THEN d.stocklocn ELSE d.retstocklocn END as stocklocn,
--							ISNULL(l.delnotebranch, 0) as delnotebranch, 
--							l.price, 
--							l.ordval,
--							l.datereqdel, 
--							l.timereqdel, 
--							l.dateplANDel, 
--							ag.empeenosale,
--							sp.FullName as EmployeeName,
--							ag.dateagrmt, 
--							cp.FullName , 
--							ISNULL(l.deliveryaddress, '') as deliveryaddress,
--							convert(varchar,s.category) as category,
--							d.buffno,
--							d.undeliveredflag,
--							d.buffbranchno,
--							convert(varchar(20),d.delorcoll) as DelOrColl,
--							s.itemdescr1,
--							s.itemdescr2,
--							ISNULL(d.transchedno,0) as picklistnumber,
--							ISNULL(d.loadno,0) as loadno,
--							s.stockactual,
--							s.stockonorder,
--							'  ' as stockstatus,
--							convert(bit,0) as released,
--							d.quantity as scheduledqty,
--							ISNULL(l.assemblyrequired, 'N') as assemblyrequired,
--							ISNULL(l.damaged, 'N') as damaged,
--							t.truckid,
--							tr.drivername,
--							l.ItemID
--			INTO	#lines
--			FROM 	accountlocking al 
--					INNER JOIN lineitem l ON al.acctno = l.acctno 
--					INNER JOIN agreement ag ON al.acctno = ag.acctno 
--					INNER JOIN custacct ca ON l.acctno = ca.acctno
--					INNER JOIN schedule d ON l.acctno = d.acctno 
--										  AND l.ItemID = d.ItemID AND l.stocklocn = d.stocklocn
--					INNER JOIN stockitem s ON s.itemID = l.ItemID
--										   AND l.stocklocn = s.stocklocn 
--					INNER JOIN Admin.[User] cp ON ag.empeenochange = cp.Id
--					INNER JOIN Admin.[User] sp ON ag.empeenosale = sp.Id
--					INNER JOIN deliveryload dl ON d.buffno = dl.buffno
--											   AND d.stocklocn = dl.buffbranchno
--											   AND d.loadno = dl.loadno
--					INNER JOIN transptsched t ON dl.branchno = t.branchno
--											  AND dl.datedel = t.datedel
--											  AND dl.loadno	= t.loadno	
--					INNER JOIN transport tr ON t.truckid = tr.truckid
--			WHERE	al.lockedby = @user
--			AND		ca.hldorjnt = 'H'
--			AND		s.itemtype != 'N'	--KEF changed to look at stockitem not lineitem as can't rely on lineitem.itemtype column being correct
--			AND		l.Iskit = 0
--	    	AND		(@branch = (CASE WHEN ISNULL(d.retstocklocn,0) = 0 THEN d.stocklocn ELSE d.retstocklocn END) OR @branch = -5)
--			AND		(l.delnotebranch = @delnotebranch OR @delnotebranch = -5)
--			AND		l.deliveryarea like @deliveryarea
--			AND		s.itemno like @itemnolike
--			AND		s.category BETWEEN @mincategory AND @maxcategory
--			AND		d.delorcoll IN (@deltype1, @deltype2, @deltype3, @deltype4)
--			AND		l.datereqdel BETWEEN @orderfrom AND @orderto
--			AND		al.acctno like @acctno
--			AND		l.deliveryprocess = 'S'
--			AND		t.truckid like @truckid
--			AND		d.dateprinted is null 
			
--			SELECT @return = @@error
--		END

--        -- If we are including 'linked' items we need to get all other items with the same
--        -- buffno (delivery note number) as those already selected that were previously excluded
--        -- by the selection criteria.
			
--		IF @@error = 0	
--		BEGIN
--			-- Catch any dodgy address types	
--			UPDATE 	#lines
--			SET		addtype = 'H'
--			WHERE 	NOT EXISTS (SELECT custid 
--								FROM   custaddress 
--								WHERE  custaddress.custid = #lines.custid
--         						AND	   custaddress.addtype = #lines.addtype 
--								AND    custaddress.datemoved is null )
--			SELECT 	@return = @@error
--		END 
 
--       	IF @@error = 0	
--		BEGIN
--			DELETE 
--			FROM	#lines 
--			WHERE	EXISTS(	SELECT * 
--							FROM	cancellation 
--							WHERE	cancellation.acctno = #lines.acctno)
--	    END
        
--        IF @@error = 0	
--   		BEGIN
--			IF @majorcategory != 'ALL' AND @minorcategory = 'ALL'
--			BEGIN -- remove items not in this category from the temporary table			
--				DELETE 
--				FROM	#lines 
--				WHERE	convert(varchar,#lines.category) NOT IN 
--														(SELECT	code 
--														 FROM	code 
--														 WHERE	category = @majorcategory)
--			END
--		END
		
--		IF @@error = 0
--		BEGIN
--			UPDATE	#lines
--			SET		stockactual = stockactual - ISNULL((select sum(quantity)
--			FROM	schedule where schedule.ItemID = #lines.ItemID 
--			AND		schedule.stocklocn = #lines.stocklocn
--			AND		(schedule.dateprinted IS NOT NULL or schedule.datePicklistPrinted IS NOT NULL)),0)
--	    END 
	    
--	    IF @@error = 0
--		BEGIN
--			UPDATE	#lines
--			SET		stockstatus = 'IS',
--					released = convert(bit,1)
--			WHERE	stockactual > 0
--		END
			
--		IF @@error = 0
--		BEGIN
--			UPDATE	#lines
--			SET		stockstatus = 'IS',
--					released = convert(bit,1)
--			WHERE	stockactual > 0
--		END

--		--collections and repossessions should always have released checked */
--		IF @@error = 0
--		BEGIN
--			IF @includecollections = 1 --only do if collections box is checked
--			BEGIN
--				UPDATE	#lines
--				SET		released = convert(bit,1)
--				WHERE	delorcoll IN (@deltype3, @deltype4)
--			END	
--		END
		
--		IF @@error = 0
--		BEGIN
--			UPDATE	#lines
--			SET		released = convert(bit,0)
--			WHERE	picklistnumber > 0
--		END

--		IF @@error = 0
--		BEGIN
--			UPDATE	#lines
--			SET		stockstatus = 'OR'
--			WHERE	stockstatus = '  ' AND stockonorder > 0
--		END

--		IF @@error = 0
--		BEGIN
--			UPDATE	#lines
--			SET		stockstatus = 'NS'
--			WHERE	stockactual <= 0 AND stockonorder <= 0
--		END
--	END		

--	IF @@error = 0	
--		BEGIN

--            -- Have to use CREATE TABLE instead of SELECT INTO because the 
--            -- compiler won't allow SELECT INTO the same #table twice even
--            -- though they are in an IF-ELSE.

--            CREATE TABLE #PrivClub
--                (CustId     VARCHAR(20) NOT NULL,
--                 CodeDesc   VARCHAR(64) NOT NULL)

--            IF EXISTS (SELECT 1 FROM CountryMaintenance
--                       WHERE  CodeName = 'TierPCEnabled' AND Value = 'True')
--            BEGIN
--                -- Tier1/2 Privilege club is enabled
                
--                INSERT INTO #PrivCLub
--                    (CustId, CodeDesc)
--                SELECT  c.custid, cd.CodeDescript
--                FROM    custcatcode c, Code cd
--                WHERE   exists (select custid from #lines l where l.custid = c.custid)
--                AND     c.Code IN ('TIR1', 'TIR2')
--                AND     ISNULL(c.DateDeleted,'') = ''
--                AND     cd.Category = 'CC1'
--                AND     cd.Code = c.Code
--            END
--            ELSE
--            BEGIN
--                -- Classic Privilege Club

--                /* KEF added temp table to find custcatcode for priviledge club as can have more than 1 record for a customer */
--                INSERT INTO #PrivCLub
--                    (CustId, CodeDesc)
--                select  c.custid, cd.CodeDescript
--                from    custcatcode c, Code cd
--                where   exists (select custid from #lines l where l.custid = c.custid)
--                and     c.code = 'CLAC'
--                AND     ISNULL(c.DateDeleted,'') = ''
--                and not exists (select custid from custcatcode c2
--                                where c2.custid = c.custid and c2.code in ('CLAS','CLAW')
--                                AND   ISNULL(c2.DateDeleted,'') = '')
--                AND     cd.Category = 'CC1'
--                AND     cd.Code = c.Code
--            END


--			SELECT DISTINCT	l.deliveryarea,
--							SUBSTRING(l.acctno,1,3)+N'-'+SUBSTRING(l.acctno,4,4)+N'-'+SUBSTRING(l.acctno,8,4)+N'-'+SUBSTRING(l.acctno,12,1) as acctno,
--							c.title, 
--							c.firstname, 
--							c.name, 
--							cad.cusaddr1, 
--							cad.cusaddr2, 
--							cad.cusaddr3, 
--							cad.cuspocode, 
--							ISNULL(codecat.catdescript,'Unknown') + ' - ' + ISNULL(code.codedescript,'Unknown') as codedescript,
--							l.buffno,
--							l.undeliveredflag,
--							l.datereqdel, 
--							l.timereqdel, 
--							ISNULL(#privclub.CodeDesc,'No') as PrivilegeClub,
--							l.dateagrmt,
--							l.itemdescr1,
--							l.itemdescr2,
--							l.empeenosale,
--							l.empeename,
--							l.DelOrColl,
--							released,
--							' ' as picked,
--							l.deliveryaddress, 
--							l.buffbranchno,
--							convert(varchar(300), 
--							cad.notes) as cusnotes, 
--							l.empname AS EmployeeName,
--							l.stocklocn,
--							l.delnotebranch,
--							l.picklistnumber,
--							l.itemno,
--							stockstatus,
--							l.stockactual as StockAvailable,
--							scheduledqty,
--							l.assemblyrequired,
--							l.damaged,
--							l.itemnotes,
--							l.truckid,
--							l.drivername,
--							l.loadno,
--							l.itemId
--			FROM	#lines l
--					INNER JOIN customer c ON l.custid = c.custid
--					INNER JOIN custaddress cad ON l.custid = cad.custid 
--											   AND l.addtype = cad.addtype
--					LEFT OUTER JOIN code ON code.code = l.category 
--										 AND code.category IN ('PCD','PCE','PCF','PCO','PCW')
--					LEFT OUTER JOIN codecat ON code.category = codecat.category
--					LEFT OUTER JOIN #privclub ON #privclub.custid = c.custid
--			WHERE	ISNULL(cad.datemoved,'1-January-1900') = '1-January-1900'
--       		ORDER BY c.name
 
--			SELECT 	@return = @@error
--		END
   
--	SET ROWCOUNT 0
--GO
--SET QUOTED_IDENTIFIER OFF 
--GO
--SET ANSI_NULLS ON 
--GO

