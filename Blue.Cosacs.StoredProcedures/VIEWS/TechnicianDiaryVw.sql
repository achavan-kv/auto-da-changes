SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[TechnicianDiaryVw]'))
	DROP VIEW [dbo].[TechnicianDiaryVw]
GO

CREATE VIEW [dbo].[TechnicianDiaryVw] AS ( 

SELECT TECH.TechnicianId, TECH.SlotDate, TECH.BookingType, 
  TECH.ServiceRequestNo, TECH.InstallationNo, 
  GROUPED_TECH.StartSlot, GROUPED_TECH.NoOfSlots
FROM dbo.SR_TechnicianDiary TECH
INNER JOIN (
  SELECT  TechnicianId, SlotDate, BookingType, ServiceRequestNo, InstallationNo, 
  MIN(SlotNo) AS StartSlot, COUNT(SlotNo) AS NoOfSlots
  FROM dbo.SR_TechnicianDiary
  GROUP BY TechnicianId, SlotDate, BookingType, ServiceRequestNo, InstallationNo
) AS GROUPED_TECH ON TECH.TechnicianId = GROUPED_TECH.TechnicianId AND
            TECH.SlotDate = GROUPED_TECH.SlotDate AND 
            TECH.SlotNo = GROUPED_TECH.StartSlot
)
GO
