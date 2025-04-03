SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_SRUpdateBatchPrintFlag]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_SRUpdateBatchPrintFlag]
GO

CREATE PROCEDURE [dbo].[DN_SRUpdateBatchPrintFlag] -- CR 1024 (NM 23/04/2009)
	@SRNo int,
	@BranchNo INT, --Cr 1056
	@return int output
AS
	-- SET NOCOUNT ON added to marginally improve performance 
	SET NOCOUNT ON;
    SET @return = 0
	
	DECLARE @IsRepairCentre BIT -- CR 1056
    
    SELECT @IsRepairCentre = ServiceRepairCentre FROM Branch WHERE BranchNo = @BranchNo
	
	IF @IsRepairCentre = 1
		UPDATE SR_ServiceRequest
		SET 
			RepairCentrePrintFlag = Case
								When RepairCentrePrintFlag = 'P' or RepairCentrePrintFlag = 'R' Then 'R'
								Else 'P'
							 End,
			BatchPrintDate = GetDate() 
		FROM SR_ServiceRequest 
		WHERE ServiceRequestNo = @SRNo
	ELSE
		UPDATE SR_ServiceRequest
		SET 
			BatchPrintFlag = Case
								When BatchPrintFlag = 'P' or BatchPrintFlag = 'R' Then 'R'
								Else 'P'
							 End,
			BatchPrintDate = GetDate() 
		FROM SR_ServiceRequest 
		WHERE ServiceRequestNo = @SRNo
	
	SET @return = @@ERROR

	RETURN @return
	
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO