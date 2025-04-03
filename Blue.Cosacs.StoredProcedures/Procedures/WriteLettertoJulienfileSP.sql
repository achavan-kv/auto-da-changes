IF EXISTS (SELECT * FROM sysobjects WHERE NAME ='WriteLettertoJulienfileSP')
   DROP PROCEDURE WriteLettertoJulienfileSP
GO
CREATE PROCEDURE WriteLettertoJulienfileSP @lettercode VARCHAR(10)
/*Procedure to write Julien dated letter to file*/
-- ================================================
-- Project      : CoSACS .NET
-- File Name    : WriteLettertoJulienfileSP.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Write Julian dated letter to file.
-- Author       : Alex Ayscough
-- Date         : ?
--
-- Procedure to write Julien dated letter to file
-- 
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 04/12/07 JEC Correct determination of Julian Date.
-- ================================================
	-- Add the parameters for the stored procedure here
as

declare  @statement sqltext,@datfile VARCHAR (200),@statement3 sqltext,
@dateletter datetime,@datestart datetime,@datefinish DATETIME,@letter_file VARCHAR(256)
,@month  smallint ,@year  smallint ,@refdate  datetime ,@day_number  smallint ,
     @ddd_str  varchar(6) 
     -- work out the name of the letter file matches what OpenROAD used to do.
     set @month = datepart(month,getdate());
     set @year= datepart(year,getdate());
     set @refdate=  convert(datetime,('01-jan-' + convert(varchar,@year)))
     -- we are creating a julien date i.e. 1 for 1-jan - 365 for 31st dec
     --set  @day_number = datediff(day,@refdate, getdate()) + 2
	 set  @day_number = datediff(day,@refdate, getdate()) + 1	-- jec 04/12/07	
	
	-- Pad out day numbers <100 to 3 chars
     if @day_number < 10 --THEN
         set @ddd_str = '00' + convert(varchar,@day_number)
     else if @day_number < 100 --THEN
         set @ddd_str = '0' + convert(varchar,@day_number)
     else
         set @ddd_str = convert(varchar,@day_number)

     select @letter_file =  country.systemdrive + '\lt' + @lettercode + @ddd_str + '.csv' from country
     select @datfile =  country.systemdrive + '\lt' + @lettercode + @ddd_str + '.dat' from country

     set @statement =db_name()

     /*copy the letter out to a flat file */ 
      declare @country char(1)
      select @country = countrycode from country 
      if @country ='H' -- will use -n for native characters 
        set @statement3 = 'execute Master.dbo.xp_cmdshell ' +
                    '''' + ' bcp ' + @statement + '.dbo.' + 'temporary_letter out ' +
--                    @letter_file + ' -t, -n -q -USA -P ' + '''';	 SC UAT 254 7/12/07
					  @letter_file + ' -t, -n -q -T ' + '''';
      else -- will use - c for characters
		  set @statement3 = 'execute Master.dbo.xp_cmdshell ' +
                    '''' + ' bcp ' + @statement + '.dbo.' + 'temporary_letter out ' +
--                    @letter_file + ' -t, -c -q -USA -P ' + '''';	 SC UAT 254 7/12/07
					  @letter_file + ' -t, -c -q -T ' + '''';	
      execute sp_executesql @statement3

     /* appending header file onto letter */
	 if @lettercode != 'LOAN'
		   BEGIN
				  set @statement = 'execute Master.dbo.xp_cmdshell ' +
							 '''' + 'copy d:\users\default'  +
							 '\header.dat + ' +  @letter_file;
			END
		ELSE
			BEGIN
				set @statement = 'execute Master.dbo.xp_cmdshell ' +
                     '''' + 'copy d:\users\default'  +
                     '\loanheader.dat + ' +  @letter_file;
             
			 END
     --set @statement = 'execute Master.dbo.xp_cmdshell ' +
     --           '''' + 'copy d:\users\default'  +
     --           '\header.dat + ' + @letter_file;
        
        set @statement = @statement + '  ' + @datfile + '''' ;
        execute sp_executesql @statement
go


