SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_LetterGetByAcctNo]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_LetterGetByAcctNo]
GO

CREATE PROCEDURE DN_LetterGetByAcctNo
-- ================================================
-- Project      : CoSACS .NET
-- File Name    : DN_LetterGetByAcctNo.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Get Letter details
-- Author       : ??
-- Date         : ??
--
-- This procedure return letter details.
-- 
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 21/09/09  jec UAT534 Include all letter code descriptions
-- =================================================================================
	-- Add the parameters for the stored procedure here
	    	@acctno varchar (12), 
		@serverdbname varchar(40),
		@linktotallyman smallint,
		@return integer output
AS  

	DECLARE @statement SQLText
	
/*	Malaysia probably is not happening so taking this out as need to add status record in. 
	SET @statement = 'SELECT acctno, dateacctlttr, + lettercode,isnull (codedescript,''No description available'') as Description ' +
           + ' into 
			 'FROM letter LEFT JOIN code ON code.category = ''LT1'' and code.code = letter.lettercode ' + 
			 'WHERE acctno = ' + @acctno
	
	IF(@linktotallyman = 1)
	BEGIN
		SET @statement = @statement + ' UNION SELECT Account_Number as acctno, Date as dateacctlttr,' + 
				              'convert(varchar(10), Letter_ID) as lettercode, Letter_Name as Description, '' as statuscode ' +
				              'FROM ' + @serverdbname + '.dbo.TM_Letters '+
				              'WHERE Account_Number = ' + @acctno
	END
*/
			SELECT acctno, dateacctlttr, + lettercode,isnull (codedescript,'No description available') as [Description] , ' ' as StatusCode,
                        isnull(ExcelGen,0) as ExcelGen    -- CR633 jec 20/06/06
			into #letter
			--FROM letter LEFT JOIN code ON code.category = 'LT1' and code.code = letter.lettercode 
			FROM letter LEFT JOIN code ON code.category like 'LT%' and code.code = letter.lettercode	-- UAT534 jec 21/05/09
			WHERE acctno =  @acctno
	
			update #letter set [description] =[description] + ' Pending ' where dateacctlttr <='1-jan-1910'

			update #letter set statuscode = status.statuscode
			from status 
		   where  #letter.acctno = status.acctno and dateadd(minute,2,status.datestatchge) > #letter.dateacctlttr and
			dateadd(minute,-2,status.datestatchge) < #letter.dateacctlttr 
         and #letter.lettercode in ('J','K','U','R','RF','JF','UF','Q','QF','C2','C4','C5','L')
         
         -- return data
			select * from #letter

	SELECT @return = @@error

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

-- End End End End End End End End End End End End End End End End End End End End End End End End End End End End End End

/*
declare @P1 int
set @P1=0
exec DN_LetterGetByAcctNo @acctno = '720004536121', @serverdbname = '', @linktotallyman = 0, @return = @P1 output
select @P1


*/