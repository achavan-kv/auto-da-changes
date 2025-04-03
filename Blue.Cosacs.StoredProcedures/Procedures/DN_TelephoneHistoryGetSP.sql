
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects
where id = object_id('[dbo].[DN_TelephoneHistoryGetSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_TelephoneHistoryGetSP]
GO


CREATE PROCEDURE dbo.DN_TelephoneHistoryGetSP
    @piCustId    VARCHAR(20),
    @Return      INTEGER OUTPUT

AS --DECLARE


BEGIN
    SET @Return = 0

    -- List all the customer telephone history
    SELECT  CustId,
            TelLocn,
            DateChange,
            EmpeeNoChange,
            DateTelAdd,
            DateDiscon,
            DialCode,
            TelNo,
            ExtnNo
    FROM  CustTel
    WHERE CustId = @piCustId
    ORDER BY TelLocn ASC, DateChange DESC, DateTelAdd DESC
    
    SET @Return = @@ERROR

END
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO
