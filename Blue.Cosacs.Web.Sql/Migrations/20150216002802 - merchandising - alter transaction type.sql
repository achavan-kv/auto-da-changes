update merchandising.transactiontype set [SplitDebitByDepartment] = 0
update merchandising.transactiontype set [SplitCreditByDepartment] = 0

alter table merchandising.transactiontype
alter column [SplitDebitByDepartment] bit not null

alter table merchandising.transactiontype
alter column [SplitCreditByDepartment] bit not null