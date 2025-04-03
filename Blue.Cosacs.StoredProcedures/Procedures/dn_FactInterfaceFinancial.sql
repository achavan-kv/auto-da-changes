/* AA 16 may 2005 UAT issue 71 
AA 20/02/2006 splitt issue 11 UAT 4.0
AA 15/03/2006 changing for .Net eod to rely on table fintransLastSummary
AA 30/06/2006 68289 Removing unions as these were causing trouble
*/

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[dn_FactInterfaceFinancial]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[dn_FactInterfaceFinancial]
GO

CREATE procedure dn_FactInterfaceFinancial  
-- ================================================
-- Version:	001
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 11/04/12  jec #9886/#9887 CR9863 - int and adm interface - Storecard INT/ADM interfaced immediately
-- 11/04/12  ip  CR9863 - #9887 - Broker changes for Store Card
-- 16/04/12  jec #9931 UAT154[UAT V6] - Financial Interface Report -CLD Transactions. duplicated 
----24-11-2020 V1 ADDED NEW PAYMETHOD EPI_vendormapping
-- ================================================

@runno integer, @financial_total money output,@return int output
as
begin

SET NOCOUNT ON

declare  @status integer,@errortext varchar(10),@statustext  varchar (128)
   /* remove any data from the tables if run has failed
   normally would be updated to different run number at the end of the procedure*/ 
   -- take a lock on cashierdeposits table to prevent inserts while this procedure is running
   update cashierdeposits set runno = 0 where runno =0
   delete from  DepositssrecentInterfaced where runno < @runno -14 -- getting rid of historic deposits interfaced
   set @return = 0;
   delete from interface_financial where runno= 0
   delete from noninterface_financial where runno= 0
   set @status = 0  
             
   create table #interface (interfaceaccount varchar (10),transvalue money, vtransvalue varchar (20) null,
               transtypecode varchar (3),reference varchar (30), branchno smallint, brokerExclude bit null)
   select 
        acctno, transvalue,runno, transtypecode,branchno,isnull(paymethod,0) as paymethod --67686 error in totals if null payment method
   into #fintrans 
   from fintransLastSummary /* this stores unsettled transactions or settled transactions for int/admin*/
   where transtypecode not in ('CLD')			-- #9931 

   declare @split varchar(6)

	select @split = value from countrymaintenance where codename = 'FactInterfaceByPMethods'


   create index ix_hash_fintrans on #fintrans (acctno ) 
 
   if @status = 0      
   begin
   
     set @statustext = 'loading securitised income and deliveries'      
     if @split='True' -- splitting by payment methods
     begin
		  insert into #interface (transvalue,interfaceaccount, transtypecode, reference, branchno)
   		  select sum (transvalue), 
   			case branchsplit
   				when 0 then '000' + t.interfacesecaccount 
   			    when 1 then  case when len(b.warehouseno)<=2 THEN '0' + b.warehouseno ELSE b.warehouseno END + t.interfacesecaccount 
			end   	, 
		  t.transtypecode,			isnull (c.codedescript,'Not applicable')	, convert(smallint, left (a.acctno, 3))
   		 from acct a, transtype t, branch b,#fintrans f
		 left join code c on convert(integer,c.code) = f.paymethod and c.category = 'FPM'
   		 where a.acctno = f.acctno           
   		 --and f.runno= 0
   		 and a.securitised ='Y'
   		 and not(f.transtypecode in ('INT','ADM') and a.currstatus != 'S')	  	
  		 and T.transtypecode =F.transtypecode                            
		 and t.interfacesecaccount !=''
		 and a.accttype!= 'T'							--IP - 11/04/12 - CR9863 - #9887
   		 and left(a.acctno, 3) =b.branchno
         and not exists(select 1 from ServiceChargeAcct a where a.AcctNo = f.acctno)
         and not exists(select 1 from SR_ChargeAcct a where a.AcctNo = f.acctno)
   		 group by t.interfacesecaccount,t.branchsplit,b.warehouseno ,t.transtypecode, convert(smallint, left (a.acctno, 3)),f.paymethod,
		  c.codedescript
			
		set @status =@@error
		
     end
     else -- don't split
     begin
     
		  insert into #interface (transvalue,interfaceaccount, transtypecode, reference, branchno)
   		  select sum (transvalue), 
   			case branchsplit
   				when 0 then '000' + t.interfacesecaccount 
   				when 1 then  case when len(b.warehouseno)<=2 THEN '0' + b.warehouseno ELSE b.warehouseno END + t.interfacesecaccount 
			end   	, 
			--      '0' + b.warehouseno + t.interfacesecaccount ,
		  t.transtypecode,''	, convert(smallint, left (a.acctno, 3))
   		 from #fintrans f,acct a, transtype t, branch b
   		 where a.acctno = f.acctno           
   		 --and f.runno= 0
   		 and a.securitised ='Y'
   		 and not(f.transtypecode in ('INT','ADM') and a.currstatus != 'S')
  		 and T.transtypecode =F.transtypecode                            
		 and t.interfacesecaccount !=''
   		 and left(a.acctno, 3) =b.branchno
   		 and a.accttype!= 'T'							--IP - 11/04/12 - CR9863 - #9887
         and not exists(select 1 from ServiceChargeAcct a where a.AcctNo = f.acctno)
         and not exists(select 1 from SR_ChargeAcct a where a.AcctNo = f.acctno)
   		 group by t.interfacesecaccount,t.branchsplit,b.warehouseno ,t.transtypecode, convert(smallint, left (a.acctno, 3))

      end
		set @status =@@error
	end 
	
   if @status = 0      
   begin         
	  if @split = 'True' 
     begin 
                                             
		 set @statustext = 'loading non-securitised income and adjustment type transactions branch split' 
           
		  insert into #interface (transvalue,interfaceaccount, transtypecode,reference, branchno)
   		  select sum (transvalue),     
   		  case branchsplit
   			when 0 then '000' + t.interfaceaccount  
		when 1 then  case when len(b.warehouseno)<=2 THEN '0' + b.warehouseno ELSE b.warehouseno END + t.interfaceaccount 
		  end   	  
			, t.transtypecode,
			isnull (c.codedescript, 'Not applicable'),
			convert(smallint, left (a.acctno, 3))
		  from acct a, transtype t, branch b,#fintrans f
		  left join code c on c.category = 'FPM' and convert(integer,c.code) = f.paymethod
   		  where a.acctno = f.acctno           
   		--and f.runno= 0
		  -- excluding adjustments for securitised accounts below
   		  and a.securitised !='Y' 
   		  and not(f.transtypecode in ('INT','ADM') and a.currstatus != 'S')
   		  and T.transtypecode =F.transtypecode 
   		  and left (a.acctno, 3) =b.branchno
		  and t.interfaceaccount !=''
		  and a.accttype != 'T'					--IP - 11/04/12 - CR9863 - #9887
          and not exists(select 1 from ServiceChargeAcct a where a.AcctNo = f.acctno)
          and not exists(select 1 from SR_ChargeAcct a where a.AcctNo = f.acctno)
		  group by t.interfaceaccount,t.branchsplit,b.warehouseno  ,t.transtypecode, convert(smallint, left (a.acctno, 3)),f.paymethod,
			 c.codedescript
	         
	      set @status =@@error
	      
	      --IP - 11/04/12 - CR9863 - #9887
	      if @status = 0
	      begin

			  set @statustext = 'loading non-securitised income and adjustment type transactions branch split' 
			  
			  insert into #interface (transvalue,interfaceaccount, transtypecode,reference, branchno)
   			  select sum (transvalue),     
   			  case branchsplit
   				when 0 then '000' + t.SCInterfaceAccount 
   				when 1 then  case when len(b.warehouseno)<=2 THEN '0' + b.warehouseno ELSE b.warehouseno END + t.SCInterfaceAccount 
		
			  end   	  
				, t.transtypecode,
				isnull (c.codedescript, 'Not applicable'),
				convert(smallint, left (a.acctno, 3))
			  from acct a, transtype t, branch b,#fintrans f
			  left join code c on c.category = 'FPM' and convert(integer,c.code) = f.paymethod
   			  where a.acctno = f.acctno           
   			--and f.runno= 0
			  -- excluding adjustments for securitised accounts below
   			  and a.securitised !='Y' 
   			  and T.transtypecode =F.transtypecode 
   			  and left (a.acctno, 3) =b.branchno
			  and t.SCInterfaceAccount !=''
			  and a.accttype = 'T'				
			  group by t.SCInterfaceAccount,t.branchsplit,b.warehouseno  ,t.transtypecode, convert(smallint, left (a.acctno, 3)),f.paymethod,
				 c.codedescript
			
			  set @status =@@error
		  end
	
      end
      else -- don't split by paymethod
        begin 
                                             
		  set @statustext = 'loading non-securitised income and adjustment type transactions branch split' 
		       
		  insert into #interface 
		  (transvalue,
		   interfaceaccount, 
		   transtypecode,
		   reference, 
		   branchno)
   		  select sum (transvalue),     
   		  case branchsplit
   		   when 0 then '000' + t.interfaceaccount 
   		
		 		when 1 then case when len(b.warehouseno)<=2 THEN '0' + b.warehouseno ELSE b.warehouseno END + t.interfaceaccount 
		
		  end 
 		   ,
			 t.transtypecode,
			'' ,
			convert(smallint, left (a.acctno, 3))
		  from #fintrans f,acct a, transtype t, branch b
   		  where a.acctno = f.acctno           
   		  --and f.runno= 0
		  -- excluding adjustments for securitised accounts below
   		  and a.securitised !='Y' 
   		  and not(f.transtypecode in ('INT','ADM') and a.currstatus != 'S')
   		  and T.transtypecode =F.transtypecode 
   		  and left (a.acctno, 3) =b.branchno
		  and t.interfaceaccount !=''
		  and a.accttype != 'T'					--IP - 11/04/12 - CR9863 - #9887
          and not exists(select 1 from ServiceChargeAcct a where a.AcctNo = f.acctno)
          and not exists(select 1 from SR_ChargeAcct a where a.AcctNo = f.acctno)
		  group by t.interfaceaccount,t.branchsplit,b.warehouseno  ,t.transtypecode, convert(smallint, left (a.acctno, 3))
		  set @status =@@error
	     	
		  --IP - 11/04/12 - CR9863 - #9887
		  if @status = 0
			  begin
					 set @statustext = 'loading non-securitised income and adjustment type transactions branch split' 
					  
					insert into #interface 
				  (transvalue,
				   interfaceaccount, 
				   transtypecode,
				   reference, 
				   branchno)
   				  select sum (transvalue),     
   				  case branchsplit
   				   when 0 then '000' + t.SCInterfaceAccount 
					 when 1 then  case when len(b.warehouseno)<=2 THEN '0' + b.warehouseno ELSE b.warehouseno END + t.SCInterfaceAccount
	
				  end 
 				   ,
					 t.transtypecode,
					'' ,
					convert(smallint, left (a.acctno, 3))
				  from #fintrans f,acct a, transtype t, branch b
   				  where a.acctno = f.acctno           
   				  --and f.runno= 0
				  -- excluding adjustments for securitised accounts below
   				  and a.securitised !='Y' 
   				  and T.transtypecode =F.transtypecode 
   				  and left (a.acctno, 3) =b.branchno
				  and t.SCInterfaceAccount !=''
				  and a.accttype = 'T'				
				  group by t.SCInterfaceAccount,t.branchsplit,b.warehouseno  ,t.transtypecode, convert(smallint, left (a.acctno, 3))
			  end
	      
		end
   		
	end

    ------------------------------------------------------------------------------------------------------------------------------------------------------

    --Seperate transactions on ServiceChargeAcct and SR_ChargeAcct, so that these can be excluded from broker export later.

      if @status = 0      
   begin
   
     set @statustext = 'loading securitised income and deliveries'      
     if @split='True' -- splitting by payment methods
     begin
		  insert into #interface (transvalue,interfaceaccount, transtypecode, reference, branchno, brokerExclude)
   		  select sum (transvalue), 
   			case branchsplit
   				when 0 then '000' + t.interfacesecaccount 
   			 when 1 then case when len(b.warehouseno)<=2 THEN '0' + b.warehouseno ELSE b.warehouseno END + t.interfacesecaccount 
	
			end   	, 
		  t.transtypecode,			
          isnull (c.codedescript,'Not applicable'), 
          convert(smallint, left (a.acctno, 3)),
          1                                             --Exclude from Broker
   		 from acct a, transtype t, branch b,#fintrans f
		 left join code c on convert(integer,c.code) = f.paymethod and c.category = 'FPM'
   		 where a.acctno = f.acctno           
   		 --and f.runno= 0
   		 and a.securitised ='Y'
   		 and not(f.transtypecode in ('INT','ADM') and a.currstatus != 'S')	  	
  		 and T.transtypecode =F.transtypecode                            
		 and t.interfacesecaccount !=''
		 and a.accttype!= 'T'							--IP - 11/04/12 - CR9863 - #9887
   		 and left(a.acctno, 3) =b.branchno
         and (exists(select 1 from ServiceChargeAcct a where a.AcctNo = f.acctno)
         or exists(select 1 from SR_ChargeAcct a where a.AcctNo = f.acctno))
   		 group by t.interfacesecaccount,t.branchsplit,b.warehouseno ,t.transtypecode, convert(smallint, left (a.acctno, 3)),f.paymethod,
		  c.codedescript
			
		set @status =@@error
		
     end
     else -- don't split
     begin
     
		  insert into #interface (transvalue,interfaceaccount, transtypecode, reference, branchno, brokerExclude)
   		  select sum (transvalue), 
   			case branchsplit
   				when 0 then '000' + t.interfacesecaccount 
   				 when 1 then case when len(b.warehouseno)<=2 THEN '0' + b.warehouseno ELSE b.warehouseno END + t.interfacesecaccount 
	
			end   	, 
			--      '0' + b.warehouseno + t.interfacesecaccount ,
		  t.transtypecode,
          '', 
          convert(smallint, left (a.acctno, 3)),
          1                                         --Exclude from Broker
   		 from #fintrans f,acct a, transtype t, branch b
   		 where a.acctno = f.acctno           
   		 --and f.runno= 0
   		 and a.securitised ='Y'
   		 and not(f.transtypecode in ('INT','ADM') and a.currstatus != 'S')
  		 and T.transtypecode =F.transtypecode                            
		 and t.interfacesecaccount !=''
   		 and left(a.acctno, 3) =b.branchno
   		 and a.accttype!= 'T'							--IP - 11/04/12 - CR9863 - #9887
         and (exists(select 1 from ServiceChargeAcct a where a.AcctNo = f.acctno)
         or exists(select 1 from SR_ChargeAcct a where a.AcctNo = f.acctno))
   		 group by t.interfacesecaccount,t.branchsplit,b.warehouseno ,t.transtypecode, convert(smallint, left (a.acctno, 3))

      end
		set @status =@@error
	end 
	
   if @status = 0      
   begin         
	  if @split = 'True' 
     begin 
                                             
		 set @statustext = 'loading non-securitised income and adjustment type transactions branch split' 
           
		  insert into #interface (transvalue,interfaceaccount, transtypecode,reference, branchno, brokerExclude)
   		  select sum (transvalue),     
   		  case branchsplit
   			when 0 then '000' + t.interfaceaccount 
   		 when 1 then  case when len(b.warehouseno)<=2 THEN '0' + b.warehouseno ELSE b.warehouseno END + t.interfaceaccount 
		  end   	  
			, t.transtypecode,
			isnull (c.codedescript, 'Not applicable'),
			convert(smallint, left (a.acctno, 3)),
            1                                                   --Exclude from Broker
		  from acct a, transtype t, branch b,#fintrans f
		  left join code c on c.category = 'FPM' and convert(integer,c.code) = f.paymethod
   		  where a.acctno = f.acctno           
   		--and f.runno= 0
		  -- excluding adjustments for securitised accounts below
   		  and a.securitised !='Y' 
   		  and not(f.transtypecode in ('INT','ADM') and a.currstatus != 'S')
   		  and T.transtypecode =F.transtypecode 
   		  and left (a.acctno, 3) =b.branchno
		  and t.interfaceaccount !=''
		  and a.accttype != 'T'					--IP - 11/04/12 - CR9863 - #9887
          and (exists(select 1 from ServiceChargeAcct a where a.AcctNo = f.acctno)
          or exists(select 1 from SR_ChargeAcct a where a.AcctNo = f.acctno))
		  group by t.interfaceaccount,t.branchsplit,b.warehouseno  ,t.transtypecode, convert(smallint, left (a.acctno, 3)),f.paymethod,
			 c.codedescript
	         
	      set @status =@@error
	      
      end
      else -- don't split by paymethod
        begin 
                                             
		  set @statustext = 'loading non-securitised income and adjustment type transactions branch split' 
		       
		  insert into #interface 
		  (transvalue,
		   interfaceaccount, 
		   transtypecode,
		   reference, 
		   branchno,
           brokerExclude)
   		  select sum (transvalue),     
   		  case branchsplit
   		   when 0 then '000' + t.interfaceaccount 
	 when 1 then  case when len(b.warehouseno)<=2 THEN '0' + b.warehouseno ELSE b.warehouseno END + t.interfaceaccount 
		  end 
 		   ,
			 t.transtypecode,
			'' ,
			convert(smallint, left (a.acctno, 3)),
            1
		  from #fintrans f,acct a, transtype t, branch b
   		  where a.acctno = f.acctno           
   		  --and f.runno= 0
		  -- excluding adjustments for securitised accounts below
   		  and a.securitised !='Y' 
   		  and not(f.transtypecode in ('INT','ADM') and a.currstatus != 'S')
   		  and T.transtypecode =F.transtypecode 
   		  and left (a.acctno, 3) =b.branchno
		  and t.interfaceaccount !=''
		  and a.accttype != 'T'					--IP - 11/04/12 - CR9863 - #9887
          and (exists(select 1 from ServiceChargeAcct a where a.AcctNo = f.acctno)
          or exists(select 1 from SR_ChargeAcct a where a.AcctNo = f.acctno))
		  group by t.interfaceaccount,t.branchsplit,b.warehouseno  ,t.transtypecode, convert(smallint, left (a.acctno, 3))

		  set @status =@@error
	     	
		end
   		
	end

    ------------------------------------------------------------------------------------------------------------------------------------------------------------

	if @status = 0
	begin     
      set @statustext = 'loading adjustment balancing -branch split' 
           	
	   insert into #interface (transvalue,interfaceaccount, transtypecode, reference, branchno)          
	   select sum (-transvalue), 
	   case branchsplitbalancing
   			when 0 then '000' + t.interfacebalancing 
   		 when 1 then case when len(b.warehouseno)<=2 THEN '0' + b.warehouseno ELSE b.warehouseno END + t.interfacebalancing 
			end,t.transtypecode,'', convert(smallint, left (a.acctno, 3)) -- Branch split is based on account number
	   from #fintrans f,acct a, transtype t, branch b
   	   where a.acctno = f.acctno           
   	   --and f.runno= 0
   	   and not(f.transtypecode in ('INT','ADM') and a.currstatus != 'S')
       and f.transtypecode not in ('DEL', 'GRT', 'ADD', 'REB','RDL','REP','PAY', 'REF', 'RET', 'COR','DPY','SHO','OVE') 
       --excluding the above transactions as either delivery or money transactions so should be split by branch that physical money taken at not account number
       -- XFR now not excluded
       and f.transtypecode not in ('REP','PAY', 'REF', 'RET', 'COR','DPY')
       and a.securitised !='Y'
   	   and T.transtypecode =F.transtypecode 
	   and t.interfacebalancing !=''
       and left (a.acctno, 3) =b.branchno   
       and a.accttype != 'T'				--IP - 11/04/12 - CR9863 - #9887         
   	   group by t.interfacebalancing,t.branchsplitbalancing,b.warehouseno  ,t.transtypecode, convert(smallint, left (a.acctno, 3))
   	   
   	   set @status =@@error

   end 
   
   	  --IP - 11/04/12 - CR9863 - #9887
   if @status = 0
   begin
	   set @statustext = 'loading adjustment balancing -branch split'      
   	   insert into #interface (transvalue,interfaceaccount, transtypecode, reference, branchno)          
	   select sum (-transvalue), 
	   case branchsplitbalancing
   			when 0 then '000' + t.SCInterfaceBalancing
   			
			 when 1 then  case when len(b.warehouseno)<=2 THEN '0' + b.warehouseno ELSE b.warehouseno END + t.SCInterfaceBalancing 
	
			end,t.transtypecode,'', convert(smallint, left (a.acctno, 3)) -- Branch split is based on account number
	   from #fintrans f,acct a, transtype t, branch b
   	   where a.acctno = f.acctno           
   	   --and f.runno= 0
       and f.transtypecode not in ('DEL', 'GRT', 'ADD', 'REB','RDL','REP','PAY', 'REF', 'RET', 'COR','DPY','SHO','OVE') 
       --excluding the above transactions as either delivery or money transactions so should be split by branch that physical money taken at not account number
       -- XFR now not excluded
       and f.transtypecode not in ('REP','PAY', 'REF', 'RET', 'COR','DPY')
       and a.securitised !='Y'
   	   and T.transtypecode =F.transtypecode 
	   and t.SCInterfaceBalancing !=''
       and left (a.acctno, 3) =b.branchno   
       and a.accttype = 'T'				      
   	   group by t.SCInterfaceBalancing,t.branchsplitbalancing,b.warehouseno  ,t.transtypecode, convert(smallint, left (a.acctno, 3))
		
	    set @status =@@error
   end        

	if @status = 0
	begin     
      set @statustext = 'loading adjustment + del balancing -branch split sec'      	
	   insert into #interface (transvalue,interfaceaccount, transtypecode, reference, branchno)          
	   select sum (-transvalue), 
	   case branchsplitbalancing
   			when 0 then '000' + t.interfacesecbalancing 
   		 when 1 then case when len(b.warehouseno)<=2 THEN '0' + b.warehouseno ELSE b.warehouseno END + t.interfacesecbalancing 		
		 end,t.transtypecode,'', convert(smallint, left (a.acctno, 3)) -- Branch split is based on account number
	    from #fintrans f,acct a, transtype t, branch b
   		where a.acctno = f.acctno           
   		--and f.runno= 0
   		and not(f.transtypecode in ('INT','ADM') and a.currstatus != 'S')
		and f.transtypecode not in ('PAY', 'REF', 'RET', 'COR','DPY','SHO','OVE') -- these transactions are excluded because the branch split is based on the branch that the transaction was taken from
		-- XFR now not excluded so included as want to interface this. 
		and a.securitised ='Y'
   		and T.transtypecode =F.transtypecode 
		and t.interfacesecbalancing !=''
    	and left (a.acctno, 3) =b.branchno   
    	and a.accttype != 'T'					--IP - 11/04/12 - CR9863 - #9887         
   	group by t.interfacesecbalancing,t.branchsplitbalancing,b.warehouseno  ,t.transtypecode, convert(smallint, left (a.acctno, 3))
   	
   	set @status =@@error
   	
   end         

   if @status = 0
	begin 
	    
      set @statustext = 'loading income balancing-2980 -branch split by branch which transaction taken at'  
          	
	   insert into #interface (transvalue,interfaceaccount, transtypecode, reference, branchno)          
	   select sum (-transvalue), 
	   case branchsplitbalancing
   			when 0 then '000' + t.interfacebalancing 
			
   				 when 1 then  case when len(b.warehouseno)<=2 THEN '0' + b.warehouseno ELSE b.warehouseno END + t.interfacebalancing
	
			end, t.transtypecode,'',b.branchno    -- Branch split is based on branch where transaction taken at as will have money
       from #fintrans f,acct a, transtype t, branch b
   	   where a.acctno = f.acctno           
   	   --and f.runno= 0
   	   --and a.securitised !='Y'
       and f.transtypecode  in ('PAY', 'REF', 'RET', 'COR','DPY','SHO','OVE')
   	   and T.transtypecode =F.transtypecode 
       and f.branchno =b.branchno            
	   and t.interfacebalancing !=''
	   and a.accttype != 'T'				--IP - 11/04/12 - CR9863 - #9887
       and not exists(select 1 from ServiceChargeAcct a where a.AcctNo = f.acctno)
       and not exists(select 1 from SR_ChargeAcct a where a.AcctNo = f.acctno)
   	   group by t.interfacebalancing,t.branchsplitbalancing,b.warehouseno  ,t.transtypecode,b.branchno  
   	   set @status =@@error
   	
   end 
   
   -----------------------------------------------------------------------------------------------------------------------------------------

   --Balancing for ServiceChargeAcct transactions so that these can be excluded from Broker Export later.
      if @status = 0
	begin 
	    
      set @statustext = 'loading income balancing-2980 -branch split by branch which transaction taken at'  
          	
	   insert into #interface (transvalue,interfaceaccount, transtypecode, reference, branchno, brokerExclude)          
	   select sum (-transvalue), 
	   case branchsplitbalancing
   			when 0 then '000' + t.interfacebalancing 
			--'000' + t.interfacebalancing 
   		
			when 1 then  case when len(b.warehouseno)<=2 THEN '0' + b.warehouseno ELSE b.warehouseno END + t.interfacebalancing 
	
			end, 
       t.transtypecode,'',
       b.branchno,    -- Branch split is based on branch where transaction taken at as will have money
       1                                                                  --Exclude from Broker                           
       from #fintrans f,acct a, transtype t, branch b
   	   where a.acctno = f.acctno           
   	   --and f.runno= 0
   	   --and a.securitised !='Y'
       and f.transtypecode  in ('PAY', 'REF', 'RET', 'COR','DPY','SHO','OVE')
   	   and T.transtypecode =F.transtypecode 
       and f.branchno =b.branchno            
	   and t.interfacebalancing !=''
	   and a.accttype != 'T'				--IP - 11/04/12 - CR9863 - #9887
       and (exists(select 1 from ServiceChargeAcct a where a.AcctNo = f.acctno)
       or exists(select 1 from SR_ChargeAcct a where a.AcctNo = f.acctno))
   	   group by t.interfacebalancing,t.branchsplitbalancing,b.warehouseno  ,t.transtypecode,b.branchno  
   	   set @status =@@error
   	
   end 

   ------------------------------------------------------------------------------------------------------------------------------------------

   if @status = 0
   begin
	   --IP - 11/04/12 - CR9863 - #9887
	   set @statustext = 'loading income balancing-2980 -branch split by branch which transaction taken at'  
   	   insert into #interface (transvalue,interfaceaccount, transtypecode, reference, branchno)          
	   select sum (-transvalue), 
	   case branchsplitbalancing
   			when 0 then '000' + t.SCInterfaceBalancing 
			--'000' + t.interfacebalancing 
   			
			when 1 then  case when len(b.warehouseno)<=2 THEN '0' + b.warehouseno ELSE b.warehouseno END + t.SCInterfaceBalancing 
	
			end, t.transtypecode,'',b.branchno    -- Branch split is based on branch where transaction taken at as will have money
       from #fintrans f,acct a, transtype t, branch b
   	   where a.acctno = f.acctno           
   	   --and f.runno= 0
   	   --and a.securitised !='Y'
       and f.transtypecode  in ('PAY', 'REF', 'RET', 'COR','DPY','SHO','OVE')
   	   and T.transtypecode =F.transtypecode 
       and f.branchno =b.branchno            
	   and t.SCInterfaceBalancing !=''
	   and a.accttype = 'T'				
   	   group by t.SCInterfaceBalancing,t.branchsplitbalancing,b.warehouseno  ,t.transtypecode,b.branchno  
	   set @status =@@error
   
   end  
         
   declare @taxrate float, @taxleft float
   
   --loading deposits no tax is applicable
   if @status = 0
	begin 
	  
      set @statustext = 'loading deposits -branch split by branch which acctno taken at'  
          	
	   insert into #interface (transvalue,interfaceaccount, transtypecode, reference, branchno)            
	   select sum (c.depositvalue),  
	   case branchsplit
   	   when 0 then '000' + t.interfaceaccount 
	when 1 then  case when len(b.warehouseno)<=2 THEN '0' + b.warehouseno ELSE b.warehouseno END + t.interfaceaccount
             end, t.transtypecode, reference,c.branchno 
	   from cashierdeposits c, transtype t, branch b
	   where t.transtypecode =c.code                  
	   and b.branchno =c.branchno
	   and c.runno = 0       
	   and t.interfaceaccount !=''     and (C.voided != 'Y' or C.voided is null)
	   and (convert (integer,c.paymethod) < 100 or convert (integer,c.paymethod) in (select Paymethod from EPI_vendormapping))
	   and t.isdeposit in (1,4,5)
   	   group by t.interfaceaccount,t.branchsplit,b.warehouseno  ,t.transtypecode, reference,c.branchno  
   	    	   
	   insert into #interface (transvalue,interfaceaccount, transtypecode, reference, branchno)            
	   select
	      sum (c.depositvalue*x.rate),  
	   case branchsplit
   			when 0 then '000' + t.interfaceaccount 
   	
			when 1 then case when len(b.warehouseno)<=2 THEN '0' + b.warehouseno ELSE b.warehouseno END + t.interfaceaccount 
            end, t.transtypecode, reference,c.branchno 
	   from cashierdeposits c, transtype t, branch b,exchangerate x
	   where t.transtypecode =c.code                  
	   and b.branchno =c.branchno
	   and c.runno = 0       
       and (C.voided != 'Y' or C.voided is null)
	   and x.currency =c.paymethod 
	   and x.status ='C' and convert (integer,c.paymethod) >= 100 and t.isdeposit in (1,4,5)
	   and t.interfaceaccount !=''
   	   group by t.interfaceaccount,t.branchsplit,b.warehouseno  ,t.transtypecode, reference,c.branchno 
   	   
	 	set @status =@@error
   end	                  

   select @taxrate= isnull (taxrate, 0) from country   
   set @taxleft = 100 -@taxrate
   -- loading petty cash and disbursements value should exclude tax.
   if @status = 0
	begin   
      set @statustext = 'loading deposits -branch split by branch which acctno taken at'      	
	   insert into #interface (transvalue,interfaceaccount, transtypecode, reference, branchno)            
	   select sum (c.depositvalue)/(1 + @taxrate/100),  
	   case branchsplit
   	   when 0 then '000' + t.interfaceaccount 
  
	 when 1 then case when len(b.warehouseno)<=2 THEN '0' + b.warehouseno ELSE b.warehouseno END + t.interfaceaccount 

             end, t.transtypecode, reference,c.branchno 
	   from cashierdeposits c, transtype t, branch b
	   where t.transtypecode =c.code                  
	   and b.branchno =c.branchno
	   and c.runno = 0       
	   and t.interfaceaccount !=''     and (C.voided != 'Y' or C.voided is null)
	   --and convert (integer,c.paymethod)< 100 and t.isdeposit in (2,3)
	    and (convert (integer,c.paymethod) < 100 or convert (integer,c.paymethod) in (select Paymethod from EPI_vendormapping))
		and t.isdeposit in (2,3)
   	group by t.interfaceaccount,t.branchsplit,b.warehouseno  ,t.transtypecode, reference,c.branchno   	   
	   insert into #interface (transvalue,interfaceaccount, transtypecode, reference, branchno)             --foreign currency translation
	   select
	      sum (c.depositvalue*x.rate)/(1 + @taxrate/100),  
	   case branchsplit
   	   when 0 then '000' + t.interfaceaccount 
   	 
	  when 1 then case when len(b.warehouseno)<=2 THEN '0' + b.warehouseno ELSE b.warehouseno END + t.interfaceaccount 

             end, t.transtypecode, reference,c.branchno 
	   from cashierdeposits c, transtype t, branch b,exchangerate x
	   where t.transtypecode =c.code                  
	   and b.branchno =c.branchno
	   and c.runno = 0       
      and (C.voided != 'Y' or C.voided is null)
	   and x.currency =c.paymethod 
	   and x.status ='C' and convert (integer,c.paymethod) >= 100 and t.isdeposit in (2,3)
   	group by t.interfaceaccount,t.branchsplit,b.warehouseno  ,t.transtypecode, reference,c.branchno 
	 	set @status =@@error
   end	                  

   if @status = 0 -- and @taxrate > 0.01
	begin   
      set @statustext = 'loading tax value for disbursements.'      	
	   insert into #interface (transvalue,interfaceaccount, transtypecode, reference, branchno)            
	    	   select sum (c.depositvalue) -(sum (c.depositvalue)/(1 + @taxrate/100)),   
       
	   case when len(b.warehouseno)<=2 THEN '0' + b.warehouseno + '2985' ELSE b.warehouseno  + '2985'  END 
        , t.transtypecode, reference,c.branchno 
	   from cashierdeposits c, transtype t, branch b
	   where t.transtypecode =c.code                  
	   and b.branchno =c.branchno
	   and c.runno = 0       
	   and t.interfaceaccount !=''     and (C.voided != 'Y' or C.voided is null)
	  -- and convert (integer,c.paymethod)< 100 
	    and (convert (integer,c.paymethod) < 100 or convert (integer,c.paymethod) in (select Paymethod from EPI_vendormapping))
	   and t.isdeposit in (2,3)
   	group by t.interfaceaccount,t.branchsplit,b.warehouseno  ,t.transtypecode, reference,c.branchno   	   
	   insert into #interface (transvalue,interfaceaccount, transtypecode, reference, branchno)            
	   select
			sum (c.depositvalue* x.rate) -(sum (c.depositvalue * x.rate)/(1 + @taxrate/100)),   
     
	  case when len(b.warehouseno)<=2 THEN '0' + b.warehouseno + '2985' ELSE b.warehouseno  + '2985'  END 
       , t.transtypecode, reference,c.branchno 
	   from cashierdeposits c, transtype t, branch b,exchangerate x
	   where t.transtypecode =c.code                  
	   and b.branchno =c.branchno
	   and c.runno = 0       
      and (C.voided != 'Y' or C.voided is null)
	   and x.currency =c.paymethod 
	   and x.status ='C' and convert (integer,c.paymethod) >= 100 and t.isdeposit in (2,3) --normal tax disbursement codes
   	group by t.interfaceaccount,t.branchsplit,b.warehouseno  ,t.transtypecode, reference,c.branchno 
	 	set @status =@@error
   end	                  

   if @status = 0  --and @taxrate > 0.01
	begin   
      set @statustext = 'loading tax value for tax only disbursements.'      	
	   insert into #interface (transvalue,interfaceaccount, transtypecode, reference, branchno)            
	    	   select sum (c.depositvalue), 
			 
			  case when len(b.warehouseno)<=2 THEN '0' + b.warehouseno + '2985' ELSE b.warehouseno  + '2985'  END 
			   ,t.transtypecode, reference,c.branchno 
	   from cashierdeposits c, transtype t, branch b
	   where t.transtypecode =c.code                  
	   and b.branchno =c.branchno
	   and c.runno = 0       
	   and t.interfaceaccount !=''     and (C.voided != 'Y' or C.voided is null)
	   --and convert (integer,c.paymethod)< 100 
	    and (convert (integer,c.paymethod) < 100 or convert (integer,c.paymethod) in (select Paymethod from EPI_vendormapping))
	   and t.isdeposit in (6,7) -- tax only disbursements/petty cash
   	group by t.interfaceaccount,t.branchsplit,b.warehouseno  ,t.transtypecode, reference,c.branchno   	   
	   insert into #interface (transvalue,interfaceaccount, transtypecode, reference, branchno)             --in the unlikely event that tax amounts will be posted.
	   select
	      sum (c.depositvalue*x.rate),  
		
			case when len(b.warehouseno)<=2 THEN '0' + b.warehouseno + '2985' ELSE b.warehouseno  + '2985'  END 
       , t.transtypecode, reference,c.branchno 
	   from cashierdeposits c, transtype t, branch b,exchangerate x
	   where t.transtypecode =c.code                  
	   and b.branchno =c.branchno
	   and c.runno = 0       
      and (C.voided != 'Y' or C.voided is null)
	   and x.currency =c.paymethod 
	   and x.status ='C' and convert (integer,c.paymethod) >= 100 and t.isdeposit in (6,7)
   	group by t.interfaceaccount,t.branchsplit,b.warehouseno  ,t.transtypecode, reference,c.branchno 
	 	set @status =@@error
   end	                  

   
   if @status = 0
	begin   
	   insert into #interface (transvalue,interfaceaccount, transtypecode, reference, branchno)            
	   select sum (-c.depositvalue),  
	   case branchsplitbalancing
   	   when 0 then '000' + t.interfacebalancing 
 		when 1 then  case when len(b.warehouseno)<=2 THEN '0' + b.warehouseno ELSE b.warehouseno END + t.interfacebalancing
		
             end, t.transtypecode, '', c.branchno 
	   from cashierdeposits c, transtype t, branch b
	   where t.transtypecode =c.code                  
	   and b.branchno =c.branchno
	   and c.runno = 0       
	   and (C.voided != 'Y' or C.voided is null)
	   --and convert (integer,c.paymethod)< 100
	    and (convert (integer,c.paymethod) < 100 or convert (integer,c.paymethod) in (select Paymethod from EPI_vendormapping))
   	group by t.interfacebalancing,t.branchsplitbalancing,b.warehouseno  ,t.transtypecode,c.branchno   
	   insert into #interface (transvalue,interfaceaccount, transtypecode, reference, branchno)             --including foreign currency
	   select
	      sum (-c.depositvalue*x.rate),  

	   case branchsplitbalancing
   	   when 0 then '000' +  t.interfacebalancing 
   	  
	  when 1 then case when len(b.warehouseno)<=2 THEN '0' + b.warehouseno ELSE b.warehouseno END + t.interfacebalancing 
		
             end, t.transtypecode, '',c.branchno
	   from cashierdeposits c, transtype t, branch b,exchangerate x
	   where t.transtypecode =c.code                  
	   and b.branchno =c.branchno
	   and c.runno = 0       
	   and t.interfacebalancing !=''     and (C.voided != 'Y' or C.voided is null)
	   and x.currency =c.paymethod 
	   and x.status ='C' and convert (integer,c.paymethod) >= 100
   	group by t.interfacebalancing,t.branchsplitbalancing,b.warehouseno  ,t.transtypecode,c.branchno   
	 	set @status =@@error
   end	                  
 /*we are now going to select total value of securitised accounts since the last run of this financial output */
    declare @Datefinancialfinish datetime
    select @Datefinancialfinish= Max (datefinish) from interfacecontrol where interface = 'UPDSMRY'
    and runno<@runno

   if @status = 0
	begin 
      delete from SecuritisedSummaryInterface
	
    insert into SecuritisedSummaryInterface(branchno, transvalue, transtypecode)
    select convert (integer,left (acctno, 3)), balance,'SBL'
    from sec_account where datesecuritised> @Datefinancialfinish
    	set @status =@@error
   end	                  

    insert into #interface (Interfaceaccount, transvalue, reference, transtypecode,branchno)
    select
	case when len(b.warehouseno)<=2 THEN '0' + b.warehouseno ELSE b.warehouseno END + t.interfacesecaccount
	,isnull (sum(transvalue), 0), '',S.transtypecode, s.branchno
    from SecuritisedSummaryInterface s, transtype t, branch b
  	 where t.transtypecode =s.transtypecode  and b.branchno =s.branchno and t.interfacesecaccount !=''
    group by s.branchno ,t.transtypecode,t.interfacesecaccount, b.warehouseno ,S.transtypecode

    insert into #interface (Interfaceaccount, transvalue, reference,transtypecode,branchno)
    select  
	case when len(b.warehouseno)<=2 THEN '0' + b.warehouseno ELSE b.warehouseno END + t.interfacebalancing 
	,-isnull (sum(transvalue), 0), '',S.transtypecode,s.branchno
    from SecuritisedSummaryInterface s, transtype t, branch b
  	 where t.transtypecode =s.transtypecode  and b.branchno =s.branchno and t.interfacebalancing !=''
    group by s.branchno ,t.transtypecode,t.interfacebalancing, b.warehouseno ,S.transtypecode

   -- for transaction types length of the code should be set to 7 -otherwise error account.
   update #interface set interfaceaccount ='0009998' where datalength(interfaceaccount) !=7 and datalength(interfaceaccount) !=8
   -- rounding to 2 decimal places so trailer record always 0. 

   --We do not want this to export to Broker
   UPDATE #interface
   SET BrokerExclude = 1 where TransTypeCode = 'SRI'

   --We still need this Service transaction to be exported to Broker as it is not handled in Web Service.
   UPDATE #interface
   SET brokerExclude = 0 where transtypecode = 'WOS' and brokerExclude = 1


   insert into interface_financial (runno,transvalue,interfaceaccount, transtypecode, reference, branchno, BrokerExclude)
   select @runno,  transvalue,Interfaceaccount, transtypecode, reference, branchno, brokerExclude from #interface
   where transtypecode not in ('DEL', 'GRT', 'REP', 'REB', 'RDL', 'ADD') -- #13079
   
	select interfaceaccount, sum (transvalue) as transvalue, convert (varchar (20),'') as vtransvalue, reference into #interface_result2
   from #interface
   where transtypecode not in ('DEL', 'GRT', 'REP', 'REB', 'RDL', 'ADD') --IP -15/09/08- CR946 - Exclude Delivery type transactions
   group by Interfaceaccount, reference
    update #interface_result2 set transvalue = round(transvalue,2)  ----24-11-2020 NCB VALUE ADDED
   update #interface_result2 set vtransvalue = left (isnull (convert (varchar (20), transvalue*100),''), datalength(isnull (convert (varchar (20), transvalue*100),'')) - 3) 

   insert into DepositssrecentInterfaced   (depositid, datedeposit, code, empeeno, runno)
   select depositid, datedeposit, code, empeeno, @runno from cashierdeposits where runno = 0
   --exec dn_FactNonInterfaceFinancial
   select @financial_total= sum(transvalue)  from #interface_result2
   select  interfaceaccount, transvalue,vtransvalue, reference from #interface_result2 where  
   transvalue != 0
                       
end
GO

--select * from DepositssrecentInterfaced
--exec dn_FactInterfaceFinancial @runno =550, @financial_total =0,@return =0

--sp_help cashierdeposits
--exec dn_FactInterfaceFinancial @runno = 534, @financial_total = 0,@return = 0
