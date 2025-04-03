if exists (select * from dbo.sysobjects where id = object_id('[dbo].[dn_ChargesdataGetbyAcctnoSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[dn_ChargesdataGetbyAcctnoSP]
GO

create procedure dbo.dn_ChargesdataGetbyAcctnoSP
@acctno char(12),
@return int OUTPUT
 as 


  select acctno,
       arrears,
       convert(decimal(7,7),mpr) as mpr,
       datefinish,
       c.runno,
       datenextdue,
       intchargesdue,
       intchargescumul,
       intchargesapplied
  from chargesdata c, interfacecontrol i
  where c.runno = i.runno and i.interface='CHARGES' 
  and i.datestart >dateadd(month,-11,getdate()) --this is because letters and charges weeks are reused annually from 1-52 so want most recent
  and c.acctno = @acctno

     
  set @return = @@error

go

if not exists(select * from sysindexes where name ='ix_chargesdata_acctno')
   create index ix_chargesdata_acctno on chargesdata(acctno)
go
