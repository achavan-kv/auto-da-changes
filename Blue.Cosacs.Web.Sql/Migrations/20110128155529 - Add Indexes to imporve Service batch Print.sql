-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

--- add indexes for optimizing Service Batch Print
IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[SR_ChargeAcct]') AND name = N'IX_SR_ChargeAcct_AcctNo')
DROP INDEX [IX_SR_ChargeAcct_AcctNo] ON [dbo].[SR_ChargeAcct] WITH ( ONLINE = OFF )
GO

CREATE NONCLUSTERED INDEX [IX_SR_ChargeAcct_AcctNo] ON [dbo].[SR_ChargeAcct] 
(
	[AcctNo] ASC, ChargeType ASC
)
GO

IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[SR_ServiceRequest]') AND name = N'IX_SR_ServiceRequest_Status_Incl_ServiceRequestNo_CustId')
DROP INDEX IX_SR_ServiceRequest_Status_Incl_ServiceRequestNo_CustId ON [dbo].[SR_ServiceRequest] WITH ( ONLINE = OFF )
GO

CREATE NONCLUSTERED INDEX IX_SR_ServiceRequest_Status_Incl_ServiceRequestNo_CustId
ON [dbo].[SR_ServiceRequest] ([Status])
INCLUDE ([ServiceRequestNo],[CustId])
go
