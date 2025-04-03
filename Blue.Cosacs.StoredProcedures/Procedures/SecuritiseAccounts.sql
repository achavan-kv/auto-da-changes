SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

IF EXISTS(SELECT * FROM sysobjects WHERE NAME = 'SecuritiseAccounts')
DROP PROCEDURE SecuritiseAccounts
GO
CREATE PROCEDURE [dbo].[SecuritiseAccounts]
--------------------------------------------------------------------------------  
--  
-- Project      : Securitisation Reports  
-- File Name    : SPROC_SecuritiseAccounts.sql  
-- File Type    : MSSQL Server Stored Procedure Script  
-- Title        : Modification to exclude Staff Accounts from Securitisation  
-- Author       : D Richardson  
-- Date         : 29 April 2004  
--  
-- Change Control  
-- --------------  
-- Date      By  Description  
-- ----      --  -----------  
-- 12/5/04   PN  Exclude Accounts that have been written off   
-- 10/05/04  AA  including other accounts with instalment plan  
-- 12/03/08  RDB I have added Malaysian Functionality and optional max receivable limit 
-- 28/05/08  JEC Include Repossessions in Delivery Total  
--               exclude time from GETDATE when checking payment date
-- 07/06/08  AA  Changing to 60 days from 61 as per Malaysia Instructions
-- 09/06/08  RDB CR947 Principal Factor Override
-- 09/07/08  AA  Securitising add-to accounts also including transfers as part of the initial deposit calculation
-- 07/09/08  SL  Include TRC transactions with Int and adm
-- 09/02/10 jec - Malaysia Merge - New in 5.2
--------------------------------------------------------------------------------  
  
       @empeeno int,  
    @runno int   
AS  
  
  
BEGIN  
 DECLARE @status integer,  
   @countrycode char(1)  
  
   
 DECLARE @maxPrincipal MONEY  
 DECLARE @spv money  
 DECLARE @ReceivablesLimit MONEY  
 --CR947 RDB 9/6/08 added
 DECLARE @PrincipalFactorOverride MONEY
  
 select @maxPrincipal = CONVERT(MONEY, value) FROM CountryMaintenance WHERE codename = 'maxprinciple'  
 select @spv = CONVERT(MONEY, value) FROM CountryMaintenance WHERE codename = 'spv'  

IF  @spv <> -1 
	SET @spv = @spv /100  
 select @ReceivablesLimit = CONVERT(MONEY, value) FROM CountryMaintenance WHERE codename = 'careceivables'  
  
 IF @ReceivablesLimit IS NULL  
  SET @ReceivablesLimit = -1  

SELECT @PrincipalFactorOverride = CONVERT(MONEY, value) FROM CountryMaintenance WHERE codename = 'principalfactoroverride'




 --CR947 RDB 9/6/08 either @spv is null or @PrincipalFactorOverride is null
