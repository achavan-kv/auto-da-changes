if exists (select * from dbo.sysobjects where id = object_id('[dbo].[BrokerExtractSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
DROP PROCEDURE [dbo].[BrokerExtractSP] 
GO
/****** Object:  StoredProcedure [dbo].[BrokerExtractSP]    Script Date: 11/29/2018 12:44:20 PM ******/
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER OFF
GO
-- ===============================================================================================
-- Version:		<010> 
-- ===============================================================================================
CREATE PROCEDURE [dbo].[BrokerExtractSP] 

-- ===============================================================================================
-- Project      : CoSACS .NET
-- File Name    : BrokerExtractSP.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Procedure that extracts data FROM Interface_Financial for the Broker Extract
-- Author       : Ilyas Parker
-- Date         : 08 September 2008
--
--
-- Change Control
-- --------------
-- Date      By           Description
-- ----      --           -----------
--08/09/08   Ilyas Parker CREATEd Procedure
--13/10/08   Alex Ayscough Multiple Changes 
-- 12/01/08  Alex Ayscough & Steve Chong 70553 Adding extra column to broker for category.
-- 06/05/09  Alex Ayscough allowing 5 characters for Broker Extract.
--07-07-11	Ruth - Update for RI 
-- 17/10/11 jec  #8406 LW74094 - CoSACS to RI - Flagging Re-run
--03-11-11  AA - Update to prevent error if failure in referred interface.
--12/12/11  ip - #8876 - Incorrect reference to Stockitem.Itemno. Should be ItemID
--23/12/11  ip - #8944 - Change Broker Interface to send cash price for BHP
-- 30/04/12 jec - #9607 CR9372 - broker cost of sale transactions
-- 30/04/12 jec - #9890 CR9510 - returned warranty broker COS
-- 04/05/12 ip  - #10040 - Merged #9485 - LW74206 from version 5.13	
-- 04/10/12 ip  - #11085 - LW75337 - Cost of Sale - Warranty Interface - Cost of Warranty Return split
--				 out to COW transtype.
-- 29/05/13 ip  - #13662 - BHI Broker
-- 17/03/14 ip  - #17622 - BHI not interfaced where Insurance not included in Service Charge
-- 11/07/14 ip  - #18612 - CR15594 - Ready Assist transactions
-- 07/05/19 CP - Included DT in BTX calculation(DT was in Not In clause; that is modified)
-- 31/10/19 AA - Modified BTX for restrcting tax dedution from the tax amount
-- 11/02/20 CP - Modified to net off tax from BHR and BCR
-- 12/02/20 CP - Modified to exclude Ready Assist from BHP and BCP
-- 12/02/20 CP - Modified to include Ready Assist in BTX
-- 25/02/20 CP - Modified to net off tax for CRC and CNR transactions 
-- 18/05/20 CP - Modified to fix BHP and BSD
-- 18/05/20 CP - Modified to fix BHD
-- 26/05/20 CP - Modified to fix BTX to exclude category 90
-- 29/06/20 CP - Modified BHP to rectify collection values
-- 13/07/20 CP - Modified BHP to rectify identical replacement values
-- ===================================================================================================
	-- Add the parameters for the stored procedure here
		@return int output

as
	SET @return=0		--initialise return code

SET nocount on 

declare @countryCode varchar(1), 
		@agreementtaxtype CHAR(1),
		@taxrate FLOAT, 
		@stocktaxtype char(1),				--IP - 23/12/11 - #8944 - Merged from 5.13.5.D
		@lastBrokerRunDate datetime, --last 'datefinish' when 'BrokerX' was run.
		@minUpdsmryRunno int,
		@maxUpdsmryRunno int,
		@minDelRunno INT,
		@maxDelRunno INT,
		@runno int,
		@status INT, 
		@insuranceitemno VARCHAR(10),
		@adminitemno VARCHAR(10),
        @isPriceFromStockLocation BIT
DECLARE @countryletter CHAR(1)

SELECT @countryletter = countrycode FROM COUNTRY

SELECT @isPriceFromStockLocation = [Value] FROM CountryMaintenance cm WHERE cm.CodeName = 'pricefromlocn'

SELECT @insuranceitemno= Value FROM CountryMaintenance WHERE codename LIKE 'insitemno'		-- #9607 use CodeName
SELECT @adminitemno=Value FROM CountryMaintenance WHERE codename LIKE 'adminitemno'
SET @status = 0

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

SELECT ItemNo, ItemID,stocklocn,unitpricecash,unitpricehp,category,taxrate,
		IUPC,CostPrice,itemtype,refcode			-- #9890
INTO #stockitem FROM stockitem

CREATE CLUSTERED INDEX ixdserdsfsf ON #stockitem(ItemId,stocklocn)

SELECT @countryCode = countrycode,
	   @agreementtaxtype=agrmttaxtype,
	   @taxrate = taxrate,
	   @stocktaxtype = taxtype																				--IP - 23/12/11 - #8944 - Merged from 5.13.5.D
 FROM country
 
 if @stocktaxtype = 'I'																						--IP - 23/12/11 - #8944 - Merged from 5.13.5.D
	update #stockitem 
	set unitpricecash = ROUND((unitpricecash*100)/(100+taxrate), @decimals),
		unitpricehp = ROUND((unitpricehp*100)/(100+ taxrate), @decimals)
		
 IF @agreementtaxtype ='E' -- if exclusive then we have a separate tax line so we don't want to remove tax from delivery items
	UPDATE #stockitem SET taxrate = 0
	
--SELECT the datefinish of the last 'BrokerX' run.

SELECT @lastBrokerRunDate =  max(datefinish) FROM interfacecontrol WHERE interface = 'BROKERX' AND result = 'P'


SELECT @runno = MAX(RUNNO) FROM interfacecontrol WHERE interface= 'BROKERX' 

IF EXISTS (SELECT * FROM interfacecontrol i WHERE datestart > @lastbrokerrundate AND interface IN ('COS FACT', 'CoSACS2RI','UPDSMRY') AND Result !='P'
AND NOT EXISTS (SELECT * FROM interfacecontrol i2 WHERE datestart > i.DateStart 
AND i.Interface = i2.Interface AND i.RunNo = i2.RunNo AND i2.Result ='P' ))
BEGIN 
   RAISERROR ('Broker Aborting as End of day failure in export since last run',16,1)
   RETURN 
END 
--SELECT the minimum runno for 'UPDSMRY' after the last time the 'BrokerX' was run.
SELECT @minUpdsmryRunno =  min(runno) FROM interfacecontrol WHERE interface = 'UPDSMRY' AND result In ('P','W')
						 AND datestart > @lastBrokerRunDate

--SELECT the maximum runno for 'UPDSMRY' after the last time the 'BrokerX' was run.
SELECT @maxUpdsmryRunno = ISNULL(max(runno),0) FROM interfacecontrol WHERE interface = 'UPDSMRY'  
						 AND datestart > @lastBrokerRunDate AND result In ('P','W')


SELECT @minDelRunno =ISNULL(min(runno),0) FROM interfacecontrol WHERE interface in ('COS FACT', 'CoSACS2RI')
						 AND datestart > @lastBrokerRunDate
						 and ISNULL(ReRunTimes,0)=0			-- #8406 jec

--SELECT the maximum runno for 'UPDSMRY' after the last time the 'BrokerX' was run.
SELECT @maxDelRunno = ISNULL(max(runno),0) FROM interfacecontrol WHERE interface in ('COS FACT', 'CoSACS2RI')
						 AND datestart > @lastBrokerRunDate
						 and ISNULL(ReRunTimes,0)=0			-- #8406 jec
						 
print '@minUpdsmryRunno ' + convert(varchar, @minUpdsmryRunno)
print '@maxUpdsmryRunno ' + convert(varchar, @minUpdsmryRunno)
print '@minDelRunno ' + convert(varchar, @minDelRunno)
print '@maxDelRunno ' + convert(varchar, @maxDelRunno)

--SELECT @minDelRunno AS mindel , @maxDelRunno AS maxdel
DELETE FROM BrokerExtractHist WHERE BrokerRunno = @runno
INSERT INTo BrokerExtractHist (BrokerRunno ,
FinIntStart , FinIntEnd ,
DelIntStart ,DelIntEnd)
VALUES ( @runno,
@minUpdsmryRunno,@maxUpdsmryRunno,
@minDelRunno, @maxDelRunno)

--Temporary TABLE to hold the 'BrokerX' data to be extracted.
CREATE TABLE #BrokerData
(
	countrycode varchar(1),
	runno int,
	interfaceaccount varchar(10),
	branchno smallint,
	transtypecode varchar(3),
	transvalue money,
	date varchar(10), 
	category SMALLINT
)

--SELECT the data FROM 'Interface_financial' based on the min AND max runno's 
--for the 'UPDSMRY' interface since the last 'BrokerX' run.
begin
	if(@status = 0)
	begin
		insert into #BrokerData
		SELECT @countryCode, 
		@runno,  
		datenextdue = case WHEN DATALENGTH(interfaceaccount)=8 THEN RIGHT(interfaceaccount,5)-- removing branch from front... 
		ELSE 
		RIGHT(interfaceaccount,4)
		end, 
		branchno,
		transtypecode,
		ROUND(transvalue, @decimals),
		'',
		NULL
		FROM interface_financial
		WHERE runno between @minUpdsmryRunno AND @maxUpdsmryRunno
		 AND BrokerExRunNo = 0 
		AND transtypecode NOT IN ('REB','DEL','REP','RDL','GRT',-- exclude delivery type transactions as these will be loaded later.
		'CRE', 'CRF',			-- #15167 - exclude Warranty Return percentage
		'BCW','BHW','BHA','BHS','BTX','BHP','BHD','BHI','BCC','BCP','BHC',	-- exclude any previously written in case of failure
		'BHR', 'BRS', 'CRA', 'CNR', 'BCR', 'CRC',                           --#18612 - exclude Ready Assist transactions. These will be loaded later.
        'BOR', 'BAS', 'CNA', 'BCA', 'CAC', 'SRI','COS')	                                -- exclude Annual Service Contract transactions. These will be loaded later.
        AND isnull(BrokerExclude,0) = 0             --Exclude ServiceChargeAcct transactions 
	SET @status = @@error
	end

--if the date of the run was after midnight up until 8am the next day then use the 
--date as the previous day else use the datestart of the run.
	if (@status = 0)
	begin
		update #BrokerData 
		SET date = (SELECT case when datepart(hh, i.datestart) >= 0 AND datepart(hh, i.datestart) <=7 AND datepart(hh, i.datefinish) <=7
				   then replace(convert(varchar(10), dateadd(d,-1,i.datestart), 120), '-', '') 
				   when datepart(hh, i.datestart) >= 0 AND datepart(hh, i.datestart) <=7 AND datepart(hh, i.datefinish) =8
						AND datepart(mi, i.datefinish) =00
				   then replace(convert(varchar(10), dateadd(d,-1,i.datestart), 120), '-', '') 
			       else
					replace(convert(varchar(10), i.datestart, 120), '-', '') 
					end)
		FROM interfacecontrol i
		WHERE i.runno = #BrokerData.runno
		AND interface = 'UPDSMRY'

	SET @status = @@error

	end

	-- Now do the delivery interface transactions
-- delivery normal items 
		
	
	------Added on 18052020------------------------------------------------Start-------
	--DECLARE @countryletter CHAR(1)
	--SELECT @countryletter = countrycode FROM COUNTRY
	IF @countryletter = 'J' 
		BEGIN
			insert into #BrokerData  
			SELECT			
				@countryCode,   
				@runno,    
				right(interfaceaccount,4),  
				CONVERT(INT,LEFT(d.acctno,3)),	-- Lets just do the branch for the account numbers for now???
				t.transtypecode,  
				--SUM((ROUND((ROUND(d.transvalue, 2) * 100) / (100 + s.taxrate), 2) * d.quantity)),  ---Charudatt 22Oct2018

				CASE 
					WHEN SUM(ROUND(ISNULL(pp.unitprice,spa.CashPrice), @decimals)) > 0 
					THEN SUM(ROUND(((ISNULL(pp.unitprice,spa.CashPrice)* 100)/ (100 + s.taxrate)), @decimals)  * d.quantity)
					ELSE SUM((ROUND((ROUND(d.transvalue, @decimals) * 100) / (100 + s.taxrate), @decimals) ))
				END, 
				'',  
				s.category  
			FROM transtype t, delivery d 
			INNER JOIN acct a 
			on d.acctno = a.acctno   
			LEFT JOIN #stockitem s ON d.itemid = s.ItemId 
			AND s.stocklocn = CASE @isPriceFromStockLocation
                                WHEN 1 THEN d.stocklocn
                                ELSE LEFT(d.acctno, 3) 
                              END
			LEFT JOIN LineItem la
			ON d.acctno = la.acctno 
			AND d.itemno = la.itemno
			AND d.stocklocn = la.stocklocn 
			AND CONVERT(VARCHAR,d.datedel,103) = CONVERT(VARCHAR,la.datereqdel,103)
			LEFT JOIN StockPriceAudit spa 
			ON spa.ID = d.ItemID 
			and spa.BranchNo = CASE @isPriceFromStockLocation
                                WHEN 1 THEN d.stocklocn
                                ELSE LEFT(d.acctno, 3) 
                           END
			and spa.DateChange = (select MAX(datechange)
                                from StockPriceAudit spa2
                                where spa2.ID = spa.ID
                                and spa2.BranchNo = spa.BranchNo
                                and spa2.DateChange <= 
														CASE WHEN d.delorcoll = 'D' 
															Then
																CASE WHEN ISNULL(la.notes,'') like 'Repl%'
																Then
																	(select MAX(d2.datetrans) 
																	from delivery d2
																	where d2.acctno = d.acctno
																	and d2.datetrans < d.datetrans
																	and d2.ItemID = d.ItemID
																	and d2.stocklocn = d.stocklocn
																	and d2.contractno = d.contractno
																	and d2.ParentItemID = d.ParentItemID
																	and d2.delorcoll = 'D') 
																
																ELSE
																	(select MAX(d2.datetrans) 
																	from delivery d2
																	where d2.acctno = d.acctno
																	and d2.datedel=d.datedel
																	and d2.ItemID = d.ItemID
																	and d2.stocklocn = d.stocklocn
																	and d2.contractno = d.contractno
																	and d2.ParentItemID = d.ParentItemID
																	and d2.delorcoll = 'D')
																END
															WHEN d.delorcoll = 'C' 
															Then
															(select MAX(d2.datetrans) 
                                                            from delivery d2
                                                            where d2.acctno = d.acctno
															and d2.datetrans < d.datetrans
                                                            and d2.ItemID = d.ItemID
                                                            and d2.stocklocn = d.stocklocn
                                                            and d2.contractno = d.contractno
                                                            and d2.ParentItemID = d.ParentItemID
                                                            and d2.delorcoll = 'D')
															ELSE
															(select MAX(d2.datetrans) 
                                                            from delivery d2
                                                            where d2.acctno = d.acctno
															and d2.datedel=d.datedel
                                                            and d2.ItemID = d.ItemID
                                                            and d2.stocklocn = d.stocklocn
                                                            and d2.contractno = d.contractno
                                                            and d2.ParentItemID = d.ParentItemID
                                                            and d2.delorcoll = 'D')
															END )
			LEFT JOIN promoprice pp 
			ON d.ItemID = pp.ItemID 
            and pp.stocklocn = CASE @isPriceFromStockLocation
                                WHEN 1 THEN d.stocklocn
                                ELSE LEFT(d.acctno, 3) 
                               END
            and ((select MAX(d2.DateChange) 
                  from LineItemaudit d2
                  where d2.acctno = d.acctno
                      and d2.ItemID = d.ItemID
                      and d2.stocklocn = d.stocklocn
                      and d2.contractno = d.contractno
                      and d2.ParentItemID = d.ParentItemID
                      and d2.valueafter>0) >= pp.fromdate
                                                            
            and (select MAX(d2.DateChange) 
                 from LineItemaudit d2
                 where d2.acctno = d.acctno
                     and d2.ItemID = d.ItemID
                     and d2.stocklocn = d.stocklocn
                     and d2.contractno = d.contractno
                     and d2.ParentItemID = d.ParentItemID
                      and d2.valueafter>0) <= pp.todate)
            and pp.hporcash = 
                case
                    when a.accttype = 'C' then 'C'
					else 'H'
                end
			WHERE  runno between @minDelRunno AND @maxDelRunno
			AND t.transtypecode ='BHP'  
			AND D.acctno LIKE '___0%' AND s.category NOT IN (select code from code where category = 'WAR')  
			AND S.Category NOT IN (90,96)  
			AND s.category NOT IN ( select code from code where category = 'PCDIS') -- discounts   
			AND d.delorcoll !='R' AND d.itemno NOT IN (@adminitemno,@insuranceitemno,'RB','SD' )  ------Charudatt added SD to eclude it from BHP 
      		AND d.BrokerExRunNo = 0  --Charudatt
			AND ISNULL(d.quantity,0) != 0
			AND NOT EXISTS(select 1 from ReadyAssistDetails ra                          --Exclude Ready Assist Items -----Added by Charudatt on 11022020
                           where ra.AcctNo = d.acctno
                          -- and ra.AgrmtNo = d.agrmtno
                           and ra.ItemId = d.ItemID)
			GROUP BY  right(interfaceaccount,4),  
			CONVERT(INT,LEFT(d.acctno,3)),	-- Lets just do the branch for the account numbers for now???
			t.transtypecode,  
			s.category,d.transvalue
		
	  
			IF @@ERROR = 0  
				BEGIN  
					PRINT 'done stage 1 -bhp'  
				END  
			ELSE  
				BEGIN  
					PRINT 'error at stage 1-bhp '  
			END  
    
			insert into #BrokerData  
			SELECT			
				@countryCode,   
				@runno,    
				 LEFT(t.interfacebalancing,2) + CONVERT(VARCHAR,S.category), 
				--right(interfaceaccount,4),  
				CONVERT(INT,LEFT(d.acctno,3)),	-- Lets just do the branch for the account numbers for now???
				t.transtypecode,  
				--SUM(-1 * (ROUND((ROUND(d.transvalue, 2) * 100) / (100 + s.taxrate), 2) * d.quantity))  , ---Charudatt 22102018
				CASE WHEN SUM(ROUND(ISNULL(pp.unitprice,spa.CashPrice), @decimals)) > 0 
				THEN SUM(-1 * ROUND(((ISNULL(pp.unitprice,spa.CashPrice)* 100)/ (100 + s.taxrate)), @decimals)  * d.quantity)
				ELSE SUM( -1 *  (ROUND((ROUND(d.transvalue, @decimals) * 100) / (100 + s.taxrate), @decimals) ))
				END, 
				'',  
				s.category  
			FROM transtype t, delivery d 
			INNER JOIN acct a 
			on d.acctno = a.acctno   
			LEFT JOIN #stockitem s ON d.itemid = s.ItemId 
			AND s.stocklocn = CASE @isPriceFromStockLocation
                                WHEN 1 THEN d.stocklocn
                                ELSE LEFT(d.acctno, 3) 
                              END
			LEFT JOIN LineItem la
			ON d.acctno = la.acctno 
			AND d.itemno = la.itemno
			AND d.stocklocn = la.stocklocn 
			AND CONVERT(VARCHAR,d.datedel,103) = CONVERT(VARCHAR,la.datereqdel,103)
			LEFT JOIN StockPriceAudit spa 
			ON spa.ID = d.ItemID 
			and spa.BranchNo = CASE @isPriceFromStockLocation
                                WHEN 1 THEN d.stocklocn
                                ELSE LEFT(d.acctno, 3) 
                           END
			and spa.DateChange = (select MAX(datechange)
                                from StockPriceAudit spa2
                                where spa2.ID = spa.ID
                                and spa2.BranchNo = spa.BranchNo
                                and spa2.DateChange <= 
														CASE WHEN d.delorcoll = 'D' 
															Then
																CASE WHEN ISNULL(la.notes,'') like 'Repl%'
																Then
																	(select MAX(d2.datetrans) 
																	from delivery d2
																	where d2.acctno = d.acctno
																	and d2.datetrans < d.datetrans
																	and d2.ItemID = d.ItemID
																	and d2.stocklocn = d.stocklocn
																	and d2.contractno = d.contractno
																	and d2.ParentItemID = d.ParentItemID
																	and d2.delorcoll = 'D') 
																
																ELSE
																	(select MAX(d2.datetrans) 
																	from delivery d2
																	where d2.acctno = d.acctno
																	and d2.datedel=d.datedel
																	and d2.ItemID = d.ItemID
																	and d2.stocklocn = d.stocklocn
																	and d2.contractno = d.contractno
																	and d2.ParentItemID = d.ParentItemID
																	and d2.delorcoll = 'D')
																END
															WHEN d.delorcoll = 'C' 
															Then
															(select MAX(d2.datetrans) 
                                                            from delivery d2
                                                            where d2.acctno = d.acctno
															and d2.datetrans < d.datetrans
                                                            and d2.ItemID = d.ItemID
                                                            and d2.stocklocn = d.stocklocn
                                                            and d2.contractno = d.contractno
                                                            and d2.ParentItemID = d.ParentItemID
                                                            and d2.delorcoll = 'D')
															ELSE
															(select MAX(d2.datetrans) 
                                                            from delivery d2
                                                            where d2.acctno = d.acctno
															and d2.datedel=d.datedel
                                                            and d2.ItemID = d.ItemID
                                                            and d2.stocklocn = d.stocklocn
                                                            and d2.contractno = d.contractno
                                                            and d2.ParentItemID = d.ParentItemID
                                                            and d2.delorcoll = 'D')
															END )
			LEFT JOIN promoprice pp 
			ON d.ItemID = pp.ItemID 
            and pp.stocklocn = CASE @isPriceFromStockLocation
                                WHEN 1 THEN d.stocklocn
                                ELSE LEFT(d.acctno, 3) 
                               END
			and ((select MAX(d2.DateChange) 
                  from LineItemaudit d2
                  where d2.acctno = d.acctno
                      and d2.ItemID = d.ItemID
                      and d2.stocklocn = d.stocklocn
                      and d2.contractno = d.contractno
                      and d2.ParentItemID = d.ParentItemID
                      and d2.valueafter>0) >= pp.fromdate
                                                            
            and (select MAX(d2.DateChange) 
                 from LineItemaudit d2
                 where d2.acctno = d.acctno
                     and d2.ItemID = d.ItemID
                     and d2.stocklocn = d.stocklocn
                     and d2.contractno = d.contractno
                     and d2.ParentItemID = d.ParentItemID
                      and d2.valueafter>0) <= pp.todate)
            and pp.hporcash = 
                case
                    when a.accttype = 'C' then 'C'
					else 'H'
                end
			WHERE  runno between @minDelRunno AND @maxDelRunno
			AND t.transtypecode ='BHP'  
			AND D.acctno LIKE '___0%' AND s.category NOT IN (select code from code where category = 'WAR')  
			AND S.Category NOT IN (90,96)  
			AND s.category NOT IN ( select code from code where category = 'PCDIS') -- discounts   
			AND d.delorcoll !='R' AND d.itemno NOT IN (@adminitemno,@insuranceitemno,'RB','SD' )  ------Charudatt added SD to eclude it from BHP 
			AND d.BrokerExRunNo = 0  --Charudatt
			AND ISNULL(d.quantity,0) != 0
            		AND NOT EXISTS(select 1 from ReadyAssistDetails ra                          --Exclude Ready Assist Items
                           where ra.AcctNo = d.acctno
                          -- and ra.AgrmtNo = d.agrmtno
                           and ra.ItemId = d.ItemID)
			GROUP BY
			LEFT(t.interfacebalancing,2) + CONVERT(VARCHAR,S.category),  
			--right(interfaceaccount,4),  
			CONVERT(INT,LEFT(d.acctno,3)),	-- Lets just do the branch for the account numbers for now???
			t.transtypecode, s.category,d.transvalue


			IF @@ERROR = 0  
				BEGIN  
					PRINT 'done stage 2 -bhp'  
				END  
			ELSE  
				BEGIN  
					PRINT 'error at stage 2-bhp '  
				END  
		END 
	ELSE 
		BEGIN
			insert into #BrokerData  
			SELECT			
				@countryCode,   
				@runno,    
				right(interfaceaccount,4),  
				CONVERT(INT,LEFT(d.acctno,3)),	-- Lets just do the branch for the account numbers for now???
				t.transtypecode,  
				--SUM((ROUND((ROUND(d.transvalue, 2) * 100) / (100 + s.taxrate), 2) * d.quantity)),  ---Charudatt 22Oct2018

				CASE 
					WHEN SUM(ROUND(ISNULL(pp.unitprice,spa.CashPrice), @decimals)) > 0 THEN 
					SUM(ROUND(((ISNULL(pp.unitprice,spa.CashPrice)* 100)/ (100 + s.taxrate)), @decimals)  * d.quantity)
					ELSE SUM((ROUND((ROUND(d.transvalue, @decimals) * 100) / (100 + s.taxrate), @decimals) ))
				END, 
				'',  
				s.category  
			FROM transtype t, delivery d 
			INNER JOIN acct a 
			on d.acctno = a.acctno   
			LEFT JOIN #stockitem s ON d.itemid = s.ItemId 
			AND s.stocklocn = CASE @isPriceFromStockLocation
                                WHEN 1 THEN d.stocklocn
                                ELSE LEFT(d.acctno, 3) 
                              END
			LEFT JOIN LineItem la
			ON d.acctno = la.acctno 
			AND d.itemno = la.itemno
			AND d.stocklocn = la.stocklocn 
			AND CONVERT(VARCHAR,d.datedel,103) = CONVERT(VARCHAR,la.datereqdel,103)
			LEFT JOIN StockPriceAudit spa 
			ON spa.ID = d.ItemID 
			and spa.BranchNo = CASE @isPriceFromStockLocation
                                WHEN 1 THEN d.stocklocn
                                ELSE LEFT(d.acctno, 3) 
                           END
			and spa.DateChange = (select MAX(datechange)
                                from StockPriceAudit spa2
                                where spa2.ID = spa.ID
                                and spa2.BranchNo = spa.BranchNo
                                and spa2.DateChange <= 
															CASE WHEN d.delorcoll = 'D' 
															Then
																CASE WHEN ISNULL(la.notes,'') like 'Repl%'
																Then
																	(select MAX(d2.datetrans) 
																	from delivery d2
																	where d2.acctno = d.acctno
																	and d2.datetrans < d.datetrans
																	and d2.ItemID = d.ItemID
																	and d2.stocklocn = d.stocklocn
																	and d2.contractno = d.contractno
																	and d2.ParentItemID = d.ParentItemID
																	and d2.delorcoll = 'D') 
																
																ELSE
																	(select MAX(d2.datetrans) 
																	from delivery d2
																	where d2.acctno = d.acctno
																	and d2.datedel=d.datedel
																	and d2.ItemID = d.ItemID
																	and d2.stocklocn = d.stocklocn
																	and d2.contractno = d.contractno
																	and d2.ParentItemID = d.ParentItemID
																	and d2.delorcoll = 'D')
																END
															WHEN d.delorcoll = 'C' 
															Then
															(select MAX(d2.datetrans) 
                                                            from delivery d2
                                                            where d2.acctno = d.acctno
															and d2.datetrans < d.datetrans
                                                            and d2.ItemID = d.ItemID
                                                            and d2.stocklocn = d.stocklocn
                                                            and d2.contractno = d.contractno
                                                            and d2.ParentItemID = d.ParentItemID
                                                            and d2.delorcoll = 'D')
															ELSE
															(select MAX(d2.datetrans) 
                                                            from delivery d2
                                                            where d2.acctno = d.acctno
															and d2.datedel=d.datedel
                                                            and d2.ItemID = d.ItemID
                                                            and d2.stocklocn = d.stocklocn
                                                            and d2.contractno = d.contractno
                                                            and d2.ParentItemID = d.ParentItemID
                                                            and d2.delorcoll = 'D')
															END )
			LEFT JOIN promoprice pp 
			ON d.ItemID = pp.ItemID 
            and pp.stocklocn = CASE @isPriceFromStockLocation
                                WHEN 1 THEN d.stocklocn
                                ELSE LEFT(d.acctno, 3) 
                               END
            and ((select MAX(d2.DateChange) 
                  from LineItemaudit d2
                  where d2.acctno = d.acctno
                      and d2.ItemID = d.ItemID
                      and d2.stocklocn = d.stocklocn
                      and d2.contractno = d.contractno
                      and d2.ParentItemID = d.ParentItemID
                      and d2.valueafter>0) >= pp.fromdate
                                                            
            and (select MAX(d2.DateChange) 
                 from LineItemaudit d2
                 where d2.acctno = d.acctno
                     and d2.ItemID = d.ItemID
                     and d2.stocklocn = d.stocklocn
                     and d2.contractno = d.contractno
                     and d2.ParentItemID = d.ParentItemID
                      and d2.valueafter>0) <= pp.todate)
            and pp.hporcash = 
                case
                    when a.accttype = 'C' then 'C'
					else 'H'
                end
			WHERE  runno between @minDelRunno AND @maxDelRunno
			AND t.transtypecode ='BHP'  
			AND D.acctno LIKE '___0%' AND s.category NOT IN (select code from code where category = 'WAR')  
			AND S.Category NOT IN (90,96)  
			AND s.category NOT IN ( select code from code where category = 'PCDIS') -- discounts   
			AND d.delorcoll !='R' AND d.itemno NOT IN (@adminitemno,@insuranceitemno,'RB' )  
      			AND d.BrokerExRunNo = 0  --Charudatt
			AND ISNULL(d.quantity,0) != 0
			AND NOT EXISTS(select 1 from ReadyAssistDetails ra                          --Exclude Ready Assist Items -----Added by Charudatt on 11022020
                           where ra.AcctNo = d.acctno
                          -- and ra.AgrmtNo = d.agrmtno
                           and ra.ItemId = d.ItemID)
			GROUP BY  right(interfaceaccount,4),  
			CONVERT(INT,LEFT(d.acctno,3)),	-- Lets just do the branch for the account numbers for now???
			t.transtypecode,  
			s.category,d.transvalue
		
	  
			IF @@ERROR = 0  
				BEGIN  
					PRINT 'done stage 1 -bhp'  
				END  
			ELSE  
				BEGIN  
					PRINT 'error at stage 1-bhp '  
			END  
    
			insert into #BrokerData  
			SELECT			
				@countryCode,   
				@runno,    
				 LEFT(t.interfacebalancing,2) + CONVERT(VARCHAR,S.category), 
				--right(interfaceaccount,4),  
				CONVERT(INT,LEFT(d.acctno,3)),	-- Lets just do the branch for the account numbers for now???
				t.transtypecode,  
				--SUM(-1 * (ROUND((ROUND(d.transvalue, 2) * 100) / (100 + s.taxrate), 2) * d.quantity))  , ---Charudatt 22102018
				CASE WHEN SUM(ROUND(ISNULL(pp.unitprice,spa.CashPrice), @decimals)) > 0 
				THEN SUM(-1 * ROUND(((ISNULL(pp.unitprice,spa.CashPrice)* 100)/ (100 + s.taxrate)), @decimals)  * d.quantity)
				ELSE SUM( -1 *  (ROUND((ROUND(d.transvalue, @decimals) * 100) / (100 + s.taxrate), @decimals) ))
				END, 
				'',  
				s.category  
			FROM transtype t, delivery d 
			INNER JOIN acct a 
			on d.acctno = a.acctno   
			LEFT JOIN #stockitem s ON d.itemid = s.ItemId 
			AND s.stocklocn = CASE @isPriceFromStockLocation
                                WHEN 1 THEN d.stocklocn
                                ELSE LEFT(d.acctno, 3) 
                              END
			LEFT JOIN LineItem la
			ON d.acctno = la.acctno 
			AND d.itemno = la.itemno
			AND d.stocklocn = la.stocklocn 
			AND CONVERT(VARCHAR,d.datedel,103) = CONVERT(VARCHAR,la.datereqdel,103)
			LEFT JOIN StockPriceAudit spa 
			ON spa.ID = d.ItemID 
			and spa.BranchNo = CASE @isPriceFromStockLocation
                                WHEN 1 THEN d.stocklocn
                                ELSE LEFT(d.acctno, 3) 
                           END
			and spa.DateChange = (select MAX(datechange)
                                from StockPriceAudit spa2
                                where spa2.ID = spa.ID
                                and spa2.BranchNo = spa.BranchNo
                                and spa2.DateChange <= 
															CASE WHEN d.delorcoll = 'D' 
															Then
																CASE WHEN ISNULL(la.notes,'') like 'Repl%'
																Then
																	(select MAX(d2.datetrans) 
																	from delivery d2
																	where d2.acctno = d.acctno
																	and d2.datetrans < d.datetrans
																	and d2.ItemID = d.ItemID
																	and d2.stocklocn = d.stocklocn
																	and d2.contractno = d.contractno
																	and d2.ParentItemID = d.ParentItemID
																	and d2.delorcoll = 'D') 
																
																ELSE
																	(select MAX(d2.datetrans) 
																	from delivery d2
																	where d2.acctno = d.acctno
																	and d2.datedel=d.datedel
																	and d2.ItemID = d.ItemID
																	and d2.stocklocn = d.stocklocn
																	and d2.contractno = d.contractno
																	and d2.ParentItemID = d.ParentItemID
																	and d2.delorcoll = 'D')
																END
															WHEN d.delorcoll = 'C' 
															Then
															(select MAX(d2.datetrans) 
                                                            from delivery d2
                                                            where d2.acctno = d.acctno
															and d2.datetrans < d.datetrans
                                                            and d2.ItemID = d.ItemID
                                                            and d2.stocklocn = d.stocklocn
                                                            and d2.contractno = d.contractno
                                                            and d2.ParentItemID = d.ParentItemID
                                                            and d2.delorcoll = 'D')
															ELSE
															(select MAX(d2.datetrans) 
                                                            from delivery d2
                                                            where d2.acctno = d.acctno
															and d2.datedel=d.datedel
                                                            and d2.ItemID = d.ItemID
                                                            and d2.stocklocn = d.stocklocn
                                                            and d2.contractno = d.contractno
                                                            and d2.ParentItemID = d.ParentItemID
                                                            and d2.delorcoll = 'D')
															END )
			LEFT JOIN promoprice pp 
			ON d.ItemID = pp.ItemID 
            		and pp.stocklocn = CASE @isPriceFromStockLocation
                                WHEN 1 THEN d.stocklocn
                                ELSE LEFT(d.acctno, 3) 
                               END
			and ((select MAX(d2.DateChange) 
                  from LineItemaudit d2
                  where d2.acctno = d.acctno
                      and d2.ItemID = d.ItemID
                      and d2.stocklocn = d.stocklocn
                      and d2.contractno = d.contractno
                      and d2.ParentItemID = d.ParentItemID
                      and d2.valueafter>0) >= pp.fromdate
                                                            
            and (select MAX(d2.DateChange) 
                 from LineItemaudit d2
                 where d2.acctno = d.acctno
                     and d2.ItemID = d.ItemID
                     and d2.stocklocn = d.stocklocn
                     and d2.contractno = d.contractno
                     and d2.ParentItemID = d.ParentItemID
                      and d2.valueafter>0) <= pp.todate)
            and pp.hporcash = 
                case
                    when a.accttype = 'C' then 'C'
					else 'H'
                end
			WHERE  runno between @minDelRunno AND @maxDelRunno
			AND t.transtypecode ='BHP'  
			AND D.acctno LIKE '___0%' AND s.category NOT IN (select code from code where category = 'WAR')  
			AND S.Category NOT IN (90,96)  
			AND s.category NOT IN ( select code from code where category = 'PCDIS') -- discounts   
			AND d.delorcoll !='R' AND d.itemno NOT IN (@adminitemno,@insuranceitemno,'RB' ) 
			AND d.BrokerExRunNo = 0  --Charudatt
			AND ISNULL(d.quantity,0) != 0
            		AND NOT EXISTS(select 1 from ReadyAssistDetails ra                          --Exclude Ready Assist Items
                           where ra.AcctNo = d.acctno
                          -- and ra.AgrmtNo = d.agrmtno
                           and ra.ItemId = d.ItemID)
			GROUP BY
			LEFT(t.interfacebalancing,2) + CONVERT(VARCHAR,S.category),  
			--right(interfaceaccount,4),  
			CONVERT(INT,LEFT(d.acctno,3)),	-- Lets just do the branch for the account numbers for now???
			t.transtypecode, s.category,d.transvalue
		
	 
			IF @@ERROR = 0  
				BEGIN  
					PRINT 'done stage 2 -bhp'  
				END  
			ELSE  
				BEGIN  
					PRINT 'error at stage 2-bhp '  
				END  
		END

	------Charudatt Added --- To make a seperate entry for SD in the broker file -------Start
	IF @countryletter = 'J' 
		BEGIN
			insert into #BrokerData  
			SELECT			
				@countryCode,   
				@runno,    
				right(interfaceaccount,4),  
				CONVERT(INT,LEFT(d.acctno,3)),	-- Lets just do the branch for the account numbers for now???
				t.transtypecode,  
				SUM((ROUND((ROUND(d.transvalue, @decimals) * 100) / (100 + s.taxrate), @decimals) )), 
				'',  
				s.category  
			FROM transtype t, delivery d   
			LEFT JOIN #stockitem s ON d.itemid = s.ItemId 
			AND s.stocklocn = CASE @isPriceFromStockLocation
                                WHEN 1 THEN d.stocklocn
                                ELSE LEFT(d.acctno, 3) 
                              END
		
			WHERE runno between @minDelRunno AND @maxDelRunno
			AND t.transtypecode ='BSD'  
			AND D.acctno LIKE '___0%'    
			AND d.delorcoll !='R' 
			AND d.itemno IN ('SD' )  
			AND d.BrokerExRunNo = 0 
			AND ISNULL(d.quantity,0) != 0
			GROUP BY  right(interfaceaccount,4),  
			CONVERT(INT,LEFT(d.acctno,3)),
			t.transtypecode,s.category,d.transvalue
	 
	  
			IF @@ERROR = 0  
				BEGIN  
					PRINT 'done stage 1 -bsd'  
				END  
			ELSE  
				BEGIN  
					PRINT 'error at stage 1-bsd '  
				END  
    
			insert into #BrokerData  
			SELECT			
				@countryCode,   
				@runno,    
				t.interfacebalancing,
				CONVERT(INT,LEFT(d.acctno,3)),	-- Lets just do the branch for the account numbers for now???
				t.transtypecode,  
				SUM(-1 * (ROUND((ROUND(d.transvalue, @decimals) * 100) / (100 + s.taxrate), @decimals) ))  , ---Charudatt 22102018
				'',  
				s.category  
			FROM transtype t, delivery d   
			LEFT JOIN #stockitem s ON d.itemid = s.ItemId 
			AND s.stocklocn = CASE @isPriceFromStockLocation
                                WHEN 1 THEN d.stocklocn
                                ELSE LEFT(d.acctno, 3) 
                              END
			WHERE  runno between @minDelRunno AND @maxDelRunno
			AND t.transtypecode ='BSD'  
			AND D.acctno LIKE '___0%' 
			AND d.delorcoll !='R' 
			AND d.itemno IN ('SD')  ------Charudatt added SD to eclude it from BHP 
			AND d.BrokerExRunNo = 0  --Charudatt
			AND ISNULL(d.quantity,0) != 0
			GROUP BY
				t.interfacebalancing,  
			CONVERT(INT,LEFT(d.acctno,3)),	-- Lets just do the branch for the account numbers for now???
			t.transtypecode,s.category,d.transvalue
	
	 
			IF @@ERROR = 0  
				BEGIN  
					PRINT 'done stage 2 -bsd'  
				END  
			ELSE  
				BEGIN  
					PRINT 'error at stage 2-bsd '  
				END  

		END
	------Charudatt Added --- To make a seperate entry for SD in the broker file -------End



	  --- #9607 Cost of Sale - Nonstocks
	  insert into #BrokerData  
	  SELECT @countryCode,   
			@runno,    
			left(t.interfaceaccount,2)+CONVERT(VARCHAR,S.category),  
			CONVERT(INT,LEFT(acctno,3)),   
			t.transtypecode,  
			SUM(ROUND(costprice*d.quantity,@decimals)),  
			'',  
			s.category  
	  FROM transtype t, delivery d   
	  LEFT JOIN #stockitem s ON d.itemid = s.ItemId AND d.stocklocn = s.stocklocn  
	  WHERE 
	        runno between @minDelRunno AND @maxDelRunno
	  AND t.transtypecode ='COS'  
	  --AND D.acctno LIKE '___0%'   
	  AND s.category NOT IN (select code from code where category = 'WAR')  
	  AND S.Category NOT IN (90,96)  
	  AND s.category NOT IN ( select code from code where category = 'PCDIS') -- discounts   
	  AND d.delorcoll !='R' AND s.IUPC NOT IN (@adminitemno,@insuranceitemno,'RB' )  
	  AND d.BrokerExRunNo = 0   ---Charudatt
	  and s.itemtype='N' and s.costprice!=0  -- non Stock with Cost price  
      and NOT EXISTS(select 1 from ServiceChargeAcct a where a.AcctNo = d.acctno)  
      AND NOT EXISTS(select 1 from SR_ChargeAcct a where a.AcctNo = d.acctno)  
      AND d.acctno not in (select isnull(ValueString,'') from config.Setting a where a.Id = 'ServiceStockAccount')  
	  and d.acctno not in (SELECT  Value  FROM CountryMaintenance WHERE CodeName IN ('ServiceInternal','ServiceWarranty')  
		  union all select acctno from acct a, code c  
		  where a.acctno= c.reference and category ='SRSUPPLIER' and a.acctno !='') --SL LW73168 remove sri,srw,srs from bcp      
	  GROUP BY  left(t.interfaceaccount,2)+CONVERT(VARCHAR,S.category),  
		CONVERT(INT,LEFT(acctno,3)),   
		t.transtypecode,  
		s.category  
	  
	  IF @@ERROR = 0  
		BEGIN  
			PRINT 'done stage 1-COS'  
		END  
	  ELSE  
		BEGIN  
			PRINT 'error at stage 1-COS '  
		END  
    
	  -- #9607 Cost of Sale Balancing - Nonstocks   
	  insert into #BrokerData  
	  SELECT @countryCode,   
			@runno,    
			right(interfacebalancing,4),  
			CONVERT(INT,LEFT(acctno,3)),   
			 t.transtypecode,  
			SUM(-ROUND(costprice*d.quantity,@decimals)),  
			'',  
			s.category  
	  FROM transtype t, delivery d   
	  LEFT JOIN #stockitem s ON d.itemId = s.ItemId AND d.stocklocn = s.stocklocn  
	  WHERE runno between @minDelRunno AND @maxDelRunno
	  AND t.transtypecode ='COS'  
	  AND s.category NOT IN (select code from code where category = 'WAR')  
	  AND S.Category NOT IN (90,96) --warranties tax DT and addto  
	  AND s.category NOT IN ( select code from code where category = 'PCDIS') -- discounts   
	  AND d.delorcoll !='R' AND s.IUPC NOT IN (@adminitemno,@insuranceitemno,'RB' )  
	  AND d.BrokerExRunNo = 0   --Charudatt
      and NOT EXISTS(select 1 from ServiceChargeAcct a where a.AcctNo = d.acctno)  
      AND NOT EXISTS(select 1 from SR_ChargeAcct a where a.AcctNo = d.acctno)  
      AND d.acctno not in (select isnull(ValueString,'') from config.Setting a where a.Id = 'ServiceStockAccount')  
	  and s.itemtype='N' and s.costprice!=0  -- non Stock with Cost price  
	  and d.acctno not in (SELECT  Value  FROM CountryMaintenance WHERE CodeName IN ('ServiceInternal','ServiceWarranty')  
		  union all select acctno from acct a, code c  
		  where a.acctno= c.reference and category ='SRSUPPLIER' and a.acctno !='') --SL LW73168 remove sri,srw,srs from bcp      
	  GROUP BY  right(interfacebalancing,4),  
		CONVERT(INT,LEFT(acctno,3)),   
		t.transtypecode,  
		s.category  
	  
	  IF @@ERROR = 0  
		BEGIN  
			PRINT 'done stage 2-COS'  
		END  
	  ELSE  
		BEGIN  
			PRINT 'error at stage 2-COS '  
		END  

-- HP price difference for all items inc warranties

       -----Added by Charudatt on 18052020------------------------Start
	SELECT d.acctno, 
               d.ItemID, 
               d.stocklocn, 
               d.contractno, 
               d.ParentItemID, 
               d.datetrans,
			  ROUND(ABS((100 * (d.transvalue / d.quantity)) / (100 + s.taxrate)), @decimals)
                   AS transvalue, 
               ABS(d.quantity) AS quantity, 
               d.quantity AS signedQuantity,
               s.category, 
               s.taxrate, 
               ABS(ROUND(s.unitpricecash, @decimals) * d.quantity) AS cashPrice
        INTO #crediCashDifference
		FROM delivery d 
		LEFT JOIN #stockitem s ON d.ItemID = s.ItemID
            AND CONVERT(INT,LEFT(d.acctno,3)) = s.stocklocn
        LEFT JOIN promoprice ppCash 
        ON d.ItemID = ppCash.ItemID 
            AND LEFT(d.acctno, 3) = ppCash.stocklocn
            AND d.datetrans >= ppCash.fromdate
            AND d.datetrans <= ppCash.todate
            and ppCash.hporcash = 'C'
        LEFT JOIN promoprice ppHP 
        ON d.ItemID = ppHP.ItemID 
            AND LEFT(d.acctno, 3) = ppHP.stocklocn
            AND d.datetrans >= ppHP.fromdate
            AND d.datetrans <= ppHP.todate
            and ppHP.hporcash = 'H'
		WHERE runno between @minDelRunno AND @maxDelRunno
		AND D.acctno LIKE '___0%' 
		AND s.category NOT IN (90,96) -- tax DT and addto
		AND s.category NOT IN ( select code from code where category = 'PCDIS') -- discounts 
		AND d.delorcoll !='R' AND d.itemno NOT IN (@adminitemno,@insuranceitemno,'RB' )
		AND (
                ROUND(unitpricecash, @decimals) != ROUND(unitpricehp, @decimals)
                OR 
                ROUND(ppCash.unitprice, @decimals) != ROUND(ppHP.unitprice, @decimals)
            )														--IP - 04/05/12 - #10040 - Merged #9485 - LW74206 from version 5.13																			
		AND d.BrokerExRunNo = 0 

        UPDATE c
        SET c.datetrans = (SELECT MAX(dd.datetrans)
							    FROM delivery dd
							    WHERE dd.acctno = c.acctno
							 	    AND dd.ItemID = c.ItemID
								    AND dd.stocklocn = c.stocklocn
								    AND dd.contractno = c.contractno
								    AND dd.ParentItemID = c.ParentItemID
								    AND dd.delorcoll = 'D'
								    AND dd.datetrans <= c.datetrans)
		FROM #crediCashDifference c
        
        UPDATE ccd
        SET ccd.cashPrice = (CASE @stocktaxtype
                                WHEN 'I' THEN ROUND((100 * ISNULL(pp.unitprice, sa.CashPrice)) / (100 + ccd.taxrate), @decimals)
                                ELSE ROUND(ISNULL(pp.unitprice, sa.CashPrice), @decimals)
                             END)
        FROM #crediCashDifference ccd
        INNER JOIN StockPriceAudit sa
        ON sa.ID = ccd.ItemID
            AND sa.BranchNo = LEFT(ccd.acctno, 3)
            AND sa.DateChange = (SELECT MAX(DateChange)
                                 FROM StockPriceAudit sa2
                                 WHERE sa2.ID = sa.ID
                                    AND sa2.BranchNo = sa.BranchNo
                                    AND sa2.DateChange <= ccd.datetrans)
        --Get any promotions at the time of delivery
         LEFT JOIN promoprice pp 
         ON ccd.ItemID = pp.ItemID 
            AND LEFT(ccd.acctno, 3) = pp.stocklocn
            AND ccd.datetrans >= pp.fromdate
            AND ccd.datetrans <= pp.todate
            and pp.hporcash = 'C'

		insert into #BrokerData
		SELECT @countryCode, 
		@runno,  
		right(t.interfaceaccount,4),
		CONVERT(INT,LEFT(d.acctno,3)),	-- Lets just do the branch for the account numbers for now???
		t.transtypecode,
		SUM(ROUND((d.transvalue - d.cashPrice), @decimals) * d.signedQuantity),
		'',
		d.category
		FROM transtype t, #crediCashDifference d 
		WHERE t.transtypecode ='BHD'
		GROUP BY 	right(t.interfaceaccount,4),
		CONVERT(INT,LEFT(d.acctno,3)),	-- Lets just do the branch for the account numbers for now???
		t.transtypecode,
		d.category
        
		IF @@ERROR = 0
		BEGIN
			PRINT 'done stage 1 -BHD'
		END
		ELSE
		BEGIN
			PRINT 'error at stage 1-BHD '
		END
			

		insert into #BrokerData
		SELECT @countryCode, 
		 @runno,  
		LEFT(t.interfacebalancing,2) + CONVERT(VARCHAR,d.category),
		CONVERT(INT,LEFT(acctno,3)),	-- Lets just do the branch for the account numbers for now???
		t.transtypecode,
		SUM(-ROUND((d.transvalue - d.cashPrice), @decimals) * d.signedQuantity),
		'',
		d.category
		FROM transtype t, #crediCashDifference d 
		WHERE t.transtypecode ='BHD'
		GROUP BY 	LEFT(t.interfacebalancing,2) + CONVERT(VARCHAR,d.category),
		CONVERT(INT,LEFT(d.acctno,3)),	-- Lets just do the branch for the account numbers for now???
		t.transtypecode,
		d.category
		IF @@ERROR = 0
		BEGIN
			PRINT 'done stage 2 -BHD'
		END
		ELSE
		BEGIN
			PRINT 'error at stage 2-BHD '
		END
		
	-------Addec by Charudatt on 18052020-------------------------End

        --DROP TABLE #crediCashDifference
	IF OBJECT_ID(N'tempdb..#crediCashDifference', N'U') IS NOT NULL   
	DROP TABLE #crediCashDifference; 

 EXEC BrokerPopulateCreditAccts @mindelrunno = @minDelRunno, @maxdelRunno = @maxDelRunno, @decimals = @decimals, @runno = 0, @query = 0 

-- broker insurance hp bhi
BEGIN 


	DECLARE @insTaxRate FLOAT

	SET @insTaxRate = ISNULL((select top 1 TaxRate from #stockitem
                    where itemno = @insuranceitemno),0)

	insert into #BrokerData  
	SELECT @countryCode,   @runno,    
	right(interfaceaccount,4),  CONVERT(INT,LEFT(acctno,3)), -- Lets just do the branch for the account numbers for now???  
	t.transtypecode,    
	CASE
		WHEN
			@agreementtaxtype = 'I'  and insincluded != 1
		THEN
			SUM(ROUND((InsuranceWeight * 100)/(100 + @insTaxRate),@decimals))
			WHEN
				@agreementtaxtype = 'E' 
		THEN
			round(SUM(InsuranceWeight),@decimals)
		ELSE
			round(SUM(InsuranceWeight),@decimals)
		END, 
	'', null  
	FROM transtype t, ##creditaccts  
	WHERE t.transtypecode ='BHI'  
	AND (insurance <>0 or InsuranceWeight <> 0)				--#17622
	GROUP BY  right(interfaceaccount,4),  
		CONVERT(INT,LEFT(acctno,3)), -- Lets just do the branch for the account numbers for now???  
		t.transtypecode,
		insincluded
	HAVING round(SUM(InsuranceWeight),@decimals) <> 0 --#14536
	  
	  
		IF @@ERROR = 0  
		BEGIN  
			PRINT 'done stage 1 -bhi'  
			END  
		ELSE  
		BEGIN  
			PRINT 'error at stage 1-bhi '  
	END  
     
	--- now for the balancing service charge has been reduced above by removing admin and insurance  
	insert into #BrokerData  
	SELECT @countryCode,   @runno,    
		right(interfacebalancing,4),  CONVERT(INT,LEFT(acctno,3)), -- Lets just do the branch for the account numbers for now???  
		t.transtypecode,    
        CASE
            WHEN
                @agreementtaxtype = 'I' and insincluded != 1
            THEN
                SUM(-ROUND((InsuranceWeight * 100)/(100 + @insTaxRate),@decimals))
            WHEN
                @agreementtaxtype = 'E' 
            THEN
                 round(SUM(-InsuranceWeight),@decimals)
            ELSE
               round(SUM(-InsuranceWeight),@decimals)
		END, 
		'',null  
	  FROM transtype t, ##creditaccts  
	  WHERE t.transtypecode ='BHI'  
	  AND (insurance <>0 or InsuranceWeight <> 0)				--#17622
	  GROUP BY  right(interfacebalancing,4),  
		CONVERT(INT,LEFT(acctno,3)), -- Lets just do the branch for the account numbers for now???  
		t.transtypecode,
        insincluded
		HAVING round(SUM(-InsuranceWeight),2) <> 0 --#14536
	 
	 
	 
	 IF @@ERROR = 0  
		BEGIN  
			PRINT 'done stage 1 -bhi'  
		END  
	 ELSE  
		BEGIN  
			PRINT 'error at stage 1-bhi '  
		END  
       
	END   
	-- broker admin hp bha  
	BEGIN   

		DECLARE @adminTaxRate FLOAT

		SET @adminTaxRate = ISNULL((select top 1 TaxRate from #stockitem where itemno = @adminitemno),0)

		insert into #BrokerData
		SELECT 
            @countryCode, 		
            @runno,  
		    right(interfaceaccount,4),		
            CONVERT(INT,LEFT(d.acctno,3)),	-- Lets just do the branch for the account numbers for now???
		    t.transtypecode,		
            CASE
                WHEN
                    @agreementtaxtype = 'I' 
                THEN
                    SUM(ROUND((d.transvalue * 100)/(100 + @adminTaxRate),@decimals))
                ELSE
                   round(SUM(transvalue),@decimals)
            END,
            '',
            null
		FROM transtype t,Delivery d
		INNER JOIN ##creditaccts ON  d.acctno=##creditaccts.acctno
		WHERE 
			runno between @minDelRunno AND @maxDelRunno
		AND d.itemno in ('Admin','AD') AND administration <>0  
		AND t.transtypecode ='BHA' 
		GROUP BY  right(interfaceaccount,4),  
		CONVERT(INT,LEFT(d.acctno,3)), -- Lets just do the branch for the account numbers for now???  
		t.transtypecode 
	 
	 
		IF @@ERROR = 0  
			BEGIN  
				PRINT 'done stage 1 -bha'  
			END  
		ELSE  
			BEGIN  
				PRINT 'error at stage 1-bha '  
			END  
     
		--- now for the balancing service charge has been reduced above by removing admin and insurance  
		insert into #BrokerData
		SELECT 
            @countryCode, 		
            @runno,  
		    right(interfacebalancing,4),		
            CONVERT(INT,LEFT(d.acctno,3)),	-- Lets just do the branch for the account numbers for now???
		    t.transtypecode,		
            CASE
                WHEN
                    @agreementtaxtype = 'I' 
                THEN
                    SUM(-ROUND((d.transvalue * 100)/(100 + @adminTaxRate),@decimals))
                ELSE
                   round(SUM(-transvalue),@decimals)
            END,
            '',
            null
		 FROM transtype t,Delivery d
		INNER JOIN ##creditaccts ON  d.acctno=##creditaccts.acctno
		WHERE 
			runno between @minDelRunno AND @maxDelRunno
		AND d.itemno in ('Admin','AD') AND administration <>0  
		AND t.transtypecode ='BHA' 
		GROUP BY  right(interfacebalancing,4),  
		CONVERT(INT,LEFT(d.acctno,3)), -- Lets just do the branch for the account numbers for now???  
		t.transtypecode 
	  
		IF @@ERROR = 0  
			BEGIN  
				PRINT 'done stage 2 -bha'  
			END  
		ELSE  
			BEGIN  
				PRINT 'error at stage 2-bha '  
			END
END 	
-- broker service charge hp take out ins + admin charge bhs 
-- '
BEGIN 
	insert into #BrokerData  
		SELECT @countryCode,   @runno,    
			right(interfaceaccount,4),  CONVERT(INT,LEFT(acctno,3)), -- Lets just do the branch for the account numbers for now???  
			t.transtypecode,		round(SUM(reducedservice),@decimals),		'', null    --#14536
		FROM transtype t, ##creditaccts  
		WHERE t.transtypecode ='BHS' 
		GROUP BY  right(interfaceaccount,4),  
		CONVERT(INT,LEFT(acctno,3)), -- Lets just do the branch for the account numbers for now???  
		t.transtypecode  
		HAVING round(SUM(reducedservice),2) <> 0    --#14536
	 
		IF @@ERROR = 0  
			BEGIN  
				PRINT 'done stage 1 -bhs'  
			END  
		ELSE  
			BEGIN  
				PRINT 'error at stage 1-bhs '  
			END  

     
		--- now for the balancing service charge has been reduced above by removing admin and insurance  
		insert into #BrokerData  
		SELECT @countryCode,   @runno,    
			right(interfacebalancing,4),  CONVERT(INT,LEFT(acctno,3)), -- Lets just do the branch for the account numbers for now???  
			t.transtypecode,		round(SUM(-reducedservice),@decimals),		'',null    --#14536
		FROM transtype t, ##creditaccts  
		WHERE t.transtypecode ='BHS'  
		GROUP BY  right(interfacebalancing,4),  
		CONVERT(INT,LEFT(acctno,3)), -- Lets just do the branch for the account numbers for now???  
		t.transtypecode  
		HAVING round(SUM(-reducedservice),2) <> 0   --#14536
	  
		IF @@ERROR = 0  
			BEGIN  
				PRINT 'done stage 1 -bhs'  
			END  
		ELSE  
			BEGIN  
				PRINT 'error at stage 1-bhs '  
			END
END 			
-- broker tax
-----Charudatt---------Start------------------Added country code and removed category 90 from BTX 

    -- Need to extract tax from the items deliveryed 
	IF( @countryletter = 'Q' OR  @countryletter = 'X' OR  @countryletter = 'O' )  
		BEGIN
	   IF @agreementtaxtype ='E'  
			BEGIN  
		  insert into #BrokerData  (
			countrycode,
			runno,
			interfaceaccount,
			branchno,
			transtypecode,
			transvalue,
			date,
			category
		)	
	  	SELECT @countryCode,   @runno,    
			right(interfaceaccount,4),  CONVERT(INT,LEFT(d.acctno,3)), -- Lets just do the branch for the account numbers for now???  
			t.transtypecode, 
		 	SUM(ROUND(d.transvalue * (taxrate/100), @decimals)),    ---modified on  31102019 to fix tax on tax	
			'', s.category  
		FROM transtype t, delivery d   
        	INNER JOIN agreement ag on d.acctno = ag.acctno
        	AND d.agrmtno = ag.agrmtno
		LEFT JOIN stockitem s ON d.ItemID = s.itemid AND d.stocklocn = s.stocklocn  
		WHERE
			runno between @minDelRunno AND @maxDelRunno  
		AND t.transtypecode ='BTX'   
        	AND d.itemno NOT IN (select w.Number from Warranty.Warranty w)  
        	AND d.itemno NOT IN ('STAX') 
		AND d.BrokerExRunNo = 0 --- Charudatt  
        	AND ISNULL(ag.TaxFree, 0) = 0
		AND d.acctno not in (select isnull(ValueString,'') from config.Setting a where a.Id = 'ServiceStockAccount')  
        	AND NOT EXISTS(select 1 from ServiceChargeAcct a where a.AcctNo = d.acctno)  
        	AND NOT EXISTS(select 1 from SR_ChargeAcct a where a.AcctNo = d.acctno)  
        	--AND NOT EXISTS(select 1 from ReadyAssistDetails ra                      --Exclude Ready Assist Items -----Removed by Charudatt on 12022020
                           --where ra.AcctNo = d.acctno
                           -- and ra.AgrmtNo = d.agrmtno
                           --and ra.ItemId = d.ItemID)
		GROUP BY  right(interfaceaccount,4),  
		CONVERT(INT,LEFT(d.acctno,3)), -- Lets just do the branch for the account numbers for now???  
		t.transtypecode,  
		s.category  
	 
	  	IF @@ERROR = 0  
			BEGIN  
			PRINT 'done stage 1 -tax'  
			END  
	  	ELSE  
			BEGIN  
			PRINT 'error at stage 1-tax '  
			END  
     
	  	insert into #BrokerData  (
			countrycode,
			runno,
			interfaceaccount,
			branchno,
			transtypecode,
			transvalue,
			date,
			category
			)	
	  	SELECT @countryCode,   @runno,    
			right(t.interfacebalancing,4),
			CONVERT(INT,LEFT(d.acctno,3)),	-- Lets just do the branch for the account numbers for now???
			t.transtypecode,  
			SUM(-(ROUND(d.transvalue * (taxrate/100), @decimals))),	    ---modified on  31102019 to fix tax on tax	
		 	'', s.category  
		FROM transtype t, delivery d   
        	INNER JOIN agreement ag on d.acctno = ag.acctno
        	AND d.agrmtno = ag.agrmtno
		LEFT JOIN stockitem s ON d.ItemID = s.itemid AND d.stocklocn = s.stocklocn  
	  	WHERE
			runno between @minDelRunno AND @maxDelRunno 
	 	AND t.transtypecode ='BTX'   
        	AND d.itemno NOT IN (select w.Number from Warranty.Warranty w)  
        	AND d.itemno NOT IN ('STAX')   
		AND d.BrokerExRunNo = 0   
        	AND ISNULL(ag.TaxFree, 0) = 0
		AND d.acctno not in (select isnull(ValueString,'') from config.Setting a where a.Id = 'ServiceStockAccount')  
        	AND NOT EXISTS(select 1 from ServiceChargeAcct a where a.AcctNo = d.acctno)  
        	AND NOT EXISTS(select 1 from SR_ChargeAcct a where a.AcctNo = d.acctno)  
        	--AND NOT EXISTS(select 1 from ReadyAssistDetails ra                      --Exclude Ready Assist Items -----Removed by Charudatt on 12022020
                          -- where ra.AcctNo = d.acctno
                           --and ra.AgrmtNo = d.agrmtno
                           --and ra.ItemId = d.ItemID)
		GROUP BY  right(t.interfacebalancing,4),  
		CONVERT(INT,LEFT(d.acctno,3)), -- Lets just do the branch for the account numbers for now???  
	  	t.transtypecode,  
	  	s.category  
	 
	 	IF @@ERROR = 0  
			BEGIN  
			 PRINT 'done stage 2 -tax'  
			END  
	 	ELSE  
			BEGIN  
			PRINT 'error at stage 2-tax '  
			END  
     
	END  
		ELSE -- Need to extract tax from the items deliveryed   
			BEGIN  
			Insert into #BrokerData (  
			countrycode,  
			runno,  
			interfaceaccount,  
			branchno,  
			transtypecode,  
			transvalue,  
	   		date,  
	   		category  
	 		 )   
	  	SELECT @countryCode,   @runno,    
		right(interfaceaccount,4),		
		CONVERT(INT,LEFT(d.acctno,3)),	-- Lets just do the branch for the account numbers for now???
		t.transtypecode,
		SUM(ROUND(transvalue-(transvalue*100)/(100+ s.taxrate),@decimals)), '' , s.category		
	  	FROM transtype t, delivery d   
        	INNER JOIN agreement ag on d.acctno = ag.acctno
        	AND d.agrmtno = ag.agrmtno
	  	LEFT JOIN #stockitem s ON d.itemid = s.itemid AND d.stocklocn = s.stocklocn  
	  	WHERE
			runno between @minDelRunno AND @maxDelRunno  
		AND t.transtypecode ='BTX'  
        	AND d.itemno NOT IN (select w.Number from Warranty.Warranty w)
        	AND d.itemno NOT IN ('STAX') 
		AND d.BrokerExRunNo = 0     --Charudatt
        	AND ISNULL(ag.TaxFree, 0) = 0 
	    	AND d.acctno not in (select isnull(ValueString,'') from config.Setting a where a.Id = 'ServiceStockAccount')  
       	 	AND NOT EXISTS(select 1 from ServiceChargeAcct a where a.AcctNo = d.acctno)  
        	AND NOT EXISTS(select 1 from SR_ChargeAcct a where a.AcctNo = d.acctno)  
        	--AND NOT EXISTS(select 1 from ReadyAssistDetails ra                          --Exclude Ready Assist Items -----Removed by Charudatt on 12022020
                  --         where ra.AcctNo = d.acctno
                    --       and ra.AgrmtNo = d.agrmtno
                      --     and ra.ItemId = d.ItemID)
	  	GROUP BY  right(interfaceaccount,4),  
		CONVERT(INT,LEFT(d.acctno,3)),	-- Lets just do the branch for the account numbers for now???
	  	t.transtypecode,  
	  	s.category  
	  
	  
	  	IF @@ERROR = 0  
			BEGIN  
			PRINT 'done stage 1 -btx'  
			END  
	  	ELSE  
			BEGIN  
			PRINT 'error at stage 1-btx '  
			END  
     
	  	Insert into #BrokerData  (
			countrycode,
			runno,
			interfaceaccount,
			branchno,
			transtypecode,
			transvalue,
			date,
			category
		)	
	  	SELECT @countryCode,   @runno,    
			RIGHT(t.interfacebalancing,4) ,
			CONVERT(INT,LEFT(d.acctno,3)),	-- Lets just do the branch for the account numbers for now???
			t.transtypecode,		
			-SUM(ROUND(d.transvalue-(transvalue*100)/(100+ s.taxrate),@decimals)),		
			'',s.category
	  	FROM transtype t, delivery d   
        	INNER JOIN agreement ag on d.acctno = ag.acctno
        	AND d.agrmtno = ag.agrmtno
	  	LEFT JOIN #stockitem s ON d.itemid = s.itemid AND d.stocklocn = s.stocklocn  
	  	WHERE 
			runno between @minDelRunno AND @maxDelRunno  
		AND t.transtypecode ='BTX'  
        	AND d.itemno NOT IN (select w.Number from Warranty.Warranty w)
        	AND d.itemno NOT IN ('STAX') 
		AND d.BrokerExRunNo = 0  
        	AND ISNULL(ag.TaxFree, 0) = 0
		AND d.acctno not in (select isnull(ValueString,'') from config.Setting a where a.Id = 'ServiceStockAccount')  
        	AND NOT EXISTS(select 1 from ServiceChargeAcct a where a.AcctNo = d.acctno)  
        	AND NOT EXISTS(select 1 from SR_ChargeAcct a where a.AcctNo = d.acctno)  
        	--AND NOT EXISTS(select 1 from ReadyAssistDetails ra                      --Exclude Ready Assist Items -----Removed by Charudatt on 12022020
                  --         where ra.AcctNo = d.acctno
                    --       and ra.AgrmtNo = d.agrmtno
                      --     and ra.ItemId = d.ItemID)
		GROUP BY 		right(t.interfacebalancing,4),
		CONVERT(INT,LEFT(D.acctno,3)),	-- Lets just do the branch for the account numbers for now???
	  	t.transtypecode,  
	  	s.category  
	  
	  
	  	IF @@ERROR = 0  
			BEGIN  
				PRINT 'done stage 2 -btx'  
			END  
	  	ELSE  
			BEGIN  
				PRINT 'error at stage 2-btx '  
			END  
     
	 END  
	END
	ELSE
		BEGIN
	   IF @agreementtaxtype ='E'  
			BEGIN  
		  insert into #BrokerData  (
			countrycode,
			runno,
			interfaceaccount,
			branchno,
			transtypecode,
			transvalue,
			date,
			category
		)	
	  	SELECT @countryCode,   @runno,    
			right(interfaceaccount,4),  CONVERT(INT,LEFT(d.acctno,3)), -- Lets just do the branch for the account numbers for now???  
			t.transtypecode, 
		 	SUM(ROUND(d.transvalue * (taxrate/100), @decimals)),    ---modified on  31102019 to fix tax on tax	
			'', s.category  
		FROM transtype t, delivery d   
        INNER JOIN agreement ag on d.acctno = ag.acctno
        AND d.agrmtno = ag.agrmtno
		LEFT JOIN stockitem s ON d.ItemID = s.itemid AND d.stocklocn = s.stocklocn  
		WHERE
			runno between @minDelRunno AND @maxDelRunno  
		AND t.transtypecode ='BTX'   
        AND d.itemno NOT IN (select w.Number from Warranty.Warranty w)  
        AND d.itemno NOT IN ('STAX') 
		AND d.BrokerExRunNo = 0 --- Charudatt  
		AND s.category NOT IN (90)  ---Added to fix BB 10.5.2 migration issue 
        AND ISNULL(ag.TaxFree, 0) = 0
		AND d.acctno not in (select isnull(ValueString,'') from config.Setting a where a.Id = 'ServiceStockAccount')  
        AND NOT EXISTS(select 1 from ServiceChargeAcct a where a.AcctNo = d.acctno)  
        AND NOT EXISTS(select 1 from SR_ChargeAcct a where a.AcctNo = d.acctno)  
        	--AND NOT EXISTS(select 1 from ReadyAssistDetails ra                      --Exclude Ready Assist Items -----Removed by Charudatt on 12022020
                           --where ra.AcctNo = d.acctno
                           -- and ra.AgrmtNo = d.agrmtno
                           --and ra.ItemId = d.ItemID)
		GROUP BY  right(interfaceaccount,4),  
		CONVERT(INT,LEFT(d.acctno,3)), -- Lets just do the branch for the account numbers for now???  
		t.transtypecode,  
		s.category  
	 
	  	IF @@ERROR = 0  
			BEGIN  
			PRINT 'done stage 1 -tax'  
			END  
	  	ELSE  
			BEGIN  
			PRINT 'error at stage 1-tax '  
			END  
     
	  	insert into #BrokerData  (
			countrycode,
			runno,
			interfaceaccount,
			branchno,
			transtypecode,
			transvalue,
			date,
			category
			)	
	  	SELECT @countryCode,   @runno,    
			right(t.interfacebalancing,4),
			CONVERT(INT,LEFT(d.acctno,3)),	-- Lets just do the branch for the account numbers for now???
			t.transtypecode,  
			SUM(-(ROUND(d.transvalue * (taxrate/100), @decimals))),	    ---modified on  31102019 to fix tax on tax	
		 	'', s.category  
		FROM transtype t, delivery d   
        INNER JOIN agreement ag on d.acctno = ag.acctno
        AND d.agrmtno = ag.agrmtno
		LEFT JOIN stockitem s ON d.ItemID = s.itemid AND d.stocklocn = s.stocklocn  
	  	WHERE
			runno between @minDelRunno AND @maxDelRunno 
	 	AND t.transtypecode ='BTX'   
        AND d.itemno NOT IN (select w.Number from Warranty.Warranty w)  
        AND d.itemno NOT IN ('STAX')   
		AND d.BrokerExRunNo = 0  
		AND s.category NOT IN (90)  ---Added to fix BB 10.5.2 migration issue  
        AND ISNULL(ag.TaxFree, 0) = 0
		AND d.acctno not in (select isnull(ValueString,'') from config.Setting a where a.Id = 'ServiceStockAccount')  
        AND NOT EXISTS(select 1 from ServiceChargeAcct a where a.AcctNo = d.acctno)  
        AND NOT EXISTS(select 1 from SR_ChargeAcct a where a.AcctNo = d.acctno)  
        	--AND NOT EXISTS(select 1 from ReadyAssistDetails ra                      --Exclude Ready Assist Items -----Removed by Charudatt on 12022020
                          -- where ra.AcctNo = d.acctno
                           --and ra.AgrmtNo = d.agrmtno
                           --and ra.ItemId = d.ItemID)
		GROUP BY  right(t.interfacebalancing,4),  
		CONVERT(INT,LEFT(d.acctno,3)), -- Lets just do the branch for the account numbers for now???  
	  	t.transtypecode,  
	  	s.category  
	 
	 	IF @@ERROR = 0  
			BEGIN  
			 PRINT 'done stage 2 -tax'  
			END  
	 	ELSE  
			BEGIN  
			PRINT 'error at stage 2-tax '  
			END  
     
	END  
		ELSE -- Need to extract tax from the items deliveryed   
			BEGIN  
			Insert into #BrokerData (  
			countrycode,  
			runno,  
			interfaceaccount,  
			branchno,  
			transtypecode,  
			transvalue,  
	   		date,  
	   		category  
	 		 )   
	  	SELECT @countryCode,   @runno,    
		right(interfaceaccount,4),		
		CONVERT(INT,LEFT(d.acctno,3)),	-- Lets just do the branch for the account numbers for now???
		t.transtypecode,
		SUM(ROUND(transvalue-(transvalue*100)/(100+ s.taxrate),@decimals)), '' , s.category		
	  	FROM transtype t, delivery d   
        INNER JOIN agreement ag on d.acctno = ag.acctno
        AND d.agrmtno = ag.agrmtno
	  	LEFT JOIN #stockitem s ON d.itemid = s.itemid AND d.stocklocn = s.stocklocn  
	  	WHERE
			runno between @minDelRunno AND @maxDelRunno  
		AND t.transtypecode ='BTX'  
        AND d.itemno NOT IN (select w.Number from Warranty.Warranty w)
        AND d.itemno NOT IN ('STAX') 
		AND d.BrokerExRunNo = 0     --Charudatt
		AND s.category NOT IN (90)  ---Added to fix BB 10.5.2 migration issue 
        AND ISNULL(ag.TaxFree, 0) = 0 
	    AND d.acctno not in (select isnull(ValueString,'') from config.Setting a where a.Id = 'ServiceStockAccount')  
       	AND NOT EXISTS(select 1 from ServiceChargeAcct a where a.AcctNo = d.acctno)  
        AND NOT EXISTS(select 1 from SR_ChargeAcct a where a.AcctNo = d.acctno)  
        	--AND NOT EXISTS(select 1 from ReadyAssistDetails ra                          --Exclude Ready Assist Items -----Removed by Charudatt on 12022020
                  --         where ra.AcctNo = d.acctno
                    --       and ra.AgrmtNo = d.agrmtno
                      --     and ra.ItemId = d.ItemID)
	  	GROUP BY  right(interfaceaccount,4),  
		CONVERT(INT,LEFT(d.acctno,3)),	-- Lets just do the branch for the account numbers for now???
	  	t.transtypecode,  
	  	s.category  
	  
	  
	  	IF @@ERROR = 0  
			BEGIN  
			PRINT 'done stage 1 -btx'  
			END  
	  	ELSE  
			BEGIN  
			PRINT 'error at stage 1-btx '  
			END  
     
	  	Insert into #BrokerData  (
			countrycode,
			runno,
			interfaceaccount,
			branchno,
			transtypecode,
			transvalue,
			date,
			category
		)	
	  	SELECT @countryCode,   @runno,    
			RIGHT(t.interfacebalancing,4) ,
			CONVERT(INT,LEFT(d.acctno,3)),	-- Lets just do the branch for the account numbers for now???
			t.transtypecode,		
			-SUM(ROUND(d.transvalue-(transvalue*100)/(100+ s.taxrate),@decimals)),		
			'',s.category
	  	FROM transtype t, delivery d   
        INNER JOIN agreement ag on d.acctno = ag.acctno
        AND d.agrmtno = ag.agrmtno
	  	LEFT JOIN #stockitem s ON d.itemid = s.itemid AND d.stocklocn = s.stocklocn  
	  	WHERE 
			runno between @minDelRunno AND @maxDelRunno  
		AND t.transtypecode ='BTX'  
        AND d.itemno NOT IN (select w.Number from Warranty.Warranty w)
        AND d.itemno NOT IN ('STAX') 
		AND d.BrokerExRunNo = 0  
        AND ISNULL(ag.TaxFree, 0) = 0
		AND s.category NOT IN (90)  ---Added to fix BB 10.5.2 migration issue 
		AND d.acctno not in (select isnull(ValueString,'') from config.Setting a where a.Id = 'ServiceStockAccount')  
        AND NOT EXISTS(select 1 from ServiceChargeAcct a where a.AcctNo = d.acctno)  
        AND NOT EXISTS(select 1 from SR_ChargeAcct a where a.AcctNo = d.acctno)  
        	--AND NOT EXISTS(select 1 from ReadyAssistDetails ra                      --Exclude Ready Assist Items -----Removed by Charudatt on 12022020
                  --         where ra.AcctNo = d.acctno
                    --       and ra.AgrmtNo = d.agrmtno
                      --     and ra.ItemId = d.ItemID)
		GROUP BY 		right(t.interfacebalancing,4),
		CONVERT(INT,LEFT(D.acctno,3)),	-- Lets just do the branch for the account numbers for now???
	  	t.transtypecode,  
	  	s.category  
	  
	  
	  	IF @@ERROR = 0  
			BEGIN  
				PRINT 'done stage 2 -btx'  
			END  
	  	ELSE  
			BEGIN  
				PRINT 'error at stage 2-btx '  
			END  
     
	 END  
	END
  -----Charudatt---------End--------------------
