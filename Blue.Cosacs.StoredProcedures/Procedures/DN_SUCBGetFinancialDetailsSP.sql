SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_SUCBGetFinancialDetailsSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_SUCBGetFinancialDetailsSP]
GO
CREATE PROCEDURE 	dbo.DN_SUCBGetFinancialDetailsSP
-- ================================================
-- Project      : CoSACS .NET
-- File Name    : DN_SUCBGetFinancialDetailsSP.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : 
-- Author       : ??
-- Date         : ??
--
--
-- 
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 20/02/12  IP #9423 - CR8262 - Flag Store Card transactions and order ensuring regular
--			    transactions are displayed first
-- ================================================
			@runno int,
			@return int OUTPUT

AS
	SET 	@return = 0			--initialise return code
    -- now going to store details in temporary table -- so that we can get oracle interface details back as well...
	CREATE TABLE #totals (interfaceaccount VARCHAR(8), acctno CHAR(12),transtypecode VARCHAR(3), datetrans DATETIME, transvalue MONEY, branchno SMALLINT, category INT , 
	transrefno INT, itemno VARCHAR(18), storecard char(1))			--IP - 20/02/12 - #9423 - CR8262
	
	INSERT INTO #totals (interfaceaccount, acctno , transtypecode ,datetrans,
	transvalue ,branchno,transrefno, storecard)						--IP - 20/02/12 - #9423 - CR8262
	SELECT 	t.interfaceaccount, acctno,		f.transtypecode,		datetrans,
		transvalue,		SUBSTRING(acctno,1,3) as branchno, transrefno, case when substring(acctno, 4,1) = '9' then 'Y'				--IP - 20/02/12 - #9423 - CR8262
																			else 'N' end
	FROM	fintrans f
    LEFT OUTER JOIN transtype t
    ON f.transtypecode = t.transtypecode
	WHERE	runno = @runno
        and t.transtypecode != 'REP'
	   and exists (select * from 
	   interface_financial i
		where i.runno = f.runno 
            and i.transtypecode = f.transtypecode 
            AND I.BrokerExclude != 1)

		BEGIN -- create #stockitem table and update tax rate to 0- if agreement tax type exclusive
		DECLARE @agreementtaxtype CHAR(1),@taxrate FLOAT

		SELECT itemno,stocklocn,unitpricecash,unitpricehp,category,taxrate
		 INTO #stockitem FROM stockitem

		CREATE CLUSTERED INDEX ixdserdsfsf ON #stockitem(itemno,stocklocn)

		SELECT 
			   @agreementtaxtype=agrmttaxtype,
			   @taxrate = taxrate
		 FROM country
		 IF @agreementtaxtype ='E' -- if exclusive then we have a separate tax line so we don't want to remove tax from delivery items
			UPDATE #stockitem SET taxrate = 0
		END 
    -- 
	declare @code varchar(6), @branchno SMALLINT 
	declare BrokerX_cursor CURSOR FAST_FORWARD READ_ONLY FOR
	SELECT b.code, f.branchno
	FROM BrokerOracleSetup b, interface_financial f 
	WHERE f.transtypecode = b.code AND f.runno = @runno AND F.BrokerExclude != 1
	AND b.code NOT IN ('BHI','bhs','bha','BTX')
	GROUP BY b.code, f.branchno
	OPEN BrokerX_cursor
	FETCH NEXT FROM BrokerX_cursor INTO @code,@branchno
	WHILE @@FETCH_STATUS = 0
	BEGIN

	EXEC BrokerInterfaceQuery @type ='D' ,@code =@code ,  @runno =@runno, @balance = 'N', @branchno= @branchno
	-- doing balancing transaction as well
	EXEC BrokerInterfaceQuery @type ='D' ,@code =@code ,  @runno =@runno, @balance = 'Y', @branchno= @branchno

	FETCH NEXT FROM BrokerX_cursor INTO @code ,@branchno


	END

	CLOSE BrokerX_cursor
	DEALLOCATE BrokerX_cursor

	--Get decimal places country maintenance setting
DECLARE @rawDecimalsSetting VARCHAR(1500),
        @decimals INT

SELECT @rawDecimalsSetting = [Value] FROM CountryMaintenance WHERE CodeName = 'decimalplaces'
BEGIN TRY
    SET @decimals = CAST(SUBSTRING(@rawDecimalsSetting, 2, LEN(@rawDecimalsSetting) - 1) AS INT)
END TRY
BEGIN CATCH
    SET @decimals = 2
END CATCH

	-- now we have to populate the insurance service charge and tax values for the run... 
	DECLARE @mindelrunno INT , 
            @maxdelrunno INT,
            @BrokerExRunno INT 

	SELECT --@finintstart = finintstart,
	@maxdelrunno= DelIntEnd,
	@mindelrunno = DelIntStart,
    @BrokerExRunno = BrokerRunno
	 FROM brokerExtracthist 
	 WHERE finintend = @runno
	EXEC BrokerPopulateCreditAccts @mindelrunno = @mindelrunno, @maxdelRunno = @mindelrunno, @decimals = @decimals, @runno = @BrokerExRunno ,@query =1



--SELECT * FROM #totals	
SELECT * FROM #totals order by transtypecode, storecard 			--20/02/12 - #9423 - CR8262
/*For testing totals match
SELECT SUM(transvalue  ),interfaceaccount,transtypecode,branchno  FROM #totals
GROUP BY interfaceaccount,transtypecode ,branchno */
	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END

GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO
/* test
declare @P1 int
set @P1=0
exec DN_SUCBGetFinancialDetailsSP @runno = 514, @return = @P1 output
select @P1*/
