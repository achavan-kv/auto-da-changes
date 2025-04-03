

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_SpiffSelectionSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_SpiffSelectionSP]
GO

CREATE PROCEDURE 	dbo.DN_SpiffSelectionSP
-- ================================================
-- Project      : CoSACS .NET
-- File Name    : DN_SpiffSelectionSP.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Spiff Selection
-- Author       : ??
-- Date         : July 2006
--
-- This procedure retrieve items that have spiffs.
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 02/07/08	 jec return item price  UAT440
-- 27/07/11  jec CR1254 RI Changes
-- 28/07/11  jec CR1254 correct check for Higher Spiff
-- 01/08/11  jec CR1254 add itemId
-- ================================================
	-- Add the parameters for the stored procedure here
				@itemno varchar(18),	
				@stocklocn smallint,
				@type char(3),
				@itemId int,				-- RI
				@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	DECLARE @value money,
	        @percentage float,
	        @RepoValue money,				-- RI
	        @RepoPercentage float,
	        @spiffComm MONEY,
	        --@RepoSpiffComm MONEY,
	        @currentdate datetime
	        
    SET @currentdate = GETDATE() 	        

	IF @type = 'PS'
	BEGIN
		SET @currentdate = CONVERT(DATETIME,CONVERT(VARCHAR(20),@currentdate,105),105)
		
		IF EXISTS(SELECT 1 FROM	SalesCommissionRates WHERE itemtext = @itemno AND commissiontype = 'SP')
		BEGIN
			SELECT	@value = value,
			        @percentage = percentage,
			        @Repovalue = Repovalue,					-- RI
			        @RepoPercentage = RepoPercentage,
			        @spiffComm= case
						when i.repossessedItem=0 then s.CreditPrice*percentage/100
						else sr.CreditPrice*RepoPercentage/100
						end
			FROM	SalesCommissionRates r INNER JOIN StockPrice s on r.ItemId=s.ID and s.branchNo=@stocklocn		-- RI
											INNER JOIN StockPrice sr on r.RepoItemId=sr.ID and s.branchNo=@stocklocn,
					StockInfo i
			WHERE	itemtext = @itemno
			and i.ID=@itemid			-- RI
			AND     commissiontype = 'SP'
			AND     @currentdate BETWEEN datefrom AND dateto
			
			SELECT	DISTINCT
			        c.itemtext,
					s2.itemdescr1 + ' ' + s2.itemdescr2 as itemdescription,
					s2.unitpricehp as unitprice,
					s2.unitpricecash as cashprice,
					case						
						when s2.RepossessedItem=0 then c.value
						else c.RepoValue 
						end as Value,
					case 
						when s2.RepossessedItem=0 then c.percentage
						else c.RepoPercentage 
						end as Percentage,
					s2.ItemId				-- RI
			FROM	SalesCommissionRates c, StockItem s, StockItem s2, StockPrice p2, StockPrice pr2				-- RI
			WHERE	s.IUPC = @itemno				-- RI s.itemno = @itemno
			AND		c.itemtext = s2.IUPC		-- RI c.itemtext = s2.itemno
			AND		s.category = s2.category
			AND		s2.stocklocn = @stocklocn
			and		s2.prodstatus!='D'				-- RI
			AND		c.commissiontype = 'SP'
			AND		(((c.value > @spiffComm OR c.percentage*p2.CreditPrice/100 > @spiffComm) and s2.RepossessedItem=0)			-- RI
				or ((c.RepoValue > @spiffComm OR c.RepoPercentage*pr2.CreditPrice/100 > @spiffComm) and s2.RepossessedItem=1))
			AND		s2.stockfactavailable > 0
			AND		c.itemtext != @itemno
			and     s2.Id=p2.ID and p2.BranchNo=@StockLocn			-- RI
			and     s2.Id=pr2.ID and pr2.BranchNo=@StockLocn
		END
	END

	IF @type = 'PA'
	BEGIN
		SELECT	DISTINCT
		        c.itemtext,
				s2.itemdescr1 + ' ' + s2.itemdescr2 as itemdescription,
				s2.unitpricehp as unitprice,
				s2.unitpricecash as cashprice,
				case 
						when s2.RepossessedItem=0 then c.value
						else c.RepoValue 
						end as Value,
					case 
						when s2.RepossessedItem=0 then c.percentage
						else c.RepoPercentage 
						end as Percentage,
				s2.ItemId				-- RI
		FROM	SalesCommissionRates c, StockItem s, StockItem s2
		WHERE	s.IUPC = @itemno			-- RI s.itemno = @itemno
		AND		c.itemtext = s2.IUPC		-- RI c.itemtext = s2.itemno
		AND		s.category = s2.category
		AND		s2.stocklocn = @stocklocn
		and		s2.prodstatus!='D'			-- RI
		AND		c.commissiontype = 'SP'
		AND		s2.stockfactavailable > 0
		AND		c.itemtext != @itemno
	END

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO


-- End End End End End End End End End End End End End End End End End End End End End End End End End End End End End End