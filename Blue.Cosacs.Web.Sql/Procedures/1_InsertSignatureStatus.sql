IF EXISTS (SELECT * FROM sysobjects 
   WHERE NAME = 'InsertSignatureStatus'
   )
BEGIN
DROP PROCEDURE [dbo].[InsertSignatureStatus]
END
GO


CREATE PROCEDURE [dbo].[InsertSignatureStatus] 
@custId as varchar(20),
@acctNo as varchar(20)
AS
BEGIN

IF Not Exists(select 1 from SignatureStatus where custId=@custId and acctNo=@acctNo) 
	
	insert into SignatureStatus(acctNo,custId,updatedDate)
	values (@acctNo,@custId,getdate())

END
