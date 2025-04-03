SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO

IF EXISTS (SELECT * FROM SysObjects
           WHERE Name = 'DN_instalmentvariableSaveSP' AND Type = 'P')
BEGIN
    DROP PROCEDURE DN_instalmentvariableSaveSP
END
GO


CREATE PROCEDURE DN_instalmentvariableSaveSP
@acctno char(12),
@instalorder smallint,
@instalment money ,
@instalmentnumber smallint ,
@datefrom datetime,
@servicecharge money,
@return integer output
AS 
set nocount on
SET @return = 0

-- Need to try an update first

UPDATE instalmentvariable
SET    instalment        = @instalment,
       instalmentnumber  = @instalmentnumber,
       datefrom          = @datefrom
WHERE  AcctNo = @AcctNo
AND    InstalOrder = @InstalOrder

IF (@@ROWCOUNT = 0)
BEGIN
    insert into instalmentvariable
        (acctno,
         instalorder,
         instalment,
         instalmentnumber,
         datefrom,
         servicecharge)
    values ( @acctno,
             @instalorder,
             @instalment,
             @instalmentnumber,
             @datefrom,
         @servicecharge)
   set @return = @@error
END

go
