CREATE TABLE Service.LabourCostMatrix
	(
	Id int identity(1,1) NOT NULL,
	Type varchar(50) NOT NULL,
	InternalTechnician money NOT NULL,
	ContractedTechnician money NOT NULL,
	Cost money NOT NULL,
	EWClaim money NOT NULL,
	CustomerCharge money NOT NULL,
	Level_1 varchar(50) SPARSE NULL,
	Level_2 varchar(50) SPARSE NULL,
	Level_3 varchar(50) SPARSE NULL,
	Level_4 varchar(50) SPARSE NULL,
	Level_5 varchar(50) SPARSE NULL,
	Level_6 varchar(50) SPARSE NULL,
	Level_7 varchar(50) SPARSE NULL,
	Level_8 varchar(50) SPARSE NULL,
	Level_9 varchar(50) SPARSE NULL,
	Level_10 varchar(50) SPARSE NULL,
	ItemList varchar(4000) SPARSE NULL
	)  ON [PRIMARY]
GO
ALTER TABLE Service.LabourCostMatrix ADD CONSTRAINT
	PK_Id_LabourCostMatrix PRIMARY KEY CLUSTERED 
	(
	Id
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
ALTER TABLE Service.LabourCostMatrix SET (LOCK_ESCALATION = TABLE)
GO
