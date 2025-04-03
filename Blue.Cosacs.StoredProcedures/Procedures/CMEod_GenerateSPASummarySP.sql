SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO

if exists (select * from sysobjects where name ='CMEod_GenerateSPASummarySP')
drop procedure CMEod_GenerateSPASummarySP
go

create procedure CMEod_GenerateSPASummarySP @rundate datetime, @return int out			--IP - 13/02/12
-- **********************************************************************
-- Title: CMEod_GenerateSPASummarySP.sql
-- Developer: Ilyas Parker
-- Date: 07/09/2009
-- Purpose: Create a Summary of SPA data for accounts that have an SPA.

-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 18/09/09  jec Only include next instalment if within 25 days of due date
-- 07/12/09  ip  UAT(933)update Nextdateexpirydue for (weekly and fortnightly arrangements).
-- 27/04/10  jec fix null values
-- 13/02/12  ip  Replace GETDATE() with @rundate
-- **********************************************************************
	-- Add the parameters for the stored procedure here
as

	set @return = 0    --initialise return code
 
    truncate table SPASummary

	insert into SPASummary (Acctno, Dateadded, Empeeno, NextDateExpiryDue, ArrangementDateExpiry, ArrangementAmt, TotalSPAInstalDue)
	select s.Acctno, 
		   s.Dateadded, 
		   s.EmpeeNo,
		   MIN(s.dateexpiry),
		   MAX(s.dateexpiry),
		   0,
		   0
	from spa s
	--where s.dateexpiry > GETDATE()
	where s.dateexpiry > @rundate				--IP - 13/02/12 - use @rundate 
	group by s.acctno, s.dateadded, s.EmpeeNo
	
	-- set correct Nextdateexpirydue
	-- only update to future instalment if within 23 days of instalment date (Monthly)
	update SPASummary
	set Nextdateexpirydue = (select MAX(s1.dateexpiry) from spa s1
							   where s1.acctno = ss.acctno
							   --and DATEADD(d,-25,s1.dateexpiry) <= GETDATE())
							   and DATEADD(d,-25,s1.dateexpiry) <= @rundate)			--IP - 13/02/12 - use @rundate
	from SPASummary ss
	INNER JOIN spa s ON ss.Acctno = s.acctno
	AND s.payfreq = 'M'
	
	--IP - 07/12/09 - UAT(933)
	-- only update to future instalment if within 4 days of instalment date (Weekly)
	update SPASummary
	set Nextdateexpirydue = (select MAX(s1.dateexpiry) from spa s1
							   where s1.acctno = ss.acctno
							   --and DATEADD(d,-4,s1.dateexpiry) <= GETDATE())
							   and DATEADD(d,-4,s1.dateexpiry) <= @rundate)				--IP - 13/02/12 - use @rundate
	from SPASummary ss
	INNER JOIN spa s ON ss.Acctno = s.acctno
	AND s.payfreq = 'W'
	
	-- only update to future instalment if within 10 days of instalment date (Fortnightly)
	update SPASummary
	set Nextdateexpirydue = (select MAX(s1.dateexpiry) from spa s1
							   where s1.acctno = ss.acctno
							   --and DATEADD(d,-10,s1.dateexpiry) <= GETDATE())
							   and DATEADD(d,-10,s1.dateexpiry) <= @rundate)			--IP - 13/02/12 - use @rundate
	from SPASummary ss
	INNER JOIN spa s ON ss.Acctno = s.acctno
	AND s.payfreq = 'F'
	
	--If todays date is greater than the last instalment date (arrangement has expired) then we still need to return a record
	--for an account that has an SPA.
	
	insert into SPASummary (Acctno, Dateadded, Empeeno, NextDateExpiryDue, ArrangementDateExpiry, ArrangementAmt, TotalSPAInstalDue)
	select s.Acctno,
		   s.Dateadded,
		   s.EmpeeNo,
		   MAX(s.dateexpiry),
		   MAX(s.dateexpiry),
		   0,
		   0
	from spa s INNER JOIN acct a on s.acctno = a.acctno
	where not exists(select * from spa s1
					--where s1.dateexpiry > GETDATE()
					where s1.dateexpiry > @rundate				--IP - 13/02/12 - use @rundate
					and s1.acctno = s.acctno)
		and a.currstatus!='S'			-- jec 27/04/10
	group by s.acctno, s.dateadded, s.EmpeeNo
	--having MAX(s.dateexpiry) < GETDATE()
	having MAX(s.dateexpiry) < @rundate							--IP - 13/02/12 - use @rundate
			
	
	--Update the ArrangementAmt    
	update SPASummary
	set ArrangementAmt = (select ISNULL(sum(s1.spainstal),123.45) from spa s1
							where s1.acctno = ss.acctno)
	from SPASummary ss
	
	--Update the TotalSPAInstalDue
	update SPASummary
	set TotalSPAInstalDue = (select ISNULL(sum(s1.spainstal),123.45) from spa s1
							   where s1.acctno = ss.acctno
							   and s1.dateexpiry <= ss.NextDateExpiryDue)
	from SPASummary ss
	
    
 IF (@@error != 0)
    BEGIN
        SET @return = @@error
    END
GO
SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO

-- End End End End End End End End End End End End End End End End End End End End End End End End End End End End End 

