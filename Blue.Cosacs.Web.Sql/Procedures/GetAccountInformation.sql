IF EXISTS ( SELECT * FROM sysobjects WHERE NAME = 'GetAccountInformation' )
BEGIN
	DROP PROCEDURE [dbo].[GetAccountInformation]
END
GO

CREATE  PROCEDURE [dbo].[GetAccountInformation] 
	@CustId As nvarchar(20)
	--,@AccountNumber AS CHAR(12) = NULL
AS
BEGIN

	Declare @AccountNumber varchar(20)
	select top 1 @AccountNumber=acct.acctno from Custacct, acct where Custacct.acctno=acct.acctno and Custacct.custid=@CustId and accttype='R'
	order by dateacctopen desc


	Select DateBorn,maritalStat,Dependants,Nationality,sex,RFCreditLimit 
	from Customer 
	where custid= @custId
	
	-- Table 1
	--Select 
	--Occupation, isnull(mthlyincome,'') as mthlyincome, empname, presstatus, DateCurrAddress,
	--isnull(Commitments3,'') As Commitments3, isnull(Commitments2,'') As Commitments2,
	--isnull(AdditionalExpenditure1,'') As AdditionalExpenditure1, isnull(AdditionalExpenditure2,'') AS AdditionalExpenditure2, 
	--isnull(Commitments1,'') As Commitments1, isnull(Addincome,'') AS Addincome , CCardNo1, isnull(otherpmnts,'') AS otherpmnts,EmploymentStatus
	----Occupation, mthlyincome, empname, presstatus, DateCurrAddress,
	----isnull(Commitments3,''), isnull(Commitments2,''),
	----isnull(AdditionalExpenditure1,''), isnull(AdditionalExpenditure2,''), 
	----isnull(Commitments1,''), isnull(Addincome,''), CCardNo1
	--from Proposal 
	--where acctno = @AccountNumber
	
	--(select top 1 acct.acctno from Custacct, acct where Custacct.acctno=acct.acctno and Custacct.custid=@CustId
	--order by dateacctopen desc) 

	-- Table 2
	--Select payfreq,DateEmployed
	--from employment 
	--where custid = @custId

	---- Table 3
	--Select cusaddr1,addtype
	--from CustAddress 
	--where custid = @custId
	
	---- Table 4
	--Select telno,tellocn 
	--from Custtel 
	--where custid = @custId and tellocn in ('W','H')
	
	---- Table 5
	--select ps.Name,ps.tel 
	--from proposalref ps 
	--	inner join custacct ca on ca.acctno = ps.acctno 
	--	inner join acct at on at.acctno = ps.acctno 
	--where ca.custid = @custId 
	--order by at.dateacctopen desc

	--(select top 1 acct.acctno from Custacct, acct where Custacct.acctno=acct.acctno and Custacct.custid=@CustId
	--order by dateacctopen desc) 

	-- Table 6
	--IF(ISNULL(@AccountNumber, '') <> '')
	BEGIN
		SELECT FolderPath, [FileName], FolderPath + [FileName] AS FullPath,
			CASE WHEN CHARINDEX('Address', [FileName]) > 0 THEN 'Address' ELSE 
				CASE WHEN CHARINDEX('Income', [FileName]) > 0 THEN 'Income' ELSE 
					CASE WHEN CHARINDEX('IdProof1', [FileName]) > 0 THEN 'IdProof1' ELSE 
					CASE WHEN CHARINDEX('IdProof2', [FileName]) > 0 THEN 'IdProof2' ELSE 
					'OTHER' END 
					END
				END 
			END AS ProofType,
			 AccountNumber
		FROM CustCreditDocuments 
		WHERE CustId = @custId and AccountNumber = @AccountNumber
		--(select top 1 acct.acctno from Custacct, acct where Custacct.acctno=acct.acctno and Custacct.custid=@CustId
	--order by dateacctopen desc)  -- AND (AccountNumber = @AccountNumber OR @AccountNumber IS NULL)
		ORDER BY CREATEDON DESC
	END

END
