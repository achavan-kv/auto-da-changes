IF NOT EXISTS (SELECT * FROM syscolumns
			   WHERE name = 'Taxrate'
               AND OBJECT_NAME(id) = 'lineitem')
BEGIN
  ALTER TABLE Lineitem
  ADD taxrate FLOAT
END
GO

UPDATE H
SET dateto = ISNULL(h2.datefrom,'1900-01-01')
FROM taxratehistory H,taxratehistory H2
WHERE h2.datefrom = (SELECT MIN(datefrom) 
	  			     FROM taxratehistory h3
				     WHERE h3.datefrom > H.datefrom)
				   

-- Fix grenada issue.
IF (SELECT countrycode FROM country) = 'G'
BEGIN
DELETE FROM taxratehistory
WHERE dateto = '1900-01-01'
AND taxrate != (SELECT taxrate FROM country)
END           
                
-- Check tax rate.
IF ((SELECT top 1 taxrate
    FROM taxratehistory
    WHERE datefrom = (SELECT MAX(datefrom) 
					  FROM taxratehistory H)) != (SELECT taxrate 
												  FROM country))
BEGIN
RAISERROR ('Country tax rate not correct in history. Contact support',16,1)
RETURN
END


IF EXISTS (SELECT * FROM sysobjects
		   WHERE xtype = 'TR'
		   AND name = 'trig_lineiteminsert')
BEGIN
	DISABLE TRIGGER trig_lineiteminsert ON lineitem
END


IF EXISTS (SELECT * FROM sysobjects
		   WHERE xtype = 'TR'
		   AND name = 'trig_lineiteminsertupdate')
BEGIN
	DISABLE TRIGGER trig_lineiteminsertupdate ON lineitem
END

IF EXISTS (SELECT * FROM sysobjects
		   WHERE xtype = 'TR'
		   AND name = 'trig_lineitemupdate')
BEGIN
	DISABLE TRIGGER trig_lineitemupdate ON lineitem
END

-- Update line items.
UPDATE l
SET taxrate = i.taxrate 
FROM lineitem l,taxratehistory i
WHERE EXISTS (SELECT * 
			  FROM delivery 
			  WHERE delivery.itemno = l.itemno
			  AND delivery.stocklocn = l.stocklocn
			  AND delivery.acctno = l.acctno
			  AND delivery.datetrans > datefrom
			  AND delivery.agrmtno = l.agrmtno
			  AND delivery.contractno = l.contractno
			  AND (delivery.datetrans <= dateto OR dateto < '1900-01-02')
			  AND datetrans > '2008-01-01')
			  

 
-- Set specific lineitems.			  
UPDATE lineitem
SET taxrate = specialrate
FROM taxitem
WHERE lineitem.itemno = taxitem.ItemNo


IF EXISTS (SELECT * FROM sysobjects
		   WHERE xtype = 'TR'
		   AND name = 'trig_lineiteminsert')
BEGIN
	ENABLE TRIGGER trig_lineiteminsert ON lineitem
END

IF EXISTS (SELECT * FROM sysobjects
		   WHERE xtype = 'TR'
		   AND name = 'trig_lineiteminsertupdate')
BEGIN
	ENABLE TRIGGER trig_lineiteminsertupdate ON lineitem
END

IF EXISTS (SELECT * FROM sysobjects
		   WHERE xtype = 'TR'
		   AND name = 'trig_lineitemupdate')
BEGIN
	ENABLE TRIGGER trig_lineitemupdate ON lineitem
END
