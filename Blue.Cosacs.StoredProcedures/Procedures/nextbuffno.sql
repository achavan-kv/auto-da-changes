SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[nextbuffno]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[nextbuffno]
GO



CREATE procedure  nextbuffno  @branchno smallint = 0 
as 
declare
    @next_buff_no   integer 

    /* DJT 25-04-2000 Added begin tran to solve transaction count problems */         
    begin tran;

    set @next_buff_no = 0;

    update branch 
    set  hibuffno = hibuffno + 1 
    where branchno = @branchno; 

    /* ICW 3.0 Corrected syntax */
    SELECT @next_buff_no = hibuffno
    FROM   branch
    WHERE  branchno = @branchno;

    commit; 

    return @next_buff_no; 

RETURN



GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

