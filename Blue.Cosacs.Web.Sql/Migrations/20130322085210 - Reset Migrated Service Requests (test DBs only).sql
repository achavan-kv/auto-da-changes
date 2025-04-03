-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

declare @row INT
	set @row = (select COUNT(*) from SR_ServiceRequest sr inner JOIN  Service.Request r on sr.ServiceRequestNo=r.id)
	if @row>0
	BEGIN
--select 'Deleting....... '
		delete  s from SR_Summary s 
			where exists(select * from SR_ServiceRequest sr  where sr.ServiceRequestNo=s.ServiceRequestNo)		
		delete  c from service.RequestContact c 
			where exists(select * from SR_ServiceRequest sr  where sr.ServiceRequestNo=c.requestid)
		delete  f from service.RequestFoodLoss f 
			where exists(select * from SR_ServiceRequest sr  where sr.ServiceRequestNo=f.requestid)
		delete  p from service.RequestPart p 
			where exists(select * from SR_ServiceRequest sr  where sr.ServiceRequestNo=p.requestid)
		delete  h from service.Charge h 
			where exists(select * from SR_ServiceRequest sr  where sr.ServiceRequestNo=h.requestid)
		delete  b from service.TechnicianBooking b
			where exists(select * from SR_ServiceRequest sr  where sr.ServiceRequestNo=b.requestid)
		delete  z from service.ZoneUser z
			where exists(select * from MergedTechnician mt   where mt.NewTechnicianId=z.UserId)
		delete  b2 from service.TechnicianBooking b2
			where exists(select * from MergedTechnician mt   where mt.NewTechnicianId=b2.UserId)	
		delete  t from service.Technician t
			where exists(select * from MergedTechnician mt  where mt.NewTechnicianId=t.UserId)
		delete  p from CourtsPersonTable p
			where exists(select * from MergedTechnician mt  where mt.NewTechnicianId=p.UserId)
		delete  a from Admin.AdditionalUserProfile a
			where exists(select * from MergedTechnician mt  where mt.NewTechnicianId=a.UserId)
		delete  u from admin.[User] u
			where exists(select * from MergedTechnician mt  where mt.NewTechnicianId=u.Id)		
		delete  c from service.Comment c
			where exists(select * from SR_ServiceRequest sr  where sr.ServiceRequestNo=c.requestid)
		delete  r from service.Request r 
			where exists(select * from SR_ServiceRequest sr  where sr.ServiceRequestNo=r.Id)
		
	END
	
