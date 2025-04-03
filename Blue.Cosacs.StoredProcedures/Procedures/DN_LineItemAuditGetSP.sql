if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_LineItemAuditGetSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_LineItemAuditGetSP]
GO

CREATE PROCEDURE 	dbo.DN_LineItemAuditGetSP			
-- **********************************************************************
-- Title: DN_LineItemAuditGetSP.sql
-- Developer: David Richardson
-- Date: 05/05/2006
-- Purpose: Procedure returns LineItemAudit for an account

-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 05/02/10  IP  CR1072 - 3.1.12 - Display DelNoteBranch on Account Details - Line Item Audit tab
-- 31/05/11  jec CR1212 RI Integration  - get item details from stockinfo
-- **********************************************************************
	-- Add the parameters for the stored procedure here
			@acctno varchar(12),
			@rowcount int,
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	SET		ROWCOUNT @rowcount

    -- 67977 RD 22/02/06 Added taxamt before and after
	SELECT	LI.acctno,
			LI.agrmtno,
			LI.empeenochange,
			isnull(u.FullName, 'Unknown') AS EmployeeName,
			--LI.itemno,
			s.IUPC as itemno,			-- RI
			LI.stocklocn,
			LI.delnotebranch, --IP - 05/02/10 - CR1072 - 3.1.12 
			LI.quantitybefore,
			LI.quantityafter,
			LI.valuebefore,
			LI.valueafter,
			LI.taxamtbefore,
			LI.taxamtafter,
			LI.datechange,
			LI.contractno,
			LI.ItemId
	FROM	LineItemAudit LI 
	LEFT OUTER JOIN Admin.[User] u ON LI.empeenochange = u.Id
			INNER JOIN StockInfo s on li.ItemID=s.ID			-- RI
	WHERE	acctno = @acctno
	ORDER BY	LI.datechange DESC

	SET		ROWCOUNT 0

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END
	
	Go
	
-- End End End End End End End End End End End End End End End End End End End End End End End End End End End End End End
