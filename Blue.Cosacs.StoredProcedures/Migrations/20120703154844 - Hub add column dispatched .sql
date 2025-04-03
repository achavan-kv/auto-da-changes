ALTER TABLE hub.Message
ADD DispatchedOn DATETIME NULL
GO

UPDATE hub.Message
SET DispatchedOn = CreatedOn
WHERE State = 'S'
GO
