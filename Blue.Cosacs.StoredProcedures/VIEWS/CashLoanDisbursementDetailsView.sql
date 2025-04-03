/*
Cash Loan Disbursement Electronic Bank Transfer Sheet Print View

*/

IF EXISTS (SELECT * FROM sys.objects so
		   INNER JOIN sys.schemas ss ON so.schema_id = ss.schema_id
		   WHERE so.type = 'V'
		   AND so.NAME = 'CashLoanDisbursementDetailsView'
		   AND ss.name = 'dbo')
DROP VIEW  dbo.[CashLoanDisbursementDetailsView]
GO


CREATE VIEW  dbo.[CashLoanDisbursementDetailsView]
AS
	

	SELECT
        c.custid as CustomerId,
        c.title + ' ' + C.firstname + ' ' + C.name as CustomerName,
        ca.cusaddr1 as CustomerAddress1,
        ca.cusaddr2 as CustomerAddress2,
        ca.cusaddr3 as CustomerAddress3,
        ca.cuspocode as CustomerPostCode,
        ca.Email as CustomerEmail,
        RTRIM(LTRIM(ct.DialCode)) + ' ' + RTRIM(LTRIM(ct.telno)) as CustomerTelephoneNumber,
        cld.AcctNo as AccountNumber,
        (cld.LoanAmount - isnull(cl.AdminCharge,0)) as LoanAmount,
        b.bankname as BankName,
        cld.BankBranch,
        bat.codedescript as BankAccountType,
        cld.BankAcctNo as BankAccountNumber,
        cld.BankReferenceNo,
        cld.BankAccountName,
        cld.Notes,
        a.Id as EmployeeDisburseEmpeeno,
        a.FullName as EmployeeDisburseFullName,
        cdm.codedescript as DisbursementType,
        cld.BankTransferRefNo as BankTransferNumber,
        cld.BankTransferDate
    FROM 
        CashLoanDisbursement cld
        INNER JOIN CashLoan cl on cld.AcctNo = cl.AcctNo
        INNER JOIN Admin.[User] a on a.Id = cl.EmpeenoDisburse
        INNER JOIN Customer c on cld.CustId = c.custid
        INNER JOIN Custaddress ca on ca.custid = c.custid
        INNER JOIN Custtel ct on ct.custid = c.custid
        INNER JOIN Bank b on b.bankcode = cld.Bank
        INNER JOIN Code bat on bat.code = cld.BankAccountType
        INNER JOIN Code cdm on cld.DisbursementType = cdm.code
    WHERE
        ca.addtype = 'H'
        AND ca.datemoved IS NULL
        AND ct.tellocn = 'H'
        AND ct.datediscon IS NULL
        AND bat.category = 'BA2'
        AND cdm.category = 'CDM'
GO
