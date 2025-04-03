if exists(select * FROM dbo.sysobjects 
               WHERE ID = object_id('dbo.vw_Summary1_BA') AND OBJECTPROPERTY(id, 'IsView') = 1)
	drop view dbo.vw_Summary1_BA
GO 
-- Bailaction
create view dbo.vw_Summary1_BA
as
	Select Distinct bailaction.acctno,max(bailaction.dateadded) AS 'datespaadded'
		From bailaction
	Where code = 'SPA'

	Group by acctno

go