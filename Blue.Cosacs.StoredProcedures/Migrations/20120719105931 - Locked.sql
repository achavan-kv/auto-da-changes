ALTER TABLE Admin.[User] 
ADD	Locked bit NOT NULL CONSTRAINT DF_User_Lock DEFAULT 0
GO
ALTER TABLE Admin.[User] SET (LOCK_ESCALATION = TABLE)
GO