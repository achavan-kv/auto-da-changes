-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
/****** Object:  View [Service].[TechnicianUser]    Script Date: 26-10-2018 4.49.57 PM ******/
IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[Service].[TechnicianUser]'))
DROP VIEW [Service].[TechnicianUser]
GO

/****** Object:  View [Service].[TechnicianUser]    Script Date: 26-10-2018 4.49.57 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE VIEW [Service].[TechnicianUser]
AS
SELECT t.*, u.FullName FROM Service.Technician t
INNER JOIN admin.[User] u ON t.UserId = u.Id
WHERE u.Locked != 1

GO


