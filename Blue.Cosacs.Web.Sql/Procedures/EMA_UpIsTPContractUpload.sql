
IF EXISTS (SELECT * FROM sysobjects WHERE NAME = 'EMA_UpIsTPContractUpload')
BEGIN
DROP PROCEDURE [dbo].[EMA_UpIsTPContractUpload]
END
GO
 
CREATE PROCEDURE [dbo].[EMA_UpIsTPContractUpload]
(
	@CustId nvarchar(20),
	@AcctNo nvarchar(20)
)
AS
BEGIN
  	INSERT INTO CustTPContract(AcctNo,CustId,IsTPContractUpload,CreatedDate) 
	VALUES (@AcctNo,@CustId,1,getdate())
END
GO

