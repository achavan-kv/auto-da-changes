
IF EXISTS (SELECT * FROM sysobjects 
   WHERE NAME = 'UpdateCreditInformation'
   )
BEGIN
DROP PROCEDURE [dbo].[UpdateCreditInformation]
END
GO


-- =============================================
-- Author:		<Author,Bhupesh>
-- Create date: <Create Date,20-Dec-2019>
-- Description:	<Description,update due date>
-- =============================================
CREATE PROCEDURE UpdateCreditInformation

    @CustId VARCHAR(20),
	@AccountNumber NVARCHAR(50),
	@DayOfMonth INT = 0,
	@Message NVARCHAR(MAX) OUTPUT,
	@Status int OUTPUT

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    SET @Message = ''
	IF(NOT EXISTS(SELECT 1 FROM dbo.customer WHERE custid = @CustId))
	BEGIN
		SET @Message = 'No user found'
		SET @Status = 404
	END
	ELSE 
	BEGIN
	IF ISNULL(@AccountNumber,'') <> ''
		BEGIN
			DECLARE @Date DATETIME
			DECLARE @DateLast DATETIME
			DECLARE @Month INT  = MONTH(GETDATE())
			DECLARE @Year INT = YEAR(GETDATE())

			SET @Date = DATEFROMPARTS(@year, @month, @DayOfMonth)

			IF(DATEDIFF(d, @Date, GetDate()) > 0)
			BEGIN
				SET @DateLast = @Date
				SET @Date = DATEADD(month, 1, @Date)
			END
			ELSE
			BEGIN
				SET @DateLast = DATEADD(month, -1, @Date)
			END

			UPDATE dbo.instalplan
			SET DATEFIRST = @Date, dueday = @DayOfMonth, datelast = @DateLast
			WHERE acctno = @AccountNumber

		END
		SET @Message = 'Record updated Successessfully'
			SET @Status = 200

		
	END

	--Select @Message,@Status
END
