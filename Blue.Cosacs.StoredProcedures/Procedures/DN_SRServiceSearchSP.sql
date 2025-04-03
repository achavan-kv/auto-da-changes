
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID('[dbo].[DN_SRServiceSearchSP]') AND OBJECTPROPERTY(id,'IsProcedure') = 1)
DROP PROCEDURE [dbo].[DN_SRServiceSearchSP]

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Peter Chong
-- Create date: 19-Oct-2006
-- Description:	Searches for a service request
-- Modified By:	Jez Hemans
-- Modified For:CR 949/958 New service type Cash & Go ('G') added for SRs	
-- =============================================
CREATE PROCEDURE DN_SRServiceSearchSP
-- ================================================
-- Project      : CoSACS .NET
-- File Name    : DN_SRServiceSearchSP.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Searches for a service request 
-- Author       : Peter Chong
-- Date         : 19-Oct-2006
--
-- This procedure will retrieve the Searches for a service request.

-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 05/11/10 jec CR1030 Include Customer Name in Search
-- 01/07/11 jec CR1254 IUPC and CourtsCode will be shown on the service request search screen
-- ================================================
	-- Add the parameters for the stored procedure here
(
	@ServiceRequestNo int = -1, 
	@CustId varchar(20) = '',
	@FirstName varchar(20) = '',		--CR1030 jec	
	@LastName varchar(20) = '',			--CR1030 jec
	@ShowCourts bit,
	@ShowNonCourts bit,
	@ShowInternal bit,
	@ShowCashGo BIT,
	@ExactMatch bit,
	@SearchLimit int ,
	@ServiceRequestNoStr varchar(16) = null,
	@return int output
)
AS
BEGIN
	SET NOCOUNT ON;

	SET ROWCOUNT @SearchLimit
    IF @ServiceRequestNo <= 0 SET @ServiceRequestNo = -1
	
	-- Insert statements for procedure here
	SELECT DISTINCT SR.ServiceRequestNo 
		,  convert(varchar(4), ServiceBranchNo) + convert(varchar(16), SR.ServiceRequestNo) [ServiceRequestNoStr]
		,  SR.CustId			[Customer Id]		-- CR1030
		,  isnull(CA.AcctNo,'')		[ChargeAcct]
		,  SR.AcctNo		
		,  DateLogged		[Date SR Logged]
		,  LoggedBy			[Logged By]
		,  Status
		--,  ProductCode		[Product Code]
		,  ISNULL(si.IUPC,ProductCode)		[Product Code]				-- RI
		,  ISNULL(si.ItemNo,'')		[CourtsCode]				-- RI
		,  Description		[Product Description]
		,  D.BuffNo			[Delivery Note Number] 
		,  D.DateDel			[Delivery Date]
		,  ExtWarranty      [Extended Warranty]
		,  Sr.ContractNo		[Contract Number]
		,  ServiceType
		,  History          [Historical]		--UAT 398 Identify historical SRs
		,  SR.InvoiceNo		[Invoice No]	-- UAT 598 see invoice number
		,  FirstName, Name  as LastName				-- CR1030 jec
	FROM SR_ServiceRequest SR  LEFT OUTER JOIN 
		SR_ChargeAcct CA ON CA.ServiceRequestNo = SR.ServiceRequestNo LEFT OUTER JOIN 
		acct A ON A.Acctno = CA.AcctNo AND A.AcctType = 'C' LEFT OUTER JOIN 
		Delivery D ON   sr.AcctNo = D.AcctNo  
			AND     sr.InvoiceNo = D.AgrmtNo  
			AND     sr.StockLocn = D.StockLocn  
			AND     sr.ProductCode = D.ItemNo  
			AND		D.DelorColl = 'D' 		and d.datetrans=
		(select max(datetrans) from delivery w
		where  sr.AcctNo = w.AcctNo  
			AND     sr.InvoiceNo = w.AgrmtNo  
			AND     sr.StockLocn = w.StockLocn  
			AND     sr.ProductCode = w.ItemNo  
			AND		w.DelorColl = 'D')
		LEFT outer join Customer C on sr.custid=c.custid		-- CR1030
		LEFT OUTER JOIN Stockinfo si on sr.ItemID=si.ID		-- RI
		WHERE 
		--Service Request No. filter
		(
			(@ExactMatch = 0 AND @ServiceRequestNo = -1) OR 
			(@ExactMatch = 1 AND ((SR.ServiceRequestNo = @ServiceRequestNo OR @CustId <> '') or (@FirstName + @LastName!='')) ) OR	--CR1030 jec
			(@ExactMatch = 0 AND dbo.fn_SRGetServiceRequestNo(SR.ServiceRequestNo) LIKE '%' + @ServiceRequestNoStr + '%')
		) 
		AND
		--Customer filter
		(
			(@ExactMatch = 0 AND @CustId = '') OR 
			(@ExactMatch = 1 AND  ((SR.CustId = @CustId OR @ServiceRequestNo <> -1) or (@FirstName + @LastName!='' and @CustId='')) ) OR	-- CR1030
			(@ExactMatch = 0 AND SR.CustId LIKE @CustId + '%')						-- CR1030
		) AND
		--Service type filter 
		ServiceType IN (
						  (SELECT CASE WHEN @ShowCourts = 1 THEN 'C' ELSE NULL END)
						, (SELECT CASE WHEN @ShowNonCourts = 1 THEN 'N' ELSE NULL END)
						, (SELECT CASE WHEN @ShowInternal = 1 THEN 'S' ELSE NULL END)
						, (SELECT CASE WHEN @ShowCashGo = 1 THEN 'G' ELSE NULL END)  
						)
		--Customer Name filter
		AND  ((@ExactMatch = 1 and (SR.custid=c.custid or @custid='')	--CR1030 jec
			and (
				((FirstName=@FirstName and @FirstName!='') or (@FirstName=''))	--CR1030 jec
					and ((Name=@LastName and @LastName!='') or (@LastName=''))
			)
			)
			or (@ExactMatch = 0 and SR.custid=c.custid 
			and (((FirstName like @FirstName + '%' and @FirstName!='') or @FirstName='') and ((Name like @LastName + '%' and @LastName!='') or @LastName=''))
			)
			or @FirstName + @LastName=''
			)
	ORDER BY SR.ServiceRequestNo DESC
	SET @return = @@error
END
GO

-- End End End End End End End End End End End End End End End End End End End End End End End End End End


