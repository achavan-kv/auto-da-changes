SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[FintransGetChargesSP]') 
			and OBJECTPROPERTY(id, 'IsProcedure') = 1)
	drop procedure [dbo].[FintransGetChargesSP]
GO


CREATE PROCEDURE  dbo.FintransGetChargesSP
-- ================================================
-- Project      : CoSACS .NET
-- File Name    : FintransGetChargesSP.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Get Interest and Admin charges
-- Author       : John Croft
-- Date         : 9 January 2009
--
-- This procedure will get the total Interest and Admin charges on an account
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 
-- ================================================
	-- Add the parameters for the stored procedure here
   			@acctno VARCHAR(12),
   			@interest money output,
   			@admin money output,
   			@return int OUTPUT
 
AS
 
 	SET  @return = 0   --initialise return code

	SELECT	@interest = isnull(sum(transvalue),0)
	FROM 		fintrans
	WHERE 	AcctNo = @acctNo 
	AND 		(transtypecode=N'INT')
	
	SELECT	@admin = isnull(sum(transvalue),0)
	FROM 		fintrans
	WHERE 	AcctNo = @acctNo 
	AND 		(transtypecode=N'ADM')

 	IF (@@error != 0)
 	BEGIN
  		SET @return = @@error
 	END

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

-- End End End End End End End End End End End End End End End End End End End End End End End End End End End End
