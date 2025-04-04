
if exists (select * from dbo.sysobjects where id = object_id('[dbo].[VE_BillGenerationSave]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
BEGIN
	DROP PROCEDURE [dbo].[VE_BillGenerationSave]
END
GO
 
Create PROCEDURE [dbo].[VE_BillGenerationSave]
       @BillGenerationHeader XML 
	   ,@IsUpdate bit
       ,@Message VARCHAR(MAX) OUTPUT
       ,@StatusCode INT OUTPUT    
AS  
BEGIN 
	SET NOCOUNT ON;   
	SET @Message = '';
	SET @StatusCode = 0
       
	DECLARE @CustId VARCHAR(20) = N'',
	@acctno VARCHAR(20) = N'',
	@CheckOutId int,
	@FirstName VARCHAR(30),
	@LastName VARCHAR(60),
	@Flag BIT = 0,
	@AccountType VARCHAR(10),
	@AddressType char(2),
	@BillType VARCHAR(20) ,
	 
	 -------------need to add and changes for Soldby User ID-------------
	@UserFirstName VARCHAR(50) ,
	@UserLastName  VARCHAR(50) ,
	@MVEUserID  VARCHAR(7),
	@CoSaCSLoginID   VARCHAR(7),
	@CoSaCSUserID  VARCHAR(7)
	-------------need to add and changes for Soldby User ID-------------

	DECLARE @itemnoDT varchar(20) = 'DT'
	DECLARE @itemnoADMIN varchar(20) = 'ADMIN'


	-----------------------------------------------------------
	--NOTE: GET account No TO IDENTIFY THE DML OPERATIONs ONT THE WHICH account no BASIS
	SELECT 
		@CustId = T.c.value('CustomerId[1]','varchar(20)'),
		@acctno = T.c.value('AccountNo[1]','varchar(20)'),
		@CheckOutId = T.c.value('CheckOutId[1]','INT'),
		@FirstName = T.c.value('FirstName[1]','VARCHAR(30)'),
		@LastName = T.c.value('LastName[1]','VARCHAR(60)'),
		@AccountType = T.c.value('AccountType[1]','VARCHAR(10)'),
		@AddressType = T.c.value('AddressType[1]','VARCHAR(2)'),
		@BillType = T.c.value('BillType[1]','VARCHAR(20)'),

		-------------need to add and changes for Soldby User ID-------------
		@UserFirstName = T.c.value('UserFirstName[1]','VARCHAR(50)'),
		@UserLastName = T.c.value('UserLastName[1]','VARCHAR(50)'),
		@MVEUserID = T.c.value('EmployeeNo[1]','VARCHAR(7)'),
		@CoSaCSLoginID=T.c.value('CoSaCSUserID[1]','VARCHAR(7)')
		-------------need to add and changes for Soldby User ID-------------

	FROM @BillGenerationHeader.nodes('/BillGenerationHeader') T(c)	
	----------------------------------------------------------- 
    -- VALIDATE DUPLICATE ORDERNO -- START
    if (ISNULL((Select COUNT(orderid) FROM (Select ISNULL(t.c.value('OrderId[1]' ,'[varchar](20)'),0) as orderid FROM @BillGenerationHeader.nodes('/BillGenerationHeader/BillGenerationList/BillGenerationLists') T(c) ) T GROUP By orderid HAving COUNT(orderid)>1),0) > 1) 
	BEGIN  
        SET @Flag = 1
        SET @Message = @Message + 'More than one OrderId Exists for this CheckOutId.'
        SET @StatusCode = 404;
    END    
    -- VALIDATE DUPLICATE ORDERNO -- END
	-----------------------------------------------------------	
	-- VALIDATE CustomerId -- START
    IF(@CustId = '' AND @CustId IS NOT NULL) and @Flag != 1
    BEGIN 
        SET @Flag = 1
        SET @Message = @Message + 'CustomerId is Required.'
        SET @StatusCode = 404; 
    END    
    -- VALIDATE CustomerId -- END	
	----------------------------------------------------------- 
    -- VALIDATE CUSTOMER Exists -- START
    if (NOT EXISTS(Select 1 from customer Where custid = @CustId)) and @Flag != 1
    BEGIN  
        SET @Flag = 1
        SET @Message = @Message + 'Customer record not exists.'
        SET @StatusCode = 404;
    END    
    -- VALIDATE CUSTOMER Exists -- END
	-----------------------------------------------------------
    -- VALIDATE FirstName -- START
    IF(@FirstName = '' AND @FirstName IS NOT NULL) and @Flag != 1
    BEGIN  
        SET @Flag = 1
        SET @Message = @Message + 'FirstName is Required.'
        SET @StatusCode = 404;
    END    
    -- VALIDATE FirstName -- END
    -----------------------------------------------------------
    -- VALIDATE FirstName -- START
    IF(@LastName = '' AND @LastName IS NOT NULL) and @Flag != 1
    BEGIN  
        SET @Flag = 1
        SET @Message = @Message + 'LastName is Required.'
        SET @StatusCode = 404;
    END    
    -- VALIDATE FirstName -- END
	----------------------------------------------------------- 
	-- VALIDATE FirstName -- START
    IF(@acctno = '' AND @acctno IS NOT NULL)
    BEGIN  
        SET @Flag = 1
        SET @Message = @Message + 'AccountNumber is required.'
        SET @StatusCode = 404;
    END    
    -- VALIDATE FirstName -- END
	-----------------------------------------------------------
    -- VALIDATE ACCOUNTNUMBER FOR CUSTOMER -- START
    if (NOT EXISTS(Select TOP 1 custid from custacct Where custid = @CustId and acctno = @acctno)) and @Flag != 1
    BEGIN  
        SET @Flag = 1
        SET @Message = @Message + 'AccountNumber does not belongs to this Customer.'
        SET @StatusCode = 404;
    END    
    -- VALIDATE ACCOUNTNUMBER FOR CUSTOMER -- END
    -----------------------------------------------------------
    -- VALIDATE ACCOUNTTYPE -- START
    IF NOT Exists(Select TOP 1 a.acctno from CustAcct,Acct  a
					Where CustAcct.acctno = a.acctno and  Custid= @CustId AND a.accttype = @AccountType
					AND a.acctno = @acctno  order by  a.dateacctopen desc) and @Flag != 1
    BEGIN 
        SET @Flag = 1
        SET @Message = @Message + 'AccountType not exists for this AccountNo.'
        SET @StatusCode = 404; 
    END    
    -- VALIDATE ACCOUNTTYPE -- END
    -----------------------------------------------------------
    -- VALIDATE ORDERNO -- START
    IF(@CheckOutId = '' OR @CheckOutId IS NULL OR @CheckOutId = '0') and @Flag != 1
    BEGIN 
        SET @Flag = 1
        SET @Message = @Message + 'CheckOutId is Required.'
        SET @StatusCode = 404; 
    END    
    -- VALIDATE ORDERNO -- END    
    
    -----------------------------------------------------------
    ---- VALIDATE CHECKOUTID EXISTS -- START
	if (EXISTS(Select TOP 1 acctno from lineitem Where Orderlineno = @CheckOutId)) and @isUpdate = 'false' and @Flag != 1
    BEGIN  
        SET @Flag = 1
        SET @Message = @Message + 'CheckOutId already exists.'
        SET @StatusCode = 404;
    END  
	---- VALIDATE CHECKOUTID EXISTS-- END
	-----------------------------------------------------------
	-- VALIDATE BillType -- START
    IF(@BillType = '' AND @BillType IS NOT NULL) and @Flag != 1
    BEGIN 
        SET @Flag = 1
        SET @Message = @Message + 'BillType is Required.'
        SET @StatusCode = 404; 
    END    
    -- VALIDATE BillType -- END	
	-----------------------------------------------------------
	  -- VALIDATE CUSTOMER Exists -- END
	-----------------------------------------------------------
	-------------need to add and changes for Soldby User ID-------------
    -- VALIDATE UserFirstName -- START
    IF(@UserFirstName = '' AND @UserFirstName IS NOT NULL) and @Flag != 1
    BEGIN  
        SET @Flag = 1
        SET @Message = @Message + 'UserFirstName is Required.'
        SET @StatusCode = 404;
    END    
    -- VALIDATE UserLastName -- END
    -----------------------------------------------------------
    -- VALIDATE UserLastName -- START
    IF(@UserLastName = '' AND @UserLastName IS NOT NULL) and @Flag != 1
    BEGIN  
        SET @Flag = 1
        SET @Message = @Message + 'UserLastName is Required.'
        SET @StatusCode = 404;
    END    
    -- VALIDATE UserLastName -- END
	----------------------------------------------------------- 
    -- VALIDATE EmployeeNo -- START
    IF(@MVEUserID = '' AND @MVEUserID IS NOT NULL) and @Flag != 1
    BEGIN  
        SET @Flag = 1
        SET @Message = @Message + 'EmployeeNo is Required.'
        SET @StatusCode = 404;
    END    
    -- VALIDATE UserFirstName -- END
	
	----------------------------------------------------------- 
	-- VALIDATE CSR user -- START
	------if((select top 1 ID  from [Admin].[User] C where 
	------(C.firstname LIKE @UserFirstName + '%' OR @UserFirstName IS NULL) AND (C.LastName LIKE @UserLastName + '%' OR @UserLastName IS NULL)  
	------and BranchNo in (select branchno from branch b ,merchandising.location l where b.branchno=l.SalesId and Fascia='Courts Optical') ) IS NULL )and @Flag != 1
	------  BEGIN  
 ------       SET @Flag = 1
 ------       SET @Message = @Message + @UserFirstName + ' ' + @UserLastName + ' User not found in CoSaCS.'
 ------       SET @StatusCode = 404;
 ------   END  
	-- VALIDATE CSR user -- END
	------------------------------------------------
	-- VALIDATE CSR user -- START
	if( NOT EXISTS (select 1  from [Admin].[User] C where [Login] = @CoSaCSLoginID ) ) and @Flag != 1
	  BEGIN  
        SET @Flag = 1
        SET @Message = @Message + ' User ID - '+ @CoSaCSLoginID +' not found in CoSaCS.'
        SET @StatusCode = 404;
    END  
	-- VALIDATE CSR user -- END
	--------------------------------------------------
		select @CoSaCSUserID =ID from [Admin].[User] U where U.[Login]= @CoSaCSLoginID 
		--select  top 1 @CoSaCSUserID = ID from [Admin].[User] C where 
		--(C.firstname LIKE @UserFirstName + '%' OR @UserFirstName IS NULL) AND (C.LastName LIKE @UserLastName + '%' OR @UserLastName IS NULL)
		--order by LastChangePassword desc
	-----------------------------------need to add and changes for Soldby User ID upto-------------

    BEGIN TRANSACTION
	IF( @Flag = 0)
	BEGIN 
		 Declare @BranchNo int
		 Select @BranchNo = SUBString(@acctno,0,4)
			   -----------------------------------------------------------
		 Declare @acctnoNew VARCHAR(20)
		 select @acctnoNew =( 
		 Select DISTINCT acctno from lineitem li Where Orderlineno = @CheckOutId 
		 and acctno not in  ( Select acctno from [dbo].[cancellation] c where li.acctno = c.acctno
		 ) and li.acctno != @acctno		 )  
		 Select @acctnoNew as acctnoNew
		 ----------------------------------------------------------------
		 DECLARE @currDate datetime		 SET @currDate = GETDATE()
	 IF (@acctnoNew IS NOT NULL)
	 BEGIN
	  Select 'In Cancellation condition True' as Cancellation
	  EXEC [DN_AccountCancelSP] @acctnoNew, @CustId, 99995, @currDate, @BranchNo, 'Change of Mind', 0, 0
	  Update stockquantity SEt qtyavailable = (qtyavailable + LI.quantity ) 
	  FROM stockquantity sk Inner Join lineitem LI
	  ON sk.itemno = LI.itemno AND sk.stocklocn = LI.stocklocn  AND LI.Orderlineno = @CheckOutId 
	  Where LI.Orderlineno = @CheckOutId AND 
	  LI.acctno in (Select c.acctno from [dbo].[cancellation] c where c.acctno = @acctnoNew )  
                         
	  Update lineitem SEt quantity = 0  
	  FROM lineitem LI Inner Join stockquantity sk 
	  ON sk.itemno = LI.itemno AND sk.stocklocn = LI.stocklocn AND LI.Orderlineno = @CheckOutId
	  Where LI.Orderlineno = @CheckOutId AND 
	  LI.acctno  in (Select c.acctno from [dbo].[cancellation] c where c.acctno = li.acctno )
	 END

 --NOTE: DECLARING BELOW VARIABLE FOR ASSIGNING DEFAULT VALUE FOR INSERT OR UPDATE
	DECLARE  @origbr smallint ,@agrmtno int ,@itemno varchar(20) ,@itemsupptext varchar(80), @quantity float ,@delqty float
         ,@stocklocn smallint ,@price money ,@ordval money ,@datereqdel datetime ,@timereqdel varchar(12) ,@dateplandel datetime
		 ,@delnotebranch smallint ,@qtydiff char ,@itemtype varchar(1) ,@notes varchar(200)
		 ,@isKit smallint = 0  
		 ,@parentitemno varchar(18) ,@parentlocation smallint ,@contractno varchar(10) ,@expectedreturndate datetime
		 ,@deliveryprocess char ,@deliveryarea varchar(8) ,@DeliveryPrinted char ,@assemblyrequired char ,@damaged char
         ,@PrintOrder smallint ,@taxrate float ,@ItemID int  ,@ParentItemID int ,@SalesBrnNo int ,@ID int ,@Express char ,@WarrantyGroupId int

	IF(@Origbr IS NULL)  SET @Origbr = 0;                     
	IF(@acctno IS NULL)  SET @acctno  = N''              
	IF(@agrmtno IS NULL) SET @agrmtno = 1
	IF(@itemsupptext IS NULL) SET @itemsupptext = N''
	IF(@delqty IS NULL) SET @delqty = 0
	IF(@notes IS NULL) SET @notes = 'CheckoutId - '+ CAST(@CheckOutId as nvarchar(15))
	IF(@itemtype IS NULL) SET @itemtype = N'S'
	IF (@isKit IS NULL) SET @isKit = 0
	IF(@parentitemno IS NULL) SET @parentitemno = N''
	IF (@parentlocation IS NULL) SET @parentlocation = 0
	IF(@contractno IS NULL) SET @contractno = N''
	IF (@deliveryprocess IS NULL) SET @deliveryprocess = N'I'

	------ GET DELIVERY AREA FROM CUSTOMER -----START
	Declare @delarea nvarchar(100)
	Select @delarea = (deliveryarea) from custAddress 
	where custid = @CustId and addtype = CASE WHEN addtype = 'H' THEN 'H'
											  WHEN addtype = 'W' THEN 'W' 
											  WHEN addtype = 'D' THEN 'D' ELSE '' END and addtype <>'' order by addressid

	IF (@deliveryarea IS NULL) SET @deliveryarea = ISNULL((@delarea ) ,'')

	------ GET DELIVERY AREA FROM CUSTOMER -----END
	IF (@DeliveryPrinted IS NULL) SET @DeliveryPrinted = N'N'
	IF (@assemblyrequired IS NULL) SET @assemblyrequired = N'N'
	IF (@damaged IS NULL) SET @damaged = N'N'
	IF (@PrintOrder IS NULL) SET @PrintOrder = 0
	IF (@taxrate IS NULL) SET @taxrate = 0
	IF (@ParentItemID IS NULL) SET @ParentItemID = 0
	IF (@Express IS NULL) SET @Express = N'N'              
	SET @datereqdel = DATEADD(day, DATEDIFF(day, '19000101', GETDATE()), '19000101')
	SET @timereqdel = FORMAT(GETDATE(),'tt')
	SET @dateplandel = '01-01-1900'

              --NOTE: CHECK temp TABLE ALREADY EXISTS 
              IF OBJECT_ID('tempdb..#Main_temp') IS NOT NULL
                     DROP TABLE #Main_temp
              
              --NOTE: BELOW QUERY TO GENERATE temp TABLE WITH DEFAULT EMPTY DML ACTION
              Select 
                       @Origbr AS Origbr
                     , @acctno AS acctno
                     , @agrmtno AS agrmtno 
                     , t.c.value('itemno[1]' ,'[varchar](20)') AS itemno  
                     , @itemsupptext AS itemsupptext
                     , t.c.value('Quantity[1]' ,'[float]') AS Quantity
                     , @delqty AS delqty
                     , t.c.value('stocklocn[1]' ,'[varchar](20)') AS stocklocn
                     ,(CASE WHEN t.c.value('Quantity[1]' ,'[float]') <> 0 
					 THEN t.c.value('Price[1]' ,'[float]')/t.c.value('Quantity[1]' ,'[float]')
					 ELSE 0 END) AS Price
                     , t.c.value('Price[1]' ,'[float]') AS ordval
                     , @datereqdel AS datereqdel
                     , @timereqdel AS timereqdel
                     , @dateplandel AS dateplandel
                     , t.c.value('stocklocn[1]' ,'[varchar](20)') AS delnotebranch
                     , 'Y' AS qtydiff                     
                     , @itemtype AS itemtype
                     , @notes AS notes
					 , CASE WHEN taxrate > 0 THEN ROUND(((CASE WHEN t.c.value('Quantity[1]' ,'[float]') <> 0 
					 THEN t.c.value('Price[1]' ,'[float]')/t.c.value('Quantity[1]' ,'[float]')
					 ELSE 0 END) * SI.taxrate/100)* t.c.value('Quantity[1]' ,'[float]'),2) ELSE 0 END as taxamt
					 , (t.c.value('Discount[1]' ,'[FLOAT]') * -1) as Discount
                     , @isKit AS isKit
                     , ISNULL(@AddressType,'H') AS AddressType
                     , @parentitemno AS parentitemno
                     , @parentlocation AS parentlocation
                     , @contractno AS contractno
                     , @expectedreturndate as expectedreturndate
                     , @deliveryprocess as deliveryprocess
                     , @deliveryarea AS deliveryarea
                     , @DeliveryPrinted AS DeliveryPrinted 
                     , @assemblyrequired AS assemblyrequired 
                     , @damaged AS damaged
					 , t.c.value('OrderId[1]' ,'[varchar](20)') AS Orderno
                     , @CheckOutId AS OrderLineNo 
                     , @PrintOrder AS PrintOrder
					 , SI.taxrate AS taxrate
                     , SI.Id AS ItemId
                     , @ParentItemID AS ParentItemID
                     , t.c.value('stocklocn[1]' ,'[varchar](20)') AS SalesBrnNo
                     , @Express AS Express
                     , @WarrantyGroupId AS WarrantyGroupId
                     , '' AS DMLAction
              INTO #Main_temp
              FROM @BillGenerationHeader.nodes('/BillGenerationHeader/BillGenerationList/BillGenerationLists') T(c) 
              INNER JOIN StockInfo SI ON t.c.value('itemno[1]' ,'[varchar](20)') = SI.itemno
		
		IF (@BillType = 'SalesOrder')
		BEGIN
		Insert into #Main_temp
			  Select Origbr,	obj_LI.acctno,	agrmtno,	obj_LI.itemno,	itemsupptext,0	Quantity,	delqty,	obj_LI.stocklocn,	obj_LI.Price,0	ordval,	datereqdel,	
			  timereqdel,	dateplandel,	
			  delnotebranch,	qtydiff,	itemtype,	notes,	obj_LI.taxamt, 0 Discount,	isKit,ISNULL(@AddressType,'H') AS	AddressType,	parentitemno,	
			  parentlocation,	contractno,	
			  expectedreturndate,	deliveryprocess,	deliveryarea,	DeliveryPrinted,	assemblyrequired,	damaged,	obj_VELI.Orderno,	OrderLineNo,	
			  PrintOrder,	obj_LI.taxrate,	obj_LI.ItemId,	ParentItemID,	SalesBrnNo,	Express,	WarrantyGroupId,'U'	DMLAction
			  From lineitem obj_LI 
			  inner join VE_lineitem obj_VELI on obj_LI.itemno = obj_VELI.itemno and obj_LI.acctno = obj_VELI.acctno
			  where obj_VELI.OrderNo not in (Select Orderno from #Main_temp mt where mt.acctno = obj_LI.acctno) and obj_LI.acctno = @acctno 
			  and obj_LI.itemno NOT IN ('STAX','ADMIN','DT')
		
		Insert into #Main_temp
			  Select Origbr,	obj_LI.acctno,	agrmtno,	obj_LI.itemno,	itemsupptext,0	Quantity,	delqty,	obj_LI.stocklocn,	obj_LI.Price,0	ordval,	datereqdel,	
			  timereqdel,	dateplandel,	
			  delnotebranch,	qtydiff,	itemtype,	notes,	obj_LI.taxamt, 0 Discount,	isKit,ISNULL(@AddressType,'H') AS	AddressType,	parentitemno,	
			  parentlocation,	contractno,	
			  expectedreturndate,	deliveryprocess,	deliveryarea,	DeliveryPrinted,	assemblyrequired,	damaged,	obj_VELI.Orderno,	OrderLineNo,	
			  PrintOrder,	obj_LI.taxrate,	obj_LI.ItemId,	ParentItemID,	SalesBrnNo,	Express,	WarrantyGroupId,'U'	DMLAction
			  From lineitem obj_LI 
			  inner join VE_lineitem obj_VELI on obj_LI.itemno = obj_VELI.itemno and obj_LI.acctno = obj_VELI.acctno
			  where obj_LI.itemno not in (Select itemno from #Main_temp mt where mt.acctno = obj_LI.acctno)
			  and obj_LI.acctno = @acctno and obj_LI.itemno NOT IN ('STAX','ADMIN','DT')

		END

	-----------------------------------------------------------
    -- VALIDATE NO ITEMS -- START
    if (NOT EXISTS(Select 1 OrderLineNo from #Main_temp Where OrderLineNo = @CheckOutId ))
    BEGIN         
		print @Message 
		UPDATE stockquantity SET qtyavailable = (qtyavailable + LI.Quantity)
        FROM stockquantity sk inner join lineitem LI 
			ON sk.itemno = LI.itemno 
			AND sk.stocklocn = LI.stocklocn
			AND LI.Orderlineno = @CheckOutId

		UPDATE lineitem SET Quantity = 0 , ordval = (0) ,price = (0), taxamt = (0) where acctno = @acctno
    
		UPDATE VE_lineitem SET Quantity = 0 , price = 0, discount = 0, taxamt = (0) WHERE CheckOutId = @CheckOutId 
        
		UPDATE acct SET agrmttotal = 0 WHERE acctno = @acctno 
    END    
    -- VALIDATE NO ITEMS -- END
    -----------------------------------------------------------
	--NOTE: SET 'U' THE DMLACTION COLUMN VALUE WHICH ARE FOR UPDATE 
	Update #Main_temp SET DMLAction = 'U'
	FROM #Main_temp t INNER JOIN lineitem Li  
	ON LI.itemno = t.itemno AND LI.stocklocn = t.stocklocn AND LI.acctno = t.acctno

	--NOTE: SET 'I' THE DMLACTION COLUMN VALUE WHICH ARE FOR INSERT 
	Update #Main_temp SET DMLAction = 'I'
	Where DMLAction != 'U'

	--NOTE: BELOW QUERY TO GENERATE VE_temp TABLE WITH DEFAULT EMPTY DML ACTION
	IF OBJECT_ID('tempdb..#VE_temp') IS NOT NULL
		DROP TABLE #VE_temp
	----------------------------------
	--NOTE: BELOW QUERY TO GENERATE #VE_temp TABLE TO SEPARATE OUT THE RECORDS TO BE INSERT INTO PHYSICAL TABLE
	Select @CheckOutId as OrderLineNo, [OrderNo], [acctno], 
	--Changes are make for Exclusive and Inclusive
	--[price],

	CASE WHEN (select Value from CountryMaintenance where CodeName ='agrmttaxtype') = 'E' 
						THEN Price 
						else 
			(CASE WHEN taxrate > 0 THEN ROUND(((Price * taxrate/100)),2) ELSE 0 END) + Price end as 'Price',
	[taxrate]
	, CASE WHEN taxrate > 0 THEN ROUND(([price] * taxrate/100) * [quantity],2) ELSE 0 END  [taxamt]
	, ([Discount] / CASE WHEN [quantity] = 0 then 1 else [quantity] end) AS [Discount]
	, [quantity], [stocklocn], [ItemNo], [ItemID],'' AS DMLAction INTO #VE_temp 
	from #Main_temp 
	--NOTE: SET 'U' THE DMLACTION COLUMN VALUE WHICH ARE FOR UPDATE 
	Update #VE_temp SET DMLAction = 'U'
	FROM #VE_temp t INNER JOIN VE_lineitem VE_Li  
	ON VE_Li.itemid = t.itemid AND VE_Li.stocklocn = t.stocklocn AND 
	VE_Li.acctno = t.acctno and t.OrderNo = VE_Li.OrderNo

	Update #VE_temp SET DMLAction = 'U'
	FROM #VE_temp t INNER JOIN VE_lineitem VE_Li  
	ON VE_Li.itemid != t.itemid AND VE_Li.acctno = t.acctno and t.OrderNo = VE_Li.OrderNo

	--NOTE: SET 'I' THE DMLACTION COLUMN VALUE WHICH ARE FOR INSERT 
	Update #VE_temp SET DMLAction = 'I'
	Where DMLAction != 'U'

	--NOTE: INSERT THE LINE ITEM PRODUCT IN PHYSICAL TABLE 
	If (@BillType != 'Exchange')
	BEGIN 
    INSERT INTO VE_lineitem(	[CheckOutId], [OrderNo], [acctno], [price], [taxrate], [taxamt], [quantity], [Discount], [stocklocn], [ItemNo], [ItemID], [CreatedDate], [BillType],MVEUserID,CoSaCSUserID)
	Select @checkOutId as CheckOutId, [OrderNo], [acctno], [price], [taxrate], [taxamt], [quantity], [Discount], [stocklocn], [ItemNo], [ItemID], GETDATE() as CreatedDate, @BillType,@MVEUserID,@CoSaCSUserID
	from #VE_temp Where DMLAction = 'I'
	END
	ELSE 
	BEGIN
	INSERT INTO VE_lineitem(	[CheckOutId], [OrderNo], [acctno], [price], [taxrate], [taxamt], [quantity], [Discount], [stocklocn], [ItemNo], [ItemID], [CreatedDate], [BillType],MVEUserID,CoSaCSUserID)
	Select NULL as CheckOutId, [OrderNo], [acctno], [price], [taxrate], [taxamt], [quantity], [Discount], [stocklocn], [ItemNo], [ItemID], GETDATE() as CreatedDate, @BillType,@MVEUserID,@CoSaCSUserID
	from #VE_temp Where DMLAction = 'I' and itemno != @itemnoADMIN

	update VE_lineitem set CheckOutId = (select top 1 CheckOutId from VE_lineitem where acctno = @acctno and CheckOutId is not null) where acctno = @acctno and CheckOutId is null

	END

	IF OBJECT_ID('tempdb..#COSACS_temp') IS NOT NULL
		DROP TABLE #COSACS_temp

		DECLARE @branchNoDON int
		Select @branchNoDON = SUBString(@acctno,0,4)

		DECLARE @itemnoDON varchar(20) = 'DON'
		DECLARE @itemnoDOT varchar(10) = 'DOT'
		DECLARE @itemnoSTAX varchar(10) = 'STAX'

		DECLARE @itemTypeDON nvarchar(10)
		Select @itemTypeDON = itemtype  from stockinfo where itemno = @itemnoDON

		DECLARE @itemTypeDOT nvarchar(10)
		Select @itemTypeDOT = itemtype  from stockinfo where itemno = @itemnoDOT

		DECLARE @itemTypeSTAX nvarchar(10)
		Select @itemTypeSTAX = itemtype  from stockinfo where itemno = @itemnoSTAX

		--Changes are make for Exclusive and Inclusive
		Declare @agrmttaxtype nvarchar(30)
		SET @agrmttaxtype =(select Value from CountryMaintenance where CodeName ='agrmttaxtype')
	----------------------------------
	--NOTE: CREATE #COSACS_temp table with DISCOUNT ROW 
	SELECT Origbr,	acctno, agrmtno, itemno, itemsupptext, Quantity, delqty, stocklocn, Price, ordval, datereqdel, timereqdel, dateplandel, delnotebranch, qtydiff
	,itemtype,	notes,	taxamt,	isKit,	AddressType,	parentitemno,	parentlocation,	contractno,	expectedreturndate,	deliveryprocess,	deliveryarea,	DeliveryPrinted
	,assemblyrequired,	damaged,	PrintOrder,	taxrate,	ItemId,	ParentItemID,	SalesBrnNo,	Express,	WarrantyGroupId,	DMLAction
	INTO #COSACS_temp From 
	(
		SELECT Origbr, acctno, agrmtno, itemno, itemsupptext,SUM(Quantity) as Quantity, delqty, stocklocn, 
		--price,
		--Changes are make for Exclusive and Inclusive
		 CASE WHEN (select Value from CountryMaintenance where CodeName ='agrmttaxtype') = 'E' 
						THEN Price else (CASE WHEN taxrate > 0 THEN ROUND(((Price * taxrate/100)* SUM(Quantity)),2) ELSE 0 END) + Price end as 'Price',

		(SUM(Quantity)*  CASE WHEN (select Value from CountryMaintenance where CodeName ='agrmttaxtype') = 'E' 
						THEN Price else (CASE WHEN taxrate > 0 THEN ROUND(((Price * taxrate/100)* SUM(Quantity)),2) ELSE 0 END) + Price end
						) as ordval,
		--(SUM(Quantity)* Price) as ordval,
		 datereqdel, timereqdel, dateplandel, delnotebranch
		, qtydiff,itemtype,	notes, (CASE WHEN taxrate > 0 THEN ROUND(((Price * taxrate/100)* SUM(Quantity)),2) ELSE 0 END) as taxamt, isKit
		, AddressType, parentitemno, parentlocation, contractno, expectedreturndate, deliveryprocess, deliveryarea, DeliveryPrinted
		, assemblyrequired,	damaged, PrintOrder, taxrate, ItemId, ParentItemID,	SalesBrnNo,	Express, WarrantyGroupId, DMLAction
		FROM #Main_temp WHERE itemno NOT IN (@itemnoDT, @itemnoADMIN)
		GROUP BY 
			Origbr,	acctno, agrmtno, itemno, itemsupptext, delqty, stocklocn, Price, ordval, datereqdel, timereqdel, dateplandel, delnotebranch, qtydiff, itemtype,	notes
		   ,taxamt,	isKit, AddressType, parentitemno, parentlocation, contractno, expectedreturndate, deliveryprocess, deliveryarea, DeliveryPrinted, assemblyrequired
		   ,damaged, PrintOrder, taxrate, ItemId, ParentItemID,	SalesBrnNo,	Express, WarrantyGroupId, DMLAction
	  
			UNION ALL

			SELECT Origbr, acctno, agrmtno, 'DOT' as itemno --('DOT' + CAST(ROW_NUMBER() OVER (ORDER BY acctno) as nvarchar(5))) as itemno
			, itemsupptext,
		CASE WHEN (Discount) != 0 THEN 1 ELSE 0 END,
		delqty,@branchNoDON as stocklocn
		,  (CASE WHEN taxrate > 0 THEN ROUND((Discount / (1 + taxrate/100)),2) ELSE Discount END) as Price
		,  (CASE WHEN taxrate > 0 THEN ROUND((Discount / (1 + taxrate/100)),2) ELSE Discount END) as ordval
				, datereqdel, timereqdel , dateplandel,stocklocn as delnotebranch, qtydiff, @itemTypeDOT as itemtype,	notes
				,ROUND(Discount -  (CASE WHEN taxrate > 0 THEN  ROUND((Discount / (1 + taxrate/100)),2) ELSE Discount END),2) taxamt
				,	isKit, AddressType, Itemno as	parentitemno
				, stocklocn as parentlocation, contractno, expectedreturndate, deliveryprocess,	deliveryarea, DeliveryPrinted, assemblyrequired, damaged, PrintOrder, '' taxrate
				, (Select TOP 1 ID from stockinfo where itemno in ('DOT')) as ItemId --40421 as ItemId
				, Itemid as ParentItemID, stocklocn as SalesBrnNo,	Express, WarrantyGroupId, CASE WHEN @isUpdate = 'true' THEN 'U' ELSE 'I' END   DMLAction
		FROM #Main_temp t WHERE itemno NOT IN (@itemnoDT, @itemnoADMIN)
		and  0 != CAse when (@isUpdate = 'false' ) and Discount = 0 then Discount else 1 end
		 and taxrate <> 0 and (Discount) != 0

		 UNION ALL

		SELECT Origbr, acctno, agrmtno, @itemnoDON as itemno
		, itemsupptext,
		CASE WHEN (Discount) != 0 THEN 1 ELSE 0 END,
		delqty,@branchNoDON as stocklocn
		,  (CASE WHEN taxrate > 0 THEN ROUND((Discount / (1 + taxrate/100)),2) ELSE Discount END) as Price
		,  (CASE WHEN taxrate > 0 THEN ROUND((Discount / (1 + taxrate/100)),2) ELSE Discount END) as ordval
				, datereqdel, timereqdel , dateplandel,stocklocn as delnotebranch, qtydiff, @itemTypeDON as itemtype,	notes
				,ROUND(Discount -  (CASE WHEN taxrate > 0 THEN  ROUND((Discount / (1 + taxrate/100)),2) ELSE Discount END),2) taxamt
				,	isKit, AddressType, itemno as	parentitemno
				, stocklocn as parentlocation, contractno, expectedreturndate, deliveryprocess,	deliveryarea, DeliveryPrinted, assemblyrequired, damaged, PrintOrder, '' taxrate
				, (Select TOP 1 ID from stockinfo where itemno in (@itemnoDON)) as ItemId -- 40421 as ItemId
				, Itemid as ParentItemID, stocklocn as SalesBrnNo,	Express, WarrantyGroupId, CASE WHEN @isUpdate = 'true' THEN 'U' ELSE 'I' END   DMLAction
		FROM #Main_temp WHERE itemno NOT IN (@itemnoDT, @itemnoADMIN)
		and  0 != CAse when (@isUpdate = 'false' ) and Discount = 0 then Discount else 1 end
		 and taxrate = 0 
		 and (Discount) != 0

		UNION ALL

		SELECT Origbr, acctno, agrmtno, @itemnoSTAX as itemno, itemsupptext,
		CASE WHEN SUM(Quantity) != 0 THEN 1 ELSE 0 END as Quantity,
		delqty,@branchNoDON as stocklocn
		, SUM( (CASE WHEN taxrate > 0 THEN ROUND(((Price * taxrate/100) * Quantity),2) ELSE 0 END)) as Price
		, SUM( (CASE WHEN taxrate > 0 THEN ROUND(((Price * taxrate/100) * Quantity),2) ELSE 0 END)) as ordval
				, datereqdel, timereqdel , dateplandel,'' as delnotebranch, qtydiff, @itemTypeSTAX as itemtype,	notes
				,ROUND(SUM(Discount) - SUM( (CASE WHEN taxrate > 0 THEN  ROUND((Discount / (1 + taxrate/100)) * 2,2) ELSE Discount END)),2) taxamt
				,	isKit, AddressType,	parentitemno
				, parentlocation, contractno, expectedreturndate, deliveryprocess,	deliveryarea, DeliveryPrinted, assemblyrequired, damaged, PrintOrder, '' taxrate
				, (Select TOP 1 ID from stockinfo where itemno in (@itemnoSTAX)) as ItemId --11633 as ItemId
				, ParentItemID, '' SalesBrnNo,	Express, WarrantyGroupId, CASE WHEN @isUpdate = 'true' THEN 'U' ELSE 'I' END   DMLAction
		FROM #Main_temp WHERE itemno NOT IN (@itemnoDT, @itemnoADMIN) and Quantity <>0
		--Changes are make for Exclusive and Inclusive
		--and  (@agrmttaxtype !='E') 
		and 0 != CASE WHEN (select Value from CountryMaintenance where CodeName ='agrmttaxtype') = 'E' 
						THEN 1 else 0 end
		GROUP BY
			Origbr, acctno, agrmtno, itemsupptext, delqty, datereqdel, timereqdel,	dateplandel, qtydiff, notes, isKit,	AddressType, parentitemno, parentlocation, contractno
		,expectedreturndate, deliveryprocess, deliveryarea, DeliveryPrinted, assemblyrequired, damaged,	PrintOrder,	ParentItemID, Express, WarrantyGroupId   

		UNION ALL

		SELECT Origbr, acctno, agrmtno,@itemnoDT as itemno, itemsupptext,SUM(Quantity) as Quantity, delqty, stocklocn, Price, (SUM(Quantity)* Price) as ordval, datereqdel, timereqdel, dateplandel, delnotebranch
		, qtydiff,itemtype,	notes, (CASE WHEN taxrate > 0 THEN ROUND(((Price * taxrate/100) * SUM(Quantity)),2) ELSE 0 END) as taxamt, isKit
		, AddressType, parentitemno, parentlocation, contractno, expectedreturndate, deliveryprocess, deliveryarea, DeliveryPrinted
		, assemblyrequired,	damaged, PrintOrder, taxrate, ItemId, ParentItemID,	SalesBrnNo,	Express, WarrantyGroupId, DMLAction
		FROM #Main_temp WHERE itemno IN (@itemnoDT)
		GROUP BY 
			Origbr,	acctno, agrmtno, itemno, itemsupptext, delqty, stocklocn, Price, ordval, datereqdel, timereqdel, dateplandel, delnotebranch, qtydiff, itemtype,	notes
		   ,taxamt,	isKit, AddressType, parentitemno, parentlocation, contractno, expectedreturndate, deliveryprocess, deliveryarea, DeliveryPrinted, assemblyrequired
		   ,damaged, PrintOrder, taxrate, ItemId, ParentItemID,	SalesBrnNo,	Express, WarrantyGroupId, DMLAction

		   UNION ALL

		SELECT Origbr, acctno, agrmtno,@itemnoADMIN as itemno, itemsupptext,SUM(Quantity) as Quantity, delqty, stocklocn, Price, (SUM(Quantity)* Price) as ordval, datereqdel, timereqdel, dateplandel, delnotebranch
		, qtydiff,itemtype,	notes, (CASE WHEN taxrate > 0 THEN ROUND(((Price * taxrate/100) * SUM(Quantity)),2) ELSE 0 END) as taxamt, isKit
		, AddressType, parentitemno, parentlocation, contractno, expectedreturndate, deliveryprocess, deliveryarea, DeliveryPrinted
		, assemblyrequired,	damaged, PrintOrder, taxrate, ItemId, ParentItemID,	SalesBrnNo,	Express, WarrantyGroupId, DMLAction
		FROM #Main_temp WHERE itemno IN (@itemnoADMIN)
		GROUP BY 
			Origbr,	acctno, agrmtno, itemno, itemsupptext, delqty, stocklocn, Price, ordval, datereqdel, timereqdel, dateplandel, delnotebranch, qtydiff, itemtype,	notes
		   ,taxamt,	isKit, AddressType, parentitemno, parentlocation, contractno, expectedreturndate, deliveryprocess, deliveryarea, DeliveryPrinted, assemblyrequired
		   ,damaged, PrintOrder, taxrate, ItemId, ParentItemID,	SalesBrnNo,	Express, WarrantyGroupId, DMLAction

	 ) t

	  if exists(Select COUNT(*) from #COSACS_temp group by itemno, Quantity having COUNT(*)> 1 and itemno = @itemnoDON and Quantity !=0)
	 BEGIN
	 Print 'DON Block'
	 IF OBJECT_ID('tempdb..#tempDON') IS NOT NULL
		DROP TABLE #tempDON
		
	 Select Origbr,	acctno, agrmtno, itemno, 
	 itemsupptext, Quantity, delqty, stocklocn, SUM(Price) Price, SUM(ordval) ordval, datereqdel, timereqdel, dateplandel, delnotebranch, qtydiff
	,itemtype,	notes,	taxamt,	isKit,	AddressType, parentitemno,	
	parentlocation,	contractno,	expectedreturndate,	deliveryprocess,	deliveryarea,	DeliveryPrinted
	,assemblyrequired,	damaged,	PrintOrder,	taxrate,	ItemId,	ParentItemID,	
	SalesBrnNo,	Express,	WarrantyGroupId,	DMLAction
	into #tempDON from #COSACS_temp group by 
	Origbr,	acctno, agrmtno, itemno, itemsupptext, Quantity, delqty, stocklocn,  datereqdel, timereqdel, dateplandel, delnotebranch, qtydiff
	,itemtype,	notes,	taxamt,	isKit,	AddressType,	parentitemno,	parentlocation,	contractno,	expectedreturndate,	deliveryprocess,	deliveryarea,	DeliveryPrinted
	,assemblyrequired,	damaged,	PrintOrder,	taxrate,	ItemId,	ParentItemID,	SalesBrnNo,	Express,	WarrantyGroupId,	DMLAction having itemno = 'DON'

	select * from #tempDON

	 Delete from #COSACS_temp where itemno = 'DON'
	 Select '#tempDON', * from #tempDON
	 insert into #COSACS_temp select * from #tempDON
	 
	 END

	 -- UPDATE DMLAction COLUMN WHEN DON and STAX amd ADMINCAHRGE are ADDED IN UPDATE CASE
	 update #COSACS_temp set DMLAction = 'I' from #COSACS_temp Ct WHERE itemno NOT IN (Select itemno from lineitem where acctno = Ct.acctno ) and (itemno != @itemnoDON OR itemno != 'DOT')
	 update #COSACS_temp set DMLAction = 'I' from #COSACS_temp Ct WHERE parentitemno NOT IN (Select parentitemno from lineitem where acctno = Ct.acctno ) and (itemno = @itemnoDON OR itemno != 'DOT')

	----------------------------------
	IF OBJECT_ID('tempdb..#AuditUpdate') IS NOT NULL
		DROP TABLE #AuditUpdate

	Select
		objCOSACS_temp.acctno, objCOSACS_temp.agrmtno,

		--'100000' as Empeenochange,
		@CoSaCSUserID as Empeenochange, -- need to add and changes for Soldby User ID
 
		 objCOSACS_temp.itemno, objCOSACS_temp.stocklocn
		, objLI.quantity as QuantityBefore
		, objLI.quantity - (Select top 1 Quantity from VE_LineItem t where t.OrderNo in (Select orderno from #VE_temp) ) as QuantityAfter
		, objLI.price * objLI.quantity as ValueBefore
		, objLI.price * (objLI.quantity - (Select top 1 Quantity from VE_LineItem t where t.OrderNo in (Select orderno from #VE_temp) )) AS ValueAfter
		, GETDATE() as Datechange, objCOSACS_temp.contractno
		, (CASE WHEN objCOSACS_temp.taxrate > 0 THEN ROUND(((objLI.Price * objLI.taxrate/100)* objLI.quantity),2) ELSE 0 END) AS TaxAmtBefore
		, (CASE WHEN objCOSACS_temp.taxrate > 0 THEN ROUND(((objCOSACS_temp.Price * objCOSACS_temp.taxrate/100)* objCOSACS_temp.quantity),2) ELSE 0 END) AS TaxAmtAfter
		, CASE WHEN @BillType = 'SalesOrder' THEN 'Revise' 
		      WHEN (@BillType = 'Return' OR @BillType = 'Exchange') THEN 'GRTCancel'
			  ELSE 'Revise' END as [source], objCOSACS_temp.ParentItemno, objCOSACS_temp.ParentLocation, objCOSACS_temp.DelNoteBranch
		, 0 as RunNo, objCOSACS_temp.ItemID, objCOSACS_temp.ParentItemID, objCOSACS_temp.SalesBrnNo
		, 'I' as DMLAction
		INTO #AuditUpdate
		from #COSACS_temp objCOSACS_temp inner join LineItem objLI 
			ON  objCOSACS_temp.acctno = objLI.acctno 
			and (objCOSACS_temp.quantity != objLI.quantity OR objCOSACS_temp.price != objLI.price OR objCOSACS_temp.taxamt != objLI.taxamt)
			and objCOSACS_temp.itemid = objLI.itemid
		where objCOSACS_temp.acctno = @acctno and objLI.quantity > 0
	----------------------------------
	 
    --NOTE: UPDATE THE qty AVAILABEL IN THE TABLE STOCKQUANTITY
    UPDATE stockquantity SET qtyavailable = (qtyavailable - (CASE WHEN LI.Quantity IS NULL THEN (ISNULL(LI.Quantity,0)+t.Quantity)  ELSE t.Quantity - LI.Quantity  END))
	FROM stockquantity sk inner join #COSACS_temp t ON sk.itemno = t.itemno and sk.stocklocn = t.stocklocn
            LEFT JOIN lineitem LI 
			ON t.itemno = LI.itemno AND t.stocklocn = LI.stocklocn 
			AND LI.acctno =  @acctno
			AND t.itemid = LI.itemid

	if exists(Select COUNT(*) from #COSACS_temp group by itemno, Quantity having COUNT(*)> 1 and itemno != @itemnoDON and Quantity !=0)
	BEGIN
	print 1

    UPDATE lineitem SET Quantity = (li.quantity-
	(Select COUNT(*) from VE_LineItem obj_VLI where obj_VLI.acctno = LI.acctno and obj_VLI.itemno = LI.itemno and obj_VLI.quantity = 0
	)) , ordval = (t.ordval) ,price = ( t.Price), taxamt = ( t.taxamt), notes = @notes, deliveryarea = @deliveryarea
			, itemno = t.itemno
            FROM #COSACS_temp t inner join lineitem LI ON t.itemno = LI.itemno AND t.stocklocn = LI.stocklocn AND LI.acctno = @acctno
			and t.Quantity > 0
	END
	ELSE IF (@BillType = 'SalesOrder')
	BEGIN
	print 2

	if (EXISTS(Select 1 from VE_LineItem VE Where VE.orderno IN (Select orderno from #COSACS_temp objCT 
																		Where VE.acctno != objCT.acctno and objCT.quantity = 1) 
																		and VE.orderno IS NOT NULL))
		BEGIN
			update VE_LineItem set PickingId = OrderNo from VE_LineItem VE where VE.orderno IN (Select orderno from #COSACS_temp objCT 
																		Where VE.acctno != objCT.acctno and objCT.quantity = 1) and VE.orderno IS NOT NULL
			
			update VE_LineItem set OrderNo = NULL from VE_LineItem VE where VE.orderno IN (Select orderno from #COSACS_temp objCT 
																		Where VE.acctno != objCT.acctno and objCT.quantity = 1) and VE.orderno IS NOT NULL
		END

		UPDATE lineitem SET Quantity = t.Quantity
		 , ordval = (t.ordval) 
		, price = CASE WHEN (@BillType = 'Return' OR @BillType = 'Exchange') THEN LI.price ELSE (t.Price) END
		, taxamt = ( t.taxamt)
		, delqty = CASE WHEN ((@BillType = 'Return' OR @BillType = 'Exchange') and t.Quantity = 0) THEN 0 ELSE t.delqty END
		, notes = @notes
		, deliveryarea = @deliveryarea
		, itemno = t.itemno
		, itemid = t.itemid
		, parentitemno = t.parentitemno
		, parentlocation = t.parentlocation
		, ParentItemID = t.ParentItemID
		, SalesBrnNo = t.SalesBrnNo
            FROM #COSACS_temp t inner join lineitem LI ON t.itemno = LI.itemno AND t.stocklocn = LI.stocklocn AND LI.acctno = @acctno
			and t.parentitemno = LI.parentitemno 
			and 0 != CASE WHEN (Select COUNT(*) from #COSACS_temp group by itemno having COUNT(*) > 1 ) > 1 THEN t.quantity else 1 end --and t.quantity != 0
			where T.itemno != @itemnoDON

			UPDATE lineitem SET Quantity = t.Quantity
		 , ordval = (t.ordval) 
		, price = CASE WHEN (@BillType = 'Return' OR @BillType = 'Exchange') THEN LI.price ELSE (t.Price) END
		, taxamt = ( t.taxamt)
		, delqty = CASE WHEN ((@BillType = 'Return' OR @BillType = 'Exchange') and t.Quantity = 0) THEN 0 ELSE t.delqty END
		, notes = @notes
		, deliveryarea = @deliveryarea
		, itemno = t.itemno
		, itemid = t.itemid
		, parentitemno = t.parentitemno
		, parentlocation = t.parentlocation
		, ParentItemID = t.ParentItemID
		, SalesBrnNo = t.SalesBrnNo
            FROM #COSACS_temp t inner join lineitem LI ON t.itemno = LI.itemno AND t.stocklocn = LI.stocklocn AND LI.acctno = @acctno
			and t.parentitemno = li.parentitemno
			where T.itemno = @itemnoDON
			
		update LineItem set quantity = 0, ordval = 0 where acctno = @acctno and itemno = @itemnoDON
		and parentitemno in (Select itemno from #COSACS_temp where quantity = 0)
		and 0 != CASE WHEN (Select COUNT(*) from #COSACS_temp group by itemno having COUNT(*) > 1 ) > 1 THEN 0 else 1 end

		if (NOT EXISTS(Select 1 from #COSACS_temp Where acctno = @acctno and itemno = @itemnoDON))
		BEGIN
			update LineItem set quantity = 0, price = 0, ordval = 0 where acctno = @acctno and itemno = @itemnoDON
		END
	END

	IF (@BillType = 'Return')
	BEGIN
	Print 'Return DiffAMt'

		--added change after Inclusive and exclusive country type
		if( ( select Value from CountryMaintenance where CodeName ='agrmttaxtype') = 'E') 
		BEGIN
		Update VE_LineItem SET ExchgAmtDiff = -1 * (LI.price + LI.discount +LI.taxamt)* 
		CASE When ve_t.quantity < 1 then LI.quantity else ve_t.quantity end
		,OldCheckOutId = @CheckOutId, BillType = @BillType
		, IsReturn = 1 from VE_LineItem LI 
		Inner Join #VE_temp ve_t on LI.acctno = ve_t.acctno and LI.Orderno = ve_t.Orderno where li.acctno = @acctno 
		END 
		ELSE 
		BEGIN 
		Update VE_LineItem SET ExchgAmtDiff = -1 * (LI.price + LI.discount)* 
		CASE When ve_t.quantity < 1 then LI.quantity else ve_t.quantity end
		,OldCheckOutId = @CheckOutId, BillType = @BillType
		, IsReturn = 1 from VE_LineItem LI 
		Inner Join #VE_temp ve_t on LI.acctno = ve_t.acctno and LI.Orderno = ve_t.Orderno where li.acctno = @acctno 
		END
		
		UPDATE lineitem SET
		  Quantity  = CASE WHEN LI.quantity != 0 THEN (LI.quantity - objVE_t.quantity) ELSE 0 END
		, ordval = LI.Price * (CASE WHEN LI.quantity != 0 THEN (LI.quantity - objVE_t.quantity) ELSE 0 END) 
		, price = CASE WHEN (@BillType = 'Return' OR @BillType = 'Exchange') THEN LI.price ELSE (t.Price) END
		, taxamt = ( t.taxamt)
		, delqty = CASE WHEN ((@BillType = 'Return' OR @BillType = 'Exchange') and t.Quantity < 1) THEN 0 ELSE t.delqty END
		, notes = @notes
		, deliveryarea = @deliveryarea
		, itemno = t.itemno
		, itemid = t.itemid
            FROM #COSACS_temp t inner join lineitem LI ON t.itemno = LI.itemno AND t.stocklocn = LI.stocklocn AND LI.acctno = @acctno
			INNER JOIN VE_LineItem obj_VLI ON t.acctno = obj_VLI.acctno and Li.itemno = obj_VLI.itemno and obj_VLI.OrderNo = (Select orderno from #VE_temp )
			left JOIN #VE_temp objVE_t on obj_VLI.OrderNo = objVE_t.Orderno
			where Li.itemno NOT IN ('STAX','ADMIN','DT','DON') --and obj_VLI.OrderNo = objVE_t.Orderno
		------------------------------------------------------------
		if (NOT EXISTS(Select 1 from #VE_temp Where acctno = @acctno and Discount != 0))
		BEGIN
			print 12345			 
			Declare @DiscountPrice float
			SET @DiscountPrice = (Select (CASE WHEN taxrate > 0 THEN ROUND((Discount / (1 + taxrate/100)),2) ELSE Discount END)
			 from  VE_LineItem where acctno = @acctno and OrderNo = (select TOP 1 orderno from #VE_temp)  ) 
			
			update lineitem set price = CASE WHEN price - @DiscountPrice = 0 THEN price ELSE price - @DiscountPrice END  --(price-@DiscountPrice) 
			, ordval = (price-@DiscountPrice)
			, Quantity = CASE WHEN price-@DiscountPrice = 0 THEN 0 ELSE 1 END 
			, delqty = CASE WHEN price-@DiscountPrice = 0 THEN 0 ELSE 1 END
			where acctno = @acctno
			and parentitemno = (select TOP 1 itemno from #VE_temp where Discount = 0)
		END

		--------------------------------------------------------------------
		UPDATE lineitem SET Orderlineno = @CheckOutId Where acctno = @acctno

		Update VE_LineItem SET OldCheckOutId = @CheckOutId, IsReturn = 1 From VE_LineItem VELI 
		INNER JOIN #VE_temp VE_t  ON VELI.acctno = VE_t.acctno and VELI.OrderNo = VE_t.Orderno
		Where VELI.acctno = @acctno and billtype = 'Return' and VE_t.quantity < 1 and IsSync != NULL

		UPDATE VE_LineItem SET IsSync='0' WHERE acctno = @acctno AND itemno = @itemnoDON 

			-------------------------------------------
		INsert into fintrans(origbr, branchno, acctno, transrefno, datetrans, transtypecode, empeeno, transupdated, transprinted, 
			transvalue, bankcode, bankacctno, chequeno, ftnotes, paymethod, runno, [source], agrmtno, ExportedToTallyman)
		Select TOP 1
			origbr, branchno, acctno, transrefno, GETDATE(), transtypecode, empeeno, transupdated, transprinted, 
			--((Select TOP 1 Discount from VE_LineItem where acctno = @acctno and OrderNo = (select TOP 1 Orderno from #VE_temp where Discount = 0))*(-1)) as transvalue
			((Select TOP 1 ISNULL(VE_LI.discount,0) - ISNULL(li.taxamt,0) 
			from VE_LineItem VE_LI left join lineitem LI on VE_LI.acctno = LI.acctno and VE_LI.itemno = LI.parentitemno
			 where VE_LI.acctno = @acctno and VE_LI.OrderNo = (select TOP 1 Orderno from #VE_temp where Discount = 0))*(-1)) as transvalue
			, bankcode, bankacctno, chequeno, 
			(Select TOP 1 ftnotes from fintrans where acctno = @acctno and ftnotes = 'DND2' order by id desc) as ftnotes, 
			paymethod, runno, [source], agrmtno, ExportedToTallyman
		from fintrans where acctno = @acctno and transtypecode = 'GRT' order by id desc
		-------------------------------------------

		IF (@AccountType = 'R')
		BEGIN
			INSERT INTO VE_TaskSchedular (ServiceCode,Code,IsInsertRecord,IsEODRecords,[Status],CheckOutID)  values ('delc',@acctno,'1','0','0',@CheckOutId)
		END
		--------------------------------------------
	END

	IF (@BillType = 'Exchange')
	BEGIN
	Print 'Exchange DiffAMt'
		UPDATE VE_lineitem SET OldOrderNo = (Select orderno from #VE_temp ve_t where ve_t.quantity < 1), BillType = @BillType , IsExchange = 1
		from #VE_temp t
		inner join VE_lineitem LI ON t.orderno = LI.orderno AND t.stocklocn = LI.stocklocn AND LI.acctno = @acctno where t.quantity > 0
		and LI.itemno != 'DT'

		--added change after Inclusive and exclusive country type
		if( ( select Value from CountryMaintenance where CodeName ='agrmttaxtype') = 'E') 
		BEGIN 
		update LIMain SET ExchgAmtDiff = ((LIMain.price+ (Select top 1 discount from #VE_temp where acctno = LIMain.acctno and Orderno = LIMain.OrderNo) --LIMain.Discount
		+LIMain.taxamt) - (LIDumy.price+LIDumy.Discount+LIDumy.taxamt))* LIMain.quantity from VE_lineitem LIMain 
		Inner Join VE_lineitem LIDumy on LIMain.acctno = LIDumy.acctno and LIMain.OldOrderNo = LIDumy.OrderNo
		where LIMain.OldOrderNo in (Select orderno from #VE_temp where quantity < 1 
		) and LIMain.ItemNo not in (@itemnoDT, @itemnoADMIN)

		END
		ELSE 
		BEGIN
		update LIMain SET ExchgAmtDiff = ((LIMain.price+ (Select top 1 discount from #VE_temp where acctno = LIMain.acctno and Orderno = LIMain.OrderNo) --LIMain.Discount
		) - (LIDumy.price+LIDumy.Discount))* LIMain.quantity from VE_lineitem LIMain 
		Inner Join VE_lineitem LIDumy on LIMain.acctno = LIDumy.acctno and LIMain.OldOrderNo = LIDumy.OrderNo
		where LIMain.OldOrderNo in (Select orderno from #VE_temp where quantity < 1 
		) and LIMain.ItemNo not in (@itemnoDT, @itemnoADMIN)
		END

		------------------------------------------------------------
		if ( EXISTS(Select 1 from #VE_temp Where acctno = @acctno and Discount = 0))
		--if ( EXISTS(Select 1 from #VE_temp t inner join VE_lineitem LI ON t.orderno = LI.orderno AND t.stocklocn = LI.stocklocn ))
		BEGIN
			print 12345			 
			Declare @DiscountPriceEx float
			SET @DiscountPriceEx = (Select (CASE WHEN taxrate > 0 THEN ROUND((Discount / (1 + taxrate/100)),2) ELSE Discount END)
			 from  VE_LineItem where acctno = @acctno and OrderNo = (select TOP 1 orderno from #VE_temp where quantity = 0)  ) 
			Select  @DiscountPriceEx as DiscountPriceEx

			update lineitem set price = CASE WHEN price - @DiscountPriceEx = 0 THEN price ELSE price - @DiscountPriceEx END
			,ordval = CASE WHEN (Select TOP 1 COUNT(*) from #VE_temp Where acctno = @acctno group by itemno order by count(*) desc) != 2 THEN (price-@DiscountPriceEx) ELSE price end
			, Quantity = CASE WHEN (price-@DiscountPriceEx = 0 and (Select TOP 1 COUNT(*) from #VE_temp Where acctno = @acctno group by itemno order by count(*) desc) != 2) THEN 0 ELSE 1 END 
			 where acctno = @acctno and itemno = @itemnoDON
			 and parentitemno = (select TOP 1 itemno from #VE_temp where quantity = 0)
		END
		--------------------------------------------------------------------

		if not exists(Select COUNT(*) from #COSACS_temp group by itemno having itemno NOT IN ('STAX','ADMIN','DT') and COUNT(*) > 1)
		BEGIN
		Print 'Exchange DiffAMt If' 
			if (NOT EXISTS(Select 1 from VE_LineItem Where orderno in (Select orderno from #VE_temp Where acctno = @acctno and quantity = 0)))
			BEGIN		

			UPDATE lineitem SET
			quantity = CASE WHEN (t.DMLAction = 'U' and t.Quantity = 0) THEN li.quantity - 1 ELSE li.quantity + ISNULL(t.quantity,1) END
			, ordval = (LI.price) * CASE WHEN (t.DMLAction = 'U' and t.Quantity = 0) THEN li.quantity - 1 ELSE li.quantity + ISNULL(t.quantity,1) END
			, price = CASE WHEN (@BillType = 'Return' OR @BillType = 'Exchange') THEN LI.price ELSE (t.Price) END
			, taxamt = ( t.taxamt)
			, delqty = CASE WHEN ((@BillType = 'Return' OR @BillType = 'Exchange') and t.Quantity < 1) THEN 0 ELSE t.delqty END
			, notes = @notes
			, deliveryarea = @deliveryarea
			, itemno = t.itemno
			, itemid = t.itemid
				FROM #COSACS_temp t inner join lineitem LI ON t.itemno = LI.itemno AND t.stocklocn = LI.stocklocn AND LI.acctno = @acctno
				AND t.parentitemno = LI.parentitemno
				where Li.itemno NOT IN ('STAX','ADMIN','DT','DON')

				END
				ELSE
				BEGIN

				UPDATE lineitem SET
			quantity = t.quantity,
			 ordval = t.price * li.quantity
			, price = CASE WHEN (@BillType = 'Return' OR @BillType = 'Exchange') THEN LI.price ELSE (t.Price) END
			, taxamt = ( t.taxamt)
			, delqty = CASE WHEN ((@BillType = 'Return' OR @BillType = 'Exchange') and t.Quantity < 1) THEN 0 ELSE t.delqty END
			, notes = @notes
			, deliveryarea = @deliveryarea
			, itemno = t.itemno
			, itemid = t.itemid
				FROM #COSACS_temp t inner join lineitem LI ON t.itemno = LI.itemno AND t.stocklocn = LI.stocklocn AND LI.acctno = @acctno
				AND t.parentitemno = LI.parentitemno
				where Li.itemno NOT IN ('STAX','ADMIN','DT','DON')

				END
		END
		else
		Begin
		Print 'Exchange DiffAMt Else'
			UPDATE VE_lineitem SET  BillType = 'IdenticalEx' from #VE_temp t
			inner join VE_lineitem LI ON t.orderno = LI.orderno AND t.stocklocn = LI.stocklocn AND LI.acctno = @acctno where t.quantity > 0 
			
		END
		-----------------------------------------------------------------
		UPDATE lineitem SET Orderlineno = @CheckOutId Where acctno = @acctno
		
		Update VE_LineItem SET OldCheckOutId = @CheckOutId	, IsExchange = 1 From VE_LineItem VELI 
		INNER JOIN #VE_temp VE_t  ON VELI.acctno = VE_t.acctno and VELI.OrderNo = VE_t.Orderno
		Where VELI.acctno = @acctno and (billtype = 'Exchange' OR billtype = 'IdenticalEx') and VE_t.quantity > 0 

		UPDATE VE_LineItem SET IsSync='0' WHERE acctno = @acctno AND itemno = @itemnoDON 
		-----------------------------------------------------------------------
	END
    
	UPDATE VE_lineitem SET Quantity = CASE WHEN (t.Quantity =1 and @BillType ='Return') then 0 else t.Quantity end 
						, price = CASE WHEN t.Quantity =0 then LI.price else t.price end
						, discount = CASE WHEN t.Quantity =0 then LI.discount else t.discount end, taxamt = ( t.taxamt)
						, itemno = t.itemno, itemid = t.itemid
            FROM #VE_temp t inner join VE_lineitem LI ON t.orderno = LI.orderno AND t.stocklocn = LI.stocklocn AND LI.acctno = @acctno
				and t.Quantity = 1
    ------------------------------------
                         
    --NOTE: INSERT THE LINE ITEM PRODUCT WHICH ADDED IN JSON       

		 INSERT INTO lineitem(
                     origbr,
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
                     isKit, 
                     deliveryaddress,    
                     parentitemno, 
                     parentlocation,     
                     contractno, 
                     expectedreturndate, 
                     deliveryprocess,  
                     deliveryarea,
                     DeliveryPrinted,  
                     assemblyrequired, 
                     damaged,    
                     Orderlineno, 
                     PrintOrder, 
                     taxrate,  
                     ItemID, 
                     ParentItemID, 
                     SalesBrnNo, 
                     Express, 
                     WarrantyGroupId
              )
              Select origbr,
                     acctno,  
                     agrmtno, 
                     itemno, 
                     itemsupptext,
                     quantity, 
                     delqty, 
                     stocklocn,
                     price,
                     (ordval) AS ordval,
                     datereqdel,
                     timereqdel,
                     dateplandel,
                     delnotebranch,
                     qtydiff,
                     itemtype,
                     notes,
                     taxamt,
                     isKit,
                     addressType,
                     parentitemno,
                     parentlocation,
                     contractno,
                     expectedreturndate, 
                     deliveryprocess,  
                     deliveryarea,
                     DeliveryPrinted, 
                     assemblyrequired, 
                     damaged,   
                     @checkOutId as Orderlineno, 
                     PrintOrder, 
                     taxrate,  
                     ItemID,   
                     ParentItemID,
                     SalesBrnNo,
                     Express,
                     WarrantyGroupId
                     FROM #COSACS_temp Where DMLAction = 'I' 

	---------------------------------
	-- NOTE: INSERT lineitemaudit RECORD INITIALY WITH 'NewAccount' status
	Insert into lineitemaudit
		(
		acctno, agrmtno, Empeenochange, itemno, stocklocn, QuantityBefore, QuantityAfter, ValueBefore, ValueAfter, Datechange, contractno, TaxAmtBefore
		, TaxAmtAfter, [source], ParentItemno, ParentLocation, DelNoteBranch, RunNo, ItemID, ParentItemID, SalesBrnNo)
		
	Select
		acctno, agrmtno,
		@CoSaCSUserID as Empeenochange,
		--'100000' as Empeenochange, 
		itemno, stocklocn,0 as QuantityBefore
		,Quantity as QuantityAfter
		,0 as ValueBefore,ordval AS ValueAfter, GETDATE() as Datechange
		, contractno,0 AS TaxAmtBefore
		,taxamt AS TaxAmtAfter,'NewAccount' as [source], ParentItemno, ParentLocation, DelNoteBranch,0 as RunNo, ItemID, ParentItemID, SalesBrnNo
		from #COSACS_temp where acctno = @acctno and quantity > 0 and DMLAction = 'I'
	---------------------------------
	-- NOTE: ADD RECORD IN FACTTRANS TABLE

	-- Sold By ID changes 
	-- UPDATE agreement TABLE WITH VALUE OF EMPLOYEE No.
	update agreement SET empeenosale = @CoSaCSUserID where acctno = @acctno

	Declare @hibuffno int
	IF (@BillType = 'SalesOrder')
	BEGIN

		Select  @hibuffno = (hibuffno + 2) from branch where branchno = @BranchNo

		update branch SET hibuffno = @hibuffno where branchno = @BranchNo

		insert into facttrans
		Select Origbr, acctno, agrmtno, @hibuffno, itemno, stocklocn, '01', 61, GETDATE(), Quantity, Price, taxamt, 0, ItemId from #COSACS_temp
		where itemno NOT IN ('STAX','ADMIN','DT','DON','DOT') and DMLAction = 'I'

	

	END

	IF (@BillType = 'Exchange')
	BEGIN
		Select  @hibuffno = (hibuffno + 2) from branch where branchno = @BranchNo

		update branch SET hibuffno = @hibuffno where branchno = @BranchNo

		insert into facttrans
		Select Origbr, acctno, agrmtno, @hibuffno, itemno, stocklocn, '01', 61, GETDATE(), Quantity, Price, taxamt, 0, ItemId from #COSACS_temp
		where itemno NOT IN ('STAX','ADMIN','DT','DON','DOT') and DMLAction = 'I'

	END

	-------------------------
	-- NOTE: INSERT lineitemaudit RECORD WITH 'Revise' status

	Insert into lineitemaudit
		Select acctno, agrmtno, Empeenochange, itemno, stocklocn, QuantityBefore, QuantityAfter, ValueBefore, ValueAfter, Datechange, contractno, TaxAmtBefore
		, TaxAmtAfter, [source], ParentItemno, ParentLocation, DelNoteBranch, RunNo, ItemID, ParentItemID, SalesBrnNo from #AuditUpdate
	---------------------------------

	---- UPDATE agreement TABLE WITH VALUE OF EMPLOYEE No.
	--update agreement SET empeenosale = empeenochange where acctno = @acctno

    END
              IF (@@error != 0)
              BEGIN
                     ROLLBACK
                     print'Transaction rolled back'
                     SET @StatusCode = 500;            
              END
              ELSE
              BEGIN
                     if @isUpdate  = 'false' and @Flag = 0
					  BEGIN
						SET @StatusCode = 201;	
						SET @Message = CAST(@acctno as VARCHAR(MAX))    
					  END
					 else if @isUpdate  = 'true' and @Flag = 0
					  BEGIN
						SET @StatusCode = 202;  
						SET @Message = CAST(@acctno as VARCHAR(MAX))          
					  END              
                     COMMIT
                     print'Transaction committed'
                     PRINT @Message
              END
         
END