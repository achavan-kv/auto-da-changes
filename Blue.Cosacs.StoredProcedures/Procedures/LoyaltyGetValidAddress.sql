
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT * FROM SYSOBJECTS 
           WHERE NAME = 'LoyaltyGetValidAddress'
           AND xtype = 'P')
BEGIN 
DROP PROCEDURE LoyaltyGetValidAddress
END
GO

CREATE PROCEDURE [dbo].[LoyaltyGetValidAddress]
@custid VARCHAR(20),
@return INT output
AS
BEGIN
	

SELECT addtype ,CASE WHEN ISNUMERIC(cuspocode) = 1 AND CAST(cuspocode AS INT) >999 THEN 1 ELSE 0 END  AS valid
FROM custaddress c1
WHERE  custid = @CUSTID
AND datemoved IS NULL 

END
GO


