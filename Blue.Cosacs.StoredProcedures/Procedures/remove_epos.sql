SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[remove_epos]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[remove_epos]
GO


/* Procedure will sum up EPOS credit transactions that are older than 6 months
   which will remove performance problems related to EPOS credit transactions */

create procedure remove_epos as

   declare @date6months datetime
   set @date6months = DATEADD(mm, -6, getdate())

   declare @status integer
   set @status = 0

   declare @message varchar(256)
   declare @fintrans_start_total money
   declare @fintrans_end_total money

   set nocount on

   begin tran

      set @message = 'Error - Calculating start total fintrans transactions for P&T accounts'
      set @fintrans_start_total = (select sum(transvalue) from fintrans 
                                  where datetrans < @date6months
                                  and ftnotes in ('EPOS', 'EPOR')
                                  and acctno in (select custacct.acctno from custacct 
                                                 where custid like 'PAID%'
                                                 and hldorjnt = 'H') )
      set @status =@@error
      if (@fintrans_start_total is null)
         set @status = 1
         set @message = 'Warning - No rows to archive'

      if @status = 0
         begin
            set @message = 'Error - Inserting totals into a temporary table'
            select acctno, transtypecode, sum(transvalue) as transvalue,
                   max(transrefno) as transrefno
            into #PT_archive_totals
            from fintrans
            where datetrans < @date6months
            and ftnotes in ('EPOS', 'EPOR')
            and acctno in (select custacct.acctno from custacct 
                           where custid like 'PAID%'
                           and hldorjnt = 'H')
            group by acctno, transtypecode
            set @status =@@error
         end

      if @status = 0
         begin
            set @message = 'Error - Inserting rows into archive table'
            insert into fintrans_epos
                   (acctno, bankacctno, bankcode, branchno, chequeno, datetrans, empeeno, ftnotes, 
                    origbr, paymethod, runno, source, transprinted, transrefno, transtypecode,
                    transupdated, transvalue)
            select acctno, bankacctno, bankcode, branchno, chequeno, datetrans, empeeno, ftnotes,
                   origbr, paymethod, runno, source, transprinted, transrefno, transtypecode, 
                   transupdated, transvalue
            from fintrans
            where datetrans < @date6months
            and ftnotes in ('EPOS', 'EPOR')
            and acctno in (select custacct.acctno from custacct 
                           where custid like 'PAID%'
                           and hldorjnt = 'H')
            set @status =@@error
         end

      if @status = 0
         begin
            set @message = 'Error - Deleting rows from fintrans table'
            delete from fintrans
            where datetrans < @date6months
            and ftnotes in ('EPOS', 'EPOR')
            and acctno in (select custacct.acctno from custacct 
                           where custid like 'PAID%'
                           and hldorjnt = 'H')
            set @status =@@error
         end

      if @status = 0
         begin
            set @message = 'Error - Inserting totals into fintrans table'
            insert into fintrans
                   (acctno, bankacctno, bankcode, branchno, chequeno, datetrans, empeeno, ftnotes, 
                    origbr, paymethod, runno, source, transprinted, transrefno, transtypecode,
                    transupdated, transvalue)
            select acctno, '', '', convert(integer,left(acctno,3)), '', @date6months, 99999, 
                   'ETOT', 0, 0, -2, 'CoSACS', 'Y', transrefno, transtypecode, 'Y', transvalue
            from #PT_archive_totals
            set @status =@@error
         end

      if @status = 0
         begin
            set @message = 'Error - Calculating end total fintrans transactions for P&T accounts'
            set @fintrans_end_total = (select sum(transvalue) from fintrans 
                                      where datetrans < @date6months
                                      and ftnotes in ('EPOS', 'EPOR')
                                      and acctno in (select custacct.acctno from custacct 
                                                     where custid like 'PAID%'
                                                     and hldorjnt = 'H') )
            set @status =@@error
         end

      if @status = 0
         begin
            set @message = 'Error - Checking start and end totals match'
            if (@fintrans_start_total <> @fintrans_end_total)
               set @status = 1
         end

      if @status = 0
         commit transaction
      else
         begin
            rollback transaction
            select @message
            insert into interfaceerror (Interface, runno, errordate, errortext, severity)
            values ('EPOS', -2, getdate(), @message, 'E')
         end

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

