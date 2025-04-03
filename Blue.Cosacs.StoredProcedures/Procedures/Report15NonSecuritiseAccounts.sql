/*
** Author	: A. Ayscough and K. Fernandez (Strategic Thought)
** Date		: 18-May-2004
** Version	: 1.0
** Name		: dbo.Report15NonSecuritiseAccounts.PRC
** Details	: CR603 - To populate Summary Table 7
**                Report to show non-securitised accounts and reason why
**
** Who  Date     Change
** ---	----	 ------								
** AA   20/05/04 If there are no accounts securitised then removing all fully delivered accounts
*/
-- 25/07/11 jec CR1254 RI Integration
--=========================================================================================
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[Report15NonSecuritiseAccounts]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[Report15NonSecuritiseAccounts]
GO

CREATE PROCEDURE Report15NonSecuritiseAccounts

    @return int out

AS
BEGIN
    SET @return = 0

    truncate table summary7

    insert into summary7 (acctno, branchno, branchname, dateacctopen, agrmttotal, currstatus, reason,
		          deliverytotal, outstbal, datedel, empeenosale, empeename, asatdate)
    select	a.acctno, a.branchno, branchname, dateacctopen, a.agrmttotal, currstatus, '',
 	        0, isnull(a.outstbal,0), g.datedel, g.empeenosale, u.FullName , getdate()
    from	acct a , instalplan i, agreement g, Admin.[User] u, branch b
    where	a.acctno =i.acctno
    and		a.securitised != 'Y'
    and		a.currstatus != 'S'
    and		g.acctno = i.acctno
    and		g.agrmtno = 1 
    and		g.empeenosale = u.id
    and		a.branchno = b.branchno

    update	summary7
    set		deliverytotal = (select isnull (sum (transvalue), 0)
    from	fintrans
    where	fintrans.acctno = summary7.acctno
    and		fintrans.transtypecode in ('DEL', 'GRT', 'ADD'))

/* if there are absolutely no securitised accounts remove all fully delivered ones */
if not exists (select top 1 * from sec_account)
   delete from summary7 where deliverytotal = agrmttotal

update	summary7
SET	reason = 'Unpaid'
where	currstatus ='U'

update	summary7
set	reason = 'Awaiting credit sanctioning'
where	currstatus ='0'

update	summary7
set	reason = 'Awaiting delivery authorisation' 
from	agreement
where	reason is null
and	agreement.acctno = summary7.acctno
and	agreement.holdprop != 'N'

update	summary7
set	reason = 'Scheduled-awaiting delivery'
where	exists (select * from schedule where schedule.acctno = summary7.acctno)

update	summary7
set	reason = reason +'-Awaiting delivery note print '
where	exists (select * from lineitem, stockitem
		where lineitem.ItemId = stockitem.ItemId							-- RI lineitem.itemno = stockitem.itemno
		and	lineitem.stocklocn = stockitem.stocklocn
		and	summary7.acctno = lineitem.acctno
		and	lineitem.quantity > 0
		and	lineitem.qtydiff= 'Y'
		and	stockitem.itemtype ='S')

    update	summary7
    set		reason = reason + '-Stockitems all delivered check non-stocks/delivery total/collections'
    where	not exists (select * from schedule where schedule.acctno = summary7.acctno)
    and		deliverytotal != agrmttotal
    and		deliverytotal > 0
    and		not exists (select lineitem.* from lineitem, stockitem
			    where	lineitem.ItemId = stockitem.ItemId							-- RI lineitem.itemno = stockitem.itemno
			    and		lineitem.stocklocn = stockitem.stocklocn 
			    and		lineitem.acctno = summary7.acctno and lineitem.quantity > 0
		  	    and		stockitem.itemtype = 'S'
			    and		not exists (select * from delivery 
		    			    where	lineitem.acctno = delivery.acctno
					    and	lineitem.ItemId = delivery.ItemId						-- RI lineitem.itemno = delivery.itemno 
					    and		lineitem.stocklocn = delivery.stocklocn) )


    delete from summary7 where outstbal<= 0 and agrmttotal= deliverytotal

    delete from summary7 where exists (select * from fintrans
				   where	fintrans.acctno = summary7.acctno
				   and		fintrans.transtypecode = 'BDW')

    delete from summary7  where acctno In ('','000000000000')

/* Create index */
    if exists (select * from sysindexes where name = 'ix_summary7')
        DROP INDEX [summary7].[ix_summary7]

    CREATE CLUSTERED INDEX [ix_summary7] ON [summary7] ([acctno]);

    SET @return = @@ERROR
    RETURN @return
END
go 

-- End End End End End End End End End End End End End End End End End End End End End End End End End End End End End End End End End End 
