-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF EXISTS(select * from sys.indexes where name = 'ix_fintrans_new_income_empeeno' and object_id = OBJECT_ID('fintrans_new_income'))
BEGIN
    DROP INDEX [ix_fintrans_new_income_empeeno] ON [dbo].[fintrans_new_income]
END
GO

CREATE NONCLUSTERED INDEX ix_fintrans_new_income_empeeno
ON [dbo].[fintrans_new_income] ([empeeno])
INCLUDE ([datetrans])