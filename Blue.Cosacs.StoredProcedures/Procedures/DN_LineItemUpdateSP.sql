IF  EXISTS (SELECT 1 
	FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DN_LineItemUpdateSP]') 
	AND type IN (N'P', N'PC'))
DROP PROCEDURE [dbo].[DN_LineItemUpdateSP]

GO

-- 67977 RD 22/02/06 Added to update taxamtbefore and taxamtafter in lineitemaudit table
CREATE PROCEDURE [dbo].[DN_LineItemUpdateSP]
-- ================================================
-- Version:		<002> 
-- ========================================================================
-- Project      : CoSACS .NET
-- File Name    : DN_LineItemUpdateSP.prc
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Line Item Update
-- Author       : ??
-- Date         : ??
--
-- This procedure will update the LineItem  and LineItemAudit tables
-- 
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 21/11/07  jec UAT114 add ParentItemno and ParentLocation to LineItemAudit
-- 12/01/10  ip  UAT963 - Check that the branch exists for the stocklocation of the item 
-- 05/02/10  ip  CR1072 - 3.1.12 - add DelNoteBranch to LineItemAudit
-- 27/04/10	 ip/jec UAT978 - Warranty incorrectly collected
-- 09/03/11  ip   Removed hasstring as column dropped in migration(20110308141100 - lineitemupdate.sql)
-- 27/04/11  jec CR1212 RI Integration - Use ItemID instead of ItemNo
-- 23/05/11  ip  CR1212 RI Integration - Save SalesBrnNo (branch where sale was processed)
-- 30/03/12  jec #9854 Incorrect TAX (Quantity and Value) and incorrect record for Warranty - remove hardcoded warranty check
-- 06/06/12 jec #10229 - Add Express delivery
-- 16/01/13  ip #11534 - LW 75555 - Merged from CoSACS 6.5
-- 27/11/19  aa CR changed the substring logic for itemno which extracts only 8 characters to 18 characters
-- 30/07/20  Zensar Optimization changes : Changed Select * to Top 1 'a' in Exists and Non Exists Statement
--										  
-- ================================================
	-- Add the parameters for the stored procedure here
			@origbr smallint,
			@acctNo char(12), --IP -12/03/08 Livewire: (69281) Changed from varchar
			@agreementNo int,
			@itemID int,	
			@itemSuppText varchar(76),
			@quantity float,
			@stockLocn smallint,
			@price money,
			@orderValue money,
			@dateReqDel datetime,
			@timeReqDel varchar(12),
			@datePlanDel datetime,
			@delNoteBranch smallint,
			@qtyDiff char(1),
			@itemType varchar(1),
			@hasString smallint,
			@notes varchar(200),
			@taxAmount float,
			@parentItemID int,		-- RI 
			@parentStockLocn smallint,
			@isKit smallint,
			@deliveryAddress char(2),
			@ordbuffno int,
			@contractNo varchar(10), --IP 12/03/08 Livewire: (69281) changed from varchar
			@expectedreturndate datetime,
			@empeenochange int,
			@countrycode char(1), --IP 12/03/08 Livewire: (69281)  changed from varchar
			@deliveryarea varchar(8),
			@deliveryprocess char(1),
			@damaged char(1),
			@assembly char(1),
			@source char(15), --IP 12/03/08 Livewire: (69281)  changed from varchar
            @taxrate FLOAT,
            @salesBrnNo smallint,	--IP - 23/05/11 - CR1212 - RI - #3651
            @express CHAR(1),			-- #10229
			@return int OUTPUT

AS
declare 	@branchno smallint,
	@datetrans DateTime, 
	@delivered_quantity smallint,
	@scheduled_qty smallint,
        @category smallint, @custid varchar (50),
	@oldquantity float, @oldvalue money,@oldtaxamt float,  -- 67977 RD
	@datechange datetime, @auditdataperiod smallint, @updated SMALLINT, @rejectedQty smallint
	
Declare @ItemNo VARCHAR(18),@ParentItemNo VARCHAR(18)		-- RI 
	Select @ItemNo= IUPC from stockInfo s where ID=@itemID
	Select @ParentItemNo = ISNULL((SELECT IUPC from stockInfo s where ID=@ParentItemID),'') 

	SET 	@return = 0			--initialise return code
	SET	@datechange = getdate()

	IF @contractNo IS NULL
		SET @contractNo = ''

    if @quantity >0 and @itemno= 'DT' AND EXISTS (SELECT TOP 1 'a' FROM lineitem WHERE acctno = @acctno AND itemID = @itemID AND quantity>=1)
    BEGIN
       set @quantity = 0 -- as we are adding to this later don't want there to be quantity 2 for DT
    END

	--Don't add items which were removed straight away
	IF(@quantity = 0 AND NOT EXISTS(SELECT TOP 1 'a' 
									FROM lineitem 
									WHERE acctno = @acctNo
										AND ItemID = @itemID
										AND stocklocn = @stockLocn
										AND parentlocation = @parentStockLocn
										AND agrmtno = @agreementNo
										AND ParentItemID = @parentItemID
										AND contractno = @contractNo))
	BEGIN
		RETURN
	END

	/* this is a hack to correct a problem with Add To - JJ 11/12/2003 */
	IF( @itemNo = 'ADDCR' OR @itemNo = 'ADDDR' )
		SET	@stockLocn = CONVERT ( INTEGER, LEFT ( @acctno, 3 ) )

	   set @branchno = convert (integer, left (@acctno, 3))
	   select @itemtype =isnull(itemtype,'') from stockitem where itemno =@itemno and stocklocn =@stocklocn 
  set @custid =''
  if right (left (@acctno, 4), 1) ='5' -- is it cash and go account.
       select @custid = custid from custacct where acctno =@acctno	

	SELECT 	@delivered_quantity = isnull(sum (quantity),0) 
	FROM 		delivery 
	WHERE 	acctno =@acctno 
	AND 		itemID =@itemID		--RI
	AND		stocklocn = @stocklocn
	AND 		agrmtno =@agreementNo
	AND		contractno = @contractNo

	SELECT	@scheduled_qty = isnull(sum(quantity),0)
	FROM		schedule 
	WHERE	acctno = @acctno
	AND 		itemID =@itemID		--RI
	AND		agrmtno = @agreementNo

    SELECT @rejectedQty = isnull(sum(lf.quantity),0)
    FROM lineitem l
    INNER JOIN LineItemBookingSchedule lbs on l.ID = lbs.LineItemID
    INNER JOIN LineItemBookingFailures lf on lf.OriginalBookingID = lbs.BookingId
    WHERE l.acctno = @acctNo
    and l.agrmtno = @agreementNo
    AND l.ItemID = @itemID
    AND l.stocklocn = @stockLocn
    AND lbs.BookingId = (select max(lbs2.BookingId)
                            from LineItemBookingSchedule lbs2
                            where lbs2.LineItemID = lbs.LineItemID)


-- Check for an Affinity category which needs to be ordered on FACT 2000
select @category = category FROM StockItem WHERE ItemID = @ItemID AND StockLocn = @StockLocn

declare @trantype char(2), @tccode char(2)

if (@quantity - @delivered_quantity - @rejectedQty <= 0)
BEGIN
	set @trantype = '04'
	set @tccode = '58'
END
ELSE
BEGIN
	set @trantype = '01'
	set @tccode = '61'	
END

if (@itemtype !='N' or (@category = 11 or (@category >=51 and @category <=59))) and @iskit =0 --inserting orders into fact 2000
begin   
    UPDATE facttrans set
        trantype =@trantype, 
        tccode =@tccode, 
        trandate =convert (smallDateTime,getdate()), 
        quantity =case 
                    when @trantype = '04' 
                    then @quantity -@delivered_quantity - @rejectedQty
                    else @quantity -@delivered_quantity
                  end,
        price =@price, 
        taxamt =@taxAmount, 
        value =@quantity * @price 
     WHERE   
        acctno =@acctno and 
        agrmtno =@agreementNo and  
        ItemID = @itemid				--ip - 16/01/13
        --itemno =@itemno and			-- RI does this need to be ID?
        AND stocklocn =@stocklocn
        AND trantype = @trantype
        AND (@quantity -@delivered_quantity - @rejectedQty) != 0
    IF(@@rowcount=0)		--the line item doesn't exist
    BEGIN
   -- get new order number but only if not already existing for this account

	set @datetrans = convert (smallDateTime,getdate())
       	select 	@datetrans =	trandate
	from 	facttrans 
	where 	acctno =@acctno 
	and 	agrmtno =@agreementNo

        IF NOT EXISTS(SELECT TOP 1 'a'
                      FROM lineitem_amend
                      WHERE acctno = @acctNo
                          AND agrmtno = @agreementNo
                          AND ItemID = @itemID
                          AND stocklocn = @stockLocn
                          AND contractno = @contractNo
                          AND ParentItemID = @parentItemID
                          AND parentlocation = @parentStockLocn)
        BEGIN 

            INSERT into facttrans 
               (origbr,
                acctno ,
                agrmtno,
                buffno,
                itemno,
                stocklocn,
                trantype,
                tccode,
                trandate,
                quantity,
                price,
                taxamt,
                value,
                ItemID)
            VALUES
                (0,
                @acctno,
                @agreementNo,
                @ordbuffno, 
                SUBSTRING(@itemno, 1, 18),
                @stocklocn,
                @trantype,
                @tccode,
                @datetrans,
                case 
                    when @trantype = '04'   --Cancel Order
                    then @quantity - @delivered_quantity - @rejectedQty
                    else @quantity - @delivered_quantity 
                end,
                @price,
                @taxAmount,
                @quantity * @price,
                @itemID)
        END 
        ELSE 
        BEGIN
            DECLARE @quantityBefore INT 

            SELECT @quantityBefore = ISNULL(quantity, 0)
            FROM lineitem_amend
            WHERE acctno = @acctNo
                AND agrmtno = @agreementNo
                AND ItemID = @itemID
                AND stocklocn = @stockLocn
                AND contractno = @contractNo
                AND ParentItemID = @parentItemID
                AND parentlocation = @parentStockLocn
                        
            IF (@quantityBefore > @quantity)
            BEGIN 
                SET @trantype = '04'
	            SET @tccode = '58'

                INSERT into facttrans 
                   (origbr,
                    acctno ,
                    agrmtno,
                    buffno,
                    itemno,
                    stocklocn,
                    trantype,
                    tccode,
                    trandate,
                    quantity,
                    price,
                    taxamt,
                    value,
                    ItemID)
                VALUES
                    (0,
                    @acctno,
                    @agreementNo,
                    @ordbuffno, 
                    SUBSTRING(@itemno, 1, 18),
                    @stocklocn,
                    @trantype,
                    @tccode,
                    @datetrans,
                    case 
                        when @rejectedQty > 0 
                        then @quantity - @delivered_quantity - @rejectedQty
                        else -(@quantityBefore - @quantity - @delivered_quantity)
                    end,
                    @price,
                    @taxAmount,
                    @quantity * @price,
                    @itemID)
            END 
            IF (@quantityBefore < @quantity)
            BEGIN 
                SET @trantype = '01'
	            SET @tccode = '61'	
       
       INSERT into facttrans 
       (origbr ,
        acctno  ,
        agrmtno ,
        buffno ,
        itemno ,
        stocklocn,
        trantype,
        tccode  ,
        trandate,
        quantity,
        price ,
        taxamt,
        value,
                    ItemID)
                VALUES
        (0,
        @acctno,
        @agreementNo,
        @ordbuffno, 
                    SUBSTRING(@itemno, 1, 18),
        @stocklocn,
                    @trantype,
                    @tccode,
        @datetrans,
                    @quantity - @quantityBefore - @delivered_quantity,
        @price    ,
        @taxAmount ,
        @quantity * @price,
                    @itemID)
            END
    END   
    END   

    --Delete cancellation of orders from facttrans
    --where the item would have been removed previously then re-added again in the
    --same run (no existing records in Merchandising.CintOrder). 
    -- No need to send a Cancellation. Only send if order changed
    -- in different runs prior to delivery.

    IF(@quantity - @delivered_quantity - @rejectedQty >0)
    BEGIN

        delete f
        from 
            facttrans f
        where
            f.acctno = @acctNo
            and f.agrmtno = @agreementNo
            and f.ItemID = @itemID
            and f.stocklocn = @stockLocn
            and @quantity > 0
            and f.trantype = '04' 
            and not exists (select TOP 1 'a' from Merchandising.CintOrder o
                                where o.PrimaryReference = f.acctno
                                and o.Sku = f.itemno
                                and o.StockLocation = f.stocklocn) 
    END
        