-- if not exit
IF @spv = -1 OR @PrincipalFactorOverride = -1
BEGIN

    CREATE TABLE #sec_accts  
    (  
  acctno char(12),   
  outstbal MONEY,
  monthsremaining INT null,
  instalment MONEY null,
  principalfactor FLOAT null,
  saleprice MONEY  null,charges MONEY null
  
 )  
  
    SELECT @countrycode = countrycode   
  FROM country  
      
    IF @countrycode = ('S')  
    BEGIN  
  INSERT INTO #sec_accts  (acctno,outstbal)
  SELECT a.acctno, a.outstbal  
  FROM acct a, termstypetable t  
  WHERE a.outstbal > 0  
	  AND a.currstatus NOT IN ('S', '9')  
	  AND SUBSTRING(a.acctno, 4, 1) in ('0','1','2','3','6','8','9')  
	  AND a.securitised != 'Y'  
	  AND NOT EXISTS(SELECT acctno  
		  FROM   acctcode ac  
		  WHERE  a.acctno = ac.acctno  
		  AND    ac.code = 'STAF')  
	  AND a.agrmttotal = (SELECT SUM(transvalue)  
				  FROM   fintrans f  
				  WHERE  a.acctno = f.acctno  
				  AND    transtypecode IN('DEL', 'GRT', 'ADD'))  
	  AND a.termstype = t.termstype  
	  AND t.DoNotSecuritise = 0 -- CR884  
       
  SET @status =@@error  
 END  
 ELSE IF @countrycode = ('Y')  
 BEGIN  
  --Malaysia  
  INSERT INTO #sec_accts (acctno,outstbal)  
  SELECT a.acctno, a.outstbal  
   FROM acct a  
		INNER join termstypetable t  
		 ON a.termstype = t.termstype  
		INNER JOIN instalplan ip  
		 ON a.acctno = ip.acctno  
		INNER JOIN agreement ag  
		 ON a.acctno = ag.acctno  
   WHERE ((a.outstbal - isnull( (SELECT SUM(transvalue)	FROM fintrans 
				WHERE fintrans.acctno = a.acctno 
					and transtypecode IN ('int','adm','trc')),0)) > 0) --  calculate interest and admin totals  
    AND a.currstatus NOT IN ('S', '9')   
    AND SUBSTRING(a.acctno, 4, 1) in ('0','1','2','3','6','8','9')  
    AND a.securitised != 'Y'  
    AND NOT EXISTS(SELECT acctno FROM  acctcode ac  
		WHERE  a.acctno = ac.acctno  
			AND    ac.code = 'STAF')  
    AND a.agrmttotal = (SELECT SUM(transvalue)  
           FROM   fintrans f  
           WHERE  a.acctno = f.acctno  
           AND    transtypecode IN('DEL', 'GRT', 'ADD', 'REP'))		-- jec 28/05/08 include repossessions
    AND t.DoNotSecuritise = 0 -- CR884  
    -- maximum agreement length 60 months  
    AND ip.instalno <= 60
	and ip.instalamount <> 0
    AND (  
		 --months in arrears level < 2.5  
		 ((a.arrears - ISNULL(( SELECT SUM(transvalue) FROM fintrans 
						WHERE fintrans.acctno = a.acctno 
							and transtypecode IN ('int','adm','trc') ),0))/ip.instalamount ) <2.5  
		 OR  
		 --months in arrears level < 4.5 AND payment made wihin 61 days  
		 (  
		 ((a.arrears - ISNULL(( SELECT SUM(transvalue) FROM fintrans 
						WHERE fintrans.acctno = a.acctno 
							and transtypecode IN ('int','adm','trc') ),0))/ip.instalamount ) <4.5  
			AND  
		  EXISTS(SELECT SUM(transvalue) FROM fintrans   
				WHERE fintrans.acctno = a.acctno  
					AND transtypecode IN ('PAY','COR','REF','RET','XFR','SCX')  
					AND datetrans >= DATEADD(dd,-60, CONVERT(datetime,CONVERT(CHAR(10), GETDATE() ,  103),103))  -- jec 28/05/08 remove time from date
				HAVING SUM(transvalue) < 0 )
		 )
		) 
   -- principal sum does not exceed 100,000  
   AND ( ag.agrmttotal - ag.serviceChg - ag.deposit ) <= @maxPrincipal  
  
   --one instalement must be paid  OR  add-to account
   AND ((   
       -(SELECT SUM(transvalue) FROM fintrans   
			WHERE fintrans.acctno = a.acctno   
				AND transtypecode IN ('PAY','COR','REF','RET','XFR','SCX')) >= (ISNULL(ag.deposit,0) + ISNULL(ip.instalamount,0))  
    )
    or 			   EXISTS(SELECT SUM(transvalue) FROM fintrans   
				WHERE fintrans.acctno = a.acctno  
					AND transtypecode IN ('ADD')  
				HAVING SUM(transvalue) > 0 ) )  
  SET @status =@@error  
  --Mayalsia end  
  
 END  
 ELSE  
 BEGIN  
	  INSERT INTO #sec_accts  (acctno,outstbal)
	  SELECT a.acctno, a.outstbal  
	  FROM acct a  
	  WHERE a.outstbal > 0  
		  AND a.currstatus NOT IN ('S', '9')  
		  AND SUBSTRING(a.acctno, 4, 1) in ('0','1','2','3','6','8','9')  
		  AND a.securitised != 'Y'  
		  AND NOT EXISTS(SELECT acctno  
			FROM   acctcode ac  
				WHERE  a.acctno = ac.acctno  
				AND    ac.code = 'STAF')  
		  AND a.agrmttotal = (SELECT SUM(transvalue)  
			 FROM   fintrans f  
				 WHERE  a.acctno = f.acctno  
				 AND    transtypecode IN('DEL', 'GRT', 'ADD'))  
	    
	  SET @status =@@error  
 END   
    
 IF @status = 0  
 BEGIN  
  CREATE CLUSTERED INDEX ixdersec_actts on #sec_accts(acctno)  
  set @status =@@error  
 END  
   
 IF @status = 0  
 BEGIN  
  DELETE FROM #sec_accts   
  WHERE EXISTS(SELECT acctno  
            FROM fintrans f  
               WHERE #sec_accts.acctno = f.acctno  
               AND f.transtypecode = 'BDW')  
  OR NOT EXISTS(SELECT *   
       FROM instalplan   
       WHERE instalplan.acctno = #sec_accts.acctno)  
    
  SET @status =@@error  
 END 


  UPDATE #sec_accts SET charges = ISNULL((SELECT SUM(f.transvalue) FROM fintrans f WHERE f.acctno=#sec_accts.acctno
  AND f.transtypecode IN ('int','adm','trc')),0)

  UPDATE #sec_accts  
  SET instalment =i.instalamount 
  FROM instalplan i
  WHERE #sec_accts.acctno= i.acctno


  
  UPDATE #sec_accts SET monthsremaining = (outstbal-ISNULL(charges,0))/instalment WHERE  instalment >0
  --CR947 RDB 9/6/08 use PrincipalFactorOverride if set up
	
  IF @PrincipalFactorOverride <> -1.00
BEGIN
		UPDATE #sec_accts SET principalfactor = @PrincipalFactorOverride
