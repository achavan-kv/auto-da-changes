/****** Object:  StoredProcedure [dbo].[DN_SRSaveTechnician]    Script Date: 10/17/2006 16:26:14 ******/
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID('[dbo].[DN_SRSaveTechnicianSP]') AND OBJECTPROPERTY(id,'IsProcedure') = 1)
DROP PROCEDURE [dbo].[DN_SRSaveTechnicianSP]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Peter Chong
-- Create date: 17-Oct-2006
-- Description:	Saves technician details (update or insert)
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 31/03/11 jec #3419 Remove Deleted check
-- =============================================
CREATE PROCEDURE DN_SRSaveTechnicianSP
(   
	@TechnicianId		int = null output ,
	@Title			varchar(25) ,
	@FirstName		varchar(30),
	@LastName		varchar(60) ,
	@Address1		varchar(26),
	@Address2		varchar(26),
	@Address3		varchar(26),
	@AddressPC		varchar(10),
	@PhoneNo		varchar(20),
	@MobileNo		varchar(20),
	@Internal		char(1),
	@HoursFrom		char(5),
	@HoursTo		char(5),
	@CallsPerDay    int,
	@VacationFrom   smalldatetime,
	@VacationTo		smalldatetime,
	@Comments		varchar(2000),
	@return			int output
	
)
AS
BEGIN
	SET NOCOUNT ON;
		
	UPDATE [SR_Technician]
	   SET [Title] = @Title
		  ,[FirstName] = @FirstName
		  ,[LastName] = @LastName
		  ,[Address1] = @Address1
		  ,[Address2] = @Address2
		  ,[Address3] = @Address3
		  ,[AddressPC] = @AddressPC
		  ,[PhoneNo] = @PhoneNo
		  ,[MobileNo] = @MobileNo
		  ,[Internal] = @Internal
		  ,[HoursFrom] = @HoursFrom
		  ,[HoursTo] = @HoursTo
		  ,[CallsPerDay] = @CallsPerDay
		  ,[Comments] = @Comments
	 WHERE TechnicianId = @TechnicianId		--AND Deleted = 0  

    IF @@ROWCOUNT = 0 
	BEGIN 
		INSERT INTO [SR_Technician]
			   ([Title]
			   ,[FirstName]
			   ,[LastName]
			   ,[Address1]
			   ,[Address2]
			   ,[Address3]
			   ,[AddressPC]
			   ,[PhoneNo]
			   ,[MobileNo]
			   ,[Internal]
			   ,[HoursFrom]
			   ,[HoursTo]
			   ,[CallsPerDay]
			   ,[Comments]
			   )
		 VALUES
			   ( @Title			
				,@FirstName		
				,@LastName		
				,@Address1		
				,@Address2		
				,@Address3		
				,@AddressPC		
				,@PhoneNo		
				,@MobileNo		
				,@Internal		
				,@HoursFrom		
				,@HoursTo		
				,@CallsPerDay
				,@Comments   )
				
			SET @TechnicianId = SCOPE_IDENTITY()
				
		END	

		IF @VacationFrom IS NOT NULL AND @VacationTo IS NOT NULL
		BEGIN
			IF EXISTS(SELECT * FROM [SR_TechnicianVacations] WHERE TechnicianID = @TechnicianId)
				BEGIN
					UPDATE [SR_TechnicianVacations]
					SET VacationStartDate = @VacationFrom,VacationEndDate = @VacationTo
					WHERE TechnicianID = @TechnicianId
				END
				ELSE
				BEGIN
					INSERT INTO [SR_TechnicianVacations]
					(TechnicianID,VacationStartDate,VacationEndDate)
					VALUES
					(@TechnicianId,@VacationFrom,@VacationTo)
				END
		END	
		SET @return = @@error
END
GO
