
/****** Object:  StoredProcedure [dbo].[DN_SRGetServiceRequestAuditSP]    Script Date: 11/03/2006 12:33:42 ******/
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID('[dbo].[DN_SRGetServiceRequestAuditSP]') AND OBJECTPROPERTY(id,'IsProcedure') = 1)
DROP PROCEDURE [dbo].[DN_SRGetServiceRequestAuditSP]

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Peter Chong
-- Create date: 03-11-2006
-- Description:	Get service request audit information for a service request no
-- Modified By: Jez Hemans
-- Modified Date: 18-07-2008
-- Description: UAT 339 Order of events changed to be according to the actual event rather than the date
-- =============================================
CREATE PROCEDURE DN_SRGetServiceRequestAuditSP  --124287 , 0
(
	@ServiceRequestNo int,
	@return int output
)
AS
BEGIN
	
	DECLARE @DateAllocated	datetime,
			@DateDeposit	datetime,	
			@DateLogged		datetime,
			@DatePayment	datetime,
			@DateResolved	datetime,
			@DateReopened	datetime,
			@DateClosed		datetime,
			@CustId			varchar(20),
			@ServiceType	char(1),
			@ServiceRequestNoStr int,
			@empNo			INT,
			@DepositAmount	money,
			@LastPaymentAmount money -- the last payment amount that was made to the charge account
			

	--Date Allocated--------------------------------------
		SELECT 
			@DateAllocated =  CASE WHEN DateAllocated = '1900-01-01' THEN NULL ELSE DateAllocated END
		FROM 
			SR_Allocation 
		WHERE 
			ServiceRequestNo = @ServiceRequestNo 
	
	--Date Deposit----------------------------------------	
		/*  Deposit auditing  removed for now at request of Nikki [PC 25/Jan/2007]
		SELECT 
			@DateDeposit = Min(datetrans),
			@empNo = empeeno, 
			@DepositAmount = isnull(Sum(TransValue), 0) 
		FROM 
			fintrans F JOIN 
			SR_ChargeAcct A		ON F.AcctNo = A.AcctNo JOIN 
			SR_ServiceRequest R ON R.ServiceRequestNo = A.ServiceRequestNo	
		WHERE 
			( SELECT -Sum(Transvalue) 
				FROM Fintrans 
				WHERE acctno = F.acctno 
					AND transTypecode = 'PAY' 
					AND dateTrans <= F.DateTrans  ) > R.DepositAmount
			AND F.transtypecode = 'PAY'
			AND R.ServiceRequestNo = @ServiceRequestNo 
			AND DepositAmount = -1 --
		GROUP BY empeeno
		*/

		
		
	
	--Date Logged, DateReopened, Customer ID-----------------------------	
		SELECT 
			@DateLogged = DateLogged,
			@DateReopened = CASE WHEN datereopened = '1900-01-01' THEN NULL ELSE datereopened END,
			@CustId = CustId,
			@ServiceType = ServiceType,
			@ServiceRequestNoStr =  dbo.fn_SRGetServiceRequestNo(ServiceRequestNo)
		FROM 
			SR_ServiceRequest 
		WHERE 
			ServiceRequestNo = @ServiceRequestNo
		
	--Date Payment
		SELECT 
			@DatePayment = Max(DateTrans)
		FROM fintrans F JOIN 
			SR_ChargeAcct A		ON F.AcctNo = A.AcctNo
			inner join sr_servicerequest s on a.servicerequestno = s.servicerequestno
		WHERE A.ServiceRequestNo = @ServiceRequestNo
			AND F.transtypecode = 'PAY'
			AND DateTrans > ISNULL(@DateDeposit, '1900-01-01')
			AND F.agrmtno like convert(varchar, s.servicebranchno) + '%' + convert(varchar, s.servicerequestno)
		
	--Date Resolved
		SELECT	
			@DateResolved = CASE WHEN DateClosed = '1900-01-01' THEN NULL ELSE DateClosed END
		FROM
			SR_Resolution
		WHERE 
			ServiceRequestNo = @ServiceRequestNo
	
	--Date Closed 
	--Date Closed should only be the Date Resolved if the charge to is not C or D or the charge account is settled
	IF EXISTS (SELECT ChargeType FROM SR_ChargeAcct WHERE ServiceRequestNo = @ServiceRequestNo AND ChargeType IN ('S','I','W'))
	BEGIN
		SELECT @DateClosed = @DateResolved
	END
	ELSE
	BEGIN 
		SELECT 
			@DateClosed = --ISNULL(
			max(dateTrans)--, @DateResolved) 
		FROM SR_ChargeAcct A JOIN 
			fintrans F			 ON F.AcctNo = A.AcctNo JOIN
			SR_ServiceRequest SR ON A.ServiceRequestNo = SR.ServiceRequestNo JOIN
			SR_Resolution R		 ON R.ServiceRequestNo = SR.ServiceRequestNo JOIN 
			Acct Ac				 ON Ac.AcctNo = A.AcctNo 
		WHERE A.ServiceRequestNo = @ServiceRequestNo 
			AND F.transtypecode = 'PAY'
			AND F.agrmtno like convert(varchar, sr.servicebranchno) + '%' + convert(varchar, sr.servicerequestno)
			AND DateTrans > isnull(@DateDeposit,  '1900-01-01')
			AND (A.ChargeType = 'D' OR A.ChargeType ='C')
			AND (Ac.CurrStatus = 'S' OR Ac.outstbal <= 0) -- Look for a settled account -- UAT 247 Possible, though highly unlikely, that account may not be settled due to over-payment
			AND SR.Status = 'C' -- UAT(5.2) - 713 , NM - 07/07/2009
	END

		
		SELECT @LastPaymentAmount = -Sum(Transvalue) 
		FROM  SR_ChargeAcct A JOIN 
			fintrans F			 ON F.AcctNo = A.AcctNo 
		inner join sr_servicerequest s on a.servicerequestno = s.servicerequestno
		WHERE A.ServiceRequestNo = @ServiceRequestNo
			AND F.transtypecode = 'PAY'
			AND F.agrmtno like convert(varchar, s.servicebranchno) + '%' + convert(varchar, s.servicerequestno)
			AND datetrans = @DateClosed
		
		
		SELECT @DateAllocated	[DateAllocated],
			@DateDeposit		[DateDeposit],	
			@DateLogged			[DateLogged],
			@DatePayment		[DatePayment],
			@DateResolved		[DateResolved],
			@DateReopened		[DateReopened],
			@ServiceType		[ServiceType],
			@CustId				[CustId],
			@ServiceRequestNoStr [ServiceRequestNoStr],
			@DateClosed			[DateClosed]
			
		
				--Open
		SELECT 1 [Order],'Opened' [Event], DateLogged [Date], 'Logged By ' + u.FullName [Description],LoggedBy AS EmpeeNo 
		FROM SR_ServiceRequest 
		INNER JOIN Admin.[User] u ON SR_ServiceRequest.LoggedBy = u.id
		WHERE ServiceRequestNo = @ServiceRequestNo
		
		UNION 	--Customer Update
		SELECT 2,'Customer Updated', DateLogged ,  convert(varchar(16), LoggedBy),LoggedBy 
		FROM SR_ServiceRequest WHERE 0 = 1
		
		UNION 	--Update
		SELECT 3,'Updated', DateLogged ,  convert(varchar(16), LoggedBy),LoggedBy 
		FROM SR_ServiceRequest WHERE 0 = 1
		
		UNION 	--Update
		SELECT 4,'Soft Scripts Completed', DateLogged ,  'Logged By ' + FullName ,LoggedBy 
		FROM SR_Allocation JOIN SR_ServiceRequest ON SR_Allocation.ServiceRequestNo = SR_ServiceRequest.ServiceRequestNo
		INNER JOIN Admin.[User] u ON SR_ServiceRequest.LoggedBy = u.Id
		WHERE SR_Allocation.ServiceRequestNo = @ServiceRequestNo
		
		UNION   --Allocation 
		SELECT 5,'Allocated', @DateAllocated, 'Technician: ' + T.FirstName + ' ' + T.LastName + '   ' + 'Estimate: ' +  convert(varchar(16), SR.RepairEstimate),a.allocatedBy
			FROM SR_Allocation A JOIN  
				SR_Technician T ON A.TechnicianId = T.TechnicianId JOIN
				SR_ServiceRequest SR ON A.ServiceRequestNo = SR.ServiceRequestNo
			WHERE A.ServiceRequestNo = @ServiceRequestNo AND @DateAllocated IS NOT NULL
		
		UNION  --Pay Deposit
		SELECT DISTINCT 6,'Paided Deposit', 
			@DateDeposit,  
			'Deposit Paid: ' + convert(varchar, @DepositAmount) + '.  Acc No: ' + A.AcctNo ,
			@empNo 
		FROM SR_ServiceRequest SR JOIN SR_ChargeAcct A  ON SR.ServiceRequestNo = A.ServiceRequestNo
		WHERE SR.ServiceRequestNo = @ServiceRequestNo AND @DateDeposit IS NOT NULL

		UNION  --Resolve
		SELECT 7,'Resolved', @DateResolved, Co.CodeDescript   + '.  Cost: ' + convert(varchar(20), TotalCost) ,case when ResolutionChangedBy = 0 then SR.LoggedBy else ResolutionChangedBy end
		FROM SR_Resolution R INNER JOIN 
		code Co ON Co.Category = 'SRRESOLVE' AND R.Resolution = Co.Code  INNER JOIN
		SR_ServiceRequest SR ON R.ServiceRequestNo = SR.ServiceRequestNo
		WHERE R.ServiceRequestNo = @ServiceRequestNo

		UNION  --Payment
		SELECT 8,'Payment', DateTrans, 'Payment Made. Amount: ' + convert(varchar, -Transvalue) + '.  Acc No: ' + A.AcctNo ,empeeno
		FROM fintrans F JOIN 
			SR_ChargeAcct A		ON F.AcctNo = A.AcctNo
			inner join sr_servicerequest s on a.servicerequestno = s.servicerequestno
			AND F.agrmtno like convert(varchar, s.servicebranchno) + '%' + convert(varchar, s.servicerequestno)
		WHERE A.ServiceRequestNo = @ServiceRequestNo
			AND F.transtypecode = 'PAY' 
			AND DateTrans > ISNULL(@DateDeposit, '1900-01-01')


		UNION  -- Reopen
		SELECT 9,'Reopened', @DateReopened,  convert(varchar(16), LoggedBy) ,LoggedBy
		FROM 
			SR_ServiceRequest 
		WHERE ServiceRequestNo = @ServiceRequestNo AND @DateReopened IS NOT NULL
		
		UNION -- Closed
		
		SELECT 10,'Closed', 
			@DateClosed,
			'Acc No: ' + C.AcctNo + case when @LastPaymentAmount IS NULL then '' else  ' Last Payment: ' +  convert(varchar, @LastPaymentAmount)  end,
			LoggedBy 
		FROM SR_ChargeAcct  C INNER JOIN SR_ServiceRequest SR ON C.ServiceRequestNo = SR.ServiceRequestNo
		WHERE  
			C.ServiceRequestNo = @ServiceRequestNo AND 
			@DateClosed IS NOT NULL --AND
			--(C.ChargeType = 'D' OR C.ChargeType ='C') 
			--AND
            --SR.Status = 'C'
			
		
		--ORDER BY [Date] 
		ORDER BY [Order]

SET @return  = @@error
	
	
END
GO
