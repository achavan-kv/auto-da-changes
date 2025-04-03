if exists (select * from sys.indexes where name = 'ix_instantcreditflag_chektype')
begin 
    drop index ix_instantcreditflag_chektype on [dbo].[instantcreditflag]
end

CREATE NONCLUSTERED INDEX ix_instantcreditflag_chektype
ON [dbo].[instantcreditflag] ([checktype])
INCLUDE ([acctno])
GO
