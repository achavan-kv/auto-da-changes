
/****** Object:  UserDefinedFunction [dbo].[fn_SRGetServiceRequestNo]    Script Date: 11/15/2006 09:55:09 ******/
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID('[dbo].[fn_SRGetServiceRequestNo]') AND xtype in ('FN', 'IF', 'TF'))
DROP FUNCTION [dbo].[fn_SRGetServiceRequestNo]
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Peter Chong
-- Create date: 15-Nov-2006
-- Description:	Calculates the service request number from a service request id
-- =============================================
CREATE FUNCTION fn_SRGetServiceRequestNo
(
	-- Add the parameters for the function here
	@ServiceRequestNo int
)
RETURNS VARCHAR(20)
AS
BEGIN
	-- Declare the return variable here
	DECLARE @Result VARCHAR(20)

	-- Add the T-SQL statements to compute the return value here
	SELECT @Result = convert(varchar(4), ServiceBranchNo) + convert(varchar(16), ServiceRequestNo)
	FROM SR_ServiceRequest
	WHERE ServiceRequestNo = @ServiceRequestNo

	-- Return the result of the function
	RETURN @Result

END
GO

