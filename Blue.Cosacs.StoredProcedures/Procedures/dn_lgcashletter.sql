SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[dn_lgcashletter]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[dn_lgcashletter]
GO

CREATE procedure dn_lgcashletter @lettercode varchar (10),
                                @runno smallint as
declare
  @fn_string varchar (500),
  @letter_file varchar (500),
  @sqltxt varchar(2000) ,
  @linestring varchar (2000) ,
  @mssql_txt SQLText ,
  @status integer,
  @errortext varchar (256)

  set @status = 0

    update #tempaccts
    set printname = firstname + ' ' + name
    where /* locate(name,'/') > size(name)  and  */
    hldorjnt in('H', 'J', 'G') 
    Print 'Processing step 11 of 12...'

    select c.custid, c.addtype, max(c.datein) as maxdatein
    into  #temp_addr 
    from custaddress c,  #tempaccts  t
    where t.custid = c.custid
    and c.addtype in('P', 'H')
    and not (c.cusaddr1 =''
    and (c.cusaddr2 is null or c.cusaddr2 ='')) 
    and (c.datemoved = ''or c.datemoved is null)
    group by c.custid, c.addtype
    
    set @status =@@error
    if @status !=0 begin
        set @errortext = 'Failed to create temp table 9.'
        drop table #tempaccts  
        drop table #temp_max  
        drop table #temp_work  
        drop table #temp_maxinstalno  
        drop table #temp_maxacctno  
        drop table #temp_sumaccounts  
        execute dn_lglogleterror @lettercode= @lettercode,
                                @runno=@runno 
        return @status  
    end
    
     update  #tempaccts 
            set cusaddr1 = custaddress.cusaddr1,
            cusaddr2 = isnull(custaddress.cusaddr2,''),
            cusaddr3 = isnull(custaddress.cusaddr3,''),
            cuspocode = isnull(custaddress.cuspocode,''),
            addtype  = custaddress.addtype
            from custaddress ,  #temp_addr 
            where  #tempaccts.custid = custaddress.custid
            and  #tempaccts.custid =  #temp_addr.custid
            and custaddress.addtype =  #temp_addr.addtype
            and custaddress.addtype = 'P'
            and custaddress.datein =  #temp_addr.maxdatein

    set @status =@@error
    if @status !=0 begin
        set @errortext = 'Failed to update cash addresses'
        drop table #tempaccts  
        drop table #temp_max  
        drop table #temp_work  
        drop table #temp_maxinstalno  
        drop table #temp_maxacctno  
        drop table #temp_sumaccounts  
        drop table #temp_addr  
                execute dn_lglogleterror @lettercode= @lettercode,   
                                         @runno=@runno 
        return @status    
    end

     update  #tempaccts 
            set cusaddr1 = custaddress.cusaddr1,
            cusaddr2 = isnull(custaddress.cusaddr2,''),
            cusaddr3 = isnull(custaddress.cusaddr3,''),
            cuspocode = isnull(custaddress.cuspocode,''),
            addtype  = custaddress.addtype
            from custaddress ,  #temp_addr 
            where  #tempaccts.custid = custaddress.custid
            and  #tempaccts.custid =  #temp_addr.custid
            and custaddress.addtype =  #temp_addr.addtype
            and custaddress.addtype = 'H'
            and  #tempaccts.addtype = ''
            and custaddress.datein =  #temp_addr.maxdatein

    set @status =@@error
    if @status !=0 begin
        set @errortext = 'Failed to update cash addresses 2'
        drop table #tempaccts  
        drop table #temp_max  
        drop table #temp_work  
        drop table #temp_maxinstalno  
        drop table #temp_maxacctno  
        drop table #temp_sumaccounts  
        drop table #temp_addr  
                execute dn_lglogleterror @lettercode= @lettercode, 
                                         @runno=@runno 
        return @status   
    end

     update  #tempaccts 
            set branchname    = branch.branchname,
            branchaddr1   = branch.branchaddr1,
            branchaddr2   = isnull(branch.branchaddr2,''),
            branchaddr3   = isnull(branch.branchaddr3,''),
            branchpocode  = isnull(branch.branchpocode,''),
            telno         = isnull(branch.telno,'')
            from branch
            where  #tempaccts.branchnohdle = branch.branchno

    set @status =@@error
    if @status !=0 begin
        set @errortext = 'Failed to update cash branch'
        drop table #tempaccts  
        drop table #temp_max  
        drop table #temp_work  
        drop table #temp_maxinstalno  
        drop table #temp_maxacctno  
        drop table #temp_sumaccounts  
        drop table #temp_addr  
                execute dn_lglogleterror @lettercode= @lettercode,
                                         @runno=@runno 
        return @status    
    end
     
    execute @status =dn_lgwriteletter @lettercode =@lettercode,
                                  @runno=@runno

    /*
    ** Select all letter details and write information to a file.
    */

    Print 'Processing complete'
    drop table #tempaccts  
    drop table #temp_addr  

    return @status 

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

