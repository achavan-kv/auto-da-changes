/*
	Who		When		What
	------- ----------- -----------------------------------------------------------------
	GAJ		10/08/2005  Initial creation
*/
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_LineItemBfCollectionGetSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_LineItemBfCollectionGetSP]
GO


CREATE PROCEDURE 	dbo.DN_LineItemBfCollectionGetSP
-- ================================================
-- Project      : CoSACS .NET
-- File Name    : DN_LineItemBfCollectionGetSP.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Get Lineitem before Collection details  
-- Author       : ??
-- Date         : ??
--
-- This procedure will retrieve the value before collection.
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 18/05/10 jec UAT160 EW lineitem not updated after cancel reversed
-- 18/05/11  IP/NM 	RI Integration changes - CR1212 - #3627 - Changed joins to join on ItemID	
-- 06/06/11  IP  CR1212 - RI - #3806 - Return WarrantyID
-- 07/08/13  IP #14477 - Source column on AgreementAudit incorrect
-- 09/08/13  IP #14477 - Source column incorrect for GRT Exchange when cancelling collection note.
-- ================================================
	-- Add the parameters for the stored procedure here
			@acctno varchar(12),
			--@itemNo varchar(8),
			@itemID int,
			@agrmtno int,
			@contractno varchar(10),
			@cancellation smallint OUT,
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code
	SELECT	L.AcctNo,
			L.AgrmtNo,
			--L.ItemNo,
			S.IUPC as ItemNo,											--IP - 20/05/11 - CR1212 - RI - #3627	
			L.Quantity,
			L.Price,
			L.OrdVal,
			L.ContractNo,
			--ISNULL(E.WarrantyNo, '') as WarrantyNo,
			ISNULL(si.IUPC, '') as WarrantyNo,				--IP - CR1212 - RI - #3806 			
			ISNULL(E.WarrantyLocn, 0) as WarrantyLocn,
			ISNULL(E.ContractNo, '') as ExchangeContractNo,
			ISNULL(E.WarrantyID,0) as WarrantyID,			--IP - CR1212 - RI - #3806 	
			L.ItemID										--IP - CR1212 - RI - #3806 	
	FROM    LineItemBfCollection L
	LEFT JOIN	Exchange E
	ON		L.Acctno = E.acctno    
	AND     L.AgrmtNo = E.AgrmtNo
	--AND     L.ItemNo = E.itemNo
	AND     L.ItemID = E.ItemID								--IP/NM - 18/05/11 - CR1212 - #3627
	LEFT JOIN StockInfo si ON E.WarrantyID = si.ID			--IP - CR1212 - RI - #3806 	
	INNER JOIN StockInfo s ON L.ItemID = s.ID				--IP - 20/05/11 - CR1212 - RI - #3627
	WHERE	L.AcctNo    = @acctno
	AND    	L.AgrmtNo   = @agrmtno
	--AND    	L.ItemNo    = @itemNo		-- UAT160 retrieve all items for account
	--AND	    L.ContractNo = @contractno	-- UAT160
	
	SELECT	@cancellation = count(*)
	FROM LineItemBfCollection				--#14477
	--FROM	agreementbfcollection
	WHERE	acctno = @acctno
	AND NOT EXISTS(select * from Exchange	--#14477
					where Exchange.acctno = LineItemBfCollection.acctno
					and Exchange.ItemID = LineItemBfCollection.ItemID) 

	SET	@return = @@error
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

-- End End End End End End End End End End End End End End End End End End End End End End End End End
