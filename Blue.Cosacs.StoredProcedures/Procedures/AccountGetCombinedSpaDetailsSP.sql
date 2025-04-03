SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS OFF
GO

IF EXISTS (SELECT * FROM SysObjects
           WHERE id = object_id('[dbo].AccountGetCombinedSpaDetailsSP') and OBJECTPROPERTY(id, 'IsProcedure') = 1)

BEGIN
    DROP PROCEDURE AccountGetCombinedSpaDetailsSP
END
GO

CREATE PROCEDURE dbo.AccountGetCombinedSpaDetailsSP 

-- ================================================
-- Project      : CoSACS .NET
-- File Name    : AccountGetCombinedSpaDetailsSP.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Gets Account details for Special Arrangements
-- Author       : John Croft
-- Date         : 12 January 2009
--
-- This procedure will get the customer account details for accounts that will have SPA.
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 12/06/09 jec  return expected settlement date 
-- ================================================
	-- Add the parameters for the stored procedure here
		@custid varchar(20),
		@return int output

as
	set @return=0
	
	Select a.acctno as 'Account No.', cast(round(a.outstbal,2) as decimal(12,2)) as 'Outstanding Bal', 
			cast(round(a.arrears,2) as decimal(12,2)) as Arrears,cast(round(i.instalamount,2) as decimal(12,2)) as 'Instal Amount',
			0.00 as 'Arrangement Amount',0.00 as 'Odd Payment',
			case 
				when datelast>getdate() then dateadd(m,cast(arrears/i.instalamount as int),datelast)
				else dateadd(m,cast(arrears/i.instalamount as int),Convert(datetime,CAST(GETDATE() as CHAR(12)),103)) 
			end as 'DateLast'
	From acct a INNER JOIN custacct ca on a.acctno = ca.acctno
				INNER JOIN instalplan i on ca.acctno = i.acctno
	where custid=@custid and hldorjnt='H'
			and currstatus!='S' and a.accttype = 'R'
			and a.outstbal > 0

	if (@@error != 0)
		set @return= @@ERROR
		
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

-- End End End End End End End End End End End End End End End End End End End End End End End 

