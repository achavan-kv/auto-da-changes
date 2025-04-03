SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_CountryMaintenanceSaveSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_CountryMaintenanceSaveSP]
GO

CREATE PROCEDURE 	dbo.DN_CountryMaintenanceSaveSP
-- ================================================
-- Project      : CoSACS .NET
-- File Name    : DN_CountryMaintenanceSaveSP.prc
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Save Country parameters
-- Author       : ??
-- Date         : ??
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 20/04/10  jec UAT75 - remove any leading spaces from Gift voucher account 
-- 21/10/10  jec UAT88 - Ensure BehaviouralScoring value is Upper Case.
-- ================================================
	-- Add the parameters for the stored procedure here
			@country char(1),
			@parameterid int,
			@value varchar(1500),
         @user int = 0,
			@return int OUTPUT

AS	
        SET 	@return = 0			--initialise return code
        DECLARE @prevvalue varchar(200),@name varchar(50),
				@codename VARCHAR(30)		--UAT75 20/04/10
        select @prevvalue = value,@name=name,@codename=codename 
        from CountryMaintenance
          	WHERE	parameterID = @parameterid
          	AND		CountryCode = @country
        if @value !=@prevvalue
        begin
        if @codename='giftvoucheraccount'
			set @value=LTRIM(@value)	-- UAT75 20/04/10 - remove any leading spaces
		if @codename='BehaviouralScorecard'		-- UAT88 
			set @value=UPPER(@value)
			
   	  UPDATE	CountryMaintenance
          SET		Value = @value
          WHERE	parameterID = @parameterid
          AND		CountryCode = @country

      	  insert into countrymaintenanceaudit (Fieldname ,Oldvalue,Newvalue,EmpeenoChange,DateChange,parameterid)
           values (@name,@prevvalue,@value,@user,getdate(),@parameterid)
        end

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

-- End End End End End End End End End End End End End End End End End End End End End End End End End