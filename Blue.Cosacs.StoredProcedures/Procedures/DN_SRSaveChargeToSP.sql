SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS OFF
GO

if exists (select * from dbo.sysobjects
where id = object_id('[dbo].[DN_SRSaveChargeToSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_SRSaveChargeToSP]
GO


CREATE PROCEDURE dbo.DN_SRSaveChargeToSP
    @ServiceRequestNo   INTEGER,
    @SortOrder          SMALLINT,
    @ActualCost         MONEY,
    @InternalCharge     MONEY,
    @ExtWarranty        MONEY,
    @Supplier           MONEY,
    @Deliverer          MONEY,
    @Customer           MONEY,
    @Return             INTEGER OUTPUT

AS
    SET NOCOUNT ON
    SET @Return = 0

    -- Save the Charge To row, avoiding duplicate data!
	IF NOT EXISTS(SELECT ServiceRequestNo FROM SR_ChargeTo WHERE ServiceRequestNo = @ServiceRequestNo
	GROUP BY ServiceRequestNo HAVING COUNT(ServiceRequestNo) = 5)
	BEGIN
    INSERT INTO SR_ChargeTo
       (ServiceRequestNo,
        SortOrder,
        ActualCost,
        Internal,
        ExtWarranty,
        Supplier,
        Deliverer,
        Customer)
    VALUES
       (@ServiceRequestNo,
		@SortOrder,
		@ActualCost,
		@InternalCharge,
		@ExtWarranty,
		@Supplier,
		@Deliverer,
		@Customer)
	END

    SET @Return = @@error

    SET NOCOUNT OFF
    RETURN @Return
GO
SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO

