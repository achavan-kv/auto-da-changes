
/****** Object:  StoredProcedure [dbo].[DN_SRGetTechnicianSP]    Script Date: 10/17/2006 11:40:27 ******/
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID('[dbo].[DN_SRGetTechnicianSP]') AND OBJECTPROPERTY(id,'IsProcedure') = 1)
DROP PROCEDURE [dbo].[DN_SRGetTechnicianSP]


SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--exec DN_SRGetTechnicianSP null,null,null,0

-- =============================================
-- Author:		Peter Chong
-- Create date: 17-Oct-2006
-- Description:	Gets technician details by either id or firstname and lastname or if all parametrs null a list
-- Change Control
-----------------
-- 14/02/11 IP Sprint 5.10 - #2975 - CR1030 - Technician Maintenance - mark as Inactive - Need to also return the
--							 Technicians marked as Deleted (in-active) as users may wish to make them active.
-- =============================================
CREATE PROCEDURE DN_SRGetTechnicianSP
(	
	@technicianId int = null,
	@firstName	  varchar(30) = null,	
	@lastName	  varchar(30) = null,
	@return		  int output
)

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	SELECT t.TechnicianId
		  ,[Title]
		  ,[FirstName]
		  ,[LastName]
		  ,[Address1]
		  ,[Address2]
		  ,[Address3]
		  ,[AddressPC]
		  ,[PhoneNo]
		  ,[MobileNo]
		  ,Case when [Internal] = '' then 'N' else [Internal] end as 'Internal' --IP - 14/02/11 - Sprint 5.10 - #2975
		  ,[HoursFrom]
		  ,[HoursTo]
		  ,[CallsPerDay]
		  ,[VacationStartDate] as [UnavailableStartDate]
		  ,[VacationEndDate] as [UnavailableEndDate]
		  ,[Comments]
		  ,[Deleted]		--IP - 14/02/11 - Sprint 5.10 - #2975
	 FROM [SR_Technician] t LEFT OUTER JOIN [SR_TechnicianVacations] tv ON t.TechnicianId = tv.TechnicianId
	 WHERE (@TechnicianId IS NULL OR  t.TechnicianId = @technicianId) AND
		(@firstName IS NULL OR  firstName = @firstName) AND
		(@lastName IS NULL OR  lastName = @lastName) --AND Deleted = 0 --IP - 14/02/11 - #2975 - Commented out AND Deleted = 0
	ORDER BY [LastName]

	SET @return = @@error
END
GO


