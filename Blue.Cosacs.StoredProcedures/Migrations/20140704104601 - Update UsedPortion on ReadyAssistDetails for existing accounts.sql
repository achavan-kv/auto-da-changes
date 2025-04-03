-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here


IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID('[dbo].[fn_ReadyAssistMonths]') AND xtype in ('FN', 'IF', 'TF'))
DROP FUNCTION [dbo].[fn_ReadyAssistMonths]

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Ilyas Parker
-- Create date: 30/06/14
-- Description:	Returns the used number of months for a Ready Assist
-- =============================================

CREATE FUNCTION fn_ReadyAssistMonths
(
  @startdate AS DATE,
  @enddate   AS DATE
) RETURNS INT
AS
BEGIN
  RETURN
    DATEDIFF(month, @startdate, @enddate)
      - CASE
          WHEN DAY(@enddate) < DAY(@startdate) THEN 1
          ELSE 0
        END
      + 1;
END
GO


UPDATE ReadyAssistDetails
SET UnusedPortion =  del.transvalue - cast(isnull(dbo.fn_ReadyAssistMonths(ra.RAContractDate, col.DateDel) * cast(c.additional as decimal(11,2)),0) as decimal(11,2))	
from 
	ReadyAssistDetails ra
inner join 
	stockinfo si on ra.itemid = si.id
inner join 
	code c on c.code = si.iupc
inner join 
	delivery col on col.acctno = ra.acctno 
	and col.agrmtno = ra.agrmtno
	and col.itemid = ra.itemid
	and col.contractno = ra.contractno
	and col.delorcoll in ('C', 'R')
	and col.quantity < 0
inner join 
	delivery del on del.acctno = ra.acctno
	and del.agrmtno = ra.agrmtno
	and del.itemid = ra.itemid
	and del.contractno = ra.contractno
	and del.delorcoll = 'D'
	and del.datetrans = (select 
							max(datetrans)
						 from 
							delivery d
						  where d.acctno = del.acctno
						  and d.agrmtno = del.agrmtno
						  and d.itemid = del.itemid
						  and d.contractno = del.contractno
						  and d.delorcoll = del.delorcoll)
where c.category = 'RDYAST'
and ra.RATermLength in (12, 24, 36)


