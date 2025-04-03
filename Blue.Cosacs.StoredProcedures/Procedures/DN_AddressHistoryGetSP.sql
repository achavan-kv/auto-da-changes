SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects
where id = object_id('[dbo].[DN_AddressHistoryGetSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_AddressHistoryGetSP]
GO


CREATE PROCEDURE dbo.DN_AddressHistoryGetSP
    @piCustId    VARCHAR(20),
    @Return      INTEGER OUTPUT

AS --DECLARE


BEGIN
    SET @Return = 0

    -- List all the customer address history
	SELECT CustId, 
		AddType, 
		DateIn, 
		ISNULL(DateMoved, CONVERT(DATETIME,'1 Jan 1900',106)) AS DateMoved, 
		CusAddr1, 
		ISNULL(CusAddr2,'')  AS CusAddr2, 
		ISNULL(CusAddr3,'')  AS CusAddr3, 
		ISNULL(CusPoCode,'') AS CusPoCode,
		Email,
		DeliveryArea
    FROM  CustAddress
    WHERE CustId = @piCustId
	ORDER BY CustId ASC, AddType ASC, DateIn DESC, DateMoved DESC
	
    SET @Return = @@ERROR

END
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO
