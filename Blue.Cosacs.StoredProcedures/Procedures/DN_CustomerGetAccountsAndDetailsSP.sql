SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_CustomerGetAccountsAndDetailsSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_CustomerGetAccountsAndDetailsSP]
GO

CREATE PROCEDURE  dbo.DN_CustomerGetAccountsAndDetailsSP
-- ================================================
-- Project      : CoSACS .NET
-- File Name    : DN_CustomerGetAccountsAndDetailsSP.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Get Customer and Account Details
-- Author       : ??
-- Date         : ??
--
-- Description:		Returns Basic customer info.
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 03/09/10 jec CR1084 Include Active action codes for CLA, NIS and INFO (used in payment screen)
-- 16/09/10 jec UAT20  NIS should be TOS
-- 27/10/11 jec #8486 CR1232 Cash Loan - Show message in Payment screen
-- ================================================
	-- Add the parameters for the stored procedure here
			@acctno	VARCHAR(12),
   			@return 	INT OUTPUT
AS
 	SET  @return = 0 --initialise return code
	declare @custid VARCHAR(50)
	set @custid = null
	SELECT @custid = custid FROM dbo.custacct WHERE acctno= @acctno 
	AND hldorjnt = 'H'

	SELECT	CA.custid as CustomerID,
			C.title as Title,
			C.firstname as FirstName,
			C.name as LastName,
			C.dateborn as DOB,
			C.rfcreditlimit,
			C.creditblocked,				--IP - 01/06/10 - UAT(1006) UAT5.2 Log
			MAX(ba1.code) as CLA,		--CR1084
			MAX(ba2.notes) as AddressNotes , 
			MAX(ba2.code) as INFO,		--CR1084
			MAX(ba2.notes) as InfoNotes  ,
			MAX(ba3.code) as TOS	,		--CR1084 UAT20
			MAX(ba3.notes) as TOSNotes,
			LoanQualified		-- #8486
	FROM	custacct CA 
			INNER JOIN customer C ON CA.custid = C.custid 
			INNER JOIN acct A ON CA.acctno = A.acctno
			LEFT OUTER JOIN bailaction ba1 on ba1.acctno=a.acctno and ba1.code='CLA' and ba1.datedue>a.datelastpaid--CR1084
			LEFT OUTER JOIN bailaction ba2 on ba2.acctno=a.acctno and ba2.code='INFO' and ba2.datedue>a.datelastpaid--CR1084
			LEFT OUTER JOIN bailaction ba3 on ba3.acctno=a.acctno and ba3.code='TOS' and ba3.datedue>a.datelastpaid --CR1084 UAT20
	WHERE	CA.custid = @custid -- AA bring back for any customer if have multiple accounts
	AND		CA.hldorjnt = 'H'
	GROUP BY CA.custid ,c.Title,c.FirstName,c.NAME,c.dateborn,c.rfcreditlimit,c.creditblocked,LoanQualified			-- #8486
			
	
	IF (@@error != 0)
 	BEGIN
  		SET @return = @@error
 	END




GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

-- End End End End End End End End End End End End End End End End End End End End End End End End End End
