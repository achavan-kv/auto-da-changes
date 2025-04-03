if exists (select * from sysobjects where name = 'dn_FactExportDeliverNonStocks')
drop procedure dn_FactExportDeliverNonStocks
go
create procedure dn_FactExportDeliverNonStocks
-- ================================================
-- Project      : CoSACS .NET
-- File Name    : dn_FactExportDeliverNonStocks.prc
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        :
-- Author       : ?
-- Date         : ?
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 23/06/11  IP   CR1212 - RI - #3987 - RI Integration system changes - join using ItemID 
-- ================================================
@return int OUT
as
-- this procedure will deliver non stocks prior to export
-- previously this was done through OpenROAD, but as .Net is a lot more hassle accessing the database
-- thought it best to do it in one big chunk
-- This procedure delivers all non-stocks which have never been delivered
-- where they either don't have a parent item or their parent item has been delivered
declare @hobranchno int,@transrefno int
SET @return = 0
SET NOCOUNT on

select @hobranchno = hobranchno from country

--select l.acctno,l.itemno,l.contractno,l.stocklocn,
select l.acctno,si.iupc as itemno,l.contractno,l.stocklocn,											--IP - 23/06/11 - CR1212 - RI - #3987
       l.agrmtno,l.delnotebranch,l.quantity,l.ordval,	
--convert(int,0) as buffno,convert(int,0) as transrefno,l.parentitemno,l.parentlocation,convert(smallint,0) as kitparent,
convert(int,0) as buffno,convert(int,0) as transrefno,si2.iupc as parentitemno,l.parentlocation,convert(smallint,0) as kitparent, --IP - 23/06/11 - CR1212 - RI - #3987
convert(smallint,0) as kitparentdel, convert(smallint,0) as hasparent, convert(smallint,0) as parentdel, l.ItemID, l.ParentItemID --IP - 23/06/11 - CR1212 - RI - #3987
into #tobedelivered 
--from lineitem l,stockitem s
from lineitem l inner join StockInfo si on si.ID = l.ItemID											--IP - 23/06/11 - CR1212 - RI - #3987
inner join StockQuantity sq on si.id = sq.id and sq.stocklocn = l.stocklocn and si.itemtype = 'N'	--IP - 23/06/11 - CR1212 - RI - #3987
left join StockInfo si2 on si2.ID = l.ParentItemID													--IP - 23/06/11 - CR1212 - RI - #3987
where exists (select d.acctno from delivery d where d.acctno = l.acctno and d.runno = 0)
--and s.itemno = l.itemno and s.stocklocn = l.stocklocn and s.itemtype ='N'
and not exists (select * from delivery n 
                where n.acctno =l.acctno	
                --and n.itemno =l.itemno	
				and n.ItemID =l.ItemID																--IP - 23/06/11 - CR1212 - RI - #3987
                and n.stocklocn =l.stocklocn
                and n.contractno = l.contractno)
and l.acctno not like '___5%' -- exclude cash and go any discounts should have been already delivered
and l.ordval <>0
AND l.quantity > 0 --RM 05-01-10 check that quantity is greater than 0 not just ordval

-- remove warranties without contract numbers 

