IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID('[dbo].[fn_CLAGetTranstypeAcctNo]') AND xtype in ('FN', 'IF', 'TF'))
	DROP FUNCTION [dbo].[fn_CLAGetTranstypeAcctNo]
GO
----------------------------------------------------------
-- Script Name	: fn_CLAGetTranstypeAcctNo
-- Parameters	: 1. TranstypeCode - String, 
--				  2. CereditOrDebit - INT (-1 for Credit, 1 for Debit)
-- Details		: Returns the Credit or Debit Account No for transaction
-- Date			: 20/08/2019
-- Author		: Rahul D, Zensar
-- Version		: 1.0
-- Change Ctrl	:	Date			--	Author	--	Details
--					20/08/19		--	  RD	--	New Script Created to return Credit or Debit Account No for transaction
--------------------------------------------------------------------
CREATE FUNCTION fn_CLAGetTranstypeAcctNo(@TranstypeCode VARCHAR(12), @CereditOrDebit INT)
RETURNS  INT
AS
BEGIN
	DECLARE @AcctNo INT
	IF (@CereditOrDebit = -1) -- In Case of Credit transaction
	BEGIN
		SELECT @AcctNo = isnull(interfacebalancing,0) FROM transtype WHERE transtypecode = @TranstypeCode
	END
	IF (@CereditOrDebit = 1) -- In Case of Debit transaction
	BEGIN
		SELECT @AcctNo = isnull(interfaceaccount,0) FROM transtype WHERE transtypecode = @TranstypeCode
	END
	RETURN isnull(@AcctNo,0)
END