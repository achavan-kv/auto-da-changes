CREATE TABLE Service.TechnicianBookingDelete
(
	Id INT NOT NULL,
	RequestId INT NOT NULL,
	UserId INT NOT NULL,
	Date DATETIME NOT NULL,
	TechincianId INT NOT NULL,
	Reason VARCHAR(256) NOT NULL
)