DELETE FROM #tobedelivered WHERE 
--EXISTS (SELECT * FROM StockInfo s WHERE s.itemno = #tobedelivered.itemno 
EXISTS (SELECT * FROM StockInfo s WHERE s.ID = #tobedelivered.ItemID								--IP - 23/06/11 - CR1212 - RI - #3987 
AND s.category IN (select distinct code from code where category = 'WAR')) 
AND ISNULL(#tobedelivered.contractno,'') ='' 


create clustered index ix_dffds3dw on #tobedelivered(acctno)
-- kit parent items are never delivered so need to update the parentitem to be a component
-- remove where parent item not delivered
update #tobedelivered set kitparent =1 where exists (select * from lineitem l where
--l.acctno = #tobedelivered.acctno and l.itemno = #tobedelivered.parentitemno and l.stocklocn =#tobedelivered.parentlocation
l.acctno = #tobedelivered.acctno and l.ItemID = #tobedelivered.ParentItemID and l.stocklocn =#tobedelivered.parentlocation			--IP - 23/06/11 - CR1212 - RI - #3987 
and l.iskit =1) --68864 some non stocks not delivering as wrongly marked as kitparent here

update #tobedelivered set kitparentdel =1 where kitparent = 1 
and exists (select * from 
            delivery d, lineitem s /*kit component*/, lineitem k /*kit parent */
--where k.acctno = #tobedelivered.acctno and k.itemno = #tobedelivered.parentitemno and k.stocklocn =#tobedelivered.parentlocation
where k.acctno = #tobedelivered.acctno and k.ItemID = #tobedelivered.ParentItemID and k.stocklocn =#tobedelivered.parentlocation	--IP - 23/06/11 - CR1212 - RI - #3987 
--and k.itemno = s.parentitemno and k.stocklocn = s.parentlocation and k.acctno = s.acctno
and k.ItemID = s.ParentItemID and k.stocklocn = s.parentlocation and k.acctno = s.acctno											--IP - 23/06/11 - CR1212 - RI - #3987 
--and d.acctno=  s.acctno and d.itemno = s.itemno and d.stocklocn = s.stocklocn and d.contractno = s.contractno)
and d.acctno=  s.acctno and d.ItemID = s.ItemID and d.stocklocn = s.stocklocn and d.contractno = s.contractno)						--IP - 23/06/11 - CR1212 - RI - #3987 

-- mark kitparent delivered if all potential component stockitems delivered i.e. there are not oustanding stockitems to be delivered
-- this is because there are instances where the parent item number has not been saved properly to the database.
update #tobedelivered set kitparentdel =1 where kitparent = 1  
--and not exists (select * from lineitem l,stockitem s,kitproduct k where l.acctno =#tobedelivered.acctno and
and not exists (select * from lineitem l inner join StockInfo si on l.ItemID = si.ID												--IP - 23/06/11 - CR1212 - RI - #3987 
					inner join StockQuantity sq on si.ID = sq.ID and l.stocklocn = sq.stocklocn										--IP - 23/06/11 - CR1212 - RI - #3987 
					inner join kitproduct k on k.ComponentID = l.ItemID																--IP - 23/06/11 - CR1212 - RI - #3987 
					where l.acctno =#tobedelivered.acctno 
					and si.itemtype = 'S' and l.quantity >0 and not exists
--l.itemno =s.itemno and l.stocklocn =s.stocklocn and s.itemtype = 'S' and l.quantity >0
--and k.componentno = l.itemno and not exists 
--(select * from delivery d where d.acctno = l.acctno and d.itemno = l.itemno and d.stocklocn = l.stocklocn))
(select * from delivery d where d.acctno = l.acctno and d.ItemID = l.ItemID and d.stocklocn = l.stocklocn))							--IP - 23/06/11 - CR1212 - RI - #3987 

--select * from #tobedelivered where acctno ='900416559040' and kitparent = 1


update #tobedelivered set hasparent =1 where exists (select * from lineitem l where
--l.acctno = #tobedelivered.acctno and l.itemno = #tobedelivered.parentitemno and l.stocklocn =#tobedelivered.parentlocation
l.acctno = #tobedelivered.acctno and l.ItemID = #tobedelivered.ParentItemID and l.stocklocn =#tobedelivered.parentlocation			--IP - 23/06/11 - CR1212 - RI - #3987 
and l.iskit =0) --68864 some non stocks not delivering as wrongly marked as kitparent here

