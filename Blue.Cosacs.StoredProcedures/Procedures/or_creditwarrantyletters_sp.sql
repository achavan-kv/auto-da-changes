SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[or_creditwarrantyletters_sp]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[or_creditwarrantyletters_sp]
GO

CREATE procedure or_creditwarrantyletters_sp @letterdate DateTime, @return int=0 OUTPUT

as 

SET @return = 0

begin
/* procedure will retrieve details of all credit warranties which have not yet been paid
and send them one of two letters*/

   declare @seed integer,@hobranchno smallint,@datedel DateTime
   set @datedel =getdate()
   select @seed =hirefno from branch , country where branchno = country.hobranchno
   select @hobranchno = hobranchno from country
   select acct.acctno,'WCR' as lettercode,max(delivery.datedel) as datedel  into #temporary_letter
   from delivery ,acct, country
   where delivery.acctno =acct.acctno    
   and acct.currstatus !='S' and delivery.contractno !=''
   and acct.termstype='WC'
   and acct.outstbal > 0
   --and dateadd (day,country.creditwarrantyreminder+1,datedel) <@letterdate
   and dateadd (day,country.creditwarrantyreminder,datedel) <@letterdate           -- #15186    
   group by acct.acctno

	create index ix_temporary_letter1a3 on #temporary_letter (acctno)            	

   -- for accounts which have already had a reminder letter and are nearing expiry period send them a grace letter
   update #temporary_letter set lettercode= 'WGL'
   from country 
   where dateadd (day,country.creditwarrantyDays ,datedel) <@letterdate   
   and exists ( select * from  letter where letter.acctno = #temporary_letter.acctno and letter.lettercode='WCR')
   and country.creditwarrantygrace !=0

   -- remove accounts with existing letters
	delete from #temporary_letter where exists (select * from 
   letter where letter.acctno = #temporary_letter.acctno and letter.lettercode = #temporary_letter.lettercode)
   
   -- now create the letter
	insert into letter (acctno, dateacctlttr, datedue, lettercode)
   select acctno,@letterdate, dateadd (day,7,@letterdate ), lettercode
   from #temporary_letter
end

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

