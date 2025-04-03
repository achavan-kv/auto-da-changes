-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- Changing datedel to small date time from date time to save space. Fixing data first to prevent errors on upgrade.
-- Put your SQL code here
UPDATE delivery SET datedel = a.dateacctopen 
FROM acct a 
WHERE datedel < '1-jan-1995'
AND a.acctno= delivery.acctno
AND a.dateacctopen >'1-jan-1995'
GO 
UPDATE delivery SET datedel = a.dateagrmt 
FROM agreement a 
WHERE delivery.datedel < '1-jan-1995'
AND a.acctno= delivery.acctno AND a.agrmtno = delivery.agrmtno
AND a.dateagrmt >'1-jan-1995'
GO 
update delivery 
SET datedel = '1-jan-1995'
WHERE datedel < '1-jan-1995'
GO 
SELECT * FROM fintrans WHERE acctno='551000851821'
GO 
-- timeout: 3600

UPDATE delivery
SET Datedel  = CASE WHEN Datedel > '2079-06-06' THEN '2079-06-06'
			        WHEN Datedel < '1900-01-01' THEN '1900-01-01'
				    ELSE Datedel END
				    
DECLARE @name VARCHAR(50)				    
				    
SET @name = (SELECT TOP 1  name FROM sysobjects
				 WHERE xtype = 'D'
				 AND NAME LIKE '%delivery%'
				 AND NAME LIKE '%datede%'
				 AND OBJECT_NAME(parent_obj) = 'delivery')

EXEC ('ALTER TABLE delivery
       DROP CONSTRAINT ' + @name)
GO
       
ALTER TABLE delivery
DROP CONSTRAINT chk_delivery_datedel    
GO				    
				    
ALTER TABLE delivery
ALTER COLUMN Datedel SMALLDATETIME NOT NULL

ALTER TABLE [dbo].[delivery]  WITH NOCHECK ADD  CONSTRAINT [chk_delivery_datedel] CHECK  (([datedel] >= '1-jan-1995'))
GO

ALTER TABLE [dbo].[delivery] CHECK CONSTRAINT [chk_delivery_datedel]
GO