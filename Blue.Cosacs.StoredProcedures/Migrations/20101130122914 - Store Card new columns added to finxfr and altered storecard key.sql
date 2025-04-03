-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

--Store Card - Add new columns to the FinXfr table to log extra information when making purchases using the Store Card
--Agrmtno will store the agreement no for Cash & Go when purchasing items through Cash & Go 

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.columns  WHERE table_name ='finxfr' AND column_name = 'agrmtno')
    alter table finxfr add agrmtno int null
go

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.columns  WHERE table_name ='finxfr' AND column_name = 'storecardno')
	alter table finxfr add storecardno bigint null
go

IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[StoreCard]') AND name = N'pk_storecard')
	ALTER TABLE [dbo].[StoreCard] DROP CONSTRAINT [pk_storecard]


ALTER TABLE [dbo].[StoreCard] ADD  CONSTRAINT [pk_storecard] PRIMARY KEY NONCLUSTERED 
(
	[CardNumber] ASC
)ON [PRIMARY]

ALTER TABLE finxfr ADD CONSTRAINT
	FK_StoreCardNo FOREIGN KEY
	(
		storecardno
	) REFERENCES StoreCard
	(
		CardNumber
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
go	