SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS OFF
GO

IF EXISTS (SELECT * FROM SysObjects
           WHERE Name = 'DN_GoodsReturn_CollectReason' AND Type = 'P')
BEGIN
    DROP PROCEDURE DN_GoodsReturn_CollectReason
END
GO


CREATE PROCEDURE DN_GoodsReturn_CollectReason

--------------------------------------------------------------------------------
--
-- Project      : CoSACS .NET
-- File Name    : DN_GoodsReturn_CollectReason.prc
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Goods Return - Colletion Reason save
-- Author       : John Croft
-- Date         : 26 October 2006
--
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 27/04/11 jec CR1212 RI Integration - Use ItemID instead of ItemNo
--------------------------------------------------------------------------------
    -- Parameters
     
    @acctno varchar(12),
    --@itemno varchar(8),
    @itemId	INT,			-- RI
    @stocklocn smallint,
    @changedBy int,
    @collectreason varchar(30),   
    @collecttype varchar(3),    
    @return int OUTPUT


AS  --DECLARE
    -- Local variables

set @return=0

 -- save collection reason CR36 jec 26/10/06
    IF @@ERROR = 0
        Begin

            insert into CollectionReason (AcctNo,ItemNo,CollectionReason,DateAuthorised,EmpeenoAuthorised,
                                            DateCommissionCalculated,StockLocn,CollectType,ItemId)			-- RI
            select @acctno,'',c.code,getdate(),@changedBy,null,@stocklocn,@collecttype,@ItemId				-- RI
                from code c
                    where category='RGR' and codedescript=@collectreason
    
            IF @@ERROR != 0 SET @return = @@ERROR
        End
    
 SET @return = @@ERROR

      
GO
GRANT EXECUTE ON DN_GoodsReturn_CollectReason TO PUBLIC
GO
SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO
-- end end end end end end end end end end end end end end end end end end end end end end end 