end


if @custid = 'PAID & TAKEN'
  begin
		set @delivered_quantity = @quantity  	
      set @scheduled_qty= 0
end

	IF( (@delivered_quantity + @scheduled_qty) != @quantity )
		SET	@qtyDiff =	N'Y'
	ELSE
		SET	@qtyDiff = 	N'N'
		
if @quantity<0 and @agreementno>0          
begin          
--Fix for discounts only
select @quantity=0 from lineitem li
INNER JOIN StockItem SI ON SI.itemID = @itemID AND si.stocklocn = li.stocklocn		-- RI
where li.acctno=@acctno and li.agrmtno=@agreementno and li.itemID=@parentitemID and  
li.quantity>0 
AND si.category  NOT IN(select code from code where category = 'WAR')		--#9854 
end          

 -- LW 71731 - To allow warranties to be added on Legacy CashandGo Return screen   ------  
 IF @custid = 'PAID & TAKEN' AND @category in (select distinct code from code where category = 'WAR') /*warranty categories*/   
    AND (@contractNo IS NULL OR  @contractNo = '') -- uat(4.3) - 156  
 BEGIN  
  SET @contractNo = '1'   -- To make sure lineitem trigger trig_lineiteminsert not throwing any exception  
 END  
   
 UPDATE  lineitem  
 SET  origbr  = @origbr,    
   acctno  = @acctNo,    
   agrmtno  = @agreementNo,    
   itemno  = @itemNo,    
   itemsupptext = @itemSuppText,    
   quantity  = case when @custid ='PAID & TAKEN' then quantity + @quantity else @quantity end,			-- #17290 - ??
   stocklocn = @stockLocn,    
   price  = @price,    
   ordval  = @orderValue,    
  
   datereqdel = @dateReqDel,    
   timereqdel = @timeReqDel,    
   dateplandel = @datePlanDel,    
   delnotebranch = @delNoteBranch,    
   qtydiff  = @qtyDiff,    
   itemtype  = @itemType,    
   notes  = @notes,    
   taxamt  = @taxAmount,    
   parentItemNo = @parentItemNo,    
   parentLocation = @parentStockLocn,    
   isKit  = @isKit,  
   deliveryAddress = @deliveryAddress,  
   expectedreturndate =  @expectedreturndate,  
   deliveryarea  = @deliveryarea,  
   deliveryprocess = @deliveryprocess,  
   damaged   = @damaged,  
   assemblyrequired = @assembly, 
   taxrate = @taxrate,
   ItemID=@itemID,					-- RI
   ParentItemID=@parentItemID,		-- RI
   Express = @express,				-- #10229
   SalesBrnNo = @salesBrnNo			--IP - 23/05/11 - CR1212 - RI - #3651
 WHERE acctno = @acctNo  
 AND  agrmtno = @agreementNo  
 AND  stocklocn = @stockLocn  
 AND  ItemID = @itemID	-- AND  itemno = @itemNo	-- RI
 AND  contractno = @contractNo  
 AND  ParentItemID = @parentItemID  -- AND  (parentItemNo = @parentItemNo or parentItemNo='') -- RI
 
 set @updated = @@ROWCOUNT
 
 if(@updated != 0)
	 BEGIN
	 
		update delivery
		set parentitemno = @parentitemno, ParentItemID = @parentItemID		-- RI
		where acctno = @acctNo  
			 AND  agrmtno = @agreementNo  
			 AND  stocklocn = @stockLocn  
			 AND  ItemID = @itemID	-- AND  itemno = @itemNo	-- RI
			 AND  contractno = @contractNo  
			 AND  (parentItemNo != @parentItemNo)
			 AND (select category from stockitem where itemID = @itemID and stocklocn = @stocklocn)in	-- RI
					(select code from code where category = 'WAR')
	 
	 END


 IF(@updated=0)  --the warranty is being re-orphaned  
 BEGIN  
 
  UPDATE  lineitem  
 SET  origbr  = @origbr,    
   acctno  = @acctNo,    
   agrmtno  = @agreementNo,    
   itemno  = @itemNo,    
   itemsupptext = @itemSuppText,    
   quantity  = case when @custid ='PAID & TAKEN' then quantity + @quantity else @quantity end,			-- #17290 - ??  
   stocklocn = @stockLocn,    
   price  = @price,    
   ordval  = @orderValue,    
   datereqdel = @dateReqDel,    
   timereqdel = @timeReqDel,    
   dateplandel = @datePlanDel,    
   delnotebranch = @delNoteBranch,    
   qtydiff  = @qtyDiff,    
   itemtype  = @itemType,    
   notes  = @notes,    
   taxamt  = @taxAmount,    
   parentItemNo = @parentItemNo,    
   parentLocation = @parentStockLocn,    
   isKit  = @isKit,  
   deliveryAddress = @deliveryAddress,  
   expectedreturndate =  @expectedreturndate,  
   deliveryarea  = @deliveryarea,  
   deliveryprocess = @deliveryprocess,  
   damaged   = @damaged,  
   assemblyrequired = @assembly,
   ItemID=@itemID,					-- RI 
   ParentItemID=@parentItemID,		-- RI  
   SalesBrnNo = @salesBrnNo			-- IP - 23/05/11 - CR1212 - RI - #3651
 WHERE acctno = @acctNo  
 AND  agrmtno = @agreementNo  
 AND  stocklocn = @stockLocn
 AND  ItemID = @itemID	-- AND  itemno = @itemNo	-- RI  
 AND  contractno = @contractNo  
 AND  (ParentItemID != @parentItemID)		-- AND  (parentItemNo != @parentItemNo)		-- RI 
 AND (select category from stockitem where ItemID = @itemID and stocklocn = @stocklocn)in	-- RI 
		(select code from code where category = 'WAR')
 
 END
  
 IF(@@rowcount=0 AND @updated=0)  --the line item doesn't exist  
 BEGIN  
   
  --IP - 12/01/10 - UAT(963)  
  IF NOT EXISTS(SELECT TOP 1 'a' FROM branch WHERE branchno = @stocklocn)  
  BEGIN  
   RAISERROR (N'Branch %i does not exist in the branch table. Please setup this branch before proceeding.', 16, 1, @stocklocn)  
   return  
  END  
   
  INSERT  
  INTO  lineitem  
    (origbr,  
    acctno,  
    agrmtno,  
    itemno,  
    itemsupptext,  
    quantity,  
    delqty,  
    stocklocn,  
    price,  
    ordval,  
    datereqdel,  
    timereqdel,  
    dateplandel,  
    delnotebranch,  
    qtydiff,  
    itemtype,  
    notes,  
    taxamt,  
    parentItemNo,  
    parentLocation,  
    isKit,  
    deliveryAddress,  
    contractno,  
    expectedreturndate,  
    deliveryarea,  
    deliveryprocess,  
    damaged,  
    assemblyrequired,
    taxrate,
    ItemID,			-- RI
    ParentItemID,	-- RI
    Express,		-- #10229
    SalesBrnNo)		--IP - 23/05/11 - CR1212 - RI - #3651
VALUES	
   (
    @origbr,	
    @acctNo ,   
    @agreementNo ,   
    @itemNo ,   
    @itemSuppText ,   
    @quantity ,   
    (@delivered_quantity + @scheduled_qty),  
    @stockLocn ,   
    @price ,   
    @orderValue ,   
    @dateReqDel ,   
    @timeReqDel ,   
    @datePlanDel ,   
    @delNoteBranch ,   
    @qtyDiff ,   
    @itemType ,   
    @notes ,   
    @taxAmount ,   
    @parentItemNo ,   
    @parentStockLocn ,   
    @isKit,  
    @deliveryAddress,  
    @contractNo,  
    @expectedreturndate,  
    @deliveryarea,  
    @deliveryprocess,  
    @damaged,  
    @assembly,
    @taxrate,
    @itemID,			-- RI
    @parentItemID,		-- RI
    @express,			-- #10229
    @salesBrnNo)		--IP - 23/05/11 - CR1212 - RI - #3651 
 END  

 
 
	/* retrieve the old quantity */
	EXEC DN_LineItemGetOldQtySP 	@acctno = @acctNo,
					@itemID = @itemID,
					@stocklocn = @stockLocn,
					@contractno = @contractNo,
					@agreementno = @agreementNo,
					@parentitemid = @parentitemid,
					@quantity = @oldquantity OUT,
					@return = @return OUT
	SET	@oldquantity = isnull(@oldquantity,0)

	/* retrieve the old value */
	EXEC DN_LineItemGetOldValueSP 	@acctno = @acctNo,
					@itemID = @itemID,
					@stocklocn = @stockLocn,
					@contractno = @contractNo,
					@agreementno = @agreementNo,
					@parentitemid = @parentitemid,
					@value = @oldvalue OUT,
					@taxamt = @oldtaxamt OUT,  -- 67977 RD
					@return = @return OUT
					
	SET	@oldvalue = isnull(@oldvalue,0)
	SET @oldtaxamt = isnull(@oldtaxamt,0)   -- 67977 RD 

	IF( (@quantity != @oldquantity) OR
	    (@orderValue != @oldvalue) OR 
	    (@taxAmount != @oldtaxamt) )   -- 67977 RD
	    
	BEGIN
		/* write an audit record */
		EXEC DN_LineItemAuditUpdateSP 	@acctno = @acctNo,
						@agrmtno = @agreementNo,
						@empeenochange = @empeenochange,
						@itemID = @itemID,
						@stocklocn = @stockLocn,
						@quantitybefore = @oldquantity,
						@quantityafter = @quantity,
						@valuebefore = @oldvalue,
						@valueafter = @orderValue,
						@taxamtbefore = @oldtaxamt,   -- 67977 RD
						@taxamtafter = @taxAmount,    -- 67977 RD
						@datechange = @datechange,
						@contractno = @contractNo,
						@source = @source,
						@parentitemID = @parentItemID,
						@parentStockLocn = @parentStockLocn,	-- jec 21/11/0799999
						@delNoteBranch = @delNoteBranch,		-- ip - 05/02/10 - CR1072 - 3.1.12
						@salesBrnNo = @salesBrnNo,				-- ip - 24/05/11 - CR1212 - RI - #3651
						@return = @return OUT	
	END

	/* delete expired audit records */
	SELECT 	@auditdataperiod = isnull(Convert(smallint, Value),3)
	FROM	CountryMaintenance
	WHERE	codename = 'auditdataperiod'
	AND		CountryCode = @countrycode

	DELETE 
	FROM 	LineItemAudit 
	WHERE 	datechange < dateadd (month,-@auditdataperiod, getdate())
	AND 	acctno = @acctNo


	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END
