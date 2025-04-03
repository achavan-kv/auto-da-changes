-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here


if exists(select * from transtype where transtypecode = 'SDW' and [description] = 'Service Debt WO')
	delete from transtype where transtypecode = 'SDW' and [description] = 'Service Debt WO'


if not exists(select * from transtype where transtypecode = 'WOS' and [description] = 'Writeoff Service')
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
			'WOS',
			19,
			0,
			'Writeoff Service',
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
			
