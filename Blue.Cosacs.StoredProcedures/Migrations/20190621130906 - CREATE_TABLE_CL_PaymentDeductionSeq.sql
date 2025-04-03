-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
/*
	CR : CLA Outstanding Balance 
	Author : Rahul D
	Date : 21/06/2019
	Details : Table to maintain deduction sequence of payment
*/

IF NOT EXISTS (SELECT 'A' FROM SYS.TABLES WHERE NAME = 'CL_PaymentDeductionSeq')
BEGIN
	CREATE TABLE CL_PaymentDeductionSeq
	(
		id int primary key,
		SequenceName varchar(max)
	)

	INSERT INTO CL_PaymentDeductionSeq (SequenceName, id) VALUES ('Amortized Interest', 1)
	INSERT INTO CL_PaymentDeductionSeq (SequenceName, id) VALUES ('Admin Fees', 2)
	INSERT INTO CL_PaymentDeductionSeq (SequenceName, id) VALUES ('Principal',3)
	INSERT INTO CL_PaymentDeductionSeq (SequenceName, id) VALUES ('Penalty Int',4)

END