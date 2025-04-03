IF EXISTS (Select * from sysobjects
		   where xtype = 'v'
		   and name = 'ItemsWithoutWarrantiesView')
BEGIN
	DROP VIEW ItemsWithoutWarrantiesView
END
GO

CREATE VIEW ItemsWithoutWarrantiesView
AS

select l.acctno, l.agrmtno, l.itemno, l.ItemID,  l.stocklocn,ca.custid,		--#16992
	 (SELECT Isnull(Max(right(code,1)), '0') FROM acctcode WHERE AcctNo = l.AcctNo AND reference = l.itemno AND LEFT(Code,3) = 'SSP') [NoOfPrompts]		
from lineitem l
inner join custacct ca on l.acctno = ca.acctno
where l.itemtype = 'S'
and not exists (Select 1 
			    from lineitem l2
				inner join stockinfo s on l2.ItemID = s.Id			--#16019
				where l2.acctno = l.acctno
				and l2.agrmtno = l.agrmtno
				and l2.ParentItemID = l.ItemID
				and l2.parentlocation = l.stocklocn
				and l2.quantity!=0								--#16019
				and s.WarrantyType <> 'F'							--#16019
				and len(rtrim(ltrim(l2.contractno))) > 0)
AND NOT EXISTS (
			SELECT * 
			FROM acctcode
			WHERE reference = l.itemno AND																	
				AcctNo = l.AcctNo AND 
				code = 'SSP' + convert(varchar(2), (select value from countrymaintenance where codename = 'WarrantySESPrompts'))
		)
