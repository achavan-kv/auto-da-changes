SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO
SET ANSI_WARNINGS ON
go

if exists (SELECT * FROM dbo.sysobjects WHERE id = object_id('[dbo].[RIKitProductDataLoadSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[RIKitProductDataLoadSP]
GO

CREATE PROCEDURE RIKitProductDataLoadSP
-- ================================================
-- Project      : CoSACS .NET
-- File Name    : RIKitProductDataLoadSP.prc
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : RI Interface - Kit Product Data Load
-- Date         : 15 March 2010
--
-- This procedure will load the Kit products from the RI interface file.
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 20/04/11 jec No longer required
-- ================================================
	-- Add the parameters for the stored procedure here
        @return int OUTPUT
AS
--BEGIN
--    SET 	@return = 0			--initialise return code
--    DECLARE @statement SQLText
--    DECLARE @BCPpath VARCHAR(500)
--    DECLARE @path varchar(100), @fileName varchar(40), @bcpCommand  varchar(400)
	
--	set @filename='KIT.csv'		-- temp for testing
--	SELECT @BCPpath = value + '\Bcp' FROM CountryMaintenance WHERE Codename = 'BCPpath' 
         
--	IF EXISTS (SELECT Table_Name FROM INFORMATION_SCHEMA.tables
--			   WHERE  Table_Name = 'RItemp_RawKitload')
--	BEGIN
--		Truncate TABLE RItemp_RawKitload
--	END	
	
--	set @bcpCommand = '"' + @BCPpath + '" ' + +db_name()+'..RItemp_RawKitload' + ' in ' +
--				'd:\users\default\' + @filename + ' -c -t, -q -Usa -P'
				
--	select  @bcpCommand
--	EXEC master.dbo.xp_cmdshell @bcpCommand
	
--	--select * from RItemp_RawKitload

--	--EXEC sp_executesql @statement

--	IF EXISTS (SELECT Table_Name FROM INFORMATION_SCHEMA.tables
--			   WHERE  Table_Name = 'RItemp_Kitload')
--	BEGIN
--		Truncate TABLE RItemp_Kitload
--	END	
	
--	Insert into RItemp_Kitload (KitItemIUPC, ComponentIUPC, ComponentQty, ComponentPrice, DeletedFlag, ItExists, CpExists, RowProcessed)
--	SELECT DISTINCT KitItemIUPC, ComponentIUPC,ComponentQty,ComponentPrice,DeletedFlag,convert(smallint,0) as itexists,
--				    convert(smallint,0),convert(smallint,0)
--    FROM  	RItemp_RawKitload
    
--END
--GO
--SET QUOTED_IDENTIFIER OFF 
--GO
--SET ANSI_NULLS ON 
--GO
--SET ANSI_WARNINGS Off
GO

-- End End End End End End End End End End End End End End End End End End End End End End End End End End End
