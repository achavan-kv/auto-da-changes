SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS OFF
GO

IF EXISTS (SELECT * FROM SysObjects
           WHERE Name = 'DN_SalesCommission_Delete' AND Type = 'P')
BEGIN
    DROP PROCEDURE DN_SalesCommission_Delete
END
GO


CREATE PROCEDURE DN_SalesCommission_Delete

--------------------------------------------------------------------------------
--
-- Project      : CoSACS .NET
-- File Name    : DN_SalesCommission_Delete.prc
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Detete Sales Commission Details after set period
-- Author       : John Croft
-- Date         : 18 October 2006
--
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
--
--------------------------------------------------------------------------------
    -- Parameters
    
    @return             INTEGER OUTPUT

AS  

    SET @return = 0
    SET NOCOUNT ON

    --DECLARE
    -- Local variables
Declare @retainCommMonths   int,
        @retainDate         datetime     


    
-- get Country Maintenance parameters
    set @retainCommMonths = (select value from countryMaintenance
                            where CodeName='retainCommMonths')
    if @retainCommMonths<6
        set @retainCommMonths=6    -- safeguard

    set @retainDate=dateadd(m,@retainCommMonths*-1,Getdate())

-- Delete Commission records prior to retain date

    delete SalesCommission
        where RunDate<@retainDate

    SET @return = @@ERROR

    SET NOCOUNT OFF

    RETURN @return

GO
GRANT EXECUTE ON DN_SalesCommission_Delete TO PUBLIC
GO
SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO
-- end end end end end end end end end end end end end end end end end end end end end end end 
