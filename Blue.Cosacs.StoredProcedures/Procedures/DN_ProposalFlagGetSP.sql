SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_ProposalFlagGetSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_ProposalFlagGetSP]
GO

/****** Object:  StoredProcedure [dbo].[DN_ProposalFlagGetSP]    Script Date: 11/05/2007 11:58:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE 	[dbo].[DN_ProposalFlagGetSP]
--------------------------------------------------------------------------------
--
-- Project      : CoSACS .NET
-- File Name    : DN_ProposalFlagGetSP.PRC
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        :
-- Author       : ??
-- Date         : ??
--
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 28/06/11  IP  5.13 - LW73619 - #3751 - This procedure is called when re-openening sanction stage. When setting @holdprop need to
--				 set to 'Y' when the holdprop = 'N' but there is an entry in delauthorise. This scenario
--				 may occurr when an account is fully delivered (no outstanding deliveries) and sanction stage 1 is re-opened.
--				 Cannot return holdprop as 'N' in this instance as the fields on the sanction stage will be read only preventing
--				 the user from completing.
--------------------------------------------------------------------------------
			@acctno char(12),
			@custid VARCHAR(20),
			@dateprop datetime,
			@holdprop char(1) OUT,
			@currentStatus char(1) OUT,
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	--SELECT	@holdprop = holdprop
	--FROM		agreement
	--WHERE	acctno = @acctno
	
	--IP - 28/06/11 - 5.13 - LW73619 - #3751	
	if((select holdprop from agreement where acctno = @acctno)='Y' or exists(select * from delauthorise where acctno = @acctno))
	begin
		set @holdprop = 'Y'
	end 
	else
		set @holdprop = 'N'

	SELECT	@currentStatus = currstatus 
	FROM		acct
	WHERE	acctno = @acctno

	SELECT	custid,
			dateprop,
			checktype,
			datecleared,
			empeenopflg
	FROM		proposalflag
	WHERE	acctno = @acctno
	AND dateprop = @dateprop
	AND custid = @custid
	

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END

