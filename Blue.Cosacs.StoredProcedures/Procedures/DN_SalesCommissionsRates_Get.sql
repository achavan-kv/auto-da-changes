SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS OFF
GO

IF EXISTS (SELECT * FROM SysObjects
           WHERE Name = 'DN_SalesCommissionsRates_Get' AND Type = 'P')
BEGIN
    DROP PROCEDURE DN_SalesCommissionsRates_Get
END
GO


CREATE PROCEDURE DN_SalesCommissionsRates_Get

--------------------------------------------------------------------------------
--
-- Project      : CoSACS .NET
-- File Name    : DN_SalesCommissionsRates_Get.prc
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Get Sales Commission Rates
-- Author       : John Croft
-- Date         : 7 September 2006
--
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 22/06/07  jec CR36 Enhancements
-- 21/07/09  jec CR1035 Enhancements
-- 07/07/11  jec CR1254 RI Changes
-- 09/05/12  jec #9785 CR9439 - all option on sales commission
--------------------------------------------------------------------------------
    -- Parameters
    @piCommissionItem   varchar(20),
    @piSelectDate       Datetime,
    @return             INTEGER OUTPUT



AS  --DECLARE
    -- Local variables
Declare @CommissionType char(2)

BEGIN
    SET @return = 0
    SET NOCOUNT ON

    set @CommissionType = case 
            when @piCommissionItem='Product Category' then 'PC'
            when @piCommissionItem='Product' then 'P '
            when @piCommissionItem='Product Class' then 'PL'
            when @piCommissionItem='Product SubClass' then 'PS'		-- RI
            when @piCommissionItem='Product SKU' then 'SK'			-- RI
            when @piCommissionItem='Single Spiffs' or @piCommissionItem='Linked Spiffs' then 'SP'
			when @piCommissionItem='Terms Type Spiffs' then 'TS'
            when @piCommissionItem='Terms Type' then 'TT'
            else 'XX'			-- #9785 All Product Levels
            
            end
-- get spiffs
if @CommissionType = 'SP' 
	or @CommissionType = 'TS'	--Terms Type Spiff
    Begin
        if @piCommissionItem !='Linked Spiffs'
        -- Single Spiffs
        Begin
            Select ItemText,CAST(Percentage as decimal(12,3)) as Percentage,
				CAST(PercentageCash as decimal(12,3)) as PercentageCash,
				CAST(Value as decimal(12,3)) as Value,
				CAST(RepoPercentage as decimal(12,3)) as RepoPercentage,		-- RI jec 08/07/11
				CAST(RepoPercentageCash as decimal(12,3)) as RepoPercentageCash,
				CAST(RepoValue as decimal(12,3)) as RepoValue,
				DateFrom,DateTo,ComBranchNo as Branch		-- jec 21/07/09
            From SalesCommissionRates

            Where CommissionType=@CommissionType
                and @piSelectDate between DateFrom and DateTo
    
            Order by ItemText,DateTo
        End
        Else		
        -- Linked Spiffs
        Begin
            Select Item1,Item2,Item3,Item4,Item5,CAST(Percentage as decimal(12,3)) as Percentage,
					CAST(Value as decimal(12,3)) as Value,DateFrom,DateTo,Description,ComBranchNo as Branch		-- jec 21/07/09
            From SalesCommissionMultiSpiffRates

            Where @piSelectDate between DateFrom and DateTo
    
            Order by Item1,DateTo
        End

    End
Else
-- get commissions except Terms Type
    if @CommissionType != 'TT'
        Begin
            Select ItemText,CAST(Percentage as decimal(12,3)) as Percentage,
					CAST(PercentageCash as decimal(12,3)) as PercentageCash,
					CAST(RepoPercentage as decimal(12,3)) as RepoPercentage,CAST(RepoPercentageCash as decimal(12,3)) as RepoPercentageCash,	-- RI 
					DateFrom,DateTo,
					case					-- #9785
						when CommissionType='PC' then 'Product Category'
						when CommissionType='P ' then 'Product'
						when CommissionType='PL' then 'Product Class'
						when CommissionType='PS' then 'Product SubClass'		
						when CommissionType='SK' then 'Product SKU'	
					end as CommissionType,	
					ComBranchNo as Branch		-- jec 21/07/09
            From SalesCommissionRates

            Where (CommissionType=@CommissionType
					or (@CommissionType='XX' and CommissionType!='TT'))		-- #9785 All Product Levels
                and @piSelectDate between DateFrom and DateTo
    
            Order by case					-- #9785
						when CommissionType='PC' then 'Product Category'
						when CommissionType='P ' then 'Product'
						when CommissionType='PL' then 'Product Class'
						when CommissionType='PS' then 'Product SubClass'		
						when CommissionType='SK' then 'Product SKU'	
					end,
					ItemText,DateTo
        End
    Else
    -- get Terms Type commissions
        Begin
        Select ItemText,CAST(Percentage as decimal(12,3)) as Percentage,
				CAST(PercentageCash as decimal(12,3)) as PercentageCash,
				CAST(Value as decimal(12,3)) as Value,
				CAST(RepoPercentage as decimal(12,3)) as RepoPercentage,CAST(RepoPercentageCash as decimal(12,3)) as RepoPercentageCash,	-- RI
				CAST(RepoValue as decimal(12,3)) as RepoValue,			-- RI
				DateFrom,DateTo,		
				ComBranchNo as Branch		-- jec 21/07/09
        From SalesCommissionRates

        Where CommissionType=@CommissionType
            and @piSelectDate between DateFrom and DateTo
    
        Order by ItemText,DateTo
    End

    SET NOCOUNT OFF

    SET @return = @@ERROR
    RETURN @return
END


GO
GRANT EXECUTE ON DN_SalesCommissionsRates_Get TO PUBLIC
GO
SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO
-- end end end end end end end end end end end end end end end end end end end end end end end 
