SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS OFF
GO

if exists (select * from dbo.sysobjects
where id = object_id('[dbo].[DN_SRLinkChargeToAccountSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_SRLinkChargeToAccountSP]
GO


CREATE PROCEDURE dbo.DN_SRLinkChargeToAccountSP
-- =============================================
-- Author:		????
-- Create date: ??
-- Title:	DN_SRLinkChargeToAccountSP
--
--	This procedure will set up the Charge Account Link to SR
-- 
-- Change Control
-----------------
-- 18/01/11 jec CR1030 - Remove UAT40 change as it causes multiple accounts to be generated when
--                       primary chargeto is Deliverer
-- =====================================================
	-- Add the parameters for the function here
    @ServiceUniqueId    INTEGER,
    @AcctNo             CHAR(12),
    @ChargeType         CHAR(1),
    @Return             INTEGER OUTPUT

AS
    SET NOCOUNT ON
    SET @Return = 0

    -- Link a payment account to an SR

	--There is no need to repeatedly enter the same data
	IF NOT EXISTS(SELECT * FROM SR_ChargeAcct WHERE ServiceRequestNo = @ServiceUniqueId AND AcctNo = @AcctNo AND ChargeType = @ChargeType)
	BEGIN
    INSERT INTO SR_ChargeAcct (ServiceRequestNo, AcctNo, ChargeType)
    VALUES (@ServiceUniqueId, @AcctNo, @ChargeType)
	END

    -- UAT 40 If the primary charge-to is DEL then remove any customer charge-to account for this SR
 --   IF EXISTS(SELECT * FROM SR_Resolution WHERE ChargeTo = 'DEL' AND ServiceRequestNo = @ServiceUniqueId)
 --   BEGIN
	--	DELETE FROM SR_ChargeAcct WHERE ChargeType = 'C' AND ServiceRequestNo = @ServiceUniqueId
	--END
	
    SET @Return = @@error

    SET NOCOUNT OFF
    RETURN @Return
GO
SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO

-- End End End End End End End End End End End End End End End End End End End End End End End End End End