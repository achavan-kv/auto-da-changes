-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
IF EXISTS(
	SELECT 1 
	FROM sys.foreign_key_columns fkc
	INNER JOIN sys.columns c ON fkc.parent_column_id = c.column_id AND fkc.parent_object_id = c.object_id
	INNER JOIN sys.columns cref ON fkc.referenced_column_id = cref.column_id AND fkc.referenced_object_id = cref.object_id
	WHERE OBJECT_NAME(parent_object_id) = 'TechnicianBookingDelete' AND OBJECT_NAME(referenced_object_id) = 'Technician')
RETURN 
ELSE
BEGIN
	ALTER TABLE Service.Holiday ADD CONSTRAINT
		FK_Holiday_User FOREIGN KEY
		(
		UserId
		) REFERENCES Admin.[User]
		(
		Id
		) ON UPDATE  NO ACTION 
		 ON DELETE  NO ACTION 


	ALTER TABLE Service.TechnicianBookingDelete ADD CONSTRAINT
		FK_TechnicianBookingDelete_User FOREIGN KEY
		(
		UserId
		) REFERENCES Admin.[User]
		(
		Id
		) ON UPDATE  NO ACTION 
		 ON DELETE  NO ACTION 
	
	ALTER TABLE Service.TechnicianBookingDelete ADD CONSTRAINT
		FK_TechnicianBookingDelete_Technician FOREIGN KEY
		(
		TechincianId
		) REFERENCES Service.Technician
		(
		UserId
		) ON UPDATE  NO ACTION 
		 ON DELETE  NO ACTION 


	ALTER TABLE Service.TechnicianBookingReject ADD CONSTRAINT
		FK_TechnicianBookingReject_User FOREIGN KEY
		(
		UserId
		) REFERENCES Admin.[User]
		(
		Id
		) ON UPDATE  NO ACTION 
		 ON DELETE  NO ACTION 
END