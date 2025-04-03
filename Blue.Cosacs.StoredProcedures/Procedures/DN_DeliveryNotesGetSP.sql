--new columns for lineitem deliveryarea, 
if  exists (select * from sysobjects where name =  'DN_DeliveryNotesGetSP' ) 
drop procedure dbo.DN_DeliveryNotesGetSP 
go
CREATE PROCEDURE dbo.DN_DeliveryNotesGetSP 	
/***********************************************************************************************************
--
-- Project      : CoSACS .NET
-- File Name    : DN_DeliveryNotesGetSP.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Description  : 
--				
-- Author       : ?
-- Date         : ?
--
--  
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 06/06/11  IP  CR1212 - RI - #3806 - join on ItemID
-- 28/06/12 jec #10544 Warehouse - Cancel Collection Notes
-- 21/01/13  IP #12003 - LW75608 - Include items scheduled for "Immediate" delivery
-- 29/05/13  IP #13697 - Quantities not correctly displayed on Cancel Collection Notes Screen
-- 29/05/13  JC #13696 - Outstanding exception when Collection Notes Cancelled
-- 01/10/13  IP #15054 - Only display in Cancel Collection Notes if in Failed Deliveries / Collections Screen.
************************************************************************************************************/
   @acctno varchar(13),
   @user INT, --current user
   @collectionsOnly BIT,
   @TimeLocked DATETIME OUTPUT, 
   @return INT OUTPUT 
AS
   select @TimeLocked = GetDate()
   SELECT @return = 0
   
--	SET ROWCOUNT 250

	
	IF (@return = 0)
	BEGIN	
		IF EXISTS (SELECT * FROM accountlocking WHERE lockcount = 0)
			DELETE 
			FROM	AccountLocking 
			WHERE	LockCount = 0
		
		IF @@error = 0	
		BEGIN
            SELECT DISTINCT
                    ag.acctno,
                    ag.agrmtno,
                    --s.itemno,
                    si.IUPC as itemno,					--IP - CR1212 - RI - #3806
                    s.stocklocn,
                    l.contractno,
                    convert(varchar(20),s.delorcoll) as DelOrColl,
                    --s.quantity,
					isnull(lbf.Quantity,0) as quantity,						--#13697
                    l.delqty,
                    l.price,
                    l.ordval,
                    s.buffbranchno,
                    s.buffno,
                    isnull(lbf.quantity,0) as lbfquantity,
                    isnull(lbf.price,0) as lbfprice,
                    isnull(lbf.ordval,0) as lbfordval,
                    l.ItemID,							--IP - CR1212 - RI - #3806
                    l.ID as LineItemId
            --INTO    #deliveries
            FROM    accountlocking al, agreement ag, schedule s, custacct ca, lineitem l
            INNER JOIN StockInfo si on l.ItemID = si.ID			--IP - cr1212 - RI - #3806
            LEFT OUTER JOIN lineitembfcollection lbf
            ON      lbf.acctno = l.acctno
            AND     lbf.agrmtno = l.agrmtno
            --AND     lbf.itemno = l.itemno
            AND		lbf.ItemID = l.ItemID						--IP - CR1212 - RI - #3806
            AND     lbf.contractno = l.contractno
            WHERE   al.lockedby = @user
            AND     al.acctno = @acctno
            AND     ag.acctno = al.acctno
            AND     l.acctno = al.acctno
            AND     l.itemtype != 'N'
            AND     s.acctno = l.acctno
            AND     s.agrmtno = l.agrmtno
            --AND     s.itemno = l.itemno
            AND		s.ItemID = l.ItemID							--IP - CR1212 - RI - #3806
            AND     s.stocklocn = l.stocklocn
            AND     (s.delorcoll = 'C' OR @collectionsOnly = 0)
            AND     ca.acctno = l.acctno
            AND     ca.hldorjnt = 'H'
            AND		s.buffno != 0
            AND     NOT EXISTS (select 1 from cancellation where cancellation.acctno = al.acctno)
            and l.deliveryprocess='I'			-- #10544 
            
            union
            -- #10544 get Scheduled booking cancellations
            SELECT DISTINCT
                    ag.acctno,
                    ag.agrmtno,
                    --s.itemno,
                    si.IUPC as itemno,					--IP - CR1212 - RI - #3806
                    l.stocklocn,
                    l.contractno,
                    convert(varchar(20),s.delorcoll) as DelOrColl,
                    --s.quantity,
				    isnull(lbf.Quantity,0) as quantity,						--#13697
                    l.delqty,
                    l.price,
                    l.ordval,
                    L.DelnoteBranch as buffbranchno,
                    s.BookingId as buffno,
                    isnull(lbf.quantity,0) as lbfquantity,
                    isnull(lbf.price,0) as lbfprice,
                    isnull(lbf.ordval,0) as lbfordval,
                    l.ItemID,							--IP - CR1212 - RI - #3806
                    l.ID as LineItemId
            --INTO    #deliveries
            FROM    accountlocking al, agreement ag, LineitemBookingschedule s INNER JOIN LineItemBookingFailures f on s.bookingId=f.OriginalBookingID --#15054 --#13696
			, custacct ca, lineitem l
            INNER JOIN StockInfo si on l.ItemID = si.ID			--IP - cr1212 - RI - #3806
            LEFT OUTER JOIN lineitembfcollection lbf
            ON      lbf.acctno = l.acctno
            AND     lbf.agrmtno = l.agrmtno
            --AND     lbf.itemno = l.itemno
            AND		lbf.ItemID = l.ItemID						--IP - CR1212 - RI - #3806
            AND     lbf.contractno = l.contractno
            WHERE   al.lockedby = @user
            AND     al.acctno = @acctno
            AND     ag.acctno = al.acctno
            AND     l.acctno = al.acctno
            AND     l.itemtype != 'N'
            AND     s.LineItemId = l.Id
            --------AND     s.agrmtno = l.agrmtno
            --AND     s.itemno = l.itemno
            --AND		s.ItemID = l.ItemID							--IP - CR1212 - RI - #3806
            --AND     s.stocklocn = l.stocklocn
            AND     (s.delorcoll = 'C' OR @collectionsOnly = 0)
            AND     ca.acctno = l.acctno
            AND     ca.hldorjnt = 'H'
            --AND		s.buffno != 0
            AND     NOT EXISTS (select 1 from cancellation where cancellation.acctno = al.acctno)
            --and l.deliveryprocess='S'
            and l.deliveryprocess IN('S','I')	--IP - 21/01/13 - #12003 - LW75608
			and f.Actioned is null		--#13696
			and s.quantity != 0			--#14313
