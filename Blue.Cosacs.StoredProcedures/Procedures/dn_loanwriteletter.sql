
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[dn_loanwriteletter]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[dn_loanwriteletter]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO
-- ==========================================================================================================
-- Version	: 002
-- Project      : CoSACS .NET
-- File Name    : dn_loanwriteletter.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Credit Collections - Prepares data for loan letters
-- Author       :
-- Date         : 
---- This procedure prepares data for loan letters.
-- 
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 07/08/19 Zensar  Optimised the stored procedure for performance by  replcaing * with 1 in all exist checks.
-- ============================================================================================================

CREATE procedure [dbo].[dn_loanwriteletter] @lettercode varchar (10),@runno smallint as
 declare   @fn_string    varchar (500),@letter_file varchar (500)

if exists (select 1 from
information_schema.tables
where table_name = 'temporary_letter')
update #tempaccts set instalamount= 1 where instalamount = 0

--IP - 18/12/2007 - More efficient to use truncate
--delete from temporary_letter
IF EXISTS(SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[temporary_letter]') AND type in (N'U'))
	DROP TABLE temporary_letter
if (@lettercode='LOANE' or @lettercode = 'LOANR')
	Begin
		IF EXISTS(SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[temporary_LoanletterSC1]') AND type in (N'U'))
			DROP TABLE temporary_LoanletterSC1
		IF EXISTS(SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[temporary_LoanletterSC2]') AND type in (N'U'))
			DROP TABLE temporary_LoanletterSC2
		IF EXISTS(SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[temporary_LoanletterSC3]') AND type in (N'U'))
			DROP TABLE temporary_LoanletterSC3
		IF EXISTS(SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[temporary_LoanletterSCO]') AND type in (N'U'))
			DROP TABLE temporary_LoanletterSCO
	End


    select  prtacctno, CustId,printname  , title   , name    ,
	 branchname   , branchaddr1  , branchaddr2 , branchaddr3 ,
	 branchpocode , telno    , cusaddr1  , cusaddr2  ,
	 cusaddr3  , cuspocode , addtype , homephone, mobile1, workphone, mobile2, deliveryphone, mobile3, convert (varchar,dateacctopen, 105) as dateacctopen,	 highststatus ,
        convert (varchar,outstbal , 0) as outstbal , convert (varchar,instalamount,0) as instalment,convert(varchar,datefirst, 105) as [datefirst], 
        day(datefirst) as dueday ,         convert (varchar,isnull (arrears/instalamount,0) , 0)as arrsoverinst,
        convert (varchar,charges, 0)as charges,convert (varchar,arrears , 0) as arrears,  
        convert (varchar,isnull(arrears,0) + isnull(charges,0), 0) as arrcharge,
        convert (varchar,datenextdue , 105) as datenextdue, convert (varchar,dateacctlttr, 105) as dateacctlttr ,
        convert (varchar,creditavailable , 0) as creditavailable, convert (varchar,combinedinstalment, 0) as combinedinstalment ,
	convert (varchar,addtoinstalment, 0) as addtoinstalment,currstatus,availablespend,datelastfreeinstalment,
	valuelastfreeinstalment,datenextfreeinstalment,valuenextfreeinstalment,totalcashspend,
	isnull(dateintotier2,'1-jan-1900') as dateintotier2
	
	into temporary_letter	
	
-- Privilege Club Tier1/2

 from   #tempaccts


	if (@lettercode='LOANE' or @lettercode = 'LOANR')
	Begin
		Select * into temporary_LoanletterSC1 from temporary_letter where highststatus='1'
		Select * into temporary_LoanletterSC2 from temporary_letter where highststatus='2'
		Select * into temporary_LoanletterSC3 from temporary_letter where highststatus='3'
		Select * into temporary_LoanletterSCO from temporary_letter where highststatus not in('1','2','3')
	End
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

