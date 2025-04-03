IF EXISTS (SELECT * FROM SYSOBJECTS WHERE NAME ='WTRDatesGetSP')
DROP PROCEDURE WTRDatesGetSP
GO
CREATE PROCEDURE dbo.WTRDatesGetSP
-- ================================================
-- Project      : CoSACS .NET
-- File Name    : WTRDatesGetSP.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Get Weekly Trading Report dates
-- Date         : 12/08/11
--
-- This procedure will retrieve the dates for the Weekly Trading Report
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 12/08/11  IP  Created
-- ================================================
	-- Add the parameters for the stored procedure here
        @dtStartCY1 datetime OUT,
        @dtEndCY1 datetime OUT,
		@dtStartLY1 datetime OUT,
		@dtEndLY1 datetime OUT,
		@dtActive1 bit OUT,
		@dtFilename1 varchar(30) OUT,
        @return int OUTPUT
AS
   
		SELECT @dtStartCY1 = DtStartCY1,
			   @dtEndCY1 = DtEndCY1,
			   @dtStartLY1 = DtStartLY1,
			   @dtEndLY1 = DtEndLY1,
			   @dtActive1 = DtActive1,
			   @dtFilename1 = isnull(DtFileName1,'')
		FROM WTRDates
		
		select * from WTRDates
		
		
		SET @return = @@error
	

