
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT * FROM SYSOBJECTS 
           WHERE NAME = 'ServiceGetItemPrintInfo'
           AND xtype = 'P')
BEGIN 
DROP PROCEDURE ServiceGetItemPrintInfo
END
GO

CREATE PROCEDURE ServiceGetItemPrintInfo
-- ================================================
-- Project      : CoSACS .NET
-- File Name    : ServiceGetItemPrintInfo.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Service Get Print Info
-- Author       : ??
-- Date         : ??
--
-- This procedure will get the Service Item details for printing
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 25/07/11  jec CR1212 RI Integration
-- 07/12/12  ip  #11791 - Changed query to look at new service table.
-- ================================================
	-- Add the parameters for the stored procedure here
@Acctno VARCHAR(12),
@itemno VARCHAR(18),			-- RI
@stocklocn varchar(3),

@return INT OUTPUT

AS
BEGIN

DECLARE @itemID INT

SET @itemID = (SELECT s.ID FROM dbo.StockInfo s INNER JOIN dbo.StockQuantity q ON s.ID = q.ID
				WHERE s.IUPC = @itemno
				AND q.stocklocn = @stocklocn)
				
	--SELECT DISTINCT serialno,ModelNo 
	--FROM SR_ServiceRequest
	--WHERE AcctNo = @Acctno
	--AND ProductCode = @itemno 
	--AND StockLocn = @stocklocn
	
	SELECT DISTINCT ISNULL(ItemSerialNumber,'') AS serialno, ISNULL(ItemModelNumber,'') AS ModelNo
	FROM service.Request
	WHERE Account = @Acctno
	AND ItemId = @itemID
	AND ItemStockLocation = @stocklocn

END

go
-- End End End End End End End End End End End End End End End End End End End End End End End End End 

