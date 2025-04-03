SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_PaymentHolidaysGetSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_PaymentHolidaysGetSP]
GO


CREATE PROCEDURE 	dbo.DN_PaymentHolidaysGetSP
			@custid varchar(20),
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code
	
	SELECT		PH.acctno,
			PH.agrmtno,
			PH.datetaken,
			PH.empeeno,
			isnull(u.FullName, 'Unknown') AS EmployeeName, 
			PH.newdatefirst
	FROM		custacct CA INNER JOIN PaymentHolidays PH
	ON		CA.acctno = PH.acctno 
	LEFT OUTER JOIN Admin.[User] u ON u.id = PH.empeeno 
	WHERE		CA.custid = @custid
	AND		CA.hldorjnt = 'H'

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