-- hp discount
BEGIN 
		insert into #BrokerData
		SELECT @countryCode, 
		@runno,  right(interfaceaccount,4),
		CONVERT(INT,LEFT(acctno,3)),	-- Lets just do the branch for the account numbers for now???
		t.transtypecode,
		SUM(ROUND((transvalue*100)/(100+ s.taxrate), @decimals)),
		'',
		s.category
		FROM transtype t, delivery d 
		LEFT JOIN #stockitem s ON d.itemid = s.itemid AND d.stocklocn = s.stocklocn
		WHERE runno between @minDelRunno AND @maxDelRunno
		AND t.transtypecode ='BHC'
		AND D.acctno LIKE '___0%' AND s.category IN (select code from code where category = 'PCDIS') -- discounts
		AND s.category Not IN (39)  --- Added by Charudatt On 11/09/2018  
		AND d.delorcoll !='R' AND d.itemno NOT IN (@adminitemno,@insuranceitemno )
		AND d.BrokerExRunNo = 0 
		GROUP BY 	right(interfaceaccount,4),
		CONVERT(INT,LEFT(acctno,3)),	-- Lets just do the branch for the account numbers for now???
		t.transtypecode,
		s.category
		IF @@ERROR = 0
		BEGIN
			PRINT 'done stage 1 -bhc'
		END
		ELSE
		BEGIN
			PRINT 'error at stage 1-bhc '
		END
			
		insert into #BrokerData
		SELECT @countryCode, 
		@runno,  
		LEFT(t.interfacebalancing,2) + CONVERT(VARCHAR,S.category),
		CONVERT(INT,LEFT(acctno,3)),	-- Lets just do the branch for the account numbers for now???
		t.transtypecode,
		SUM(ROUND((-transvalue*100)/(100+ s.taxrate), @decimals)),
		'',
		s.category
		FROM transtype t, delivery d 
		LEFT JOIN #stockitem s ON d.itemid = s.itemid AND d.stocklocn = s.stocklocn
		WHERE runno between @minDelRunno AND @maxDelRunno
		AND t.transtypecode ='BHC'
		AND D.acctno LIKE '___0%' AND s.category IN (select code from code where category = 'PCDIS') -- discounts
		AND s.category Not IN (39)  --- Added by Charudatt On 11/09/2018  
		AND d.delorcoll !='R' AND d.itemno NOT IN (@adminitemno,@insuranceitemno )
		AND d.BrokerExRunNo = 0 
		GROUP BY 		LEFT(t.interfacebalancing,2) + CONVERT(VARCHAR,S.category),
		CONVERT(INT,LEFT(acctno,3)),	-- Lets just do the branch for the account numbers for now???
		t.transtypecode,
		s.category
		IF @@ERROR = 0
		BEGIN
			PRINT 'done stage 2 -bhc'
		END
		ELSE
		BEGIN
			PRINT 'error at stage 2-bhc '
		END