GO



if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_LineItemUpdateSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_LineItemUpdateSP]
GO

-- 67977 RD 22/02/06 Added to update taxamtbefore and taxamtafter in lineitemaudit table
CREATE PROCEDURE dbo.DN_LineItemUpdateSP
-- ================================================
-- Project      : CoSACS .NET
-- File Name    : DN_LineItemUpdateSP.prc
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Line Item Update
-- Author       : ??
-- Date         : ??
--
-- This procedure will update the LineItem  and LineItemAudit tables
-- 
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 21/11/07  jec UAT114 add ParentItemno and ParentLocation to LineItemAudit
-- 12/01/10  ip  UAT963 - Check that the branch exists for the stocklocation of the item 
-- 05/02/10  ip  CR1072 - 3.1.12 - add DelNoteBranch to LineItemAudit
-- 27/04/10	 ip/jec UAT978 - Warranty incorrectly collected
-- 09/03/11  ip   Removed hasstring as column dropped in migration(20110308141100 - lineitemupdate.sql)
-- 27/04/11  jec CR1212 RI Integration - Use ItemID instead of ItemNo
-- 23/05/11  ip  CR1212 RI Integration - Save SalesBrnNo (branch where sale was processed)
-- 30/03/12  jec #9854 Incorrect TAX (Quantity and Value) and incorrect record for Warranty - remove hardcoded warranty check
-- 06/06/12 jec #10229 - Add Express delivery
-- 16/01/13  ip #11534 - LW 75555 - Merged from CoSACS 6.5
-- 27/11/19  aa CR changed the substring logic for itemno which extracts only 8 characters to 18 characters
-- ================================================
	-- Add the parameters for the stored procedure here
			@origbr smallint,
			@acctNo char(12), --IP -12/03/08 Livewire: (69281) Changed from varchar
			@agreementNo int,
			@itemID int,	
			@itemSuppText varchar(76),
			@quantity float,
			@stockLocn smallint,
			@price money,
			@orderValue money,
			@dateReqDel datetime,
			@timeReqDel varchar(12),
			@datePlanDel datetime,
			@delNoteBranch smallint,
			@qtyDiff char(1),
			@itemType varchar(1),
			@hasString smallint,
			@notes varchar(200),
			@taxAmount float,
			@parentItemID int,		-- RI 
			@parentStockLocn smallint,
			@isKit smallint,
			@deliveryAddress char(2),
			@ordbuffno int,
			@contractNo varchar(10), --IP 12/03/08 Livewire: (69281) changed from varchar
			@expectedreturndate datetime,
			@empeenochange int,
			@countrycode char(1), --IP 12/03/08 Livewire: (69281)  changed from varchar
			@deliveryarea varchar(8),
			@deliveryprocess char(1),
			@damaged char(1),
			@assembly char(1),
			@source char(15), --IP 12/03/08 Livewire: (69281)  changed from varchar
            @taxrate FLOAT,
            @salesBrnNo smallint,	--IP - 23/05/11 - CR1212 - RI - #3651
            @express CHAR(1),			-- #10229
			@return int OUTPUT

AS
declare 	@branchno smallint,
	@datetrans DateTime, 
	@delivered_quantity smallint,
	@scheduled_qty smallint,
        @category smallint, @custid varchar (50),
	@oldquantity float, @oldvalue money,@oldtaxamt float,  -- 67977 RD
	@datechange datetime, @auditdataperiod smallint, @updated SMALLINT, @rejectedQty smallint
	
Declare @ItemNo VARCHAR(18),@ParentItemNo VARCHAR(18)		-- RI 
	Select @ItemNo= IUPC from stockInfo s where ID=@itemID
	Select @ParentItemNo = ISNULL((SELECT IUPC from stockInfo s where ID=@ParentItemID),'') 

	SET 	@return = 0			--initialise return code
	SET	@datechange = getdate()

	IF @contractNo IS NULL
		SET @contractNo = ''

    if @quantity >0 and @itemno= 'DT' AND EXISTS (SELECT * FROM lineitem WHERE acctno = @acctno AND itemID = @itemID AND quantity>=1)
    BEGIN
       set @quantity = 0 -- as we are adding to this later don't want there to be quantity 2 for DT
    END

	--Don't add items which were removed straight away
	IF(@quantity = 0 AND NOT EXISTS(SELECT 'a' 
									FROM lineitem 
									WHERE acctno = @acctNo
										AND ItemID = @itemID
										AND stocklocn = @stockLocn
										AND parentlocation = @parentStockLocn
										AND agrmtno = @agreementNo
										AND ParentItemID = @parentItemID
										AND contractno = @contractNo))
	BEGIN
		RETURN
	END

	/* this is a hack to correct a problem with Add To - JJ 11/12/2003 */
	IF( @itemNo = 'ADDCR' OR @itemNo = 'ADDDR' )
		SET	@stockLocn = CONVERT ( INTEGER, LEFT ( @acctno, 3 ) )

	   set @branchno = convert (integer, left (@acctno, 3))
	   select @itemtype =isnull(itemtype,'') from stockitem where itemno =@itemno and stocklocn =@stocklocn 
  set @custid =''
  if right (left (@acctno, 4), 1) ='5' -- is it cash and go account.
       select @custid = custid from custacct where acctno =@acctno	

	SELECT 	@delivered_quantity = isnull(sum (quantity),0) 
	FROM 		delivery 
	WHERE 	acctno =@acctno 
	AND 		itemID =@itemID		--RI
	AND		stocklocn = @stocklocn
	AND 		agrmtno =@agreementNo
	AND		contractno = @contractNo

	SELECT	@scheduled_qty = isnull(sum(quantity),0)
	FROM		schedule 
	WHERE	acctno = @acctno
	AND 		itemID =@itemID		--RI
	AND		agrmtno = @agreementNo

    SELECT @rejectedQty = isnull(sum(lf.quantity),0)
    FROM lineitem l
    INNER JOIN LineItemBookingSchedule lbs on l.ID = lbs.LineItemID
    INNER JOIN LineItemBookingFailures lf on lf.OriginalBookingID = lbs.BookingId
    WHERE l.acctno = @acctNo
    and l.agrmtno = @agreementNo
    AND l.ItemID = @itemID
    AND l.stocklocn = @stockLocn
    AND lbs.BookingId = (select max(lbs2.BookingId)
                            from LineItemBookingSchedule lbs2
                            where lbs2.LineItemID = lbs.LineItemID)


