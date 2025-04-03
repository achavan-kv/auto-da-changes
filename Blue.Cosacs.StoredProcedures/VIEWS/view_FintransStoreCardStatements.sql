IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.tables WHERE table_name ='view_FintransStoreCardStatements')
DROP VIEW view_FintransStoreCardStatements
GO 
CREATE VIEW view_FintransStoreCardStatements 
AS 
SELECT f.branchno AS BranchNo, f.acctno AS AcctNo, f.datetrans AS DateTrans, f.transtypecode AS Code, t.description AS [Description], 
f.empeeno AS Empeeno , f.transvalue AS Value,
x.acctnoxfr AS TransferAccount ,x.acctname AS Name,x.agrmtno ,x.storecardno AS CardNumber, b.branchname
FROM fintrans f
INNER JOIN transtype t ON f.transtypecode= t.transtypecode 
INNER JOIN branch b ON b.branchno = f.branchno
LEFT JOIN finxfr x ON f.acctno = x.acctno and f.transrefno = x.transrefno  AND f.datetrans = x.datetrans 
GO 


