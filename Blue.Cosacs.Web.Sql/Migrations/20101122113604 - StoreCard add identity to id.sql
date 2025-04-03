-- put your SQL code here
IF EXISTS (SELECT * FROM sysobjects WHERE NAME = 'pk_storecard')
ALTER TABLE dbo.StoreCard DROP CONSTRAINT pk_storecard  
GO 
CREATE CLUSTERED INDEX ix_acctno ON dbo.StoreCard(acctno)

GO 

ALTER TABLE dbo.StoreCard
DROP COLUMN id 
GO 
ALTER TABLE storecard ADD id INT IDENTITY
GO 
ALTER TABLE dbo.storecard ADD CONSTRAINT
pk_storecard PRIMARY KEY(id )
GO 
