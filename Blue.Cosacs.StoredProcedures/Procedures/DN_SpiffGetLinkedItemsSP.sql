
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_SpiffGetLinkedItemsSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_SpiffGetLinkedItemsSP]
GO

CREATE PROCEDURE 	dbo.DN_SpiffGetLinkedItemsSP
-- ================================================
-- Project      : CoSACS .NET
-- File Name    : DN_SpiffGetLinkedItemsSP.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Get Linked Spiff Items
-- Author       : ??
-- Date         : July 2006
--
-- This procedure retrieve linked items that have spiffs.
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 02/07/08	 jec return item price  UAT440
-- 27/07/11  jec CR1254 RI Changes
-- 01/08/11  jec Remove stock available check  (Santosh)
-- ================================================
	-- Add the parameters for the stored procedure here
				@itemno varchar(18),			-- RI
				@stocklocn smallint,
				@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	DECLARE @currentdate datetime
      
CREATE TABLE #LinkedItems
	(
		LinkedItem varchar(18),					
		ItemDescription varchar(65),
		Percentage float,
		Value MONEY,
		ItemId INT			-- RI
	)	

	SET @currentdate = CONVERT(DATETIME,CONVERT(VARCHAR(20),GETDATE(),105),105)
	
	INSERT INTO #LinkedItems(LinkedItem, ItemDescription, Percentage, Value,ItemId)			-- RI
	SELECT	Item2,
			'',
			Percentage,
			Value,
			ItemId2						-- RI
	FROM	SalesCommissionMultiSPIFFRates
	WHERE	Item1 = @itemno
	AND     @currentdate BETWEEN datefrom AND dateto

	INSERT INTO #LinkedItems(LinkedItem, ItemDescription, Percentage, Value,ItemId)			-- RI)
	SELECT	Item3,
			'',
			Percentage,
			Value,
			ItemId3						-- RI
	FROM	SalesCommissionMultiSPIFFRates
	WHERE	Item1 = @itemno
	AND     @currentdate BETWEEN datefrom AND dateto

	INSERT INTO #LinkedItems(LinkedItem, ItemDescription, Percentage, Value,ItemId)			-- RI
	SELECT	Item4,
			'',
			Percentage,
			Value,
			ItemId4						-- RI
	FROM	SalesCommissionMultiSPIFFRates
	WHERE	Item1 = @itemno
	AND     @currentdate BETWEEN datefrom AND dateto

	INSERT INTO #LinkedItems(LinkedItem, ItemDescription, Percentage, Value,ItemId)			-- RI
	SELECT	Item5,
			'',
			Percentage,
			Value,
			ItemId5						-- RI
	FROM	SalesCommissionMultiSPIFFRates
	WHERE	Item1 = @itemno
	AND     @currentdate BETWEEN datefrom AND dateto

	DELETE 
	FROM	#LinkedItems
    WHERE NOT EXISTS(SELECT	1
		             FROM	stockitem s
		             WHERE	s.ItemId = #LinkedItems.ItemId					-- RI s.itemno = #LinkedItems.LinkedItem
		             AND	s.stocklocn = @stocklocn
					 --AND	s.stockfactavailable > 0					-- 
					 )

	UPDATE	#LinkedItems
	SET		ItemDescription = s.itemdescr1 + ' ' + s.itemdescr2
	FROM	stockitem s
	WHERE	s.IUPC = #LinkedItems.LinkedItem					-- RI s.itemno = #LinkedItems.LinkedItem
	AND		s.stocklocn = @stocklocn

	SELECT	LinkedItem itemno,	
			ItemDescription,
			unitpricehp as unitprice,			-- jec 02/07/08
			unitpricecash as cashprice,			-- jec 02/07/08
			Value,
			Percentage,
			#LinkedItems.ItemId				-- RI	
	FROM	#LinkedItems INNER JOIN stockitem s on s.ItemId = #LinkedItems.ItemId				-- RI s.itemno = #LinkedItems.LinkedItem	
	WHERE	s.stocklocn = @stocklocn
 

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

-- End End End End End End End End End End End End End End End End End End End End End End End End End End End End End End