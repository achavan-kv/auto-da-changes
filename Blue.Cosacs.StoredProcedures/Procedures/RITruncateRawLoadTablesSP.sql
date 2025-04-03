SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO
SET ANSI_WARNINGS ON
go

if exists (SELECT * FROM dbo.sysobjects WHERE id = object_id('[dbo].[RITruncateRawLoadTablesSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[RITruncateRawLoadTablesSP]
GO

CREATE PROCEDURE RITruncateRawLoadTablesSP
-- ================================================
-- Project      : CoSACS .NET
-- File Name    : RITruncateRawTablesSP.prc
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : RI Interface - Truncate Raw Load Tables
-- Date         : 16 March 2011
--
-- This procedure will initialise the Raw load tables prior to importing the RI interface data files.
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 
-- ================================================
	-- Add the parameters for the stored procedure here		
        @return int OUTPUT
AS
	set @return=0
	
	Truncate TABLE RItemp_RawKitload
	Truncate TABLE RItemp_RawPOload
	Truncate TABLE RItemp_RawProductload
	Truncate TABLE RItemp_RawProductloadRepo
	Truncate TABLE RItemp_RawStkQtyload
	Truncate TABLE RItemp_RawStkQtyloadRepo
	Truncate TABLE RItemp_RawProductImport
	Truncate TABLE RItemp_RawProductImportRepo
	Truncate TABLE RItemp_RawProductImportError
	Truncate TABLE RItemp_RawProductHeirarchy

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO
SET ANSI_WARNINGS Off
GO

-- End End End End End End End End End End End End End End End End End End End End End End End End End End End
