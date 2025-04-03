SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects
where id = object_id('[dbo].[DN_ProposalAuditResultSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_ProposalAuditResultSP]
GO


CREATE PROCEDURE 	dbo.DN_ProposalAuditResultSP
			@custid varchar(20),
			@acctno char(12),
			@dateprop smalldatetime,
			@empeeno int,
			@return int OUTPUT

AS

	SET @return = 0

    INSERT INTO ReferralOverride
           (CustId, AcctNo, DateProp, SystemRecommendation, UWResult, EmpeeNoCleared, DateCleared)
    SELECT  CustId, AcctNo, DateProp, SystemRecommendation, PropResult, @empeeNo, GETDATE()
    FROM    Proposal
	WHERE	custid = @custid
	AND		acctno = @acctno
	AND		dateprop = @dateprop

    SET @return = @@error
    
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