END 
--cash/special discounts
		insert into #BrokerData
		SELECT @countryCode, 
		@runno,  
		right(interfaceaccount,4),
		CONVERT(INT,LEFT(acctno,3)),	-- Lets just do the branch for the account numbers for now???
		t.transtypecode,
		SUM(ROUND((transvalue*100)/(100+ s.taxrate), @decimals)),
		'',
		s.category
		FROM transtype t, delivery d 
		LEFT JOIN #stockitem s ON d.itemid = s.itemid AND d.stocklocn = s.stocklocn
		WHERE runno between @minDelRunno AND @maxDelRunno
		AND t.transtypecode ='BCC'
		AND D.acctno NOT LIKE '___0%' AND s.category IN (select code from code where category = 'PCDIS') --discounts
		AND d.delorcoll !='R' AND d.itemno NOT IN (@adminitemno,@insuranceitemno )
		AND d.BrokerExRunNo = 0
		GROUP BY 	right(interfaceaccount,4),
		CONVERT(INT,LEFT(acctno,3)),	-- Lets just do the branch for the account numbers for now???
		t.transtypecode,
		s.category
		IF @@ERROR = 0
		BEGIN
			PRINT 'done stage 1 -bcc'
		END
		ELSE
		BEGIN
			PRINT 'error at stage 1-bcc '
		END

		insert into #BrokerData
		SELECT @countryCode, 
		@runno,  
		LEFT(t.interfacebalancing,2) + CONVERT(VARCHAR,S.category),
		CONVERT(INT,LEFT(acctno,3)),	-- Lets just do the branch for the account numbers for now???
		t.transtypecode,
		SUM(ROUND((-transvalue*100)/(100+ s.taxrate), @decimals)),
		'',
		s.category
		FROM transtype t, delivery d 
		LEFT JOIN #stockitem s ON d.itemid = s.itemid AND d.stocklocn = s.stocklocn
		WHERE runno between @minDelRunno AND @maxDelRunno
		AND t.transtypecode ='BCC'
		AND D.acctno NOT LIKE '___0%' AND s.category IN (select code from code where category = 'PCDIS') -- discounts
		AND d.delorcoll !='R' AND d.itemno NOT IN (@adminitemno,@insuranceitemno )
		AND d.BrokerExRunNo = 0 
		GROUP BY 		LEFT(t.interfacebalancing,2) + CONVERT(VARCHAR,S.category),
		CONVERT(INT,LEFT(acctno,3)),	-- Lets just do the branch for the account numbers for now???
		t.transtypecode,
		s.category
		IF @@ERROR = 0
		BEGIN
			PRINT 'done stage 2 -bcc'
		END
		ELSE
		BEGIN
			PRINT 'error at stage 2-bcc '
		END

