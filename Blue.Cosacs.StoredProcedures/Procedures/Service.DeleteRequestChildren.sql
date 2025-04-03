if exists (select * from dbo.sysobjects where id = object_id('Service.DeleteRequestChildern') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
DROP PROCEDURE Service.DeleteRequestChildern
GO

CREATE PROCEDURE Service.DeleteRequestChildern
	@Requestid INT
AS
	DELETE FROM Service.RequestContact
	WHERE RequestId = @Requestid
	
	DELETE FROM Service.RequestFoodLoss
	WHERE RequestId = @Requestid
	
	
	DELETE FROM Service.RequestFoodLoss
	WHERE RequestId = @Requestid
	
	
	DELETE FROM Service.RequestPart
	WHERE RequestId = @Requestid
	
	DELETE FROM Service.RequestScriptAnswer
	WHERE RequestId = @Requestid
	
GO
