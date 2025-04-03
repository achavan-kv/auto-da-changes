/* To prevent any potential data loss issues, you should review this script in detail before running it outside the context of the database designer.*/

SET QUOTED_IDENTIFIER ON
SET ARITHABORT ON
SET NUMERIC_ROUNDABORT OFF
SET CONCAT_NULL_YIELDS_NULL ON
SET ANSI_NULLS ON
SET ANSI_PADDING ON
SET ANSI_WARNINGS ON

GO
CREATE TABLE Service.Tmp_Request
	(
	Id int NOT NULL IDENTITY (1, 1),
	CreatedOn smalldatetime NOT NULL,
	CreatedBy varchar(50) NOT NULL,
	Branch smallint NOT NULL,
	Type char(2) NOT NULL,
	RequestState int NOT NULL,
	InvoiceNumber varchar(50) NULL,
	CustomerId varchar(50) NULL,
	CustomerTitle varchar(25) NULL,
	CustomerFirstName varchar(50) NULL,
	CustomerLastName varchar(50) NULL,
	CustomerAddressLine1 varchar(50) NULL,
	CustomerAddressLine2 varchar(50) NULL,
	CustomerAddressLine3 varchar(50) NULL,
	CustomerPostcode varchar(10) NULL,
	CustomerNotes varchar(4000) NULL,
	ItemId varchar(25) NULL,
	ItemAmount money NULL,
	ItemSoldOn smalldatetime NULL,
	ItemSoldBy varchar(50) NULL,
	ItemDeliveredOn smalldatetime NULL,
	ItemStockLocation smallint NULL,
	Item varchar(100) NULL,
	ItemSupplier varchar(50) NULL,
	ItemSerialNumber varchar(50) NULL,
	WarrantyContractId int NULL,
	WarrantyLength smallint NULL,
	TransitNotes varchar(4000) NULL,
	Evaluation int NULL,
	EvaluationLocation int NULL,
	EvaluationAction int NULL,
	EstimateReceived smalldatetime NULL,
	EstimateLabourCost money NULL,
	EstimateAdditionalLabourCost money NULL,
	EstimateTransportCost money NULL,
	AllocationItemReceivedOn smalldatetime NULL,
	AllocationPartExpectOn smalldatetime NULL,
	AllocationZone int NULL,
	AllocationTechnician int NULL,
	AllocationServiceScheduledOn datetime NULL,
	AllocationInstructions varchar(4000) NULL,
	Resolution int NULL,
	ResolutionDate smalldatetime NULL,
	ResolutionSupplierToCharge int NULL,
	ResolutionCategory int NULL,
	ResolutionReport varchar(4000) NULL,
	FinalizedFailure int NULL,
	FinializeReturnDate smalldatetime NULL,
	Comments varchar(4000) NULL,
	CreatedById varchar(30) NULL,
	ResolutionLabourCost money NULL,
	ResolutionAdditionalCost money NULL,
	ResolutionTransportCost money NULL,
	LastUpdatedUser int NOT NULL,
	LastUpdatedUserName varchar(100) NOT NULL,
	LastUpdatedOn datetime NOT NULL
	)  ON [PRIMARY]
