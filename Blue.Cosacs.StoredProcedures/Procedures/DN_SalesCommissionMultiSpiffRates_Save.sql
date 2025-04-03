SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS OFF
GO

IF EXISTS (SELECT * FROM SysObjects
           WHERE Name = 'DN_SalesCommissionMultiSpiffRates_Save' AND Type = 'P')
BEGIN
    DROP PROCEDURE DN_SalesCommissionMultiSpiffRates_Save
END
GO


CREATE PROCEDURE DN_SalesCommissionMultiSpiffRates_Save

--------------------------------------------------------------------------------
--
-- Project      : CoSACS .NET
-- File Name    : DN_SalesCommissionMultiSpiffRates_Save.prc
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Save Sales Commission Multi Spiff Rates
-- Author       : John Croft
-- Date         : 4 October 2006
--
--
-- Change Control
-- --------------
-- Date      By  Description
-- 21/07/09  jec CR1035 Enhancements - add Commission Branchno
-- 07/07/11  jec CR1254 RI Changes
-- ----      --  -----------------------------------------------
    -- Parameters
    @piCommissionItem   varchar(20),
    @piDescription      varchar(32),
    @piItem1            varchar(18),
    @piItem2            varchar(18),
    @piItem3            varchar(18),
    @piItem4            varchar(18),
    @piItem5            varchar(18),
    @piPercentage       float,
    @piValue            money,        -- was int
    @piDateFrom         Datetime,
    @piDateTo           Datetime,
    @piEmpeeNo          int,
    @piBranchNo			char(3),		-- 20/07/09
    @return             INTEGER OUTPUT

AS  

    SET @return = 0
    SET NOCOUNT ON

    --DECLARE
    -- Local variables
Declare @CommissionType     char(2),
        @recDate            datetime
-- Commission Type is not used in this procedure it has been retained for consistency across this group of procedures
 set @CommissionType = case 
            when @piCommissionItem='Product Category' then 'PC'
            when @piCommissionItem='Product' then 'P '
            when @piCommissionItem='Product Class' then 'PL'			-- RI
            when @piCommissionItem='Product SubClass' then 'PS'
            when @piCommissionItem='Product SKU' then 'SK'
            when @piCommissionItem='Single Spiffs' or @piCommissionItem='Linked Spiffs' then 'SP'
            when @piCommissionItem='Terms Type' then 'TT'
            
            end

-- !!! need to check if rate is to be deleted - dateto = 1999-01-01
if @piDateTo = '1999-01-01'
    Begin
        delete SalesCommissionMultiSpiffRates
            Where Item1= @piItem1
            and DateFrom= @piDateFrom

    goto End_Procedure
    End




-- If record exists - no changed has occurred?
if  not exists (select * from SalesCommissionMultiSpiffRates
        Where Item1= @piItem1
        --and Percentage=@piPercentage
        and DateFrom= @piDateFrom
        and ComBranchNo=@piBranchNo			-- 21/07/09
        and DateTo=@piDateTo)
    BEGIN
    -- here if record does not exist - therefore changed in .Net
        
    Select @RecDate=max(DateFrom)
        From SalesCommissionMultiSpiffRates
        Where Item1= @piItem1
			and ComBranchNo=@piBranchNo			-- 21/07/09

    -- Update existing commission end date
    update SalesCommissionMultiSpiffRates
        set DateTo=Dateadd(mi,-1,@piDateFrom)

         Where Item1= @piItem1
            and DateFrom=@RecDate
            and ComBranchNo=@piBranchNo			-- 21/07/09

        -- Insert New rate
     insert into SalesCommissionMultiSpiffRates 
            (Description,Item1,Item2,Item3,Item4,Item5,Percentage,Value,DateFrom,DateTo,EmpeenoChanged,ComBranchNo)
     select @piDescription,@piItem1,@piItem2,@piItem3,@piItem4,@piItem5,@piPercentage,@piValue,
				@piDateFrom,@piDateTo,@piEmpeeno,@piBranchNo			-- 21/07/09
	  -- Update ItemId  - if Item exists
	 UPDATE SalesCommissionMultiSpiffRates
		set ItemId1=s.Id 
	 from SalesCommissionMultiSpiffRates m INNER JOIN StockInfo s on m.Item1=s.iupc
	 Where m.Item1 !='' and m.ItemId1=0
	 
	 UPDATE SalesCommissionMultiSpiffRates
		set ItemId2=s.Id 
	 from SalesCommissionMultiSpiffRates m INNER JOIN StockInfo s on m.Item2=s.iupc
	 Where m.Item2 !='' and m.ItemId2=0
	 
	 UPDATE SalesCommissionMultiSpiffRates
		set ItemId3=s.Id 
	 from SalesCommissionMultiSpiffRates m INNER JOIN StockInfo s on m.Item3=s.iupc
	 Where m.Item3 !='' and m.ItemId3=0
	 
	 UPDATE SalesCommissionMultiSpiffRates
		set ItemId4=s.Id 
	 from SalesCommissionMultiSpiffRates m INNER JOIN StockInfo s on m.Item4=s.iupc
	 Where m.Item4 !='' and m.ItemId4=0
	 
	 UPDATE SalesCommissionMultiSpiffRates
		set ItemId5=s.Id 
	 from SalesCommissionMultiSpiffRates m INNER JOIN StockInfo s on m.Item5=s.iupc
	 Where m.Item5 !='' and m.ItemId5=0

    
    END

End_Procedure:

    SET NOCOUNT OFF

    SET @return = @@ERROR
    RETURN @return

GO
GRANT EXECUTE ON DN_SalesCommissionMultiSpiffRates_Save TO PUBLIC
GO
SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO
-- end end end end end end end end end end end end end end end end end end end end end end end 
