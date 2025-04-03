--ADDEDBY DATE		PURPOSE
--KD	21/06/2019	TO Alter Acct Table to add new Column  

IF NOT EXISTS (SELECT * FROM syscolumns
			   WHERE name = 'IsAmortizedOutStandingBal'
               AND OBJECT_NAME(id) = 'acct')


Alter table acct add IsAmortizedOutStandingBal bit NOT NULL Default(0) 