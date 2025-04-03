SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_GetEodOptionsSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_GetEodOptionsSP]
GO


CREATE PROCEDURE 	dbo.DN_GetEodOptionsSP
--------------------------------------------------------------------------------
--
-- Project      : CoSACS .NET
-- File Name    : DN_GetEodOptionsSP.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : DN_GetEodOptionsSP.sql
-- Description	: 
-- Author       : 
-- Date         : 
--
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 22/06/11  IP  CR1212 - RI - #3987 - If the interface method set in Country Parameters
--				 is 'FACT' then we do not want to return the option for RI Export.
--				 If 'RI' then we do not want to display the FACT 2000 Export option.
--------------------------------------------------------------------------------
			@category varchar(12),
			@flag varchar(1),
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	declare @interfaceMethod varchar(5)				--IP - 22/06/11 - CR1212 - RI - #3987
	declare @code varchar(12)						--IP - 22/06/11 - CR1212 - RI - #3987
	
	set @code = ''
	select @interfaceMethod = value from countrymaintenance where codename = 'RIInterfaceOptions'
	
	if(@interfaceMethod = 'FACT')
	begin
		set @code = 'CoSACS2RI'
	end
	else if(@interfaceMethod = 'RI')
	begin
		set @code = 'COS FACT'
	end

	
	SELECT	code,
			codedescript,
			reference
	FROM		code
	WHERE	category = @category
	AND		statusflag = @flag
	AND		(code!= @code or @code = '')	--IP - 22/06/11 - CR1212 - RI - #3987
	--AND		code != ''
	order by codedescript	
	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

