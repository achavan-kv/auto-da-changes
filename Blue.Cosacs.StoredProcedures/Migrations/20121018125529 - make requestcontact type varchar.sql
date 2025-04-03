ALTER TABLE Service.RequestContact
ALTER COLUMN TypeId VARCHAR(20) NOT NULL
GO

sp_rename 'service.RequestContact.TypeId','Type','Column'
GO