-- Cash account deliveries
	insert into #BrokerData
		SELECT @countryCode, 
		@runno,  
		right(interfaceaccount,4),
		CONVERT(INT,LEFT(d.acctno,3)),	-- Lets just do the branch for the account numbers for now???
		t.transtypecode,
		SUM(CASE 
                WHEN @agreementtaxtype = 'E' OR ag.TaxFree = 1
                    THEN ROUND((transvalue), @decimals)
                ELSE ROUND((transvalue * 100)/(100 + s.taxrate), @decimals)
            END),
		'',
		s.category
		FROM transtype t, delivery d 
        INNER JOIN agreement ag
        ON d.acctno = ag.acctno
            AND d.agrmtno = ag.agrmtno
		LEFT JOIN #stockitem s ON d.itemid = s.itemid AND d.stocklocn = s.stocklocn
		WHERE runno between @minDelRunno AND @maxDelRunno
		AND t.transtypecode ='BCP'
		AND D.acctno NOT LIKE '___0%' AND s.category not in (select code from code where category = 'WAR')
		AND s.category NOT IN (90,96)
		AND s.category NOT IN ( select code from code where category = 'PCDIS') --discounts
		AND d.delorcoll !='R'
		and d.acctno not in (SELECT  Value  FROM CountryMaintenance WHERE CodeName IN ('ServiceInternal','ServiceWarranty')		-- #9607 use CodeName
		union all select acctno from acct a, code c
		where a.acctno= c.reference and category ='SRSUPPLIER' and a.acctno !='') --SL LW73168 remove sri,srw,srs from bcp 
		AND d.BrokerExRunNo = 0
        AND NOT EXISTS(select 1 from ServiceChargeAcct a where a.AcctNo = d.acctno)
        AND NOT EXISTS(select 1 from SR_ChargeAcct a where a.AcctNo = d.acctno)
        AND d.acctno not in (select isnull(ValueString,'') from config.Setting a where a.Id = 'ServiceStockAccount')
		AND NOT EXISTS(select 1 from ReadyAssistDetails ra                          --Exclude Ready Assist Items -----Added by Charudatt on 12022020
                           where ra.AcctNo = d.acctno
                          -- and ra.AgrmtNo = d.agrmtno
                           and ra.ItemId = d.ItemID)	
		GROUP BY 	right(interfaceaccount,4),
		CONVERT(INT,LEFT(d.acctno,3)),	-- Lets just do the branch for the account numbers for now???
		t.transtypecode,
		s.category
		IF @@ERROR = 0
		BEGIN
			PRINT 'done stage 1 -bcp'
		END
		ELSE
		BEGIN
			PRINT 'error at stage 1-bcp '
		END

		insert into #BrokerData
		SELECT @countryCode, 
		@runno,  
		LEFT(t.interfacebalancing,2) + CONVERT(VARCHAR,S.category),
		CONVERT(INT,LEFT(d.acctno,3)),	-- Lets just do the branch for the account numbers for now???
		t.transtypecode,
		SUM(CASE 
                WHEN @agreementtaxtype = 'E' OR ag.TaxFree = 1
                    THEN ROUND((-transvalue), @decimals)
                ELSE ROUND((-transvalue * 100)/(100 + s.taxrate), @decimals)
            END),
		'',
		s.category
		FROM transtype t, delivery d 
        INNER JOIN agreement ag
        ON d.acctno = ag.acctno
            AND d.agrmtno = ag.agrmtno
		LEFT JOIN #stockitem s ON d.itemid = s.itemid AND d.stocklocn = s.stocklocn
		WHERE runno between @minDelRunno AND @maxDelRunno
		AND t.transtypecode ='BCP'
		AND D.acctno NOT LIKE '___0%' -- not credit accounts
		AND s.category NOT IN (90,96)
		AND s.category NOT IN (select code from code where category = 'WAR')
		AND s.category NOT IN ( select code from code where category = 'PCDIS') --discounts
		AND d.delorcoll !='R'
		and d.acctno not in (SELECT  Value  FROM CountryMaintenance WHERE CodeName IN ('ServiceInternal','ServiceWarranty')		-- #9607 use CodeName
		union all select acctno from acct a, code c
		where a.acctno= c.reference and category ='SRSUPPLIER' and a.acctno !='') --SL LW73168 remove sri,srw,srs from bcp
		AND d.BrokerExRunNo = 0 
        AND NOT EXISTS(select 1 from ServiceChargeAcct a where a.AcctNo = d.acctno)
        AND NOT EXISTS(select 1 from SR_ChargeAcct a where a.AcctNo = d.acctno)
        AND d.acctno not in (select isnull(ValueString,'') from config.Setting a where a.Id = 'ServiceStockAccount')
		AND NOT EXISTS(select 1 from ReadyAssistDetails ra                          --Exclude Ready Assist Items -----Added by Charudatt on 12022020
                           where ra.AcctNo = d.acctno
                          -- and ra.AgrmtNo = d.agrmtno
                           and ra.ItemId = d.ItemID)	
		GROUP BY 		LEFT(t.interfacebalancing,2) + CONVERT(VARCHAR,S.category),
		CONVERT(INT,LEFT(d.acctno,3)),	-- Lets just do the branch for the account numbers for now???
		t.transtypecode,
		s.category
		IF @@ERROR = 0
		BEGIN
			PRINT 'done stage 2 -bcp'
		END
		ELSE
		BEGIN
			PRINT 'error at stage 2-bcp '
		END

-- lets now do the repos/redeliveries after repos.
		insert into #BrokerData
		SELECT @countryCode, 
		@runno,  
		right(interfaceaccount,4),
		CONVERT(INT,LEFT(acctno,3)),	-- Lets just do the branch for the account numbers for now???
		t.transtypecode,
		SUM(ROUND((transvalue*100)/(100+ s.taxrate), @decimals)),
		'',
		s.category
		FROM transtype t, delivery d 
		LEFT JOIN #stockitem s ON d.itemid = s.itemid AND d.stocklocn = s.stocklocn
		WHERE runno between @minDelRunno AND @maxDelRunno
		AND t.transtypecode ='REP'
		AND d.delorcoll ='R'
		AND d.BrokerExRunNo = 0 
		GROUP BY 	right(interfaceaccount,4),
		CONVERT(INT,LEFT(acctno,3)),	-- Lets just do the branch for the account numbers for now???
		t.transtypecode,
		s.category
		IF @@ERROR = 0
		BEGIN
			PRINT 'done stage 1 -rep'
		END
		ELSE
		BEGIN
			PRINT 'error at stage 1-rep '
		END
		
	    insert into #BrokerData
		SELECT @countryCode, 
		@runno,  
		'code' = CASE WHEN cast(s.category as varchar) IN (select code from code where category = 'PCF') THEN '5185' -- furniture categories
		ELSE '5115' -- electrical categories
		END ,
		CONVERT(INT,LEFT(acctno,3)),	-- Lets just do the branch for the account numbers for now???
		t.transtypecode,
		SUM(ROUND((-transvalue*100)/(100+ s.taxrate), @decimals)),
		'',
		s.category
		FROM transtype t, delivery d 
		LEFT JOIN #stockitem s ON d.itemid = s.itemid AND d.stocklocn = s.stocklocn
		WHERE runno   between @minDelRunno AND @maxDelRunno
		AND t.transtypecode ='REP'
		AND d.delorcoll ='R'
		AND d.BrokerExRunNo = 0 
		GROUP BY 	s.category,
		CONVERT(INT,LEFT(acctno,3)),	-- Lets just do the branch for the account numbers for now???
		t.transtypecode,
		s.category
		IF @@ERROR = 0
		BEGIN
			PRINT 'done stage 2 -rep'
		END
		ELSE
		BEGIN
			PRINT 'error at stage 2-rep '
		END

