SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_ExchangeGetSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_ExchangeGetSP]
GO

CREATE PROCEDURE 	dbo.DN_ExchangeGetSP
			@acctno varchar(12),
			@agrmtno int,
			@return int OUTPUT
			
-- **********************************************************************
-- Title: DN_ExchangeGetSP.sql
-- Developer: David Richardson
-- Date: 05/05/2006
-- Purpose: 

-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 29/01/10 IP  LW 72136 
-- **********************************************************************

AS

	SET 	@return = 0			--initialise return code
   	
	DECLARE	@months smallint
   	
	SELECT	@months = CONVERT(int, value)
	FROM	countrymaintenance
	WHERE	codename = 'warrantyvalidity'
	
	-- It is possible for there to be more than one entry in Exchange
	-- for the same item, so use DISTINCT
	SELECT DISTINCT
	        l.ItemID, 
			l.contractno,
			l.stocklocn, 
			l.ParentItemID,
			l.parentlocation,
			s.refcode,
			s.iupc as ItemNo, --#16292
			isnull(e.LinkIrwId,0) as LinkIrwId,  --#17290
			isnull(e.LinkIrwContractno, '') as LinkIrwContractno	--#17290
	FROM lineitem l
	INNER JOIN delivery d ON l.acctno = d.acctno
	INNER JOIN Exchange e ON l.acctno = e.AcctNo
	INNER JOIN stockitem s ON l.ItemID = s.ItemID
	WHERE l.acctno = @acctno
	AND   	d.agrmtno = @agrmtno
	AND		l.agrmtno = @agrmtno
	AND   	d.itemid = l.itemid
	AND   	d.stocklocn = l.stocklocn
	AND   	d.contractno = l.contractno
	AND   	e.agrmtno = l.agrmtno
	AND   	e.WarrantyID = l.itemid
	AND   	e.WarrantyLocn = l.stocklocn
	AND   	(e.contractno = l.contractno
                or (l.ContractNo = SUBSTRING(e.ContractNo,1, DATALENGTH(e.ContractNo)-1)))   --Migrated Free warranties do not exist on Lineitem
	AND   	d.datedel > DATEADD(month, -@months, GETDATE()) 
	AND 	l.quantity != 0
	AND		s.stocklocn = l.stocklocn
	AND (l.ParentItemID = 0
            or (l.ContractNo = SUBSTRING(e.ContractNo,1, DATALENGTH(e.ContractNo)-1))) --Migrated Free warranties do not exist on Lineitem
    AND e.ContractNo != ''

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

