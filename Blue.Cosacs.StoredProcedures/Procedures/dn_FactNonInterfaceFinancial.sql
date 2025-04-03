SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[dn_FactNonInterfaceFinancial]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[dn_FactNonInterfaceFinancial]
GO


CREATE procedure  dn_FactNonInterfaceFinancial 
-- ================================================
-- Project      : CoSACS .NET
-- File Name    : dn_FactNonInterfaceFinancial.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Fact Non Financial Interface
-- Author       : ??
-- Date         : ??
--
--
-- 
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 05/10/12  jec #10138 - LW75030 - SUCR - Cash Loan 
-- ================================================
as
declare  @status integer,@errortext varchar(10),@statustext  varchar (128)
set @status= 0
   if @status = 0      
   begin
      set @statustext = 'loading securitised income'      
      insert into noninterface_financial(runno,transvalue, transtypecode)
   	select 0,sum(transvalue), f.transtypecode
   	from fintrans f,acct a, transtype t, branch b
   	where a.acctno = f.acctno           
   	and f.runno= 0
   	and a.securitised ='Y'
   	and f.transtypecode not in ('DEL','GRT','REB','ADD','INT','ADM','CLD')			-- #10138
   	and T.transtypecode =F.transtypecode                            
   	and t.interfacesecaccount =''
   	and left(a.acctno, 3) =b.branchno
   	group by t.transtypecode,t.interfacesecaccount,f.transtypecode
		set @status =@@error
	end 
	
   if @status = 0      
   begin                                                  
      set @statustext = 'loading non-securitised transactions-exclude delivery and interest charges not branch split'      
      insert into noninterface_financial(runno,transvalue, transtypecode)
      select 0, sum (transvalue),      f.transtypecode  	   
 from fintrans f,acct a, transtype t, branch b
   	where a.acctno = f.acctno           
   	and f.runno= 0
   	and a.securitised !='Y'
	   and f.transtypecode not in ('DEL','GRT','REB','ADD','INT','ADM','CLD')			-- #10138
   	and T.transtypecode =F.transtypecode 
   	and t.interfaceaccount =''       
   	and left (a.acctno, 3) =b.branchno
     	group by t.transtypecode,t.interfaceaccount ,f.transtypecode
   	set @status =@@error
	end
	
	if @status = 0
	begin     
      set @statustext = 'loading non-securitised balancing-exclude delivery and interest charges -branch split'      	
	   insert into noninterface_financial(runno,transvalue, transtypecode)
	   select 0,sum (-transvalue),  
	    f.transtypecode   	   
	   from fintrans f,acct a, transtype t, branch b
   	where a.acctno = f.acctno           
   	and f.runno= 0
   	and a.securitised ='Y'
   	and f.transtypecode not in ('DEL','GRT','REB','ADD','INT','ADM','PAY','COR','RET','REF','CLD')			-- #10138
   	and T.transtypecode =F.transtypecode 
   	and left (a.acctno, 3) =b.branchno                       
   	and t.interfacebalancing =''
   	group by t.transtypecode,t.interfacebalancing,f.transtypecode 
   	set @status =@@error
   end         
   
	if @status = 0
	begin
	   set @statustext = 'loading non-securitised interest transactions-branch split'         
	   insert into noninterface_financial(runno,transvalue, transtypecode)
	   select 0,sum (transvalue) ,f.transtypecode
	   from fintrans f,acct a, transtype t, branch b
   	where a.acctno = f.acctno           
   	and f.runno= 0
   	and a.securitised !='Y'
   	and f.transtypecode in ('INT','ADM')
   	and T.transtypecode =F.transtypecode 
      and t.interfaceaccount =''
   	and left (a.acctno, 3) =b.branchno
   	and a.currstatus = 'S'
   	group by t.transtypecode,t.interfaceaccount ,f.transtypecode
   	set @status =@@error
   end       
   	if @status = 0
	begin
	   set @statustext = 'loading non-securitised balancing transactions-branch split'         
	   insert into noninterface_financial(runno,transvalue, transtypecode)          
	   select 0,sum (-transvalue),f.transtypecode 
	   from fintrans f,acct a, transtype t,  branch b 
   	where a.acctno = f.acctno           
   	and f.runno= 0
   	and a.securitised !='Y'
   	and f.transtypecode in ('INT','ADM')
   	and T.transtypecode =F.transtypecode 
   	and left (a.acctno, 3) =b.branchno
   	and a.currstatus = 'S'                   
   	and t.interfacebalancing =''
   	group by t.transtypecode,t.interfacebalancing,f.transtypecode
   	set @status =@@error
   end 
   	
   if @status = 0
	begin   
	   insert into noninterface_financial(runno,transvalue, transtypecode)
	   select 0,sum (c.depositvalue),  T.transtypecode
	   from cashierdeposits c, transtype t, branch b
	   where t.transtypecode =c.code                  
	   and b.branchno =c.branchno
	   and c.runno = 0       
	   and t.interfaceaccount =''
   	group by t.transtypecode,t.interfaceaccount 
   	set @status =@@error
   end	                                                      

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

-- End End End End End End End End End End End End End End End End End End End End End End End End End End End End End End