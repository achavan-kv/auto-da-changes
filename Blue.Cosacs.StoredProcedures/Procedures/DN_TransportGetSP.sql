SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_TransportGetSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_TransportGetSP]
GO

CREATE PROCEDURE 	dbo.DN_TransportGetSP
            @truckid varchar(26),
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	IF @truckid = ''
	BEGIN
		SELECT	truckid,
				drivername,
				phoneno,
				carrierNumber,
				Status
		FROM 	Transport
		ORDER BY truckid 
	END
	ELSE
	BEGIN
		SELECT	truckid,
				drivername,
				phoneno,
				carrierNumber,
				Status
		FROM 	Transport
		WHERE truckid LIKE @truckid + '%' and Status = 'Active'
	END

	SET @return = @@error
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO
/* code for testing only
declare @return integer
exec DN_TransportGetSP
@truckid = '',
@return = 0 */

