ALTER TABLE [Merchandising].[CintError] DROP CONSTRAINT [PK_Merchandising_CintError] WITH ( ONLINE = OFF )
GO

ALTER TABLE Merchandising.CintError
DROP COLUMN ID
GO

ALTER TABLE Merchandising.CintError
ADD CONSTRAINT PK_CintError PRIMARY KEY CLUSTERED (MessageId)
GO

ALTER TABLE Merchandising.CintError
ADD Resolved BIT
GO

UPDATE e
SET Resolved = CASE WHEN q.state = 'S' THEN 1 ELSE 0 END
FROM  Merchandising.CintError e
LEFT JOIN Hub.QueueMessage q on e.MessageId = q.MessageId

ALTER TABLE Merchandising.CintError
ALTER COLUMN Resolved BIT NOT NULL
GO

ALTER TABLE Merchandising.CintsError
ADD Resolved BIT
GO

UPDATE e
SET Resolved = CASE WHEN q.state = 'S' THEN 1 ELSE 0 END
FROM  Merchandising.CintsError e
LEFT JOIN Hub.QueueMessage q on e.MessageId = q.MessageId

ALTER TABLE Merchandising.CintsError
ALTER COLUMN Resolved BIT NOT NULL
GO

