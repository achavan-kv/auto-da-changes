SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_ProposalRefSaveSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_ProposalRefSaveSP]
GO


CREATE PROCEDURE  dbo.DN_ProposalRefSaveSP
            @acctno varchar(12),
            @refno tinyint,
            
            @name varchar(30),
            @surname varchar(35),
            @relation varchar(4),
            @yrsknown int,
            
            @address1 varchar(26),
            @address2 varchar(26),
            @city varchar(26),
            @postcode varchar(10),
            
            @waddress1 varchar(26),
            @waddress2 varchar(26),
            @wcity varchar(26),
            @wpostcode varchar(10),
            
            @telcode varchar(8),
            @tel varchar(13),
            
            @wtelcode varchar(8),
            @wtel varchar(13),
            
            @mtelcode varchar(8),
            @mtel varchar(13),

            @directions varchar(300),
            @comment varchar(300),
            
            @empeeno int,
            @datechecked datetime,
            
            @return int OUTPUT
AS

    SET @return = 0        --initialise return code
    
    UPDATE proposalref
    SET    name      = @name,
           surname   = @surname,
           relation  = @relation,
           yrsknown  = @yrsknown,
           
           address1  = @address1,
           address2  = @address2,
           city      = @city,
           postcode  = @postcode,
           
           waddress1 = @waddress1,
           waddress2 = @waddress2,
           wcity     = @wcity,
           wpostcode = @wpostcode,
           
           telcode   = @telcode,
           tel       = @tel,
           wtelcode  = @wtelcode,
           wtel      = @wtel,
           mtelcode  = @mtelcode,
           mtel      = @mtel,

           directions = @directions,
           comment    = @comment,
           empeenochange = @empeeno,
           datechange    = @datechecked
    WHERE acctno = @acctno
    AND   refno  = @refno

    IF (@@rowcount = 0 AND @@error = 0)
    BEGIN
        INSERT INTO proposalref
            (acctno,
             refno,
             name,
             surname,
             relation,
             yrsknown,
             
             address1,
             address2,
             city,
             postcode,
             
             waddress1,
             waddress2,
             wcity,
             wpostcode,
             
             telcode,
             tel,
             wtelcode,
             wtel,
             mtelcode,
             mtel,

             directions,
             comment,
             empeenochange,
             datechange)
        VALUES
            (@acctno,
             @refno,
             @name,
             @surname,
             @relation,
             @yrsknown,
             
             @address1,
             @address2,
             @city,
             @postcode,
             
             @waddress1,
             @waddress2,
             @wcity,
             @wpostcode,
             
             @telcode,
             @tel,
             @wtelcode,
             @wtel,
             @mtelcode,
             @mtel,
             
             @directions,
             @comment,
             @empeeno,
             @datechecked)
    END

    IF (@@rowcount = 0) SET @return = -1

    IF (@@error != 0) SET @return = @@error
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

