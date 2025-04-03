SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_AccountGetSundryCreditSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_AccountGetSundryCreditSP]
GO

CREATE PROCEDURE 	dbo.DN_AccountGetSundryCreditSP
-- ================================================
-- Project      : CoSACS .NET
-- File Name    : DN_AccountGetSundryCreditSP.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : DN_AccountGetSundryCreditSP
-- Author       : ?
-- Date         : ?
--
-- This procedure will return the Sundry Account for a branch
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 09/09/10  ip  CR1092 - COASTER to CoSACS Enhancements - UAT5.4 - UAT(6) If the Sundry Account does not exist for the branch passed in
--				 select the minimum Sundry Account branch.

			@branchno smallint,
			@acctno varchar(12) OUT,
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	SELECT	@acctno = custacct.acctno 
	FROM	custacct ,acct a
	WHERE	custid LIKE 'SUNDRY%CREDIT%'
	AND		custacct.acctno LIKE convert ( varchar, @branchno ) + '%'
   	AND     custacct.acctno = a.acctno 
   	AND		a.currstatus !='S'
   	AND		a.acctno LIKE '___5%'
   	
   	--IP - 09/09/10 - CR1092 - UAT5.4 - UAT(6)
   	if(@acctno is null)
   	begin
   		set	@acctno =(select min(custacct.acctno) 
		FROM	custacct ,acct a
		WHERE	custid LIKE 'SUNDRY%CREDIT%'
   		AND     custacct.acctno = a.acctno 
   		AND		a.currstatus !='S'
   		AND		a.acctno LIKE '___5%')
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

