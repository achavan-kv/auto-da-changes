SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[nexttransrefno]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[nexttransrefno]
GO



CREATE procedure nexttransrefno  @branchno smallint = 0, 
    @numrefs    smallint = 0  
as 
declare
    @next_ref_no   integer, 
    @notes    varchar(256) 
    set @next_ref_no = 0;
    set @notes = " ";

    if 1 = 0
        BEGIN

        set @notes = ' alex changed this to allow multiple transrefnos to be allocated  to  prevent deadlocks innew account screen 
                         from multiple transactions  processed 
                        as addtos. it accepts @numrefs  for the number ofN' +  ' transrefnos to increase the branch by '; 

         END


   if @numrefs = 0
         BEGIN

                /* DJT 25-04-2000 Added begin tran to solve transaction count problems */         
                begin tran;

                update branch 
                set hirefno = hirefno + 1 
                where branchno = @branchno; 

                /* ICW 3.0 Corrected syntax */
                SELECT @next_ref_no = hirefno
                FROM   branch
                WHERE  branchno = @branchno;

                commit; 

                return @next_ref_no; 

           END
    ELSE
          
                BEGIN

                /* DJT 25-04-2000 Added begin tran to solve transaction count problems */         
                begin tran;

                update branch 
                set hirefno = hirefno + @numrefs 
                where branchno = @branchno; 

                /* ICW 3.0 Corrected syntax */
                SELECT @next_ref_no = hirefno
                FROM   branch
                WHERE  branchno = @branchno;
 
                set @next_ref_no = @next_ref_no + 1 - @numrefs; 
                commit; 

                 return @next_ref_no; 

                 END


RETURN



GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

