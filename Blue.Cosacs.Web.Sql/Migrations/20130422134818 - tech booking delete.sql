
/****** Object:  Index [u_techBookingRequestId]    Script Date: 22/04/2013 14:46:26 ******/
ALTER TABLE [Service].[TechnicianBooking] DROP CONSTRAINT [u_techBookingRequestId]
GO


ALTER TABLE [Service].[TechnicianBooking] DROP CONSTRAINT [FK_TechnicianBooking_Request]
GO


