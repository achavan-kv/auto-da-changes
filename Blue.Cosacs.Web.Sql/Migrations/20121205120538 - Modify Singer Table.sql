-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

 if  not exists (select * from sys.objects where name = 'SingerAcct')

			CREATE TABLE SingerAcct
			(
				id INT IDENTITY(1,1),
				AcctNo VARCHAR(12),
				Custid varchar(20),
				SingerAcctNo varchar(7),
				SingerCustId varchar(70),
				BranchNo SMALLINT,
				AgreementTotal MONEY,
				ServiceCharge MONEY,
				OutstandingBalance MONEY,
				CashPrice MONEY,
				Deposit money ,
				Arrears MONEY,
				InstalNo INT,
				InterestRate INT,
				DateMigrated SMALLDATETIME
			)