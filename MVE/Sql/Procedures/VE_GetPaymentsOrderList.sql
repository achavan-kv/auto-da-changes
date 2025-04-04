 
if exists (select * from dbo.sysobjects where id = object_id('[dbo].[VE_GetPaymentsOrderList]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
BEGIN
	DROP PROCEDURE [dbo].[VE_GetPaymentsOrderList]
END
GO

create PROCEDURE [dbo].[VE_GetPaymentsOrderList]
	  @acctno varchar(12)
	, @Id VARCHAR(20) = N''
	, @Message varchar(MAX) output
	, @StatusCode varchar(5) output
AS
BEGIN
BEGIN TRY
	SET NOCOUNT ON;
	Declare @Accounttype varchar(10)
	Declare @DTPercentage numeric(19,6)
	Declare @DTEligible numeric(19,6)
	SET @Message = ''
	SET @StatusCode = ''
	
	IF NOT EXISTS (select 1 from custacct where acctno=@acctno)
	BEGIN
		SET @Message = 'Account not found'
		SET @StatusCode = '404'
		RETURN		
	END
	ELSE
	BEGIN
	SELECT @Accounttype=accttype from acct where acctno=@acctno
	SELECT @DTPercentage=Value from [dbo].[CountryMaintenance] WHERE CodeName='globdelpcent'
	
	IF(@Accounttype NOT LIKE '%C%')
	BEGIN
	SELECT @DTEligible= 1 
	WHERE
	(	(
			(
				SELECT 
					SUM(Price+taxamt) 
				FROM VE_LineItem T0
				WHERE 
				acctno=@acctno
				AND IsDeliver='1' 
				GROUP BY acctno
			)
			/
			(
				SELECT SUM(Price+taxamt)
				FROM VE_LineItem
				WHERE acctno=@acctno
			)
		)*100
	)>=@DTPercentage
	END
	
	IF(@Accounttype LIKE '%C%')
	BEGIN

	SELECT   DISTINCT
		@Id as 'ExternalPaymentId',
		@Id as 'CheckOutId',
		CASE WHEN ISNULL(ExchgAmtDiff,0) >= 0 THEN 'Order' ELSE 'Refund' END as 'PaymentType',
		'' as AdjustmentType ,
		agr.dateauth as 'PaymentDate',
		isnull(l.MVEUserID,1)as 'EmployeeId',
		--'1' as 'EmployeeId',
		'Cash' as 'PaymentMethod' ,
		ft.ChequeNo as 'ChequeNumber',
		l.OrderNo as 'OrderId',
		li.itemno as 'ExternalItemNo',
		CASE 
			WHEN ISNULL(l.ExchgAmtDiff,0.0)=0.0 THEN 
			--Changes are make for Exclusive and Inclusive
			--((li.price * l.quantity + li.taxamt )+ISNULL(l.discount* l.quantity,0))
			CASE WHEN (select Value from CountryMaintenance where CodeName ='agrmttaxtype') = 'E' THEN
					((li.price * l.quantity + li.taxamt )+ISNULL(l.discount* l.quantity,0))
					ELSE 
					((li.price * l.quantity )+ISNULL(l.discount* l.quantity,0))
			END
			WHEN ISNULL(l.ExchgAmtDiff,0.0)!=0.0 THEN l.ExchgAmtDiff
		END
		as 'Amount'
	FROM 
	VE_LineItem l 	
	INNER JOIN fintrans ft on ft.acctno=l.acctno
	INNER JOIN Agreement agr on agr.acctno=l.acctno
	INNER JOIN lineitem li on li.acctno=l.acctno and li.ItemID=l.ItemID and li.stocklocn=l.stocklocn
	WHERE l.ACCTNO=@acctno 
		AND li.itemno not in ('DT','STAX')
		AND ft.transtypecode IN ('PAY')
		AND ISNULL(l.IsDeliver,'0')='1' 
		AND ISNULL(l.OldCheckOutId,l.CheckOutId)=@Id
		AND ISNULL(l.ShipmentCancel,'0')='0'
	
	UNION ALL

	SELECT   DISTINCT
		@Id as 'ExternalPaymentId',
		@Id as 'CheckOutId',
		CASE WHEN ISNULL(ExchgAmtDiff,0) >= 0 THEN 'Order' ELSE 'Refund' END as 'PaymentType',
		'' as AdjustmentType ,
		agr.dateauth as 'PaymentDate',
		isnull(l.MVEUserID,1)as 'EmployeeId',
		--'1' as 'EmployeeId',
		'Cash' as 'PaymentMethod' ,
		ft.ChequeNo as 'ChequeNumber',
		l.OrderNo as 'OrderId',
		li.itemno as 'ExternalItemNo',
		CASE 
			WHEN ISNULL(l.ExchgAmtDiff,0.0)=0.0 THEN 
			--Changes are make for Exclusive and Inclusive
			--((li.price * l.quantity + li.taxamt )+ISNULL(l.discount* l.quantity,0))
			CASE WHEN (select Value from CountryMaintenance where CodeName ='agrmttaxtype') = 'E' THEN
					((li.price * l.quantity + li.taxamt )+ISNULL(l.discount* l.quantity,0))
					ELSE 
					((li.price * l.quantity )+ISNULL(l.discount* l.quantity,0))
			END
			
		
			WHEN ISNULL(l.ExchgAmtDiff,0.0)!=0.0 THEN l.ExchgAmtDiff
		END
		as 'Amount'
	FROM 
	VE_LineItem l 	
	INNER JOIN fintrans ft on ft.acctno=l.acctno
	INNER JOIN Agreement agr on agr.acctno=l.acctno
	INNER JOIN lineitem li on li.acctno=l.acctno and li.ItemID=l.ItemID and li.stocklocn=l.stocklocn
	WHERE l.ACCTNO=@acctno 
		AND li.itemno not in ('DT','STAX')
		AND ft.transtypecode IN ('XFR')
		AND ISNULL(l.IsDeliver,'0')='1' 
		AND l.CheckOutId=@Id
		AND ISNULL(l.ShipmentCancel,'0')='0'
		AND ISNULL(l.IsSync,'0')='0'
	UNION ALL

		SELECT   DISTINCT
		@Id as 'ExternalPaymentId',
		@Id as 'CheckOutId',
		'Order' as 'PaymentType',
		'' as AdjustmentType ,
		agr.dateauth as 'PaymentDate',
		--'1' as 'EmployeeId',
		isnull(l.MVEUserID,1)as 'EmployeeId',
		'Cash' as 'PaymentMethod' ,
		ft.ChequeNo as 'ChequeNumber',
		l.OrderNo as 'OrderId',
		li.itemno as 'ExternalItemNo',
		0 as 'Amount'
	FROM 
	VE_LineItem l 	
	INNER JOIN fintrans ft on ft.acctno=l.acctno
	INNER JOIN Agreement agr on agr.acctno=l.acctno
	INNER JOIN lineitem li on li.acctno=l.acctno and li.ItemID=l.ItemID and li.stocklocn=l.stocklocn
	WHERE l.ACCTNO=@acctno 
		AND li.itemno not in ('DT','STAX')
		AND ft.transtypecode IN ('PAY')
		AND ISNULL(l.IsDeliver,'0')='1' 
		AND ISNULL(l.OldCheckOutId,l.CheckOutId)=@Id  
		AND ISNULL(l.ShipmentCancel,'0')='1'

	UNION ALL

		SELECT   DISTINCT
		@Id as 'ExternalPaymentId',
		@Id as 'CheckOutId',
		'Order' as 'PaymentType',
		'' as AdjustmentType ,
		agr.dateauth as 'PaymentDate',
		--'1' as 'EmployeeId',
		isnull(l.MVEUserID,1)as 'EmployeeId',
		'Cash' as 'PaymentMethod' ,
		ft.ChequeNo as 'ChequeNumber',
		l.OrderNo as 'OrderId',
		li.itemno as 'ExternalItemNo',
		0 as 'Amount'
	FROM 
	VE_LineItem l 	
	INNER JOIN fintrans ft on ft.acctno=l.acctno AND ft.transtypecode=l.PaymentType
	INNER JOIN Agreement agr on agr.acctno=l.acctno
	INNER JOIN lineitem li on li.acctno=l.acctno and li.ItemID=l.ItemID and li.stocklocn=l.stocklocn
	WHERE l.ACCTNO=@acctno 
		AND li.itemno not in ('DT','STAX')
		AND ft.transtypecode IN ('REF','XFR')
		AND ISNULL(l.IsDeliver,'0')='1' 
		AND ISNULL(l.OldCheckOutId,l.CheckOutId)=@Id  
		AND ISNULL(l.ShipmentCancel,'0')='1'

	END
	
	ELSE IF(@Accounttype LIKE '%R%')
	BEGIN
	
	SELECT  DISTINCT
		@Id as 'ExternalPaymentId',
		@Id as 'CheckOutId',
		CASE WHEN ISNULL(ExchgAmtDiff,0)<0 THEN 'Refund' ELSE 'Order' END as 'PaymentType',
		'' as AdjustmentType,
		agr.dateagrmt as 'PaymentDate',
		--'1' as 'EmployeeId',
		isnull(l.MVEUserID,1)as 'EmployeeId',
		'Credit' as 'PaymentMethod' ,
		'' as 'ChequeNumber',
		l.OrderNo as 'OrderId',
		li.itemno as 'ExternalItemNo',
		CASE 
			WHEN ISNULL(ExchgAmtDiff,0.0)=0.0 THEN 
			--Changes are make for Exclusive and Inclusive
			--((li.price * l.quantity + li.taxamt )+ISNULL(l.discount* l.quantity,0))
				CASE WHEN (select Value from CountryMaintenance where CodeName ='agrmttaxtype') = 'E' THEN
					((li.price * l.quantity + li.taxamt )+ISNULL(l.discount* l.quantity,0))
					ELSE 
					((li.price * l.quantity )+ISNULL(l.discount* l.quantity,0))
			END
			WHEN ISNULL(ExchgAmtDiff,0.0)!=0.0 THEN ExchgAmtDiff
		END
		as 'Amount'
	FROM 
	 VE_LineItem l  	
	INNER JOIN Agreement agr on agr.acctno=l.acctno
	INNER JOIN lineitem li on li.acctno=l.acctno and li.ItemID=l.ItemID and li.stocklocn=l.stocklocn
	WHERE l.ACCTNO=@acctno
		AND li.itemno not in ('DT','STAX')
		AND ISNULL(l.IsDeliver,'0')='1' 
		AND ISNULL(l.OldCheckOutId,l.CheckOutId)=@Id
		AND ISNULL(l.ShipmentCancel,'false')='false'
	UNION ALL

	SELECT DISTINCT A.* FROM(
		SELECT TOP 1
		@Id AS 'ExternalPaymentId',
		@Id AS 'CheckOutId',
		CASE WHEN ISNULL(ExchgAmtDiff,0)<0 THEN 'Refund' ELSE 'Order' END as 'PaymentType',
		'' as AdjustmentType ,
		agr.dateagrmt as 'PaymentDate',
		--'1' as 'EmployeeId',
		isnull(T0.MVEUserID,1)as 'EmployeeId',
		'Credit' as 'PaymentMethod' ,
		'' as 'ChequeNumber',
		T0.OrderNo as 'OrderId',
		li.itemno as 'ExternalItemNo',
		--Changes are make for Exclusive and Inclusive
		--((li.price * T0.quantity + li.taxamt )+ISNULL(T0.discount* T0.quantity,0)) as 'Amount'
			CASE WHEN (select Value from CountryMaintenance where CodeName ='agrmttaxtype') = 'E' THEN
					((li.price * T0.quantity + li.taxamt )+ISNULL(T0.discount* T0.quantity,0))
					ELSE 
					((li.price * T0.quantity)+ISNULL(T0.discount* T0.quantity,0)) 
			END as 'Amount'
		FROM VE_LineItem T0 
		INNER JOIN  Agreement agr on agr.acctno=T0.acctno
		inner join lineitem li on li.acctno=T0.acctno and li.ItemId=T0.ItemId and li.stocklocn=T0.stocklocn
		inner join Warehouse.Booking bo ON T0.acctno=bo.AcctNo
		WHERE @DTEligible='1' 
			AND li.itemno='DT' 
			AND T0.acctno=@acctno
			AND ISNULL(bo.DeliveryRejected,'false')!='true'
			AND T0.CheckOutId=@Id
			AND ISNULL(T0.OldCheckOutId,'')=''
			AND ISNULL(T0.ShipmentCancel,'false')='false'
		ORDER BY T0.OrderNo DESC
	) A

	UNION ALL

	SELECT DISTINCT A.* FROM(
		SELECT TOP 1
		@Id AS 'ExternalPaymentId',
		@Id AS 'CheckOutId',
		CASE WHEN ISNULL((
			SELECT Transvalue 
			FROM Delivery
			WHERE transrefno=(
				SELECT TOP 1 T1.transrefno 
				FROM VE_LineItem T0
				INNER JOIN Delivery T1 ON T0.acctno=T1.acctno AND T0.ItemNo=T1.ItemNo AND T1.delorcoll='D'
				WHERE T0.acctno=@acctno
				AND T0.Orderno=
				(
					SELECT DISTINCT T0.OrderNo
					FROM VE_LineItem T0
					WHERE T0.acctno=@acctno
						AND T0.OldCheckOutId=@Id
						AND T0.ItemNo='DT'
				)
				AND T0.ItemNo='DT'
				ORDER BY transrefno DESC
				) AND acctno=@acctno AND ItemNo='DT' AND delorcoll='D'
		),0)<0 THEN 'Refund' ELSE 'Order' END as 'PaymentType',
		'' as AdjustmentType ,
		agr.dateagrmt as 'PaymentDate',
		--'1' as 'EmployeeId',
		isnull(T0.MVEUserID,1)as 'EmployeeId',
		'Credit' as 'PaymentMethod' ,
		'' as 'ChequeNumber',
		T0.OrderNo as 'OrderId',
		T0.itemno as 'ExternalItemNo',
		(
			SELECT Transvalue 
			FROM Delivery
			WHERE transrefno=(
				SELECT TOP 1 T1.transrefno 
				FROM VE_LineItem T0
				INNER JOIN Delivery T1 ON T0.acctno=T1.acctno AND T0.ItemNo=T1.ItemNo AND T1.delorcoll='D'
				WHERE T0.acctno=@acctno
				AND T0.Orderno=
				(
					SELECT DISTINCT T0.OrderNo 
					FROM VE_LineItem T0
					WHERE T0.acctno=@acctno
						AND T0.OldCheckOutId=@Id
						AND T0.ItemNo='DT'
				)
				AND T0.ItemNo='DT'
				ORDER BY transrefno DESC
				) AND acctno=@acctno AND ItemNo='DT' AND delorcoll='D'
		) as 'Amount'
	FROM VE_LineItem T0 
	INNER JOIN  Agreement agr on agr.acctno=T0.acctno
		WHERE T0.itemno='DT' 
			AND T0.acctno=@acctno
			AND T0.OldCheckOutId=@Id
			AND T0.ItemNo!='ADMIN'
			AND ISNULL(T0.ShipmentCancel,'false')='false'
		ORDER BY T0.OrderNo DESC
	) A

	UNION ALL

		SELECT   DISTINCT
		@Id as 'ExternalPaymentId',
		@Id as 'CheckOutId',
		'Order' as 'PaymentType',
		'' as AdjustmentType ,
		agr.dateauth as 'PaymentDate',
		--'1' as 'EmployeeId',
		isnull(l.MVEUserID,1)as 'EmployeeId',
		'Cash' as 'PaymentMethod' ,
		ft.ChequeNo as 'ChequeNumber',
		l.OrderNo as 'OrderId',
		li.itemno as 'ExternalItemNo',
		0 as 'Amount'
	FROM 
	VE_LineItem l 	
	INNER JOIN fintrans ft on ft.acctno=l.acctno
	INNER JOIN Agreement agr on agr.acctno=l.acctno
	INNER JOIN lineitem li on li.acctno=l.acctno and li.ItemID=l.ItemID and li.stocklocn=l.stocklocn
	WHERE l.ACCTNO=@acctno 
		AND li.itemno not in ('DT','STAX')
		AND ft.transtypecode IN ('PAY','XFR')
		AND ISNULL(l.IsDeliver,'0')='1' 
		AND ISNULL(l.OldCheckOutId,l.CheckOutId)=@Id  
		AND ISNULL(l.ShipmentCancel,'0')='1'

UNION ALL

		SELECT   DISTINCT
		@Id as 'ExternalPaymentId',
		@Id as 'CheckOutId',
		'Order' as 'PaymentType',
		'' as AdjustmentType ,
		agr.dateauth as 'PaymentDate',
		--'1' as 'EmployeeId',
		isnull(l.MVEUserID,1)as 'EmployeeId',
		'Cash' as 'PaymentMethod' ,
		ft.ChequeNo as 'ChequeNumber',
		l.OrderNo as 'OrderId',
		li.itemno as 'ExternalItemNo',
		0 as 'Amount'
	FROM 
	VE_LineItem l 	
	INNER JOIN fintrans ft on ft.acctno=l.acctno AND ft.transtypecode=l.PaymentType
	INNER JOIN Agreement agr on agr.acctno=l.acctno
	INNER JOIN lineitem li on li.acctno=l.acctno and li.ItemID=l.ItemID and li.stocklocn=l.stocklocn
	WHERE l.ACCTNO=@acctno 
		AND li.itemno not in ('DT','STAX')
		AND ft.transtypecode IN ('REF','XFR')
		AND ISNULL(l.IsDeliver,'0')='1' 
		AND ISNULL(l.OldCheckOutId,l.CheckOutId)=@Id  
		AND ISNULL(l.ShipmentCancel,'0')='1'
	END

	SET @Message = 'Payment Details found'
		SET @StatusCode = '200'
	END
	IF (@@error != 0)
              BEGIN
                     print'Transaction rolled back'
                  SET @StatusCode = 500;           
              END
              ELSE
              BEGIN
                     SET @StatusCode = 200;         
                     SET @Message = 'Payment Details found succesfully.'
                     print'Transaction committed'
              END
       END TRY 
              BEGIN CATCH 
                     IF (@@error > 0)
              SET @StatusCode = 500;         
              SET @Message = ERROR_Message()
      
       END CATCH
END
	print @Message