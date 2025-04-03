SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[sploadclearance_updatedate]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[sploadclearance_updatedate]
GO

CREATE procedure sploadclearance_updatedate
-- ================================================
-- Project      : CoSACS .NET
-- File Name    : sploadclearance_updatedate.prc
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Update Date changed - DN_AccountLoadClearance
-- Author       : ??
-- Date         : ??
--
-- This procedure will update datechanged details
-- 
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 30/09/08  jec UAT536 Only update datechanged if >= current datechange. 
--				 DN_AccountLoadClearance SP will be changed to set initial datechange=dateacctopen.
-- =================================================================================
	-- Add the parameters for the stored procedure here
AS
BEGIN
declare @status integer,@query_text  varchar (256)
   UPDATE #temp_acct1 SET empeenochange =  a.empeenochange,
   datechange = a.datechange
   FROM dbo.agreement a 
   WHERE a.acctno = #temp_acct1.acctno 
   AND a.empeenochange <>0
   AND a.datechange > #temp_acct1.datechange 

   update #temp_acct1 set
                  empeenochange = p.empeenochange,
                  datechange = p.datechange,
                  custid = p.custid,
                  dateprop = p.dateprop
                  from    proposal p,
                  custacct c,
                #temp_acct1     t
                  where   c.acctno = t.acctno
                  and     p.acctno = c.acctno
                  and     p.custid = c.custid
                  and     c.hldorjnt =   'H'
                  and     t.accttype !=  'C'
                  and p.datechange >= t.datechange		-- UAT536 jec 30/09/08
                  AND p.empeenochange <>0 
    set @status =@@error
    if @status = 0
     begin
      	set @query_text =N'updating who changed from proposalflag table'
        update     #temp_acct1     set
                  empeenochange = p.empeenopflg,
                  datechange = p.datecleared
                  from    proposalflag p,
                #temp_acct1     t      where
                  p.custid = t.custid
                  and (p.datecleared > t.datechange
                  OR t.datechange is null)
                  AND p.empeenopflg <>0 
    	set @status =@@error
    end
    if @status = 0
     begin
      	set @query_text =N'updating who changed from customer address table'
        update     #temp_acct1     set
                  empeenochange = p.empeenochange,
                  datechange = p.datechange
                  from   custaddress p,
                #temp_acct1     t      where
                  p.custid = t.custid
                  and (p.datechange > t.datechange
                  OR t.datechange is null)
                  and (p.datemoved is null or p.datemoved = '1-Jan-1900')
                  AND p.empeenochange <>0
    	set @status =@@error
    end
     update #temp_acct1 set empeenochange = 0 where empeenochange is null
    return @status
end

GO 
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

