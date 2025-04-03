SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO

IF EXISTS (SELECT * FROM SysObjects
           WHERE Name = 'DN_instalmentvariableGetbyacctnoSP' AND Type = 'P')
BEGIN
    DROP PROCEDURE DN_instalmentvariableGetbyacctnoSP
END
GO

CREATE PROCEDURE DN_instalmentvariableGetbyacctnoSP
@acctno char(12),
@return integer output
AS 
  set nocount on
  SET @return = 0

  select 
      acctno,
      instalorder,
      instalment as instalamount,
      instalmentnumber,
      datefrom,
      dateto,
      servicecharge
      from instalmentvariable
      where acctno = @acctno

      set @return = @@error

go
