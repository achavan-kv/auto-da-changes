SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS OFF
GO

IF EXISTS (SELECT * FROM SysObjects
           WHERE Name = 'DN_SalesCommissionsRates_CategoryCheck' AND Type = 'P')
BEGIN
    DROP PROCEDURE DN_SalesCommissionsRates_CategoryCheck
END
GO


CREATE PROCEDURE DN_SalesCommissionsRates_CategoryCheck

--------------------------------------------------------------------------------
--
-- Project      : CoSACS .NET
-- File Name    : DN_SalesCommissionsRates_CategoryCheck.prc
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Check for Product Category Commission Rates
-- Author       : John Croft
-- Date         : 28 September 2006
--
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
--
--------------------------------------------------------------------------------
    -- Parameters
    --@piCommissionItem   varchar(20),
    --@piItemText         varchar(10),
    --@piPercentage       float,
    --@piValue            int,
    --@piDateFrom         Datetime,
    --@piDateTo           Datetime,
    @poCategory         varchar(10) output,
             
    @return             INTEGER OUTPUT



AS  --DECLARE
    -- Local variables
--Declare @CommissionType char(2)

BEGIN
    SET @return = 0
    SET @poCategory = ''
    SET NOCOUNT ON
/*
    set @CommissionType = case 
            when @piCommissionItem='Product Category' then 'PC'
            when @piCommissionItem='Product' then 'P '
            when @piCommissionItem='Product Level' then 'PL'
            when @piCommissionItem='SPIFF' then 'SP'
            when @piCommissionItem='Terms Type' then 'TT'
            
            end
*/
-- Validate Commission item 
set @poCategory = (select distinct top 1 s.category
        from stockitem s,code c
        where c.category like 'PC%'
        and s.category!=0				-- RI
        and cast(s.category as varchar(3))=c.code
        and not exists(select * from SalesCommissionRates 
                    where Itemtext=cast(s.category as varchar(3)))
        and not exists(select itemtext 
                from SalesCommissionRates c1
                    where s.category=c1.itemtext
                        and CommissionType = 'PC'
                        and getdate() between Datefrom and Dateto))
    

    SET NOCOUNT OFF

    SET @return = @@ERROR
    RETURN @return
END


GO
GRANT EXECUTE ON DN_SalesCommissionsRates_CategoryCheck TO PUBLIC
GO
SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO
-- end end end end end end end end end end end end end end end end end end end end end end end 
