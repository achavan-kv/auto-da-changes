SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[dbnewauth]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[dbnewauth]
GO


/*
**
** Notes:
** 
** History:
** Who   When        Why  
** ICW   30/05/2001  Correction to procdure, using ingres syntax of 'new'
*/

CREATE PROCEDURE dbnewauth   @acctno  char(12)
AS
DECLARE @status  integer,
        @norules integer

        DELETE 
        FROM  delauthorise 
        WHERE acctno = @acctno;

        SET @status = @@error;

        if @status !=0 
        BEGIN
           raiserror ('Error 72: delauthorise delete failed',1,72);
        END

        INSERT INTO delauthorise(
               acctno, 
               datein) 
        VALUES (
               @acctno,
               GETDATE());

      set @status = @@error; 
      if @status != 0
      BEGIN
         raiserror ('Error 71: delauthorise insert failed',1,71);
	END
RETURN


GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