/*
            IF (@collectionsOnly = 1)
            BEGIN
                -- Load any deliveries matching collections (and assume they are replacements)
                SELECT DISTINCT
                        d.acctno,
                        d.agrmtno,
                        d.itemno,
                        s.stocklocn,
                        d.contractno,
                        'D' as DelOrColl,
                        s.quantity,
                        d.delqty,
                        d.price,
                        d.ordval,
                        s.buffbranchno,
                        s.buffno,
                        convert(float,0) as lbfquantity,
                        convert(money,0) as lbfprice,
                        convert(money,0) as lbfordval
                INTO    #replacements
                FROM    #deliveries d, schedule s
                WHERE   s.acctno = d.acctno
                AND     s.agrmtno = d.agrmtno
                AND     s.itemno = d.itemno
                AND     s.delorcoll = 'D'

                SELECT * FROM #deliveries
                UNION
                SELECT * FROM #replacements ORDER BY AcctNo, AgrmtNo, ItemNo, DelOrColl
            END
            ELSE
            BEGIN
                SELECT * FROM #deliveries ORDER BY AcctNo, AgrmtNo, ItemNo
            END
*/
			
			SELECT @return = @@error
		END
		
	END
   
	SET ROWCOUNT 0

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO
/* For test
execute dbo.DN_DeliveryNotesGetSP 
   @acctno ='720403607860', 
   @user =9, --current user
   @collectionsOnly = 1,
   @TimeLocked =null ,  
   @return = 0
*/
go

-- End End End End End End End End End End End End End End End End End End End End End End End End End End End End End End End

