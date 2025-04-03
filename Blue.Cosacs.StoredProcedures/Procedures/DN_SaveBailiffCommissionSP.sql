SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_SaveBailiffCommissionSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_SaveBailiffCommissionSP]
GO


CREATE PROCEDURE DN_SaveBailiffCommissionSP

--------------------------------------------------------------------------------
--
-- Project      : CoSACS .NET ? 2003 Strategic Thought Ltd.
-- File Name    : DN_SaveBailiffCommissionSP.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Save Bailiff Commision transaction
-- Author       : D Richardson
-- Date         : 29 May 2003
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
--
--------------------------------------------------------------------------------

    -- Parameters
    @EmpeeNo        INT,
    @TransRefNo     INT,
    @AcctNo         VARCHAR(12),
    @DateTrans      DATETIME,
    @TransValue     FLOAT,
    @ChequeColln    CHAR(1),
    @Status         CHAR(1),

    @Return         INT OUTPUT
    

AS -- DECLARE
    -- Local variables

BEGIN

    SET @Return = 0
    set @TransValue = round(@TransValue,2)
    
    INSERT INTO BailiffCommn
        (EmpeeNo,
         TransRefNo,
         AcctNo,
         DateTrans,
         TransValue,
         ChequeColln,
         Status)
     VALUES
        (@EmpeeNo,
         @TransRefNo,
         @AcctNo,
         @DateTrans,
         @TransValue,
         @ChequeColln,
         @Status)


    SET @Return = @@ERROR
    RETURN @Return
END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

