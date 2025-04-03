-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

Alter Table dbo.StoreCard
	Alter Column 
		[ProofAddress] [varchar](200) NULL
		
Alter Table dbo.StoreCard
	Alter Column 
		[ProofID] [varchar](200) NULL
		
Alter Table dbo.StoreCard
	Alter Column 
		[SecurityQ] [varchar](200) NULL
		
Alter Table dbo.StoreCard
	Alter Column 
		[SecurityA] [varchar](5000) NULL
		
