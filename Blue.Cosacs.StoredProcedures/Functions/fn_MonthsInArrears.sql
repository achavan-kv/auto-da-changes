SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

if exists (select * from information_schema.columns where table_name = 'Acct' and column_name = 'MonthsInArrears')
begin
   alter table acct DROP column MonthsInArrears 
end
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[fn_MonthsInArrears]') AND xtype in (N'FN', N'IF', N'TF'))
DROP FUNCTION [dbo].[fn_MonthsInArrears]
go


-- =======================================================================
-- Author:		Ilyas Parker
-- Create date: 28/10/09
-- Description:	Calculates the number of months an account is in arrears.
-- =======================================================================

CREATE FUNCTION fn_MonthsInArrears
(
	@AcctNo varchar(12)
)
RETURNS float
AS 
BEGIN
	
	
	RETURN (SELECT CASE WHEN ISNULL(i.instalamount,0) = 0 THEN 0
				ELSE CONVERT(FLOAT,ISNULL(a.arrears/i.instalamount, 0)) END 
				FROM acct a LEFT JOIN instalplan i
				ON a.acctno = i.acctno
				WHERE a.acctno = @AcctNo)
			

END

GO

if not exists (select * from information_schema.columns where table_name = 'Acct' and column_name = 'MonthsInArrears')
begin
   alter table acct add MonthsInArrears AS ([dbo].[fn_MonthsInArrears](acctno))
end
GO