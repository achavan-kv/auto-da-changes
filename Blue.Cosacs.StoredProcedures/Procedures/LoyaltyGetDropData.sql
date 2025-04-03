
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT * FROM SYSOBJECTS 
           WHERE NAME = 'LoyaltyGetDropData'
           AND xtype = 'P')
BEGIN 
DROP PROCEDURE LoyaltyGetDropData
END
GO

CREATE PROCEDURE [dbo].[LoyaltyGetDropData]
@return INT OUTPUT

AS
BEGIN
	SELECT codedescript,code,category,reference,sortorder  FROM code
	WHERE category IN ('HCM','HCA','HCR','HCI','HCC')
	AND statusflag = 'L'
	
	SET @return = 0
END
GO


