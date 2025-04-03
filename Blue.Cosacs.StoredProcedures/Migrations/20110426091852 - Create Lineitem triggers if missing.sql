-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF EXISTS (SELECT * FROM sys.triggers WHERE object_id = OBJECT_ID(N'[dbo].[trig_lineiteminsert]'))
BEGIN
	drop trigger [dbo].[trig_lineiteminsert]
END
go

create trigger [dbo].[trig_lineiteminsert]
on [dbo].[lineitem] for insert
as declare @acctno char(12) ,@itemno varchar (10),@stocklocn smallint,@contractno varchar (10),@quantity SMALLINT,
@error varchar(256), @category integer

select @acctno = acctno,@itemno = itemno,@stocklocn = stocklocn,@contractno= contractno,@quantity=quantity from inserted

if @contractno =''
begin
	select @category = category from stockitem where itemno =@itemno and stocklocn =@stocklocn
   if @category in (12, 82) --warranty categories
   begin
      set @error =' error blank contract saving ' + @acctno + ' ' +  @itemno
  		RAISERROR(@error, 16, 1) 
  end
END


if @quantity >1
begin
	select @category = ISNULL(category,0) from stockitem where itemno =@itemno and stocklocn =@stocklocn
   if @category in (12, 82) --warranty categories
   begin
      set @error =' error quantity cannot be >1 saving ' + @acctno + ' ' +  @itemno
  		RAISERROR(@error, 16, 1) 
  end
end
	


go


IF EXISTS (SELECT * FROM sys.triggers WHERE object_id = OBJECT_ID(N'[dbo].[trig_lineiteminsertupdate]'))
BEGIN
	drop trigger [dbo].[trig_lineiteminsertupdate]
END
go
	
