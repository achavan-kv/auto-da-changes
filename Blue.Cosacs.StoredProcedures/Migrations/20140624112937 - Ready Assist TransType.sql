-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here


IF NOT EXISTS(select * from transtype where transtypecode = 'RAR')
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
					  branchsplitbalancing
					  )
select	null,
		'RAR',
	    19,
	    0,
	    'Ready Assist Recovery',
	    'A',
		'C',
		'FIN',
		'',
		'',
		1,
		0,
		'',
		0,
		0,
		0,
		0,
		'',
		1
		
END