SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO
SET ANSI_WARNINGS ON
go

if exists (SELECT * FROM dbo.sysobjects WHERE id = object_id('[dbo].[RIProductHeirarchyLoadSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[RIProductHeirarchyLoadSP]
GO

CREATE PROCEDURE RIProductHeirarchyLoadSP
-- ================================================
-- Project      : CoSACS .NET
-- File Name    : RIProductHeirarchyLoadSP.prc
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : RI Interface - Product Heirachy Data Load
-- Date         : 15 June 2011
--
-- This procedure will load the Product Heirachy detail from the RI interface file.
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 27/07/11  jec CR1254 Truncate table if not imported from same source - Not required
-- 28/07/11  jec CR1254 PK changed - truncate not required
-- ================================================
	-- Add the parameters for the stored procedure here
		@interface varchar(10),
		@runno int,
		@rerun BIT,		
		@repo BIT,
		@return int OUTPUT
AS
BEGIN
    SET 	@return = 0			--initialise return code
    
    SELECT TOP 1 * 
    INTO #RawProductHeirarchyload
    FROM RItemp_RawProductHeirarchy
    
    TRUNCATE TABLE #RawProductHeirarchyload
    
    IF @repo = 0
		INSERT INTO #RawProductHeirarchyload 
		SELECT * FROM RItemp_RawProductHeirarchy
	ELSE
		INSERT INTO #RawProductHeirarchyload 
		SELECT * FROM RItemp_RawStkQtyloadRepo
		
	-- clear table if Source differs - avoid duplicates
	--if not exists(select top 1 * from ProductHeirarchy where RISource=(select distinct RISource from #RawProductHeirarchyload))	
	--	truncate TABLE  ProductHeirarchy
    
    -- Update existing deatils 
	UPDATE ProductHeirarchy
	SET CodeDescription	= l.CodeDescription,CodeStatus=l.CodeStatus	
	FROM ProductHeirarchy h 	
	INNER JOIN  #RawProductHeirarchyload l ON h.CatalogType=l.CatalogType and h.PrimaryCode=l.PrimaryCode and h.ParentCode=l.ParentCode
	WHERE h.CodeDescription != l.CodeDescription or h.CodeStatus!=l.CodeStatus
			
	
	-- Insert new details 
	INSERT INTO ProductHeirarchy (RISource, CatalogType, PrimaryCode, CodeDescription, ParentCode, CodeStatus)
	SELECT l.RISource, l.CatalogType, l.PrimaryCode, l.CodeDescription, l.ParentCode, l.CodeStatus
	FROM  #RawProductHeirarchyload l 
	WHERE NOT EXISTS (SELECT * FROM ProductHeirarchy h 
					  WHERE h.CatalogType=l.CatalogType and h.PrimaryCode=l.PrimaryCode and h.ParentCode=l.ParentCode)	
						
	      
END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO
SET ANSI_WARNINGS Off
GO

-- End End End End End End End End End End End End End End End End End End End End End End End End End End End
