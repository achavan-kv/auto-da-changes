SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO

if exists (select * from dbo.sysobjects
where id = object_id('[dbo].[DN_ItemGetDetailsSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
    drop procedure [dbo].[DN_ItemGetDetailsSP]
GO
CREATE PROCEDURE [dbo].[DN_ItemGetDetailsSP]
-- ================================================
-- Project      : CoSACS .NET
-- File Name    : DN_ItemGetDetailsSP.prc
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Item Get Details 
-- Date         : ??
--
-- This procedure will retrieve the Item details 
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 21/04/11  jec CR1212 - pass item Identity & return Colour and Style
-- 16/05/11  ip  RI Integration changes - CR1212 - #3627 - Changed joins to use Kit.ComponentID
-- 16/06/11 jec CR1212 - #3923 - Return RepossessedItem flag
-- 27/07/11  ip  RI - #4415 - Return Class and SubClass
-- 16/06/11 jec #4417 $UAT25[RI Integration] - Cash Prices and Duty Free prices
-- 09/09/11  ip #8136 - UAT46 - Kit Component prices should be retrieved from the KitProduct table.
-- 19/09/11  ip RI - #8218 - CR8201 - Agreement print out changes - description needs to be: descr+brand+vendor style long. Returning brand
-- 03/05/12  ip #10025 - LW74941 - If Interface Type is not FACT only then retrieve the values for a Kit Component from KitProduct table, otherwise
--			    from Stockitem.	
-- 09/07/13  ip #13716 - CR12949 - Return if item is a Ready Assist item.
-- 21/03/13  ip #17883 - changed isFree to WarrantyType to return (F = Free, E = Extended, I = Instant Replacement or '')
-- 18/06/14  ip #18604 - CR15594 - Ready Assist - Ready Assist items are in category 11, therefore no need to check categories set up in contracts screen.
-- ================================================
-- Add the parameters for the stored procedure here
        @itemNo VARCHAR(18) OUT,
        @location smallint,
		@branch smallint,
        @accounttype VARCHAR(3),
        @country VARCHAR(3),
        @dutyfree smallint,
        @taxExempt smallint,
        @agrmtno int,
        @acctno varchar(12),
        @stock float OUT,
        @stockdamage float OUT,
        @itemdescr1 VARCHAR(32) OUT,
        @itemdescr2 VARCHAR(40) OUT,
        @suppliercode VARCHAR(40) OUT, --CR 1024 (NM 23/04/2009)
        @UnitPrice float OUT,
        @CashPrice float OUT,
		@CostPrice money OUT,
        @HPPrice float OUT,
        @DutyFreePrice float OUT,
        @valueControlled int OUT,
        @kit int OUT,
        @isStock int  OUT,
        @discount int OUT,
        @isWarranty int OUT,
        @taxrate float OUT,
        @isAffinity int OUT,
        @isStampDuty smallint OUT,
        @isFreeGift smallint OUT,
        @promobranch smallint = 0,
        @isComponent smallint,
        @qtyonorder float OUT,
        @leadtime smallint OUT,
        @delnotebranch smallint OUT,
        @assemblyrequired char(1) OUT,
        @productcategory VARCHAR(4) OUT,
        @deleted char(1) OUT,
        @itemCategory smallint OUT,
        @sparePartsCategory VARCHAR(4) OUT, -- Required to identify spare parts for Service Request
        @refcode VARCHAR(3) OUT, --IP - 28/01/10 - LW 72136
        @isInstallation bit OUT, --IP - 24/02/11 - Sprint 5.10 - #3130
        @itemID INT,				-- Unique Identifier CR1212 jec 21/04/11
        @colourName VARCHAR(12) out,	-- CR1212 jec 21/04/11
        @style VARCHAR(25) out,			-- CR1212 jec 21/04/11
        @repoItem BIT out,					-- jec 16/06/11
        @class varchar(3) out,		--IP - 27/07/11 - RI - #4415
        @subClass varchar(5) out,	--IP - 27/07/11 - RI - #4415
        @brand VARCHAR(25) out,		--IP - 19/09/11 - RI - #8218 - CR8201
		@readyAssist bit out,		--#13716 - CR12949
		@warrantyType char out,		--#17883	--#15888
        @isAssembly bit OUT,
        @isAnnualServiceContract bit OUT,
        @isGenericService bit OUT,
		@Addisionaltaxrate float OUT, --BCX : This is used for LUX tax for curacao 
        @return int OUTPUT
AS
    /* AA- 09/12/03 63892- promotional price change */
    /* AA- 17/08/04 Jamaica go-live error*/
	DECLARE @countrycode varchar(2)
    SET @return = 0            --initialise return code

    declare @category smallint, @DateToday datetime, @itemtype varchar(1), @AgTaxType char(1),
            @TaxType char(1), @priceaccounttype varchar (1), @pricefromlocn varchar(10),
            @interfaceType varchar(10),		--IP - 03/05/12 - #10025 - LW74941
			@PriceBranchForPromos smallint
            
    select @pricefromlocn = value from CountryMaintenance where CodeName =  'pricefromlocn' 
    select @interfaceType = value from CountryMaintenance where CodeName = 'RIInterfaceOptions'	--IP - 03/05/12 - #10025 - LW74941

	set	@PriceBranchForPromos = CASE WHEN @pricefromlocn = 'false' THEN @branch ELSE @location END

    if @AccountType = 'C' or @accounttype = 'S'
        set @priceaccounttype = 'C'
    else
        set @priceaccounttype = 'H'

    SET @DateToday = getdate()
    SET @UnitPrice = 0
    SET @isStampDuty = 0
    SET @delnotebranch = 0


    SELECT  @itemNo = IUPC,
			@stock = stockfactavailable,
            @stockdamage = stockdamage,
            @itemdescr1 = itemdescr1,
            @suppliercode = Case  --CR 1024 (NM 23/04/2009)	
								When supplierCode is NOT NULL and supplierCode != '' Then supplierCode
								When supplier is NOT NULL and supplier != '' Then supplier				 
								Else ''
							End,
            @itemdescr2 = itemdescr2,
            @CashPrice = unitpricecash,
			@CostPrice = CostPrice,
            @HPPrice = unitpricehp,
            @DutyFreePrice = unitpricedutyfree,
            @category = category,
            @itemtype = itemtype,
            @taxrate = taxrate,
            @refcode = refcode,
            @leadtime = leadtime,
            @assemblyrequired = isnull(assemblyrequired,'N'),
			@itemID = ItemID,
            @deleted = deleted,
            @colourName= colourName,@style=isnull(VendorLongStyle,''),
            @repoItem=repossesseditem,				-- RI jec 17/06/11
            @class = Class,							--IP - 27/07/11 - RI - #4415
            @subClass = SubClass,					--IP - 27/07/11 - RI - #4415
            @brand = rtrim(ltrim(isnull(brand,''))),		    --IP - 19/09/11 - RI - #8218 - CR8201
			@warrantyType = WarrantyType			--#17883 --#15888
	FROM    stockitem
    WHERE   ItemID = @itemID -- CR1212
    AND     stocklocn = @location
    

	if(@pricefromlocn = 'false')
	BEGIN
		select @CashPrice = unitpricecash,
			@CostPrice = CostPrice,
            @HPPrice = unitpricehp
        FROM    stockitem
		WHERE   ID = @itemID
		 AND     stocklocn = @branch
	END




    SELECT  @qtyonorder = quantityonorder
    FROM PurchaseOrderOutstanding
    WHERE	ItemID = @itemID
			AND stocklocn = @location

    SET @qtyonorder = ISNULL(@qtyonorder, 0)
    SET @return = @@error


    --if this item appears in the sundchgtyp table then we need to get the price
    --from there rather than the stock item table
    SELECT  @CashPrice = amount,
            @HPPrice = amount,
            @DutyFreePrice = amount
    FROM    sundchgtyp
    WHERE   ItemID = @itemId
    AND     accttype = @priceaccounttype

    IF (@@rowcount>0) SET @isStampDuty = 1


    IF(@return = 0)
    BEGIN
        -- Price type (Cash, HP or Duty Free)
        IF (@priceaccounttype IN ('C','S'))    /* Acct Type Translation DSR 29/9/03 */
            SET     @UnitPrice = @CashPrice 
        ELSE
            SET     @UnitPrice = @HPPrice

        IF (@dutyfree=1)
            SET     @UnitPrice = @DutyFreePrice

        -- Stock type
        IF (@itemtype = 'S')
            SET     @isStock = 1
        ELSE
            SET     @isStock = 0

        -- Discount
        IF (@category in ( select code from code where category = 'PCDIS'))
            SET     @discount = 1
        ELSE
            SET     @discount = 0

        -- Warranty
        IF (@category in (select distinct code from code where category = 'WAR'))
            SET     @isWarranty = 1
		ELSE
            SET     @isWarranty = 0

        -- Affinity
        IF (@category = 11 or (@category >=51 and @category <=59)) -- AA 18/08/04
            SET     @isAffinity = 1
        ELSE
            SET     @isAffinity = 0

        -- Free Gift
        --IF (@category in (14, 24, 84))
        IF (@category in (select CONVERT(SMALLINT, code) FROM code WHERE category = 'FGC')) --IP - 09/02/10 - CR1048 (Ref:3.1.3) Merged - Malaysia Enhancements (CR1072)
            SET     @isFreeGift = 1
        ELSE
            SET     @isFreeGift = 0
           
            
        --Installation
		IF EXISTS(select * from code where category = 'INST' and @itemNo = code)
		BEGIN
			SET @isInstallation = 1
		END 
		ELSE
			SET @isInstallation = 0

        --Assembly Cost
        IF EXISTS(select * from code where category = 'ASSY' and @itemNo = code)
		BEGIN
			SET @isAssembly = 1
		END 
		ELSE
			SET @isAssembly = 0

        --Annual Service Contract
        IF EXISTS(select * from code where category = 'ANNSERVCONT' and @itemNo = code)
		BEGIN
			SET @isAnnualServiceContract = 1
		END 
		ELSE
			SET @isAnnualServiceContract = 0

        --Generic Service
        IF EXISTS(select * from code where category = 'GENSERVICE' and @itemNo = code)
		BEGIN
			SET @isGenericService = 1
		END 
		ELSE
			SET @isGenericService = 0

    END


    IF (@return = 0)
    BEGIN
    IF (@dutyfree=0)  -- 68045 RD 09/03/2006  Only apply promotion price if not duty free
        IF (@isComponent = 0)
        BEGIN


            SELECT  @UnitPrice = unitprice
            FROM    promoprice
            WHERE   itemId = @itemID
            AND     stocklocn = @PriceBranchForPromos
            AND     ((hporcash = 'H' AND @priceaccountType NOT IN ('C','S'))
                     OR (hporcash = 'C' AND @priceaccountType IN ('C','S')))    /* Acct Type Translation DSR 29/9/03 */
             AND    (	CONVERT(DATE, fromdate) <= CONVERT(DATE, @DateToday)
						AND CONVERT(DATE, todate) >= CONVERT(DATE, @DateToday)
						AND CAST(CAST(CONVERT(DATE,@DateToday) AS VARCHAR(10)) + ' ' + CONVERT(VARCHAR(8),fromdate, 108) AS DATETIME) <= @DateToday
						AND CAST(CAST(CONVERT(DATE,@DateToday) AS VARCHAR(10)) + ' ' + CONVERT(VARCHAR(8),todate, 108) AS DATETIME) >= @DateToday
					)
					  

            SELECT  @CashPrice = isnull(unitprice, @CashPrice)
            FROM    promoprice
            WHERE   itemId = @itemID
            AND     stocklocn = @PriceBranchForPromos
            AND     hporcash = 'C'
			AND    (	CONVERT(DATE, fromdate) <= CONVERT(DATE, @DateToday)
						AND CONVERT(DATE, todate) >= CONVERT(DATE, @DateToday)
						AND CAST(CAST(CONVERT(DATE,@DateToday) AS VARCHAR(10)) + ' ' + CONVERT(VARCHAR(8),fromdate, 108) AS DATETIME) <= @DateToday
						AND CAST(CAST(CONVERT(DATE,@DateToday) AS VARCHAR(10)) + ' ' + CONVERT(VARCHAR(8),todate, 108) AS DATETIME) >= @DateToday
					) 

            SELECT  @HPPrice = isnull(unitprice, @HPPrice)
            FROM    promoprice
            WHERE   itemId = @itemID
            AND     stocklocn = @PriceBranchForPromos
            AND     hporcash = 'H'
            AND    (	CONVERT(DATE, fromdate) <= CONVERT(DATE, @DateToday)
						AND CONVERT(DATE, todate) >= CONVERT(DATE, @DateToday)
						AND CAST(CAST(CONVERT(DATE,@DateToday) AS VARCHAR(10)) + ' ' + CONVERT(VARCHAR(8),fromdate, 108) AS DATETIME) <= @DateToday
						AND CAST(CAST(CONVERT(DATE,@DateToday) AS VARCHAR(10)) + ' ' + CONVERT(VARCHAR(8),todate, 108) AS DATETIME) >= @DateToday
					)  
        
		
		END

        SET    @return = @@error
    END


    IF (@return = 0)
    BEGIN
        IF EXISTS (SELECT 1 FROM kitproduct WHERE itemId = @itemID)
        BEGIN
            -- KIT COMPONENTS
            -- If it's a kit then we need to calculate the price based on the components

            -- FR67663 Need to return all price types, not just the unit price.
            -- FR67663 Also fix kit component promo prices - promo prices should be ignored
            -- when a component is sold as part of a kit. The promo price only applies to the
            -- parent of the kit and therefore only affects the discount line for the kit.
 IF(@pricefromlocn = 'false')
            BEGIN
				SELECT  @CashPrice		= @CashPrice + SUM(S.unitpricecash * K.componentqty),
						@HPPrice		= @HPPrice + SUM(S.unitpricehp * K.componentqty),
						@DutyFreePrice	= @DutyFreePrice + SUM(S.unitpricedutyfree * K.componentqty)
				FROM    kitproduct K, stockitem S
				WHERE   K.ItemID = @itemID
				--AND     S.itemno = K.componentno	--IP - 16/05/11 - CR1212 - #3627
				AND		s.ItemID = K.ComponentID
				AND     S.stocklocn = @branch
			END
			ELSE
			BEGIN
            SELECT  @CashPrice		= @CashPrice + SUM(S.unitpricecash * K.componentqty),
                    @HPPrice		= @HPPrice + SUM(S.unitpricehp * K.componentqty),
                    @DutyFreePrice	= @DutyFreePrice + SUM(S.unitpricedutyfree * K.componentqty)
            FROM    kitproduct K, stockitem S
            WHERE   K.ItemID = @itemID
            --AND     S.itemno = K.componentno
            AND		s.ItemID = K.ComponentID		--IP - 16/05/11 - CR1212 - #3627
            AND     S.stocklocn = @location
            END
           

            IF (@dutyfree=1)
            BEGIN
                SET @UnitPrice = @DutyFreePrice
            END
            ELSE
            BEGIN
                IF (@priceaccounttype IN ('C','S'))    /* Acct Type Translation DSR 29/9/03 */
                    SET  @UnitPrice = @CashPrice
                ELSE
                    SET  @UnitPrice = @HPPrice
            END

            SET @kit = 1
        END
        ELSE
        BEGIN
            SET @kit = 0
        END

        SET    @return = @@error
    END


    IF(@return = 0)
    BEGIN
        IF (@UnitPrice = 0 and @category not in (14,24,84))        --free gift categories
        BEGIN
            -- only set to value controlled if it's not in the sundchgtyp table
            -- and it's not the deferred terms
            IF (@isStampDuty = 0 and @itemNo != 'DT')
                SET @valueControlled = 1
            ELSE
                SET @valueControlled = 0
        END
        ELSE
        BEGIN
            SET @valueControlled = 0
        END

        SET @return = @@error
    END

	--IP - 09/09/11 - RI - #8136 - UAT46
	IF(@return = 0)
	BEGIN
		IF (@isComponent = 1 and @interfaceType != 'FACT')	--IP - 03/05/12 - #10025 - LW74941	
		BEGIN
				 SELECT @CashPrice		= K.ComponentPrice,
						@HPPrice		= K.ComponentPrice,
						@DutyFreePrice	= K.ComponentPrice,
						@UnitPrice		= K.ComponentPrice
				FROM    kitproduct K
				WHERE   K.ComponentID = @itemID
		END
	END

    -- At this point we have all the relevant price values for the item, but we haven't
    -- taken into account whether that price is inclusive or exclusive of tax and
    -- whether it should be displayed and stored inclusive or exclusive of tax 

    IF (@return = 0)
    BEGIN
        SELECT  @AgTaxType = agrmttaxtype,
                @TaxType = taxtype
        FROM    country
        WHERE   countrycode = @country

        IF(@AgTaxType = 'E' and @TaxType = 'I')
        BEGIN
            --need to remove tax
            SET @UnitPrice = ROUND(@UnitPrice / (1 + (@taxrate/100)),2)
            SET @CashPrice = ROUND(@CashPrice / (1 + (@taxrate/100)),2)
            SET @HPPrice = ROUND(@HPPrice / (1 + (@taxrate/100)),2)
            SET @DutyFreePrice = ROUND(@DutyFreePrice / (1 + (@taxrate/100)),2)
        END

        IF(@AgTaxType = 'I' and @TaxType = 'E' and @taxExempt = 0)
        BEGIN
            --need to add tax on
            SET @UnitPrice = ROUND(@UnitPrice * (1 + (@taxrate/100)),2)
            SET @CashPrice = ROUND(@CashPrice * (1 + (@taxrate/100)),2)
            SET @HPPrice = ROUND(@HPPrice * (1 + (@taxrate/100)),2)
            --SET @DutyFreePrice = ROUND(@DutyFreePrice * (1 + (@taxrate/100)),2)
            SET @DutyFreePrice = ROUND(@DutyFreePrice,2)				-- do not add tax to dutyfree jec 09/08/11 
        END

        IF(@AgTaxType = 'I' and @TaxType = 'I' and @taxExempt = 1)
        BEGIN
            --need to remove tax
            SET @UnitPrice = ROUND(@UnitPrice / (1 + (@taxrate/100)),2)
            SET @CashPrice = ROUND(@CashPrice / (1 + (@taxrate/100)),2)
            SET @HPPrice = ROUND(@HPPrice / (1 + (@taxrate/100)),2)
            SET @DutyFreePrice = ROUND(@DutyFreePrice / (1 + (@taxrate/100)),2)
        END
        SET @return = @@error
    END
    
	IF (@return = 0)
	BEGIN
		SELECT	@delnotebranch = ISNULL(reference, 0)
		FROM	stockitem s, code c
		WHERE	s.ItemID = @itemID
		AND     s.stocklocn = @location
		AND		s.category = c.code
		AND 	c.category IN ('PCE','PCF','PCW')

		SET @return = @@error
	END

    IF (@return = 0)
	BEGIN
		SELECT	@productcategory = c.category
		FROM	stockitem s, code c
		WHERE	s.ItemID = @itemID
		AND     s.stocklocn = @location
		AND		s.category = c.code
		AND 	c.category IN ('PCE','PCF','PCW')

		SET @return = @@error
	END

    IF (@return = 0)
	BEGIN
		SELECT	@sparePartsCategory = c.category
		FROM	stockitem s, code c
		WHERE	s.ItemID = @itemID
		AND     s.stocklocn = @location
		AND		s.category = c.code
		AND 	c.category IN ('PCO')

		SET @return = @@error
	END

	IF (@return = 0)
	BEGIN
	    IF (@discount = 1)
	    BEGIN
		    SET @productcategory = @category
		END    

		SET @return = @@error
	END

	IF(@delnotebranch = 0 AND @return = 0)
	BEGIN
		SELECT 	@delnotebranch = ISNULL(CONVERT(smallint, value),0)
		FROM	countrymaintenance
		WHERE	codename = 'defdelnotebranch'

		SET @return = @@error
	END

	IF(@delnotebranch = 0 AND @return = 0)
		SET @delnotebranch = @location
		
	SET @itemCategory = @category	

    IF len(isnull(@acctno,'')) = 12 AND @agrmtno != 0
BEGIN
	SELECT @taxrate = isnull(taxrate,@taxrate), @cashprice = price, @HPPrice = price
	FROM lineitem
	WHERE acctno = @acctno
	AND agrmtno = @agrmtno
	AND ItemID = @itemID
	AND stocklocn = @location
END

set @Addisionaltaxrate = 0
    -- Below change for Curacao Tax : LUX and OB tax is application for this country
    select @countrycode = countrycode from country
	IF(@countrycode ='Q')
	BEGIN
		select @taxrate = ( Rate * 100) from [Merchandising].[TaxRate] where Name = 'OB' and productid is NULL and EffectiveDate < getdate()
		select @Addisionaltaxrate = Rate from [Merchandising].[TaxRate] where Name = 'LUX' and productid in (select id from Merchandising.Product where sku = @itemNo) and EffectiveDate < getdate()
		IF(@Addisionaltaxrate > 0)
		BEGIN
		  SET @taxrate = @Addisionaltaxrate *100
		END 
	END

	--set @taxrate = @taxrate *100

	--select * from [Merchandising].[TaxRate] where Name = 'LUX' and productid in (select id from Merchandising.Product where sku = @itemNo) and EffectiveDate < getdate()
	--select @taxrate
	--select @Addisionaltaxrate
	--select @itemID
--#13716 - CR12949
IF (@return = 0)
	BEGIN
		SELECT @readyAssist = case when (exists(select * from code c
										where c.code = s.iupc
										and c.category = 'RDYAST') --or exists(select * from sales.LinkedContracts lc			--#18604
																		--where lc.category = s.category)
																		) then 1
					  else 0 end
		FROM stockitem s
		WHERE s.ItemId = @itemID

		SET @return = @@error
	END

	
GO
SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO
