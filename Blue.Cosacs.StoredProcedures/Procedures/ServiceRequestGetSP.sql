
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT * FROM SYSOBJECTS 
           WHERE NAME = 'ServiceRequestGetSP'
           AND xtype = 'P')
BEGIN 
DROP PROCEDURE ServiceRequestGetSP
END
GO

CREATE PROCEDURE ServiceRequestGetSP
@acctno varchar(12),
				@itemId varchar(12),
				@stocklocn int 

AS
BEGIN
	select * from Service.Request 
	Where Account=@acctno and ItemId=@itemId 
	and State='New' and ItemStockLocation=@stocklocn
END

