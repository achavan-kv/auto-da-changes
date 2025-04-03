
if exists (select * from sysobjects where name = 'trig_scheduleinsertupdate')
drop trigger trig_scheduleinsertupdate
go
--Do not need this as not sending schedule details to Oracle. 
/*(create trigger trig_scheduleinsertupdate
on schedule FOR update, INSERT  
as
IF EXISTS (SELECT * FROM CountryMaintenance WHERE CodeNAME = 'OracleLineExport' AND value IN ('F','P','L'))
BEGIN -- populate the export table for interface to Oracle.... 
  
	UPDATE l
	set
	quantity = li.quantity,
	ordval = li.ordval,
	buffno= i.buffno
	FROM LineitemOracleExport l,inserted i, lineitem li
	WHERE l.acctno= i.acctno AND l.agrmtno=i.agrmtno AND l.itemno = i.itemno
	AND l.stocklocn = i.stocklocn AND l.contractno = i.contractno AND l.runno= 0
	AND li.acctno = i.acctno AND li.itemno = i.itemno AND li.stocklocn = i.stocklocn AND li.agrmtno = i.agrmtno 
	AND (i.loadno >0 OR i.datetranschednoprinted IS NOT NULL OR i.dateprinted IS NOT NULL) -- AND l.buffno= i.buffno
	AND i.quantity > 0 -- Only for order schedules, not for collection schedules
	AND l.TYPE !='D' */
	-- insert those records where not already in the table for runno of zero
/*	INSERT INTO LineitemOracleExport (
		acctno,		agrmtno,
		itemno,		contractno,
		quantity,		stocklocn,
		ordval,		[type],
		runno,		buffno,
		orderlineno,		orderno) 
	SELECT l.acctno,		l.agrmtno,
		l.itemno,		l.contractno,
		l.quantity,		l.stocklocn,
		l.ordval,		'O',-- Order
		0,		i.buffno,
		l.orderlineno,		l.orderno
		FROM inserted i, lineitem l
		WHERE NOT EXISTS (SELECT * FROM LineitemOracleExport li WHERE 
		li.acctno= i.acctno AND li.agrmtno=i.agrmtno AND li.itemno = i.itemno
		AND li.stocklocn = i.stocklocn AND li.contractno = i.contractno AND i.buffno = li.buffno ) 
		AND l.acctno = i.acctno AND l.itemno = i.itemno AND i.quantity > 0 -- Only for order schedules, not for collection schedules
		AND l.stocklocn = i.stocklocn AND l.agrmtno = i.agrmtno AND l.contractno= i.contractno 
		AND (i.loadno >0 OR i.datetranschednoprinted IS NOT NULL OR i.dateprinted IS NOT NULL) 
		AND l.deliveryprocess = 'S'*/
	-- removing any order records which do not have a buffno if buffno >0 
		--DELETE FROM  lineitemoracleexport
		--WHERE TYPE ='O' AND runno= 0 AND buffno= 0 AND EXISTS 
		--(SELECT * FROM inserted d
		--WHERE d.acctno= lineitemoracleexport.acctno AND d.agrmtno = lineitemoracleexport.agrmtno
		--AND d.itemno = lineitemoracleexport.itemno AND d.stocklocn = lineitemoracleexport.stocklocn
		--AND d.contractno= lineitemoracleexport.contractno 
		--AND d.buffno >0)




--END

GO 