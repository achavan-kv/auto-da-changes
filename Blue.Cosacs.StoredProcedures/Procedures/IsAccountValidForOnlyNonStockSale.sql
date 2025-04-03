IF EXISTS (
		SELECT 1
		FROM SYS.PROCEDURES WITH (NOLOCK)
		WHERE NAME = 'IsAccountValidForOnlyNonStockSale'
			AND TYPE = 'P'
		)
	DROP PROCEDURE [dbo].[IsAccountValidForOnlyNonStockSale]
GO

-- ===================================================================================
-- Procedure Name: [dbo].[IsAccountValidForOnlyNonStockSale]
-- Author:		Ashwini Akula
-- Create date: 03/02/2019
-- Description:	Procedure to check if nonstock is delivered for the given account  
-- ===================================================================================
CREATE PROCEDURE [dbo].[IsAccountValidForOnlyNonStockSale] 
	 @AcctNo VARCHAR(12)
	,@IsDel INT = 0
	,@IsNonStock INT = 0
	,@return INT OUT
AS
BEGIN
	SET @return = 0

	SET @IsDel = 0
	SET @IsNonStock = 0

	-- check if the only nonstock items are delivered on the respective account
	IF EXISTS (
				SELECT	1
				FROM	NonStockAccounts nsa
						INNER JOIN Delivery d 
							ON nsa.acctno = d.acctno
				WHERE	nsa.acctno = @AcctNo
			)
	BEGIN
		SET @IsDel = 1
	END

	--check if the account is nonstock only account
	IF EXISTS (
			SELECT 1
			FROM NonStockAccounts
			WHERE acctno = @AcctNo
			)
	BEGIN
		SET @IsNonStock = 1
	END

	SELECT	@IsDel AS Isdel
			,@IsNonStock AS IsNonStock
END