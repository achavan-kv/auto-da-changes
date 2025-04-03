CREATE TABLE Service.TechnicianBooking
(
	Id INT IDENTITY(1,1),
	UserId INT NOT NULL,
	RequestId INT NOT NULL,
	[Date] SMALLDATETIME NOT NULL,
	Slot INT NOT NULL,
	SlotExtend INT NOT NULL,
	Reject BIT NOT NULL
)
GO

ALTER TABLE Service.TechnicianBooking
ADD CONSTRAINT PK_TechnicianBooking PRIMARY KEY (Id)
GO

ALTER TABLE Service.TechnicianBooking
ADD CONSTRAINT FK_TechnicianBooking_Request FOREIGN KEY (RequestId) REFERENCES Service.Request (Id)
GO

ALTER TABLE Service.TechnicianBooking
ADD CONSTRAINT FK_TechnicianBooking_Technician FOREIGN KEY (UserId) REFERENCES Service.Technician (UserId)
GO


CREATE NONCLUSTERED INDEX IX_UserId
    ON Service.TechnicianBooking (UserId)
    INCLUDE ([Date])
GO
