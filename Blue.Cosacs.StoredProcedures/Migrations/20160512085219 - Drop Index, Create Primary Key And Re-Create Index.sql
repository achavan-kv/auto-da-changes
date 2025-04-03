-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

DROP INDEX [pk_facttrans] ON [dbo].[facttrans]
GO

ALTER TABLE facttrans
ADD Id INT IDENTITY(1, 1)
GO

ALTER TABLE facttrans
ADD CONSTRAINT PK_FactTrans PRIMARY KEY CLUSTERED (Id)
GO

--This is ridiculous but it was already there as a clustered index...
CREATE NONCLUSTERED INDEX IX_FactTrans_Acctno_Agrmtno_Buffno_Itemno_Stocklocn_Trantype_Tccode_Trandate
ON facttrans ([acctno] ASC,
	          [agrmtno] ASC,
	          [buffno] ASC,
	          [itemno] ASC,
	          [stocklocn] ASC,
	          [trantype] ASC,
	          [tccode] ASC,
	          [trandate] ASC)
GO