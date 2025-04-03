SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

IF  EXISTS (SELECT 1 
	FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DN_LineItemAuditUpdateSP]') 
	AND type IN (N'P', N'PC'))
DROP PROCEDURE [dbo].[DN_LineItemAuditUpdateSP]

GO

CREATE PROCEDURE 	[dbo].[DN_LineItemAuditUpdateSP]
-- ================================================
-- Version:		<002> 
-- ========================================================================
-- Project      : CoSACS .NET
-- File Name    : DN_LineItemAuditUpdateSP.prc
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Line Item Audit Update
-- Author       : ??
-- Date         : ??
--
-- This procedure will update the LineItemAudit table.
-- 
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 21/11/07  jec UAT114 add ParentItemno and ParentLocation to LineItemAudit
-- 05/02/10  ip  CR1072 - 3.1.12 - add DelNoteBranch to LineItemAudit
-- 20/04/11  jec CR1212 RI Integration - set RunNo=0
-- 27/04/11 jec CR1212 RI Integration - Use ItemID instead of ItemNo
-- 17/05/11 jec CR1212 Use first 8 chars of IUPC
-- 27/11/19 aa CR changed the substring logic for itemno which extracts only 8 characters to 18 characters
-- 30/07/20  Zensar Optimization changes : Changed Select * to Top 1 'a' in Exists and Non Exists Statement
--										   
-- ================================================
			@acctno varchar(12),
			@agrmtno int,
			@empeenochange int,
			--@itemno varchar(8),
			@itemID int,
			@stocklocn smallint, 
			@quantitybefore float,
			@quantityafter float,
			@valuebefore money,
			@valueafter money,
			@taxamtbefore float,
			@taxamtafter float,
			@datechange datetime,
			@contractno varchar(10),
			@source varchar(15),
			--@parentItemNo varchar(8),		-- jec 21/11/07
			@parentitemID int,
			@parentStockLocn smallint,		-- jec 21/11/07
			@delnotebranch smallint,		-- ip - 05/02/10 - CR1072 - 3.1.12
			@salesBrnNo smallint = null,			--ip - 24/05/11 - CR1212 - RI - #3651
			@return int OUTPUT
		
AS

	SET 	@return = 0			--initialise return code
		-- 69231 RDB 23/10/07 when saving STAX two records are saved with all
		-- where clause parameters the same, the first is instantly overwritten
		-- the datechange is not enough, have added valuebefore and valueafter
		-- to primary key and removed update code (should not be updating an audit table!)
		

	-- 67977 RD 22/02/06 Added update for new columns taxamtbefore and taxamtafter
/*
	UPDATE	LineItemAudit
	SET		QuantityBefore = ISNULL(@quantitybefore,0),
			QuantityAfter = ISNULL(@quantityafter,0),
			ValueBefore = ISNULL(@valuebefore,0),
			ValueAfter = ISNULL(@valueafter,0),
			TaxAmtBefore = ISNULL(@taxamtbefore,0),
			TaxAmtAfter = ISNULL(@taxamtafter,0),
			Source = @source
	WHERE	acctno = @acctno
	AND		agrmtno = @agrmtno
	AND		empeenochange = @empeenochange
	AND		itemno = @itemno
	AND		stocklocn = @stocklocn
	AND		contractno = @contractno
	AND		datechange = @datechange

	IF(@@rowcount = 0)
	BEGIN
*/
	DECLARE @ItemNo VARCHAR(18),@ParentItemNo VARCHAR(18)		-- RI 
	SELECT @ItemNo= IUPC FROM stockInfo s WHERE ID=@itemID
	SELECT @ParentItemNo= ISNULL(IUPC,'') FROM stockInfo s WHERE ID=@ParentItemID
	
-- Don't insert if already a record in in the last 15 seconds or duplicate error on insert
	IF  NOT EXISTS (SELECT TOP 1 'a' FROM LineitemAudit l 
	WHERE l.acctno= @acctno		--AND l.itemno= @itemno		-- RI
			  AND l.ItemID= @itemID 
			  AND l.stocklocn=  @stocklocn AND l.agrmtno = @agrmtno
			  AND l.contractno = @contractno AND Datechange > DATEADD(second,-15,@datechange)
			  AND l.QuantityAfter = @quantityafter  )
		INSERT
		INTO		LineItemAudit
				(acctno, agrmtno, empeenochange, itemno, stocklocn, 
				quantitybefore, quantityafter, valuebefore, valueafter,
				taxamtbefore,taxamtafter,contractno, datechange, source, 
				ParentItemno, ParentLocation, DelNoteBranch,RunNo,ItemID,ParentItemID, SalesBrnNo)		-- RI	-- jec 20/04/11		--IP - 24/05/11 - CR1212 - RI - #3651
		VALUES	(@acctno, @agrmtno, @empeenochange, LEFT(ISNULL(@itemno,''),18), @stocklocn,		-- RI	-- jec 17/05/11
				ISNULL(@quantitybefore,0), ISNULL(@quantityafter,0), ISNULL(@valuebefore,0), ISNULL(@valueafter,0),
				ISNULL(@taxamtbefore,0), ISNULL(@taxamtafter,0),@contractno, @datechange, @source, 
				LEFT(ISNULL(@parentItemNo, ''),18), ISNULL(@parentStockLocn, 0), @delnotebranch,0,		-- RI	-- jec 17/05/11
				@itemID,@parentitemID, @salesBrnNo)					-- RI --IP - 24/05/11 - CR1212 - RI - #3651
	--END	

	IF (@@ERROR != 0)
	BEGIN
		SET @return = @@ERROR
	END

GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO