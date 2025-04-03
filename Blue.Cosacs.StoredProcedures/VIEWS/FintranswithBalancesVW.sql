IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.tables WHERE table_name ='fintranswithBalancesVW')
DROP VIEW fintranswithBalancesVW 
GO 
CREATE VIEW fintranswithBalancesVW 
AS SELECT f.transvalue , SUM(fa.transvalue ) AS total,f.datetrans,f.acctno, f.transtypecode , pd.InterestRate, a.outstbal AS Balance,pd.Status 
FROM fintrans f ,fintrans fa
JOIN acct a ON fa.acctno = a.acctno
LEFT JOIN storecardpaymentdetails pd ON fa.acctno = pd.acctno
WHERE f.acctno= fa.acctno  
and fa.datetrans <= (SELECT max(m.datetrans) FROM fintrans m WHERE m.acctno= fa.acctno AND m.datetrans<=f.datetrans)
GROUP BY f.transvalue,f.datetrans,f.acctno,f.transtypecode ,pd.InterestRate,a.outstbal ,pd.Status
GO 
