ALTER TABLE Admin.AdditionalUserProfile
 ADD CONSTRAINT
	FK_userprofile_user FOREIGN KEY
	(
		UserId
	) REFERENCES Admin.[User]
	(
		Id
	) ON UPDATE  NO ACTION ON DELETE  NO ACTION 
	
GO

ALTER TABLE Admin.AdditionalUserProfile
 ADD CONSTRAINT
	FK_userprofile_profile FOREIGN KEY
	(
		ProfileId
	) REFERENCES Admin.AddtionalProfile
	(
		Id
	) ON UPDATE  NO ACTION ON DELETE  NO ACTION 
	
GO