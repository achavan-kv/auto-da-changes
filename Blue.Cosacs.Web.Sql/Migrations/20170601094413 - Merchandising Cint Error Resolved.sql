ALTER TABLE Merchandising.CintError
ALTER COLUMN Resolved SMALLDATETIME
GO

ALTER TABLE Merchandising.CintsError
ALTER COLUMN Resolved SMALLDATETIME
GO

UPDATE e
SET Resolved = q.DispatchedOn
FROM  Merchandising.CintError e
LEFT JOIN Hub.QueueMessage q on e.MessageId = q.MessageId AND q.State = 'S'


UPDATE e
SET Resolved = q.DispatchedOn
FROM  Merchandising.CintsError e
LEFT JOIN Hub.QueueMessage q on e.MessageId = q.MessageId AND q.State = 'S'