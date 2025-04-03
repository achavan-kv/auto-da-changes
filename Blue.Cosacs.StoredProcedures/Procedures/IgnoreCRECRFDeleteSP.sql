
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT * FROM SYSOBJECTS 
           WHERE NAME = 'IgnoreCRECRFDeleteSP'
           AND xtype = 'P')
BEGIN 
DROP PROCEDURE IgnoreCRECRFDeleteSP
END
GO

CREATE PROCEDURE IgnoreCRECRFDeleteSP
(
    @Acctno VARCHAR(12),
    @ContractNo VARCHAR(10),
    @StockLocn int,
    @return int OUTPUT
)

AS
BEGIN
	DELETE FROM IgnoreCRECRF
    WHERE
        AcctNo = @Acctno
        and ContractNo = @ContractNo
        and StockLocn = @StockLocn
END

SELECT @return = @@ERROR

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO
