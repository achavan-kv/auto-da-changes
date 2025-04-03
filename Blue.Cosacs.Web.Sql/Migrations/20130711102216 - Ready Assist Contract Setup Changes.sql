-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here


IF EXISTS(select * from codecat where category = 'RDYCON')
BEGIN
	DELETE FROM codecat where category = 'RDYCON'

	DELETE FROM code where category = 'RDYCON'
END


IF  NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES  WHERE TABLE_SCHEMA = 'Sales' AND TABLE_NAME = 'LinkedContractNames')
BEGIN

	CREATE TABLE [Sales].[LinkedContractNames](
		[Contract] [varchar](25) NOT NULL
	 CONSTRAINT [pk_LinkedContractNames] PRIMARY KEY CLUSTERED 
(
	[Contract] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY])
END
GO

ALTER TABLE [Sales].[LinkedContracts] ADD CONSTRAINT
	FK_LinkedContracts_Contract FOREIGN KEY
	(
		[Contract]
	) REFERENCES [Sales].[LinkedContractNames]
	(
		[Contract]
	)


GO

