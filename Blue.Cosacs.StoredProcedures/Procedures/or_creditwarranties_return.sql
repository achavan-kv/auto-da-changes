SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[or_creditwarranties_return]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[or_creditwarranties_return]
GO

CREATE procedure or_creditwarranties_return 
--------------------------------------------------------------------------------
--
-- Project      : CoSACS .NET
-- File Name    : or_creditwarranties_return.PRC
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : or_creditwarranties_return.PRC
-- Description	: Collects warranties on credit that have passed the Warranties on Credit grace period
-- Author       : ?
-- Date         : ?
--
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 21/06/11  IP  CR1212 - RI - #3979 - RI System Changes. This procedure will now be called when running the RI Export.
--			     Previously was being called when running CoSACS to FACT Export.
-- 23/06/11 jec  #3978 fix ParentItemId
--------------------------------------------------------------------------------
 @return           INT  out
as 
begin
/* procedure will retrieve details of all credit warranties which have not yet been paid
return the warranties and settle the accounts*/

set @return = 0

   declare @seed integer,
		   @hobranchno smallint,
		   @datedel DateTime
		   
   set @datedel =getdate()
   select @seed =hirefno from branch , country where branchno = country.hobranchno

   select @hobranchno = hobranchno from country
	select @seed as seed into #seed
   /* create #temp table */
   select acctno, agrmtno, branchno, buffbranchno, 
   buffno, contractno, datedel, datetrans, 
   delorcoll, itemno, origbr, quantity, 
   ReplacementMarker, retitemno, retstocklocn, retval, 
   runno, stocklocn, transrefno, transvalue ,IDENTITY ( int , 1 , 1) AS reference_number,convert (integer,0) as newbuffno,
		0 as ItemID, 0 as ParentItemID,CAST('' as VARCHAR(18)) as ParentItemNo			-- jec #3978
   into #temporary_delivery 
   from delivery where  acctno = 'bob' -- acctno never 'bob' so will always be 0 rows
   SET IDENTITY_INSERT #temporary_delivery ON
   -- just populating the buffno for the identity column
   insert into  #temporary_delivery
      (acctno, agrmtno, branchno, buffbranchno, 
       buffno, contractno, datedel, datetrans, 
       delorcoll, itemno, origbr, quantity, 
       ReplacementMarker, retitemno, retstocklocn, retval, 
       runno, stocklocn, transrefno, transvalue, reference_number,newbuffno,ItemID,ParentItemID,ParentItemNo)	-- jec #3978 IP - 20/06/11 - #3979
   select  top 1  acctno, agrmtno, branchno, buffbranchno, 
      buffno, contractno, datedel, datetrans, 
      delorcoll, itemno, origbr, quantity, 
      ReplacementMarker, retitemno, retstocklocn, retval, 
      runno, stocklocn, transrefno, transvalue,@seed, 0,0,0,''			-- jec #3978
      from  delivery					--IP - 20/06/11 - CR1212 - RI - #3979
   delete from #temporary_delivery 
   
   SET IDENTITY_INSERT #temporary_delivery off
   insert into  #temporary_delivery
      (acctno, agrmtno, branchno, buffbranchno, 
       buffno, contractno, datedel, datetrans, 
       delorcoll, itemno, origbr, quantity, 
       ReplacementMarker, retitemno, retstocklocn, retval, 
       runno, stocklocn, transrefno, transvalue,newbuffno, ItemID,ParentItemID,ParentItemNo)		-- jec #3978 IP - 20/06/11 - #3979
   select d.acctno, d.agrmtno, d.branchno, d.buffbranchno, 
   d.buffno, d.contractno, d.datedel, d.datetrans, 
   --d.delorcoll, d.itemno, d.origbr, d.quantity, 
   d.delorcoll, si.IUPC as itemno, d.origbr, d.quantity,								--IP - 20/06/11 - CR1212 - RI - #3979
   d.ReplacementMarker, d.retitemno, d.retstocklocn, d.retval, 
   d.runno, d.stocklocn, d.transrefno, d.transvalue,0, d.ItemID,d.ParentItemID,ParentItemNo				-- jec #3978 IP - 20/06/11 - #3979
   --from delivery d,acct, country 
   from country,delivery d inner join acct a on d.acctno =a.acctno						--IP - 20/06/11 - CR1212 - RI - #3979
   inner join stockinfo si on d.ItemID = si.ID											--IP - 20/06/11 - CR1212 - RI - #3979
   --where d.acctno =acct.acctno    
   where a.currstatus !='S' and d.contractno !=''
   and a.termstype='WC' -- WC = WARRANTIES on credit
   and a.outstbal > 0 -- balance > 0 so customer not paid off.
   and dateadd (day,country.creditwarrantyDays + country.creditwarrantygrace,datedel) <getdate()           
   and not exists (select * from 
   --delivery e where	d.acctno = e.acctno and d.itemno = e.itemno and d.agrmtno = e.agrmtno
   delivery e where	d.acctno = e.acctno and d.ItemID= e.ItemID and d.agrmtno = e.agrmtno		--IP - 20/06/11 - CR1212 - RI - #3979
   and d.contractno = e.contractno and d.delorcoll= 'C')
	--create index ix_temporary_delivery_12 on #temporary_delivery (acctno, itemno, stocklocn) 
	create index ix_temporary_delivery_12 on #temporary_delivery (acctno, ItemID, stocklocn)    --IP - 20/06/11 - CR1212 - RI - #3979           
	   --print 'stage 4 '
	--delete from #temporary_delivery where not exists (select * from 
 --       stockitem where stockitem.itemno = #temporary_delivery.itemno and stockitem.stocklocn = #temporary_delivery.stocklocn
 --       and stockitem.category in (12, 82))
 
	--IP - 20/06/11 - CR1212 - RI - #3979   
 	delete from #temporary_delivery where not exists (select * from 
        stockinfo si inner join stockquantity sq on si.id = sq.id
        inner join #temporary_delivery t on si.id = t.ItemID and sq.stocklocn = t.stocklocn
        and si.category in (select distinct code from code where category = 'WAR'))
        
   -- not sure why but we have to remove items already collected (which should have been excluded in the
   -- not exists above but one account was being repeatedely collected. Anyway do it again just to make sure)
   DELETE  a
   FROM #temporary_delivery a
    WHERE  EXISTS 
   --(SELECT * FROM delivery d WHERE d.acctno =a.acctno AND d.itemno = a.itemno AND d.contractno = a.contractno
   (SELECT * FROM delivery d WHERE d.acctno =a.acctno AND d.ItemID = a.ItemID AND d.contractno = a.contractno
   AND d.delorcoll = 'C' AND d.agrmtno = a.agrmtno )

   declare @buffno integer,@newbuffno integer,@reference_number integer
   DECLARE	delivery_cursor CURSOR 
  	FOR SELECT  newbuffno, reference_number from #temporary_delivery
   FOR UPDATE of newbuffno

   OPEN delivery_cursor
   FETCH NEXT FROM delivery_cursor INTO @buffno,@reference_number

   WHILE (@@fetch_status <> -1)
   BEGIN
	   IF (@@fetch_status <> -2)
   	BEGIN
		update branch set HIbuffno =hibuffno + 1 where branchno =@hobranchno
      select @newbuffno =hibuffno from branch where branchno =@hobranchno
		UPDATE #temporary_delivery
		SET newbuffno = @newbuffno
		WHERE reference_number =@reference_number
	  END
     FETCH NEXT FROM delivery_cursor INTO @buffno,@reference_number

   END

   CLOSE delivery_cursor
   DEALLOCATE delivery_cursor
        
   --print 'stage Five '
   if @seed is not null  and @newbuffno is not null  and @seed > 0 and @newbuffno  > 0
   update branch   set hirefno=@seed,hibuffno =@newbuffno
   where branchno =@hobranchno
   update #temporary_delivery
   set transvalue =  - transvalue, quantity = -quantity,
   retitemno =country.CWitemno, datedel =@datedel, datetrans = getdate()
   from country
   select acctno, sum(transvalue) as transvalue into   #values
   from #temporary_delivery
   group by acctno
   --print 'stage 6 '
   if exists (select * from #temporary_delivery )
   begin         
   insert into delivery (acctno, agrmtno, branchno, buffbranchno, 
   buffno, contractno, datedel, datetrans, 
   delorcoll, itemno, origbr, quantity, 
   ReplacementMarker,retitemno, retstocklocn, retval, 
   runno,stocklocn, transrefno, transvalue,ftnotes, ItemID,ParentItemID,ParentItemNo)			-- jec #3978 IP - 21/06/11 - #3979
   
   select acctno, agrmtno, branchno, buffbranchno, 
   newbuffno, contractno, datedel, datetrans, 
   'C', itemno, origbr, quantity, 
   ReplacementMarker,retitemno, retstocklocn, retval, 
   0,stocklocn, reference_number, transvalue,'CWRT', ItemID,ParentItemID,ParentItemNo				-- jec #3978 IP - 21/06/11 - #3979
   from #temporary_delivery
   --print 'insert into fintrans'
/*   insert into fintrans (
		acctno, bankacctno, bankcode, branchno, 
		chequeno, datetrans, empeeno, ftnotes, 
      origbr,  paymethod,  runno,   source, 
      transprinted,transrefno, transtypecode, transupdated, 
      transvalue)
   select        
      acctno,'', '', @hobranchno, 
		'', datedel, 9999, 'CWRT', 
      @HObranchno,  0, 0, 'COSACS', 
      'N',reference_number, 'GRT', 'Y', 
      transvalue from #temporary_delivery
   */ 
    update acct 
    set outstbal= outstbal + transvalue,agrmttotal =agrmttotal  + transvalue
    from #values
    where #values.acctno =acct.acctno
    --print 'settling accounts'
    update acct set currstatus ='S', lastupdatedby = 99999 from #values
    where #values.acctno =acct.acctno and acct.outstbal= 0


   end
   
   
    SET @Return = @@ERROR
    RETURN @Return
   --select * from #temporary_delivery
end


GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

-- End End End End End End End End End End End End End End End End End End End End End End End End End End End End End End End End End