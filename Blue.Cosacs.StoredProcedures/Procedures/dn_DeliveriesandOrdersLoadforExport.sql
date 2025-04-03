/* dn_DeliveriesandOrdersLoadforExport - the procedure for exporting data to FACT 2000 
It exports Orders, Deliveries and Cancellation records.
27 Nov 2007 - Issue 67 - allowing deliveries to go through for service request special accounts though no linking 
agreement number on the agreement table. */

if NOT exists (select * FROM information_schema.columns WHERE table_name ='interfacecontrol' AND column_name ='StageCompleted')
alter table interfacecontrol add StageCompleted varchar(32) NOT null default ''
go
if exists (select * from sysobjects WHERE NAME LIKE 'dn_DeliveriesandOrdersLoadforExport')
drop  procedure dn_DeliveriesandOrdersLoadforExport 
Go
create procedure dn_DeliveriesandOrdersLoadforExport 
--------------------------------------------------------------------------------
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 03/06/11  IP  CR1212 - RI - #3392 - Spare parts interface to FACT not RI. Only interface 
--				 Spare Parts
-- 22/06/11  IP  CR1212 - RI - #3987 - Based on the new Country Parameter found beneath the Interface section called 'Ri Interface Options',	
--										if set to 'Parts' only spare parts will be interfaced to FACT.
-- 12/12/11  IP  #8876 - Received foreign key constraint error on insert, as trying to insert agreement record
--				 where there is no corresponding acct record. This is a data issue and would rarely ocurr.
-- 23/12/11  IP  #8849 - LW73492 - Checking for type '0' instead of 'O' Merged from 5.13
-- 04/05/12  jec #10089 LW74967 - Repo codes interface without 'Z'
-- 16/01/13  IP  #11771 LW75641 -  send blank order with delivery - Merged from CoSACS 6.5
-- 22/01/14  ip  #17083 Service Request charge account table changed from SR_ChargeAcct to ServiceChargeAcct
-- 16/05/14  ip	 #17979 Exclude warranties from being exported.
-- 01/03/2022  JM BlueStart - API Code Integration BSAccountMapping Table for Merchandising Job.  
-- 05/09/22 -  Mambu accounts needs to be included in Merchandising Sales Export GDIEC-94
--------------------------------------------------------------------------------
                 @runno int out, 
                 @deliverytotal money out, 
                 @OrderandDeliverytotal money out,
                 @empeeno int,
                 @return int out
as declare
    @status            integer  ,
    @strate            float  ,
    @stitemno          varchar(8) ,
    @custid            varchar(20),
    @retstocklocn      integer ,
    @delorcoll         varchar(1) ,
    @Maxquantity smallint ,
    @hasaffinity  smallint,
    @hasaffinitytext varchar(6),
    @serviceItemLabour varchar(8),			--IP - 03/06/11 - CR1212 - RI
    @serviceItemPartsCourts varchar(8),		--IP - 03/06/11 - CR1212 - RI
    @serviceItemPartsOther varchar(8),		--IP - 03/06/11 - CR1212 - RI
    @interfaceMethod varchar(5),				--IP - 22/06/11 - CR1212 - RI
    @priceFromStockLocation varchar(5) 
    
SET ARITHABORT OFF
SET ANSI_WARNINGS OFF   

begin 
	
   select @interfaceMethod = value from countrymaintenance where codename = 'RIInterfaceOptions'
   select @serviceItemLabour = value from countrymaintenance where codename = 'ServiceItemLabour'						--IP - 03/06/11 - CR1212 - RI 
   select @serviceItemPartsCourts = value from countrymaintenance where codename = 'ServiceItemPartsCourts'				--IP - 03/06/11 - CR1212 - RI
   select @serviceItemPartsOther = value from countrymaintenance where codename = 'ServiceItemPartsOther'				--IP - 03/06/11 - CR1212 - RI
   select @priceFromStockLocation = value from countrymaintenance where codename = 'pricefromlocn' -- Merchandising - required for cash price
              
   truncate table lastfactexport
   set @return = 0
   select  @hasaffinitytext = value FROM countrymaintenance
   WHERE Name='Country has affinity'
   --select @runno = max(runno) FROM interfacecontrol WHERE interface ='COS FACT' AND result ='P'
   declare @debug int
     set @debug = 1

   if @debug = 0
     set nocount on
      
   if @hasaffinitytext = 'True'                     
       SET @hasaffinity = 1


   select @runno = max(runno) from interfacecontrol where interface ='COS FACT'   
/* loading up discounts so that they do NOT apply to individual branches*/
-- AA to do    stock.getdiscounts(stockitemset =globaldiscount_array)

