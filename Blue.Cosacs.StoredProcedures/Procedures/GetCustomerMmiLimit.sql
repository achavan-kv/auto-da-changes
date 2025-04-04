IF EXISTS(
			SELECT	1
			FROM	dbo.sysobjects
			WHERE	id = OBJECT_ID('[dbo].[GetCustomerMmiLimit]')
					AND OBJECTPROPERTY(id, 'IsProcedure') = 1
		  )
BEGIN
	DROP PROCEDURE [dbo].[GetCustomerMmiLimit]
END
GO


SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =======================================================================================
-- Project			: CoSaCS.NET
-- Procedure Name   : GetCustomerMmiLimit
-- Author			: Zensar (Amit)
-- Create Date		: 17 July 2020
-- Description		: This stored procedure is used to get customer MMI details.

-- Change Control
-- --------------
-- Ver	Date(YYYY-MM-DD)	By					Description
-- ---	----------------	----------------	------------
-- 001	2020-07-17			Zensar (Amit)		Get customer MMI limit value.
-- =======================================================================================

CREATE PROCEDURE [dbo].[GetCustomerMmiLimit]  
@CustId VARCHAR(20)
, @return INT OUT
  
AS  
BEGIN  

	SET NOCOUNT ON;
	SET @return = 0

	SELECT  ISNULL(CM.MmiLimit,0) AS MmiLimit
	FROM	[dbo].[CustomerMmi] AS CM WITH(NOLOCK)
	WHERE	CM.CustId = @CustId

	SELECT @return = @@error
END  
