ALTER TABLE service.Request
ALTER COLUMN Internal CHAR(1) NOT NULL
GO

sp_rename 'Service.Request.Internal','Type','Column'
GO