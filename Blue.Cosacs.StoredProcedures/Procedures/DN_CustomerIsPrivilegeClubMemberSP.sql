SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects
where id = object_id('[dbo].[DN_CustomerIsPrivilegeClubMemberSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_CustomerIsPrivilegeClubMemberSP]
GO

CREATE PROCEDURE dbo.DN_CustomerIsPrivilegeClubMemberSP
    @custid             varchar(20),
    @privClubCode       varchar(4) OUTPUT,
    @privClubDesc       varchar(64) OUTPUT,
    @return             int OUTPUT

AS --DECLARE

BEGIN
    SET @return = 0
    SET @privClubCode = null
    SET @privClubDesc = ''

if (select Value from CountryMaintenance where name like 'Loyalty Card')='True'  
BEGIN

    IF EXISTS (SELECT 1 FROM CountryMaintenance
               WHERE  CodeName = 'TierPCEnabled' AND Value = 'True')
    BEGIN
        -- Tier1/2 Privilege club is enabled
        
        SELECT  @privClubCode = MAX(Code)
        FROM    custcatcode
        WHERE   custid = @custid
        AND     ISNULL(DateDeleted,'') = ''
        AND     Code IN ('TIR1', 'TIR2')
    END
    ELSE
    BEGIN
        -- Classic Privilege Club

        SELECT  @privClubCode = Code
        FROM    custcatcode
        WHERE   custid = @custid
        AND     ISNULL(DateDeleted,'') = ''
        AND     Code = 'CLAC'
        AND NOT EXISTS (SELECT  1
                        FROM    custcatcode
                        WHERE   custid = @custid
                        AND     ISNULL(DateDeleted,'') = ''
                        AND     Code IN ('CLAS', 'CLAW'))
    END

    IF (@privClubCode IS NOT NULL)
    BEGIN
        SELECT @privClubDesc = CodeDescript
        FROM   Code
        WHERE  Category = 'CC1'
        AND    Code = @privClubCode
        
        SET @privClubDesc = ISNULL(@privClubDesc,'')
    END
    END

    SET @return = @@error
END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

