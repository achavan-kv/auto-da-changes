-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 

DELETE FROM Merchandising.CintError

ALTER TABLE Merchandising.CintError ADD MessageId INT NOT NULL

ALTER TABLE Merchandising.CintError  WITH CHECK ADD  CONSTRAINT [FK_CintError_Message] FOREIGN KEY([MessageId])
REFERENCES [Hub].[Message](Id)


ALTER TABLE Merchandising.CintError CHECK CONSTRAINT [FK_CintError_Message]