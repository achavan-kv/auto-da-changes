

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

IF EXISTS (select * from dbo.sysobjects where id = object_id('[dbo].[GetStrategyAccountsToAllocateSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
DROP PROCEDURE [dbo].[GetStrategyAccountsToAllocateSP]
GO

-- =============================================
-- Author:		Jez Hemans
-- Create date: 	29/05/2007
-- Description:		Returns all the accounts in the bailiff or collector strategies
-- =============================================

CREATE  PROCEDURE GetStrategyAccountsToAllocateSP
@return INT OUT 
AS
SET NOCOUNT ON

SET @return = 0

--IP - 03/06/09 - Credit Collection Walkthrough Changes - added w.Dateto as the accounts need to be in the 
--worklist.
SELECT DISTINCT acctno FROM code c INNER JOIN CMWorklistsAcct w ON c.code = w.Strategy
WHERE reference > '0' AND category = 'SS1'
AND w.Dateto is null

SET @return = @@error

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

