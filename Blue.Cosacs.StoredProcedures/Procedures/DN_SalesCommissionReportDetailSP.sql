

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID('[dbo].[DN_SalesCommissionReportDetailSP]') AND OBJECTPROPERTY(id,'IsProcedure') = 1)
DROP PROCEDURE [dbo].[DN_SalesCommissionReportDetailSP]
GO

-- =============================================
-- Author:		Peter Chong
-- Create date: 2006-Dec-19
-- Description:	Get commission report detail information
-- Modification:
--				2006-Jan-22: Negative valued DT items are cancellations [PC]
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 06/07/07 Jec Use NetCommissionValue for commission % calc
-- 03/07/08 jec Calc Commission % excluding uplift% UAT441
-- 03/07/08 jec Show Delivery Amount net of taxes UAT443
-- =============================================
CREATE PROCEDURE DN_SalesCommissionReportDetailSP  --2004, '2007-01-01', '2007-01-22', 1 ,1 ,1 
(	-- Add the parameters for the stored procedure here
	
	@empeeno int,
	@FromDate datetime,
	@ToDate datetime,
	@ShowCommission bit,
	@ShowSPIFF bit,
	@return int output
)
AS

SELECT   
   
  S.Rundate [Rundate]  
, S.Acctno [AcctNo]  
, S.AgrmtNo [InvoiceNo]  
, case 
	when D.itemno = 'RB'	then 'REB'  
	when D.DelorColl = 'C' OR (S.ItemNo = 'DT' AND S.CommissionAmount < 0)	then 'CAN' 
	when D.DelorColl = 'D'	then 'DEL' 
	when D.DelorColl = 'R'	then 'R' 
	else ''  
  end  [TransTypeCode]
, isnull(C.codedescript, '') [CommissionType]  
, S.ItemNo [Itemno]  
, D.StockLocn [StockLocn]  
, S.CommissionAmount [CommissionAmount]  
--, round(isnull(S.CommissionAmount /nullif(D.TransValue, 0), 0), 3)   [commnpercent]
, case					-- jec 06/07/07
	when s.netcommissionvalue != 0 then
		round(isnull(S.CommissionAmount /nullif(s.netcommissionvalue, 0), 0), 3)		
	else
		round(isnull(S.CommissionAmount /(1.00+ (UpliftRate/100)) /nullif(D.TransValue-cast(l.taxamt as decimal(9,2)) , 0), 0), 3) 
	end as  [commnpercent]
-- Uplift Commission Rate  - jec 22/06/07  CR36 Enhancements
, UpliftRate/100.00  as 'Uplift Commission %Rate'  
, D.TransValue -l.taxamt [DeliveryAmount]	-- jec03/07/08  UAT443

FROM   
 Delivery D INNER JOIN  
 Agreement A		ON D.AcctNo = A.AcctNo AND D.AgrmtNo = A.AgrmtNo 
 INNER JOIN Admin.[User] u ON A.Empeenosale = u.id 
 INNER JOIN
 SalesCommission S	ON  
	S.Employee		= A.Empeenosale   
	AND S.Acctno	=  A.Acctno   
	AND S.AgrmtNo	= A.AgrmtNo 
	AND S.ItemNo	= D.Itemno
	AND S.StockLocn = D.StockLocn	
	AND S.Buffno	= D.BuffNo LEFT OUTER JOIN   
 Code C ON  C.Code	= S.CommissionType AND C.category = 'CTP' 
 inner join lineItem l			-- required for taxamt jec 03/07/08
					on l.AcctNo=d.AcctNo and 
						l.itemno=d.ItemNo and 
						l.AgrmtNo=d.AgrmtNo and 					
						l.StockLocn=d.StockLocn 
 WHERE 
	A.Empeenosale = @empeeno AND  
	D.DateTrans BETWEEN  @FromDate AND @ToDate   AND 
	(
		(@ShowCommission = 1 AND S.CommissionType  NOT IN	('SP', 'LS', 'ES')) OR
		(@ShowSPIFF = 1		 AND S.CommissionType  IN		('SP', 'LS', 'ES'))
	)

 Order by
	S.Rundate, S.Acctno
 
  
SET @return = @@error  

-- End End End End End End End End End End End End End End End End End End End End End End End End End End End End 

