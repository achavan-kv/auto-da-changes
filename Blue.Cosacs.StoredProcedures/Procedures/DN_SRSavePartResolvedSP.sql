SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS OFF
GO

if exists (select * from dbo.sysobjects
where id = object_id('[dbo].[DN_SRSavePartResolvedSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_SRSavePartResolvedSP]
GO


CREATE PROCEDURE dbo.DN_SRSavePartResolvedSP
-- =============================================
-- Author:		????
-- Create date: ??
-- Title:	DN_SRSavePartResolvedSP.PRC
--
--	This procedure will save any spare parts for a Service Request
-- 
-- Change Control
-----------------
-- 04/07/11 IP CR1254 - RI - #3994 - RI System Integration
-- =============================================
    @ServiceRequestNo   INTEGER,
    --@PartNo             VARCHAR(8),
    @PartNo             VARCHAR(18),		--IP - 04/07/11 - CR1254 - RI - #3994
    @PartID				INTEGER,			--IP - 04/07/11 - CR1254 - RI - #3994
    @Quantity           FLOAT,
    @UnitPrice          MONEY,
    @Description        VARCHAR(25),
    @PartType           VARCHAR(30),
	@StockLocn			SMALLINT,			 --IP - 18/06/09 - UAT(687) - Added StockLocn
    @Return             INTEGER OUTPUT

AS
    SET NOCOUNT ON
    SET @Return = 0


    -- Mark any existing SR as a history record
    INSERT INTO SR_PartListResolved
       (ServiceRequestNo,
        PartNo,
        Quantity,
        UnitPrice,
        Description,
        PartType,
		StockLocn,							--IP - 18/06/09 - UAT(687) - Added StockLocn)
		PartID)								--IP - 04/07/11 - CR1254 - RI - #3994					
    VALUES	
       (@ServiceRequestNo,
        @PartNo,												
        @Quantity,
        @UnitPrice,
        @Description,
        @PartType,
		@StockLocn,							--IP - 18/06/09 - UAT(687) - Added StockLocn)   
		@PartID)							--IP - 04/07/11 - CR1254 - RI - #3994					


    SET @Return = @@error

    SET NOCOUNT OFF
    RETURN @Return
GO
SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO

