
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID('[dbo].[view_CollCommns_MaxArrears]') AND OBJECTPROPERTY(id, 'IsView') = 1)
DROP VIEW [dbo].[view_CollCommns_MaxArrears]
GO



CREATE VIEW view_CollCommns_MaxArrears


/***********************************************************************************************************
--
-- Project      : CoSACS .NET
-- File Name    : view_CollCommns_MaxArrears.sql
-- File Type    : MSSQL Server Script
-- Description  : Selects the max arrears for an account for Collection Commissions
-- Author       : Ilyas Parker
-- Date         : 15 June 2010
--
--  
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 15/06/10	 IP  Creation.

************************************************************************************************************/

AS 

 select cr.ID, cr.RuleName, ca.EmpeeNo, ca.EmpeeType, ca.AcctNo, MAX(arrearsbefore) as maxarrears,count(*) as Norows
	 from CollectionCommnAccts ca, CollectionCommnRules cr
	 where((cr.NoOfDaysSinceAction = 0 or (cr.NoOfDaysSinceAction>0 and cr.PcentArrearsColl>0 and ca.PaymentDaysSinceAction <= cr.NoOfDaysSinceAction))
		and (cr.MinBal = 0 or (cr.MinBal > 0 and ca.BalanceBefore >= cr.MinBal))
		and (cr.MaxBal = 0 or (cr.MaxBal > 0 and ca.BalanceBefore <= cr.MaxBal))
		and (cr.MinValColl = 0 or (cr.MinValColl > 0 and ca.PaymentValue >= cr.MinValColl))
		and (cr.MaxValColl = 0 or (cr.MaxValColl > 0 and ca.PaymentValue <= cr.MaxValColl))
		and (cr.MinMnthsArrears = 0 or (cr.MinMnthsArrears > 0 and ca.MonthsInArrearsBefore >= cr.MinMnthsArrears))
		and (cr.MaxMnthsArrears = 0 or (cr.MaxMnthsArrears > 0 and ca.MonthsInArrearsBefore <= cr.MaxMnthsArrears)))
		and ca.RunDate = (select MAX(ca1.RunDate) from CollectionCommnAccts ca1)
	 group by cr.ID, cr.RuleName, ca.EmpeeNo, ca.EmpeeType, ca.AcctNo

GO

GRANT SELECT ON view_CollCommns_MaxArrears TO PUBLIC

GO