-- lets now do the rebates.
		insert into #BrokerData
		SELECT @countryCode, 
		@runno,  
		right(interfaceaccount,4),
		CONVERT(INT,LEFT(acctno,3)),	-- Lets just do the branch for the account numbers for now???
		t.transtypecode,
		SUM(ROUND(transvalue, @decimals)),
		'',
		s.category
		FROM transtype t, delivery d 
		LEFT JOIN #stockitem s ON d.itemid = s.itemid AND d.stocklocn = s.stocklocn
		WHERE runno between @minDelRunno AND @maxDelRunno
		AND t.transtypecode ='REB'
		AND D.acctno LIKE '___0%'
		AND d.delorcoll !='R' AND d.itemno = 'RB'
		AND d.BrokerExRunNo = 0 
		GROUP BY 	right(interfaceaccount,4),
		CONVERT(INT,LEFT(acctno,3)),	-- Lets just do the branch for the account numbers for now???
		t.transtypecode,
		s.category
		IF @@ERROR = 0
		BEGIN
			PRINT 'done stage 1 -Reb'
		END
		ELSE
		BEGIN
			PRINT 'error at stage 1-reb '
		END

		insert into #BrokerData
		SELECT @countryCode, 
		@runno,  
		LEFT(t.interfacebalancing,4),
		CONVERT(INT,LEFT(acctno,3)),	-- Lets just do the branch for the account numbers for now???
		t.transtypecode,
		SUM(ROUND(-transvalue, @decimals)),
		'',
		null
		FROM transtype t, delivery d 
		WHERE runno between @minDelRunno AND @maxDelRunno
		AND t.transtypecode ='REB'
		AND D.acctno LIKE '___0%'
		AND d.delorcoll !='R' AND d.itemno = 'RB'
		AND d.BrokerExRunNo = 0 
		GROUP BY 		LEFT(t.interfacebalancing,4) ,
		CONVERT(INT,LEFT(acctno,3)),	-- Lets just do the branch for the account numbers for now???
		t.transtypecode
		IF @@ERROR = 0
		BEGIN
			PRINT 'done stage 2 -reb'
		END
		ELSE
		BEGIN
			PRINT 'error at stage 2-reb '
		END


