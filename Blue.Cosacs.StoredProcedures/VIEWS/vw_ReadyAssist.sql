
IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[vw_ReadyAssist]'))
DROP VIEW [dbo].[vw_ReadyAssist]
GO

-- Ready Assist Accounts
create view [dbo].[vw_ReadyAssist]	
-- this view is no longer used in vw_Summary1_B >> Summary1SP
as
 select distinct l.acctno,a.accttype
 from acct a 
	INNER JOIN lineitem l 
		ON a.acctno=l.acctno
	INNER JOIN ReadyAssistDetails ra				
		ON l.acctno = ra.acctno 
		and l.agrmtno = ra.agrmtno 
		and l.itemid = ra.itemid
		and l.contractno = ra.contractno
 where 
 l.quantity>0 --#14417 --l.itemno  like 'READY%'
 --and exists(select * from code where category = 'RDYAST'
	--		 and code.code = l.itemno)
 and l.agrmtno = 1  --#14417

GO