-- Check for an Affinity category which needs to be ordered on FACT 2000
select @category = category FROM StockItem WHERE ItemID = @ItemID AND StockLocn = @StockLocn

declare @trantype char(2), @tccode char(2)

if (@quantity - @delivered_quantity - @rejectedQty <= 0)
BEGIN
	set @trantype = '04'
	set @tccode = '58'
END
ELSE
BEGIN
	set @trantype = '01'
	set @tccode = '61'	
END

if (@itemtype !='N' or (@category = 11 or (@category >=51 and @category <=59))) and @iskit =0 --inserting orders into fact 2000
begin   
    UPDATE facttrans set
        trantype =@trantype, 
        tccode =@tccode, 
        trandate =convert (smallDateTime,getdate()), 
        quantity =case 
                    when @trantype = '04' 
                    then @quantity -@delivered_quantity - @rejectedQty
                    else @quantity -@delivered_quantity
                  end,
        price =@price, 
        taxamt =@taxAmount, 
        value =@quantity * @price 
     WHERE   
        acctno =@acctno and 
        agrmtno =@agreementNo and  
        ItemID = @itemid				--ip - 16/01/13
        --itemno =@itemno and			-- RI does this need to be ID?
        AND stocklocn =@stocklocn
        AND trantype = @trantype
        AND (@quantity -@delivered_quantity - @rejectedQty) != 0
    IF(@@rowcount=0)		--the line item doesn't exist
    BEGIN
   -- get new order number but only if not already existing for this account

	set @datetrans = convert (smallDateTime,getdate())
       	select 	@datetrans =	trandate
	from 	facttrans 
	where 	acctno =@acctno 
	and 	agrmtno =@agreementNo

        IF NOT EXISTS(SELECT 'a'
                      FROM lineitem_amend
                      WHERE acctno = @acctNo
                          AND agrmtno = @agreementNo
                          AND ItemID = @itemID
                          AND stocklocn = @stockLocn
                          AND contractno = @contractNo
                          AND ParentItemID = @parentItemID
                          AND parentlocation = @parentStockLocn)
        BEGIN 

            INSERT into facttrans 
               (origbr,
                acctno ,
                agrmtno,
                buffno,
                itemno,
                stocklocn,
                trantype,
                tccode,
                trandate,
                quantity,
                price,
                taxamt,
                value,
                ItemID)
            VALUES
                (0,
                @acctno,
                @agreementNo,
                @ordbuffno, 
                SUBSTRING(@itemno, 1, 18),
                @stocklocn,
                @trantype,
                @tccode,
                @datetrans,
                case 
                    when @trantype = '04'   --Cancel Order
                    then @quantity - @delivered_quantity - @rejectedQty
                    else @quantity - @delivered_quantity 
                end,
                @price,
                @taxAmount,
                @quantity * @price,
                @itemID)
        END 
        ELSE 
        BEGIN
            DECLARE @quantityBefore INT 

            SELECT @quantityBefore = ISNULL(quantity, 0)
            FROM lineitem_amend
            WHERE acctno = @acctNo
                AND agrmtno = @agreementNo
                AND ItemID = @itemID
                AND stocklocn = @stockLocn
                AND contractno = @contractNo
                AND ParentItemID = @parentItemID
                AND parentlocation = @parentStockLocn
                        
            IF (@quantityBefore > @quantity)
            BEGIN 
                SET @trantype = '04'
	            SET @tccode = '58'

                INSERT into facttrans 
                   (origbr,
                    acctno ,
                    agrmtno,
                    buffno,
                    itemno,
                    stocklocn,
                    trantype,
                    tccode,
                    trandate,
                    quantity,
                    price,
                    taxamt,
                    value,
                    ItemID)
                VALUES
                    (0,
                    @acctno,
                    @agreementNo,
                    @ordbuffno, 
                    SUBSTRING(@itemno, 1, 18),
                    @stocklocn,
                    @trantype,
                    @tccode,
                    @datetrans,
                    case 
                        when @rejectedQty > 0 
                        then @quantity - @delivered_quantity - @rejectedQty
                        else -(@quantityBefore - @quantity - @delivered_quantity)
                    end,
                    @price,
                    @taxAmount,
                    @quantity * @price,
                    @itemID)
            END 
            IF (@quantityBefore < @quantity)
            BEGIN 
                SET @trantype = '01'
	            SET @tccode = '61'	
       
       INSERT into facttrans 
       (origbr ,
        acctno  ,
        agrmtno ,
        buffno ,
        itemno ,
        stocklocn,
        trantype,
        tccode  ,
        trandate,
        quantity,
        price ,
        taxamt,
        value,
                    ItemID)
                VALUES
        (0,
        @acctno,
        @agreementNo,
        @ordbuffno, 
                    SUBSTRING(@itemno, 1, 18),
        @stocklocn,
                    @trantype,
                    @tccode,
        @datetrans,
                    @quantity - @quantityBefore - @delivered_quantity,
        @price    ,
        @taxAmount ,
        @quantity * @price,
                    @itemID)
            END
    END   
    END   

    --Delete cancellation of orders from facttrans
    --where the item would have been removed previously then re-added again in the
    --same run (no existing records in Merchandising.CintOrder). 
    -- No need to send a Cancellation. Only send if order changed
    -- in different runs prior to delivery.

    IF(@quantity - @delivered_quantity - @rejectedQty >0)
    BEGIN

        delete f
        from 
            facttrans f
        where
            f.acctno = @acctNo
            and f.agrmtno = @agreementNo
            and f.ItemID = @itemID
            and f.stocklocn = @stockLocn
            and @quantity > 0
            and f.trantype = '04' 
            and not exists (select 1 from Merchandising.CintOrder o
                                where o.PrimaryReference = f.acctno
                                and o.Sku = f.itemno
                                and o.StockLocation = f.stocklocn) 
    END
        
