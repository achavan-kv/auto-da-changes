SET QUOTED_IDENTIFIER OFF
GO

SET ANSI_NULLS ON
GO

IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = object_id('[dbo].[DN_DeliveryCashAndGoSearchSP]')
			AND OBJECTPROPERTY(id, 'IsProcedure') = 1
		)
	DROP PROCEDURE [dbo].[DN_DeliveryCashAndGoSearchSP]
GO
CREATE PROCEDURE [dbo].[DN_DeliveryCashandGoSearchSP]
	-- ================================================
	-- Project      : CoSACS .NET
	-- File Name    : DN_DeliveryCashandGoSearchSP.sql
	-- File Type    : MSSQL Server Stored Procedure Script
	-- Title        : Cash & Go Search 
	-- Author       : ??
	-- Date         : ??
	-- Version 		: 002
	-- This procedure will retrieve details of Cash & Go Sales.
	-- Change Control
	-- --------------
	-- Date      By  Description
	-- ----      --  -----------
	-- 14/05/10 jec UAT142 Returns details of items returned.
	-- 09/05/11 jec CR1212 RI Integration - use ItemID
	-- 29/03/12 jec #9854 Row returned when item has been returned (qty 2, 1 returned)
	-- 08/05/12 ip  #9608 - Customer Reprint - CR8520 - Return the payments for each invoice
	-- 17/05/12 ip  #9447 - CR1239 - Returning SalesPersonName, CashierName and CashierEmpeeNo
	-- 18/05/12 ip  #10144 - CR1239 - Returning TaxExempt
	-- 22/05/12 ip  #10156 - Increase transvalue for paymethod to include change if change exists on CashAndGoReceipt table.
	-- 23/05/12 ip  #10162 - Include Store Card payments
	-- 22/01/13 ip  #11624 - LW75335 - Non Stock Misc/Occasional item with 0 stock price was not being returned in 
	-- 10/07/2020 Snehalata - To Search New Invoice On Search Cash and Go Page because the invoice number is more than exsiting old invoice number
	--                        so sp will show  old invoice and new invoice also.Search Cash & Go Screen.  
	-- ================================================
	-- Add the parameters for the stored procedure here
	@buffno BIGINT
	,@branchno SMALLINT
	,@datefrom DATETIME
	,@dateto DATETIME
	,@searchWarrantyReturns SMALLINT
	,@return INT OUT
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @acctno VARCHAR(12)
	DECLARE @branchtext VARCHAR(6)

	SET ROWCOUNT 250
	/* this procedure is only for cash and go accounts-which used to be known as paid and taken
          and are identified by having fourth digit 5 and customer ID PAID & TAKEN-there is one for each branch*/
	SET @return = 0
	SET @branchtext = convert(VARCHAR, @branchno) + '5%'  
	SET @dateto = DATEADD(MINUTE, -1, DATEADD(dd, 1, @dateto))

	/*     this is no longer required because we are not just interested in the paid and taken account. This is because of 
       warranties on credit - JJ
       SELECT        @acctno =custacct.acctno 
       FROM          custacct, customer
       WHERE  acctno like @branchtext 
       AND           customer.custid ='PAID & TAKEN'
       AND           customer.custid = custacct.custid
  
       SET           @return =@@error
*/
	--ip - 22/01/13 - #11624 - LW75335
	DECLARE @staxId INT

	SELECT @staxId = id
	FROM stockinfo
	WHERE iupc = 'STAX'

	IF (@return = 0)
		--WARNING! ERRORS ENCOUNTERED DURING SQL PARSING!
	BEGIN
		CREATE TABLE #delivery (
			acctno VARCHAR(12)
			,agrmtno BIGINT
			,itemno VARCHAR(18)
			,-- RI 09/05/11 
			stocklocn SMALLINT
			,quantity FLOAT
			,stockprice MONEY
			,category VARCHAR(10)
			,contractno VARCHAR(10)
			,itemID INT -- RI 09/05/11 
			,InvoiceNumber BIGINT
			)

		IF (@buffno > 0) --should be an exact match
		BEGIN
			INSERT INTO #delivery
			SELECT d.acctno
				,d.agrmtno
				,d.itemno
				,d.stocklocn
				,sum(d.quantity) AS quantity
				,s.unitpricecash
				,s.category
				,d.contractno
				,d.itemID -- RI 09/05/11 
				,d.agrmtno AS InvoiceNumber
			FROM delivery d
			LEFT JOIN stockitem s ON d.itemId = s.ID -- RI 09/05/11  d.itemno = s.itemno
				AND d.stocklocn = s.stocklocn
			WHERE d.acctno LIKE @branchtext
				AND d.agrmtno = @buffno
			GROUP BY d.acctno
				,d.agrmtno
				,d.itemno
				,d.stocklocn
				,s.unitpricecash
				,s.category
				,d.contractno
				,d.itemID

			INSERT INTO #delivery
			SELECT d.acctno
				,d.agrmtno
				,d.itemno
				,d.stocklocn
				,sum(d.quantity) AS quantity
				,s.unitpricecash
				,s.category
				,d.contractno
				,d.itemID -- RI 09/05/11 
				,dd.AgreementInvNoVersion AS InvoiceNumber
			FROM invoiceDetails dd
			INNER JOIN delivery d ON d.acctno = dd.acctno
				AND d.agrmtno = dd.agrmtno
				AND d.stocklocn = dd.stocklocn
				AND d.itemno = dd.itemno
			LEFT JOIN stockitem s ON d.itemId = s.ID -- RI 09/05/11  d.itemno = s.itemno
				AND d.stocklocn = s.stocklocn
			WHERE cast(dd.datedel  as date) BETWEEN @datefrom
					AND @dateto
				AND dd.AgreementInvNoVersion = @buffno
				AND d.acctno LIKE @branchtext
			GROUP BY d.acctno
				,d.agrmtno
				,d.itemno
				,d.stocklocn
				,s.unitpricecash
				,s.category
				,d.contractno
				,d.itemID
				,dd.AgreementInvNoVersion

			SET @return = @@error
		END
		ELSE
		BEGIN
			INSERT INTO #delivery
			SELECT d.acctno
				,d.agrmtno
				,d.itemno
				,d.stocklocn
				,sum(d.quantity) AS quantity
				,s.unitpricecash
				,s.category
				,d.contractno
				,d.ItemID
				,d.agrmtno AS InvoiceNumber
			FROM delivery d
			LEFT JOIN stockitem s ON d.ItemID = s.ItemID
				AND d.stocklocn = s.stocklocn
			WHERE /*d.acctno = @acctno exists
                     AND    */ EXISTS (
					SELECT DISTINCT agrmtno
					FROM delivery
					WHERE delivery.acctno = d.acctno
						AND delivery.acctno LIKE @branchtext --@acctno
						AND delivery.agrmtno = d.agrmtno
						AND   cast(datetrans as date) BETWEEN @datefrom
							AND @dateto
						AND agrmtno != 1
					)
			GROUP BY d.acctno
				,d.agrmtno
				,d.itemno
				,d.stocklocn
				,s.unitpricecash
				,s.category
				,d.contractno
				,d.ItemId -- RI 09/05/11 

			-- Delete rows populating from above statement when only date fiters are applied and InvoiceNumber Filter is not applied.
			-- Becuase they are again populating in invoiceDetails table in sql statement next to this.
			DELETE
			FROM #delivery
			WHERE agrmtno IN (
					SELECT d.agrmtno
					FROM invoiceDetails dd
					INNER JOIN delivery d ON d.acctno = dd.acctno
						AND d.agrmtno = dd.agrmtno
						AND d.stocklocn = dd.stocklocn
						AND d.itemno = dd.itemno
					WHERE /*d.acctno = @acctno exists
											AND    */
						EXISTS (
							SELECT DISTINCT agrmtno
							FROM invoiceDetails
							WHERE invoiceDetails.acctno = d.acctno
								AND invoiceDetails.acctno LIKE @branchtext --@acctno
								AND invoiceDetails.agrmtno = d.agrmtno
								AND cast(dd.datedel as date) BETWEEN @datefrom
									AND @dateto
								AND d.agrmtno != 1
							)
					)

			INSERT INTO #delivery
			SELECT dd.acctno
				,dd.agrmtno
				,dd.itemno
				,dd.stocklocn
				,sum(dd.quantity) AS quantity
				,s.unitpricecash
				,s.category
				,dd.contractno
				,dd.ItemID
				,dd.AgreementInvNoVersion AS InvoiceNumber
			FROM invoiceDetails dd
			INNER JOIN delivery d ON d.acctno = dd.acctno
				AND d.agrmtno = dd.agrmtno
				AND d.stocklocn = dd.stocklocn
				AND d.itemno = dd.itemno
			LEFT JOIN stockitem s ON dd.ItemID = s.ItemID
				AND dd.stocklocn = s.stocklocn
			WHERE /*d.acctno = @acctno exists
                     AND    */ EXISTS (
					SELECT DISTINCT agrmtno
					FROM delivery
					WHERE delivery.acctno = dd.acctno
						AND delivery.acctno LIKE @branchtext --@acctno
						AND delivery.agrmtno = dd.agrmtno
						AND cast(delivery.datedel as date) BETWEEN @datefrom
							AND @dateto
						AND agrmtno != 1
					)
			GROUP BY dd.acctno
				,dd.agrmtno
				,dd.itemno
				,dd.stocklocn
				,s.unitpricecash
				,s.category
				,dd.contractno
				,dd.ItemId
				,dd.AgreementInvNoVersion -- RI 09/05/11 

			SET @return = @@error
		END

		CREATE CLUSTERED INDEX ix_cangdelivery ON #delivery (
			acctno
			,agrmtno
			,itemid
			,stocklocn
			)

		SELECT DISTINCT d.acctno
			,d.itemno
			,d.ItemID
			,-- RI 09/05/11
			isnull(s.itemdescr1, '') AS itemdescr1
			,d.stocklocn
			,quantity = CASE 
				WHEN isnull(l2.contractno, '') = ''
					THEN d.quantity
				ELSE l2.quantity
				END
			,l.price
			,transvalue = CASE 
				WHEN isnull(l2.contractno, '') = ''
					THEN (d.quantity * l.price)
				ELSE l.price
				END
			,d.agrmtno AS buffno
			,
			----warrantyno =     CASE
			----                 WHEN   (isnull(wb.warrantylength, 2) * 12) <= (datediff(dd, a.dateagrmt, getdate())/30.4375)  THEN   N'' 
			----                 ELSE   isnull(l2.itemno,'') 
			----          END,
			isnull(l2.itemno, '') AS warrantyno
			,-- #17290
			a.dateagrmt
			,s.taxrate
			,l2.contractno
			,1 AS valueControlled
			,d.category
			,convert(MONEY, 0) AS discount
			,a.empeenosale
			,l2.ItemID AS WarItemId
			,-- RI 09/05/11
			l2.parentitemID -- #17290 
			,d.InvoiceNumber
		INTO #temp
		FROM #delivery d
		INNER JOIN lineitem l ON l.acctno = d.acctno
			AND l.agrmtno = d.agrmtno
			AND
			--l.itemno = d.itemno AND
			l.itemID = d.itemID
			AND -- RI 09/05/11 
			l.stocklocn = d.stocklocn
		INNER JOIN agreement a ON d.agrmtno = a.agrmtno
			AND a.acctno = d.acctno
		LEFT JOIN lineitem l2 ON l.acctno = l2.acctno
			AND l.agrmtno = l2.agrmtno
			AND l.stocklocn = l2.parentlocation
			AND
			--l.itemno = l2.parentitemno AND
			l.itemID = l2.parentitemID
			AND -- RI 09/05/11 
			l2.contractno != ''
		------LEFT JOIN warrantyband wb ON
		--------wb.waritemno = l2.itemno
		------wb.itemID = l2.itemID              -- RI 09/05/11
		LEFT JOIN stockitem s ON
			--l.itemno = s.itemno AND
			l.itemID = s.ID
			AND -- RI 09/05/11
			l.stocklocn = s.stocklocn
		WHERE d.quantity > 0
			AND l.itemtype = 'S'
			AND (
				d.stockprice = 0
				AND d.category IN (
					'14'
					,'24'
					,'84'
					)
				)
			AND (
				(
					isnull(l2.contractno, '') = ''
					AND l2.quantity > 0
					) -- 71722, uat(4.3)172 Simone's changes, 4.3 merge
				OR (d.quantity > 0)
				)
		
		UNION
		
		SELECT DISTINCT d.acctno
			,d.itemno
			,d.ItemID
			,-- RI 09/05/11
			isnull(s.itemdescr1, '') AS itemdescr1
			,d.stocklocn
			,quantity = CASE 
				WHEN isnull(l2.contractno, '') = ''
					THEN d.quantity
				ELSE l2.quantity
				END
			,l.price
			,transvalue = CASE 
				WHEN isnull(l2.contractno, '') = ''
					THEN (d.quantity * l.price)
				ELSE l.price
				END
			,d.agrmtno AS buffno
			,
			----warrantyno =     CASE
			----          WHEN   (isnull(wb.warrantylength, 2)*12) <= (datediff(dd, a.dateagrmt, getdate())/30.4375)  THEN   N'' 
			----          ELSE   isnull(l2.itemno,'') 
			----          END,
			isnull(l2.itemno, '') AS warrantyno
			,-- #17290
			a.dateagrmt
			,s.taxrate
			,l2.contractno
			,0 AS valueControlled
			,d.category
			,convert(MONEY, 0) AS discount
			,a.empeenosale
			,l2.ItemID AS WarItemId
			,-- RI 09/05/11 
			l2.parentitemID -- #17290 
			,d.InvoiceNumber
		FROM #delivery d
		INNER JOIN lineitem l ON l.acctno = d.acctno
			AND l.agrmtno = d.agrmtno
			AND
			--l.itemno = d.itemno AND
			l.itemID = d.itemID
			AND -- RI 09/05/11 
			l.stocklocn = d.stocklocn
		INNER JOIN agreement a ON d.acctno = a.acctno
			AND d.agrmtno = a.agrmtno
		LEFT JOIN lineitem l2 ON l.acctno = l2.acctno
			AND l.agrmtno = l2.agrmtno
			AND l.stocklocn = l2.parentlocation
			AND
			--l.itemno = l2.parentitemno AND
			l.itemID = l2.parentitemID
			AND -- RI 09/05/11 
			l2.contractno != ''
			AND (
				l2.ordval > 0 -- #17290 ????  -- #9854 jec
				OR l2.quantity > 0
				) -- #18409   - free warranty          
			------LEFT JOIN warrantyband wb ON
			--------wb.waritemno = l2.itemno
			------wb.itemID = l2.itemID              -- RI 09/05/11
		LEFT JOIN stockitem s ON
			--l.itemno = s.itemno AND
			l.itemID = s.ID
			AND -- RI 09/05/11
			l.stocklocn = s.stocklocn
		WHERE d.quantity > 0
			AND l.itemtype = 'S'
			AND (
				d.stockprice > 0
				OR d.category NOT IN (
					'14'
					,'24'
					,'84'
					)
				)
			AND (
				(
					isnull(l2.contractno, '') = ''
					AND l2.quantity > 0
					) -- 71722, uat(4.3)172 Simone's changes, 4.3 merge
				OR (d.quantity > 0)
				)
		--IP - 22/01/13 - #11624 - LW75335       
		
		UNION
		
		SELECT DISTINCT d.acctno
			,d.itemno
			,d.ItemID
			,isnull(s.itemdescr1, '') AS itemdescr1
			,d.stocklocn
			,d.quantity
			,l.price
			,(d.quantity * l.price) AS transvalue
			,d.agrmtno AS buffno
			,'' AS warrantyno
			,a.dateagrmt
			,s.taxrate
			,'' AS contractno
			,0 AS valueControlled
			,d.category
			,convert(MONEY, 0) AS discount
			,a.empeenosale
			,0 AS WarItemId
			,0 AS parentitemID -- #17290
			,d.InvoiceNumber
		FROM #delivery d
		INNER JOIN lineitem l ON l.acctno = d.acctno
			AND l.agrmtno = d.agrmtno
			AND l.itemID = d.itemID
			AND l.stocklocn = d.stocklocn
		INNER JOIN agreement a ON d.agrmtno = a.agrmtno
			AND a.acctno = d.acctno
		LEFT JOIN stockitem s ON l.itemID = s.ID
			AND l.stocklocn = s.stocklocn
		WHERE d.quantity > 0
			AND l.itemtype = 'N'
			AND l.contractno = ISNULL(l.contractno, '')
			AND l.ParentItemID = ISNULL(l.ParentItemID, 0)
			AND d.stockprice >= 0
			AND d.ItemId != @staxId
			AND d.category NOT IN (
				SELECT code
				FROM code
				WHERE category = 'PCDIS'
				)
			AND d.category NOT IN (
				SELECT code
				FROM code
				WHERE category = 'WAR'
				)
		ORDER BY d.agrmtno DESC

		-- uat(4.3)172 Simone's changes, 4.3 merge
		INSERT INTO #temp
		SELECT t.acctno
			,t.itemno
			,t.itemID
			,t.itemdescr1
			,t.stocklocn
			,d.quantity - SUM(t.quantity)
			,t.price
			,(d.quantity - SUM(t.quantity)) * t.price
			,buffno
			,''
			,dateagrmt
			,taxrate
			,''
			,valuecontrolled
			,t.category
			,Discount
			,empeenosale
			,t.WarItemID
			,t.ParentItemID -- #17290
			,d.InvoiceNumber
		FROM #temp t
		INNER JOIN #delivery d ON t.buffno = d.agrmtno
			AND t.acctno = d.acctno
			--and t.itemno=d.itemno
			AND t.itemID = d.itemID -- RI 09/05/11
			AND t.stocklocn = d.stocklocn
			AND t.quantity != d.quantity
		GROUP BY t.acctno
			,t.itemno
			,t.itemID
			,t.itemdescr1
			,t.stocklocn
			,d.quantity
			,-- RI 09/05/11
			t.price
			,buffno
			,dateagrmt
			,taxrate
			,valuecontrolled
			,t.category
			,Discount
			,empeenosale
			,t.WarItemID
			,t.ParentItemID -- #17290
			,d.InvoiceNumber

		UPDATE #temp
		SET discount = ISNULL(abs(l.ordval), 0)
		FROM lineitem l
			,stockitem s
		WHERE #temp.acctno = l.acctno
			AND #temp.buffno = l.agrmtno
			AND #temp.itemno = l.parentitemno
			--AND  l.itemno = s.itemno
			AND l.itemID = s.ID
			AND l.stocklocn = s.stocklocn -- RI 09/05/11
			--AND  s.category IN(36, 37, 38, 46, 47, 48, 86, 87, 88)
			AND s.category IN (
				SELECT code
				FROM code
				WHERE category = 'PCDIS'
				) --IP - 29/10/09 - CoSACS Improvement - Remove hardcoded discounts & warranties

		SELECT DISTINCT t.acctno
			,itemno
			,itemdescr1
			,stocklocn
			,quantity
			,price
			,transvalue - discount AS transvalue
			,discount
			,buffno
			,InvoiceNumber AS InvoiceNo
			,warrantyno
			,dateagrmt
			,taxrate
			,contractno
			,valueControlled
			,empeenosale
			,itemID
			,WarItemID
			,c1.FullName AS SalesPersonName
			,--IP - 17/05/12 - #9447 - CR1239  
			ISNULL(r.CashierEmpeeNo, 0) AS CashierEmpeeNo
			,--IP - 17/05/12 - #9447 - CR1239  
			ISNULL(c2.FullName, '') AS CashierName
			,--IP - 17/05/12 - #9447 - CR1239  
			ISNULL(r.TaxExempt, 0) AS TaxExempt
			,--IP - 18/05/12 - #10144 - CR1239  
			ISNULL(r.Change, 0) AS Change
			,--IP - 21/05/12 - #10145 - CR1239   
			ParentItemId
			,-- #17290
			a.accttype --#18435
		FROM #temp t
		INNER JOIN acct a ON t.acctno = a.acctno --#18435 
		INNER JOIN Admin.[User] c1 ON t.empeenosale = c1.Id --IP - 17/05/12 - #9447 - CR1239  
		LEFT JOIN CashAndGoReceipt r ON t.acctno = r.acctno
			AND t.buffno = r.agrmtno --IP - 17/05/12 - #9447 - CR1239             
		LEFT JOIN Admin.[User] c2 ON c2.Id = r.CashierEmpeeNo --IP - 17/05/12 - #9447 - CR1239  
		WHERE quantity != 0 -- uat(4.3)172 Simone's changes, 4.3 merge  

		--IP - 08/05/12 - #9608 - CR8520 - Return the payments for each invoice
		SELECT DISTINCT f.acctno
			,f.agrmtno
			,CASE 
				WHEN f.paymethod = isnull(cg.PayMethod, 0)
					AND isnull(cg.Change, 0) > 0
					THEN Abs(f.transvalue) + cg.Change
				ELSE Abs(f.transvalue)
				END AS value
			,--IP - 22/05/12 - #10156
			f.datetrans
			,f.bankcode
			,f.bankacctno
			,f.chequeno
			,f.paymethod
			,c.codedescript
		FROM fintrans f
		INNER JOIN #temp t ON f.acctno = t.acctno
			AND f.agrmtno = t.buffno
		LEFT JOIN CashAndGoReceipt cg ON f.acctno = cg.acctno
			AND f.agrmtno = cg.agrmtno --IP - 22/05/12 - #10156
		INNER JOIN code c ON f.paymethod = c.code
		WHERE c.category = 'FPM'
			AND f.transtypecode IN (
				'PAY'
				,'SCT'
				) --IP - 23/05/12 - #10162 - Include Store Card payments
	END

	SET ROWCOUNT 0
END

GO
