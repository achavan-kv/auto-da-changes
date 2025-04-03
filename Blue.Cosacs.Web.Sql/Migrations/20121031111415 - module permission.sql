ALTER TABLE Admin.AdditionalProfile
ADD Permission int
CONSTRAINT PermissionDefault DEFAULT 0 NOT NULL
GO
ALTER TABLE Admin.AdditionalProfile
DROP CONSTRAINT PermissionDefault 
GO


 
