disable trigger trig_lineiteminsert on LineItem
GO
disable trigger trig_lineiteminsertupdate on LineItem
GO
disable trigger trig_lineitemupdate on LineItem
GO


UPDATE lineitem
SET price = 0
WHERE price IS NULL
GO

ALTER TABLE lineitem
ALTER COLUMN price MONEY NOT NULL
GO

enable trigger trig_lineiteminsert on LineItem
GO
enable trigger trig_lineiteminsertupdate on LineItem
GO
enable trigger trig_lineitemupdate on LineItem
GO