-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
CREATE TABLE Merchandising.CintOrderStats (
	Id int IDENTITY(1,1) NOT NULL,
	Reference varchar(50) NOT NULL,
	QtyOrdered int NOT NULL DEFAULT(0),
	QtyDelivered int NOT NULL DEFAULT(0),
	QtyReturned int NOT NULL DEFAULT(0),
	QtyRepossessed int NOT NULL DEFAULT(0),
	CONSTRAINT[pk_CintOrderStats] PRIMARY KEY CLUSTERED ([Id]))