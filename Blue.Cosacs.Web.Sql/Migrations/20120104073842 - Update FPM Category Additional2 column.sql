-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

--Additional2 column on category FPM is used to determine 1) if a shortage/overage is allowed 2) to remove a paymethod from the CashierDeposits screen.
-- Combinations:00 - AShortage/Overage disallowed. Paymethod not displayed in Cashier Deposits screen. 
--				01 - Shortage/Overage disallowed. Paymethod displayed in Deposits screen.	
--				10 - Shortage/Overage allowed. Paymethod not displayed in Cashier Deposits screen. 
--				11 - Shortage/Overage allowed. Paymethod displayed in Cashier Deposits screen.

IF EXISTS(select * from code where category = 'FPM')
BEGIN
	UPDATE code
	SET Additional2 = '11'
	WHERE category = 'FPM'
END


IF EXISTS(select * from codecat where category = 'FPM')
BEGIN
	UPDATE codecat
	SET Additional2HeaderText = 'Cashier Ref',
	ToolTipText = 'Display Screen entered as: P, B, C or left empty.P = Payment, C = Cash & Go, entering B or leaving empty, Pay Method will appear in both screens. Cashier Ref entered as 2 digits e.g. 00. First to allow/disallow shortage/overage 0 = allow. Second display or not in Cashier Deposits. 0 = not display'
	WHERE category = 'FPM'
END