end
  ELSE
	BEGIN
		UPDATE #sec_accts SET principalfactor =(1-(@spv *monthsremaining/2))
	END
		
		
  UPDATE #sec_accts SET saleprice = (outstbal-charges)* principalfactor
  

   
  
IF @ReceivablesLimit < 0   
begin  
 IF @status = 0  
 BEGIN		
	--26/06/08 rdb balance to include any charges
  INSERT INTO  sec_account(acctno, runno,balance, datesecuritised,empeenorun,principalfactor,saleprice)  
  SELECT s.acctno, @runno, s.outstbal - ISNULL(s.charges,0), getdate(), @empeeno  ,principalfactor,saleprice
  FROM #sec_accts s  
   
  SET @status =@@error  
 END  
   
 IF @status = 0  
 BEGIN  
  UPDATE acct  
  SET  securitised = 'Y'  
  FROM #sec_accts  
  WHERE acct.acctno = #sec_accts.acctno  
      
  SET @status =@@error  
 END  
  
   
 IF @status = 0  
 BEGIN  
  SELECT SUM(outstbal), COUNT(*)  
  FROM #sec_accts  
      
  SET @status =@@error  
 END  
    
  
END   
ELSE  --so @receivableslimit >0
BEGIN 
  DECLARE @acctno VARCHAR(12) 
-- 06/05/08 The @ReceivablesLimit is not per run
-- it should be a maximum for all accounts that exist
-- we need to go through all the accts in sec_Account
-- and calculate their principal balance based on there 
-- current outstanding balance
	DECLARE @currOutstBal MONEY
	DECLARE @principalFactor MONEY 
	DECLARE @currcharges MONEY
	DECLARE @currSalePrice MONEY

	DECLARE @TotalSecuritised MONEY
	SET @TotalSecuritised = 0 
   
   
   SELECT a.outstbal,s.principalfactor,ISNULL(SUM(f.transvalue),0) AS charges  ,a.acctno
   INTO #sectotals
   FROM sec_account s 
   JOIN acct a ON s.acctno = a.acctno
   LEFT JOIN fintrans f ON a.acctno = f.acctno AND f.transtypecode IN ('int','adm','trc')
   GROUP BY a.outstbal,s.principalfactor ,a.acctno
   
   SELECT @TotalSecuritised = ISNULL(SUM((outstbal- charges) *principalfactor),0) FROM #sectotals
	
 --loop through accounts and add until Receivables Limit is reached  
 
  DECLARE @outstbal MONEY  
  DECLARE @runningTotal MONEY  
  DECLARE @Count int  
  DECLARE @salePrice money  
 
  
  DECLARE @MonthsRemaining MONEY  
  DECLARE @InstalAmount MONEY  
  DECLARE @charges  money  
   
  SET @runningTotal = 0  
  SET @count = 0  
  
  SET NOCOUNT ON
  DECLARE c1 CURSOR READ_ONLY FOR SELECT acctno, outstbal,saleprice, principalFactor,charges FROM #sec_accts  
  
  OPEN c1  
   FETCH NEXT FROM c1 INTO @acctno,@outstbal ,@salePrice ,@principalFactor,@charges
   WHILE @@FETCH_STATUS = 0  
   BEGIN  
      
    -- increase running total
	-- 06/05/08 code moved up and amended from below as we need to calculate 
	-- salePrice earlier and use instead of outstandingbalance against receivables limit
	
--     update  sec_account  
--      SET salePrice = @salePrice, principalFactor = @principalFactor  
--     WHERE acctno = @acctno  

     -- 06/05/08 added TotalSecuritised and chaged to saleprice
    --IF (@runningTotal + @outstbal ) <= @ReceivablesLimit  
	IF (@runningTotal + @salePrice + @TotalSecuritised) <= @ReceivablesLimit  
    BEGIN  
		--PRINT 'insert row'
     --SET @runningTotal = @runningTotal + @outstbal  
		SET @runningTotal = @runningTotal + @salePrice  
		SET @count = @count + 1  
  
		INSERT INTO  sec_account  
			(acctno, runno,balance, datesecuritised,empeenorun, salePrice, principalFactor)  
		values   
			(@acctno, @runno, (@outstbal - ISNULL(@charges,0)), getdate(), @empeeno, @salePrice, @principalFactor)  
  
		SET @status =@@error  
  
		IF @status = 0  
		BEGIN  
			UPDATE acct  
			SET  securitised = 'Y'  
			WHERE acct.acctno = @acctno  
			SET @status =@@error  
		END  
    
    END

    ELSE  
	BEGIN
		PRINT 'break'
		break  
	END  
     
  
    FETCH NEXT FROM c1 INTO  @acctno,@outstbal  ,@salePrice,@principalFactor,@charges
   END  
  
  CLOSE c1  
  DEALLOCATE c1  
  
    
  IF @status = 0  
 BEGIN  
  SELECT @runningTotal, @count   
    
      
  SET @status =@@error  
 END  
 END  
  
 return @status  
END  
  
  END


go
-- End End End End End End End End End End End End End End End End End End End End End End End End End End

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

