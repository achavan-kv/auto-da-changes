
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT * FROM SYSOBJECTS 
           WHERE NAME = 'auditloyalty'
           AND xtype = 'TR')
BEGIN 
DROP TRIGGER auditloyalty
END
GO

IF EXISTS (SELECT * FROM SYSOBJECTS 
           WHERE NAME = 'trig_auditloyalty'
           AND xtype = 'TR')
BEGIN 
DROP TRIGGER trig_auditloyalty
END
GO


CREATE TRIGGER trig_auditloyalty 
ON Loyalty
AFTER INSERT,UPDATE,DELETE 
AS 
  BEGIN 
    INSERT INTO loyaltyaudit 
               (memberno, 
                custid,
                loyaltyacct, 
                startdate, 
                enddate, 
                membertype, 
                statusacct, 
                statusvoucher, 
                empeeno,
                changetype, 
                dateaudit) 
    SELECT memberno, 
           custid, 
           loyaltyacct,
           startdate, 
           enddate, 
           membertype, 
           statusacct, 
           statusvoucher, 
           empeeno,
           'D', 
           Getdate() 
    FROM   deleted 
     
    INSERT INTO loyaltyaudit 
               (memberno, 
                custid, 
                loyaltyacct,
                startdate, 
                enddate, 
                membertype, 
                statusacct, 
                statusvoucher, 
                empeeno,
                changetype, 
                dateaudit) 
    SELECT memberno, 
           custid, 
           loyaltyacct,
           startdate, 
           enddate, 
           membertype, 
           statusacct, 
           statusvoucher, 
           empeeno,
           'I', 
           Getdate() 
    FROM   inserted 
  END 


GO


