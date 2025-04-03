SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS OFF
GO

IF EXISTS (SELECT * FROM SysObjects
           WHERE Name = 'DN_SalesCommissionsRates_Check' AND Type = 'P')
BEGIN
    DROP PROCEDURE DN_SalesCommissionsRates_Check
END
GO


CREATE PROCEDURE DN_SalesCommissionsRates_Check

--------------------------------------------------------------------------------
--
-- Project      : CoSACS .NET
-- File Name    : DN_SalesCommissionsRates_Get.prc
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Check Sales Commission Rates
-- Author       : John Croft
-- Date         : 27 September 2006
--
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 25/06/07  jec CR36 Enhancements
-- 21/07/09  jec CR1035 Enhancements - add Commission Branchno
-- 07/07/11  jec CR1254 RI Changes
-- 09/05/12  jec #9785 CR9439 - all option on sales commission
--------------------------------------------------------------------------------
    -- Parameters
    @piCommissionItem   varchar(20),
    @piItemText         varchar(18),
    --@piPercentage       float,
    --@piValue            int,
    @piDateFrom         Datetime,
    @piDateTo           Datetime,
    @poValid            char(1) output,
        -- @poValid return values; N - item does not exist
        --                         F - item already exists with FROM date in range
        --                         T - item already exists with TO date in range 
    @piBranchNo			char(3),		-- 21/07/09       
    @return             INTEGER OUTPUT



AS  --DECLARE
    -- Local variables
Declare @CommissionType char(2)

BEGIN
    SET @return = 0
    SET @poValid = ' '
    SET NOCOUNT ON

    set @CommissionType = case 
            when @piCommissionItem='Product Category' then 'PC'
            when @piCommissionItem='Product' then 'P '
            when @piCommissionItem='Product Class' then 'PL'
            when @piCommissionItem='Product SubClass' then 'PS'		-- RI
            when @piCommissionItem='Product SKU' then 'SK'			-- RI
            when @piCommissionItem='Single Spiffs' then 'SP'
            when @piCommissionItem='Linked Spiffs' then 'LS'
			when @piCommissionItem='Terms Type Spiffs' then 'TS'	-- jec 25/06/07
            when @piCommissionItem='Terms Type' then 'TT'
            
            end

If @CommissionType != 'LS'
Begin
-- Validate Commission item 
set @poValid = case 
-- Does a record already exist for this item?
when   not exists (select * from SalesCommissionRates
        Where CommissionType=@CommissionType
        and ComBranchNo=@piBranchNo			-- 21/07/09
        and ItemText= @piItemText)
        and @CommissionType in ('P ', 'SP', 'TS', 'TT', 'SK','PL','PS','PC')    -- RI

        then 'N'       
-- Does a record already exist with a start or end date within this range?
when  exists (select * from SalesCommissionRates
        Where CommissionType=@CommissionType
        and ItemText= @piItemText
        and ComBranchNo=@piBranchNo			-- 21/07/09
        and (@piDateFrom between DateFrom and DateTo)
        and DateTo != (select max(dateTo) from SalesCommissionRates
                        Where CommissionType=@CommissionType
                        and ItemText= @piItemText))
        then 'F'            

when  exists (select * from SalesCommissionRates
        Where CommissionType=@CommissionType
        and ItemText= @piItemText
        and ComBranchNo=@piBranchNo			-- 21/07/09
        and (@piDateTo between DateFrom and DateTo)
        and DateTo != (select max(dateTo) from SalesCommissionRates
                        Where CommissionType=@CommissionType
                        and ItemText= @piItemText))            
        then 'T'

when  exists (select * from SalesCommissionRates
        Where CommissionType=@CommissionType
        and ItemText= @piItemText
        and ComBranchNo=@piBranchNo			-- 21/07/09
        and (DateFrom between @piDateFrom and @piDateTo)
        and DateFrom != @piDateFrom)		-- #9785		
		--and DateTo != (select max(dateTo) from SalesCommissionRates
        --                Where CommissionType=@CommissionType
        --                and ItemText= @piItemText)) 
                
        then 'T'

        else 'Y'    -- valid 
end

End

Else -- Linked Spiff 'LP'
Begin
-- Validate Commission item 
set @poValid = case 
-- Does a record already exist for this item?
when   not exists (select * from SalesCommissionMultiSpiffRates
        Where Item1= @piItemText
        and ComBranchNo=@piBranchNo)		-- 21/07/09)
        
        then 'N'       
-- Does a record already exist with a start or end date within this range?
when  exists (select * from SalesCommissionMultiSpiffRates
        Where Item1= @piItemText
        and ComBranchNo=@piBranchNo			-- 21/07/09
        and (@piDateFrom between DateFrom and DateTo)
        and DateTo != (select max(dateTo) from SalesCommissionMultiSpiffRates
                        Where Item1 = @piItemText))
        then 'F'            

when  exists (select * from SalesCommissionMultiSpiffRates
        Where Item1= @piItemText
        and ComBranchNo=@piBranchNo			-- 21/07/09
        and (@piDateTo between DateFrom and DateTo)
        and DateTo != (select max(dateTo) from SalesCommissionMultiSpiffRates
                        Where Item1= @piItemText))            
        then 'T'

        else 'Y'    -- valid 
end

End

-- if item does not exist in commission rates tables - check if item is valid

If @poValid='N'

set @poValid=case 
    -- does category exist?
    when @CommissionType= 'PC'  
            and exists (select code from code 
                where category like 'PC%' and code = @piItemText) then 'Y'
    
    -- does Product Class exist?
    when @CommissionType= 'PL'				-- RI
            and exists (select distinct Class from StockInfo 
                where ISNULL(Class,'') =@piItemText) then 'Y'
                
    -- does Product SubClass exist?
    when @CommissionType= 'PS'				-- RI
            and exists (select distinct SubClass from StockInfo 
                where ISNULL(SubClass,'') =@piItemText) then 'Y'

    -- is Product "Non Stock" i.e warranty?
    when (@CommissionType= 'SP' or @CommissionType= 'LS')
            and exists (select distinct itemno from stockitem 
                where itemno = @piItemText and itemtype!='S') then 'W'

    -- does Product exist?
    when (@CommissionType= 'P ' or  @CommissionType= 'SP' or @CommissionType= 'LS')
    --        and exists (select distinct itemno from stockitem 
    --            --where itemno = @piItemText and itemtype='S') then 'Y' -- itemtype='S' removed jec 08/01/07
				--where itemno = @piItemText) then 'Y'
				and exists (select distinct IUPC from StockInfo 
				where IUPC = @piItemText) then 'Y'
				
	-- does SKU exist?		-- RI
    when (@CommissionType= 'SK')
            and exists (select distinct SKU from stockinfo                
				where ISNULL(SKU,'') = @piItemText) then 'Y'

    -- does Terms Type exist?
    when @CommissionType in('TT', 'TS')		-- CR36 Enhancements  jec 25/06/07  
            and exists (select distinct termstype from termstype 
                where termstype = @piItemText) then 'Y'
    else
    -- item does not exist
        'X'        
end

    SET NOCOUNT OFF

    SET @return = @@ERROR
    RETURN @return
END


GO
GRANT EXECUTE ON DN_SalesCommissionsRates_Check TO PUBLIC
GO
SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO
-- end end end end end end end end end end end end end end end end end end end end end end end 
