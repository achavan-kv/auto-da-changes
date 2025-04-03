-- Trigger for Audit of Stockinfo table

IF EXISTS (SELECT * FROM sysobjects WHERE NAME= 'Trig_SR_Allocation_InsertUpdate')
DROP TRIGGER Trig_SR_Allocation_InsertUpdate
GO 

CREATE Trigger [dbo].[Trig_SR_Allocation_InsertUpdate] ON [dbo].[SR_Allocation]
For UPDATE, INSERT

AS

INSERT INTO SR_AllocationAudit(ServiceRequestNo, DateAllocated, Zone, TechnicianId, 
			PartsDate, RepairDate, IsAM, Instructions, allocatedBy, ReAssignCode, DateChange, ReAssignedBy)
	Select ServiceRequestNo, DateAllocated, Zone, TechnicianId, 
			PartsDate, RepairDate, IsAM, Instructions, allocatedBy, ReAssignCode, GETDATE(), ReAssignedBy
	From INSERTED I
	Where DateAllocated!=''
	

-- End End End End End End End End End End End End End End End End End End End End End End End End