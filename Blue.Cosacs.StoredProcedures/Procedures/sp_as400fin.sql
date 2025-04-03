SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[sp_as400fin]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[sp_as400fin]
GO



create procedure sp_as400fin @account char(12), 
    @tranno   integer, 
    @datetran datetime, 
    @trancode   varchar(3), 
    @tranvalue  float, 
    @bno    integer  
as 
    insert into  as400fin (acctno, 
        datetrans, 
        transtypecode, 
        transvalue, 
        transrefno, 
        origbr) 
    values (@account, 
        @datetran, 
        @trancode, 
        @tranvalue, 
        @tranno, 
        @bno); 

RETURN


GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

