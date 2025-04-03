SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_AgreementAuditGetSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_AgreementAuditGetSP]
GO

-- =============================================
-- Modified By:	Jez Hemans
-- Modified On date: 04/02/2008
-- Description:	69516 - Additional fields 'OldCODflag' and 'NewCODflag' to be retrieved as well
-- =============================================


CREATE PROCEDURE 	dbo.DN_AgreementAuditGetSP
-- ================================================      
-- Project      : CoSACS .NET      
-- File Name    : DN_AgreementAuditGetSP.sql      
-- File Type    : MSSQL Server Stored Procedure Script      
-- Title        : Agreement Audit Get     
-- Author       : ??      
-- Date         : ??      
--      
-- This procedure will retrieve details of Agreement changes.      
--       
-- Change Control      
-- --------------      
-- Date      By  Description      
-- ----      --  -----------      
-- 01/11/10  jec UAT105 Incorrect Service Charge % displayed 
-- =================================================================================      
 -- Add the parameters for the stored procedure here
   
			@acctno varchar(12),
			@rowcount int,
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	SET		ROWCOUNT @rowcount

	SELECT	A.acctno,
			A.agrmtno,
			A.oldagreementtotal,
			A.newagreementtotal,
			A.oldservicecharge,
			A.newservicecharge,
			A.olddeposit,
			A.newdeposit,
			A.empeenochange,
			A.datechange,
			A.systemusername,
			isnull(u.FullName, 'Unknown') as EmployeeName,
			A.OldCODflag,
			A.NewCODflag,
			A.oldTermstype,
			A.NewTermstype,
			CONVERT(FLOAT ,0.0) AS OldIntrate,
			CONVERT(float,0.0) AS NewIntrate
			
    INTO #agreementaudit 			
	FROM		agreementaudit A 
	LEFT OUTER JOIN Admin.[User] u ON		A.empeenochange = u.id 
	WHERE	acctno = @acctno
	ORDER BY	A.datechange DESC

	
	UPDATE #agreementaudit
	SET OldIntrate = i.intrate
	FROM intratehistory i ,
	acct a inner join proposal p on a.acctno=p.acctno		--UAT105 01/11/10
	WHERE a.acctno = @acctno 
	AND i.termstype = oldtermstype 
	AND i.datefrom < a.dateacctopen
	AND ( i.dateto > a.dateacctopen 
	OR ISNULL(i.dateto ,'1-jan-1900') = '1-Jan-1900')
	and i.band=p.ScoringBand		--UAT105 01/11/10
	
	
	UPDATE #agreementaudit
	SET NEWIntrate = i.intrate
	FROM intratehistory i ,
	acct a inner join proposal p on a.acctno=p.acctno		--UAT105 01/11/10
	WHERE a.acctno = @acctno 
	AND i.termstype = NEWtermstype 
	AND i.datefrom < a.dateacctopen
	AND ( i.dateto > a.dateacctopen 
	OR ISNULL(i.dateto ,'1-jan-1900') = '1-Jan-1900')
	and i.band=p.ScoringBand		--UAT105 01/11/10
	
	SELECT * FROM #agreementaudit
	

	SET		ROWCOUNT 0



	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

-- End End End End End End End End End End End End End End End End End End End End End End End End End End End End End End End End 
