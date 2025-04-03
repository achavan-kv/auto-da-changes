SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_DeliveryAcctsLoadSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_DeliveryAcctsLoadSP]
GO

CREATE PROCEDURE [dbo].[DN_DeliveryAcctsLoadSP] 
/***********************************************************************************************************
--
-- Project      : CoSACS .NET
-- File Name    : DN_DeliveryAcctsLoadSP.PRC
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
-- 06/06/11  IP  CR1212 - RI - use ItemID
************************************************************************************************************/
			@acctno VARCHAR(13), 
			@user INT, 
			@branch INT, 
			@TimeLocked DATETIME OUTPUT, 
			@return INT OUTPUT 
AS
		SELECT @return = 0

	SET ROWCOUNT 250

	IF (ISNULL(@acctno,'') = '' OR @acctno = '000000000000')
		SELECT @acctno = '%'
	ELSE
		SELECT @acctno = @acctno+N'%'
	
	IF (@return = 0)
	BEGIN	
		select @TimeLocked = getdate()
		if exists (select * from accountlocking where acctno like @acctno and LockCount <= 0 )--AA performance
			DELETE 
			FROM	AccountLocking 
			--WHERE	LockCount <= 0  -- RD 20/09/05 67595 Modified to ensure that accounts are loaded correctly in the Print Delivery Screen
			WHERE	LockCount = 0  -- RD 20/09/05 67595 Modified to ensure that accounts are loaded correctly in the Print Delivery Screen --IP - 17/02/10 - CR1072 - LW 70230 - Delivery Fixes from 4.3
			AND  acctno LIKE @acctno
		-- Lock accounts that have lineitems to be delivered and that are not locked
		INSERT INTO accountlocking (acctno, lockedby, lockedat, lockcount)
		SELECT DISTINCT(l.acctno), @user, @TimeLocked, 1
		FROM lineitem l
      INNER JOIN lineitemosDelnotes d ON l.acctno =d.acctno and l.agrmtno = d.agrmtno 
             and l.contractno = d.contractno and l.stocklocn = d.stocklocn and l.contractno =d.contractno
		INNER JOIN acct ON d.acctno = acct.acctno
   	INNER JOIN agreement g on g.acctno =d.acctno and g.agrmtno =d.agrmtno
		--INNER JOIN stockitem s ON d.itemno = s.itemno AND d.stocklocn = s.stocklocn
		INNER JOIN StockInfo si ON d.ItemID = si.ID										--IP - 06/06/11 - CR1212 - RI
		INNER JOIN StockQuantity sq ON d.ItemID = sq.ID AND d.stocklocn = sq.stocklocn	--IP - 06/06/11 - CR1212 - RI
		--LEFT OUTER JOIN accountlocking a ON l.acctno = a.acctno --IP - 17/02/10 - CR1072 - LW 70230 - Delivery Fixes from 4.3
		WHERE --a.acctno is null --IP - 17/02/10 - CR1072 - LW 70230 - Delivery Fixes from 4.3
		d.acctno LIKE @acctno
		--AND l.qtydiff = 'Y' lineitemosDelnotes only has qtydiff = 'Y'
		AND d.DelNoteBranch = @branch --so not printed but should be based on the delivery not branch --SC 70230 09/10/08 --IP - 17/02/10 - CR1072 - LW 70230 - Delivery Fixes from 4.3
		AND acct.currstatus not in ('0', 'U', 'S') 
		--AND d.itemno not in ('DT','STAX') 	/*will be removing all non-stock items later*/  
		AND si.IUPC not in ('DT','STAX')												--IP - 06/06/11 - CR1212 - RI				  
		AND l.quantity != 0  
		AND l.Iskit = 0
		AND si.itemtype != 'N'															--IP - 06/06/11 - CR1212 - RI	
	  	AND G.holdprop = 'N'
        AND NOT EXISTS (SELECT c.AcctNo FROM Cancellation c WHERE c.AcctNo = d.AcctNo)
        and not exists (select a.acctno from accountlocking a where acct.acctno=a.acctno) --IP - 17/02/10 - CR1072 - LW 70230 - Delivery Fixes from 4.3
		--and not exists (select sch.acctno from schedule sch where sch.acctno=d.acctno and sch.itemno=d.itemno) --IP - 17/02/10 - CR1072 - LW 70230 - Delivery Fixes from 4.3
		and not exists (select sch.acctno from schedule sch where sch.acctno=d.acctno and sch.ItemID=d.ItemID) --IP - 17/02/10 - CR1072 - LW 70230 - Delivery Fixes from 4.3	--IP - 06/06/11 - CR1212 - RI
		
		IF @@error = 0	
		BEGIN
			-- Fetch address type from lineitem table
			SELECT 	DISTINCT 
					al.acctno, 
					isnull (l.deliveryaddress, '') as addtype, 
					ca.custid, 
					--l.itemno, 
					si.IUPC as itemno,							--IP - 06/06/11 - CR1212 - RI		
					l.quantity, 
				       	l.delqty, 
					l.notes as itemnotes, 
					l.stocklocn, 
					l.price, 
					l.ordval,
			       		l.datereqdel, 
					l.timereqdel, 
					l.dateplandel, 
					ag.empeenosale,  
					isnull (l.deliveryaddress, '') as deliveryaddress,
					--IP - 19/02/10 - CR1072 - LW 69770 - Printing Fixes from 4.3 - Merge
					' ' AS delorcoll,
					'        ' AS RetItemno,
					0 AS BuffNo ,
					0 AS BuffBranchno
			INTO 		#lines
			FROM 		accountlocking al
			INNER JOIN 	lineitem l ON al.acctno = l.acctno 
			INNER JOIN  StockInfo si ON l.ItemID = si.ID		--IP - 06/06/11 - CR1212 - RI
			INNER JOIN 	agreement ag ON al.acctno = ag.acctno 
			INNER JOIN 	custacct ca ON l.acctno = ca.acctno
			WHERE 	al.lockedby = @user
			AND 		CONVERT(char(27), al.lockedat, 109) = CONVERT(char(27), @TimeLocked, 109)
			AND 		al.lockedat = @TimeLocked
			AND 		ca.hldorjnt = 'H'
			AND 		l.itemtype != 'N'
			AND 		l.qtydiff = 'Y'
			AND l.delqty !=l.quantity --IP - 19/02/10 - CR1072 - LW 69770 - Printing Fixes from 4.3 - Merge
			AND l.DelNoteBranch = @branch  --IP - 19/02/10 - CR1072 - LW 69770 - Printing Fixes from 4.3 - Merge --SC 70230 09/10/08 --IP - 17/02/10 - CR1072 - LW 70230 - Delivery Fixes from 4.3

		SELECT @return = @@error
		END
	
		IF @@error = 0	
		BEGIN
			-- Catch any dodgy address types	
			UPDATE 	#lines
			SET 		addtype = 'H'
			WHERE 	NOT EXISTS (	SELECT custid 
							FROM 	  custaddress 
							WHERE  custaddress.custid = #lines.custid
         							AND 	   custaddress.addtype = #lines.addtype 
							AND 	    custaddress.datemoved is null )
			SELECT 	@return = @@error
		END 
 
   	IF @@error = 0	
		BEGIN
			delete from #lines where exists (select * from cancellation where cancellation.acctno = #lines.acctno)
	   END

		-- IP - 10/02/10 - CR1048 (Ref:3.1.4 & 3.1.5) Merged - Malaysia Enhancements (CR1072)
		-- CR1048 Concatenate Itemno's 
		declare @itemtable TABLE (acctno CHAR(12),items VARCHAR(100) ) 
		DECLARE @itemno varchar(8),@acctnox CHAR(12),@itemlist VARCHAR(100),@prevacctno CHAR(12)
		set @prevacctno=''
		set @itemlist=''

		DECLARE  itemcur cursor FOR select acctno,itemno from #lines
		OPEN itemcur

		FETCH NEXT FROM itemcur INTO @acctnox,@itemno
		WHILE @@FETCH_STATUS = 0
			BEGIN

			if @prevacctno!=@acctnox and @prevacctno!=''
			Begin
				insert into @itemtable 
					select @prevacctno,@itemlist
				set @itemlist=''	
			End
			set @prevacctno=@acctnox
			-- build itemlist
			set @itemlist=case when @itemlist='' then @itemlist+@itemno else @itemlist+','+@itemno end
			
			FETCH NEXT FROM itemcur INTO @acctnox,@itemno
			END
			-- insert last row
			insert into @itemtable 
				select @prevacctno,@itemlist
			
		CLOSE itemcur
		DEALLOCATE itemcur
		
		IF @@error = 0	
		BEGIN
			-- Return account and address details for display
			-- IP - 10/02/10 - CR1048 (Ref:3.1.4 & 3.1.5) Merged - Malaysia Enhancements (CR1072)
			SELECT 	DISTINCT 
					 SUBSTRING(l.acctno,1,3)+N'-'+SUBSTRING(l.acctno,4,4)+N'-'+SUBSTRING(l.acctno,8,4)+N'-'+SUBSTRING(l.acctno,12,1) as acctno,
					 i.items as Items,
					 l.empeenosale as EmpeenoSale,
					 l.DelorColl,
					 l.BuffBranchNo,        
					 l.BuffNo,
					 l.deliveryaddress,       
					 l.datereqdel,         
					 l.timereqdel, 

					 c.firstname,         
					 c.name,         
					 c.title,         
					 cad.cusaddr1,         
					 cad.cusaddr2,         
					 cad.cusaddr3,         
					 cad.cuspocode,         
					 convert(varchar(300),         
					 cad.notes) as cusnotes,
					 l.stocklocn,
					 --IP - 19/02/10 - CR1072 - LW 69770 - Printing Fixes from 4.3
					 l.DelorColl,
					 l.BuffBranchNo,
					 l.BuffNo
			FROM 		#lines l
			INNER JOIN 	customer c ON l.custid = c.custid
			INNER JOIN 	custaddress cad ON l.custid = cad.custid 
			AND 		l.addtype = cad.addtype
			INNER JOIN @itemtable i on l.acctno=i.acctno -- IP - 10/02/10 - CR1048 (Ref:3.1.4 & 3.1.5) Merged - Malaysia Enhancements (CR1072)
			WHERE	ISNULL(cad.datemoved,'1-January-1900') = '1-January-1900'
        			ORDER BY 	c.name

			SELECT 	@return = @@error
		END
	END

	SET ROWCOUNT 0
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