GO
ALTER TABLE Service.Tmp_Request SET (LOCK_ESCALATION = TABLE)
GO
SET IDENTITY_INSERT Service.Tmp_Request ON
GO
IF EXISTS(SELECT * FROM Service.Request)
	 EXEC('INSERT INTO Service.Tmp_Request (Id, CreatedOn, CreatedBy, Branch, Type, RequestState, InvoiceNumber, CustomerId, CustomerTitle, CustomerFirstName, CustomerLastName, CustomerAddressLine1, CustomerAddressLine2, CustomerAddressLine3, CustomerPostcode, CustomerNotes, ItemId, ItemAmount, ItemSoldOn, ItemSoldBy, ItemDeliveredOn, ItemStockLocation, Item, ItemSupplier, ItemSerialNumber, WarrantyContractId, WarrantyLength, TransitNotes, Evaluation, EvaluationLocation, EvaluationAction, EstimateReceived, EstimateLabourCost, EstimateAdditionalLabourCost, EstimateTransportCost, AllocationItemReceivedOn, AllocationPartExpectOn, AllocationZone, AllocationTechnician, AllocationServiceScheduledOn, AllocationInstructions, Resolution, ResolutionDate, ResolutionSupplierToCharge, ResolutionCategory, ResolutionReport, FinalizedFailure, FinializeReturnDate, Comments, CreatedById, ResolutionLabourCost, ResolutionAdditionalCost, ResolutionTransportCost, LastUpdatedUser, LastUpdatedUserName, LastUpdatedOn)
		SELECT Id, CreatedOn, CreatedBy, Branch, Type, RequestState, InvoiceNumber, CustomerId, CustomerTitle, CustomerFirstName, CustomerLastName, CustomerAddressLine1, CustomerAddressLine2, CustomerAddressLine3, CustomerPostcode, CustomerNotes, ItemId, ItemAmount, ItemSoldOn, ItemSoldBy, ItemDeliveredOn, ItemStockLocation, Item, ItemSupplier, ItemSerialNumber, WarrantyContractId, WarrantyLength, TransitNotes, Evaluation, EvaluationLocation, EvaluationAction, EstimateReceived, EstimateLabourCost, EstimateAdditionalLabourCost, EstimateTransportCost, AllocationItemReceivedOn, AllocationPartExpectOn, AllocationZone, AllocationTechnician, AllocationServiceScheduledOn, AllocationInstructions, Resolution, ResolutionDate, ResolutionSupplierToCharge, ResolutionCategory, ResolutionReport, FinalizedFailure, FinializeReturnDate, Comments, CreatedById, ResolutionLabourCost, ResolutionAdditionalCost, ResolutionTransportCost, LastUpdatedUser, LastUpdatedUserName, LastUpdatedOn FROM Service.Request WITH (HOLDLOCK TABLOCKX)')
GO
SET IDENTITY_INSERT Service.Tmp_Request OFF
GO
ALTER TABLE Service.RequestContact
	DROP CONSTRAINT FK_Contact_Request
GO
ALTER TABLE Service.RequestFoodLoss
	DROP CONSTRAINT FK_FoodLoss_Request
GO
ALTER TABLE Service.RequestPart
	DROP CONSTRAINT FK_Part_Request
GO

ALTER TABLE Service.RequestScriptAnswer
	DROP CONSTRAINT FK_RequestScript_Request
GO

DROP TABLE Service.Request
GO

EXECUTE sp_rename N'Service.Tmp_Request', N'Request', 'OBJECT' 
GO
ALTER TABLE Service.Request ADD CONSTRAINT
	PK_Request PRIMARY KEY CLUSTERED 
	(
	Id
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]


GO
ALTER TABLE Service.RequestPart ADD CONSTRAINT
	FK_Part_Request FOREIGN KEY
	(
	RequestId
	) REFERENCES Service.Request
	(
	Id
	) ON UPDATE  NO ACTION 
	 ON DELETE  CASCADE 
	
GO
ALTER TABLE Service.RequestPart SET (LOCK_ESCALATION = TABLE)

GO
ALTER TABLE Service.RequestFoodLoss ADD CONSTRAINT
	FK_FoodLoss_Request FOREIGN KEY
	(
	RequestId
	) REFERENCES Service.Request
	(
	Id
	) ON UPDATE  NO ACTION 
	 ON DELETE  CASCADE 
	
GO
ALTER TABLE Service.RequestFoodLoss SET (LOCK_ESCALATION = TABLE)

GO
ALTER TABLE Service.RequestContact ADD CONSTRAINT
	FK_Contact_Request FOREIGN KEY
	(
	RequestId
	) REFERENCES Service.Request
	(
	Id
	) ON UPDATE  NO ACTION 
	 ON DELETE  CASCADE 
	
GO
ALTER TABLE Service.RequestContact SET (LOCK_ESCALATION = TABLE)
GO

ALTER TABLE [Service].[RequestScriptAnswer]  WITH CHECK ADD  CONSTRAINT [FK_RequestScript_Request] FOREIGN KEY([RequestId])
REFERENCES [Service].[Request] ([Id])
ON DELETE CASCADE
GO

ALTER TABLE [Service].[RequestScriptAnswer] CHECK CONSTRAINT [FK_RequestScript_Request]
GO
