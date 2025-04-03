SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO

---------------------------------------------------------------------------------------------------

IF exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[DN_GetCustID]') and objectproperty(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[DN_GetCustID]
GO

------------------------------------------------------------------------------------
-- Author : NM
-- CR 1037 - Home Club
-- 21/08/2009
-- ---------------------------------------------------------------------------------

CREATE PROCEDURE [dbo].[DN_GetCustID] 	
	@AcctNo Char(12),
	@return Int OUTPUT
AS


SET NOCOUNT ON; 
SET @return = 0

SELECT	CustID, AcctNo 
FROM	CustAcct 
WHERE	AcctNo = @AcctNo and HldorJnt = 'H'


SET @return = @@ERROR

RETURN @return