
if  exists (select * from sysobjects where name = 'vw_cashieroutstandingincome')
drop view vw_cashieroutstandingincome
go
CREATE VIEW 	vw_cashieroutstandingincome 
AS 

SELECT	empeeno,
		depositoutstanding, 
		paymethod, 
		branchno,
		0 as localchange
FROM		cashieroutstanding WHERE  CONVERT(INT,paymethod) !=13 
UNION
SELECT 	empeeno, 				/* this is local currency transactions */
		sum (-transvalue), 
		paymethod, 
		branchno,
		0 as localchange
FROM 		fintrans_new_income
WHERE	Convert(int,paymethod) < 100 AND CONVERT(INT,paymethod) !=13 -- exclude storecard....
GROUP BY 	empeeno, paymethod, branchno
UNION					
SELECT	FTN.empeeno,				/* for foreign currency transactions must use the foreign tender amount */
		sum(FTE.foreigntender),
		FTN.paymethod, 
		FTN.branchno,
		sum(FTE.localchange)  as localchange
FROM 		fintrans_new_income FTN
		INNER JOIN fintransexchange FTE ON FTN.acctno = FTE.acctno 
		AND FTN.transrefno = FTE.transrefno
WHERE	Convert(int,FTN.paymethod) >= 100 
GROUP BY 	FTN.empeeno, FTN.paymethod, FTN.branchno

GO
