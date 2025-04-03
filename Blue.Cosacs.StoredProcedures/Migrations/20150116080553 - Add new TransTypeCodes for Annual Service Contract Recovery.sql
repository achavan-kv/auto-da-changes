-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF NOT EXISTS(select * from transtype where transtypecode = 'ASR')
BEGIN

    insert into transtype(origbr, transtypecode, tccodedr, tccodecr, description, balordue, exportfilesuffix, batchtype, interfaceaccount, interfacesecaccount, branchsplit, 
                            isdeposit, interfacebalancing, IncludeinGFT, empeenochange, referencemandatory, referenceunique, interfacesecbalancing, branchsplitbalancing, 
                            SCInterfaceAccount, SCInterfaceBalancing)
    select NULL, 'ASR', 32, 11, 'Annual Service Contract Recovery Credit', 'B', 'C', 'FIN', 1301, '', 1,
           0, 5111, 0, 0, 0, 0, '', 1,
           '', ''

END
GO

IF NOT EXISTS(select * from transtype where transtypecode = 'ASC')
BEGIN

    insert into transtype(origbr, transtypecode, tccodedr, tccodecr, description, balordue, exportfilesuffix, batchtype, interfaceaccount, interfacesecaccount, branchsplit, 
                            isdeposit, interfacebalancing, IncludeinGFT, empeenochange, referencemandatory, referenceunique, interfacesecbalancing, branchsplitbalancing, 
                            SCInterfaceAccount, SCInterfaceBalancing)
    select NULL, 'ASC', 32, 11, 'Annual Service Contract Recovery Cash', 'B', 'C', 'FIN', 1301, '', 1,
           0, 5211, 0, 0, 0, 0, '', 1,
           '', ''

END
GO

IF NOT EXISTS(select * from transtype where transtypecode = 'BOR')
BEGIN

	insert into TransType(origbr, 
					  transtypecode,
					  tccodedr,
					  tccodecr,
					  [description],
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
					  )
select	null,
		'BOR',
	    32,
	    11,
	    'Credit Sale of Annual Service',
	    'B',
		'C',
		'FIN',
		'1301',
		'',
		1,
		0,
		'5111',
		0,
		0,
		0,
		0,
		'',
		1,
		'',
		''	
END
GO

IF NOT EXISTS(select * from transtype where transtypecode = 'BCA')
BEGIN

	insert into TransType(origbr, 
					  transtypecode,
					  tccodedr,
					  tccodecr,
					  [description],
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
					  )
select	null,
		'BCA',
	    32,
	    11,
	    'Cash Sale of Annual Service',
	    'B',
		'C',
		'FIN',
		'1301',
		'',
		1,
		0,
		'5211',
		0,
		0,
		0,
		0,
		'',
		1,
		'',
		''	
END
GO

IF NOT EXISTS(select * from transtype where transtypecode = 'BAS')
BEGIN

	insert into TransType(origbr, 
					  transtypecode,
					  tccodedr,
					  tccodecr,
					  [description],
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
					  )
select	null,
		'BAS',
	    32,
	    11,
	    'Service Charge on Credit Annual Service',
	    'B',
		'C',
		'FIN',
		'1301',
		'',
		1,
		0,
		'5390',
		0,
		0,
		0,
		0,
		'',
		1,
		'',
		''	
END
GO

IF NOT EXISTS(select * from transtype where transtypecode = 'CAA')
BEGIN

	insert into TransType(origbr, 
					  transtypecode,
					  tccodedr,
					  tccodecr,
					  [description],
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
					  )
select	null,
		'CAA',
	    32,
	    11,
	    'Cost of Sale for Annual Service',
	    'B',
		'C',
		'FIN',
		'6011',
		'',
		1,
		0,
		'2915',
		0,
		0,
		0,
		0,
		'',
		1,
		'',
		''	
END
GO

IF NOT EXISTS(select * from transtype where transtypecode = 'CNA')
BEGIN

	insert into TransType(origbr, 
					  transtypecode,
					  tccodedr,
					  tccodecr,
					  [description],
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
					  )
select	null,
		'CNA',
	    32,
	    11,
	    'Cancellation of Annual Service Credit',
	    'B',
		'C',
		'FIN',
		'1301',
		'',
		1,
		0,
		'5111',
		0,
		0,
		0,
		0,
		'',
		1,
		'',
		''	
END
GO

IF NOT EXISTS(select * from transtype where transtypecode = 'CAC')
BEGIN

	insert into TransType(origbr, 
					  transtypecode,
					  tccodedr,
					  tccodecr,
					  [description],
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
					  )
select	null,
		'CAC',
	    32,
	    11,
	    'Cancellation of Annual Service Cash',
	    'B',
		'C',
		'FIN',
		'1301',
		'',
		1,
		0,
		'5211',
		0,
		0,
		0,
		0,
		'',
		1,
		'',
		''	
END
GO