SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO

IF EXISTS (SELECT * FROM SysObjects
           WHERE Name = 'DN_ArchiveCurrentDatabase' AND Type = 'P')
BEGIN
    DROP PROCEDURE DN_ArchiveCurrentDatabase
END
GO

CREATE PROCEDURE DN_ArchiveCurrentDatabase 

--------------------------------------------------------------------------------
--
-- Project      : CoSACS .NET
-- File Name    : DN_ArchiveCurrentDatabase.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Procedure to archive current database
-- Author       : Alex Ayscough
-- Date         : 02 May 2006
--
-- Change Control
-- --------------
-- Date      	By  Description
-- ----      	--  -----------
--

@return int OUTPUT 
AS
BEGIN

	SET @Return = 0

	DECLARE	@archive_database varchar(64),
		@current_database varchar (64), 
		@status integer,
		@statement sqltext,
		@monthsNoMovementcash varchar (12), 
		@monthsNoMovementhp varchar (12),
		@ArchiveNumberofHp varchar(12),
		@ArchiveNumberofCash varchar (12),
		@runno integer,
		@statustext varchar (400),
		@result varchar(1),
		@monthscashandGo varchar (12),
		@beforetotal money,
		@aftertotal money
	
	select @beforetotal= sum(transvalue) from fintrans

	print 'stage 1'

	select @archive_database =value from countrymaintenance where name ='ArchiveDatabaseName'
	
	select @monthsNoMovementcash =value from countrymaintenance where name ='No of Months no movement Cash'
	
	select @monthsNoMovementhp =value from countrymaintenance where name ='No of Months no movement HP'
	
	select @ArchiveNumberofHp =value from countrymaintenance where name ='Archive Number of Hp'
	
	print 'stage 2'

	select @ArchiveNumberofCash =value from countrymaintenance where name ='Archive Number of Cash'

	select @monthscashandGo =value from countrymaintenance where name ='No of Months to retain Cash and Go'

	select @current_database =db_name()
	
	select @runno = isnull (Max (runno),0)-1 from Interfacecontrol where Interface = 'Archive'

	if @runno != -1 -- check whether last run failed
		select @result = result from Interfacecontrol where Interface = 'Archive' and runno = @runno

	print 'stage 3'

	--print 'result & runno'
	--print @result
	--print @runno
	
	
	if @result ='P' or @runno = 0-- last run did not pass so rerun last run -- otherwise incremente
	begin
	
		set @runno =@runno +1


		set @statement = ' insert into Acct_archive (acctno,date_archive,runno)'+
						 ' select top ' + @ArchiveNumberofHp + ' acctno, getdate(), ' + convert (varchar,@runno) + 
	       		 		 ' from '+
		       			 ' acct where currstatus = ''S'' and outstbal = 0 '  +
						 ' and acctno like ''___0%'' ' +
		       			 ' and datelastpaid < dateadd(month,-' + @monthsNoMovementhp + ', getdate()) ' +
		       			 ' and not exists ( select * from fintrans f where ' +
		                 ' f.acctno = acct.acctno and ' +
		                 ' f.datetrans >dateadd(month,-' + @monthsNoMovementhp + ', getdate()) )'  + 
		       			 ' and not exists ( select * from acct_archive r where ' +
		                 ' r.acctno = acct.acctno) '
		execute sp_executesql @statement

	--print @statement

		set @status =@@error

		print 'stage 4'
		if @status = 0
		begin
	
			set @statement = ' insert into Acct_archive (acctno,date_archive,runno)'+
					 ' select top ' + @ArchiveNumberofcash + ' acctno, getdate(), ' + convert (varchar,@runno) + 
					 ' from '+
					 ' acct where currstatus = ''S'' and outstbal = 0 '  +
					 ' and acctno like ''___4%'' ' +
					 ' and datelastpaid < dateadd(month,-' + @monthsNoMovementcash + ', getdate()) ' +
					 ' and not exists ( select * from fintrans f where ' +
					 ' f.acctno = acct.acctno and ' +
					 ' f.datetrans >dateadd(month,-' + @monthsNoMovementcash + ', getdate()) )' +
					 ' and not exists ( select * from acct_archive r where ' +
					 ' r.acctno = acct.acctno) '
			execute sp_executesql @statement

			set @status =@@error
		end

		print 'stage 5'

		if @status = 0
		begin

	-- print 'cash and go'

		set @statement = ' insert into CashandGo_Archive (acctno,date_archive,runno,dateagrmt,agrmtno)'+
				 ' select custacct.acctno, getdate(), ' + convert (varchar,@runno) + ', dateagrmt ,agrmtno' +
				 ' from '+
				 ' agreement,custacct where  '  +
				 ' agreement.acctno =custacct.acctno and ' +
				 ' custacct.custid like ''PAID & TAKEN%'' and custacct.hldorjnt =''H'' ' +
				 ' and agreement.acctno like ''___5%'' ' +
				 ' and (dateagrmt < dateadd(month,-' + @monthscashandGo + ', getdate()) ' +
              	 		 ' or agrmtno = 1) ' + 
              	 		 ' and not exists ( select * from CashandGo_Archive r where ' +
              	 		 ' r.acctno = agreement.acctno and r.dateagrmt =agreement.dateagrmt and r.agrmtno >1) ' + 
              	 		 '  and exists ( select * from fintrans f where ' +
              	 		 ' f.acctno = agreement.acctno ) '

	-- print @statement

		execute sp_executesql @statement
		set @status =@@error
		end

		print 'stage 6'
	
		if @status = 0 -- archive deceased people
		begin 
		
			insert into cust_archive (custid,date_archive,runno)
			select 	custid, getdate(), @runno from customer 
			where 	exists (select * from custcatcode c where  c.custid = customer.custid and c.code = 'DECD' ) -- deceased
		    	and 	not exists ( select * from cust_archive A  where customer.custid = A.custid)
		
			set @status =@@error
		end
	end -- if @result ='P' -- last run did not pass so rerun

	truncate table notesstring

	delete from Apperrorlog where err_date < dateadd (month, - 6, getdate())

	print 'stage 7'

	if @status = 0
	begin
	
	    set @statustext ='Ar_interfacevalueArchive' --purging unnecessary interfacevalue records
	    execute @status =Ar_interfacevalueArchive
	end


	print 'stage 7b'

	set @statement = 'exec '  + @archive_database + '.dbo.AR_Archivetables @archive_database=''' + @archive_database +
	                 ''' ,@source_database = ''' + @current_database + ''' , @runno = ' + convert (varchar,@runno) + ',@statustext =''''  ' 

	execute @status = sp_executesql @statement
	declare @branchno smallint

	if @status = 0
	begin
		select @branchno = hobranchno from country
		insert into interfacevalue (interface, runno, counttype1, counttype2, 
					    branchno, accttype, countvalue, value)
		select 'Archive',@runno,'HP','HP',@branchno,'H',count (*), 0
		from acct_archive
		where runno = @runno and acctno like '___0%'
		
		print 'stage 8'
		insert into interfacevalue (interface, runno, counttype1, counttype2, 
		 			    branchno, accttype, countvalue, value)
		select 'Archive',@runno,'cash','cash',@branchno,
			   'C',count (*), 0
		from acct_archive
		where runno = @runno and acctno like '___4%'
	
	
		insert into interfacevalue (interface, runno, counttype1, counttype2, 
								    branchno, accttype, countvalue, value)
		select 'Archive',@runno,'special','special',
	        	@branchno,'S',count (*), 0
		from CashandGo_Archive
		where runno = @runno 

	end


	if @status = 0
	begin
	
	  update Interfacecontrol set datefinish=getdate(), result='P' where Interface='Archive'
	  and runno = @runno
	end




	select @aftertotal= sum(transvalue) from fintrans
	if @beforetotal =@aftertotal 
		print ' database values for financial transactions the same-succeeded'
	else
	  begin
	   set @statement = ' database value for financial transactions differ -update failed before:' + convert (varchar,@beforetotal)
	    + ' after:' + convert (varchar,@aftertotal)
		 raiserror (@statement ,16,1)
	  end

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END
END
GO

GRANT EXECUTE ON DN_ArchiveCurrentDatabase TO PUBLIC
GO
SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO
