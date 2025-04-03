SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_FintransGetForTransferSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_FintransGetForTransferSP]
GO

CREATE PROCEDURE 	dbo.DN_FintransGetForTransferSP
-- ============================================================================================
-- Author:		?
-- Create date: ?
-- Description:	
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 14/02/12  IP  #8819 - CR1234 - Transferring of Overage/Shortage
-- 20/02/12  IP  #9633 - CR1234 - Prevent transfer of transactions already transferred.
-- ============================================================================================
			@acctno varchar(12),
			@before datetime,
			@availableTransfer money OUTPUT,
			@limitRows bit,
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

    -- DSR 6/10/04 Available to transfer is all transactions
    -- not just next 250 tranasctions.
    SELECT	@availableTransfer = -SUM(FT.transvalue)
	FROM	fintrans FT
	WHERE	FT.acctno = @acctno
	AND		FT.transtypecode in ('PAY', 'COR', 'DDE', 'DDG', 'DDN', 'DDR', 'REF', 'RET', 'XFR', 'SCX', 'JLX', 'ADX', 'OVE', 'SHO', 'CAS','BEX')		--IP - 14/02/12 - #8819 - CR1234

    IF @limitRows = 1
    BEGIN
        -- Next 250 transactions
	    SELECT	TOP 250
			    FT.transrefno,
			    FT.datetrans,
			    FT.chequeno,
			    FT.transtypecode,
			    -(FT.transvalue) as transvalue,
			   --isnull(FX.acctname, '') as reference
			    case when FT.transtypecode in ('OVE', 'SHO') then convert(varchar,isnull(ctb.cashiertotalid,''))						--IP - 14/02/12 - #8819 - CR1234
					else isnull(FX.acctname, '') end as reference,
				case when exists(select * from finxfr f1
									where f1.acctno = FT.acctno 
									and f1.OrigTransRefNo = FT.transrefno) then 0 else 1 end as 'AllowTransfer'						--IP - 20/02/12 - #9633 - CR1234
	    FROM		fintrans FT LEFT OUTER JOIN
			    finxfr FX ON  
			    FT.acctno = FX.acctno
	    AND		FT.transrefno = FX.transrefno
	    LEFT OUTER JOIN cashiertotalsbreakdown ctb ON FT.ID = ctb.FintransId															--IP - 14/02/12 - #8819 - CR1234
	    WHERE	FT.acctno = @acctno
	    AND		FT.transtypecode in ('PAY', 'COR', 'DDE', 'DDG', 'DDN', 'DDR', 'REF', 'RET', 'XFR', 'SCX', 'JLX', 'ADX', 'SHO', 'OVE', 'CAS','BEX')	--IP - 14/02/12 - #8819 - CR1234
	    --AND		FT.datetrans <= @before
		AND		FT.datetrans < @before		-- 68674 jec 28/11/06 stops transaction repeating on next page
	    ORDER BY	FT.datetrans DESC
	END
	ELSE
	BEGIN
	    SELECT	FT.transrefno,
			    FT.datetrans,
			    FT.chequeno,
			    FT.transtypecode,
			    -(FT.transvalue) as transvalue,
			    --isnull(FX.acctname, '') as reference
			    case when FT.transtypecode in ('OVE', 'SHO') then convert(varchar,isnull(ctb.cashiertotalid,''))						--IP - 14/02/12 - #8819 - CR1234
					else isnull(FX.acctname, '') end as reference,
				case when exists(select * from finxfr f1
									where f1.acctno = FT.acctno 
									and f1.OrigTransRefNo = FT.transrefno) then 'N' else 'Y' end as 'AllowTransfer'						--IP - 20/02/12 - #9633 - CR1234
	    FROM		fintrans FT LEFT OUTER JOIN
			    finxfr FX ON  
			    FT.acctno = FX.acctno
	    AND		FT.transrefno = FX.transrefno
	    LEFT OUTER JOIN cashiertotalsbreakdown ctb ON FT.ID = ctb.FintransId															--IP - 14/02/12 - #8819 - CR1234
	    WHERE	FT.acctno = @acctno
	    AND		FT.transtypecode in ('PAY', 'COR', 'DDE', 'DDG', 'DDN', 'DDR', 'REF', 'RET', 'XFR', 'SCX', 'JLX', 'ADX', 'SHO', 'OVE', 'CAS','BEX')	--IP - 14/02/12 - #8819 - CR1234	
	    -- AND		FT.datetrans <= @before
		AND		FT.datetrans < @before		-- 68674 jec 28/11/06
	    ORDER BY	FT.datetrans DESC
	END    

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

