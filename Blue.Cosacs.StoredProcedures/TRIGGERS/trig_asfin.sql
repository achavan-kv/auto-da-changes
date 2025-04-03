
IF EXISTS (SELECT * FROM sysobjects 
		   WHERE  type = 'TR' AND name = 'trig_asfin')
DROP TRIGGER trig_asfin
GO

CREATE TRIGGER trig_asfin
ON fintrans
FOR INSERT
AS 

    DECLARE @new_acctno char(12)
    DECLARE @new_datetrans datetime
    DECLARE @new_transtypecode varchar(3)
    DECLARE @new_transvalue float
    DECLARE @new_transrefno integer
    DECLARE @new_branchno integer

    SELECT  @new_acctno        = acctno,
            @new_datetrans     = datetrans,
            @new_transtypecode = transtypecode,
            @new_transvalue    = transvalue,
            @new_transrefno    = transrefno,
            @new_branchno      = branchno
    FROM inserted 

    -- Check rowcount otherwise this can pass a null transrefno
    IF (@@ROWCOUNT > 0)
    BEGIN
        EXECUTE sp_as400fin @account   = @new_acctno,
                            @datetran  = @new_datetrans,
                            @trancode  = @new_transtypecode,
                            @tranvalue = @new_transvalue,
                            @tranno    = @new_transrefno,
                            @bno       = @new_branchno
    END
GO