--------------------------------------------------------------------------------------------------------------------------------------------------------------

--#18612 - CR15594 - RA - Ready Assist Broker transactions

--BHR - Credit Sale of Ready Assist 
insert into 
	#BrokerData
select 
	@countryCode,
	@runno,
	right(interfaceaccount,4),
	convert(int,LEFT(d.acctno,3)),
	t.transtypecode,
	SUM((ROUND((ROUND(d.transvalue, @decimals) * 100) / (100 + s.taxrate), @decimals) )) ,  ----- Added by Charudatt on 11022020
	'',
	s.category
from 
	transtype t,
	delivery d 
inner join 
	ReadyAssistDetails ra on ra.acctno = d.acctno
	and ra.agrmtno = d.agrmtno
	and ra.itemid = d.itemid
	and ra.contractno = d.contractno
inner join 
	#stockitem s on s.ItemID = d.itemid
	and s.stocklocn = d.stocklocn
where 
	runno between @minDelRunno AND @maxDelRunno
	and t.transtypecode = 'BHR'
	and d.acctno LIKE '___0%'
	and d.delorcoll = 'D'
	and d.BrokerExRunNo  = 0  
group by 	
	right(interfaceaccount,4),
	convert(int,LEFT(d.acctno,3)),
	t.transtypecode,
	s.category

--BHR - Credit Sale of Ready Assist (Balancing)
insert into 
	#BrokerData
select 
	@countryCode,
	@runno,
	right(interfacebalancing,4),
	convert(int,LEFT(d.acctno,3)),
	t.transtypecode,
	SUM(-1 * (ROUND((ROUND(d.transvalue, @decimals) * 100) / (100 + s.taxrate), @decimals) )) ,  -----Added by Charudatt on 11022020
	'',
	s.category
from 
	transtype t,
	delivery d 
inner join 
	ReadyAssistDetails ra on ra.acctno = d.acctno
	and ra.agrmtno = d.agrmtno
	and ra.itemid = d.itemid
	and ra.ContractNo = d.contractno
inner join 
	#stockitem s on s.ItemID = d.itemid
	and s.stocklocn = d.stocklocn
where 
	runno between @minDelRunno AND @maxDelRunno
	and t.transtypecode = 'BHR'
	and d.acctno LIKE '___0%'
	and d.delorcoll = 'D'
	and d.BrokerExRunNo = 0  
group by 	
	right(interfacebalancing,4),
	convert(int,LEFT(d.acctno,3)),
	t.transtypecode,
	s.category

IF @@ERROR = 0
BEGIN
	PRINT 'done BHR'
END
ELSE
BEGIN
	PRINT 'error at BHR'
END


--BCR - Cash Sale of Ready Assist 
insert into 
	#BrokerData
select 
	@countryCode,
	@runno,
	right(interfaceaccount,4),
	convert(int,LEFT(d.acctno,3)),
	t.transtypecode,
	SUM((ROUND((ROUND(d.transvalue, @decimals) * 100) / (100 + s.taxrate), @decimals) )), -----Added by Charudatt on 11022020
	'',
	s.category
from 
	transtype t,
	delivery d 
inner join 
	ReadyAssistDetails ra on ra.acctno = d.acctno
	and ra.agrmtno = d.agrmtno
	and ra.itemid = d.itemid
	and ra.contractno = d.contractno
inner join 
	acct a on d.acctno = a.acctno
inner join 
	#stockitem s on s.ItemID = d.itemid
	and s.stocklocn = d.stocklocn
where 
	runno between @minDelRunno AND @maxDelRunno
	and t.transtypecode = 'BCR'
	and a.accttype = 'C'
	and d.delorcoll = 'D'
	and d.BrokerExRunNo = 0 
group by 	
	right(interfaceaccount,4),
	convert(int,LEFT(d.acctno,3)),
	t.transtypecode,
	s.category

--BCR - Cash Sale of Ready Assist (Balancing)
insert into 
	#BrokerData
select 
	@countryCode,
	@runno,
	right(interfacebalancing,4),
	convert(int,LEFT(d.acctno,3)),
	t.transtypecode,
	SUM(-1 * (ROUND((ROUND(d.transvalue, @decimals) * 100) / (100 + s.taxrate), @decimals) )), -----Added by Charudatt on 11022020
	'',
	s.category
from 
	transtype t,
	delivery d 
inner join 
	ReadyAssistDetails ra on ra.acctno = d.acctno
inner join 
	acct a on d.acctno = a.acctno
	and ra.agrmtno = d.agrmtno
	and ra.itemid = d.itemid
	and ra.ContractNo = d.contractno
inner join 
	#stockitem s on s.ItemID = d.itemid
	and s.stocklocn = d.stocklocn
where 
	runno between @minDelRunno AND @maxDelRunno
	and t.transtypecode = 'BCR'
	and a.accttype = 'C'
	and d.delorcoll = 'D'
	and d.BrokerExRunNo = 0 
group by 	
	right(interfacebalancing,4),
	convert(int,LEFT(d.acctno,3)),
	t.transtypecode,
	s.category

IF @@ERROR = 0
BEGIN
	PRINT 'done BCR'
END
ELSE
BEGIN
	PRINT 'error at BCR'
END


--BRS - Service Charge on Credit Sale of Ready Assist

insert into 
	#BrokerData
select 
	@countryCode,
	@runno,
	right(interfaceaccount,4),
	convert(int,LEFT(d.acctno,3)),
	t.transtypecode,
	SUM(ROUND(transvalue, @decimals)),
	'',
	s.category
from 
	transtype t,
	delivery d 
inner join 
	#stockitem s on s.ItemID = d.itemid
	and s.stocklocn = d.stocklocn
where 
	exists(select * from ReadyAssistDetails ra
				where ra.acctno = d.acctno
				and ra.agrmtno = d.agrmtno)
	and runno between @minDelRunno AND @maxDelRunno
	and t.transtypecode = 'BRS'
	and d.acctno LIKE '___0%'
	and s.iupc = 'DT'
	and d.BrokerExRunNo = 0 
group by 	
	right(interfaceaccount,4),
	convert(int,LEFT(d.acctno,3)),
	t.transtypecode,
	s.category

--BRS - Service Charge on Credit Sale of Ready Assist (Balancing)

insert into 
	#BrokerData
select 
	@countryCode,
	@runno,
	right(interfacebalancing,4),
	convert(int,LEFT(d.acctno,3)),
	t.transtypecode,
	SUM(ROUND(-1 * transvalue, @decimals)),
	'',
	s.category
from 
	transtype t,
	delivery d 
inner join 
	#stockitem s on s.ItemID = d.itemid
	and s.stocklocn = d.stocklocn
where 
	exists(select * from ReadyAssistDetails ra
				where ra.acctno = d.acctno
				and ra.agrmtno = d.agrmtno)
	and runno between @minDelRunno AND @maxDelRunno
	and t.transtypecode = 'BRS'
	and d.acctno LIKE '___0%'
	and s.iupc = 'DT'
	and d.BrokerExRunNo = 0 
group by 	
	right(interfacebalancing,4),
	convert(int,LEFT(d.acctno,3)),
	t.transtypecode,
	s.category

IF @@ERROR = 0
BEGIN
	PRINT 'done BRS'
END
ELSE
BEGIN
	PRINT 'error at BRS'
END

IF @@ERROR = 0
BEGIN
	PRINT 'done CRA'
END
ELSE
BEGIN
	PRINT 'error at CRA'
END


-------CANCELLATION

--CNR - Cancellation of Ready Assist Credit
insert into 
	#BrokerData
select 
	@countryCode,
	@runno,
	right(interfaceaccount,4),
	convert(int,LEFT(d.acctno,3)),
	t.transtypecode,
	--SUM(ROUND(transvalue, @decimals)),
	SUM((ROUND((ROUND(d.transvalue, @decimals) * 100) / (100 + s.taxrate), @decimals) )), -----Added by Charudatt on 25022020
	'',
	s.category
from 
	transtype t,
	delivery d 
inner join 
	ReadyAssistDetails ra on ra.acctno = d.acctno
	and ra.agrmtno = d.agrmtno
	and ra.itemid = d.itemid
	and ra.contractno = d.contractno
inner join 
	#stockitem s on s.ItemID = d.itemid
	and s.stocklocn = d.stocklocn
where 
	runno between @minDelRunno AND @maxDelRunno
	and t.transtypecode = 'CNR'
	and d.acctno LIKE '___0%'
	and d.delorcoll in ('R', 'C')
	and d.quantity < 0
	and d.BrokerExRunNo = 0 
group by 	
	right(interfaceaccount,4),
	convert(int,LEFT(d.acctno,3)),
	t.transtypecode,
	s.category

--CNR - Cancellation of Ready Assist Credit (Balancing)
insert into 
	#BrokerData
select 
	@countryCode,
	@runno,
	right(interfacebalancing,4),
	convert(int,LEFT(d.acctno,3)),
	t.transtypecode,
	--SUM(ROUND(-1 * transvalue, @decimals)),
	SUM(-1 *(ROUND((ROUND(d.transvalue, @decimals) * 100) / (100 + s.taxrate), @decimals) )), -----Added by Charudatt on 25022020
	'',
	s.category
from 
	transtype t,
	delivery d 
inner join 
	ReadyAssistDetails ra on ra.acctno = d.acctno
	and ra.agrmtno = d.agrmtno
	and ra.itemid = d.itemid
	and ra.contractno = d.contractno
inner join 
	#stockitem s on s.ItemID = d.itemid
	and s.stocklocn = d.stocklocn
where 
	runno between @minDelRunno AND @maxDelRunno
	and t.transtypecode = 'CNR'
	and d.acctno LIKE '___0%'
	and d.delorcoll in ('R', 'C')
	and d.quantity < 0
	and d.BrokerExRunNo = 0 
group by 	
	right(interfacebalancing,4),
	convert(int,LEFT(d.acctno,3)),
	t.transtypecode,
	s.category

IF @@ERROR = 0
BEGIN
	PRINT 'done CNR'
END
ELSE
BEGIN
	PRINT 'error at CNR'
END


--CRC - Cancellation of Ready Assist Cash
insert into 
	#BrokerData
select 
	@countryCode,
	@runno,
	right(interfaceaccount,4),
	convert(int,LEFT(d.acctno,3)),
	t.transtypecode,
	--SUM(ROUND(transvalue, @decimals)),
	SUM((ROUND((ROUND(d.transvalue, @decimals) * 100) / (100 + s.taxrate), @decimals) )), -----Added by Charudatt on 25022020
	'',
	s.category
from 
	transtype t,
	delivery d 
inner join 
	ReadyAssistDetails ra on ra.acctno = d.acctno
	and ra.agrmtno = d.agrmtno
	and ra.itemid = d.itemid
	and ra.contractno = d.contractno
inner join 
	acct a on ra.acctno = a.acctno
inner join 
	#stockitem s on s.ItemID = d.itemid
	and s.stocklocn = d.stocklocn
where 
	runno between @minDelRunno AND @maxDelRunno
	and t.transtypecode = 'CRC'
	and d.delorcoll in ('R', 'C')
	and d.quantity < 0
	and a.accttype = 'C'
	and d.BrokerExRunNo = 0  
group by 	
	right(interfaceaccount,4),
	convert(int,LEFT(d.acctno,3)),
	t.transtypecode,
	s.category

--CRC - Cancellation of Ready Assist Cash (Balancing)
insert into 
	#BrokerData
select 
	@countryCode,
	@runno,
	right(interfacebalancing,4),
	convert(int,LEFT(d.acctno,3)),
	t.transtypecode,
	--SUM(ROUND(-1 * transvalue, @decimals)),
	SUM(-1 *(ROUND((ROUND(d.transvalue, @decimals) * 100) / (100 + s.taxrate), @decimals) )), -----Added by Charudatt on 25022020
	'',
	s.category
