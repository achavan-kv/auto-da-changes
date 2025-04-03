-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here


if not exists(select * from Transtype where TransTypeCode='CLD')
BEGIN
	Insert into dbo.transtype (
		origbr,
		transtypecode,
		tccodedr,
		tccodecr,
		description,
		balordue,
		exportfilesuffix,
		batchtype,
		interfaceaccount,
		interfacesecaccount,
		branchsplit,
		isdeposit,
		interfacebalancing,
		IncludeinGFT,
		empeenochange,
		referencemandatory,
		referenceunique,
		interfacesecbalancing,
		branchsplitbalancing
	) VALUES ( 
		/* origbr - smallint */ 0,
		/* transtypecode - varchar(3) */ 'CLD',
		/* tccodedr - int */ 32,
		/* tccodecr - int */ 11,
		/* description - nvarchar(40) */ N'Cash Loan Disbursement',
		/* balordue - char(1) */ 'B',
		/* exportfilesuffix - char(1) */ 'C',
		/* batchtype - varchar(3) */ 'FIN',
		/* interfaceaccount - varchar(10) */ '',
		/* interfacesecaccount - varchar(10) */ '',
		/* branchsplit - smallint */ 1,
		/* isdeposit - smallint */ 0,
		/* interfacebalancing - varchar(10) */ '',
		/* IncludeinGFT - smallint */ 0,
		/* empeenochange - int */ 0,
		/* referencemandatory - smallint */ 0,
		/* referenceunique - smallint */ 0,
		/* interfacesecbalancing - varchar(10) */ '',
		/* branchsplitbalancing - smallint */ 1 ) 
END
