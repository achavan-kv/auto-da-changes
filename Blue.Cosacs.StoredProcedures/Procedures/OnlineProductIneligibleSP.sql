SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[OnlineProductIneligibleSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[OnlineProductIneligibleSP]
GO

CREATE PROCEDURE 	dbo.OnlineProductIneligibleSP
/***********************************************************************************************************
--
-- Project      : CoSACS .NET
-- File Name    : OnlineProductIneligibleSP.SQL
-- File Type    : MSSQL Server Stored Procedure Script
-- Description  : Export Online Products
--				
-- Author       : John Croft
-- Date         : July 2013
--
--  
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------

************************************************************************************************************/
	
AS	
	declare @stocklocn smallint		
	select  @stocklocn= value from CountryMaintenance where codename='OnlineDistCentre'
	
	SELECT  UPPER(ISNULL(IUPC,'IUPC Missing')) AS 'Product Code', 
			Replace(ItemDescr1,',',' ') AS 'Desc 01',
			Replace(ItemDescr2,',',' ')  AS 'Desc 02',
			OnlineDateAdded as 'Online Date Added'	--,
			--prodstatus as 'Product Status'			-- #14499
	FROM	StockInfo SI INNER JOIN StockPrice SP ON SI.ID = SP.ID AND SP.BranchNo = @stocklocn
						INNER JOIN StockQuantity SQ ON  SI.ID = SQ.ID AND SQ.StockLocn = @stocklocn
	WHERE  (si.prodstatus in ('D','R')	or SQ.Deleted = 'Y')
			and isnull(OnlineAvailable,0)=1
	
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

-- End End End End End End End End End End End End End End End End End End End End End End End End End End End End End End End End End End