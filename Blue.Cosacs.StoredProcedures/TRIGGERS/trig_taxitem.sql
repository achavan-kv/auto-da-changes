IF EXISTS (SELECT * FROM sysobjects WHERE NAME = 'trig_taxitem')
DROP TRIGGER trig_taxitem
GO 

GO 
CREATE TRIGGER trig_taxitem ON taxitem FOR INSERT, UPDATE, DELETE
AS DECLARE 
	@status INT 

	INSERT INTO taxitemhist (
		itemno,
		specialrate,
		datefrom,
		dateto,
		taxapplied,
		ItemId
	) 
	SELECT itemno,
		specialrate,
		GETDATE(),
		NULL ,
		taxapplied,
		ItemId
	FROM inserted 
	--WHERE NOT EXISTS (SELECT * FROM deleted WHERE deleted.itemno= INSERTed.itemno 
	WHERE NOT EXISTS (SELECT * FROM deleted WHERE deleted.ItemId= INSERTed.ItemId 
	AND deleted.specialrate = inserted.specialrate)
	SELECT @@ROWCOUNT AS numrows 
	
	UPDATE t
	SET dateto = GETDATE()
	FROM deleted d ,taxitemhist t 
	--WHERE t.itemno= d.itemno --AND t.datefrom = d.datefrom 
	WHERE t.ItemId= d.ItemId --AND t.datefrom = d.datefrom 
	--AND NOT EXISTS (SELECT * FROM inserted i WHERE d.itemno= i.itemno 
	AND NOT EXISTS (SELECT * FROM inserted i WHERE d.ItemId= i.ItemId 
	AND d.specialrate = i.specialrate)
	AND d.specialrate = t.specialrate 
	
GO 