end


if @custid = 'PAID & TAKEN'
  begin
		set @delivered_quantity = @quantity  	
      set @scheduled_qty= 0
end

	IF( (@delivered_quantity + @scheduled_qty) != @quantity )
		SET	@qtyDiff =	N'Y'
	ELSE
		SET	@qtyDiff = 	N'N'
		
if @quantity<0 and @agreementno>0          
begin          
--Fix for discounts only
select @quantity=0 from lineitem li
INNER JOIN StockItem SI ON SI.itemID = @itemID AND si.stocklocn = li.stocklocn		-- RI
where li.acctno=@acctno and li.agrmtno=@agreementno and li.itemID=@parentitemID and  
li.quantity>0 
AND si.category  NOT IN(select code from code where category = 'WAR')		--#9854 
end          

 -- LW 71731 - To allow warranties to be added on Legacy CashandGo Return screen   ------  
 IF @custid = 'PAID & TAKEN' AND @category in (select distinct code from code where category = 'WAR') /*warranty categories*/   
    AND (@contractNo IS NULL OR  @contractNo = '') -- uat(4.3) - 156  
 BEGIN  
  SET @contractNo = '1'   -- To make sure lineitem trigger trig_lineiteminsert not throwing any exception  
 END  
   
 UPDATE  lineitem  
 SET  origbr  = @origbr,    
   acctno  = @acctNo,    
   agrmtno  = @agreementNo,    
   itemno  = @itemNo,    
   itemsupptext = @itemSuppText,    
   quantity  = case when @custid ='PAID & TAKEN' then quantity + @quantity else @quantity end,			-- #17290 - ??
   stocklocn = @stockLocn,    
   price  = @price,    
   ordval  = @orderValue,    
  
   datereqdel = @dateReqDel,    
   timereqdel = @timeReqDel,    
   dateplandel = @datePlanDel,    
   delnotebranch = @delNoteBranch,    
   qtydiff  = @qtyDiff,    
   itemtype  = @itemType,    
   notes  = @notes,    
   taxamt  = @taxAmount,    
   parentItemNo = @parentItemNo,    
   parentLocation = @parentStockLocn,    
   isKit  = @isKit,  
   deliveryAddress = @deliveryAddress,  
   expectedreturndate =  @expectedreturndate,  
   deliveryarea  = @deliveryarea,  
   deliveryprocess = @deliveryprocess,  
   damaged   = @damaged,  
   assemblyrequired = @assembly, 
   taxrate = @taxrate,
   ItemID=@itemID,					-- RI
   ParentItemID=@parentItemID,		-- RI
   Express = @express,				-- #10229
   SalesBrnNo = @salesBrnNo			--IP - 23/05/11 - CR1212 - RI - #3651
 WHERE acctno = @acctNo  
 AND  agrmtno = @agreementNo  
 AND  stocklocn = @stockLocn  
 AND  ItemID = @itemID	-- AND  itemno = @itemNo	-- RI
 AND  contractno = @contractNo  
 AND  ParentItemID = @parentItemID  -- AND  (parentItemNo = @parentItemNo or parentItemNo='') -- RI
 
 set @updated = @@ROWCOUNT
 
 if(@updated != 0)
	 BEGIN
	 
		update delivery
		set parentitemno = @parentitemno, ParentItemID = @parentItemID		-- RI
		where acctno = @acctNo  
			 AND  agrmtno = @agreementNo  
			 AND  stocklocn = @stockLocn  
			 AND  ItemID = @itemID	-- AND  itemno = @itemNo	-- RI
			 AND  contractno = @contractNo  
			 AND  (parentItemNo != @parentItemNo)
			 AND (select category from stockitem where itemID = @itemID and stocklocn = @stocklocn)in	-- RI
					(select code from code where category = 'WAR')
	 
	 END


 IF(@updated=0)  --the warranty is being re-orphaned  
 BEGIN  
 
  UPDATE  lineitem  
 SET  origbr  = @origbr,    
   acctno  = @acctNo,    
   agrmtno  = @agreementNo,    
   itemno  = @itemNo,    
   itemsupptext = @itemSuppText,    
   quantity  = case when @custid ='PAID & TAKEN' then quantity + @quantity else @quantity end,			-- #17290 - ??  
   stocklocn = @stockLocn,    
   price  = @price,    
   ordval  = @orderValue,    
   datereqdel = @dateReqDel,    
   timereqdel = @timeReqDel,    
   dateplandel = @datePlanDel,    
   delnotebranch = @delNoteBranch,    
   qtydiff  = @qtyDiff,    
   itemtype  = @itemType,    
   notes  = @notes,    
   taxamt  = @taxAmount,    
   parentItemNo = @parentItemNo,    
   parentLocation = @parentStockLocn,    
   isKit  = @isKit,  
   deliveryAddress = @deliveryAddress,  
   expectedreturndate =  @expectedreturndate,  
   deliveryarea  = @deliveryarea,  
   deliveryprocess = @deliveryprocess,  
   damaged   = @damaged,  
   assemblyrequired = @assembly,
   ItemID=@itemID,					-- RI 
   ParentItemID=@parentItemID,		-- RI  
   SalesBrnNo = @salesBrnNo			-- IP - 23/05/11 - CR1212 - RI - #3651
 WHERE acctno = @acctNo  
 AND  agrmtno = @agreementNo  
 AND  stocklocn = @stockLocn
 AND  ItemID = @itemID	-- AND  itemno = @itemNo	-- RI  
 AND  contractno = @contractNo  
 AND  (ParentItemID != @parentItemID)		-- AND  (parentItemNo != @parentItemNo)		-- RI 
 AND (select category from stockitem where ItemID = @itemID and stocklocn = @stocklocn)in	-- RI 
		(select code from code where category = 'WAR')
 
 END
  
 IF(@@rowcount=0 AND @updated=0)  --the line item doesn't exist  
 BEGIN  
   
  --IP - 12/01/10 - UAT(963)  
  IF NOT EXISTS(SELECT * FROM branch WHERE branchno = @stocklocn)  
  BEGIN  
   RAISERROR (N'Branch %i does not exist in the branch table. Please setup this branch before proceeding.', 16, 1, @stocklocn)  
   return  
  END  
   
  INSERT  
  INTO  lineitem  
    (origbr,  
    acctno,  
    agrmtno,  
    itemno,  
    itemsupptext,  
    quantity,  
    delqty,  
    stocklocn,  
    price,  
    ordval,  
    datereqdel,  
    timereqdel,  
    dateplandel,  
    delnotebranch,  
    qtydiff,  
    itemtype,  
    notes,  
    taxamt,  
    parentItemNo,  
    parentLocation,  
    isKit,  
    deliveryAddress,  
    contractno,  
    expectedreturndate,  
    deliveryarea,  
    deliveryprocess,  
    damaged,  
    assemblyrequired,
    taxrate,
    ItemID,			-- RI
    ParentItemID,	-- RI
    Express,		-- #10229
    SalesBrnNo)		--IP - 23/05/11 - CR1212 - RI - #3651
