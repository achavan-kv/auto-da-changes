SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS OFF
GO

IF EXISTS (SELECT * FROM SysObjects
           WHERE Name = 'DN_EODConfigurationOptionsGetDetailsSP' AND Type = 'P')
BEGIN
    DROP PROCEDURE DN_EODConfigurationOptionsGetDetailsSP
END
GO


CREATE PROCEDURE DN_EODConfigurationOptionsGetDetailsSP

--------------------------------------------------------------------------------
--
-- Project      : CoSACS .NET
-- File Name    : DN_EODConfigurationOptionsGetDetailsSP.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Load the list of options for an EOD configuration
-- Author       : D Richardson
-- Date         : 1 Mar 2006
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 27/11/09 jec UAT683 Progress bar not working
-- 01/12/09 ip  UAT675 Status for running 'Ad Hoc' scripts incorrect.
-- 31/08/10 ip  CR1092 COASTER to CoSACS Enhancements - If EODResult = Warning then return the status as PASS(W) to indicate warnings.
-- 05/04/11 jec Return ReRunNo
-- 22/06/11  IP  CR1212 - RI - #3987 - If the interface method set in Country Parameters
--				 is 'FACT' then we do not want to return the option for RI Export.
--				 If 'RI' then we do not want to display the FACT 2000 Export option if these are in the configuration
-- 09/11/11 jec #8606 Cannot run any EOD interface. Also correct AVG calc
-- 13/06/13  ip #13820 Delete option previously automatically added.
--------------------------------------------------------------------------------

    -- Parameters
    @configurationName	VARCHAR(12),
	@startdate			DATETIME OUT,
	@frequency			INT OUT,
    @return             INT OUTPUT

AS  --DECLARE
    -- Local variables

BEGIN

    SET @return = 0
    
    declare @interfaceMethod varchar(5)				--IP - 22/06/11 - CR1212 - RI - #3987
	declare @code varchar(12)						--IP - 22/06/11 - CR1212 - RI - #3987
	
	set @code = ''
	select @interfaceMethod = value from countrymaintenance where codename = 'RIInterfaceOptions'
	
	if(@interfaceMethod = 'FACT')
	begin
		set @code = 'CoSACS2RI'
	end
	else if(@interfaceMethod = 'RI')
	begin
		set @code = 'COS FACT'
	end
	
	--#13820
	delete EODConfigurationOption
	where ConfigurationName = @configurationName
	and CanReRun = 'D'

    -- use declared table instead of temp table
    declare @options TABLE (OptionCode VARCHAR(12),SortOrder SMALLINT,CodeDescript VARCHAR(64),
			AvgRunTime BIGINT,status VARCHAR(7), ReRunNo int )
		
	declare @avg TABLE (Interface VARCHAR(12),AvgRunTime BIGINT)			-- #8606 
			
	insert into @options		
    SELECT	e.OptionCode, e.SortOrder, 
			c.CodeDescript, CONVERT(bigint,0) AS AvgRunTime, 
			CASE ISNULL(status,'') WHEN 'P' THEN 'PASS' WHEN 'F' THEN 'FAIL' WHEN 'W' THEN 'PASS(W)' ELSE '' END AS status, --IP - 31/08/10 - CR1092 - Changed 'PASS' to 'PASS(W)'
			0 
	--INTO	#options		
    FROM	EODConfigurationOption e, code c
    WHERE	ConfigurationName = @configurationName
	AND		c.category = 'EDC'
    AND		e.OptionCode = c.code
    AND     (e.OptionCode!= @code or @code = '')			--IP - 22/06/11 - CR1212 - RI - #3987
    ORDER BY e.SortOrder
    
    -- #8606 Group avg by Interface option
    insert into @avg 
    select interface,ISNULL(AVG(DATEDIFF( s, datestart, datefinish)),600) 
    FROM	interfacecontrol ic,@options o
					WHERE	o.OptionCode = ic.interface
					AND		ic.result = 'P'
					and		ic.datefinish!='1900-01-01'			-- #8606 date diff too large
					AND		DATEADD(WEEK, 3, ic.datestart) > GETDATE()
					group by ic.interface
	-- #8606 Update Avg for each option
    UPDATE	@options
    Set AvgRunTime =a.AvgRunTime
    from @options o INNER JOIN @Avg a on o.OptionCode=a.Interface
    
    --UPDATE	@options
    --SET		AvgRunTime = (
				--	-- UAT683 get average in seconds otherwise average is 0 when hours or minutes used
				--	SELECT	ISNULL(AVG(DATEDIFF( s, ic.datestart, ic.datefinish)),600)
				--	FROM	interfacecontrol ic,@options o
				--	WHERE	o.OptionCode = ic.interface
				--	AND		ic.result = 'P'
				--	AND		DATEADD(WEEK, 3, ic.datestart) > GETDATE())
					
	-- UAT683 - not required average returned as seconds
	--To convert the value return by datediff to seconds from hour //NM 30/01/2009
	-- only for positive values 
	--UPDATE  #options
	--set		AvgRunTime = AvgRunTime * 60 * 60
	--Where	AvgRunTime > 0

	-- cater for negative value - should never happen but could if datestart/datefinish are manually set incorrectly (jec 29/01/07)
	UPDATE	@options
	set		AvgRunTime=600
	Where	AvgRunTime<0
 	
 	--IP - 01/12/09 - UAT5.2 (675) - Should not update the status to blank, should display 'PASS' or 'FAIL' on End Of Day screen.			
    --UPDATE	@options
    --SET		status = ''
	--WHERE	OptionCode = 'ADHOC'	
	
	SELECT	OptionCode, SortOrder, CodeDescript, AvgRunTime, status,ReRunNo
	FROM	@options
	ORDER BY SortOrder

	SELECT	@startdate = startdate,
			@frequency = frequency
	FROM	EODConfiguration
	WHERE	ConfigurationName = @configurationName
    
    SET @return = @@ERROR
    RETURN @return
END

GO
GRANT EXECUTE ON DN_EODConfigurationOptionsGetDetailsSP TO PUBLIC
GO
SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO

-- End End End End End End End End End End End End End End End End End End End End End 

