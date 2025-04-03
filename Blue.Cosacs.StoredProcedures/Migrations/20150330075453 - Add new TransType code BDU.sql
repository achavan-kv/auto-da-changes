-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF NOT EXISTS(select * from transtype where transtypecode = 'BDU')
BEGIN

    insert into transtype(origbr, transtypecode,tccodedr, tccodecr, description, balordue, exportfilesuffix, batchtype, 
                          interfaceaccount,branchsplit, isdeposit,interfacebalancing,
                          IncludeinGFT,empeenochange,referencemandatory,referenceunique,
                          interfacesecbalancing,branchsplitbalancing,SCInterfaceAccount,SCInterfaceBalancing)

    select  null,'BDU', 0,19,'Bad Debt Unearned Finance Income','A','C','FIN',
            1301,1,0,1306,
            0,0,0,0,
            '',1,
            1301,
            1306
                 
END
GO