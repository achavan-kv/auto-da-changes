if exists (select * from dbo.sysobjects where id = object_id('[dbo].[VE_GetDateofBirth]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
BEGIN
	DROP PROCEDURE [dbo].[VE_GetDateofBirth]
END
GO

Create PROCEDURE [dbo].[VE_GetDateofBirth]
	@CustId varchar(50)
AS
BEGIN
	SET NOCOUNT ON;
	select dateborn from customer where custid=@CustId
END




