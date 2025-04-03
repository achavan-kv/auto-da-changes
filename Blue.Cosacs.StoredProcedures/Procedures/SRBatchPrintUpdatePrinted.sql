IF EXISTS (SELECT * 
		   FROM sysobjects 
		   WHERE name = 'SRBatchPrintUpdatePrinted'
		   AND xtype = 'P')
BEGIN
	DROP PROCEDURE SRBatchPrintUpdatePrinted
END
GO

CREATE PROCEDURE SRBatchPrintUpdatePrinted
@srPrintedList varchar(MAX),
@branchno smallint
AS

BEGIN
	IF (SELECT ServiceRepairCentre FROM Branch WHERE BranchNo = @BranchNo) = 1
		UPDATE SR_ServiceRequest
		SET RepairCentrePrintFlag = Case
								When RepairCentrePrintFlag = 'P' or RepairCentrePrintFlag = 'R' Then 'R'
								Else 'P'
							 End,
			BatchPrintDate = GetDate() 
		FROM SR_ServiceRequest 
		INNER JOIN SplitFN(@srPrintedList,',') ON items = ServiceRequestNo 

	ELSE
		UPDATE SR_ServiceRequest
		SET 
			BatchPrintFlag = Case
								When BatchPrintFlag = 'P' or BatchPrintFlag = 'R' Then 'R'
								Else 'P'
							 End,
			BatchPrintDate = GetDate() 
		FROM SR_ServiceRequest 
		INNER JOIN SplitFN(@srPrintedList,',') ON items = ServiceRequestNo 
END
GO
