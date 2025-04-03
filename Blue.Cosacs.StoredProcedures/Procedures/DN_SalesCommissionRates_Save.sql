SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS OFF
GO

IF EXISTS (SELECT * FROM SysObjects
           WHERE Name = 'DN_SalesCommissionRates_Save' AND Type = 'P')
BEGIN
    DROP PROCEDURE DN_SalesCommissionRates_Save
END
GO


CREATE PROCEDURE DN_SalesCommissionRates_Save

--------------------------------------------------------------------------------
--
-- Project      : CoSACS .NET
-- File Name    : DN_SalesCommissionRates_Save.prc
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Save Sales Commission Rates
-- Author       : John Croft
-- Date         : 14 September 2006
--
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 25/06/07  jec CR36 Enhancements
-- 20/07/09  jec CR1035 Enhancements - add Commission Branchno
-- 17/11/09  jec UAT661 Correct update to SalesCommissionRatesAudit
-- 07/07/11  jec CR1254 RI Changes
--------------------------------------------------------------------------------
    -- Parameters
    @piCommissionItem   varchar(20),
    @piItemText         varchar(18),		-- RI
    @piPercentage       float,
	@piPercentageCash   float,
    @piValue            money,
    @piDateFrom         Datetime,
    @piDateTo           Datetime,
    @piEmpeeNo          int,
    @piBranchNo			char(3),		-- 20/07/09
    @piRepoPercentage       float,			-- RI
	@piRepoPercentageCash   float,
    @piRepoValue            money,
    @return             INTEGER OUTPUT

AS  

    SET @return = 0
    SET NOCOUNT ON

    --DECLARE
    -- Local variables
Declare @CommissionType     char(2),
        @MaxDate            datetime,
        @CurrDate           datetime     

 set @currDate=Getdate()
    
 set @CommissionType = case 
            when @piCommissionItem='Product Category' then 'PC'
            when @piCommissionItem='Product' then 'P '
            when @piCommissionItem='Product Class' then 'PL'
            when @piCommissionItem='Product SubClass' then 'PS'
            when @piCommissionItem='Product SKU' then 'SK'
            when @piCommissionItem='Single Spiffs' or @piCommissionItem='Linked Spiffs' then 'SP'
			when @piCommissionItem='Terms Type Spiffs' then 'TS'
            when @piCommissionItem='Terms Type' then 'TT'
            
            end

-- !!! need to check if rate is to be deleted - dateto = 1999-01-01
if @piDateTo = '1999-01-01'
    Begin
        -- insert DELETE audit record
        insert into SalesCommissionRatesAudit 
        (ItemText,CommissionType,Percentage,PercentageCash,Value,DateFrom,
			DateTo,EmpeenoChanged,DateChanged,ActionType,ComBranchNo,
			ItemId,RepoPercentage,RepoPercentageCash,RepoValue,RepoItemId)		-- RI
        select ItemText,CommissionType,Percentage,PercentageCash,Value,DateFrom,
			DateTo,EmpeenoChanged,@currDate,'D',ComBranchNo,
			ItemId,RepoPercentage,RepoPercentageCash,RepoValue,RepoItemId		-- RI
        from SalesCommissionRates
                Where CommissionType=@CommissionType
            and ItemText= @piItemText
            and DateFrom= @piDateFrom
            and ComBranchNo=@piBranchNo			-- 20/07/09
        -- Update empeeno to current employee 
        update SalesCommissionRatesAudit
            set EmpeenoChanged=@piEmpeeNo
                Where CommissionType=@CommissionType
                    and ItemText= @piItemText
                    and DateFrom= @piDateFrom
                    and DateChanged=@CurrDate
                    and ComBranchNo=@piBranchNo			-- 20/07/09
        -- now delete record
        delete SalesCommissionRates
            Where CommissionType=@CommissionType
            and ItemText= @piItemText
            and DateFrom= @piDateFrom
            and ComBranchNo=@piBranchNo			-- 20/07/09

    goto End_Procedure
    End