/* returning warranty on credit warranties where not paid after period. */
	
   IF(@interfaceMethod = 'FACT')								--IP - 22/06/11 - CR1212 - RI - #3987
   BEGIN
	   execute @status =or_creditwarranties_return 0	--RI

	   /* updating one-for-one replacement quantities */
	   execute @status  =dn_oneforonereplacementmarker	--RI

	   /* collecting warranties whose parent items have been collected in OpenROAD */
	   execute @status  = OR_CollectWarranties		--RI

	/*Collecting warranties where collection has been processed and
	collected warranty in schedule table*/
	   execute @status  =OR_DeliveryCollectWarranties @empeeno = 99999 --RI
	   
	   /* deliver outstanding renewal warranties for accounts which  */
	   /* have not been cancelled  and have been delivery authorised */
	  execute @status = DN_DeliverWarrantyRenewalSP @empeeno = @empeeno, @return = 0 --RI
  		
  END
     		
    -- AA inserting into facttrans where missing from lineitem as 
   -- outstanding orders were being left on fact2000 which should have been removed when the stocklocation was changed. 
   DECLARE @datelastrun DATETIME
   SELECT @datelastrun = datestart FROM interfacecontrol  WHERE interface ='cos fact'
   AND runno = @runno-1

   INSERT INTO facttrans( origbr, la.acctno, agrmtno, buffno, 
                  itemno,  stocklocn, trantype, tccode,
                  trandate, quantity, price, taxamt, value, ItemID)								--IP - 27/06/11 - CR1212 - RI - #3987
   SELECT         0,la.acctno, la.agrmtno, 0,
                  si.iupc, la.stocklocn, '01', '58',											--IP - 27/06/11 - CR1212 - RI - #3987					
                  Datechange, 0, 0, 0,0, la.ItemID												--IP - 27/06/11 - CR1212 - RI - #3987	
   FROM LineitemAudit la
   INNER JOIN StockInfo si on la.ItemID = si.ID													--IP - 27/06/11 - CR1212 - RI - #3987
   INNER JOIN StockQuantity sq on si.ID = sq.ID													--IP - 27/06/11 - CR1212 - RI - #3987
   AND la.stocklocn = sq.stocklocn																--IP - 27/06/11 - CR1212 - RI - #3987
   WHERE Datechange >@datelastrun
   AND NOT EXISTS (SELECT * FROM lineitem l WHERE l.acctno = la.acctno AND l.stocklocn  =la.stocklocn
      AND l.ItemID = la.ItemID)																	--IP - 27/06/11 - CR1212 - RI - #3987
   AND si.itemtype ='S' and la.quantityafter=0													--IP - 27/06/11 - CR1212 - RI - #3987												
   AND (((si.SparePart = 1 OR si.itemno in (@serviceItemLabour, @serviceItemPartsCourts, @serviceItemPartsOther)) and @interfaceMethod = 'Parts') or @interfaceMethod = 'FACT')	--IP - 03/06/11 - CR1212 - RI - Spare Parts --IP - 22/06/11 - CR1212 - RI - Check country parameter
   AND NOT EXISTS (SELECT * FROM facttrans f WHERE f.acctno = la.acctno AND f.ItemID = la.ItemID --IP - 27/06/11 - CR1212 - RI - #3987	
   AND f.quantity=0 AND f.stocklocn = la.stocklocn)


	-- 70439: remove accounts from factrans where cancelled in run period
	delete from facttrans
	where exists
	(select 'x' from cancellation c, acct a
	where c.acctno=facttrans.acctno
	and c.acctno=a.acctno
	and a.dateacctopen>=@datelastrun)


   declare @newbuffno INT,@stocklocn SMALLINT,@buffbranchno SMALLINT,
   @acctno CHAR(12), @itemID INT,@agrmtno INT,@trandate DATETIME								--IP - 27/06/11 - CR1212 - RI - #3987	
   declare buff_cursor CURSOR   FOR
   SELECT acctno,ItemID,stocklocn ,agrmtno,trandate												--IP - 27/06/11 - CR1212 - RI - #3987	   
   FROM facttrans 
   JOIN branch b ON b.branchno= facttrans.stocklocn
   WHERE buffno=0
   OPEN buff_cursor
   FETCH NEXT FROM buff_cursor INTO @acctno,@itemID,@stocklocn,@agrmtno,@trandate				--IP - 27/06/11 - CR1212 - RI - #3987	
   WHILE @@FETCH_STATUS = 0
   BEGIN
      UPDATE branch SET hibuffno = hibuffno + 1 WHERE branchno = @stocklocn
      SELECT @newbuffno =hibuffno FROM branch WHERE branchno = @stocklocn
      
      UPDATE facttrans SET buffno = @newbuffno WHERE acctno = @acctno
      AND agrmtno = @agrmtno AND ItemID = @itemID AND stocklocn =@stocklocn						--IP - 27/06/11 - CR1212 - RI - #3987								
      AND trandate = @trandate
      IF @@ERROR !=0
          SELECT @acctno,@stocklocn,@itemID														--IP - 27/06/11 - CR1212 - RI - #3987				

    FETCH NEXT FROM buff_cursor INTO @acctno,@itemID,@stocklocn,@agrmtno,@trandate				--IP - 27/06/11 - CR1212 - RI - #3987	


   END

   CLOSE buff_cursor
   DEALLOCATE buff_cursor

    /* FR453 - hold tax exempt customers in a global array rather than  */
    /* select FROM custcatcode for each order row found below.          */
    /* There shouldn't be hundreds AND hundreds of these, so holding in */
    /* memory is ok        */
    
    /* FR358 */
    /* As revise agreement writes out lineitems again to facttrans, then we  */
    /* need to make sure we only send over to fact the latest ones.          */
    /* Incidentally, this prevents multiple rows of the same item no being   */
    /* sent over - this has always happened AND FACT have amended their code */
    /* to take care of it, but it's better NOT to send them in the 1st place */


    select AcctNo, agrmtno, tccode, max(TranDate) as trandate, buffno, ItemID, stocklocn
    into #temp_maxdate 
    FROM facttrans 
    group by acctno, agrmtno, tccode, buffno, ItemID, stocklocn

    
    /* TC58's are a bit of a special case. They are written by delivery  */
    /* NOTification AND so, unlike TC61's, have different trandates for  */
    /* the same accountno. This means that we have to UPDATE all TC58    */
    /* trandate values for an account to be that of the highest trandate */
    /* for that account - otherwise only 1 TC58 per account will be      */
    /* picked up by this fact interface AND the rest deleted!            */
    /* AA 18 feb 07 Pick up the latest TC 58 record only or will be duplicate key on update */

    UPDATE facttrans
    SET   trandate = #temp_maxdate.trandate
    FROM   #temp_maxdate
    WHERE  facttrans.acctno =  #temp_maxdate.acctno 
    AND    facttrans.agrmtno = #temp_maxdate.agrmtno
    AND    facttrans.tccode =  #temp_maxdate.tccode 
    AND    facttrans.tccode = 58
    AND NOT EXISTS (SELECT * FROM facttrans f where f.acctno=#temp_maxdate.acctno 
    AND f.agrmtno = #temp_maxdate.agrmtno
    AND f.tccode = #temp_maxdate.tccode AND f.tccode=58 and f.acctno= facttrans.acctno
    AND f.ItemID = facttrans.ItemID AND f.stocklocn =facttrans.stocklocn and f.tccode = facttrans.tccode		--IP - 27/06/11 - CR1212 - RI - #3987	
    AND f.trantype =facttrans.trantype AND f.trandate>=facttrans.trandate )   

    /* Remove facttrans rows that are before the max trandate as selected    */
    /* above. This is so that any superseded rows will NOT be picked up next */
    /* time the fact interface is run (coz they won't be deleted later).     */

             delete FROM facttrans   
             WHERE trandate <  
             (select trandate  
              FROM     #temp_maxdate   t 
              WHERE  t.acctno = facttrans.acctno 
              AND    t.agrmtno = facttrans.agrmtno 
              AND    t.tccode = facttrans.tccode
              AND    t.buffno = facttrans.buffno
              AND    t.ItemID = facttrans.ItemID
              AND    t.stocklocn = facttrans.stocklocn) 


    /*
    ** First load the new orders AND order amendments into the
    ** array.
    */
    /* FR453 - remove the custacct join AND UPDATE the custid FROM the   */
    /* custacct table as a separate UPDATE afterwards - makes it quicker */

SELECT code AS discountcategory INTO #discounts 
FROM code WHERE category = 'pcdis'   

 

