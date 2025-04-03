

IF EXISTS (SELECT * FROM sysobjects WHERE NAME = 'EMA_GetEmailContracts')
BEGIN
DROP PROCEDURE [dbo].[EMA_GetEmailContracts]
END
GO
 
CREATE PROCEDURE [dbo].[EMA_GetEmailContracts]
	AS
BEGIN
		SELECT DISTINCT
				T0.custId AS 'custId'
			,T0.acctNo AS 'accountNumber'
			,T1.Email AS 'emailId'
			,ccd.Custid+'/'+ ccd.AccountNumber+'/'+ ccd.FileName AS 'filePath'
			,'' AS 'mailBody'
			,'Third Party Contract' AS 'mailSubject'
			,cust.firstname AS 'FirstName'
			,cust.name AS 'LastName'
		FROM [dbo].[CustTPContract] T0
		INNER JOIN custaddress T1 ON T0.custId=T1.custId AND T1.addtype='H'
		INNER JOIN CustCreditDocuments AS ccd  ON T0.acctno = ccd.accountnumber and T0.custid = ccd.custid
		INNER JOIN Customer cust ON T0.custId=cust.custid
		WHERE T0.isTPContractSend IS NULL 
		AND ccd.FileName like 'SignedContract%'

		UPDATE CustTPContract SET IsTPContractSend=0 ,UpdateDate=getdate()
		FROM [dbo].[CustTPContract] T0
		INNER JOIN custaddress T1 ON T0.custId=T1.custId AND T1.addtype='H'
		INNER JOIN CustCreditDocuments AS ccd  ON T0.acctno = ccd.accountnumber and T0.custid = ccd.custid
		INNER JOIN Customer cust ON T0.custId=cust.custid
		WHERE T0.isTPContractSend IS NULL 
		AND ccd.FileName like 'SignedContract%'
END	 
GO

