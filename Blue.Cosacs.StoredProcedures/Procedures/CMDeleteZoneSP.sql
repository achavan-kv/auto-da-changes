SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
if exists (select * from sysobjects where name ='CMDeleteZoneSP')
drop procedure CMDeleteZoneSP
go

CREATE PROCEDURE [dbo].[CMDeleteZoneSP] @return INTEGER OUT, @Zone VARCHAR (4) 

-- ================================================
-- Project      : CoSACS .NET
-- File Name    : CMDeleteZoneSP.prc
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Delete Zone
-- Author       : ??
-- Date         : ?? 2007
--
-- This procedure will delete zones
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 
-- ================================================
	-- Add the parameters for the stored procedure here
AS  
	SET @return = 0 
	DELETE FROM CMZone WHERE zone = @Zone
	SET @return = @@ERROR
	IF @return = 0 
	BEGIN
		DELETE FROM  CmZoneAllocation WHERE zone = @Zone
		SET @return = @@ERROR
	END
	

GO

-- End End End End End End End End End End End End End End End End End End End End End End End End End End End End End 