CREATE TABLE #OrdersandDels (
	AcctNo varchar (12) NOT NULL ,
	AgrmtNo int NOT NULL ,
	BuffNo varchar (20) NULL ,
	custid varchar (20) NOT NULL ,
	BuffBranchNo smallint NULL ,
	Code varchar (30) NULL ,
	DateAcctOpen datetime NULL ,
	DeliveryFlag varchar (1) NULL ,
	TranDate datetime NOT NULL ,
	EmpeeNoSale varchar(9) not null ,
	HeaderWareHouseNo varchar (3) NOT NULL ,
	ItemNo varchar (10) NOT NULL ,
	ItemType varchar(1) NOT NULL,
	OriginalItem varchar (10)  ,
	LineWareHouseNo varchar (3) NOT NULL ,
	Price float NULL ,
	Quantity float NULL ,
	Value float NULL ,
	SOA varchar (4) NOT NULL ,
	TCCode varchar (2) NOT NULL ,
	TranType varchar (3) NOT NULL ,
	taxtype char (1) NOT NULL ,
	agrmttaxtype varchar (1) NOT NULL ,
	taxrate float NOT NULL ,
	StockLocn smallint NOT NULL ,
	TransRefNo int NOT NULL ,
	MoreRewardsNo varchar (13) NOT NULL ,
	PaymentMethod varchar (2) NOT NULL ,
	replacementmarker varchar (10) NULL ,
	termstype varchar (2) NOT NULL ,
        type char(1), -- D for delivery O for order C for cancellation
        retstocklocn smallint, -- the below transactions for delivery only
        delorcoll char(1),
        warrantyitemno varchar(10),
        contractno varchar(10),
        warrantydets smallint,
        iswarranty smallint,
        linkeditemIndicator char(1),
        vardatedel varchar(8),
        taxamt money,
        retitemno varchar(10),	
        retwarehouseno char(3),
        branchno smallint,
        Dateagrmt varchar(6),
        datedel datetime,
        [name] varchar(60),
        cusaddr1 varchar(50),
        cusaddr2 varchar(50),
        cusaddr3 varchar(50),
        cuspocode varchar(20),
        taxexempt smallint,
        category smallint,
        affinity smallint,
       parentitemno varchar(10),
       parentlocation smallint,
       telno varchar(20),
	   warrantyrenewcount int ,
      originalStockacctno char(12),
      originalcontractno varchar(10),
      ChargeType CHAR(1),
      ItemID int,													--IP - 23/06/11 - CR1212 - RI - #3987					
      ParentItemID int,												--IP - 23/06/11 - CR1212 - RI - #3987
      OriginalItemItemID int,										--IP - 27/06/11 - CR1212 - RI - #3987
      WarrantyItemID int,											--IP - 27/06/11 - CR1212 - RI - #3987	
      RetItemID int,												--IP - 27/06/11 - CR1212 - RI - #3987					
	  ServiceRequestNo int,		-- #12948
	  Discount bit, -- interface linked discount to merchandising
      CashPrice money, -- interface cash price for orders to merchandising
	  AcctType char(1), -- for matching to promoprice
	  PromotionId int -- interface promotion Id for orders to merchandising
	)

   -- first inserting orders
   insert into #OrdersandDels
   (AcctNo,AgrmtNo,BuffNo,custid,
   BuffBranchNo,Code,DateAcctOpen,  DeliveryFlag,
   TranDate,EmpeeNoSale,HeaderWareHouseNo,ItemNo, ItemType,
   LineWareHouseNo,Price,Quantity,Value,
   SOA,TCCode,TranType,taxtype,
   agrmttaxtype,taxrate,StockLocn,TransRefNo,
   MoreRewardsNo,PaymentMethod,replacementmarker,termstype,
   type,dateagrmt, ItemID,s.ServiceRequestNo,
   parentitemno, ParentItemID, parentlocation)										-- #12948
  
   select ft.AcctNo, ft.AgrmtNo, left(ft.acctno,10) , '' ,
   convert (smallint,left(ft.acctno,3)) ,   CONVERT(varchar,'') , ac.DateAcctOpen, ag.DeliveryFlag, 
   ft.TranDate, convert(varchar(9),ag.EmpeeNoSale),  b1.WareHouseNo, ft.ItemNo, si.itemtype,
   b2.WareHouseNo, ft.Price, ft.Quantity, ft.Value, 
   isnull(ag.SOA,'XXXX'), ft.TCCode, ft.TranType, ct.taxtype,
   ct.agrmttaxtype, si.taxrate, ft.StockLocn, 0 ,						--IP - 27/06/11 - CR1212 - RI - #3987
   '' as MoreRewardsNo,ag.PaymentMethod , NULL,ac.termstype,
   'O',replace(convert (varchar,ac.dateacctopen,5),'-',''), ft.ItemID, s.ServiceRequestNo,
   ISNULL(l.parentitemno, ''), ISNULL(l.ParentItemID, 0), ISNULL(l.parentlocation, 0)
   FROM FactTrans ft INNER JOIN Agreement ag ON ag.AcctNo = ft.AcctNo
   AND ag.AgrmtNo = ft.AgrmtNo
   INNER JOIN  Acct ac ON ac.AcctNo = ft.AcctNo
   INNER JOIN Branch b1 ON  b1.BranchNo = convert(integer,left(ft.AcctNo, 3))
   INNER JOIN Branch b2 ON b2.BranchNo = ft.StockLocn
   INNER JOIN StockInfo si ON si.ID = ft.ItemID
   INNER JOIN StockQuantity sq ON si.ID = sq.ID AND ft.stocklocn = sq.stocklocn
   INNER JOIN Country ct ON ct.countrycode = b1.countrycode
   INNER JOIN #temp_maxdate t ON ft.acctno = t.acctno AND ft.agrmtno = t.agrmtno AND ft.tccode = t.tccode AND ft.trandate = t.trandate
   AND ft.buffno = t.buffno AND ft.ItemID = t.ItemID AND ft.stocklocn = t.stocklocn
   LEFT OUTER JOIN lineitem l 
   ON l.acctno = ft.acctno
        AND l.agrmtno = ft.agrmtno
        AND l.ItemID = ft.ItemID
        AND l.stocklocn = ft.stocklocn
        AND l.price = ft.price
        AND l.quantity > 0
	Left outer join ServiceChargeAcct s on ft.acctno=s.acctno			-- #12948				 
   WHERE ft.TranType    = '01'
   AND si.iupc NOT in ('ADDCR','ADDDR') --add to transactions don't go to FACT 2000		--IP - 29/06/11 - CR1212 - RI - #3987
   AND (((si.SparePart = 1 OR si.itemno in (@serviceItemLabour, @serviceItemPartsCourts, @serviceItemPartsOther)) and @interfaceMethod = 'Parts') or @interfaceMethod = 'FACT')	--IP - 03/06/11 - CR1212 - RI - Spare Parts --IP - 22/06/11 - CR1212 - RI - Check country parameter
   
   
    drop table #temp_maxdate
    -- remove cash and go orders 61/58 as not required.

    --MERCHANDISING - INCLUDE ORDERS FOR PAID AND TAKEN

  /* Now UPDATE the custid FROM the custacct table. This was originally part */
    /* of the above select but that proved to slow it down too much. (FR453)   */

    UPDATE  #OrdersandDels
    SET   custid = isnull(custacct.custid ,'')
    FROM   custacct
    WHERE #OrdersandDels.acctno = custacct.acctno
    AND    custacct.hldorjnt = 'H'
    
    -- now updating orders and deliveries verifying amount delivered
    select d.acctno,si.iupc as itemno,sq.stocklocn,d.quantity, convert(int,0) as delivered,d.agrmtno,		--IP - 27/06/11 - CR1212 - RI - #3987
    convert(int, 0) as outstanding, d.ItemID																--IP - 27/06/11 - CR1212 - RI - #3987
    into #orders
    from #ordersanddels d inner join stockinfo si on si.ID = d.ItemID									--IP - 27/06/11 - CR1212 - RI - #3987
    inner join stockquantity sq on si.id = sq.id and d.stocklocn = sq.stocklocn							--IP - 27/06/11 - CR1212 - RI - #3987
    where d.type = 'O'																					--IP - 23/12/11 - #8849 - LW73492 - Merged from 5.13
    and si.itemtype = 'S'																				--IP - 27/06/11 - CR1212 - RI - #3987
    
    -- verifying delivered amount....
    update #orders 
    set delivered = (select sum(quantity) from delivery d where  d.acctno = #orders.acctno
    and d.ItemID =#orders.ItemID and d.stocklocn=#orders.stocklocn and d.agrmtno = #orders.agrmtno)		--IP - 29/06/11 - CR1212 - RI - #3987
    
    update #orders set outstanding = delivered -quantity
   
    -- here we are updating the 61/58 record with the correct amounts
    -- providing there is not already a subsequent 58 zeroising record
    -- already present....

    /*
    ** Now append all the delivery details with runno = 0
    */

    /* FR453 - Original query was getting quite complex AND involved joining */
    /* seven tables. To ease the pain, build a temp_delivery table here AND  */
    /* join to that rather the the original one.                             */

    /* CR285 addition of more rewards card number */
         select d.*, /*isnull (l.taxamt,*/0  as taxamt, '             ' as morerewardsno
        , datedel as vardatedel
       INTO #temp_delivery 
       FROM  Delivery d
       inner join stockinfo si on si.ID = d.ItemID			--IP - 29/06/11 - CR1212 - RI - #3987
       inner join stockquantity sq on sq.ID = si.ID			--IP - 29/06/11 - CR1212 - RI - #3987
       and d.stocklocn = sq.stocklocn						--IP - 29/06/11 - CR1212 - RI - #3987
       and (((si.SparePart = 1 OR si.itemno in (@serviceItemLabour, @serviceItemPartsCourts, @serviceItemPartsOther)) and @interfaceMethod = 'Parts') or @interfaceMethod = 'FACT')	--IP - 03/06/11 - CR1212 - RI - Spare Parts --IP - 22/06/11 - CR1212 - RI - Check country parameter
       left join Lineitem l on  
       d.AcctNo     = l.AcctNo
       AND   d.AgrmtNo     = l.AgrmtNo
       AND   d.ItemID = l.ItemID							--IP - 29/06/11 - CR1212 - RI - #3987
       AND   d.StockLocn    = l.StockLocn
       AND d.contractno =l.contractno
       AND d.ParentItemID = l.ParentItemID					--IP - 29/06/11 - CR1212 - RI - #3987  --RM2962 / LW73164  RM 25/01/11
       LEFT OUTER JOIN stockInfo sr on d.RetItemId=sr.ID	-- #10089 
       WHERE d.runno = 0
       AND   d.datedel <= GETDATE()
       AND si.iupc NOT in ('ADDDR','ADDCR')					--IP - 29/06/11 - CR1212 - RI - #3987
	   AND NOT EXISTS(select * from code c					--#17979 - Exclude warranties
						where c.code = si.category
						and c.category = 'WAR')
       
    
    --IP - 03/06/11 - CR1212 - RI 
   
	  IF(@interfaceMethod = 'FACT')	--IP - 22/06/11 - CR1212 - RI - #3987
	  BEGIN
		  print 'doing rebates'
		/* FR 909 - Insert Rebates into temporary table so they can be sent over to FACT */
		  insert into   #temp_delivery 
		  select d.*, 0,'',  replace (convert (varchar, getdate(),05),'-','')   
		  FROM   delivery d 
		  inner join stockinfo si on d.ItemID = si.ID		--IP - 29/06/11 - CR1212 - RI - #3987
		  WHERE  runno = 0 
		  AND    d.datedel <= GETDATE() 
		  AND    si.iupc = 'RB'								--IP - 29/06/11 - CR1212 - RI - #3987
		  AND NOT exists (select * FROM  #temp_delivery   t 
		  inner join stockinfo si2 on t.ItemID = si2.ID
		  WHERE t.acctno =d.acctno AND si2.iupc ='RB')		--IP - 29/06/11 - CR1212 - RI - #3987
      END
   
    /* FR 1054 */
     UPDATE #temp_delivery
     SET itemno = rtrim(itemno)
     declare @taxtype char(1),@agrmttaxtype char(1),@loyaltycard char(1),@currenttaxrate float,@netcashandgo smallint
     select @taxtype =taxtype,
     @agrmttaxtype = agrmttaxtype,
     @loyaltycard = loyaltycard,
     @currenttaxrate = taxrate,
     @netcashandgo = netcashandgo
     FROM country
     
     
    -- tax type inclusive - so price on database includes tax. Agreement Tax type exclusive so seperate tax line. Example country: Jamaica
    if @taxtype ='I' AND @agrmttaxtype ='E' 
    begin
    -- changing values due to tax but only for rebates or non stock items. 
             UPDATE   #temp_delivery
             SET  taxamt =   round((#temp_delivery.transvalue * si.taxrate) / 100,2)	--IP - 29/06/11 - CR1212 - RI - #3987
             FROM stockinfo si inner join stockquantity sq on si.ID = sq.ID				--IP - 29/06/11 - CR1212 - RI - #3987
             WHERE   #temp_delivery.ItemID = si.ID										--IP - 29/06/11 - CR1212 - RI - #3987
             AND   #temp_delivery.stocklocn = sq.stocklocn								--IP - 29/06/11 - CR1212 - RI - #3987
             AND   (si.iupc = 'RB') 
              
    END
	-- tax type exclusive no on database- agreement includes tax this will be extracted in FACT export so agreement sent net of tax but again only or non stocks where tax rate not stored on database.
    if @taxtype ='E' AND @agrmttaxtype ='I' 
    begin
          UPDATE  #temp_delivery
           SET   taxamt = isnull(round(transvalue -(( #temp_delivery.transvalue *100) / (100 *  si.taxrate)),2),0)
             FROM stockinfo si inner join stockquantity sq on si.ID = sq.ID				--IP - 29/06/11 - CR1212 - RI - #3987
             WHERE  #temp_delivery.ItemID = si.ID										--IP - 29/06/11 - CR1212 - RI - #3987 
             AND   #temp_delivery.stocklocn = sq.stocklocn								--IP - 29/06/11 - CR1212 - RI - #3987 
             AND  si.iupc = 'RB'														--IP - 29/06/11 - CR1212 - RI - #3987 
             AND isnull(si.taxrate,0) > 0 -- but only if tax rate > 0....				--IP - 29/06/11 - CR1212 - RI - #3987 
             
    end        

--SELECT -64.0/(-320 + 64.0)
    /* CR285 updating of more rewards card number, if appropriate
    ** For epos sales, AND returns. The epos sale or epos return loyalty card number
    ** will take precidenece over the loyalty card number held against the customer
    */
    IF @loyaltycard = 'Y'
    BEGIN
              UPDATE   #temp_delivery
              SET   morerewardsno = eposloyaltycard.morerewardsno 
              FROM   eposloyaltycard 
              WHERE   #temp_delivery.transrefno = eposloyaltycard.transrefno  
              AND     #temp_delivery.datetrans  = eposloyaltycard.datetrans  
              AND     #temp_delivery.acctno     = eposloyaltycard.acctno 
    END       
    

    /* FR453 - speed up this query by removing the 'union'.                     */
    /* The calling code will process the value of the itemno only whereas we    */
    /* need it to process both itemno AND retitemno.                            */
    /* NB - We are interested in the ITEMNO WHERE the retitemno is '' or null   */
    /*      AND the RETITEMNO WHERE the retitemno is NOT '' AND NOT null.       */
    /* To achieve this without an expensive 'union' then select both itemno AND */
    /* retitemno AND use a separate UPDATE to SETitemno to retitemno WHERE     */
    /* retitemno is NOT null AND NOT blank.                                     */

   CREATE TABLE #serviceaccounts (acctno CHAR(12))
   
   INSERT INTO #serviceaccounts 
   SELECT value FROM countrymaintenance WHERE NAME IN ('Service Stock Account','Service Warranty Account','Service Internal Account')
   AND DATALENGTH(value)=12
   
   -- inserting into agreement for accounts which are missing -- mainly service accounts UAT 407 , but will work. 
	INSERT INTO agreement
	(origbr, acctno, agrmtno, dateagrmt,  --1
	empeenosale, datedepchqclr, holdmerch, holdprop, --2
	datedel, datenextdue, oldagrmtbal, cashprice, --3
	discount, pxallowed, servicechg, sdrychgtot, --4
	agrmttotal, deposit, codflag, soa, paymethod, --5
	unpaidflag, deliveryflag, fulldelflag, PaymentMethod, --6
	empeenoauth, dateauth, empeenochange, datechange, --7
	AdminFee, InsCharge, datefullydelivered, createdby,--8
	 paymentcardline, paymentholidays, AgreementPrinted, --9
	TaxInvoicePrinted, WarrantyPrinted, source)
	SELECT 
	0, acctno,agrmtno,max(datedel), --1
	0,NULL,'N','N', --2
	max(datedel),NULL,0,SUM(transvalue),--3
	0,0,0,0,--4
	SUM(transvalue),0,'N','',0,--5
	'N','Y','Y',0, --6
	99999,max(datedel),99999,GETDATE(),--7
	0,0,max(datedel),99999,
	0,0,'Y',
	'Y','Y','FCEX'
	FROM  delivery
	WHERE runno = 0 AND 
	NOT EXISTS (SELECT * FROM agreement g WHERE g.acctno= delivery.acctno
	AND g.agrmtno =delivery.agrmtno)
	AND EXISTS (SELECT * from acct a WHERE a.acctno = delivery.acctno) --IP - 12/12/11 - #8876
	AND datedel > DATEADD(DAY,-30,GETDATE()) -- don't interface or insert really old transactions --
	--70835 was missing service accounts to export..... 
	GROUP BY acctno,agrmtno
   -- Deliveries.....
   insert into #OrdersandDels
   (AcctNo,AgrmtNo,BuffNo,custid,--1
   BuffBranchNo,Code,DateAcctOpen,  DeliveryFlag,--2
   TranDate,EmpeeNoSale,HeaderWareHouseNo,ItemNo, ItemType, --3
   LineWareHouseNo,Price,Quantity,Value,--4
   SOA,TCCode,TranType,taxtype,--5
   agrmttaxtype,taxrate,StockLocn,TransRefNo,--6
   MoreRewardsNo,PaymentMethod,replacementmarker,termstype,--7
   type,       retstocklocn ,delorcoll , warrantyitemno,--8
   contractno, warrantydets,iswarranty,  linkeditemindicator,--9
   vardatedel, taxamt,dateagrmt ,         retitemno,--10 fact wants date format ddmmyy
   retwarehouseno,  branchno, parentitemno,
   ItemID, ParentItemID, WarrantyItemID,RetItemID,ServiceRequestNo)  -- #12948

        /* CR285 addition of more rewards card number */
        -- LiveWire Issue 69656 - FACT Export should be using stock location and not buffbranchno
         SELECT ag.AcctNo, dl.agrmtNo, convert(varchar(3),dl.stocklocn) +'\' + convert(varchar(10),dl.BuffNo), ca.Custid, --1 
         dl.stocklocn, '', ac.DateAcctOpen /* dateagrmt*/, ag.DeliveryFlag, --2
         dl.datetrans,  convert(varchar(9),ag.EmpeeNoSale), b1.WarehouseNo, dl.itemno, si.itemtype, --3
         b2.WarehouseNo ,  0 /* Price*/, dl.Quantity, dl.Transvalue,  --4
         isnull(ag.SOA,'XXXX'), '00' /* TCCode*/, '00' /*TranType*/,@taxtype,  --5
          ct.agrmttaxtype, 0 /* taxrate*/, dl.stocklocn, isnull(dl.transrefno,0), --6
         dl.MoreRewardsNo ,ag.PaymentMethod ,dl.replacementmarker,   ac.termstype,--7
         'D',isnull(dl.retstocklocn,0), dl.delorcoll,'',  --8
         isnull (dl.contractno, '') , 0 , 0,'', --9
         vardatedel, dl.taxamt,replace(convert (varchar,dateacctopen,5),'-',''),isnull(sr.itemno,''),  -- #10089 
     	    '00',dl.branchno, dl.parentitemno, --11
		 dl.ItemID, dl.ParentItemID, 0, dl.RetItemID,s.ServiceRequestNo								-- #12948
		 FROM Acct ac inner join Agreement ag on ac.AcctNo = ag.AcctNo				--IP - 29/06/11 - CR1212 - RI - #3987 - replaced below
		 inner join CustAcct ca on ag.AcctNo = ca.AcctNo 
		 inner join #temp_delivery dl on ag.AcctNo = dl.AcctNo 
		 and ag.AgrmtNo = dl.AgrmtNo
		 inner join stockinfo si on si.Id = dl.ItemID
		 inner join Branch b1 on b1.BranchNo = convert(integer,left(ag.AcctNo,3)) 
		 inner join  Branch b2 on b2.BranchNo = dl.StockLocn 
		 inner join Country ct on ct.countrycode = b1.countrycode
		 LEFT OUTER JOIN stockInfo sr on dl.RetItemId=sr.ID	-- #10089
		Left outer join ServiceChargeAcct s on dl.acctno=s.acctno			-- #12948		 
         WHERE ca.hldorjnt = 'H'  
		 
        /* The union used to specificaly include warranties FROM the delivery  */
        /* table when they were stock items.                                   */

         update #OrdersandDels set Originalitem = itemno
		 update #OrdersandDels set OriginalItemItemID= ItemID					--IP - 29/06/11 - CR1212 - RI - #3987 

         update #OrdersandDels set parentitemno = si.iupc,						--IP - 29/06/11 - CR1212 - RI - #3987 - replaces the below
								   ParentItemID = si.ID
            
         from lineitem l inner join #OrdersandDels d on  l.acctno =d.acctno and  l.agrmtno =d.agrmtno 
         inner join stockinfo si on l.ParentItemID = si.ID
         where l.ItemID =d.ItemID and l.stocklocn =d.stocklocn 
         and l.contractno=d.contractno
         and d.parentitemno is null  --RM2962 / LW73164  RM 25/01/11
         
		 update #OrdersandDels set --parentitemno = l.parentitemno
            parentlocation = l.parentlocation --RM2962 / LW73164  RM 25/01/11
         from lineitem l
         where 
         l.acctno =#OrdersandDels.acctno and l.agrmtno =#OrdersandDels.agrmtno 
         and l.ItemID =#OrdersandDels.ItemID and l.stocklocn =#OrdersandDels.stocklocn					--IP - 29/06/11 - CR1212 - RI - #3987 
         and l.contractno=#OrdersandDels.contractno and l.ParentItemID = #OrdersandDels.ParentItemID	--IP - 29/06/11 - CR1212 - RI - #3987 
         and #OrdersandDels.parentlocation is null  --RM2962 / LW73164  RM 25/01/11



   /* Now doing cancellations */
   insert into #OrdersandDels
   ( AcctNo,AgrmtNo,BuffNo,custid,--1
   BuffBranchNo,Code,DateAcctOpen,  DeliveryFlag,--2
   TranDate,EmpeeNoSale,HeaderWareHouseNo,ItemNo, ItemType,--3
   LineWareHouseNo,Price,Quantity,Value,--4
   SOA,TCCode,TranType,taxtype,--5
   agrmttaxtype,taxrate,StockLocn,TransRefNo,--6
   MoreRewardsNo,PaymentMethod,replacementmarker,termstype,--7
   dateagrmt,type, 
   ItemID)  --8																			--IP - 29/06/11 - CR1212 - RI - #3987 									

  SELECT  ft.AcctNo,ft.AgrmtNo, left(ft.AcctNo ,10), isnull(cac.custid,''),--1
   ac.branchno, ca.Code,ac.DateAcctOpen,  ag.DeliveryFlag, --2
   ft.TranDate, convert(varchar(9),ag.EmpeeNoSale),  b1.WareHouseNo,  ft.ItemNo, si.itemtype,--3
    b2.WareHouseNo,  0,   ft.quantity,   0,--4			-- 16381
    isnull(ag.SOA,'XXXX'), ft.TCCode, ft.TranType, @taxtype,--5
    @agrmttaxtype,'', ft.StockLocn, 0, --6
   '','','','',  --7 
    replace(convert (varchar,ac.dateacctopen,5),'-',''),'C', --8
    ft.ItemID																			--IP - 29/06/11 - CR1212 - RI - #3987 		
		   FROM Branch b1, 
	           FactTrans ft INNER JOIN Agreement ag ON ag.AcctNo = ft.AcctNo 
	           AND ag.AgrmtNo = ft.AgrmtNo 
		   INNER JOIN Acct ac ON ac.AcctNo = ft.AcctNo 
		   INNER JOIN StockInfo si on si.Id = ft.ItemID
		   INNER JOIN Branch b2 ON b2.BranchNo = ft.StockLocn			-- 16381
		   LEFT JOIN Cancellation ca ON ca.AcctNo = ft.AcctNo --Merchandising - removing link to cancellation, record all cancels
		   AND ca.AgrmtNo = ft.AgrmtNo 
		   LEFT JOIN custacct cac ON cac.AcctNo = ac.AcctNo AND cac.hldorjnt='H'
		   WHERE ft.TranType = '04' 
		   AND b1.BranchNo = convert(integer,left(ft.AcctNo, 3))

    IF @loyaltycard = 'Y'
    begin
              UPDATE   #OrdersandDels 
              SET   morerewardsno = customer.morerewardsno 
              FROM   customer 
              WHERE   #OrdersandDels.custid = customer.custid  
              AND     #OrdersandDels.morerewardsno = '' and customer.morerewardsno !=''
    end
    
    -- the warehouse for discounts will always be the account number
    update #OrdersandDels set linewarehouseno = b.warehouseno from						--IP - 29/06/11 - CR1212 - RI - #3987  - Replaces the below
    branch b,stockinfo si inner join stockquantity sq on si.ID = sq.ID
    where b.branchno =convert(smallint,left(#OrdersandDels.acctno,3))
    and si.ID =#OrdersandDels.ItemID
    and sq.stocklocn = #OrdersandDels.stocklocn
    and si.category in (select code from code where category = 'PCDIS') --discount categories
    
    /* Now remove warranty dets that were selected FROM the delivery table only in    */

    /* Don't delete FROM delivery table as lineitem warranties are
       ignored if deliveryflag ='Y' - fr 1073 commented out for the time being  */

    
    /* SETthe itemno to be the retitemno WHERE the retitemno has a value - Part of FR453 */

    UPDATE   #OrdersandDels 
    SET  ItemNo = RetItemNo 
    WHERE   #OrdersandDels.RetItemNo is NOT NULL 
    AND     #OrdersandDels.RetItemNo != '' and type ='D' -- deliveries only

    /* SETthe retwarehouseno WHERE the retstocklocn has a value - FR562 */
    

      UPDATE   #OrdersandDels
      SET   RetWarehouseNo = branch.warehouseno 
             FROM   branch 
             WHERE  branch.branchno =   #OrdersandDels.RetStockLocn  
             AND    #OrdersandDels.RetStockLocn is NOT NULL 
             AND    #OrdersandDels.RetStockLocn != ''
             AND   type ='D' -- deliveries and collections only
    
    /* if using  .NET  cash AND go employee salesperson information stored on agreement table  
    otherwise stored on fintrans table*/
    if @netcashandgo= 0
    begin
       UPDATE   #OrdersandDels 
            SET   DateAgrmt = replace(convert (varchar,  #OrdersandDels.DateDel,5),'-',''), 
                   EmpeeNoSale = convert(varchar(9),fintrans.empeeno )
            FROM   fintrans 
            WHERE   #OrdersandDels.custid = 'PAID & TAKEN' 
            AND     #OrdersandDels.transrefno = fintrans.transrefno 
            AND     #OrdersandDels.acctno = fintrans.acctno
     end  
     else /* still need to UPDATE the date agreement to the date of delivery*/
     BEGIN 
    --72368 Due to service accounts now changing this so that any agreement > 1 will be updated for type 5 accounts which include service as well as paid and taken
            UPDATE   #OrdersandDels
            SET   DateAgrmt =     replace(convert (varchar,  g.Dateagrmt ,5),'-','') 
            FROM   agreement g
            WHERE   #OrdersandDels.acctno LIKE '___5%'
            AND     #OrdersandDels.agrmtno = g.agrmtno
            AND     #OrdersandDels.acctno = g.acctno
            AND #OrdersandDels.AgrmtNo >1
     end  
    --72368 Due to service accounts now changing this so that any agreement > 1 will be updated for type 5 accounts which include service as well as paid and taken
    update #ordersanddels 
     SET   DateAgrmt =     replace(convert (varchar,  g.dateagrmt,5),'-','') ,
			dateacctopen=  g.dateagrmt
            FROM   agreement g
            WHERE   #OrdersandDels.acctno LIKE '___5%'
            AND     #OrdersandDels.agrmtno = g.agrmtno
            AND     #OrdersandDels.acctno = g.acctno
            AND #OrdersandDels.AgrmtNo >1
            

    /* here am selecting transtypecode to determine if
       Redelivery after repo - if so then you should take
       tax off stockitem table NOT using taxamt of lineitem table
     */
    select d.acctno,d.transrefno,f.transtypecode 
    into   #temp_types
    FROM    #OrdersandDels   d, fintrans f 
    WHERE f.acctno = d.acctno AND f.transrefno = d.transrefno
    AND d.transrefno !=0 and f.transtypecode ='RDL'

  -- now we are updating the tax rate issue is this gets overwritten later on.
   UPDATE   #OrdersandDels															--IP - 29/06/11 - CR1212 - RI - #3987  - Replaces the below
   SET   taxrate = isnull(si.taxrate, 0) 
   FROM   stockinfo si inner join stockquantity sq on si.ID = sq.ID
   WHERE   #OrdersandDels.ItemID = si.ID 
   AND     #OrdersandDels.stocklocn = sq.stocklocn
      
  declare @num int
  select @num = count(*) from taxratehistory
  if @num = 1 -- only one tax rate so not much of a problem
  begin
     update #OrdersandDels set taxrate = t.taxrate from taxratehistory t where #OrdersandDels.taxrate >0
  end
  else
  begin -- slightly more complicated

     update #OrdersandDels set taxrate = t.taxrate
     from taxratehistory t 
     where #OrdersandDels.dateacctopen > t.datefrom and #OrdersandDels.taxrate >0
     and (#OrdersandDels.dateacctopen < t.dateto OR t.dateto='1-jan-1900') -- this is the current live tax rate
     AND NOT (#OrdersandDels.custid = 'PAID & TAKEN' AND #OrdersandDels.agrmtno=1)-- don't know when originally delivered so use current tax rate
     
END
     -- now for items with special rate
     update #OrdersandDels set taxrate = t.specialrate 
     from taxitemhist t 
     where #OrdersandDels.dateacctopen > t.datefrom and #OrdersandDels.taxrate >0
     AND t.ItemID = #OrdersandDels.ItemID													--IP - 29/06/11 - CR1212 - RI - #3987 
     AND (#OrdersandDels.dateacctopen <t.dateto OR  ISNULL(t.dateto,'1-jan-1900') ='1-jan-1900') -- this is the current live tax rate
     AND NOT (#OrdersandDels.custid = 'PAID & TAKEN' AND #OrdersandDels.agrmtno=1)

  -- setting tax rate 0 where customer tax exempt  
  update #OrdersandDels set taxrate =0 , taxexempt = 1
  from custacct t,custcatcode c 
  where c.code in('0TAX','TE') and t.acctno =#OrdersandDels.acctno and t.custid = c.custid 
  and t.hldorjnt ='H'  and c.datedeleted is null and t.custid not like 'paid &%' -- exclude paid & taken /cash and go
  
-- setting tax rate 0 where account tax exempt  
  update #OrdersandDels set taxrate =0 
  from custacct t,acctcode c 
  where c.code in ('TE') and t.acctno =#OrdersandDels.acctno and t.acctno = c.acctno 
  and t.hldorjnt ='H'  and c.datedeleted is null and t.custid not like 'paid &%' -- exclude paid & taken /cash and go
-- now updating taxrate setting this to 0. 
UPDATE #ordersanddels SET taxrate =ISNULL((SELECT MAX(p.taxrate) FROM #ordersanddels p		--IP - 29/06/11 - CR1212 - RI - #3987 - Replaces the below
 WHERE p.ItemID= #ordersanddels.ParentItemID AND p.stocklocn  
 =#ordersanddels.parentlocation AND p.acctno= #ordersanddels.acctno 
 AND p.agrmtno=  #ordersanddels.agrmtno ),taxrate)
 WHERE EXISTS (SELECT * FROM dbo.StockInfo s, #discounts d 
 WHERE s.ID = #ordersanddels.ItemID AND s.category = d.discountcategory)
 
  -- updating value for orders
  update #OrdersandDels set value = price * quantity where type='O' and value !=price *quantity
-- sorted...
    if @taxtype ='I' AND @agrmttaxtype ='E' 
    BEGIN
				--IP - 29/06/11 - CR1212 - RI - #3987 - Replaces the below
				UPDATE   #OrdersandDels
                SET  taxamt =  round((#OrdersandDels.value * #OrdersandDels.taxrate) / 100 ,2)
                FROM  stockinfo si inner join stockquantity sq on si.ID = sq.ID,  #temp_types -- #temp_types stores redeliveries after repo.....
                WHERE   #OrdersandDels.acctno =   #temp_types.acctno 
                AND   #OrdersandDels.transrefno =   #temp_types.transrefno 
                AND   #OrdersandDels.ItemID = si.ID 
                AND   #OrdersandDels.stocklocn = sq.stocklocn
				AND ISNULL(taxexempt,0) !=1
                
    END
    else
    if @taxtype ='E' AND @agrmttaxtype ='I' 
    BEGIN
				 --IP - 29/06/11 - CR1212 - RI - #3987 - Replaces the below
				  UPDATE   #OrdersandDels 
                    SET  taxamt = round(value -(#OrdersandDels.value *100)/(100 + #OrdersandDels.taxrate),2)
                    FROM  stockinfo si inner join stockquantity sq on si.ID = sq.ID,   #temp_types 
                    WHERE   #OrdersandDels.acctno =   #temp_types.acctno 
                    AND   #OrdersandDels.transrefno =   #temp_types.transrefno 
                    AND   #OrdersandDels.ItemID = si.ID 
                    AND   #OrdersandDels.stocklocn = sq.stocklocn
					AND ISNULL(taxexempt,0) !=1
					
    END	
		  UPDATE  #OrdersandDels										--IP - 29/06/11 - CR1212 - RI - #3987 - Replaces the below
         SET LinkedItemIndicator = 'A'   /* associated product*/
         WHERE exists (SELECT l.ItemID, l.acctno  
               FROM   lineitem e inner join lineitem l on e.ItemID = l.ParentItemID 
               and e.stocklocn = l.parentlocation 
               and e.acctno= l.acctno 
               and e.agrmtno =l.agrmtno 
               inner join stockinfo si on l.ItemID = si.ID
               inner join stockquantity sq on si.ID = sq.ID
               and l.stocklocn = sq.stocklocn
               WHERE  l.ItemID =   #OrdersandDels.ItemID  
			   AND    l.acctno =   #OrdersandDels.acctno  
               AND    l.ParentItemID <>0 
               AND    si.itemtype = 'S'
         AND e.iskit =0 )    /*237 NOT for kit products*/
         AND type ='D'
    
    if  @hasaffinity = 1  /*setting vardatedel for non affinities to blank*/
    begin 
    
		--IP - 29/06/11 - CR1212 - RI - #3987 - Replaces the below
		UPDATE #OrdersandDels SET vardatedel ='' ,Affinity = 1
        FROM stockinfo si inner join stockquantity sq on si.ID = sq.ID 
        WHERE  si.ID= #OrdersandDels.ItemID
        AND sq.stocklocn = #OrdersandDels.stocklocn and
        (si.category !=11 AND si.category NOT between 51 AND 59) 
        AND type ='D' 
    end
    else
    begin
        UPDATE  #OrdersandDels  SET vardatedel =''
    end	 		                    

    SET @DeliveryTotal = 0

            /* cannot have negative prices on FACT 2000 so if value less than 0
               quantity reversed
             */
        /* FACT 2000 rejects negative prices for deliveries even if
        a discount so need to make sure quantity <0, the price is value/quantity*/
        update #OrdersandDels set 
               quantity = -quantity
        where (quantity >=0 and value <0) Or (Quantity <= 0 AND Value >0)                 
         and type ='D'            
         
         

		update #OrdersandDels set 
               price = value/quantity
        where quantity !=0 and type ='D'

        update #OrdersandDels set 
               price = abs(value),quantity = 1
        where quantity =0 and type ='D' and value >=0

        update #OrdersandDels set 
               price = abs(value),quantity = -1
        where quantity =0 and type ='D' and value <0

        update #OrdersandDels set quantity = 1 where quantity = 0 and value <>0 

            /* Now populate the trantype - SETto '03' for positive deliveries  */
            /* AND to '13' for negative deliveries.                             */
            /* As we are checking the value column to determine this, we can    */
            /* also SETthe tccode - '32' for delivery, '11' for a return.      */
            /* (unless its a repo, tccode '17')        FR368                    */
        update #OrdersandDels set Trantype='13'
        where (Value < 0 OR (Value = 0 AND Quantity < 0)) AND type='D'

        update #OrdersandDels set TcCode='17'
        where (DelorColl='R' and Value < 0 OR (Value = 0 AND Quantity < 0)) AND type='D'

         /* If a repossession then calculate the tax element using TODAYS tax rate against
         ** the current value of the item (as specified in RetGoods screen) - this is even
         ** though the item may have originally been delivered at a different tax rate. Any
         ** shortfall remains on the customers account anyway.
         */

        /* TaxRate will be zero for tax-exempt customers/items */
 
		--IP - 29/06/11 - CR1212 - RI - #3987 - Replaces the below
		update #OrdersandDels set TCCode ='14' 
		from stockinfo si 
		where #OrdersandDels.ItemID = si.ID
		and si.iupc ='RB' and type ='D'
        and (value < 0   OR (Value = 0 and Quantity < 0) ) 
        
        --IP - 29/06/11 - CR1212 - RI - #3987 - Replaces the below
        update #OrdersandDels set Tccode ='32' 
        from stockinfo si
		where #OrdersandDels.ItemID = si.ID
		and si.iupc !='RB' and delorcoll ='D' and type ='D'
        and (value < 0   OR (Value = 0 and Quantity < 0) ) 
        
        --IP - 29/06/11 - CR1212 - RI - #3987 - Replaces the below
        update #OrdersandDels set Tccode ='11' 
        from stockinfo si
        where #OrdersandDels.ItemID = si.ID
        and si.iupc !='RB' and delorcoll not in ('D','R') and type ='D'
        and (value < 0   OR (Value = 0 and Quantity < 0) )  
        
        update #OrdersandDels set Taxamt = -Taxamt where Taxamt >0
        and (value < 0   OR (Value = 0 and Quantity < 0) )   and type ='D'

        update #OrdersandDels 
        set Trantype ='03',      TcCode='32'
        where type ='D'  and (value > 0   OR (value = 0 and Quantity > 0) )  
        
       

        UPDATE   #OrdersandDels
                SET  taxamt= round(taxamt,2)

        update #OrdersandDels 
        set name = left((c.firstname + ' ' + c.name) ,60)
        from customer c
        where #OrdersandDels.custid = c.custid
--print 'ol'
        update #OrdersandDels 
        set cusaddr1 = ca.cusaddr1,
    	cusaddr2 = ca.cusaddr2,
	    cusaddr3 = ca.cusaddr3,
        cuspocode = ca.cuspocode 
        from custaddress ca
        where #OrdersandDels.custid = ca.custid
        and ca.addtype='H' and ca.datemoved is null

        -- update if tax exempt
        update #OrdersandDels 
        set taxexempt = 1 
        from custcatcode c
        where c.custid= #OrdersandDels.custid and c.code ='0TAX'
        and c.datedeleted is null

    -- # 15273 - Re-instated
	update  #OrdersandDels set empeenosale =   ISNULL(c.FACTEmployeeNo,#OrdersandDels.empeenosale) 
        FROM   CourtsPerson c 
        WHERE  c.EmpeeNo = convert(int,#OrdersandDels.empeenosale)
        and c.FACTEmployeeNo not in ('','0')

	--IP - 29/06/11 - CR1212 - RI - #3987 - Replaces the below
	update  #OrdersandDels set category =  si.category
        FROM   stockinfo si inner join stockquantity sq on si.ID = sq.ID
        WHERE  sq.stocklocn = #OrdersandDels.stocklocn
        and si.ID = #OrdersandDels.ItemID

   update #OrdersandDels set telno= isnull(c.telno,'') from 
    custtel c where c.custid = #OrdersandDels.custid and c.tellocn ='H' and datediscon is null
-- make sure date is ddmmyy
   update #OrdersandDels set dateagrmt= '0' + dateagrmt where datalength(dateagrmt) ='5'

	/* Tax rates should be gotten from what is saved on lineitem for discounts which may have parent item 
	discount rate AA removing as taxrate calculated ab  */
	update #OrdersandDels set warrantyrenewcount = isnull((select count(*) from WarrantyRenewalPurchase w
	where   w.acctno = #OrdersandDels.acctno and w.ItemID = #OrdersandDels.ItemID),0)
	
	update #OrdersandDels set originalStockacctno = isnull(w.stockitemacctno,'') ,
	originalcontractno=w.originalcontractno	
	from WarrantyRenewalPurchase w
    where   w.acctno = #OrdersandDels.acctno and  w.contractno =#OrdersandDels.contractno ;

    UPDATE #OrdersandDels
    SET    ChargeType =
           ISNULL((SELECT TOP 1 s.ChargeType
                   FROM   ServiceChargeAcct s							--#17083
                   WHERE  s.AcctNo = #OrdersandDels.AcctNo),'')
 
    UPDATE #OrdersandDels
    SET    ChargeType = 'E'
    WHERE  ChargeType = 'W'
	
    /* set discount flag for Merchandising */

	update #OrdersandDels
	set Discount = 1
	where itemno in (
		select itemno 
		from stockinfo
		where category in
			(
				select code
				from code
				where category in ('PCDIS')
			) 
		)

	update #OrdersandDels
	set Accttype = case substring(acctno, 4,1)  
					when '4' then 'C'  
					WHEN  '5' then 'C'
					Else 'H'
					end

-- Set todays cash price for merchandising on orders only
    if(@priceFromStockLocation = 'false')
	BEGIN
        

        update o
        set CashPrice = unitpricecash
        from #OrdersandDels o 
        inner join stockitem i
            on i.id = o.itemid
            and i.stocklocn = left(o.acctno, 3)
        --where ( o.TranType = '01' and o.TCCode = '61' )

        --If there is no Sale location price default to stock location(Win Cosacs already does this)
        update o
        set CashPrice = unitpricecash
        from #OrdersandDels o 
        inner join stockitem i
            on i.id = o.itemid
            and i.stocklocn = o.StockLocn
        where o.CashPrice IS NULL

		
		update o
		set promotionid = p.PromotionId
		FROM    #OrdersandDels o 
        inner join promoprice p 
			on   p.itemId = o.itemID
            AND  p.stocklocn = left(o.acctno, 3)
            AND  hporcash = accttype
            AND  (fromdate < getdate() AND todate > getdate())
        where ( o.TranType = '01' and o.TCCode = '61' ) OR -- Order and Deliveries.
		      ( o.TranType = '03' and o.TCCode = '32' ) 

		update o
		set CashPrice = p.unitprice
		FROM    #OrdersandDels o 
        inner join promoprice p 
			on   p.itemId = o.itemID
            AND  p.stocklocn = left(o.acctno, 3)
            AND  hporcash = 'C'
            AND  (fromdate < getdate() AND todate > getdate())
        where ( o.TranType = '01' and o.TCCode = '61' ) OR
		      ( o.TranType = '03' and o.TCCode = '32' ) 

    END
    else
    BEGIN
        
        update o
        set CashPrice = unitpricecash
        from #OrdersandDels o 
        inner join stockitem i
            on i.itemno = o.itemno
            and i.stocklocn = o.stocklocn
        --where o.TranType = '01' and o.TCCode = '61'
    	
		update o
		set promotionid = p.PromotionId
		FROM    #OrdersandDels o 
        inner join promoprice p 
			on   p.itemId = o.itemID
            AND  p.stocklocn = o.stocklocn
            AND  hporcash = accttype
            AND  (fromdate < getdate() AND todate > getdate())
        --where o.TranType = '01' and o.TCCode = '61'

    END

select @deliverytotal = sum(value) from #ordersanddels
where type ='D'

select @OrderandDeliverytotal=  sum(value) from #ordersanddels

insert into lastfactexport 
( AcctNo, AgrmtNo, BuffNo, custid, 
BuffBranchNo, Code, DateAcctOpen, DeliveryFlag, 
TranDate, EmpeeNoSale, HeaderWareHouseNo, ItemNo, 
OriginalItem, LineWareHouseNo, Price, Quantity, 
Value, SOA, TCCode, TranType, 
taxtype, agrmttaxtype, taxrate, StockLocn, 
TransRefNo, MoreRewardsNo, PaymentMethod, replacementmarker, 
termstype, type, retstocklocn, delorcoll, 
warrantyitemno, contractno, warrantydets, iswarranty, 
linkeditemIndicator, vardatedel, taxamt, retitemno, 
retwarehouseno, branchno, Dateagrmt, datedel, 
name, cusaddr1, cusaddr2, cusaddr3, 
cuspocode, taxexempt, category, affinity, 
parentitemno, parentlocation, telno, ItemID, ParentItemID, OriginalItemItemID, WarrantyItemID, RetItemID)  --IP - 29/06/11 - CR1212 - RI - #3987

select 
 AcctNo, AgrmtNo, BuffNo, custid, 
BuffBranchNo, Code, DateAcctOpen, DeliveryFlag, 
TranDate, EmpeeNoSale, HeaderWareHouseNo, ItemNo, 
OriginalItem, LineWareHouseNo, Price, Quantity, 
Value, SOA, TCCode, TranType, 
taxtype, agrmttaxtype, taxrate, StockLocn, 
TransRefNo, MoreRewardsNo, PaymentMethod, replacementmarker, 
termstype, type, retstocklocn, delorcoll, 
warrantyitemno, contractno, warrantydets, iswarranty, 
linkeditemIndicator, vardatedel, taxamt, retitemno, 
retwarehouseno, branchno, Dateagrmt, datedel, 
name, cusaddr1, cusaddr2, cusaddr3, 
cuspocode, taxexempt, category, affinity, 
parentitemno, parentlocation, telno, ItemID, isnull(ParentItemID,0) as ParentItemID, isnull(OriginalItemItemID,0) as OriginalItemItemID, isnull(WarrantyItemID,0) as WarrantyItemID, isnull(RetItemID,0) as	RetItemID--IP - 29/06/11 - CR1212 - RI - #3987
 from #OrdersandDels



	-- Return the list of Orders and Deliveries
            SELECT Acctno            = AcctNo,
               AgreementNo           = AgrmtNo,
               BuffNo            = convert(varchar,BuffNo),
               BuffBranchNo            = BuffBranchNo,
               CustomerId            = Custid,
               Code              = code, -- this is going to store cancellation code
               DateDel          = DATEADD(second, DATEDIFF(second, GETDATE(), GETUTCDATE()), isnull(TranDate,getdate())), 
               dateagrmt       = DateAgrmt,
               DeliveryFlag      = DeliveryFlag,
               Salesperson       = EmpeeNoSale,
               HeaderWareHouseNo = HeaderWareHouseNo,
               RetItemNo         = RetItemNo,
               RetWareHouse    = RetWareHouseNo,
               ItemNo            = ItemNo,
			   ItemType			 = ItemType, 
               LineWareHouseNo   = LineWareHouseNo,
               Price             = Price,
               TaxAmt            = isnull(TaxAmt,0),
               Quantity          = Quantity,
               Value             = value,
               SOA               = SOA,
               TCCode            = TCCode,
               TranType          = TranType,
               TaxType           = taxtype,
               AgrmtTaxType      = agrmttaxtype,
               TaxRate           = isnull(taxrate,0),
               StockLocn         = StockLocn,
               DelOrColl         = delorcoll,
               RetStockLocn      = retstocklocn,
			   TransRefNo        = transrefno,
               WarrantyNo    = WarrantyItemNo,
               ContractNo    = ContractNo,
               MoreRewardsNo     = MoreRewardsNo,
               [Payment Method]     = PaymentMethod  /* FR1350 */,
               ReplacementMarker = ReplacementMarker,
               termstype         = termstype,
               AffinityFormatDate = vardatedel,
  	            AssociatedItemIndicator     = LinkedItemIndicator, /* CR557 */
               TaxExempt = isnull(TaxExempt,0),
	            CustomerName = isnull(name,''),		
               cusaddr1 = isnull(cusaddr1,''),
               cusaddr2 = isnull(cusaddr2,''),
               cusaddr3 = isnull(cusaddr3,''),
               cuspocode = isnull(cuspocode, ''),
               Category = ISNULL(category,0),
               Affinity = isnull(Affinity,0),
               ParentItemno = ParentItemno,
               parentlocation = parentlocation,
               originalitem = Originalitem, -- this used as retitemno may overwrite item number
               telno = telno,
		       warrantyrenewcount ,
		       originalStockacctno , 
			   originalcontractno  ,
			   ChargeType,
			   ItemID,																				--IP - 29/06/11 - CR1212 - RI - #3987
               ParentItemID,																		--IP - 29/06/11 - CR1212 - RI - #3987
               OriginalItemItemID,																	--IP - 29/06/11 - CR1212 - RI - #3987
			   WarrantyItemID,																		--IP - 29/06/11 - CR1212 - RI - #3987
			   RetItemID,																			--IP - 29/06/11 - CR1212 - RI - #3987
				ServiceRequestNo,				-- #12948
				Discount,
                CashPrice,
				PromotionId
        FROM   #OrdersandDels
        ORDER BY
            Acctno,
            AgrmtNo,
            ItemNo,
            TransRefNo asc,
            TranDate asc
    --ORDER BY Type desc,
    --         TCCode desc,
    --         buffno,
    --         Acctno,
    --         AgrmtNo,
    --         ItemNo,
    --         TranDate

-- d for del, or for orders, c for cancellation
end
return @return

GO 
