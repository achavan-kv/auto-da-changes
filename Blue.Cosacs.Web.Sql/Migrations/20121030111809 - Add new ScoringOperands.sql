-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
-- Related to issue: #11535 & #11536

DECLARE @operandID INT

SET @operandID = (SELECT MAX(OperandID) FROM dbo.ScoringOperand)

SELECT @operandID

IF NOT EXISTS(SELECT * FROM ScoringOperand WHERE OperandName = 'Agreement Total Account')
BEGIN
	INSERT INTO dbo.ScoringOperand
	SELECT @operandID + 1, 'Agreement Total Account', 'float', 0, ''
END

IF NOT EXISTS(SELECT * FROM ScoringOperand WHERE OperandName = 'Outstanding Balance of Accounts')
BEGIN
	INSERT INTO dbo.ScoringOperand
	SELECT @operandID + 2, 'Outstanding Balance of Accounts', 'float', 0, ''
END