
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[GetPaymentFileDelimitersSP]') 
        and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[GetPaymentFileDelimitersSP]
GO

CREATE PROCEDURE 	dbo.GetPaymentFileDelimitersSP
-- ================================================
-- Project      : CoSACS .NET
-- File Name    : GetPaymentFileDelimitersSP
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : GetPaymentFileDelimitersSP
-- Author       : Ilyas Parker
-- Date         : 16/09/2010
--
-- This procedure will retrieve all delimiters that can be used in Payment Files
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 16/09/10 ip  CR1092 - COASTER to CoSACS Enhancements - Creation

-- ====================================================

						@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	select delimiter
	FROM storderdelimiters


	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

