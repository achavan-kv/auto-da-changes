if exists (select * from sysobjects where name ='dn_lettersGenerateCSVfiles')
drop procedure dn_lettersGenerateCSVfiles 
go

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

create procedure [dbo].[dn_lettersGenerateCSVfiles] 
-- ============================================================================================
-- Version: 002
-- Author:		?
-- Create date: ?
-- Description:	
--
-- Change Control
-- --------------
-- Date      By					Description
-- ----      --					-----------
-- 22/02/12  IP					#9601 - UAT89 - Cash Loan letters were not printed with Employment Details
-- 07/08/19  Zensar  	#001   Strategy Job Optimization - Added Code to Configure Sending SMS Based on Value Set in Country Parameters. 
-- ============================================================================================
	@runno int,
	@type varchar(15), --IP - 18/07/08 - UAT 5.2 - UAT (24) changed from varchar 10 to 15 for 'COLLECTIONS' type.
	@return int output
as
	set @return = 0
	declare @lettercode varchar (10) ,@statement sqltext,@csvfile varchar (200),@datfile varchar (200),@statement3 sqltext,
	@dateletter datetime,@datestart datetime,@datefinish datetime
	,@statementSC1 sqltext,@statementSC2 sqltext,@statementSC3 sqltext,@statementSCO sqltext,@statementCopy sqltext,@Database varchar (20),			-- #17133
	@systemdrive varchar(50),@bcpOptions varchar(30), @driveLTR varchar(50), @execCmdShell varchar(40)

	select @systemdrive= value from CountryMaintenance with(nolock) where codename='systemdrive'
	set @bcpOptions=' -t, -c -q -T '
	set @driveLTR=@systemdrive + '\LTR'
	set @execCmdShell='execute Master.dbo.xp_cmdshell '

	--Start Configure Sending Letters based on values set in Country Parameter: Dt : 06 Aug 2019 : Zensar(SH)
	DECLARE @lettersApplicable char(1)
	Set @lettersApplicable = 'Y'
	If(@type = 'COLLECTIONS')
	BEGIN
		select @lettersApplicable = convert(char(1), Value) from countrymaintenance with(nolock) where codename = 'LettersApplicable'
	END
	
	if(@lettersApplicable = 'Y')
    BEGIN
		--End Zensar(SH)
		-- we are interested in all letters generated since the end of the last letters and charges run.
		--IP - 18/07/08 - UAT5.2 - UAT(24) - Added OR clause for 'COLLECTIONS'.
		if @type ='CHARGES' OR @type = 'PCLUBTIERS' OR @type = 'COLLECTIONS' OR @type = 'CASHLOANQUAL'	--IP - 23/02/12 - #9601 - UAT89
																																																																																																																																											BEGIN


			select @datestart =dateadd(minute,1,max(datefinish)) from interfacecontrol 
			where interface =@type  and runno =@runno -1 
			and datestart > dateadd(day,-20,getdate()) --this is to make sure this years letter run -20 not 20 silly. 
	  
			if @datestart = '1-jan-1900' or @datestart is null -- last run failed just get letters generated in the current run
				select @datestart = max(datestart) from interfacecontrol  with(nolock) where interface =@type  and runno =@runno 
				set @datefinish = getdate()

				--select @datestart
				DECLARE letter_cursor CURSOR 
  				FOR SELECT distinct lettercode
				from letter with(nolock)
				where (dateacctlttr between @datestart and @datefinish --and lettercode ='J'
				or dateacctlttr ='1-jan-1910') -- created from customer mailing screen.
				and isnull(ExcelGen,0) = 0 -- not going to generate those created from Excel
				and lettercode not in ('GR','GX','GC') -- exclude giro letters which are done below
				group by lettercode
   
				OPEN letter_cursor
				FETCH NEXT FROM letter_cursor INTO @lettercode

				WHILE (@@fetch_status <> -1)
				BEGIN
					IF (@@fetch_status <> -2)
   						begin
					begin tran
					 -- here updating letters created from customer mailing screen so that they dont get created more than once
					   update letter set dateacctlttr = getdate() where lettercode = @lettercode and dateacctlttr='1-jan-1910'
							   -- puts letters into temporary_letter table ready for copying out. 
					   --if @lettercode != 'LOAN'
					   if (@lettercode != 'LoanS' and @lettercode != 'LoanP' and @lettercode != 'LoanE' and @lettercode != 'LoanR')		--IP - 21/02/12 - #9601 - UAT89
					   BEGIN
						   execute  dn_lglettergeneration
							 @gen        = 1,

							 @runno      = @runno,
							 @lettercode = @lettercode,
							  @datestart        = @datestart ,
								@datefinish       = @datefinish
						END
						ELSE


						BEGIN
							execute dn_loanlettergeneration
							@gen        = 1,

							 @runno      = @runno,
							 @lettercode = @lettercode,
							  @datestart        = @datestart ,
								@datefinish       = @datefinish
						END


					commit

					set @Database =db_name()          
          
				if (@lettercode != 'LoanE' and @lettercode != 'LoanR' )
				Begin 
				  set @csvfile =  @driveLTR + @lettercode + convert (varchar,@runno) + '.CSV'
               
				  set @statement3 = @execCmdShell +
							  '''' + ' bcp ' + @Database + '.dbo.' + 'temporary_letter out ' +
							  --@csvfile + ' -t, -c -q -T ' + ''''
								@csvfile + @bcpOptions + ''''
              
				  execute sp_executesql @statement3
				End
			else
			-- #17133 Split Loan letters
			Begin
				set @statementCopy = @execCmdShell +
						 '''' + 'copy ' + @systemdrive  + '\loanheader.dat + '
				-- Loan letter SC1
				set @csvfile =  @driveLTR +	@lettercode + 'SC1_' + convert (varchar,@runno) + '.CSV'

				set @statementSC1 = @execCmdShell +
						  '''' + ' bcp ' + @Database + '.dbo.' + 'temporary_LoanletterSC1 out ' +
						  @csvfile + @bcpOptions + ''''
              
				execute sp_executesql @statementSC1
				set @datfile =  + @driveLTR + @lettercode + 'SC1_' + convert (varchar,@runno) + '.DAT'
				set @statement = @statementCopy + @csvfile + '  ' + @datfile + '''' 
				execute sp_executesql @statement
				-- Loan letter SC2
				set @csvfile =  @driveLTR +	@lettercode + 'SC2_' + convert (varchar,@runno) + '.CSV'

				set @statementSC2 = @execCmdShell +
						  '''' + ' bcp ' + @Database + '.dbo.' + 'temporary_LoanletterSC2 out ' +
						  @csvfile + @bcpOptions + ''''
              
				execute sp_executesql @statementSC2
				set @datfile =  + @driveLTR + @lettercode + 'SC2_' + convert (varchar,@runno) + '.DAT'
				set @statement = @statementCopy + @csvfile + '  ' + @datfile + '''' 
				execute sp_executesql @statement
				-- Loan letter SC3
				set @csvfile =  @driveLTR +	@lettercode + 'SC3_' + convert (varchar,@runno) + '.CSV'

				set @statementSC3 = @execCmdShell +
						  '''' + ' bcp ' + @Database + '.dbo.' + 'temporary_LoanletterSC3 out ' +
						  @csvfile + @bcpOptions + ''''
              
				execute sp_executesql @statementSC3
				set @datfile =  + @driveLTR + @lettercode + 'SC3_' + convert (varchar,@runno) + '.DAT'
				set @statement = @statementCopy + @csvfile + '  ' + @datfile + '''' 
				execute sp_executesql @statement
				-- Loan letter SC Other (4-9)
				set @csvfile =  @driveLTR +	@lettercode + 'SCO_' + convert (varchar,@runno) + '.CSV'

				set @statementSCO = @execCmdShell +
						  '''' + ' bcp ' + @Database + '.dbo.' + 'temporary_LoanletterSCO out ' +
						  @csvfile + @bcpOptions + ''''
              
				execute sp_executesql @statementSCO
				set @datfile =  + @driveLTR + @lettercode + 'SCO_' + convert (varchar,@runno) + '.DAT'
				set @statement = @statementCopy + @csvfile + '  ' + @datfile + '''' 
				execute sp_executesql @statement

			
			End
  
			  /* appending header file onto letter */
			if (@lettercode != 'LoanE' and @lettercode != 'LoanR'  and @lettercode != 'LoanS' and @lettercode != 'LoanP')	--IP - 21/02/12 -  #9601 - UAT89
			   BEGIN
					  set @statement = @execCmdShell +
								 '''' + 'copy ' + @systemdrive  + '\header.dat + ' + @csvfile
				END
			ELSE


				BEGIN
			
					set @statement = @execCmdShell +
						 '''' + 'copy ' + @systemdrive  + '\loanheader.dat + ' + @csvfile;
             
				 END
			if (@lettercode != 'LoanE' and @lettercode != 'LoanR')	-- #17133 LOAN letters split by Status code above
			Begin     
				set @datfile =  @driveLTR +		-- 'd:\users\default\LTR' +
								@lettercode +
								convert (varchar,@runno) + '.DAT';
				set @statement = @statement + '  ' + @datfile + '''' ;
			  execute sp_executesql @statement
			End
   	
	   END
      FETCH NEXT FROM letter_cursor INTO @lettercode
	

   END
   

   CLOSE letter_cursor
   DEALLOCATE letter_cursor
end -- if type ='charges' 

	END  
	
 
--End Configure Sending Letters based on values set in Country Parameter: Dt : 06 Aug 2019 : Zensar

	--IP - 18/07/08 - UAT 5.2 - UAT(24) - Excluding 'COLLECTIONS'   
	declare @dailyorweekly char(1), @counter smallint, @letter_file varchar(32)
	set @counter = 0
	if @type !='CHARGES' AND @type!='PCLUBTIERS' AND @type!= 'COLLECTIONS' and @type! = 'CASHLOANQUAL' -- here we are doing giro letters see above for normal letters --IP - 23/02/12 - #9601 - UAT89
	begin
	while 1 = 1 
	 begin
	
       set @counter=@counter + 1
      

    IF @type ='GIRO' --do Giro letters and change letter code
    BEGIN
      if @counter = 1
        set @lettercode = 'GX'
      else if @counter = 2
        set @lettercode = 'GC'
      else if @counter = 3
        set @lettercode = 'GC'
    END 

      IF @type !='GIRO'
      BEGIN
         SET @lettercode=@type
         SET @counter=3  -- this will force out of the loop after doing one letter the current one
      END

      select @datestart =dateadd(second,10,datelast ) ,
	    @dailyorweekly=dorw from letterdate where lettercode =@lettercode
      set @datefinish = getdate()
      update letterdate set datelast = getdate() where lettercode = @lettercode
       if @dailyorweekly = 'W' --then check if sunday - if so then generate provided none generated recently (4 days ago). Also generate if gap more than 7 days since last letter
       begin
            if  NOT ( (datepart(weekday,getdate()) =1 /* sunday*/  and datediff(day,@datestart,getdate()) > 4 ) OR
                datediff(day,@datestart,getdate()) >  7)
	                continue -- don't generate check next letter
       end     
        -- generates the letter -- puts the data into the table temporary_leter
	    if (@lettercode != 'LoanE' and @lettercode != 'LoanR' and @lettercode != 'LoanS' and @lettercode != 'LoanP')		--IP - 24/02/12 - #9601 - UAT89 
		BEGIN  
		  execute  dn_lglettergeneration
			@gen        = 1,
			@runno      = @runno,
			@lettercode = @lettercode,
			@datestart        = @datestart ,
			@datefinish       = @datefinish
		END  
	    ELSE  
		BEGIN  
			execute dn_loanlettergeneration  
				@gen        = 1,  
				@runno      = @runno,  
				@lettercode = @lettercode,  
				@datestart        = @datestart ,  
				@datefinish       = @datefinish  
	   END
       -- now write the letter to csv file
       EXEC WriteLettertoJulienfileSP @lettercode=@lettercode
	if @counter >=3 
	      break
      end

	  

END


   
   return @return

go