-- moved to here - jec 21/07/09
Select @MaxDate=max(DateFrom)
        From SalesCommissionRates

        Where CommissionType=@CommissionType
        and ItemText= @piItemText
		and ComBranchNo=@piBranchNo
		
-- If record exists - no changed has occurred?
if  not exists (select * from SalesCommissionRates
        Where CommissionType=@CommissionType
        and ItemText= @piItemText
        and Percentage=@piPercentage
		and PercentageCash=@piPercentageCash		-- CR36 enhancement jec 22/06/07
		and Value=@piValue
        and DateFrom= @piDateFrom
        and DateTo=@piDateTo
        and ComBranchNo=@piBranchNo
        and RepoPercentage=@piRepoPercentage					-- RI
		and RepoPercentageCash=@piRepoPercentageCash		
		and RepoValue=@piRepoValue
        )		
    BEGIN
    -- here if record does not exist - therefore changed in .Net

-- New code	31/10/06
if  exists (select * from SalesCommissionRates
        Where CommissionType=@CommissionType
        and ItemText= @piItemText
        --and Percentage=@piPercentage
        and DateFrom= @piDateFrom
        and ComBranchNo=@piBranchNo)		-- 20/07/09
      --  and DateTo=@piDateTo)
	Begin
-- update existing row 
		update SalesCommissionRates
        set DateTo=@piDateTo,Percentage=@piPercentage,PercentageCash=@piPercentageCash,Value=@piValue,
							RepoPercentage=@piRepoPercentage,RepoPercentageCash=@piRepoPercentageCash,RepoValue=@piRepoValue		-- RI

         Where CommissionType=@CommissionType
            and ItemText= @piItemText
            and DateFrom=@piDateFrom
            and ComBranchNo=@piBranchNo		-- 20/07/09

-- insert CHANGE audit record

    insert into SalesCommissionRatesAudit (ItemText, CommissionType, Percentage, PercentageCash, Value, DateFrom, DateTo, 
					EmpeenoChanged, DateChanged, ActionType, ComBranchNo, 
					ItemId,RepoPercentage,RepoPercentageCash,RepoValue,RepoItemId)		-- RI 
            select ItemText,CommissionType,Percentage,PercentageCash,Value,DateFrom,DateTo,EmpeenoChanged, -- jec 21/07/09
					@CurrDate,'C',ComBranchNo,
					ItemId,RepoPercentage,RepoPercentageCash,RepoValue,RepoItemId			-- RI
            from SalesCommissionRates
                Where CommissionType=@CommissionType
                    and ItemText= @piItemText
                    and DateFrom=@MaxDate
                    and ComBranchNo=@piBranchNo		-- 20/07/09
                    
        update SalesCommissionRatesAudit
            set EmpeenoChanged=@piEmpeeNo
                Where CommissionType=@CommissionType
                    and ItemText= @piItemText
                    and DateFrom= @MaxDate
                    and DateChanged=@CurrDate
                    and ComBranchNo=@piBranchNo		-- 20/07/09

	goto End_Procedure

	End

-- end New Code	31/10/06

