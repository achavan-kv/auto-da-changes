
/************************************************/
/**  Create trigger trig_asChangeStatus        **/
/************************************************/

if exists (select * from sysobjects 
		where type='TR' and name = 'trig_asChangeStatus')
drop trigger trig_asChangeStatus
go
CREATE TRIGGER trig_asChangeStatus 
-- =============================================
-- Author:		?
-- Create date: ?
-- Title: trig_asChangeStatus.sql
-- Description:	
-- Change Control
-- *****************
-- Date      Author    Change
-- 25/07/11  IP		   RI system Integration changes.
-- =============================================
 ON acct 
 FOR update 
 AS 
declare @rowcount integer
SET NOCOUNT ON
SELECT  @ROWCOUNT =COUNT(*) FROM INSERTED

   DECLARE @new_acctno     char(12), 
    @new_currstatus char(1), 
    @old_currstatus char (1),
    @lastupdatedby integer,
    @datetran       datetime ,
    @new_holdprop varchar(6)
IF @ROWCOUNT = 1 /* a single row */
begin

   SELECT @new_acctno      = acctno, 
          @new_currstatus  = currstatus ,
          @lastupdatedby   = lastupdatedby
   FROM   inserted 

   SELECT @old_currstatus  = currstatus 
   FROM   deleted  
   SELECT @datetran = GETDATE() 

	if @new_currstatus ='S'    -- AA check whether any outstanding free gifts if so do not settle.
   	begin --first check whether stockitem deliveries in the last 2 months if there is then check whether any non-stocks outstanding
        -- we are doing this to prevent accounts that need to be settled when fully paid to be settled even if outstanding non-stocks as they have passed the time in which they should have delivered them
		   if exists (select * from delivery, stockitem 
			      --where  delivery.itemno = stockitem.itemno 
			      where  delivery.ItemID = stockitem.ID										--IP - 25/07/11 - RI 
			      and    delivery.stocklocn = stockitem.stocklocn
			      and    stockitem.itemtype ='S' 
			      and    delivery.transvalue > 0 
			      and    delivery.acctno = @new_acctno
                              and    dateadd(month,2,delivery.datedel) >getdate() ) 
	      	   begin 
         		if exists (select lineitem.acctno from lineitem, stockitem 
				   where  acctno =@new_acctno 
				   --and 	  lineitem.itemno = stockitem.itemno 
				   and 	  lineitem.ItemID = stockitem.ID									--IP - 25/07/11 - RI
				   and    lineitem.stocklocn = stockitem.stocklocn  
                        	  -- and    stockitem.category in (14, 24, 84) am removing this - were just doing free gifts but now any item
				   and    quantity > 0 
                   and    iskit = 0 --69565 KEF added so accounts with only the main kit item left can be settled
                   --and    lineitem.itemno not in ('ADDDR','ADDCR','DT','STAX')
                   and    stockitem.iupc not in ('ADDDR','ADDCR','DT','STAX')				--IP - 25/07/11 - RI
				   and not exists (select * from delivery  -- check if free gift not delivered
						   where  delivery.acctno = lineitem.acctno 
						   --and    lineitem.itemno = delivery.itemno
						   and    lineitem.ItemID = delivery.ItemID							--IP - 25/07/11 - RI
						   and    delivery.stocklocn = lineitem.stocklocn)
               and not exists (select sum(transvalue) from fintrans f where f.acctno = lineitem.acctno
               and f.transtypecode in ('BDW','ADD') 
		having sum(transvalue) <0 ) -- but if writing off or if settling by add to then you would want to settle the account anyway
               and not exists (select * from cancellation c where c.acctno = lineitem.acctno)) -- if you are cancelling the account you want to settle it anyway.
		  	            begin
            				update acct set currstatus = @old_currstatus where acctno = @new_acctno
            				set @new_currstatus = @old_currstatus
            		  	end	

	           end
	           if @new_currstatus='S' -- still settling
	           -- remove authorisation record. This should mean that certain screens which reference this table are quicker.....
            		EXECUTE dbremoveauth @acctno = @new_acctno
    end
    else
    if @new_currstatus !='S'  and @old_currstatus ='S'
    begin
       SELECT  @new_holdprop = agreement.holdprop
       FROM    inserted,agreement
       where agreement.acctno =inserted.acctno and agreement.agrmtno = 1

       IF @new_holdprop = 'Y'
       BEGIN
          
           EXECUTE dbnewauth @acctno = @new_AcctNo
       END
    end

   /* insert record into status table for .net through trigger-@lastupdatedby defaults to 0
    but will be updated through  .NET app. */

	--IP - 05/02/09 - CR971 - is no longer necessary
   if @new_currstatus != @old_currstatus --and ( @lastupdatedby !=0 or APP_NAME() ='.Net SqlClient Data Provider')
   begin
       -- FR67742 But don't insert the same status record again
       IF NOT EXISTS (SELECT * FROM Status
                      WHERE  AcctNo = @New_AcctNo
                      AND    StatusCode = @New_CurrStatus
                      AND    DateStatChge = (SELECT MAX(DateStatChge)
                                             FROM   Status
                                             WHERE  AcctNo = @New_AcctNo))
       BEGIN
           insert into status ( origbr,acctno,datestatchge,empeenostat,statuscode)
           values (0,@new_acctno,getdate(),@lastupdatedby,@new_currstatus)
       END
   end

end
else

   -- Insert settled record in status table otherwise
   insert into status ( origbr,acctno,datestatchge,empeenostat,statuscode)
   select   0,i.acctno,getdate(),i.lastupdatedby,i.currstatus
   from     inserted i, deleted d
   where    i.acctno =  d.acctno
   and      i.currstatus !=d.currstatus
go