VALUES	
   (
    @origbr,	
    @acctNo ,   
    @agreementNo ,   
    @itemNo ,   
    @itemSuppText ,   
    @quantity ,   
    (@delivered_quantity + @scheduled_qty),  
    @stockLocn ,   
    @price ,   
    @orderValue ,   
    @dateReqDel ,   
    @timeReqDel ,   
    @datePlanDel ,   
    @delNoteBranch ,   
    @qtyDiff ,   
    @itemType ,   
    @notes ,   
    @taxAmount ,   
    @parentItemNo ,   
    @parentStockLocn ,   
    @isKit,  
    @deliveryAddress,  
    @contractNo,  
    @expectedreturndate,  
    @deliveryarea,  
    @deliveryprocess,  
    @damaged,  
    @assembly,
    @taxrate,
    @itemID,			-- RI
    @parentItemID,		-- RI
    @express,			-- #10229
    @salesBrnNo)		--IP - 23/05/11 - CR1212 - RI - #3651 
 END  

 
 
	/* retrieve the old quantity */
	EXEC DN_LineItemGetOldQtySP 	@acctno = @acctNo,
					@itemID = @itemID,
					@stocklocn = @stockLocn,
					@contractno = @contractNo,
					@agreementno = @agreementNo,
					@parentitemid = @parentitemid,
					@quantity = @oldquantity OUT,
					@return = @return OUT
	SET	@oldquantity = isnull(@oldquantity,0)

	/* retrieve the old value */
	EXEC DN_LineItemGetOldValueSP 	@acctno = @acctNo,
					@itemID = @itemID,
					@stocklocn = @stockLocn,
					@contractno = @contractNo,
					@agreementno = @agreementNo,
					@parentitemid = @parentitemid,
					@value = @oldvalue OUT,
					@taxamt = @oldtaxamt OUT,  -- 67977 RD
					@return = @return OUT
					
	SET	@oldvalue = isnull(@oldvalue,0)
	SET @oldtaxamt = isnull(@oldtaxamt,0)   -- 67977 RD 

	IF( (@quantity != @oldquantity) OR
	    (@orderValue != @oldvalue) OR 
	    (@taxAmount != @oldtaxamt) )   -- 67977 RD
	    
	BEGIN
		/* write an audit record */
		EXEC DN_LineItemAuditUpdateSP 	@acctno = @acctNo,
						@agrmtno = @agreementNo,
						@empeenochange = @empeenochange,
						@itemID = @itemID,
						@stocklocn = @stockLocn,
						@quantitybefore = @oldquantity,
						@quantityafter = @quantity,
						@valuebefore = @oldvalue,
						@valueafter = @orderValue,
						@taxamtbefore = @oldtaxamt,   -- 67977 RD
						@taxamtafter = @taxAmount,    -- 67977 RD
						@datechange = @datechange,
						@contractno = @contractNo,
						@source = @source,
						@parentitemID = @parentItemID,
						@parentStockLocn = @parentStockLocn,	-- jec 21/11/0799999
						@delNoteBranch = @delNoteBranch,		-- ip - 05/02/10 - CR1072 - 3.1.12
						@salesBrnNo = @salesBrnNo,				-- ip - 24/05/11 - CR1212 - RI - #3651
						@return = @return OUT	
	END

	/* delete expired audit records */
	SELECT 	@auditdataperiod = isnull(Convert(smallint, Value),3)
	FROM	CountryMaintenance
	WHERE	codename = 'auditdataperiod'
	AND		CountryCode = @countrycode

	DELETE 
	FROM 	LineItemAudit 
	WHERE 	datechange < dateadd (month,-@auditdataperiod, getdate())
	AND 	acctno = @acctNo


	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

-- End End End End End End End End End End End End End End End End End End End End End End End End End End 