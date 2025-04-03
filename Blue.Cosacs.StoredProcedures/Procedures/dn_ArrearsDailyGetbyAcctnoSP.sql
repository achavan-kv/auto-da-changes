if exists (select * from dbo.sysobjects where id = object_id('[dbo].[dn_ArrearsDailyGetbyAcctnoSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[dn_ArrearsDailyGetbyAcctnoSP]
GO

create procedure dbo.dn_ArrearsDailyGetbyAcctnoSP
@acctno char(12),
@return int OUTPUT
 as 
set @return = 0
SELECT arrears,
       isnull(convert(varchar,datefrom,103),' ') as datefrom,
       isnull(convert(varchar,dateto,103),' ') as dateto
FROM ARREARSDAILY
where acctno = @acctno
set @return = @@error
go