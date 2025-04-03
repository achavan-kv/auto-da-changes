SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS OFF
GO

if exists (select * from dbo.sysobjects
where id = object_id('[dbo].[DN_SRSaveHistorySP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_SRSaveHistorySP]
GO

CREATE PROCEDURE dbo.DN_SRSaveHistorySP
-- =============================================
-- Author:		?
-- Create date: ?
-- Description:	
-- Modified By:	Jez Hemans
-- Modified For:	
-- Change Control
-- --------------
-- 03/08/10 - IP - UAT(16) UAT5.1.9.0 - Merged LW (72107) from 5.1.8.3
-- 29/06/11 jec		#3969 - CR1254 Service request use itemId.
-- =============================================

    @AcctNo             CHAR(12),
    @InvoiceNo          INTEGER,
    @StockLocn          SMALLINT,
    --@ProductCode        VARCHAR(8),
    @itemid				INT,				-- RI
    @SerialNo           VARCHAR(30),
    @Return             INTEGER OUTPUT

AS
    SET NOCOUNT ON
    SET @Return = 0

    -- Mark any existing and closed SR as a history record.
    -- Note that Courts Accounts can have multiple open SRs against
    -- the same item where a multiple quantity was sold.
    
    -- Need to uniquely identify multiple items with distinct serial numbers and not set history on their SR's; unless exactly the same one is being re-opened.
     
    UPDATE SR_ServiceRequest
    SET    History = 'Y'
    FROM   SR_Resolution r
    WHERE  SR_ServiceRequest.AcctNo = @AcctNo
    AND    SR_ServiceRequest.InvoiceNo = @InvoiceNo
    AND    SR_ServiceRequest.StockLocn = @StockLocn
    --AND    SR_ServiceRequest.ProductCode = @ProductCode
    AND    SR_ServiceRequest.ItemID = @itemid			-- RI
    AND    SR_ServiceRequest.History = 'N'
    AND    r.ServiceRequestNo = SR_ServiceRequest.ServiceRequestNo
    AND    r.DateClosed > CONVERT(SMALLDATETIME, '1 Jan 1900', 106)
	AND    SR_ServiceRequest.SerialNo = @SerialNo
	AND		rtrim(ltrim(@SerialNo)) != '' --IP - 03/08/10 

    SET @Return = @@error

    SET NOCOUNT OFF
    RETURN @Return
GO
SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO

-- End End End End End End End End End End End End End End End End End End End End End End End End End End End End End End End End End
