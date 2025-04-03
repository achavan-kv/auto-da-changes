
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[BookingFailuresView]'))
	DROP VIEW [dbo].[BookingFailuresView]
GO

CREATE VIEW BookingFailuresView 
-- ================================================      
-- Project      : CoSACS .NET      
-- File Name    : BookingFailuresView.sql      
-- File Type    : MSSQL Server Stored Procedure Script      
-- Title        :   
-- Author       : John Croft      
-- Date         : 07/06/12      
--      
-- Change Control      
-- --------------      
-- Date      By  Description      
-- ----      --  -----------      
-- 21/06/12 jec #10474 A new column del or coll should be available on failed Deliveries and collection screen
-- 23/05/13 jec #13604 Hub stops working when tried to complete immediate collection of a re-booked item
-- 04/10/13 ip  #15246 Prevent re-submitting an order on a cancelled account.
-- =================================================================================   

AS 

SELECT lbf.ID,lbf.BookingID,lbf.OriginalBookingID,l.quantity as OrderedQuantity, lbf.Quantity as FailedQty,
		-- #10474  
	    Case when ls.DelOrColl = 'D' then 'Delivery'
		     when ls.DelOrColl = 'C' then 'Collection'
		     when ls.DelOrColl = 'R' then 'Re-Delivery'
		     end as BookingType ,
		lbf.CancelReason,l.acctno, s.IUPC,s.itemdescr1,s.itemdescr2,rtrim(c.title)+' '+rtrim(c.firstname)+ ' ' +rtrim(c.name) as CustomerName,
		c.custid,lb.LineItemID,ISNULL(l.SalesBrnNo,LEFT(l.acctno,3)) as SalesBrnNo,Actioned, empeenosale as SalesPerson,
		ls.RetItemID,ls.RetVal,ls.RetStockLocn   -- #13604
			
FROM LineItemBookingFailures lbf INNER JOIN LineItemBooking lb ON lbf.OriginalBookingId = lb.ID
					INNER JOIN lineitem l ON lb.LineItemId= l.ID
					INNER JOIN StockInfo s on s.id=L.ItemID
					INNER JOIN Custacct ca on ca.acctno=l.acctno and ca.hldorjnt='H'
					INNER JOIN Customer c on ca.custid=c.custid
					INNER JOIN LineItemBookingSchedule ls ON ls.BookingId = lb.ID			-- #10474
					INNER JOIN agreement ag on l.acctno = ag.acctno 
WHERE NOT EXISTS (select * from cancellation c												-- #15246
					where c.acctno = l.acctno)

GO 

-- End End End End End End End End End End End End End End End End End End End End End End End End End End End End
