/****** Object:  StoredProcedure [dbo].[DN_SRGetSingleServiceRequestSP]    Script Date: 01/15/2007 17:02:31 ******/
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID('[dbo].[DN_SRGetSingleServiceRequestSP]') AND OBJECTPROPERTY(id,'IsProcedure') = 1)
DROP PROCEDURE [dbo].[DN_SRGetSingleServiceRequestSP]

SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER OFF
GO


--DECLARE @Return INT EXEC DN_SRGetSingleServiceRequestSP '18125','900',@Return

CREATE PROCEDURE [dbo].[DN_SRGetSingleServiceRequestSP]
-- ================================================
-- Project      : CoSACS .NET
-- File Name    : DN_SRGetSingleServiceRequestSP.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Get Single Service Request 
-- Author       : Peter Chong
-- Date         : 19-Oct-2006
--
-- This procedure will retrieve the Searches for a service request.

-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 04/07/11 jec CR1254 IUPC and CourtsCode will be shown on the service request search screen
-- ================================================
	-- Add the parameters for the stored procedure here
	@ServiceUniqueId	INTEGER,
	@ServiceBranchNo	SMALLINT,
	@User				INT,
    @Return             INTEGER OUTPUT

AS    
	SET NOCOUNT ON
    SET @Return = 0

		DECLARE @CustId VARCHAR(20),@AcctNo CHAR(12),@InvoiceNo INT,@ServiceType CHAR(1),--,@Return1 INT
				@IsPaidAndTaken BIT --IP - 31/07/09 - UAT(741) - Pass in IsPaidAndTaken as false into DN_SRGetCourtsAccountSP

		SET @IsPaidAndTaken = 0
		
		SELECT @CustId = CustId,
			   @AcctNo = AcctNo,
		       @InvoiceNo = InvoiceNo,
			   @ServiceType = ServiceType
        FROM   SR_ServiceRequest
        WHERE  ServiceBranchNo = @ServiceBranchNo
        AND    ServiceRequestNo = @ServiceUniqueId

		IF @ServiceType = 'C'
		BEGIN
			EXEC DN_SRGetCourtsAccountSP @ServiceBranchNo,@ServiceUniqueId,@AcctNo,@InvoiceNo,0,@User,@IsPaidAndTaken,@Return 
		END
		ELSE IF @ServiceType = 'N'
		BEGIN
			EXEC DN_SRGetNonCourtsAccountSP @ServiceBranchNo,@ServiceUniqueId,@CustId,@Return
		END
		ELSE IF @ServiceType = 'S'
		BEGIN
			EXEC DN_SRGetInternalStockSP @ServiceBranchNo,@ServiceUniqueId,@CustId,@Return
		END
		-- CR 949/958
		ELSE IF @ServiceType = 'G'
		BEGIN
			EXEC DN_SRGetNonCourtsAccountSP @ServiceBranchNo,@ServiceUniqueId,@CustId,@Return
		END


        

    SET @Return = @@error

	SET NOCOUNT OFF
	RETURN @Return
go
-- End End End End End End End End End End End End End End End End End End End End End End End End End End End End End End End End End End End End
