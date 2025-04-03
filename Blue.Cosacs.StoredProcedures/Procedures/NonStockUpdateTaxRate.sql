IF EXISTS (SELECT * 
		   FROM sysobjects
		   WHERE xtype = 'P'
		   AND name = 'NonStockUpdateTaxRate')
BEGIN 
	DROP PROCEDURE NonStockUpdateTaxRate
END
GO

CREATE PROCEDURE NonStockUpdateTaxRate

-- ================================================
-- Project      : CoSACS .NET
-- File Name    : NonStockUpdateTaxRate.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Updates Non Stock Tax Rates
-- Author       : Stephen Chong
-- Date         : Feb 2011
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 02/03/11  IP  #3251 - Update taxrate for Non Stock from Code Maintenance
-- ================================================
	-- Add the parameters for the stored procedure here
AS
BEGIN
	
	--Now update the taxrate from code maintenance
	UPDATE stockinfo
	SET taxrate = 
	isnull((select cast(c.codedescript as float)
					from code c
					where c.category = 'TXR'
					and c.code = s.itemno),0)
	from stockinfo s 
	where itemtype = 'N'
		AND category not in (select code from code where category = 'WAR')
	
	UPDATE StockInfo
	SET taxrate = (SELECT taxrate 
					FROM country)
	WHERE itemtype = 'N'
	AND TaxRate = 0
	AND NOT EXISTS (SELECT * 
				    FROM code
					WHERE code.category = 'TXR'
					AND code.code = stockinfo.itemno)
	AND category not in (select code from code where category = 'WAR')

END
GO



