SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[dn_lgwriteletter]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[dn_lgwriteletter]
GO



CREATE procedure dn_lgwriteletter @lettercode varchar (10),@runno smallint as
 declare   @fn_string    varchar (500),@letter_file varchar (500)

if exists (select * from
information_schema.tables
where table_name = 'temporary_letter')
update #tempaccts set instalamount= 1 where instalamount = 0

--IP - 18/12/2007 - More efficient to use truncate
--delete from temporary_letter
--TRUNCATE TABLE temporary_letter
DROP TABLE temporary_letter


		--insert 
	--(prtacctno,printname,title,name,
	--branchname,branchaddr1,branchaddr2,branchaddr3,
	--branchpocode,telno,cusaddr1,cusaddr2,
	--cusaddr3,cuspocode,dateacctopen,highststatus,
	--outstbal,instalment,datefirst,DueDay,
	--arrsoverinst,charges,arrears,arrcharge,
	--datenextdue,dateacctlttr,creditavailable,combinedinstalment,
	--addtoinstalment,currstatus,availablespend,datelastfreeinstalment,
	--valuelastfreeinstalment,datenextfreeinstalment,valuenextfreeinstalment,totalcashspend,
	--dateintotier2)
	

      select  prtacctno, printname  , title   , name    ,
	 branchname   , branchaddr1  , branchaddr2 , branchaddr3 ,
	 branchpocode , telno    , cusaddr1  , cusaddr2  ,
	 cusaddr3  , cuspocode  , 
	convert (varchar,dateacctopen, 105) as dateacctopen,	 highststatus ,
        convert (varchar,outstbal , 0) as outstbal , convert (varchar,instalamount,0) as instalment ,convert(varchar,datefirst, 105) as [datefirst], 
		day(datefirst) as dueday ,convert (varchar,isnull (arrears/instalamount,0), 0) as arrsoverinst,
		convert (varchar,charges, 0) as charges,convert (varchar,arrears , 0) as arrears, 
		convert (varchar,isnull(arrears,0) + isnull(charges,0), 0) as arrcharge,
        convert (varchar,datenextdue , 105) as datenextdue, convert (varchar,dateacctlttr, 105) as dateacctlttr ,
		convert (varchar,creditavailable , 0) as creditavailable, convert (varchar,combinedinstalment, 0) as combinedinstalment,
	convert (varchar,addtoinstalment, 0) as addtoinstalment, currstatus,availablespend,datelastfreeinstalment,
	valuelastfreeinstalment,datenextfreeinstalment,valuenextfreeinstalment,totalcashspend,
	isnull(dateintotier2,'1-jan-1900') as dateintotier2

	into temporary_letter
	
-- Privilege Club Tier1/2

 from   #tempaccts
/*    commit
    select @fn_string =  country.systemdrive + '\LTR' + @lettercode + convert (VARCHAR,@runno) + '.DAT'
    from country
    set @letter_file = @fn_string
    declare @statement varchar(256)
    select @statement = 'bcp ' + db_name() + '.dbo.' + 'temporary_letter out ' +
       @letter_file + ' -t, -c -q -T'
    --select @statement
    execute master.dbo.xp_cmdshell @statement*/

GO
SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO

