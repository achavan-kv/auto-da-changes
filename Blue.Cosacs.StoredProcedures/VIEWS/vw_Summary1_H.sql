if exists(select * FROM dbo.sysobjects 
               WHERE ID = object_id('dbo.vw_Summary1_H') AND OBJECTPROPERTY(id, 'IsView') = 1)
	drop view dbo.vw_Summary1_H
GO 
-- Rebates
create view dbo.vw_Summary1_H	WITH SCHEMABINDING 
as
		Select acctno,r.monthsarrears as rebatemonthsarrears,r.rebate,r.rebatewithin12mths,r.rebateafter12mths	--,ra.asatdate
		From dbo.rebates r	--, dbo.rebates_asat ra

go