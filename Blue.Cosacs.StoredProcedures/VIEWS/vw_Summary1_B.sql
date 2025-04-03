/****** Object:  View [dbo].[vw_Summary1_B]    Script Date: 01/30/2012 12:03:31 ******/
IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[vw_Summary1_B]'))
DROP VIEW [dbo].[vw_Summary1_B]
GO

/****** Object:  View [dbo].[vw_Summary1_B]    Script Date: 01/30/2012 12:03:31 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- Accttypegroup
create view [dbo].[vw_Summary1_B]	--	WITH SCHEMABINDING		-- removed jec
as
 select a.acctno,a.accttype,accttypegroup=case
  WHEN s.acctno IS NOT NULL THEN 'SGR'		-- Singer Account
  --WHEN v.acctno IS NOT NULL THEN 'AST'	--#19510	-- Assist Account
  when ca.custid like 'PAID%' then 'PT' 
  WHEN c.acctno IS NOT NULL THEN 'CLN' 
  when at.accttype in ('C','S') then 'C'  
  when at.accttype ='R' then 'RF'  
  when at.accttype in ('B','D','E','F','G', 'M', 'H', 'O') then 'HP'    
  WHEN at.accttype = 'T' THEN 'SC'
  when a.termstype = 'WC' /* Warranty on credit */  
   AND right(left(a.acctno,4),1) = '5' then 'PT'  
  else ' '  
  end   
 from dbo.acct a 
	INNER JOIN dbo.custacct ca 
		ON a.acctno=ca.acctno
		and hldorjnt='H'
	LEFT OUTER JOIN cashloan c ON c.acctno = ca.acctno and c.custid = ca.custid
	LEFT OUTER JOIN SingerAcct s on s.acctno = ca.acctno
	--LEFT OUTER JOIN vw_Summary1_1 v on v.acctno = ca.acctno		--#19510 --  Assist accounts
	INNER JOIN dbo.accttype at 
	-- note: there may not be a custacct
	on (a.accttype=at.accttype
	or a.accttype=at.genaccttype)

GO