create trigger [dbo].[trig_lineiteminsertupdate]      
	on [dbo].[lineitem]      
	for update, INSERT,DELETE   as      
	SET NOCOUNT ON       
	   
	DECLARE @acctno varchar (12), @itemno varchar (12), @testing SMALLINT           
	          
	-- creating export table to reduce locking          
	--SELECT  'tetst' AS [fff],* FROM @ExportTable          
	          
	if @itemno = 'refin'           
	begin          
	   update agreement set deliveryflag ='Y' where acctno = @acctno          
	END          
	          
	DECLARE @minrundate DATETIME             
	SELECT @minrundate = ISNULL(MIN(datestart),GETDATE()) FROM interfacecontrol WHERE interface = 'OrInteg2'           
	        
	-- because inserted and deleted don't have indexes on - in order to improve performance putting these into a new          
	-- #tables. Mauritius actually hung when 18,000 rows updated on lineitem table.           
	SELECT TOP 0 * INTO #insert FROM inserted            
	          
	CREATE CLUSTERED INDEX ix_insert_lineitem2324 ON #insert (acctno,itemno,agrmtno, stocklocn,contractno )          
	          
	CREATE INDEX ix_insertStock ON #insert(itemno,stocklocn, itemtype , qtydiff)          
	          
	SELECT TOP 0 * INTO #delete FROM deleted          
	CREATE CLUSTERED INDEX ix_delete_lineitem2324 ON #delete (acctno,itemno,agrmtno, stocklocn,contractno )          
	          
	INSERT INTO #insert SELECT * FROM inserted           
	          
	INSERT INTO #delete SELECT * FROM deleted           
	        
	IF EXISTS (select *           
		   from #insert i, #delete d          
		   where i.acctno = d.acctno           
		   and i.qtydiff ='N'           
		   and d.qtydiff !='N'           
		   and d.itemno = i.itemno           
		   and d.stocklocn = i.stocklocn          
		   and d.contractno = i.contractno          
		   AND d.agrmtno= i.agrmtno       )          
	BEGIN          
	 delete           
	 from lineitemosdelnotes           
	 where exists (select *           
		 from #insert i, #delete d          
		 where i.acctno =d.acctno           
		 and i.qtydiff ='N'           
		 and d.qtydiff !='N'           
		 and d.itemno =i.itemno           
		 and d.stocklocn =i.stocklocn          
		 and d.contractno =i.contractno          
		 and d.acctno =lineitemosdelnotes.acctno          
		 and d.agrmtno=lineitemosdelnotes.agrmtno          
		 and d.itemno =lineitemosdelnotes.itemno          
		 and d.stocklocn =lineitemosdelnotes.stocklocn          
		 and d.contractno =lineitemosdelnotes.contractno)          
	END           
	-- reduce locking by only inserting/updating if not in the table....          
	if exists (select *           
		from #insert i ,stockitem s          
		where i.qtydiff !='N'           
		AND i.itemno = s.itemno           
		AND i.stocklocn = s.stocklocn          
		AND i.itemtype !='N'          
		AND not exists (select *           
		   from lineitemosdelnotes l          
						 where l.acctno = i.acctno           
		   and l.itemno = i.itemno           
			 and l.stocklocn = i.stocklocn          
			 and l.delnotebranch=i.delnotebranch))          
	 begin         
	              
	 insert into lineitemosdelnotes(acctno, itemno, stocklocn, contractno,agrmtno,delnotebranch)          
	 select a.acctno, lineitem.itemno, lineitem.stocklocn, lineitem.contractno,          
	  lineitem.agrmtno,lineitem.delnotebranch          
	 from  #insert lineitem , acct a, stockitem           
	 where  lineitem.qtydiff ='Y'          
	 and  a.acctno =lineitem.acctno           
	 and  stockitem.itemno = lineitem.itemno           
	 and  stockitem.stocklocn = lineitem.stocklocn          
	 and  stockitem.itemtype !='N'           
	 and  a.currstatus !='S'          
	 and  lineitem.quantity > 0          
	 and  not exists (select *           
		   from  custacct           
		   where custacct.hldorjnt= 'H'          
		   and   custacct.acctno =lineitem.acctno           
		   and custacct.custid like 'PAID & TAKEN%')          
	 and  not exists (select *           
	from lineitemosdelnotes l          
						 where l.acctno = A.acctno           
		   and l.itemno = lineitem.itemno           
			 and l.stocklocn = lineitem.stocklocn          
			 and l.delnotebranch=lineitem.delnotebranch) -- RD/AA Added this to resolve issue 67726          
	end  
	        
	IF EXISTS (SELECT * FROM CountryMaintenance WHERE codeNAME = 'OracleLineExport' AND value IN ('F','P','L'))          
	BEGIN -- populate the export table for interface to Oracle....           
	 -- U where quantity changed but not 0           
		-- C where quantity 0 from positive value.... and don't have a delivery for that item          
		-- SET @TESTING = 1          
	              
	              
	              
		IF @testing =1           
		BEGIN          
	  PRINT ' updating oracle export figures '          
	 END           
	 -- so get the details into table so we can update the newstatus type -- will only work for updates and inserts          


	 INSERT INTO TempStatusType           
		SELECT 
		I.AcctNo, 
		I.ItemNo, 
		I.AgrmtNo,
		I.StockLocn, 
		I.ContractNo,          
		Case  When-- new insert with quantity >0 -- was cancelled and now being re-ordered 
				 (D.Quantity is NULL and ISNULL(I.Quantity,0) >0 )  OR (d.quantity = 0 AND i.quantity >0) Then 'O'          
			  When -- quantity or value has been amended so update
				 (I.Quantity is not NULL and (I.Quantity <> D.Quantity OR i.ordval<>d.ordval) and I.Quantity <> 0 AND i.ordval!=0) 
				 then 'U'
			  When -- Cancelled
				  I.Quantity = 0 and ISNULL(D.Quantity,0)>=0 Then 'C'  		
   			  Else '-' --Default value for unknown type          
		End as NewStatusType ,          
		i.quantity,
		i.ordval ,
		CONVERT(VARCHAR(1),'') AS PrevInttoOr, 
		CONVERT(CHAR(1),'N') AS PrevDel,           
		ISNULL(s.itemtype,'S') AS Itemtype, 
		CONVERT(MONEY,0) AS PrevIntVal, 
		CONVERT(FLOAT,0) AS PrevIntQ ,           
		CONVERT(CHAR(1),'') AS deleteflag, 
		'Z' AS Currenttype,          
		CONVERT(CHAR(1),'') AS Del2bX,
		ISNULL(d.quantity,0) AS DELETEdQty ,           
		isnull(d.ordval,0) AS deleteval,
		i.orderno,
		i.orderlineno, 
		CONVERT(MONEY,0) AS PrevDelVal ,          
		CONVERT(FLOAT,0) AS PrevDelQ , 
		CONVERT(FLOAT,0) AS CURRENTDel , 
		ISNULL(s.category ,0) AS category                  
	 FROM #insert I          
	 LEFT JOIN StockInfo s ON s.itemno= i.itemno           
	 LEFT JOIN #delete D ON I.AcctNo = D.AcctNo and I.ItemNo = D.ItemNo and          
			I.AgrmtNo = D.AgrmtNo and I.StockLocn = D.StockLocn and          
			I.ContractNo = D.ContractNo  
			and i.itemno!='RB'        
	    
	     
	            
	 INSERT INTO TempStatusType           
	 (acctno,itemno,agrmtno,stocklocn,contractno,NewStatusType,quantity,          
	 ordval,deleteval,DELETEdQty,itemtype,deleteflag,          
	 Del2bX,PrevIntQ,PrevIntVal,Currenttype,orderno,orderlineno,          
	 PrevDelVal , PrevDelQ, CURRENTDel, category)                    
	 SELECT  
		d.AcctNo, 
		d.ItemNo, 
		d.AgrmtNo, 
		d.StockLocn, 
		d.ContractNo,
		'C',0,0 ,          
		d.ordval ,
		d.quantity , 
		ISNULL(s.itemtype,'S'),'',          
	 '',0,0,'',orderno,orderlineno,0,0,0 ,0                
	 FROM #delete d           
	 LEFT JOIN stockinfo s ON s.itemno= d.itemno           
	 WHERE NOT EXISTS (SELECT * FROM #insert i           
						WHERE i.acctno= d.acctno 
						AND i.itemno= d.itemno 
						AND i.agrmtno = d.agrmtno           
						AND i.stocklocn = d.stocklocn 
						AND isnull(i.contractno,'')= isnull(d.contractno,'')) 
	and d.itemno!='RB'           
	           
	end
