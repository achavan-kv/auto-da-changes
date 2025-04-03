-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

if not exists (select * from transType where transtypecode='COS')
BEGIN
	insert into dbo.transtype (
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
		branchsplitbalancing,
		SCInterfaceAccount,
		SCInterfaceBalancing
	) VALUES ( 
		/* origbr - smallint */ 0,
		/* transtypecode - varchar(3) */ 'COS',
		/* tccodedr - int */ 0,
		/* tccodecr - int */ 0,
		/* description - nvarchar(40) */ N'Broker Cost of Sale',
		/* balordue - char(1) */ 'B',
		/* exportfilesuffix - char(1) */ 'C',
		/* batchtype - varchar(3) */ 'FIN',
		/* interfaceaccount - varchar(10) */ '6000',
		/* interfacesecaccount - varchar(10) */ '6000',
		/* branchsplit - smallint */ 1,
		/* isdeposit - smallint */ 0,
		/* interfacebalancing - varchar(10) */ '2915',
		/* IncludeinGFT - smallint */ 0,
		/* empeenochange - int */ 0,
		/* referencemandatory - smallint */ 0,
		/* referenceunique - smallint */ 0,
		/* interfacesecbalancing - varchar(10) */ '2915',
		/* branchsplitbalancing - smallint */ 1,
		/* SCInterfaceAccount - varchar(10) */ '6000',
		/* SCInterfaceBalancing - varchar(10) */ '2915' ) 
	
	
	
	 
END
