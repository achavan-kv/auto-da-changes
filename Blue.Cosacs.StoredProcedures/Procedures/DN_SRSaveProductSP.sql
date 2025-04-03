SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS OFF
GO

if exists (select * from dbo.sysobjects
where id = object_id('[dbo].[DN_SRSaveProductSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_SRSaveProductSP]
GO

-- =============================================
-- Author:		?
-- Create date: ?
-- Description:	Saves SR details
-- Modified By:	Jez Hemans
-- Modified For:CR 949/958 Print Location, ActionRequired, ReturnDate, FailureReason, CustomerCollected, Delivered and RepairedHome added	

-- Modified By:	Ilyas Parker
-- Modified For:#3681 - LW73631 - The Resolution.DateClosed was not always being saved with a timestamp.
-- =============================================


CREATE PROCEDURE dbo.DN_SRSaveProductSP
-- =============================================
-- Author:		????
-- Create date: ??
-- Title:	DN_SRSaveProductSP
--
--	This procedure will save the SR Product details.
-- 
-- Change Control
-----------------
-- 10/11/10 jec CR1030 - save Retailer
-- 29/11/10 jec CR1030 - save CustomerID
-- 06/01/11 jec CR1030 - save ReAssign technician code
-- 10/01/11 jec CR1030 - Update datechange if status or comments change
-- 12/01/11 jec CR1030 - Remove reassign columns from save here(saved in DN_SRBookServiceRequestSP)
-- 29/06/11 jec		#3969 - CR1254 Service request use itemId.
-- =============================================
	-- Add the parameters for the function here
    @ServiceBranchNo        SMALLINT,
    @ServiceUniqueId        INTEGER,
    @DateLogged             SMALLDATETIME,
    @DateReopened           SMALLDATETIME,
    @PurchaseDate           SMALLDATETIME,
    @StockLocn              SMALLINT,
    @ProductCode            VARCHAR(18),			-- RI
    @UnitPrice              MONEY,
    @Description            VARCHAR(25),
    @ModelNo                VARCHAR(15),
    @SerialNo               VARCHAR(30),
    @Status                 CHAR(1),
    @ReceivedDate           SMALLDATETIME,
    @ServiceEvaln           VARCHAR(12),
    @ServiceLocn            VARCHAR(12),
    @RepairEstimate         MONEY,
    @DeliveryDamage         CHAR(1),
    @ExtWarranty            CHAR(1),
    @GoodsOnLoan            CHAR(1),
    @DepositAmount          MONEY,
    @DepositPaid            CHAR(1),
    @TransitNotes           VARCHAR(200),
    @Comments               VARCHAR(5000),
    @DateCollected          SMALLDATETIME,
    @Zone                   VARCHAR(12),
    @TechnicianId           INTEGER,
    @PartsDate              SMALLDATETIME,
    @RepairDate             SMALLDATETIME,
    @IsAM                   CHAR(1),
    @Instructions           VARCHAR(200),
    @DateClosed             SMALLDATETIME,
    @Resolution             VARCHAR(12),
    @ResolutionChangedBy    INTEGER,
    @ChargeTo               VARCHAR(12),
    @ChargeToChangedBy      INTEGER,
    @ChargeToMake           VARCHAR(30),
    @ChargeToModel          VARCHAR(30),
    @HourlyRate             MONEY,
    @Hours                  MONEY,
    @LabourCost             MONEY,
    @AdditionalCost         MONEY,
    @TotalCost              MONEY,
    @GoodsOnLoanCollected   CHAR(1),
    @Replacement            CHAR(1),
    @FoodLoss               CHAR(1),
    @SoftScript             CHAR(1),
    @Deliverer              VARCHAR(12),
    @Fault					CHAR(4),
    @ActionRequired			VARCHAR(50),
    @PrintLocn				SMALLINT,
    @ReturnDate				SMALLDATETIME,
    @FailureReason			VARCHAR(12),
    @Delivered				CHAR(1),
    @CustomerCollected		CHAR(1),
    @RepairedHome			CHAR(1),
    @LbrCostEstimate		MONEY, -- CR 1024 (NM 29/04/2009)
    @AdtnlLbrCostEstimate	MONEY, -- CR 1024 (NM 29/04/2009)
    @TransportCostEstimate	MONEY, -- CR 1024 (NM 29/04/2009)
    @TechnicianReport		VARCHAR(200), -- CR 1024 (NM 29/04/2009)
    @TransportCost			MONEY, -- CR 1024 (NM 29/04/2009)
    @Retailer				VARCHAR(25),		--CR1030 jec
    @CustomerID				VARCHAR(20),		--CR1030 jec
    @ReAssignCode			VARCHAR(12),		--CR1030 jec
	--CR1030 - needs to be included with Reports Release
	--@SoftScriptDate			DateTime,		--CR1030 RM
	@ReAssignedBy			int,		--CR1030 jec
	--@itemid					INT,			-- RI
    @Return                 INTEGER OUTPUT

AS 
    SET NOCOUNT ON
    SET @Return = 0
    
    DECLARE @ExistingStatus CHAR(1), -- LW 71104
			@ExistingDateClosed smalldatetime							--IP - 26/09/11 - #3681 - LW73631
	
	SELECT @ExistingStatus = IsNull(Status, '')	FROM SR_ServiceRequest 
	WHERE ServiceBranchNo = @ServiceBranchNo AND 
			ServiceRequestNo = @ServiceUniqueId
				
	SELECT @ExistingDateClosed = DateClosed FROM SR_Resolution			--IP - 26/09/11 - #3681 - LW73631
	WHERE ServiceRequestNo = @ServiceUniqueId

	-- LW 71104 - Status can change to Resolved only if the resolution date is set
	IF @Status = 'R' and @ExistingStatus != @Status and @DateClosed = '1900-01-01'
		SET @Status = @ExistingStatus
	
	
	--IP - 26/09/11 - #3681 - LW73631	
	IF @DateClosed != '1900-01-01' AND @ExistingDateClosed = '1900-01-01' 
	BEGIN
		IF(datepart(minute, @DateClosed) = '00' and datepart(second, @DateClosed) = '00')
		begin
			SET @DateClosed = @DateClosed + convert(varchar(5), getdate(), 108)
		end	
	END
	ELSE	
		SET @DateClosed = @ExistingDateClosed

    -- Save the SR
    UPDATE  SR_ServiceRequest
    SET     DateLogged      = @DateLogged,
            DateReopened    = @DateReopened,
            PurchaseDate    = @PurchaseDate,
            StockLocn       = @StockLocn,
            ProductCode     = @ProductCode,
            UnitPrice       = @UnitPrice,
            Description     = @Description,
            ModelNo         = @ModelNo,
            SerialNo        = @SerialNo,
            Status          = @Status,
            ReceivedDate    = @ReceivedDate,
            ServiceEvaln    = @ServiceEvaln,
            ServiceLocn     = @ServiceLocn,
            RepairEstimate  = @RepairEstimate,
            DeliveryDamage  = @DeliveryDamage,
            ExtWarranty     = @ExtWarranty,
            GoodsOnLoan     = @GoodsOnLoan,
            DepositAmount   = @DepositAmount,
            DepositPaid     = @DepositPaid,
            TransitNotes    = @TransitNotes,
            Comments        = @Comments,
            --//CR1030 - needs to be included with Reports Release
			--SoftScriptDate  = @softscriptdate,
            DateCollected   = @DateCollected,
            ActionRequired	= @ActionRequired,
            PrintLocn		= @PrintLocn,
            LbrCostEstimate	= @LbrCostEstimate, -- CR 1024 (NM 29/04/2009)
            AdtnlLbrCostEstimate = @AdtnlLbrCostEstimate,  -- CR 1024 (NM 29/04/2009)
            TransportCostEstimate = @TransportCostEstimate, -- CR 1024 (NM 29/04/2009)
            TechnicianReport = @TechnicianReport, -- CR 1024 (NM 29/04/2009)
            Retailer = @Retailer,		--CR1030 jec
            CustId = @CustomerID,		--CR1030 jec
            -- Update Date change if status or comments have changed	--CR1030 jec
            DateChange = case when Status != @Status or Comments != @Comments then GETDATE() 
							else DateChange end
	--From StockInfo s					-- RI
    WHERE   ServiceBranchNo = @ServiceBranchNo
    AND     ServiceRequestNo = @ServiceUniqueId
    --and s.ID = @itemId					-- RI

	-- 72107 SC 15/06/10
	UPDATE SR 
	SET Sequence = (SELECT COUNT(*) 
					FROM SR_ServiceRequest SR2
					WHERE SR.AcctNo      = SR2.AcctNo         
					 AND  SR.InvoiceNo   = SR2.InvoiceNo      
					 AND  SR.StockLocn   = SR2.StockLocn      
					 --AND  SR.ProductCode = SR2.ProductCode 
					 AND  SR.ItemID = SR2.ItemID				-- RI   
					 AND  SR.SerialNo    = SR2.SerialNo)  
	FROM sr_servicerequest SR
	WHERE ServiceRequestNo = @ServiceUniqueId

    UPDATE  SR_Allocation
    SET     Zone          = @Zone,
            TechnicianId  = @TechnicianId,
            PartsDate     = @PartsDate,
            RepairDate    = @RepairDate,
            IsAM          = @IsAM,
            Instructions  = @Instructions
            --ReAssignCode  = @ReAssignCode,		--CR1030 
            --ReAssignedBy  = @ReAssignedBy		--CR1030
    WHERE   ServiceRequestNo = @ServiceUniqueId
		-- Update only if changed		--jec
			and (Zone != @Zone
				or TechnicianId  != @TechnicianId
				or PartsDate     != @PartsDate
				or RepairDate    != @RepairDate
				or IsAM          != @IsAM
				or Instructions  != @Instructions)
				--or ISNULL(ReAssignCode,'') != @ReAssignCode)


    UPDATE  SR_Resolution
    SET     DateClosed           = @DateClosed,
            Resolution           = @Resolution,
            ResolutionChangedBy  = @ResolutionChangedBy,
            ChargeTo             = @ChargeTo,
            ChargeToChangedBy    = @ChargeToChangedBy,
            ChargeToMake         = @ChargeToMake,
            ChargeToModel        = @ChargeToModel,
            HourlyRate           = @HourlyRate,
            Hours                = @Hours,
            LabourCost           = @LabourCost,
            AdditionalCost       = @AdditionalCost,
            TotalCost            = @TotalCost,
            GoodsOnLoanCollected = @GoodsOnLoanCollected,
            Replacement          = @Replacement,
            FoodLoss             = @FoodLoss,
            SoftScript           = @SoftScript,
            Deliverer            = @Deliverer,
            Fault				 = @Fault,
            ReturnDate			 = @ReturnDate,
            FailureReason		 = @FailureReason,
            Delivered			 = @Delivered,
            CustomerCollected	 = @CustomerCollected,
            RepairedHome		 = @RepairedHome,
            TransportCost		 = @TransportCost -- CR 1024 (NM 29/04/2009)
            
    WHERE   ServiceRequestNo = @ServiceUniqueId

	IF (SELECT TotalCost FROM SR_Resolution WHERE ServiceRequestNo = @ServiceUniqueId) > 0		
	-- UAT 416 Payment should only be written to the SR_TechnicianPayment table if the SR has been resolved i.e. a resolution date has been set
	AND @DateClosed <> '1900-01-01'
	BEGIN
		EXEC DN_SRSaveTechnicianPayment @TechnicianId,@ServiceUniqueId,@DateClosed,@TotalCost,'',0
	END

    SET @Return = @@error

    SET NOCOUNT OFF
    RETURN @Return
GO
SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO

-- End End End End End End End End End End End End End End End End End End End End End End End End End End
