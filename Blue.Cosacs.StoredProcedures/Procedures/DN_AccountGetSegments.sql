-- RD & DR 25/11/04 Added left outer join to Code table for CR721 change for Tallyman fee

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO
SET ANSI_WARNINGS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_AccountGetSegments]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_AccountGetSegments]
GO

CREATE PROCEDURE 	dbo.DN_AccountGetSegments
-- =============================================
-- Author:		??
-- Create date: ??
-- Description:	Returns the Tallyman segment
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 15/02/10  jec CR1072 Malaysia merge 
-- =============================================
			@acctno varchar(12),
			@serverdbname varchar(50),
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	DECLARE @statement SQLText

	SET 	@return = 0			--initialise return code
/* Malaysia Merge - Cashiers payment screen 
	SET @statement = 'SELECT s.Segment_ID, s.Segment_Name,s.User_id, s.Date, ISNULL(c.reference,0) AS Reference ' +
		         'FROM ' + @serverdbname + '.dbo.TM_Segments s ' +
		         'LEFT OUTER JOIN code c ON c.category = ''TFA'' AND c.code = s.segment_Id ' +
		         'WHERE Account_Number = ' + @acctno

	EXECUTE sp_executesql @statement
	
	*/
	-- TM_Segments now interfaced to Cosacs aa-- now relying again on TFA to determine where Fee field in payment screen
	SELECT s.Segment_ID, s.Segment_Name, s.Date, ISNULL(c.reference,0) AS Reference
		         FROM TM_Segments s 
		         LEFT OUTER JOIN code c ON c.category = 'TFA' AND c.codedescript = s.segment_name
		         WHERE Account_Number = @acctno and isnumeric(reference) = 1
				 order by date

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_WARNINGS OFF 
GO

