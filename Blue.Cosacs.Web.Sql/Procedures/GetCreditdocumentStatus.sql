
IF EXISTS (SELECT * FROM sysobjects 
   WHERE NAME = 'GetCreditdocumentStatus'
   )
BEGIN
DROP PROCEDURE [dbo].[GetCreditdocumentStatus]
END
GO



CREATE PROCEDURE [dbo].[GetCreditdocumentStatus]

AS
BEGIN

SELECT 
		Distinct prop.custid AS custId
		,prop.acctno As acctNo
		, CASE 
			WHEN prop.propresult<>'X' and Sig.isApproved = 0 THEN 
				CASE 
					WHEN prop.propresult='A' THEN 'APPROVED'
					WHEN prop.propresult='D' 
					--OR prop.propresult='R' 
					THEN'DENIED' 
				end 
			WHEN  Sig.isActive = 0 and Sig.isApproved =1 THEN
				'ACTIVE'
		  END AS 'DocumentStatus'
		, CASE 
			WHEN Sig.isApproved = 0 THEN 
				(select email from custaddress where custid=sig.custid and addtype='H') 
			ELSE '' 
		  END As email
	FROM proposal As prop 
		Inner join SignatureStatus As Sig on Sig.acctno=Prop.acctno and sig.custid=Prop.custid
		--INNER JOIN CustCreditDocuments AS ccd  ON sig.acctno = ccd.accountnumber and sig.custid = ccd.custid
	WHERE 
		--(
		(prop.propresult not in('X','R') and Sig.isApproved = 0) 
		--OR 
		--(Sig.isActive = 0 and Sig.isApproved =1 AND ccd.FileName like 'SignedContract%')
		--) 

	----Update Active status
	--UPDATE  ss 
	--SET isActive = 1, activeDate=GETDATE() 
	--FROM SignatureStatus AS ss INNER JOIN CustCreditDocuments AS ccd  ON ss.acctno = ccd.accountnumber and ss.custid = ccd.custid
	--where ss.isActive = 0 and ss.isApproved =1 AND ccd.FileName like 'SignedContract%'

	--Update Approve status
	UPDATE ss set isApproved = 1 ,approvedDate=getdate() from SignatureStatus As ss
	Inner join proposal As prop on Prop.acctno=ss.acctNo and ss.custid=Prop.custid
	where ss.isApproved = 0 and prop.propresult not in('X','R')
	

End
