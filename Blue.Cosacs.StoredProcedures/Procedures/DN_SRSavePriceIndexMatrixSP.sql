SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS OFF
GO

if exists (select * from dbo.sysobjects
where id = object_id('[dbo].[DN_SRSavePriceIndexMatrixSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_SRSavePriceIndexMatrixSP]
GO


CREATE PROCEDURE dbo.DN_SRSavePriceIndexMatrixSP
    @Supplier               VARCHAR(30),
    @Product                VARCHAR(30),
    @Year                   SMALLINT,
    @PartType               VARCHAR(30),
    @PartPercent            MONEY,
    @PartLimit              MONEY,
    @LabourPercent          MONEY,
    @LabourLimit            MONEY,
    @AdditionalPercent      MONEY,
    @AdditionalLimit        MONEY,
    @Return                 INTEGER OUTPUT

AS
    SET NOCOUNT ON
    SET @Return = 0


    -- Make sure every Service Supplier appears in Code Maintenance
    IF NOT EXISTS (SELECT 1 FROM Code
                   WHERE  Category = 'SRSUPPLIER'
                   AND    CodeDescript = @Supplier)
    BEGIN
        DECLARE @NextCode INTEGER

        -- Get the next code (expected to be numeric)
        SELECT  @NextCode = MAX(Code)
        FROM    Code
        WHERE   Category = 'SRSUPPLIER'
        AND     ISNUMERIC(Code) = 1

        -- The users must set up the special acctno in the Reference column
        INSERT INTO Code
           (Category, Code, CodeDescript, StatusFlag, SortOrder, Reference)
        VALUES
           ('SRSUPPLIER', ISNULL(@NextCode,0) + 1, @Supplier, 'L', 0, '')
    END


    -- Save the matrix row
    INSERT INTO SR_PriceIndexMatrix
       (Supplier,
        Product,
        Year,
        PartType,
        PartPercent,
        PartLimit,
        LabourPercent,
        LabourLimit,
        AdditionalPercent,
        AdditionalLimit)
    VALUES
       (@Supplier,
        @Product,
        @Year,
        @PartType,
        @PartPercent,
        @PartLimit,
        @LabourPercent,
        @LabourLimit,
        @AdditionalPercent,
        @AdditionalLimit)

    SET @Return = @@error

    SET NOCOUNT OFF
    RETURN @Return
GO
SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO

