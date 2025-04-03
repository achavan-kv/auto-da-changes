IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.tables WHERE table_name ='CashierTotalsView')
DROP VIEW CashierTotalsView 
GO 

CREATE VIEW CashierTotalsView AS
-- **********************************************************************
-- Title: CashierTotalsView.sql
-- Developer: Alex
-- Date: 6th December 2011
-- Purpose: Cashier Totals

-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 19/12/11 jec/IP  #8914 An entry is made under disbursement for store card transaction in cashier totals screen.
-- 03/01/12 IP   #8473 - Cashier Deposits gift vouchers. 
-- 21/02/12 IP   #9602 - Disbursement(Tax-Exempt) was incorrectly appearing under Remittance. 
-- 14/05/12 IP   #10113 - LW75009 - include DPY transtype in Payments and exclude from Disbursements.
-- ********************************************************************** 

WITH  CTE AS 
(
			SELECT	 FT.empeeno,
					FT.transtypecode,
					C.codedescript ,
					FT.paymethod,
					-SUM(FT.transvalue) AS transvalue ,
					isnull(SUM(FTE.foreigntender),0) as ForeignTender,
					isnull(SUM(FTE.localchange),0) as LocalChange ,
					0 AS RUNNO ,
					0 AS ISdeposit,
					ft.datetrans,
					--CONVERT(BIT,c.additional2) AS AllowOS,
					case when Len(c.additional2) >=1 and Substring(c.additional2,1,1) = '0' then Convert(BIT,0)				--IP - 03/01/12 - #8473 - CR1234 
					else Convert(BIT, 1) end as AllowOS,
					CONVERT(BIT,0) AS IncludeInCashierTotals
			FROM		code C	LEFT JOIN fintrans_new_income FT ON
					FT.paymethod = C.code 
					  LEFT OUTER JOIN fintransexchange FTE ON
					FT.acctno = FTE.acctno AND
					FT.datetrans = FTE.datetrans AND
					FT.transrefno = FTE.transrefno INNER JOIN acct A ON
					FT.acctno = A.acctno
					WHERE C.category = 'FPM'
					GROUP BY ft.empeeno,ft.transtypecode ,c.codedescript,ft.paymethod, ft.datetrans ,c.additional2
			UNION 
			SELECT	CD.empeeno,
					CD.code,
					C.codedescript,
					CD.paymethod,
					-SUM(cd.depositvalue) as transvalue,
					0,
					0 ,
					CD.runno,
					t.isDeposit ,
					cd.datedeposit,
					--CONVERT(BIT,c.additional2) AS AllowOS,						
					case when Len(c.additional2) >=1 and Substring(c.additional2,1,1) = '0' then Convert(BIT,0)				--IP - 03/01/12 - #8473 - CR1234 
					else Convert(BIT, 1) end as AllowOS,
					CONVERT(BIT,cd.includeincashiertotals) 
			FROM	CashierDeposits CD
			LEFT OUTER JOIN transtype T ON CD.code = T.transtypecode
			INNER JOIN code C	ON
					CD.paymethod = C.code AND
					C.category = 'FPM'  AND cd.includeincashiertotals = 1
					
			WHERE	CD.datevoided is NULL

			GROUP BY CD.empeeno,CD.code,C.codedescript,CD.paymethod,cd.runno,t.isdeposit, cd.datedeposit,c.additional2,cd.includeinCashierTotals
)		
			 --SELECT * FROM CTE WHERE EMPEENO= @employeeno
			SELECT	n.empeeno,n.paymethod ,n.codedescript ,n.foreigntender,n.localchange,
			--Payments = CASE transtypecode WHEN 'PAY' THEN SUM(transvalue) ELSE 0 END,
			Payments = CASE WHEN transtypecode IN ('PAY','DPY') THEN SUM(transvalue) ELSE 0 END,	--IP - 14/05/12 - #10113 - LW75009
			Corrections = CASE transtypecode WHEN 'COR' THEN SUM(transvalue) ELSE 0 END,
			Refunds = CASE transtypecode WHEN 'REF' THEN SUM(transvalue) ELSE 0 END,
			CASE WHEN ISDEPOSIT  in (2,4,6)  THEN SUM(-transvalue) ELSE 0 END AS PettyCash,
			--Remittance =CASE WHEN IsDeposit IN (5,8) THEN SUM(-transvalue) ELSE 0 END,
			Remittance =CASE WHEN IsDeposit IN (8) THEN SUM(-transvalue) ELSE 0 END,				--IP - 21/02/12 - #9602 - UAT83
			Allocation =CASE WHEN IsDeposit =1 THEN SUM(-transvalue) ELSE 0 END,
			--Disbursement=CASE WHEN IsDeposit IN(3,7) -- disbursments can also include cash loans
			Disbursement=CASE WHEN IsDeposit IN(3,5,7) -- disbursments can also include cash loans	--IP - 21/02/12 - #9602 - UAT83
			--OR (isdeposit = 0 AND transtypecode NOT IN ('PAY','COR','REF','RET') ) THEN SUM(-transvalue) ELSE 0 END,		
				OR (isdeposit = 0 AND transtypecode NOT IN ('PAY','COR','REF','RET','DPY') ) THEN SUM(-transvalue) ELSE 0 END,	--IP - 14/05/12 - #10113 - LW75009
			NetReceipts =  SUM(transvalue),
			--CASE WHEN ISDEPOSIT = 0 THEN SUM(transvalue) ELSE 0 END ,
			 datetrans ,
			 AllowOS,
			 IncludeInCashierTotals
			FROM CTE N
			where transtypecode NOT IN ('SCT','STR')  -- #8914 not Storecard  
			GROUP BY n.empeeno,n.paymethod ,n.codedescript,n.transtypecode,isdeposit,datetrans
,n.foreigntender,n.localchange,AllowOS,n.IncludeInCashierTotals

GO 

-- End End End End End End End End End End End End End End End End End End End End End End End End End End End End
