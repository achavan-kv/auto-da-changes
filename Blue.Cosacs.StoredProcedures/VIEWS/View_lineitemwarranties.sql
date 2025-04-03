IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.tables WHERE table_name =	'lineitemwarranties')
drop view lineitemwarranties
go
create view lineitemwarranties
--------------------------------------------------------------------------------
--
-- Project      : 
-- File Name    : View_lineitemwarranties.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : 
-- Author       : ?
-- Date         : ?
--
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 26/07/11  ip  RI Integration
--------------------------------------------------------------------------------
as 

select w.acctno,
w.agrmtno,
--w.itemno as warrantyno,
isnull(s.iupc,'') as warrantyno,			--IP - 26/07/11 - RI
--'' as warrantyid,
w.ItemID as warrantyid,						--IP - 26/07/11 - RI
w.stocklocn,
--w.parentitemno as itemno,
isnull(sp.iupc,'') as itemno,				--IP - 26/07/11 - RI
w.contractno,
d.runno as deltofact,
--isnull(lpp.parentitemno, '') as kitno,
isnull(spp.iupc, '') as kitno,			--IP - 26/07/11 - RI
0 as nocharge,
--d.retitemno,
isnull(sr.iupc,'') as retitemno,			--IP - 26/07/11 - RI
w.ParentItemID,								--IP - 26/07/11 - RI
isnull(d.RetItemID,0) as RetItemID								--IP - 26/07/11 - RI
 from lineitem w /*associated stockitem */
--join  stockitem s on s.itemno = w.itemno and s.stocklocn =w.stocklocn and s.category in (select distinct code from code where category = 'WAR') /*warranty categories */
join  stockitem s on s.ID = w.ItemID and s.stocklocn =w.stocklocn and s.category in (select distinct code from code where category = 'WAR') /*warranty categories */	--IP - 26/07/11 - RI
--left join lineitem lp on lp.acctno =w.acctno and w.parentitemno =lp.itemno 
left join lineitem lp on lp.acctno =w.acctno and w.ParentItemID =lp.ItemID		--IP - 26/07/11 - RI 
          and w.parentlocation =lp.stocklocn
left join stockinfo sp on lp.ItemID = sp.ID										--IP - 26/07/11 - RI
left join  lineitem lpp /*kit parent of associated stockitem*/
          --on lpp.acctno =lp.acctno and lpp.itemno = lp.parentitemno and lpp.stocklocn =lp.parentlocation and lpp.iskit =1
          on lpp.acctno =lp.acctno and lpp.ItemID = lp.ParentItemID and lpp.stocklocn =lp.parentlocation and lpp.iskit =1		--IP - 26/07/11 - RI
left join stockinfo spp on lpp.ItemID = spp.ID									--IP - 26/07/11 - RI
--left join delivery d on d.acctno =w.acctno and d.itemno =w.itemno 
left join delivery d on d.acctno =w.acctno and d.ItemID =w.ItemID				--IP - 26/07/11 - RI 
          and d.stocklocn =w.stocklocn /*and d.contractno=w.contractno */ 
left join stockinfo sr on d.RetItemID = sr.ID									--IP - 26/07/11 - RI
go
-- grant all on lineitemwarranties to public
grant DELETE, INSERT, REFERENCES, SELECT, UPDATE on lineitemwarranties to public