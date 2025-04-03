-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
/* To prevent any potential data loss issues, you should review this script in detail before running it outside the context of the database designer.*/
ALTER TABLE Service.Comment ADD CONSTRAINT
	FK_Comment_Request FOREIGN KEY
	(
		RequestId
	) REFERENCES Service.Request
	(
		Id
	) 
	ON UPDATE  NO ACTION 
	ON DELETE  NO ACTION 
GO
ALTER TABLE Service.Payment ADD CONSTRAINT
	FK_Payment_Request FOREIGN KEY
	(
		RequestId
	) REFERENCES Service.Request
	(
		Id
	) 
	ON UPDATE  NO ACTION 
	ON DELETE  NO ACTION 
	
GO
ALTER TABLE Service.TechnicianBooking ADD CONSTRAINT
	FK_TechnicianBooking_Request FOREIGN KEY
	(
		RequestId
	) REFERENCES Service.Request
	(
		Id
	) 
	ON UPDATE  NO ACTION 
	ON DELETE  NO ACTION 
	
GO
ALTER TABLE Service.TechnicianBookingReject ADD CONSTRAINT
	FK_TechnicianBookingReject_Request FOREIGN KEY
	(
		RequestId
	) REFERENCES Service.Request
	(
		Id
	) 
	ON UPDATE  NO ACTION 
	ON DELETE  NO ACTION 