from 
	transtype t,
	delivery d 
inner join 
	ReadyAssistDetails ra on ra.acctno = d.acctno
	and ra.agrmtno = d.agrmtno
	and ra.itemid = d.itemid
	and ra.contractno = d.contractno
inner join 
	acct a on ra.acctno = a.acctno
inner join 
	#stockitem s on s.ItemID = d.itemid
	and s.stocklocn = d.stocklocn
where 
	runno between @minDelRunno AND @maxDelRunno
	and t.transtypecode = 'CRC'
	and d.delorcoll in ('R', 'C')
	and d.quantity < 0
	and a.accttype = 'C'
	and d.BrokerExRunNo = 0 
group by 	
	right(interfacebalancing,4),
	convert(int,LEFT(d.acctno,3)),
	t.transtypecode,
	s.category

IF @@ERROR = 0
BEGIN
	PRINT 'done CRC'
END
ELSE
BEGIN
	PRINT 'error at CRC'
END


--RAR & RRC for Ready Assist Used portion dealt with in COS FACT Export
--------CANCELLATION

----------------------------------------------------------------------------------------------------------------------------------------------------------------
--BOR - Credit Sale of Annual Service Contract
insert into 
	#BrokerData
select 
	@countryCode,
	@runno,
	right(interfaceaccount,4),
	convert(int,LEFT(d.acctno,3)),
	t.transtypecode,
	SUM(ROUND(transvalue, @decimals)),
	'',
	s.category
from 
	transtype t,
	delivery d 
inner join 
	#stockitem s on s.ItemID = d.itemid
	and s.stocklocn = d.stocklocn
inner join 
    code c on c.code = s.ItemNo
where 
	runno between @minDelRunno AND @maxDelRunno
	and t.transtypecode = 'BOR'
	and d.acctno LIKE '___0%'
	and d.delorcoll = 'D'
	and d.BrokerExRunNo = 0 
    and c.category = 'ANNSERVCONT'
group by 	
	right(interfaceaccount,4),
	convert(int,LEFT(d.acctno,3)),
	t.transtypecode,
	s.category

--BOR - Credit Sale of Annual Service Contract (Balancing)
insert into 
	#BrokerData
select 
	@countryCode,
	@runno,
	right(interfacebalancing,4),
	convert(int,LEFT(d.acctno,3)),
	t.transtypecode,
	SUM(ROUND(-1 * transvalue, @decimals)),
	'',
	s.category
from 
	transtype t,
	delivery d 
inner join 
	#stockitem s on s.ItemID = d.itemid
	and s.stocklocn = d.stocklocn
inner join 
    code c on c.code = s.ItemNo
where 
	runno between @minDelRunno AND @maxDelRunno
	and t.transtypecode = 'BOR'
	and d.acctno LIKE '___0%'
	and d.delorcoll = 'D'
	and d.BrokerExRunNo = 0 
    and c.category = 'ANNSERVCONT'
group by 	
	right(interfacebalancing,4),
	convert(int,LEFT(d.acctno,3)),
	t.transtypecode,
	s.category


IF @@ERROR = 0
BEGIN
	PRINT 'done BOR'
END
ELSE
BEGIN
	PRINT 'error at BOR'
END


--BCA - Cash Sale of Annual Service
insert into 
	#BrokerData
select 
	@countryCode,
	@runno,
	right(interfaceaccount,4),
	convert(int,LEFT(d.acctno,3)),
	t.transtypecode,
	SUM(ROUND(transvalue, @decimals)),
	'',
	s.category
from 
	transtype t,
	delivery d 
inner join 
	#stockitem s on s.ItemID = d.itemid
	and s.stocklocn = d.stocklocn
inner join 
    acct a on a.acctno = d.acctno
inner join 
    code c on c.code = s.ItemNo
where 
	runno between @minDelRunno AND @maxDelRunno
	and t.transtypecode = 'BCA'
	and a.accttype = 'C'
	and d.delorcoll = 'D'
	and d.BrokerExRunNo = 0 
    and c.category = 'ANNSERVCONT'
group by 	
	right(interfaceaccount,4),
	convert(int,LEFT(d.acctno,3)),
	t.transtypecode,
	s.category

--BCA - Cash Sale of Annual Service (Balancing)
insert into 
	#BrokerData
select 
	@countryCode,
	@runno,
	right(interfacebalancing,4),
	convert(int,LEFT(d.acctno,3)),
	t.transtypecode,
	SUM(ROUND(-1 * transvalue, @decimals)),
	'',
	s.category
from 
	transtype t,
	delivery d 
inner join 
	#stockitem s on s.ItemID = d.itemid
	and s.stocklocn = d.stocklocn
inner join 
    acct a on a.acctno = d.acctno
inner join 
    code c on c.code = s.ItemNo
where 
	runno between @minDelRunno AND @maxDelRunno
	and t.transtypecode = 'BCA'
	and a.accttype = 'C'
	and d.delorcoll = 'D'
	and d.BrokerExRunNo = 0 
    and c.category = 'ANNSERVCONT'
group by 	
	right(interfacebalancing,4),
	convert(int,LEFT(d.acctno,3)),
	t.transtypecode,
	s.category

IF @@ERROR = 0
BEGIN
	PRINT 'done BCA'
END
ELSE
BEGIN
	PRINT 'error at BCA'
END


--BAS - Service Charge on Credit Annual Service
insert into 
	#BrokerData
select 
	@countryCode,
	@runno,
	right(interfaceaccount,4),
	convert(int,LEFT(d.acctno,3)),
	t.transtypecode,
	SUM(ROUND(transvalue, @decimals)),
	'',
	s.category
from 
	transtype t,
	delivery d 
inner join 
	#stockitem s on s.ItemID = d.itemid
	and s.stocklocn = d.stocklocn
where 
	exists(select * from delivery d1
                inner join code c on d1.itemno = c.code
				where d1.acctno = d.acctno
				and d1.agrmtno = d.agrmtno
                and c.category = 'ANNSERVCONT')
	and runno between @minDelRunno AND @maxDelRunno
	and t.transtypecode = 'BAS'
	and d.acctno LIKE '___0%'
	and s.itemno = 'DT'
	and d.BrokerExRunNo = 0  
group by 	
	right(interfaceaccount,4),
	convert(int,LEFT(d.acctno,3)),
	t.transtypecode,
	s.category

--BAS - Service Charge on Credit Annual Service (Balancing)

insert into 
	#BrokerData
select 
	@countryCode,
	@runno,
	right(interfacebalancing,4),
	convert(int,LEFT(d.acctno,3)),
	t.transtypecode,
	SUM(ROUND(-1 * transvalue, @decimals)),
	'',
	s.category
from 
	transtype t,
	delivery d 
inner join 
	#stockitem s on s.ItemID = d.itemid
	and s.stocklocn = d.stocklocn
where 
	exists(select * from delivery d1
                inner join code c on d1.itemno = c.code
				where d1.acctno = d.acctno
				and d1.agrmtno = d.agrmtno
                and c.category = 'ANNSERVCONT')
	and runno between @minDelRunno AND @maxDelRunno
	and t.transtypecode = 'BAS'
	and d.acctno LIKE '___0%'
	and s.itemno = 'DT'
	and d.BrokerExRunNo = 0 
group by 	
	right(interfacebalancing,4),
	convert(int,LEFT(d.acctno,3)),
	t.transtypecode,
	s.category

IF @@ERROR = 0
BEGIN
	PRINT 'done BRS'
END
ELSE
BEGIN
	PRINT 'error at BRS'
END

-------CANCELLATION

--CNA - Cancellation of Annual Service Credit
insert into 
	#BrokerData
select 
	@countryCode,
	@runno,
	right(interfaceaccount,4),
	convert(int,LEFT(d.acctno,3)),
	t.transtypecode,
	SUM(ROUND(transvalue, @decimals)),
	'',
	s.category
from 
	transtype t,
	delivery d 
inner join 
	#stockitem s on s.ItemID = d.itemid
	and s.stocklocn = d.stocklocn
inner join 
    code c on c.code = d.itemno
where 
	runno between @minDelRunno AND @maxDelRunno
	and t.transtypecode = 'CNA'
	and d.acctno LIKE '___0%'
	and d.delorcoll in ('R', 'C')
	and d.quantity < 0
	and d.BrokerExRunNo = 0 
    and c.category = 'ANNSERVCONT'
group by 	
	right(interfaceaccount,4),
	convert(int,LEFT(d.acctno,3)),
	t.transtypecode,
	s.category

--CNA -Cancellation of Annual Service Credit (Balancing)
insert into 
	#BrokerData
select 
	@countryCode,
	@runno,
	right(interfacebalancing,4),
	convert(int,LEFT(d.acctno,3)),
	t.transtypecode,
	SUM(ROUND(-1 * transvalue, @decimals)),
	'',
	s.category
from 
	transtype t,
	delivery d 
inner join 
	#stockitem s on s.ItemID = d.itemid
	and s.stocklocn = d.stocklocn
inner join 
    code c on c.code = d.itemno
where 
	runno between @minDelRunno AND @maxDelRunno
	and t.transtypecode = 'CNA'
	and d.acctno LIKE '___0%'
	and d.delorcoll in ('R', 'C')
	and d.quantity < 0
	and d.BrokerExRunNo = 0 
    and c.category = 'ANNSERVCONT'
group by 	
	right(interfacebalancing,4),
	convert(int,LEFT(d.acctno,3)),
	t.transtypecode,
	s.category

IF @@ERROR = 0
BEGIN
	PRINT 'done CNA'
END
ELSE
BEGIN
	PRINT 'error at CNA'
END


--CAC - Cancellation of Annual Service Cash
insert into 
	#BrokerData
select 
	@countryCode,
	@runno,
	right(interfaceaccount,4),
	convert(int,LEFT(d.acctno,3)),
	t.transtypecode,
	SUM(ROUND(transvalue, @decimals)),
	'',
	s.category
from 
	transtype t,
	delivery d 
inner join 
	acct a on d.acctno = a.acctno
inner join 
	#stockitem s on s.ItemID = d.itemid
	and s.stocklocn = d.stocklocn
inner join 
    code c on c.code = d.itemno
where 
	runno between @minDelRunno AND @maxDelRunno
	and t.transtypecode = 'CAC'
	and d.delorcoll in ('R', 'C')
	and d.quantity < 0
	and a.accttype = 'C'
	and d.BrokerExRunNo = 0  
    and c.category = 'ANNSERVCONT'
group by 	
	right(interfaceaccount,4),
	convert(int,LEFT(d.acctno,3)),
	t.transtypecode,
	s.category

--CAC - Cancellation of Annual Service Cash (Balancing)
insert into 
	#BrokerData
select 
	@countryCode,
	@runno,
	right(interfacebalancing,4),
	convert(int,LEFT(d.acctno,3)),
	t.transtypecode,
	SUM(ROUND(-1 * transvalue, @decimals)),
	'',
	s.category
from 
	transtype t,
	delivery d 
inner join 
	acct a on d.acctno = a.acctno
inner join 
	#stockitem s on s.ItemID = d.itemid
	and s.stocklocn = d.stocklocn
inner join 
    code c on c.code = d.itemno
where 
	runno between @minDelRunno AND @maxDelRunno
	and t.transtypecode = 'CAC'
	and d.delorcoll in ('R', 'C')
	and d.quantity < 0
	and a.accttype = 'C'
	and d.BrokerExRunNo = 0  
    and c.category = 'ANNSERVCONT'
group by 	
	right(interfacebalancing,4),
	convert(int,LEFT(d.acctno,3)),
	t.transtypecode,
	s.category

IF @@ERROR = 0
BEGIN
	PRINT 'done CAC'
END
ELSE
BEGIN
	PRINT 'error at CAC'
END


--RAR & RRC for Ready Assist Used portion dealt with in COS FACT Export
--------CANCELLATION
-----------------------------------------------------------------------------------------------------------------------------------------------------------------

	if (@status = 0)
	begin
		update #BrokerData 
		SET date = (SELECT case when datepart(hh, i.datestart) >= 0 AND datepart(hh, i.datestart) <=7 AND datepart(hh, i.datefinish) <=7
				   then replace(convert(varchar(10), dateadd(d,-1,i.datestart), 120), '-', '') 
				   when datepart(hh, i.datestart) >= 0 AND datepart(hh, i.datestart) <=7 AND datepart(hh, i.datefinish) =8
						AND datepart(mi, i.datefinish) =00
				   then replace(convert(varchar(10), dateadd(d,-1,i.datestart), 120), '-', '') 
			       else
					replace(convert(varchar(10), i.datestart, 120), '-', '') 
					end)
		FROM interfacecontrol i
		WHERE i.runno = #BrokerData.runno
		AND interface = 'BROKERX'
    END
    -- for single digit categories we need to put in the 0 in the middle e.g
    -- category 5 could be 515 - it should show as 5105
    UPDATE #BrokerData 
	SET Interfaceaccount = LEFT(Interfaceaccount,2) + '0' + RIGHT(Interfaceaccount,1)
	WHERE DATALENGTH(Interfaceaccount) =3

	
	DELETE FROM #BrokerData WHERE Transvalue = 0 ---- Added by Charudatt

	DELETE FROM #BrokerData WHERE ISNULL(Transvalue,'') = '' ---- Added by Charudatt

    -- now inserting into the interface_financial table where not already existing
    -- for the current run number for the delivery style transactions.
	INSERT INTO interface_financial (
		runno,		transvalue,		Interfaceaccount,		
		transtypecode,		reference,		branchno, brokerExrunno
	) 
	SELECT @maxUpdsmryRunno,
           ROUND(transvalue, @decimals),		
           Interfaceaccount,		
		   transtypecode,
           CONVERT(VARCHAR,runno),
           branchno, 
           @runno
	FROM #BrokerData WHERE 
	NOT EXISTS (SELECT * FROM interface_financial f 
	WHERE f.transtypecode = #BrokerData.transtypecode 
	AND f.runno = @maxUpdsmryRunno)
	and transvalue is not null --sl lw72025
/* This was just used to check the totals were 0.
	SELECT SUM(transvalue),transtypecode  FROM #BrokerData
	GROUP BY transtypecode 
*/	
	UPDATE delivery SET brokerExrunno = @runno 
	WHERE runno between @minDelRunno AND @maxDelRunno
    AND NOT EXISTS(select 1 from ServiceChargeAcct a
                            where a.AcctNo = delivery.acctno)
    AND NOT EXISTS(select 1 from SR_ChargeAcct a
                        where a.AcctNo = delivery.acctno)
    AND delivery.acctno not in (select ValueString from config.Setting a where a.Id = 'ServiceStockAccount')
	AND brokerExrunno= 0 

	UPDATE interface_financial SET  brokerExRunNo = @runno
	WHERE  runno between @minUpdsmryRunno AND @maxUpdsmryRunno 
	AND brokerExrunno= 0 

	SELECT * FROM #BrokerData
END

