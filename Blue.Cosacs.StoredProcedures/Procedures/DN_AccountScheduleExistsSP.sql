--------------------------------------------------------------------------------
--
-- Project      : CoSACS .NET
-- File Name    : DN_GetAcctPayments.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Check if there are any schedule records for an account
-- Author       : Ilyas Parker
-- Date         : 28th November 2007
--
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 28/11/07  IP  69360 
--------------------------------------------------------------------------------

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_AccountScheduleExistsSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_AccountScheduleExistsSP]
GO

CREATE PROCEDURE [DN_AccountScheduleExistsSP] 
(
	@acctNo VARCHAR(12),
	@scheduleExists BIT OUTPUT, 	
	@return int OUTPUT
)
AS


IF EXISTS (SELECT * FROM schedule WHERE acctno = @acctNo)

	SET @scheduleExists = 1
ELSE
	SET @scheduleExists = 0

SELECT @return = @@ERROR

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO




