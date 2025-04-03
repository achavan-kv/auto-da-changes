SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_WarrantiesGetProductSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_WarrantiesGetProductSP]
GO

CREATE PROCEDURE 	dbo.DN_WarrantiesGetProductSP
--------------------------------------------------------------------------------
--
-- Project      : eCoSACS r 2002 Strategic Thought Ltd.
-- File Name    : DN_WarrantiesGetProductSP.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Get Related Warranties
-- Author       : ??
-- Date         : ??
--
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 02/06/11 jec  CR1212 RI Integration - #3786 Not all the related warranties are displayed in the warranty pop up screen
-- 16/06/11 ip   CR1212 RI Integration - #3956 Exclude renewal warranties 
--------------------------------------------------------------------------------

    -- Parameters
		@itemId int,
		@location smallint,
		@unitPrice float,
        @refcode varchar(3)=N'',
		@paidAndTaken smallint,
		@return int OUTPUT
AS

	SET @return = 0
    /*refcode although stored as varchar 3 is in fact varchar 2-but for some reason on the import a
	line character is stored as the third digit*/

    declare @country char(1)
    select @country = countrycode from country

	IF(@refCode != '')
		SET @refcode = @refcode + '%'

	SELECT DISTINCT 
		W.waritemno,
		S1.itemdescr1 as 'Description',
		W.warrantylength 'Duration',
		S1.unitpricecash as 'Cash Price',
		S1.unitpricehp as 'HP Price',
		W.refcode as 'Code',
		S.stocklocn as 'Location',
		S1.IUPC,
		W.ItemID  
	FROM WarrantyBand W 
	INNER JOIN StockItem S on LEFT(S.RefCode,3) = LEFT(W.RefCode,3)
	INNER JOIN StockItem S1 ON S1.ItemID = W.ItemID
	WHERE S.StockLocn = @location AND	
		  S1.StockLocn = @location AND	
		  (@unitprice = 0 OR @unitPrice BETWEEN W.MinPrice AND W.MaxPrice) AND	
		  --(RIGHT(S1.ItemNo,1) <> '1' or @country = 'S') AND	-- RI jec --68252 Singapore use 1 year warranties	
		  (@itemId = 0 OR S.ID = @itemId) AND 
		  (@refCode = '' OR s.RefCode like @refcode)
		  AND isnull(nullif(S1.warrantyrenewalflag,''), 'N') = 'N'					--IP - 16/06/11 - CR1212 - RI - #3956 - Exclude renewal warranties
	
	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

-- End End End End End End End End End End End End End End End End End End End End End End End End End End End