-- moved to above        
    --Select @MaxDate=max(DateFrom)
    --    From SalesCommissionRates
    --    Where CommissionType=@CommissionType
    --    and ItemText= @piItemText
        
    If exists (select * from SalesCommissionRates
                Where CommissionType=@CommissionType
                    and ItemText= @piItemText
                    and DateFrom=@MaxDate
                    and ComBranchNo=@piBranchNo		-- 20/07/09
                    and @MaxDate<@piDateFrom)
    Begin
    -- Update existing commission end date
    update SalesCommissionRates
        set DateTo=Dateadd(mi,-1,@piDateFrom)

         Where CommissionType=@CommissionType
            and ItemText= @piItemText
            and DateFrom=@MaxDate
            and ComBranchNo=@piBranchNo		-- 20/07/09
            
     -- insert CHANGE audit record

    insert into SalesCommissionRatesAudit (ItemText, CommissionType, Percentage, PercentageCash, Value, DateFrom, DateTo,EmpeenoChanged, 
					DateChanged, ActionType, ComBranchNo, 
					ItemId,RepoPercentage,RepoPercentageCash,RepoValue,RepoItemId)		-- RI
            select ItemText,CommissionType,Percentage,PercentageCash,Value,DateFrom,DateTo,EmpeenoChanged, -- jec 21/07/09
					@CurrDate,'C',ComBranchNo,
					ItemId,RepoPercentage,RepoPercentageCash,RepoValue,RepoItemId		-- RI
            from SalesCommissionRates
                Where CommissionType=@CommissionType
                    and ItemText= @piItemText
                    and DateFrom=@MaxDate
                    and ComBranchNo=@piBranchNo		-- 20/07/09
        update SalesCommissionRatesAudit
            set EmpeenoChanged=@piEmpeeNo
                Where CommissionType=@CommissionType
                    and ItemText= @piItemText
                    and DateFrom= @MaxDate
                    and DateChanged=@CurrDate
                    and ComBranchNo=@piBranchNo		-- 20/07/09
    End
        -- Insert New rate
     insert into SalesCommissionRates 
            (ItemText,CommissionType,Percentage,PercentageCash,Value,DateFrom,
				DateTo,EmpeenoChanged,ComBranchNo,
				ItemId,RepoPercentage,RepoPercentageCash,RepoValue,RepoItemId)		-- RI
     select @piItemText,@CommissionType,@piPercentage,@piPercentageCash,@piValue,@piDateFrom,
				@piDateTo,@piEmpeeno,@piBranchNo, 		-- 20/07/09				
				0,		-- RI
				@piRepoPercentage,@piRepoPercentageCash,@piRepoValue,
				0		-- RI repoitemid
	 	 
	 -- Update rates without itemId   (Rate can be added before item exists) 
    UPDATE SalesCommissionRates
		set itemId=ISNULL(s.Id,0)
	from SalesCommissionRates c, StockInfo s 
	Where (c.itemId=0)
		and c.CommissionType in('P','SP')
		and ((((c.CommissionType='P' and ISNULL(s.IUPC,'')=c.ItemText) or (c.CommissionType='SP' and ISNULL(s.IUPC,'')=c.ItemText)) and s.RepossessedItem=0))	-- Regular item Id
	
	UPDATE SalesCommissionRates
		set repoItemId=ISNULL(s.Id,0)
	from SalesCommissionRates c, StockInfo s 
	Where (c.repoItemId=0)
		and c.CommissionType in('P','SP')
		and ((((c.CommissionType='P' and ISNULL(s.IUPC,'')=c.ItemText) or (c.CommissionType='SP' and ISNULL(s.IUPC,'')=c.ItemText)) and s.RepossessedItem=1))	-- Repossessed item Id
	
			
        -- insert ADD audit record
 
    insert into SalesCommissionRatesAudit (ItemText, CommissionType, Percentage, PercentageCash, Value, DateFrom, DateTo, EmpeenoChanged, 
					DateChanged, ActionType, ComBranchNo, 
					ItemId,RepoPercentage,RepoPercentageCash,RepoValue,RepoItemId)		-- RI
            select ItemText,CommissionType,Percentage,PercentageCash,Value,DateFrom,DateTo,EmpeenoChanged, -- jec 21/07/09
					@CurrDate,'A',ComBranchNo,
					ItemId,RepoPercentage,RepoPercentageCash,RepoValue,RepoItemId		-- RI  
            from SalesCommissionRates
                Where CommissionType=@CommissionType
                    and ItemText= @piItemText
                    and DateFrom=@piDateFrom
                    and ComBranchNo=@piBranchNo		-- 20/07/09
    
    END

End_Procedure:

    SET NOCOUNT OFF

    SET @return = @@ERROR
    RETURN @return

GO
GRANT EXECUTE ON DN_SalesCommissionRates_Save TO PUBLIC
GO
SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO
-- end end end end end end end end end end end end end end end end end end end end end end end 
