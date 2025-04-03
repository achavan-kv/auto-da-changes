-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[Service].[FK_FoodLoss_Request]') AND parent_object_id = OBJECT_ID(N'[Service].[RequestFoodLoss]'))
ALTER TABLE [Service].[RequestFoodLoss] DROP CONSTRAINT [FK_FoodLoss_Request]
GO

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[Service].[FK_Part_Request]') AND parent_object_id = OBJECT_ID(N'[Service].[RequestPart]'))
ALTER TABLE [Service].[RequestPart] DROP CONSTRAINT [FK_Part_Request]
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[Service].[FK_Contact_Request]') AND parent_object_id = OBJECT_ID(N'[Service].[RequestContact]'))
ALTER TABLE [Service].[RequestContact] DROP CONSTRAINT [FK_Contact_Request]
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[Service].[FK_RequestScript_Request]') AND parent_object_id = OBJECT_ID(N'[Service].[RequestScriptAnswer]'))
ALTER TABLE [Service].[RequestScriptAnswer] DROP CONSTRAINT [FK_RequestScript_Request]
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[Service].[FK_TechnicianBooking_Request]') AND parent_object_id = OBJECT_ID(N'[Service].[TechnicianBooking]'))
ALTER TABLE [Service].[TechnicianBooking] DROP CONSTRAINT [FK_TechnicianBooking_Request]
GO



IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Service].[Request]') AND type in (N'U'))
DROP TABLE [Service].[Request]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO

CREATE TABLE [Service].[Request](
	[Id] [int] NOT NULL,
	[CreatedOn] [smalldatetime] NOT NULL,
	[CreatedBy] [varchar](50) NOT NULL,
	[Branch] [smallint] NOT NULL,
	[Type] [char](2) NOT NULL,
	[RequestState] [int] NOT NULL,
	[Account] [char](12) NULL,
	[InvoiceNumber] [varchar](50) NULL,
	[CustomerId] [varchar](50) NULL,
	[CustomerTitle] [varchar](25) NULL,
	[CustomerFirstName] [varchar](50) NULL,
	[CustomerLastName] [varchar](50) NULL,
	[CustomerAddressLine1] [varchar](50) NULL,
	[CustomerAddressLine2] [varchar](50) NULL,
	[CustomerAddressLine3] [varchar](50) NULL,
	[CustomerPostcode] [varchar](10) NULL,
	[CustomerNotes] [varchar](4000) NULL,
	[ItemId] [varchar](25) NULL,
	[ItemAmount] [money] NULL,
	[ItemSoldOn] [smalldatetime] NULL,
	[ItemSoldBy] [varchar](50) NULL,
	[ItemDeliveredOn] [smalldatetime] NULL,
	[ItemStockLocation] [smallint] NULL,
	[Item] [varchar](100) NULL,
	[ItemSupplier] [varchar](50) NULL,
	[ItemSerialNumber] [varchar](50) NULL,
	[WarrantyContractId] [int] NULL,
	[WarrantyLength] [smallint] NULL,
	[TransitNotes] [varchar](4000) NULL,
	[EvaluationClaimFoodLoss] [bit] NULL,
	[EvaluationLocation] [varchar](100) NULL,
	[EvaluationAction] [varchar](100) NULL,
	[EstimateReceived] [smalldatetime] NULL,
	[EstimateLabourCost] [money] NULL,
	[EstimateAdditionalLabourCost] [money] NULL,
	[EstimateTransportCost] [money] NULL,
	[AllocationItemReceivedOn] [smalldatetime] NULL,
	[AllocationPartExpectOn] [smalldatetime] NULL,
	[AllocationZone] [varchar](100) NULL,
	[AllocationTechnician] [int] NULL,
	[AllocationServiceScheduledOn] [datetime] NULL,
	[AllocationInstructions] [varchar](4000) NULL,
	[Resolution] [varchar](100) NULL,
	[ResolutionDate] [smalldatetime] NULL,
	[ResolutionSupplierToCharge] [varchar](100) NULL,
	[ResolutionCategory] [varchar](100) NULL,
	[ResolutionReport] [varchar](4000) NULL,	
	[ResolutionLabourCost] [money] NULL,
	[ResolutionAdditionalCost] [money] NULL,
	[ResolutionTransportCost] [money] NULL,
	[ResolutionPrimaryCharge] [varchar](100) NULL,
	[FinalisedFailure] [varchar](100) NULL,
	[FinaliseReturnDate] [smalldatetime] NULL,
	[Comments] [varchar](4000) NULL,
	[CreatedById] [varchar](30) NULL,
	[LastUpdatedUser] [int] NOT NULL,
	[LastUpdatedUserName] [varchar](100) NOT NULL,
	[LastUpdatedOn] [datetime] NOT NULL,
	
	
 CONSTRAINT [PK_Request] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [Service].[RequestFoodLoss]  WITH CHECK ADD  CONSTRAINT [FK_FoodLoss_Request] FOREIGN KEY([RequestId])
REFERENCES [Service].[Request] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [Service].[RequestFoodLoss] CHECK CONSTRAINT [FK_FoodLoss_Request]
GO

ALTER TABLE [Service].[RequestPart]  WITH CHECK ADD  CONSTRAINT [FK_Part_Request] FOREIGN KEY([RequestId])
REFERENCES [Service].[Request] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [Service].[RequestPart] CHECK CONSTRAINT [FK_Part_Request]
GO

ALTER TABLE [Service].[RequestContact]  WITH CHECK ADD  CONSTRAINT [FK_Contact_Request] FOREIGN KEY([RequestId])
REFERENCES [Service].[Request] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [Service].[RequestContact] CHECK CONSTRAINT [FK_Contact_Request]
GO

ALTER TABLE [Service].[RequestScriptAnswer]  WITH CHECK ADD  CONSTRAINT [FK_RequestScript_Request] FOREIGN KEY([RequestId])
REFERENCES [Service].[Request] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [Service].[RequestScriptAnswer] CHECK CONSTRAINT [FK_RequestScript_Request]
GO

ALTER TABLE [Service].[TechnicianBooking]  WITH CHECK ADD  CONSTRAINT [FK_TechnicianBooking_Request] FOREIGN KEY([RequestId])
REFERENCES [Service].[Request] ([Id])
GO
ALTER TABLE [Service].[TechnicianBooking] CHECK CONSTRAINT [FK_TechnicianBooking_Request]
GO

IF NOT EXISTS(select * from HiLo where sequence = 'Service.Request')
BEGIN
 INSERT INTO HiLo
 SELECT 'Service.Request', 1, 50
END

