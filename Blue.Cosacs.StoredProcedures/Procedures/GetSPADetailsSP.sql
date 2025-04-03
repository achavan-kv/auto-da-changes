
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

IF EXISTS (select * from dbo.sysobjects where id = object_id('[dbo].[GetSPADetailsSP]') and 
OBJECTPROPERTY(id, 'IsProcedure') = 1)
DROP PROCEDURE [dbo].[GetSPADetailsSP]
GO

CREATE  PROCEDURE GetSPADetailsSP
-- ================================================
-- Project      : CoSACS .NET
-- File Name    : GetSPADetailsSP.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Get SPA details
-- Author       : Jez Hemans
-- Date         : 29 May 2007
--
-- Description:		Returns the account number, instalment and expiry date for accounts in spa 
--					which have not yet expired
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      -- CR1084  -----------
-- 03/09/10 jec Include PTP details and add account no parameter
-- 16/09/10 jec UAT17 check account in PTP strategy
-- ================================================
	-- Add the parameters for the stored procedure here
		@acctno CHAR(12),		--CR1084
		@return INT OUT 
AS
SET NOCOUNT ON

SET @return = 0

SELECT     acctno, spainstal, dateexpiry
FROM         spa
WHERE  acctno=@acctno and (dateexpiry >= GETDATE())		--CR1084

-- now select PTP details --CR1084
select b.acctno,actionvalue,datedue
from bailaction b INNER JOIN CMStrategyAcct a on b.acctno=a.acctno 
			and a.strategy='PTP' and a.dateto is null  --UAT17
WHERE  b.acctno=@acctno and (datedue >= GETDATE()) and code='PTP' 

SET @return = @@error

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

-- End End End End End End End End End End End End End End End End End End End End End End End End End End End End End
