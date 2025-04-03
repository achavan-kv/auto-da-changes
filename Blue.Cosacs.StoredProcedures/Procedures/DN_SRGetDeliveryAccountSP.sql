

/****** Object:  StoredProcedure [dbo].[DN_SRGetDeliveryAccountSP]    Script Date: 11/20/2007 17:50:11 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
if exists (select * from dbo.sysobjects
where id = object_id('[dbo].[DN_SRGetDeliveryAccountSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_SRGetDeliveryAccountSP]
GO
-- =============================================
-- Author:		Jez Hemans
-- Create date: 20/11/2007
-- Description:	Returns the delivery account for the selected deliverer
-- =============================================
CREATE PROCEDURE [dbo].[DN_SRGetDeliveryAccountSP]
	@deliveryID VARCHAR(20),
	@ServiceUniqueId INT,
	@Return             INTEGER OUTPUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON
    SET @Return = 0
    
    DECLARE @AcctNo CHAR(12)
    
    SET @AcctNo = (SELECT DISTINCT acctno FROM acct a INNER JOIN code c ON a.acctno = c.reference
    WHERE c.category = 'SRDELIVERER' AND c.code = @deliveryID)
    
    IF NOT EXISTS(SELECT * FROM SR_ChargeAcct WHERE ServiceRequestNo = @ServiceUniqueId AND AcctNo = @AcctNo AND ChargeType = 'D')
	BEGIN
    INSERT INTO SR_ChargeAcct (ServiceRequestNo, AcctNo, ChargeType)
    VALUES (@ServiceUniqueId, @AcctNo, 'D')
	END
	
	SELECT @AcctNo
	
    SET NOCOUNT OFF
    
END
