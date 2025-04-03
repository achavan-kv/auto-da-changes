-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
IF NOT EXISTS (SELECT * FROM sysobjects WHERE NAME = 'StoreCardStatements')
CREATE TABLE StoreCardStatements 
(Acctno CHAR(12) NOT NULL ,
 DateFrom SMALLDATETIME NOT NULL ,
 DateTo	SMALLDATETIME NOT null,
 DatePrinted	SMALLDATETIME,
 DateLastReprinted SMALLDATETIME,	
 ReprintedBy INT )
 
GO 
IF NOT EXISTS (SELECT * FROM sysobjects WHERE NAME = 'StoreCardStatements')
	ALTER TABLE StoreCardStatements ADD CONSTRAINT pk_StoreCardStatements PRIMARY KEY ( acctno,datefrom)
GO 	