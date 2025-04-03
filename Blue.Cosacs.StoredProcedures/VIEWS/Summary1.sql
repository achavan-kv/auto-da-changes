/****** Object:  View [dbo].[Summary1]    Script Date: 01/30/2012 16:37:43 ******/
IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[Summary1]'))
DROP VIEW [dbo].[Summary1]
GO

/****** Object:  View [dbo].[Summary1]    Script Date: 01/30/2012 16:37:43 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

create view [dbo].[Summary1]		-- WITH SCHEMABINDING 
as
Select c.countrycode,a.branchno,b.branchname,ca.custid,a.acctno,a.accttype,
		a.agrmttotal,a.dateacctopen,a.datelastpaid,
		CASE WHEN accttype = 'T' 
			THEN 'T' 
			ELSE a.currstatus
			END as currstatus,a.outstbal,a.arrears,a.dateintoarrears,a.paidpcent,
		isnull(i.instalamount,0) as instalamount,isnull(i.instalno,0) as instalno,
		isnull(i.datelast,'1900-01-01') as datelast,a.termstype,a.securitised, 
		CASE WHEN accttype = 'T' 
			THEN 0
			ELSE(agrmttotal - delamount) 
			END as followamount,
		mr.acctchar4,mr.accttypegroup,mr.acctlife,mr.outstbalcorr,mr.dayssincereposs,mr.spaflag,mr.datespaadded,
		mr.datedel1,mr.delamount,mr.monthsarrears,mr.baldue12mths,mr.baldueafter12mths,mr.payamount,mr.fullydeliveredflag,
		mr.monthsarrearsnew,			
		mr.outstbalcorrnew,mr.bdw,mr.cancelledflag,mr.servicechg,mr.deposit,mr.acctstatusdescr,
		mr.Datefullydelivered,		
		mr.dateagrmtrevised,mr.deliveryflag,isnull(mr.daysarrears,0) as daysarrears,mr.StatusCodeBand			-- jec 12/12/08

	From dbo.acct a left outer join dbo.instalplan i on a.acctno=i.acctno,dbo.custacct ca,
		dbo.branch b,dbo.country c,	--,dbo.vw_Summary1_B vB
		Summary1_MR mr			-- jec 15/12/08 was  temp_Summary1_MR???
	Where a.acctno=ca.acctno and ca.hldorjnt='H'
		and a.branchno=b.branchno
		--and a.acctno=vB.acctno
		and a.acctno=mr.acctno
		
-- 3m 19s Jamaica

GO

