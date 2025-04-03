SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[sploadclearance]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[sploadclearance]
GO



CREATE procedure sploadclearance
@BranchRestriction varchar(4),
@includecash integer,
@includeHP integer,
@holdflags varchar (4)
as
begin
declare @status integer, @query_text varchar (256)
set nocount on
    set @query_text =N'initial select '
     select
      A.AcctNo, A.AcctType, B.AgrmtNo,A.termstype,
      A.AgrmtTotal, A.BranchNo, B.CODFlag,
      A.DateAcctOpen, B.DateAgrmt, B.DateDepChqClr,
      B.Deposit, B.HoldProp, C.InstalPreDel,
      B.empeenosale, convert (varchar (20), '') as salesperson  ,convert ( smalldatetime, null) as datechange,
      convert (datetime, null) as dateprop,convert (integer, 0) as  empeenochange,convert ( money, 0) as alreadypaid,
      convert (tinyint,0) as chequeos,convert (tinyint, 0) as paymentos,d.custid, convert (varchar (200),'') as dbhandle,
      convert (tinyint, 0) as hasstring, convert (varchar (200), '') as propnotes,convert (varchar (1), '') as propresult,
      convert (varchar (800),'') as notes
     into   #temp_acct1
     from delauthorise l,ACCT A, AGREEMENT B, TermsType C,custacct d

     where A.AcctNo = l.AcctNo
     and b.acctno =l.acctno 
     and d.acctno =a.acctno
     and d.hldorjnt = 'H'
     and C.TermsType = isnull(a.TermsType,00)
     and l.AcctNo like   @BranchRestriction
     and A.AcctType != 'S'
     and A.CurrStatus not in ('0','U','S')
     and B.HoldProp = 'Y'
     and A.AgrmtTotal > 0
     set @status =@@error
     if @status = 0
     begin
        set @query_text =N'remove HP '
        if @includehp= 0
         	delete from  #temp_acct1
        	where acctno like '___0%'
        set @status =@@error
     end
     set @status =@@error
     if @status = 0
     begin
     	set @query_text =N'remove cash '
        if @includecash = 0
            delete from  #temp_acct1
        	where acctno like '___4%'
        set @status =@@error
     set @status =@@error
     end
     if @status = 0
     begin
     	set @query_text =N'removing non flags'
     	execute @status =sploadclearance_remnonflags @holdflags=@holdflags
     end
     if @status = 0
     begin
     	set @query_text =N'creating index '
    	create clustered index  tindex on  #temp_acct1( acctno )
        set @status =@@error

     set @status =@@error
     end
     if @status = 0
     begin
     	set @query_text =N'updating salesperson name '

    	update  #temp_acct1  set salesperson = c.empeename
        from courtsperson c, #temp_acct1 t
        where t.empeenosale = c.empeeno
        set @status =@@error

     end
     if @status = 0
     begin
        set @query_text =N'updating deposit for accounts with instalment before delivery'
        update #temp_acct1 set deposit = instalplan.instalamount
        from instalplan
        where instalpredel= 'Y'
        and instalplan.acctno =#temp_acct1.acctno
        set @status =@@error
     end

     if @status = 0
     begin
     	set @query_text =N'update date last updated '
     	execute @status =sploadclearance_updatedate
     end
     if @status = 0
     begin
     	set @query_text =N'updating payment amounts '
      execute @status = sploadclearance_updatepayment
     end

     if @status = 0
     begin
     	set @query_text =N'getting installed amounts '
        select t.*,i.instalamount into #temp_hp1
        from instalplan i,#temp_acct1 t
        where i.acctno =t.acctno
        and accttype !=N'C'
        set @status =@@error
     end



     if @status = 0
     begin
     	set @query_text =N'getting referral details '
        select t.acctno, r.reflresult
        into     #referral
        from    referral r,
                proposal p,
                #temp_hp1  t
        where   p.acctno = t.acctno
        and     p.custid = r.custid
        and     p.dateprop = r.dateprop
        and     p.propresult = 'D'
        and     t.accttype != 'C'
        set @status =@@error
     end

     if @status = 0
     begin
      	set @query_text =N'updating from proposal table'
        update #temp_hp1
        set hasstring =p.hasstring,
        dbhandle=left (isnull(p.propnotes, ''), 200),
        notes =left (isnull (p.notes, ''), 800),
        propresult =isnull (p.propresult, ''),
        dateprop=isnull(p.dateprop,'')
        from    proposal p
        where     p.custid = #temp_hp1.custid
        and     p.acctno = #temp_hp1.acctno
        set @status =@@error
     end
     if @status = 0
     begin
      	set @query_text =N'updating from proposal table2'
        create clustered index  tindex on  #temp_hp1( acctno )
        set @status =@@error
        delete from #temp_hp1 where
             (propresult   in ('X','R',''))
            or  (propresult  = 'D' and
                acctno in (select acctno from
                #referral
                where reflresult != 'A'))
        delete from #temp_hp1 where
        propresult  = 'D' and
        acctno not in (select acctno from #referral)
        set @status =@@error
    end
     if @status = 0
     begin

        select
          AcctNo,  AcctType, termstype,  AgrmtNo,
           AgrmtTotal,  BranchNo,  CODFlag,
            DateAcctOpen,  DateAgrmt,  DateDepChqClr,
           Deposit,  HoldProp, InstalPreDel,custid,
           salesperson  ,datechange,
          dateprop, empeenochange,  alreadypaid,
          chequeos, paymentos, hasstring ,
        dbhandle,notes
         from
          #temp_hp1 where accttype not in ('C', 'S')
          union
         select
           AcctNo,  AcctType,termstype,  AgrmtNo,
           AgrmtTotal,  BranchNo,  CODFlag,
          DateAcctOpen,  DateAgrmt,  DateDepChqClr,
           Deposit,  HoldProp, InstalPreDel,custid,
          salesperson  ,datechange,
          dateprop, empeenochange, alreadypaid,
          chequeos, paymentos, hasstring ,
          dbhandle,notes from
          #temp_acct1 where accttype  in ('C', 'S')
        order by acctno
    end



        return @status

end

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

