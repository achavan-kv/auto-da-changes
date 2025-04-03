SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_TermsTypeGetDefaultSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_TermsTypeGetDefaultSP]
GO

CREATE PROCEDURE 	dbo.DN_TermsTypeGetDefaultSP
-- Project      : CoSACS .NET
-- File Name    : DN_TermsTypeGetDefaultSP.prc
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Get the default Termstype
-- Author       : ??
-- Date         : ??
--
-- This procedure will get the default termstype for a new HP account
-- 
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 22/08/07  jec CR903  - get default termstype for the branch store type
-- 06/09/07  rdb CR906  - get default termstype for Non/Loan account
-- ================================================
	-- Add the parameters for the stored procedure here

			@termstype varchar(2) OUT,
			@pibranchNo int,				--CR903 jec 22/08/07
			@IsLoan bit,					--CR906 rdb 06/09/07
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	-- if branch passed in is zero set to lowest branch		--CR903 jec 22/08/07
	if @pibranchNo=0 
		set @pibranchNo=(select min(branchno) from branch)
	
	-- get termstype for branch storetype
	SELECT 	TOP 1
				@termstype = t.termstype
	FROM	termstype t inner join termstypetable tt on t.termstype=tt.termstype,branch b 
	WHERE 	t.isactive = 1
	and b.branchno=@pibranchNo and (b.storetype=tt.storetype or tt.storetype='A')
	AND		(t.delnonstocks = 0 or @isloan =1) -- if cash loan then allow automated non stocks to be brought back
	AND	tt.IsLoan = @IsLoan
	ORDER BY 	t.servpcent DESC, t.termstype

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

