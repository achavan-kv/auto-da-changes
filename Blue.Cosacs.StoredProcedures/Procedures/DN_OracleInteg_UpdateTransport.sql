SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_OracleInteg_UpdateTransport]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_OracleInteg_UpdateTransport]
GO

-- =============================================
-- Author:		Mohamed Nasmi
-- Create date: 03/03/2009
-- Description:	To update Transport(Freight Carrier) table from Oracle inbound CSV files
-- =============================================

		
CREATE PROCEDURE [dbo].[DN_OracleInteg_UpdateTransport] 
	@truckid varchar(26),
	@drivername varchar(50),
	@phoneno varchar(20),
	@carrierNumber varchar(20),
	@status varchar(12),
	@createdDate datetime,
	@activeEndDate datetime,
	@lastUpdatedDate datetime,	
	@return INT OUTPUT

AS
	-- SET NOCOUNT ON added to marginally improve performance 
	SET NOCOUNT ON;
    SET @return = 0
	
	UPDATE Transport
	SET 
		drivername = @drivername,
		--phoneno = @phoneno,
		carrierNumber = @carrierNumber,
		status = @status,
		createdDate = @createdDate,
		activeEndDate = @activeEndDate,
		lastUpdatedDate = @lastUpdatedDate			
	WHERE 
	truckid = @truckid

	IF @@ROWCOUNT = 0

   INSERT INTO Transport
    ( 
		origbr, truckid, drivername, phoneno,
		carrierNumber, status, createdDate,
		lastUpdatedDate, activeEndDate
	)
	VALUES
	( 
		0, @truckid, @drivername, @phoneno, 
		@carrierNumber, @status, @createdDate, 
		@lastUpdatedDate, @activeEndDate
	)  

	SET @return = @@ERROR

	RETURN @return
	
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO