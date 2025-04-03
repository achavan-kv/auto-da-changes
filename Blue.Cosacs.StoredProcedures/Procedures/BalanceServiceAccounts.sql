IF EXISTS (SELECT * FROM sysobjects WHERE TYPE = 'P' AND NAME = 'BalanceServiceAccounts')
DROP PROCEDURE BalanceServiceAccounts
GO

CREATE PROCEDURE BalanceServiceAccounts 
@return INT OUTPUT 
AS
SET NOCOUNT ON 
SET @return = 0

DECLARE 
		@balance MONEY,
		@hobranchno SMALLINT,
		@hirefno int

DECLARE @config TABLE 
(
	id VARCHAR(100),
	account VARCHAR(25),
	code VARCHAR(3)
)

INSERT INTO @config
(id, account,code)
SELECT id,valueString, 'SRI'
FROM config.Setting
WHERE id IN ('InstallationStockAccount','ServiceStockAccount')

SELECT * FROM  @config

DECLARE @transtypeError VARCHAR(4000)

SELECT @transtypeError = 'Transaction type ' + transtypecode + ' has a blank interface account - this needs correcting'
       FROM transtype 
       INNER JOIN @config c ON c.code = transtype.transtypecode
       AND (interfaceaccount = '' OR interfacebalancing='')

IF LEN(@transtypeError) = 0
BEGIN
	RAISERROR (@transtypeError, 14, 1)	
END

SELECT @hobranchno = hobranchno FROM country
SELECT @hirefno=hirefno FROM branch WHERE branchno = @hobranchno
	
INSERT INTO fintrans 
	(origbr, branchno, acctno, transrefno, 
	datetrans, transtypecode, empeeno, transupdated, 
	transprinted, transvalue, ftnotes, paymethod, runno,
source)
SELECT 0,@hobranchno,f.acctno, @hirefno + ROW_NUMBER() OVER (ORDER by f.acctno),
	   GETDATE(),c.code,99999,'Y',
	   'Y',SUM(f.transvalue) * -1,'BSAC',0,0,
	    'COSACS' -- Code Added
FROM fintrans f
INNER JOIN @config c ON c.account = f.acctno
GROUP BY f.acctno, c.code

UPDATE dbo.branch
SET hirefno = hirefno + @@rowcount
WHERE branchno = @hobranchno

UPDATE acct 
SET outstbal = 0 
FROM @config c 
WHERE c.account = acct.acctno

create table #acctstoreverse(acctno char(12), balance money,transrefno int)

insert into #acctstoreverse select acctno, outstbal,0 from acct a, code c
where a.acctno= c.reference and category ='SRSUPPLIER' and a.outstbal >0 and a.acctno !=''

--insert into #acctstoreverse select acctno, outstbal,0 from acct a, countrymaintenance c
--where a.acctno= c.value  and c.name in ('Service Stock Account','Service Warranty Account')  and a.outstbal >0
declare @acctno char(12)
select @hobranchno = hobranchno from country
declare @transrefno int,@count int
select @count = isnull(count(*),0) + 1 from #acctstoreverse
update #acctstoreverse set balance = 
	isnull((select sum(transvalue) from fintrans f where f.acctno=#acctstoreverse.acctno),0)

if @count > 1
begin
	update branch set hirefno = hirefno + @count where branchno = @hobranchno
	select @transrefno= hirefno from branch where branchno = @hobranchno
   DECLARE trans_cursor CURSOR 
  	FOR SELECT acctno ,transrefno
   from #acctstoreverse order by acctno desc
   
   OPEN trans_cursor
   FETCH NEXT FROM trans_cursor INTO @acctno,@transrefno

   WHILE (@@fetch_status <> -1)
   BEGIN
        IF (@@fetch_status <> -2)
   	begin

		update #acctstoreverse set transrefno = @transrefno where acctno= @acctno
		set @transrefno = @transrefno -1       
   	
	END
      FETCH NEXT FROM trans_cursor INTO @acctno,@transrefno

   END

   CLOSE trans_cursor
   DEALLOCATE trans_cursor


	insert into fintrans
	(      origbr,      branchno,      acctno,      transrefno,
      datetrans,      transtypecode,      empeeno,      transupdated,
      transprinted,      transvalue,      bankcode,      bankacctno,
      chequeno,      ftnotes,      paymethod,      runno,
	source)
	select @hobranchno,        @hobranchno,        acctno,        transrefno,
        getdate(),        'SRS',        99999,        'Y',
        'Y',        -balance,         '',         '',
	'',	'Bnet',	0,	0,
 	'COSACS'
        FROM #acctstoreverse

	update acct 
	set outstbal = 0 
	from #acctstoreverse b
	where acct.acctno = b.acctno

	update sr_servicerequest set status = 'C' where status ='R' and exists (select * from acct a where
	a.acctno= sr_servicerequest.acctno and a.acctno !='' and a.outstbal = 0 and a.currstatus ='S')
end

GO


