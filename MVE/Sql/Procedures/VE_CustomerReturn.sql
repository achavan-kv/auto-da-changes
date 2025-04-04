if exists (select * from dbo.sysobjects where id = object_id('[dbo].[VE_CustomerReturn]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
BEGIN
	DROP PROCEDURE [dbo].[VE_CustomerReturn]
END
GO

Create PROCEDURE [dbo].[VE_CustomerReturn]
	 @CustomerReturnxml XML 
	,@Message VARCHAR(MAX) OUTPUT
	,@StatusCode INT OUTPUT	     
AS  
BEGIN  

	SET NOCOUNT ON;   
	SET @Message = '';
	SET @StatusCode = 0	
	DECLARE 
	@Mode VARCHAR(10),
	@Flag int=0,
	@CurrentBookingId INT = N'',
	@AccountNo VARCHAR(MAX) = N'',
	@Name VARCHAR(MAX) = N'',
	@CustomerId VARCHAR(MAX) = N'',
	@NOTES VARCHAR(MAX) = N'',
	@TotalCreditValuetoacct VARCHAR(MAX) = N'',
	@CheckOutId VARCHAR(MAX) = N'',
	@OrderId VARCHAR(MAX) = N'',
	@ReturnType VARCHAR(MAX) = N'',
	@CollectionReason VARCHAR(MAX) = N'',
	@ItemNo VARCHAR(MAX) = N'',
	@Quantity VARCHAR(MAX) = N'',
	@Stocklocn VARCHAR(MAX) = N'',
	@ReturnStocklocn VARCHAR(MAX) = N'',
	@TotalValue VARCHAR(MAX) = N'',
	@CancelStatus VARCHAR(MAX) = 'Schwer',
	@ID VARCHAR(MAX) = N'',
	@ProductId VARCHAR(MAX) = N'',
	@CoSaCSUserID int,
	@Status1  VARCHAR(MAX) = N'',
	@OriginalID VARCHAR(MAX) = N''
	SET @Mode='MVE'
	IF(@Mode='MVE')
	BEGIN
		SELECT 
				@AccountNo = T.c.value('AccountNo[1]','VARCHAR(MAX)'),
				@CustomerId = T.c.value('CustomerId[1]','VARCHAR(MAX)'),
				@CheckOutId = T.c.value('CheckOutId[1]','VARCHAR(MAX)')		
		FROM	@CustomerReturnxml.nodes('/CustomerReturnModel') T(c)			
	END

	IF( @Flag = 0)
	BEGIN TRANSACTION	

	IF(@Mode='MVE')
	BEGIN

		IF OBJECT_ID('tempdb..#temp') IS NOT NULL
		DROP TABLE #temp

		SELECT 
			  t.c.value('OrderId[1]' ,'[varchar](20)') AS OrderId
			, t.c.value('ReturnType[1]' ,'[varchar](20)') AS ReturnType
			, t.c.value('CollectionReason[1]' ,'[varchar](MAX)') AS CollectionReason
			, t.c.value('ItemNo[1]' ,'[varchar](MAX)') AS ItemNo
			, t.c.value('Quantity[1]' ,'money') AS Quantity  
			, t.c.value('Stocklocn[1]' ,'[varchar](MAX)') AS Stocklocn	
			, t.c.value('ReturnStocklocn[1]' ,'[varchar](MAX)') AS ReturnStocklocn	
			, t.c.value('TotalValue[1]' ,'money') AS TotalValue
			,T1.ItemID AS ProductId
			,isnull(T1.CoSaCSUserID,'10000') AS CoSaCSUserID
		INTO #temp
		FROM @CustomerReturnxml.nodes('/CustomerReturnModel/CustomerReturnList/CustomerReturnList') T(c)
		LEFT OUTER JOIN VE_LineItem T1 ON T1.acctno=@AccountNo AND T1.itemno=t.c.value('ItemNo[1]' ,'[varchar](MAX)') AND T1.OrderNo=t.c.value('OrderId[1]' ,'[varchar](20)' )		
	END

	IF (ISNULL((SELECT COUNT(OrderId) FROM #temp),0) <= 0) 
	BEGIN  
        SET @Flag = 1
        SET @Message = @Message + 'Received null Customer return order array list'
        SET @StatusCode = 404;
    END 
	Else 
	SET @StatusCode = 201;
	
	SELECT * FROM #temp
IF(@Flag=0)
BEGIN
	IF(@Mode='MVE' AND @Flag=0)
	BEGIN
	DECLARE Cur_CustomerReturn CURSOR FOR  
	SELECT	 OrderId
			,ReturnType
			,CollectionReason
			,ItemNo,Quantity
			,Stocklocn
			,ReturnStocklocn
			,TotalValue
			,ProductId 
			,CoSaCSUserID
	FROM #temp;
	OPEN Cur_CustomerReturn 
	FETCH NEXT FROM Cur_CustomerReturn     
	INTO 
			 @OrderId
			,@ReturnType
			,@CollectionReason
			,@ItemNo
			,@Quantity
			,@Stocklocn
			,@ReturnStocklocn
			,@TotalValue
			,@ProductId  
			,@CoSaCSUserID
	END

	WHILE @@FETCH_STATUS = 0    
	BEGIN 	

	IF(@Mode='MVE' AND @Flag=0)   --After Delivery
	BEGIN

		DECLARE @LatestID VARCHAR(50);
		SET @LatestID=(SELECT NextHi FROM Hilo WHERE Sequence='warehouse.booking')	
		select 	@LatestID
		DECLARE @Date VARCHAR(100)
		SET @Date=CONVERT(VARCHAR, GETDATE(),126);
		
		--PRINT '----- START INSERT [Warehouse].[Booking]----------------- '
		INSERT [Warehouse].[Booking]
		(
			  [Id],
			  [CustomerName]
			, [AddressLine1]
			, [AddressLine2]
			, [AddressLine3]
			, [PostCode]
			, [StockBranch]
			, [DeliveryBranch]
			, [DeliveryOrCollection]
			, [DeliveryOrCollectionDate]
			, [ItemNo]
			, [ItemId]
			, [ItemUPC]
			, [ProductDescription]
			, [ProductBrand]
			, [ProductModel]
			, [ProductArea]
			, [ProductCategory]
			, [Quantity]
			, [RepoItemId]
			, [Comment]
			, [DeliveryZone]
			, [ContactInfo]
			, [OrderedOn]
			, [Damaged]
			, [AssemblyReq]
			, [AcctNo]
			, [OriginalId]
			, [TruckId]
			, [PickingId]
			, [PickingAssignedBy]
			, [PickQuantity]
			, [PickingComment]
			, [PickingRejectedReason]
			, [PickingRejected]
			, [ScheduleId]
			, [ScheduleComment]
			, [ScheduleSequence]
			, [PickingAssignedDate]
			, [UnitPrice]
			, [Path]
			, [ScheduleRejected]
			, [ScheduleRejectedReason]
			, [DeliveryRejected]
			, [DeliveryRejectedReason]
			, [DeliveryConfirmedBy]
			, [DeliveryRejectionNotes]
			, [ScheduleQuantity]
			, [DeliverQuantity]
			, [Exception]
			, [Express]
			, [AddressNotes]
			, [BookedBy]
			, [Fascia]
			, [PickUp]
			, [PickUpDatePrinted]
			, [PickUpNotePrintedBy]
			, [DeliveryConfirmedDate]
			, [DeliveryConfirmedOnDate]
			, [DeliveryOrCollectionSlot]
			, [ScheduleRejectedDate]
			, [ScheduleRejectedBy]
			, [SalesBranch]
			, [NonStockServiceType]
			, [NonStockServiceItemNo]
			, [NonStockServiceDescription]
			, [ReceivingLocation]
		)
			SELECT 	TOP 1
				@LatestID,
				[Extent1].[CustomerName] AS [CustomerName], 
				[Extent1].[AddressLine1] AS [AddressLine1], 
				[Extent1].[AddressLine2] AS [AddressLine2], 
				[Extent1].[AddressLine3] AS [AddressLine3], 
				[Extent1].[PostCode] AS [PostCode], 
				[Extent1].[StockBranch] AS [StockBranch], 
				[Extent1].[DeliveryBranch] AS [DeliveryBranch], 
				'C' AS [DeliveryOrCollection], 
				[Extent1].[DeliveryOrCollectionDate] AS [DeliveryOrCollectionDate], 
				[Extent1].[ItemNo] AS [ItemNo], 
				[Extent1].[ItemId] AS [ItemId], 
				[Extent1].[ItemUPC] AS [ItemUPC], 
				[Extent1].[ProductDescription] AS [ProductDescription], 
				[Extent1].[ProductBrand] AS [ProductBrand], 
				[Extent1].[ProductModel] AS [ProductModel], 
				[Extent1].[ProductArea] AS [ProductArea], 
				[Extent1].[ProductCategory] AS [ProductCategory], 
				CAST(CAST(@Quantity AS DECIMAL) AS SMALLINT)  AS [Quantity],
				[Extent1].[ItemId] AS [RepoItemId], 
				@CollectionReason AS [Comment], 
				[Extent1].[DeliveryZone] AS [DeliveryZone], 
				[Extent1].[ContactInfo] AS [ContactInfo], 
				[Extent1].[OrderedOn] AS [OrderedOn], 
				[Extent1].[Damaged] AS [Damaged], 
				[Extent1].[AssemblyReq] AS [AssemblyReq], 
				[Extent1].[AcctNo] AS [AcctNo], 
				NULL AS [OriginalId], 
				NULL AS [TruckId], 
				NULL AS [PickingId], 
				NULL AS [PickingAssignedBy], 
				NULL AS [PickQuantity], 
				NULL AS [PickingComment], 
				NULL AS [PickingRejectedReason], 
				NULL AS [PickingRejected], 
				NULL AS [ScheduleId], 
				NULL AS [ScheduleComment], 
				NULL AS [ScheduleSequence], 
				NULL AS [PickingAssignedDate], 
				[Extent1].[UnitPrice] AS [UnitPrice],
				CONCAT(@LatestID,'.')  AS [Path], 
				NULL AS [ScheduleRejected], 
				NULL AS [ScheduleRejectedReason], 
				NULL AS [DeliveryRejected], 
				NULL AS [DeliveryRejectedReason], 
				NULL AS [DeliveryConfirmedBy], 
				NULL AS [DeliveryRejectionNotes], 
				NULL AS [ScheduleQuantity], 
				NULL AS [DeliverQuantity], 
				'0' AS [Exception], 
				[Extent1].[Express] AS [Express], 
				[Extent1].[AddressNotes] AS [AddressNotes], 
				[Extent1].[BookedBy] AS [BookedBy], 
				[Extent1].[Fascia] AS [Fascia], 
				'1' AS [PickUp],
				NULL AS  [PickUpDatePrinted], 
				NULL AS  [PickUpNotePrintedBy], 
				NULL AS [DeliveryConfirmedDate], 
				NULL AS [DeliveryConfirmedOnDate], 
				[Extent1].[DeliveryOrCollectionSlot] AS [DeliveryOrCollectionSlot], 
				NULL AS [ScheduleRejectedDate], 
				NULL AS [ScheduleRejectedBy], 
				[Extent1].[SalesBranch] AS [SalesBranch], 
				[Extent1].[NonStockServiceType] AS [NonStockServiceType], 
				[Extent1].[NonStockServiceItemNo] AS [NonStockServiceItemNo], 
				[Extent1].[NonStockServiceDescription] AS [NonStockServiceDescription], 
				[Extent1].[ReceivingLocation] AS [ReceivingLocation]
			FROM [Warehouse].[Booking] AS [Extent1]	
			WHERE [Extent1].AcctNo=@AccountNo 
				AND [Extent1].ItemNo=@ItemNo 
				AND [Extent1].[DeliveryBranch] = [Extent1].[StockBranch]
				AND [Extent1].[DeliveryOrCollection]='D'	
				ORDER BY [Extent1].[OrderedOn]  DESC		

		--) 
		--PRINT '----- END INSERT [Warehouse].[Booking]----------------- '

		--PRINT '----- START HiLoAllocate----------------- '	
		declare @p2 INT
		SET @p2=(SELECT NextHi FROM Hilo WHERE Sequence='warehouse.booking')
		DECLARE @p3 INT
		SET @p3=(SELECT MaxLo FROM Hilo WHERE Sequence='warehouse.booking')
		EXEC dbo.HiLoAllocate @Sequence='warehouse.booking',@CurrentHi=@p2 OUTPUT,@MaxLo=@p3 OUTPUT
		SELECT @p2, @p3
		--PRINT '----- END HiLoAllocate----------------- '	
		
		SET  @CurrentBookingId = @LatestID

		declare @p4 nchar(1)
		set @p4='1'
		declare @p5 int
		set @p5=0
		declare @p8 int 
		set @p8=0
		declare @Qty1 VARCHAR(5)
		set @Qty1='-1'
		select @p8
		declare @p19 int
		set @p19=0
		set @p8=0
		select @p8
		set @p8=0
		set @p4=0
		set @p8=0
		declare @p7 int
		set @p7=0
		set @p2=0
		select @p2
		--PRINT '----- START INSERT [LineItemBookingSchedule]----------------- '
		INSERT INTO [dbo].[LineItemBookingSchedule]  
		(
			  [LineItemID]
			, [DelOrColl]
			, [RetItemID]
			, [RetVal]
			, [RetStockLocn]
			, [BookingId]
			, [Quantity]
			, [ItemID]
			, [StockLocn]
			, [Price]
			, [RepUnitPrice])
		(   
			SELECT				
				T0.ID AS [LineItemID]
				,'C' AS [DelOrColl]
				,T2.ItemId AS [RetItemID]
				,T1.price*@Quantity AS [RetVal]
				,@ReturnStocklocn AS [RetStockLocn]
				,@LatestID
				,(CAST(@Quantity AS decimal(10,9))*-1) AS [Quantity]
				,T2.ItemId AS [ItemID]
				,@Stocklocn AS [StockLocn]
				,T1.price AS [Price]
				,NULL AS [RepUnitPrice]
			FROM lineitem T0
			INNER JOIN VE_LineItem T1 on T0.acctno=T1.acctno AND T0.itemno=T1.itemno
			INNER JOIN Warehouse.Booking T2 ON T1.acctno=T2.AcctNo AND T1.ItemID=T2.ItemId			
			WHERE T0.acctno=@AccountNo AND T0.itemno=@ItemNo AND T1.OrderNo=@OrderId ANd T2.id=@LatestID
		)

		--PRINT '------------END LineItemBookingSchedule-----------------------'

		--PRINT '------------START CollectionReason-----------------------'
		INSERT INTO CollectionReason (
			 AcctNo
			,ItemNo
			,CollectionReason
			,DateAuthorised
			,EmpeenoAuthorised
			,DateCommissionCalculated
			,StockLocn
			,CollectType
			,ItemId
		)		
		(SELECT TOP 1
			 @AccountNo
			,''
			,@CollectionReason
			,getdate()
			,isnull(@CoSaCSUserID,'10000')
			,null
			,@stocklocn
			,CASE WHEN @ReturnType='Return' THEN 'C' ELSE 'E' END 
			,T0.ItemId			
		FROM
		Warehouse.Booking T0 where T0.acctno=@AccountNo AND T0.itemno=@ItemNo


		--added for Discount Issue(04/11/2019)
		UNION ALL

		SELECT TOP 1
			 @AccountNo
			,''
			,@CollectionReason
			,getdate()
			,isnull(@CoSaCSUserID,'10000')
			,null
			,@stocklocn
			,CASE WHEN @ReturnType='Return' THEN 'C' ELSE 'E' END 
			,T1.ItemId	
		FROM lineitem T1 
		WHERE T1.AcctNo=@AccountNo
			AND T1.parentitemno=@ItemNo 
			AND itemno IN ('DON','DOT')
			)

		--Closed added for Discount Issue(04/11/2019)

		--PRINT '------------END CollectionReason-----------------------'


		--PRINT '------------START LineItemBooking-----------------------'
		INSERT INTO [dbo].[LineItemBooking]   --need to paste after booking entry
		(
			  [ID]
			, [LineItemID]
			, [Quantity]
		)   
		SELECT 
			 @CurrentBookingId
			,T0.ID
			,@Quantity
		FROM lineitem T0
		INNER JOIN VE_LineItem T1 on T0.acctno=T1.acctno AND T0.itemno=T1.itemno
		WHERE T0.acctno=@AccountNo AND T0.itemno=@ItemNo AND T1.OrderNo=@OrderId
		--PRINT '------------END LineItemBooking-----------------------'

		--added for Discount Issue(04/11/2019)
		--INSERT INTO lineitembfCollection
		--	(
		--		 acctno
		--		,agrmtno
		--		,itemno
		--		,quantity
		--		,price
		--		,ordval	
		--		,contractno	
		--		,ItemID
		--	)
		--SELECT 
		--		 T0.acctno
		--		,1
		--		,''
		--		,T0.quantity
		--		,T0.price
		--		,T0.price
		--		,''
		--		,T0.ItemID
		--FROM VE_LineItem T0
		--WHERE T0.acctno=@AccountNo AND T0.itemno=@ItemNo AND T0.OrderNo=@OrderId
		--AND @ReturnType='Return'
		
		IF(@ReturnType='Return')
		BEGIN
			declare @p800 int
			Declare @Price decimal(10,2)
			Declare @DiscPrice decimal(10,2)

			SET @Price=(SELECT (Price * @Quantity) FROM LineItem WHERE Acctno=@AccountNo AND ItemId=@ProductId AND @ReturnType='Return')
			SET @DiscPrice=(SELECT (Price * @Quantity) FROM LineItem WHERE Acctno=@AccountNo AND parentItemId=@ProductId AND @ReturnType='Return')

			set @p800=0
			exec DN_LineItemBfCollectionSaveSP @acctNo=@AccountNo,@agreementNo=1,@itemID=@ProductId,@quantity=1,@price=@Price,@orderValue=@Price,@contractNo=N'',@return=@p800 output
			select @p800

			--declare @p34 int
			--set @p34=0
			--exec DN_AgreementUpdateSP @origBr=0,@acctNo=@AccountNo,@agreementNo=1,@agreementDate=@Date,@salesPerson=0,@depChqClears=@Date,@holdMerch=N'Y',@holdProp=N'N',@dateDel=@Date,@dateNextDue=@Date,@oldAgreementBal=$0.0000,@cashPrice=$0.0000,@discount=@DiscPrice,@pxallowed=$0.0000,@serviceCharge=$0.0000,@sundryChargeTotal=$0.0000,@agreementTotal=$0.0000,@deposit=$0.0000,@codFlag=N'N',@soa=N'ECOM',@paymethod=N'',@unpaidFlag=N'',@deliveryFlag=N'Y',@fullDelFlag=N'',@PaymentMethod=N' ',@employeeNumAuth=100000,@dateAuth=@Date,@employeeNumChange=100000,@dateChange=@Date,@createdby=0,@paymentholidays=0,@source=N'GRTCancel',@taxFree=0,@return=@p34 output
			--select @p34

		END

		--Closed added for Discount Issue(04/11/2019)

		--added for Discount Issue(04/11/2019)

		--IF NOT EXISTS(select * FROM agreementbfcollection T0 WHERE T0.ACCTNo=@AccountNo AND @ReturnType = 'Return')
		--BEGIN
		--	INSERT INTO agreementbfcollection( 
		--		 acctno
		--		,agrmttotal
		--		,empeeno
		--		)
		--	SELECT 
		--		 T0.acctno
		--		--,(T0.price-(T0.discount * (-1)))
		--		,((SELECT (SUM(transvalue)*(-1)) FROM fintrans WHERE acctno=@AccountNo AND transtypecode='PAY')-(SELECT ISNULL(SUM(transvalue),0) FROM fintrans WHERE acctno=@AccountNo AND transtypecode='GRT'))
		--		,''
		--	FROM VE_LineItem T0
		--	WHERE T0.acctno=@AccountNo 
		--		AND T0.itemno=@ItemNo 
		--		AND T0.OrderNo=@OrderId
		--		AND @ReturnType='Return'

		--END
		--ELSE
		--BEGIN
		--	UPDATE agreementbfcollection SET agrmttotal=(SELECT (SUM(transvalue)*(-1)) FROM fintrans WHERE acctno=@AccountNo AND transtypecode='PAY')
		--	WHERE acctno=@AccountNo
		--END
		--Closed added for Discount Issue(04/11/2019)


		--added for Discount Issue(04/11/2019)
		
		Update acct SET agrmttotal=(agrmttotal-@TotalValue)
		WHERE acctno=@AccountNo AND @ReturnType='Return'


		--Added logic for Return with Taxable Items(18/11/2019)
		DECLARE @TotalTaxAmt FLOAT
		SET @TotalTaxAmt=(SELECT ISNULL(TAXAmt,0) FROM VE_LineItem WHERE OrderNo=@OrderId AND AcctNo=@AccountNo AND ItemId=@ProductId)
		--End Added logic for Return with Taxable Items

		UPDATE agreement SET cashprice=(cashprice-@TotalValue),agrmttotal=(agrmttotal-@TotalValue),source='GRTCancel'
		WHERE acctno=@AccountNo AND @ReturnType='Return'     
		
		--Added logic for Cutomer Return for RF Account(19/11/2019)
		DECLARE @AcctType VARCHAR(10)
		SELECT @AcctType=accttype from acct where acctno=@AccountNo

		UPDATE VE_LineItem SET IsReturn='true' 
		where acctno=@AccountNo 
			AND @ReturnType='Return'
			AND OrderNo=@OrderId
			AND @AcctType='R'      
		--END Added logic for Cutomer Return for RF Account


		------declare @InvoiceVersion INT
		------declare @AgreementInvNoVersion VARCHAR(30)

		------SET @InvoiceVersion=(SELECT (MAX(InvoiceVersion)+1) FROM invoiceDetails WHERE acctno=@AccountNo AND @ReturnType='Return')
		------SET @AgreementInvNoVersion=(SELECT TOP 1 AgreementInvNoVersion FROM invoiceDetails WHERE acctno=@AccountNo AND @ReturnType='Return')
	

		------INSERT INTO invoiceDetails(	acctno,	
		------		agrmtno,	
		------		InvoiceVersion,	datedel,	itemno,	stocklocn,	
		------		quantity,	branchno,	Price,	taxamt,	ItemID,	
		------		ParentItemID,	AgreementInvNoVersion,	contractno,	
		------		returnquantity,	RetItemNo,	RetVal,	LineItemID,	OrdVal)
		------(	
		------SELECT 
		------	T0.acctno,
		------	T0.agrmtno,
		------	@InvoiceVersion
		------	,datereqdel,itemno,stocklocn,
		------	CASE WHEN itemno='STAX' THEN 1 ELSE (0-1) END,
		------	stocklocn,price,taxamt,ItemID,ParentItemID
		------	,@AgreementInvNoVersion
		------	,contractno
		------	,NULL
		------	,NULL
		------	,NULL
		------	,ID,price
		------FROM  LineItem T0
		------WHERE ACCTNo=@AccountNo AND 'Return'='Return' AND itemno='STAX'

		------UNION ALL

		------SELECT 
		------	T0.acctno,
		------	T0.agrmtno,
		------	@InvoiceVersion
		------	,datereqdel,itemno,stocklocn,(0-1),stocklocn,price,taxamt,ItemID,ParentItemID
		------	,@AgreementInvNoVersion
		------	,contractno
		------	,(0-1),itemno,price,ID,price
		------FROM  LineItem T0
		------WHERE ACCTNo=@AccountNo AND @ReturnType='Return' AND (ItemId=@ProductId OR ParentItemID=@ProductId)
		------)
		--Closed added for Discount Issue(04/11/2019)

		--select count(*),acctno from acct group by acctno
		--having count(*)>1

		DELETE FROM schedule where ACCTNo=@AccountNo AND @ReturnType = 'Return' --AND ParentItemID=@ProductID

		INSERT INTO schedule(origbr,	acctno,	agrmtno,	datedelplan,	delorcoll,	itemno	
		,stocklocn	,quantity,	retstocklocn,	retitemno	,retval	,vanno,	
		buffbranchno,	buffno,	loadno,	dateprinted	,printedby	,Picklistnumber
		,undeliveredflag,	datePicklistPrinted	,picklistbranchnumber,
		contractno,	transchedno,	transchednobranch,	datetranschednoprinted,	OrigBuffno,	RunnoImport,	
		[Sequence],	runNo,	DHLPickingDate,	consignmentNoteNo,	deliveryLineNo,	DHLDNNo,
		CreatedBy,	DateCreated,GRTnotes,	ItemID,	RetItemID,	ParentItemID)
			
		(SELECT 
				origbr,
				acctno,
				agrmtno,
				datereqdel,
				'C' AS delorcoll,
				'' AS itemno,
				stocklocn,
				CASE WHEN itemno='DON' THEN -1 ELSE (0-1) END,
				stocklocn,
				'' AS retitemno,
				price,
				'' AS vanno,
				0 AS buffbranchno,
				0 AS buffno,	
				0 AS loadno,	
				NULL AS dateprinted	,				
				isnull(@CoSaCSUserID,'10000') AS printedby,	
				0 AS Picklistnumber,
				'' AS undeliveredflag,		
				NULL AS datePicklistPrinted,	
				NULL AS picklistbranchnumber,
				0 AS contractno,				0 AS transchedno,	
				0 AS transchednobranch,	
				NULL AS datetranschednoprinted	,
				0 AS OrigBuffno	,				NULL AS RunnoImport	,				0 AS [Sequence]	,
				NULL AS runNo	,				NULL AS DHLPickingDate	,				NULL AS consignmentNoteNo,	
				NULL AS  deliveryLineNo	,				NULL AS DHLDNNo	,
				isnull(@CoSaCSUserID,'10000') AS CreatedBy,				getdate() AS DateCreated,				'rtn' AS GRTnotes,					ItemID AS ItemID,	
				0 AS RetItemID	,				@ProductID AS ParentItemID
		FROM  LineItem T0
		WHERE T0.ACCTNo=@AccountNo AND @ReturnType = 'Return' AND T0.ParentItemID=@ProductID
		)	

		DECLARE @XML1 NVARCHAR(MAX)		
		SET @Date=CONVERT(VARCHAR, GETDATE(),126);
		SELECT @XML1=
			(
				SELECT
				  @LatestID  AS [Id]
				, ((([t7].[title] + ' ') + [t7].[firstname]) + ' ') + [t7].[name] AS [Recipient]
				, [t8].[cusaddr1] AS [AddressLine1]
				, [t8].[cusaddr2] AS [AddressLine2]
				, [t8].[cusaddr3] AS [AddressLine3]
				, [t8].[cuspocode] AS [PostCode]
				, (CASE 
					WHEN 0 = 1 THEN [t1].[stocklocn]
					ELSE 128
					END) AS [StockBranch] 
				, (CASE 
					WHEN 0 = 1 THEN [t1].[delnotebranch]
					ELSE 128
					END) AS [DeliveryBranch] 
				,'C' AS [Type]
				, GETDATE() AS [RequestedDate]
				, [t9].[itemno] AS [SKU]
				, [t9].[Id] AS [ItemId]
				, [t9].[IUPC] AS [ItemUPC]
				, (([t9].[itemdescr1] + ' ') + ' ') + [t9].[itemdescr2] AS [ProductDescription]
				, (CASE 
					WHEN [t9].[Brand] IS NULL THEN CONVERT(NVarChar(MAX),'')
					ELSE [t9].[Brand]
					END) AS [ProductBrand]
				, (CASE 
					WHEN [t9].[VendorLongStyle] IS NULL THEN CONVERT(NVarChar(MAX),'')
					ELSE [t9].[VendorLongStyle]
					END) AS [ProductModel]
				, '' AS ProductArea
				, CONVERT(NVarChar,[t9].[category]) AS [ProductCategory]
				, CONVERT(SmallInt,[t0].[Quantity]) AS [Quantity]
				, [t9].[Id] AS [RepoItemId]
				,@CollectionReason AS [Comment]	
				, '' AS [DeliveryZone]
				, '' AS [ContactInfo]
				, getdATE() AS [OrderedOn]
				, 'false' AS [Damaged]	
				, 'false' AS [AssemblyReq]
				, 'false' AS [Express]
				, [t1].[acctno] AS [Reference]
				, (CASE 
					WHEN 1 = 1 THEN CONVERT(Decimal(33,4),[t1].[price])
					WHEN 0 = 1 THEN CONVERT(Decimal(33,4),0)
					WHEN ([t2].[RepUnitPrice]) IS NULL THEN CONVERT(Decimal(33,4),0)
					ELSE CONVERT(Decimal(33,4),[t2].[RepUnitPrice])
					END) AS [UnitPrice]
				, (CASE 
					WHEN 0 = 1 THEN 0
					ELSE [t6].[createdby]
					END) AS [CreatedBy], [t8].[Notes] AS [AddressNotes]
				, [t3].[StoreType] AS [Fascia]
				, 'true' AS [PickUp]
				, CONVERT(SmallInt,[t1].[SalesBrnNo]) AS [SalesBranch]
				,'RS1' AS [NonStockServiceType]
				,'RS2' AS [NonStockServiceItemNo]
				,'RS3' AS [NonStockServiceDescription]
			FROM [dbo].[LineItemBooking] AS [t0]
			INNER JOIN [dbo].[LineItem] AS [t1] ON [t0].[LineItemID] = [t1].[ID]
			INNER JOIN [dbo].[LineItemBookingSchedule] AS [t2] ON ([t0].[ID]) = [t2].[BookingId]
			INNER JOIN [dbo].[Branch] AS [t3] ON (
				(CASE 
					WHEN [t1].[SalesBrnNo] IS NOT NULL THEN [t1].[SalesBrnNo]
					ELSE CONVERT(Int,CONVERT(SmallInt,SUBSTRING([t1].[acctno], 0 + 1, 3)))
				 END)) = (CONVERT(Int,[t3].[branchno]))
			INNER JOIN [dbo].[CustAcct] AS [t4] ON [t1].[acctno] = [t4].[acctno]
			INNER JOIN [dbo].[Acct] AS [t5] ON [t4].[acctno] = [t5].[acctno]
			INNER JOIN [dbo].[Agreement] AS [t6] ON [t5].[acctno] = [t6].[acctno]
			INNER JOIN [dbo].[Customer] AS [t7] ON [t4].[custid] = [t7].[custid]
			INNER JOIN [dbo].[CustAddress] AS [t8] ON [t4].[custid] = [t8].[custid]
			INNER JOIN [dbo].[StockInfo] AS [t9] ON [t1].[ItemID] = [t9].[Id]
			LEFT OUTER JOIN [dbo].[CustTel] AS [t10] ON ([t10].[tellocn] = 'H') AND ([t10].[datediscon] IS NULL) AND ([t7].[custid] = [t10].[custid])
			LEFT OUTER JOIN [dbo].[CustTel] AS [t11] ON ([t11].[tellocn] = 'W') AND ([t11].[datediscon] IS NULL) AND ([t7].[custid] = [t11].[custid])
			LEFT OUTER JOIN [dbo].[CustTel] AS [t12] ON ([t12].[tellocn] = 'M') AND ([t12].[datediscon] IS NULL) AND ([t7].[custid] = [t12].[custid])
			WHERE ([t4].[hldorjnt] = 'H') AND ([t8].[addtype] = (
				(CASE 
					WHEN 0 = 1 THEN [t1].[deliveryaddress]
					ELSE CONVERT(NVarChar(MAX),'H')
				 END))) AND ([t8].[datemoved] IS NULL) AND (NOT ([t9].[RepossessedItem] = 1)) AND ([t0].[ID] = (SELECT TOP 1 ID FROM Warehouse.Booking WHERE itemno=@ItemNo AND AcctNo=@AccountNo AND DeliveryOrCollection='D'))
	
			FOR XML PATH ('BookingSubmit')
		)

		--SELECT @XML1;
		SELECT @XML1= (SELECT REPLACE(@XML1,'<BookingSubmit>','<BookingSubmit xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns="http://www.bluebridgeltd.com/cosacs/2012/schema.xsd">'))
		SELECT @XML1= (SELECT REPLACE(@XML1,'RS1','xsi:nil="true"'))
		SELECT @XML1= (SELECT REPLACE(@XML1,'RS2','xsi:nil="true"'))
		SELECT @XML1= (SELECT REPLACE(@XML1,'RS3','xsi:nil="true"'))
		--PRINT '----- START [Hub].[Message]----------------- '

		INSERT INTO [Hub].[Message]
			(CreatedOn, Body, CorrelationId, Routing, IsRouted)
			VALUES (@Date, @XML1, @AccountNo, 'warehouse.Booking.Submit', 0);

		--PRINT '----- END [Hub].[Message]----------------- '

		DECLARE @MessageId1 VARCHAR(50)
		SET @MessageId1=(SELECT MAX(id) FROM [Hub].[Message])
		exec sp_executesql N'
            INSERT INTO [Hub].[QueueMessage]
            (QueueId, CreatedOn, MessageId,Runcount,State)
            VALUES (@QueueId, @CreatedOn, @MessageId,1,''S'');
            ',N'@MessageId int,@QueueId int,@CreatedOn datetime',@MessageId=@MessageId1,@QueueId=1,@CreatedOn=@Date

		----added for Discount Issue(04/11/2019)

		IF(@ReturnType='Return')
		BEGIN
		Declare @NewQty FLOAT
		SET @NewQty=(SELECT (Quantity-CAST(@Quantity AS FLOAT)) FROM lineitem WHERE AcctNo=@AccountNo AND ItemID=@ProductId AND stocklocn=@Stocklocn)

		declare @p10 int
		set @p10=0
		--exec DN_LineItemUpdateQuantitySP @acctNo=@AccountNo,@itemID=@ProductId,@location=@Stocklocn,@newQty=0,@agreementno=1,@contractno=N'',@source=N'GRTCancel',@parentItemID=0,@user=10000,@return=@p10 output
		exec DN_LineItemUpdateQuantitySP @acctNo=@AccountNo,@itemID=@ProductId,@location=@Stocklocn,@newQty=@NewQty,@agreementno=1,@contractno=N'',@source=N'GRTCancel',@parentItemID=0,@user=@CoSaCSUserID,@return=@p10 output
		
		select @p10

		declare @p8000 int
		set @p8000=0
		declare @RetQty FLOAT 
		set @RetQty=(SELECT CAST(@Quantity AS FLOAT)*(-1))
		--exec DN_LineItemsUpdateDelQtySP @acctno=@AccountNo,@agreementno=1,@stocklocn=@Stocklocn,@itemID=@ProductId,@contractno=N'',@qty=-1,@parentItemID=0,@return=@p8000 output
		exec DN_LineItemsUpdateDelQtySP @acctno=@AccountNo,@agreementno=1,@stocklocn=@Stocklocn,@itemID=@ProductId,@contractno=N'',@qty=@RetQty,@parentItemID=0,@return=@p8000 output
		select @p8000		
		END
		----END added for Discount Issue(04/11/2019)

		declare @p23 int
		declare @Branch varchar(20)
		declare @BuffNo varchar(20)
		set @Branch=(SELECT SUBString(@AccountNo,0,4))
		set @BuffNo=(SELECT TOP 1 hibuffno from branch where branchno=@Branch)

		declare @p72 int
		set @p72=4007
		declare @p73 int
		set @p73=0

		Declare @Value Varchar(10)
		SELECT @Value=taxamt FROM
		VE_LineItem T0
		WHERE T0.acctno=@AccountNo ANd T0.itemno=@ItemNo AND T0.OrderNo=@OrderId

		set @p23=0
		declare @p20 int
		set @p20=0
		declare @pp2 int
		set @pp2=0
		declare @pp3 int
		set @pp3=0
		declare @p214 nchar(1)
		set @p214= 'N'
		declare @p113 int
		set @p113=0
		declare @pe4 money
		set @pe4=0.0000
		declare @pe6 int
		set @pe6=0
		declare @pq2 int
		set @pq2=0
	END

	FETCH NEXT FROM Cur_CustomerReturn     
	INTO 
			 @OrderId
			,@ReturnType
			,@CollectionReason
			,@ItemNo
			,@Quantity
			,@Stocklocn
			,@ReturnStocklocn
			,@TotalValue
			,@ProductId
			,@CoSaCSUserID        
	END     
	CLOSE Cur_CustomerReturn;    
	DEALLOCATE Cur_CustomerReturn; 
	SET  @Message = (select ID from Warehouse.Booking where ID = @CurrentBookingId)
	END
	
	IF (@@ERROR != 0)
		BEGIN
			ROLLBACK
			print'Transaction rolled back'
			SET @StatusCode = 500;		
		END
	ELSE
		BEGIN	
			print CONVERT(VARCHAR(3), @StatusCode)
			COMMIT
			print'Transaction committed'
			PRINT @Message
		END
	PRINT @Message
END