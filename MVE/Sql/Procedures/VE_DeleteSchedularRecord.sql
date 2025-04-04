if exists (select * from dbo.sysobjects where id = object_id('[dbo].[VE_DeleteSchedularRecord]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
BEGIN
	DROP PROCEDURE [dbo].[VE_DeleteSchedularRecord]
END
GO

Create PROCEDURE [dbo].[VE_DeleteSchedularRecord]
	 @ServiceCode		VARCHAR(10)
	,@Code				VARCHAR(10)
	,@IsInsertRecord	BIT = 0
	,@IsEODRecords		BIT = 0
	,@Message VARCHAR(MAX) OUTPUT
AS
BEGIN
	SET NOCOUNT ON;
	
	SET @Message = ''	
	BEGIN			
		UPDATE VE_TaskSchedular SET 
			Status='True' 
		WHERE
			Code=@Code 
			AND ServiceCode=@ServiceCode
			AND IsInsertRecord=@IsInsertRecord 
			AND IsEODRecords=@IsEODRecords				
		SET @Message = 'Record deleted successfully.';
	END
END