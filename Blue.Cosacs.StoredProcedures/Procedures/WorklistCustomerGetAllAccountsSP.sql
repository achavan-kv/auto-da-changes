
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID('[dbo].[WorklistCustomerGetAllAccountsSP]') AND OBJECTPROPERTY(id,'IsProcedure') = 1)
DROP PROCEDURE [dbo].[WorklistCustomerGetAllAccountsSP]
GO

CREATE PROCEDURE WorklistCustomerGetAllAccountsSP
-- ================================================
-- Project      : CoSACS .NET
-- File Name    : WorklistCustomerGetAllAccountsSP
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : WorklistCustomerGetAllAccountsSP
-- Author       : John Croft
-- Date         : August 2010
--
-- This procedure will get all the accounts associated to the customer whose account is being processed
--	and in the same worklist
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 
-- ================================================
	-- Add the parameters for the stored procedure here

	@acctno varchar(12),
	@return int OUTPUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    SET 	@return = 0			--initialise return code
	
	;with Customer as
	(
		select custid,worklist from custacct ca INNER JOIN CMWorklistsAcct w on ca.acctno = w.acctno
		where ca.acctno=@acctno and hldorjnt='H'
	)
	
	select w.acctno,c.worklist
	From Customer c INNER JOIN custacct ca on c.custid = ca.custid and hldorjnt='H'
					INNER JOIN CMWorklistsAcct w on ca.acctno = w.acctno 
					where c.worklist=w.Worklist 
						and w.acctno!=@acctno and w.Dateto is null

	SET	@return = @@error
END
GO

--End End End End End End End End End End End End End End End End End End End End End End End End End 