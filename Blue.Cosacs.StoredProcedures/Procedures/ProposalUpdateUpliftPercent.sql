IF EXISTS (SELECT * FROM sysobjects WHERE NAME ='ProposalUpdateUpliftPercent')
DROP PROCEDURE ProposalUpdateUpliftPercent
GO 
CREATE PROCEDURE ProposalUpdateUpliftPercent @acctno CHAR(12), @custid VARCHAR(20), @upliftpercent INT 
AS 
UPDATE proposal SET CreditPercentUplift = @upliftpercent  
WHERE acctno = @acctno AND custid= @custid 
GO 