update #tobedelivered set parentdel =1 where hasparent = 1 
and exists (select * from 
            delivery d
--where d.acctno = #tobedelivered.acctno and d.itemno = #tobedelivered.parentitemno and d.stocklocn =#tobedelivered.parentlocation
where d.acctno = #tobedelivered.acctno and d.ItemID = #tobedelivered.ParentItemID and d.stocklocn =#tobedelivered.parentlocation	--IP - 23/06/11 - CR1212 - RI - #3987 
and d.agrmtno = #tobedelivered.agrmtno)


-- don't deliver kit warranties or discounts if no components have been delivered
delete from #tobedelivered where (kitparentdel = 0 and kitparent =1)
or (parentdel = 0 and hasparent = 1)

update #tobedelivered set quantity = 1 where quantity = 0
declare @NumberofStockItemsDelivered int,@numberofwarrantiesalreadydelivered int ,@numberofwarrantiestodeliver int,
@prevacctno char(12), @itemno varchar(18),@warranty varchar(18),@acctno char(12),@stocklocn smallint,@contractno varchar(10),
@prevwarrantyno  varchar(18),@parentitemno varchar(18),@parentlocation smallint, @itemID int, @parentItemID int, @prevwarrantyID int						--IP - 23/06/11 - CR1212 - RI - #3987 
-- check where more than one item ordered and more than one warranty sold that each warranty item is 
-- delivered only if that quantity of parent item has already been delivered. 
set @prevwarrantyno = ''
set @prevacctno = ''
set @numberofwarrantiestodeliver =0

DECLARE grthanOneorderwarranty_cursor CURSOR 
  	FOR SELECT acctno,itemno,stocklocn,contractno,parentitemno,parentlocation, ItemID, ParentItemID									--IP - 23/06/11 - CR1212 - RI - #3987 
    from #tobedelivered where exists (select * from lineitem l where l.acctno =#tobedelivered.acctno and
    --l.itemno = #tobedelivered.parentitemno and l.agrmtno =#tobedelivered.agrmtno and l.quantity >1)
      l.ItemID = #tobedelivered.ParentItemID and l.agrmtno =#tobedelivered.agrmtno and l.quantity >1)								--IP - 23/06/11 - CR1212 - RI - #3987 
    and contractno !=''
	order by acctno,itemno,stocklocn,contractno
    OPEN grthanOneorderwarranty_cursor
   FETCH NEXT FROM grthanOneorderwarranty_cursor INTO @acctno,@warranty,@stocklocn,@contractno,@parentitemno,@parentlocation, @itemID, @parentItemID		--IP - 23/06/11 - CR1212 - RI - #3987 

   WHILE (@@fetch_status <> -1)
   BEGIN
       IF (@@fetch_status <> -2)
       BEGIN
			if @acctno = @prevacctno and @itemID = @prevwarrantyID --@warranty = @prevwarrantyno				--IP - 23/06/11 - CR1212 - RI - #3987 
			begin
				set @numberofwarrantiestodeliver = @numberofwarrantiestodeliver +1
			end
			else
			begin
				set @numberofwarrantiestodeliver=1
			end
            
            
			select @numberofwarrantiesalreadydelivered = isnull(sum(quantity),0) from delivery d where
			--acctno =@acctno and itemno = @warranty and stocklocn = @stocklocn
			acctno =@acctno and ItemID = @itemID and stocklocn = @stocklocn										--IP - 23/06/11 - CR1212 - RI - #3987 

		    select @NumberofStockItemsDelivered= isnull(sum(quantity),0) from delivery where acctno = @acctno 
			--and itemno = @parentitemno and stocklocn = @parentlocation
			and ItemID = @parentItemID and stocklocn = @parentlocation											--IP - 23/06/11 - CR1212 - RI - #3987 
			
		    if @numberofwarrantiestodeliver + @numberofwarrantiesalreadydelivered >@NumberofStockItemsDelivered
			begin -- don't deliver this warranty as number of parent items delivered is not enough. 
				--delete from #tobedelivered where acctno = @acctno and itemno = @warranty and contractno = @contractno
				delete from #tobedelivered where acctno = @acctno and ItemID = @itemID and contractno = @contractno		--IP - 23/06/11 - CR1212 - RI - #3987 
				--print 'removed account ' + @acctno + ' warranty ' + @warranty + ' from list '
			end
            
			/*if @acctno ='901402627900'
			begin
			  select @numberofwarrantiesalreadydelivered as wdel,@NumberofStockItemsDelivered as sdel
			  ,@parentitemno as pitem,@parentlocation as plocn,@prevwarrantyno as pwarrant
			end*/ 
			set @prevacctno = @acctno
            --set @prevwarrantyno = @warranty
            set @prevwarrantyID = @itemID																		--IP - 23/06/11 - CR1212 - RI - #3987 
       END
       FETCH NEXT FROM grthanOneorderwarranty_cursor INTO @acctno,@warranty,@stocklocn,@contractno,@parentitemno,@parentlocation, @itemID, @parentItemID		--IP - 23/06/11 - CR1212 - RI - #3987 
   END
   CLOSE grthanOneorderwarranty_cursor
   DEALLOCATE grthanOneorderwarranty_cursor



declare @numberoftrans int
select @numberoftrans =( count(*) + 1) from #tobedelivered
update branch set hirefno = hirefno + @numberoftrans where branchno = @hobranchno
select @transrefno = hirefno from branch where branchno = @hobranchno

-- get the highest refno then reduce
--declare @acctno char(12),@itemno varchar(10)
SET NOCOUNT on
DECLARE del_cursor CURSOR 
  	FOR SELECT acctno,itemno,stocklocn,contractno, ItemID								--IP - 23/06/11 - CR1212 - RI - #3987 					
    from #tobedelivered
	for update of transrefno 
    OPEN del_cursor
   FETCH NEXT FROM del_cursor INTO @acctno,@itemno,@stocklocn,@contractno, @itemID		--IP - 23/06/11 - CR1212 - RI - #3987 	

   WHILE (@@fetch_status <> -1)
   BEGIN
       IF (@@fetch_status <> -2)
       BEGIN
            update #tobedelivered set transrefno = @transrefno 
            --where acctno= @acctno and itemno = @itemno
            where acctno= @acctno and ItemID = @itemID									--IP - 23/06/11 - CR1212 - RI - #3987 	
            and stocklocn = @stocklocn and contractno = @contractno
            set @transrefno = @transrefno -1
       END
       FETCH NEXT FROM del_cursor INTO  @acctno,@itemno,@stocklocn,@contractno, @itemID	--IP - 23/06/11 - CR1212 - RI - #3987 	
   END
   CLOSE del_cursor
   DEALLOCATE del_cursor


    update #tobedelivered set buffno = d.buffno
    from delivery d where 
    d.acctno = #tobedelivered.acctno
    and d.runno = 0
-- here were are assigning the buffno to the non-stock item


    insert into delivery 
     (origbr,acctno,agrmtno,datedel,
      delorcoll,itemno,stocklocn,quantity,
      retitemno,retstocklocn,retval,buffno,
      buffbranchno,datetrans,branchno,transrefno,
      transvalue,runno,contractno,ReplacementMarker,
      NotifiedBy,ParentItemNo, ItemID, ParentItemID)				--IP - 23/06/11 - CR1212 - RI - #3987 	
      select
      @hobranchno,acctno,agrmtno,getdate(),
      'D',itemno,stocklocn,quantity,
      '',null,0,buffno,
      delnotebranch,getdate(),@hobranchno,transrefno,
      ordval,0,contractno,'',
      99999,Parentitemno, ItemID, ParentItemID						--IP - 23/06/11 - CR1212 - RI - #3987 	
      from #tobedelivered

    update acct set outstbal = (select sum(transvalue) from fintrans f
     where f.acctno =acct.acctno)
     where exists (select * from #tobedelivered d where d.acctno = acct.acctno)
	--select * from #tobedelivered where acctno ='901402627900'
	 drop table #tobedelivered

    update delivery set quantity = 1 where quantity = 0 and runno = 0
go