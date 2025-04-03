SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[dbcustacctadd]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[dbcustacctadd]
GO




create procedure  dbcustacctadd @origbr smallint, 
    @custid  varchar(15) = " ", 
    @acctno char(12) = " ", 
    @hldorjnt char(1) = " "   
as 
declare
    @status integer 
    set @status = 0;
	IF NOT EXISTS (SELECT * FROM custacct WHERE acctno= @acctno AND custid = @custid)
		insert into custacct( origbr, 
			custid, 
			acctno, 
			hldorjnt) 
		values ( @origbr, 
			 @custid, 
			 @acctno, 
			 @hldorjnt); 

    set @status = @@error; 

    return @status; 





GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

