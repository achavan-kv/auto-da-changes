IF EXISTS (
			SELECT 1
			FROM SYS.PROCEDURES WITH (NOLOCK)
			WHERE NAME = 'SaveAccountForSaleOnlyNonStock'
				AND TYPE = 'P'
		)
	DROP PROCEDURE [dbo].[SaveAccountForSaleOnlyNonStock]
GO

-- ===================================================================================
-- Procedure Name:	SaveAccountForSaleOnlyNonStock
-- Author:			Ashwini Akula
-- Create Date:		03/02/2019
-- Description:		Procedure to insert a 'nonstock only' acct in to NonStockAccounts table
-- ===================================================================================

CREATE PROCEDURE [dbo].[SaveAccountForSaleOnlyNonStock] 
	@AcctNo VARCHAR(12)
	,@return INT OUT
AS
BEGIN
	SET @return = 0

	--check if the account is present and insert if not into the nonstock table to differentiate the account
	--as 'nonstock only' account
	IF NOT EXISTS (
					SELECT acctno
					FROM NonStockAccounts
					WHERE acctno = @AcctNo
				  )
	BEGIN
		INSERT INTO NonStockAccounts
		VALUES (@AcctNo)
	END
END