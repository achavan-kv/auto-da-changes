SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_ScheduleGetRevisedSchedulesSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_ScheduleGetRevisedSchedulesSP]
GO


CREATE PROCEDURE 	dbo.DN_ScheduleGetRevisedSchedulesSP
			@branchno smallint,
			@loadno int,
			@pickno int,
			@revisefrom datetime,
			@reviseto datetime,
			@user int,
			@timelocked datetime OUTPUT,	
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	SET @timelocked = GETDATE()
	SET @revisefrom = CONVERT(SMALLDATETIME, CONVERT(VARCHAR(10), @revisefrom, 105), 105)
	SET @reviseto = CONVERT(SMALLDATETIME, CONVERT(VARCHAR(10), @reviseto, 105), 105)
	
	SELECT	s.acctno, 
			o.buffno, 
			o.loadno, 
			t.truckid,
			o.picklistnumber,
			o.picklistbranch,
			o.transchedno, 
			o.transchednobranch,
			ISNULL(COUNT(o.buffno), 0) as totalitems
	INTO	#accts
	FROM	schedule s
			INNER JOIN order_removed o ON s.acctno = o.acctNo
										AND s.agrmtno = o.agrmtno
										AND s.itemID = o.itemID
										AND s.stocklocn = o.stocklocn
										AND	(@loadno = -5 OR o.loadno = @loadno)
										AND	(@pickno = -5 OR o.transchedno = @pickno)
										AND	o.loadno > 0
										AND	o.dateprinted IS NULL
			INNER JOIN deliveryload dl ON o.buffno = dl.buffno
										AND o.originallocation = dl.buffbranchno
										AND o.loadno = dl.loadno
			INNER JOIN transptsched t ON dl.branchno = t.branchno
										AND dl.datedel = t.datedel
										AND dl.loadno	= t.loadno		      	
	WHERE	CONVERT(DATETIME, CONVERT(VARCHAR(10), o.dateremoved, 105), 105) BETWEEN @revisefrom AND @reviseto
	AND		o.dateconfirmed IS NULL
	GROUP BY s.acctno, o.buffno, o.loadno, t.truckid, o.picklistnumber, o.picklistbranch,
			 o.transchedno, o.transchednobranch
	
	INSERT INTO #accts
	SELECT	o.acctno, 
			o.buffno, 
			o.loadno, 
			t.truckid,
			o.picklistnumber,
			o.picklistbranch,
			o.transchedno, 
			o.transchednobranch,
			1
	FROM	order_removed o
			INNER JOIN deliveryload dl ON o.buffno = dl.buffno
										AND o.originallocation = dl.buffbranchno
										AND o.loadno = dl.loadno
			INNER JOIN transptsched t ON dl.branchno = t.branchno
										AND dl.datedel = t.datedel
										AND dl.loadno	= t.loadno		      	
	WHERE	(@loadno = -5 OR o.loadno = @loadno)
	AND		(@pickno = -5 OR o.transchedno = @pickno)
	AND		o.loadno > 0
	AND		o.dateprinted IS NULL
	AND		CONVERT(DATETIME, CONVERT(VARCHAR(10), o.dateremoved, 105), 105) BETWEEN @revisefrom AND @reviseto
	AND		o.dateconfirmed IS NULL
	AND		NOT EXISTS(	SELECT 1
						FROM schedule s
						WHERE s.acctno = o.acctNo
						AND s.agrmtno = o.agrmtno
						AND s.itemID = o.itemID
						AND s.stocklocn = o.stocklocn)
	AND		NOT EXISTS(	SELECT 1
						FROM #accts a
						WHERE a.acctno = o.acctNo
						AND a.buffno = o.buffno)
	
	UPDATE	#accts 
	SET		totalitems = totalitems + (	SELECT COUNT(s.buffno)
										FROM	schedule s
										WHERE	#accts.acctno = s.acctno
										AND		#accts.buffno = s.buffno)    

	INSERT	INTO accountlocking (acctno, lockedby, lockedat, lockcount)
	SELECT	DISTINCT(acctno), @user, @timelocked, 1 
	FROM	#accts
	WHERE NOT EXISTS(SELECT	1 
					 FROM	accountlocking 
					 WHERE	accountlocking.acctno = #accts.acctno)

	SELECT	acctno, 
			buffno, 
			totalitems,
			loadno, 
			truckid,
			picklistnumber,
			picklistbranch,
			transchedno, 
			transchednobranch
	FROM	#accts
							
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