go

IF EXISTS (SELECT * FROM sys.triggers WHERE object_id = OBJECT_ID(N'[dbo].[trig_lineitemupdate]'))
BEGIN
	
	drop TRIGGER [dbo].[trig_lineitemupdate]
END

go

CREATE TRIGGER [dbo].[trig_lineitemupdate]
ON [dbo].[lineitem] For UPDATE

As 
DECLARE 
	@acctno char(12) ,@itemno varchar (10),@stocklocn smallint,@oldcontractno varchar (10),
	@error varchar(256), @category integer, @newcontractno varchar (10)

SELECT
	@acctno = inserted.acctno,
	@itemno = inserted.itemno,
	@stocklocn = inserted.stocklocn,
	@newcontractno = ISNULL(inserted.contractno,''),
	@oldcontractno = ISNULL(deleted.contractno,'')
FROM inserted, deleted
WHERE inserted.acctno = deleted.acctno and inserted.itemno = deleted.itemno and 
		inserted.stocklocn = deleted.stocklocn and deleted.contractno != inserted.contractno and
		inserted.parentitemno = deleted.parentitemno -- NM 09/04/2009 to avoid an issue in Oracle Export (CR996)
		
IF @oldcontractno != '' and  @newcontractno = ''
BEGIN
	Select @category = category From stockitem Where itemno = @itemno and stocklocn = @stocklocn
	
	--IF @category in (12, 82) --warranty categories
	IF @category in (select distinct reference from code where category ='WAR') --warranty categories  --IP - 30/10/09 - CoSACS Improvement - Remove hardcoded discounts & warranties
	BEGIN
		SET @error =' error blank contract saving ' + @acctno + ' ' +  @itemno
  		RAISERROR(@error, 16, 1) 
	END
END

go 
