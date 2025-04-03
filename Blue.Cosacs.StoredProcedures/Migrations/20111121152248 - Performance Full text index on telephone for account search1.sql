-- transaction: false
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

-- creating a full text index on telephone to allow searching
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.columns WHERE column_name = 'CustTelId' AND table_name ='custtel')
ALTER TABLE custtel ADD CustTelId INT IDENTITY NOT NULL 
--DROP INDEX custtel.ix_custtel_id 
IF NOT EXISTS (SELECT * FROM sysindexes WHERE NAME = 'ix_custtel_id')
CREATE UNIQUE INDEX ix_custtel_id  ON custtel(CusttelId ) 

IF NOT EXISTS(SELECT 1 FROM sysobjects s, sysobjects p -- p for parent
		      WHERE p.id = s.parent_obj AND s.TYPE = 'IT' AND p.NAME = 'custtel')
CREATE FULLTEXT INDEX ON custtel(telno,dialcode)
KEY INDEX ix_custtel_id 
 ON Address 


