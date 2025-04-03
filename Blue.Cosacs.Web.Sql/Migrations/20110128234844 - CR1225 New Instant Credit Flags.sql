-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

INSERT INTO [codecat]
           ([origbr],[category],[catdescript],[codelgth]
           ,[forcenum],[forcenumdesc],[usermaint])

SELECT		hobranchno, 'ICF', 'Instant Credit Flags', 3
			,'N', 'N', 'N'
FROM country
GO

INSERT INTO [code]
           ([origbr],[category],[code]
           ,[codedescript],[statusflag]
           ,[sortorder],[reference])
           
SELECT		hobranchno, 'ICF', 'DEP'
			,'Awaiting Deposit', 'L'
			,0, 0
FROM country
 
GO

INSERT INTO [code]
           ([origbr],[category],[code]
           ,[codedescript],[statusflag]
           ,[sortorder],[reference])
           
SELECT		hobranchno, 'ICF', 'INS'
			,'Awaiting First Instalment', 'L'
			,0, 0
FROM country

GO
  
INSERT INTO [code]
           ([origbr],[category],[code]
           ,[codedescript],[statusflag]
           ,[sortorder],[reference])
           
SELECT		hobranchno, 'ICF', 'ARR'
			,'Account in Arrears', 'L'
			,0, 0
FROM country

GO

INSERT INTO [code]
           ([origbr],[category],[code]
           ,[codedescript],[statusflag]
           ,[sortorder],[reference])
           
SELECT		hobranchno, 'ICF', 'CHQ'
			,'Awaiting Cheque Clearance', 'L'
			,0, 0
FROM country

GO


