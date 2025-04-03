IF EXISTS (SELECT * FROM sysindexes WHERE NAME = 'ix_accountlocking_lockcount')
DROP INDEX AccountLocking.ix_accountlocking_lockcount