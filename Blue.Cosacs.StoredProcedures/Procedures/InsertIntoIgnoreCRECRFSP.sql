
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT * FROM SYSOBJECTS 
           WHERE NAME = 'InsertIntoIgnoreCRECRFSP'
           AND xtype = 'P')
BEGIN 
DROP PROCEDURE InsertIntoIgnoreCRECRFSP
END
GO

CREATE PROCEDURE InsertIntoIgnoreCRECRFSP
(
    @Acctno VARCHAR(12),
    @ContractNo VARCHAR(10),
    @StockLocn int,
    @return int OUTPUT
)

AS
BEGIN
	INSERT INTO IgnoreCRECRF(AcctNo, ContractNo, StockLocn)
    SELECT @Acctno, @ContractNo, @StockLocn
END

SELECT @return = @@ERROR

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO
