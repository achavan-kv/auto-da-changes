SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects
           where id = object_id('[dbo].[CustomerUpdateStoreCardLimit]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[CustomerUpdateStoreCardLimit]
GO

CREATE PROCEDURE  CustomerUpdateStoreCardLimit
/***********************************************************************************************************
--
-- Project      : CoSACS .NET
-- File Name    : CustomerUpdateStoreCardLimit.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Description  : Procedure that updates the Store Card Limit for a Customer
-- Author       : ?
-- Date         : ?
--
--  
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 20/12/10  IP  Created Procedure

************************************************************************************************************/
            @custID varchar(20),
            @storeCardLimit money,
            @return int OUTPUT
  
AS
 
SET @return = 0
DECLARE @approved BIT 
IF @storeCardLimit >0 
	SET  @approved =1
ELSE
	SET @approved = 0 
	
IF(@storeCardLimit > 0)
BEGIN
	UPDATE Customer
	SET	   StoreCardLimit = @storeCardLimit,
	StoreCardApproved=@approved 
	WHERE  Custid = @custID
	AND NOT EXISTS (SELECT * FROM storecard s
					INNER JOIN StoreCardStatus ss ON s.CardNumber = ss.CardNumber
					WHERE ss.StatusCode = 'A'
					AND S.custid = @custid
					AND DateChanged = (SELECT MAX(DateChanged)
										 FROM StoreCardStatus ss2
										 WHERE ss2.CardNumber = ss.CardNumber))
END

IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END