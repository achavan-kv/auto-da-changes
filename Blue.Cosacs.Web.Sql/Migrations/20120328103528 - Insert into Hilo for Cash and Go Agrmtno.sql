-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

DECLARE @maxAgrmtNo int

SET @maxAgrmtNo = (select MAX(agrmtno) from agreement ag INNER JOIN custacct ca on ag.acctno = ca.acctno
where ag.acctno like '___5%' and agrmtno>1 and custid like 'Paid & Taken%')


set @maxAgrmtNo=ceiling(@maxAgrmtNo/1000000.00) * 1000000 + 1


IF NOT EXISTS(select * from HiLo where sequence = 'CashAndGoAgrmtNo')
BEGIN
	INSERT INTO HiLo
	SELECT 'CashAndGoAgrmtNo', @maxAgrmtNo, 100
END


