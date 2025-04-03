IF EXISTS (
		SELECT 1
		FROM SYS.TRIGGERS
		WHERE NAME = 'tr_UpdateIterestAndFeeInCLAPaymentHistory'
		)
	DROP TRIGGER tr_UpdateIterestAndFeeInCLAPaymentHistory
GO



-- =============================================
-- Author:  Rahul D, Zensar
-- Create date: 17/06/2019
-- Description: Trigger will update intrest in table CLAmortizationPaymentHistory 
--				And also post various cashloan realted transtypes for broker file
-- ============================================="

CREATE TRIGGER tr_UpdateIterestAndFeeInCLAPaymentHistory ON [DBO].[FINTRANS]
AFTER INSERT
AS
BEGIN
	IF EXISTS (
			SELECT 1
			FROM INSERTED
			WHERE transtypecode IN ('INT')
			) --script will execute only for interest transaction type
	BEGIN
		-- Update interest with interest(CLAmortizationPaymentHistory) +transvalue (fintrans), for missed installment period
		UPDATE [dbo].[CLAmortizationPaymentHistory]
		SET [dbo].[CLAmortizationPaymentHistory].interest = [dbo].[CLAmortizationPaymentHistory].interest + INSERTED.transvalue
		FROM INSERTED
		WHERE INSERTED.acctno = [dbo].[CLAmortizationPaymentHistory].acctno
			AND transtypecode = 'INT'
			AND [dbo].[CLAmortizationPaymentHistory].installmentNo = (
				SELECT TOP 1 C.installmentNo
				FROM [dbo].[CLAmortizationPaymentHistory] C
				WHERE C.acctno = INSERTED.acctno
				ORDER BY C.installmentNo DESC
				)

		INSERT INTO CLANewPaymentDetails
		SELECT c.acctno
			,transvalue
			,transtypecode
			,getdate()
			,0
			,dbo.fn_CLAGetTranstypeAcctNo('INT', 1)
		FROM INSERTED I
		JOIN acct C ON I.acctno = C.acctno
			AND i.transtypecode = 'INT'
			AND isAmortized = 1
			AND IsAmortizedOutStandingBal = 1
		
		UNION ALL
		
		SELECT c.acctno
			,- 1 * transvalue
			,transtypecode
			,getdate()
			,0
			,dbo.fn_CLAGetTranstypeAcctNo('INT', - 1)
		FROM INSERTED I
		JOIN acct C ON I.acctno = C.acctno
			AND i.transtypecode = 'INT'
			AND isAmortized = 1
			AND IsAmortizedOutStandingBal = 1
	END

	IF EXISTS (
			SELECT 1
			FROM INSERTED
			WHERE transtypecode IN ('CLD')
			) --script will execute only for CLD transaction type
	BEGIN
		INSERT INTO CLANewPaymentDetails
		SELECT c.acctno
			,transvalue
			,transtypecode
			,getdate()
			,0
			,dbo.fn_CLAGetTranstypeAcctNo('CLD', 1)
		FROM INSERTED I
		JOIN acct C ON I.acctno = C.acctno
			AND i.transtypecode = 'CLD'
			AND isAmortized = 1
			AND IsAmortizedOutStandingBal = 1
		WHERE NOT EXISTS (
				SELECT 1
				FROM CLANewPaymentDetails D
				WHERE D.acctno = C.acctno
					AND D.TranstypeCode = 'CLD'
					AND D.CreditDebitNo = dbo.fn_CLAGetTranstypeAcctNo('CLD', 1)
				)
		
		UNION ALL
		
		SELECT c.acctno
			,- 1 * transvalue
			,transtypecode
			,getdate()
			,0
			,dbo.fn_CLAGetTranstypeAcctNo('CLD', - 1)
		FROM INSERTED I
		JOIN acct C ON I.acctno = C.acctno
			AND i.transtypecode = 'CLD'
			AND isAmortized = 1
			AND IsAmortizedOutStandingBal = 1
		WHERE NOT EXISTS (
				SELECT 1
				FROM CLANewPaymentDetails D
				WHERE D.acctno = C.acctno
					AND D.TranstypeCode = 'CLD'
					AND D.CreditDebitNo = dbo.fn_CLAGetTranstypeAcctNo('CLD', - 1)
				)
		
		UNION ALL
		
		SELECT c.acctno
			,transvalue
			,'BHP'
			,getdate()
			,0
			,dbo.fn_CLAGetTranstypeAcctNo('BHP', 1)
		FROM INSERTED I
		JOIN acct C ON I.acctno = C.acctno
			AND i.transtypecode = 'CLD'
			AND isAmortized = 1
			AND IsAmortizedOutStandingBal = 1
		WHERE NOT EXISTS (
				SELECT 1
				FROM CLANewPaymentDetails D
				WHERE D.acctno = C.acctno
					AND D.TranstypeCode = 'BHP'
					AND D.CreditDebitNo = dbo.fn_CLAGetTranstypeAcctNo('BHP', 1)
				)
		
		UNION ALL
		
		SELECT c.acctno
			,- 1 * transvalue
			,'BHP'
			,getdate()
			,0
			,dbo.fn_CLAGetTranstypeAcctNo('BHP', - 1)
		FROM INSERTED I
		JOIN acct C ON I.acctno = C.acctno
			AND i.transtypecode = 'CLD'
			AND isAmortized = 1
			AND IsAmortizedOutStandingBal = 1
		WHERE NOT EXISTS (
				SELECT 1
				FROM CLANewPaymentDetails D
				WHERE D.acctno = C.acctno
					AND D.TranstypeCode = 'BHP'
					AND D.CreditDebitNo = dbo.fn_CLAGetTranstypeAcctNo('BHP', - 1)
				)
	END

	IF EXISTS (
			SELECT 1
			FROM INSERTED
			WHERE transtypecode IN ('FEE')
			) --script will execute only for FEE transaction type
	BEGIN
		INSERT INTO CLANewPaymentDetails
		SELECT c.acctno
			,transvalue
			,transtypecode
			,getdate()
			,0
			,dbo.fn_CLAGetTranstypeAcctNo('FEE', 1)
		FROM INSERTED I
		JOIN acct C ON I.acctno = C.acctno
			AND i.transtypecode = 'FEE'
			AND isAmortized = 1
			AND IsAmortizedOutStandingBal = 1
		
		UNION ALL
		
		SELECT c.acctno
			,- 1 * transvalue
			,transtypecode
			,getdate()
			,0
			,dbo.fn_CLAGetTranstypeAcctNo('FEE', - 1)
		FROM INSERTED I
		JOIN acct C ON I.acctno = C.acctno
			AND i.transtypecode = 'FEE'
			AND isAmortized = 1
			AND IsAmortizedOutStandingBal = 1
	END

	IF EXISTS (
			SELECT 1
			FROM INSERTED
			WHERE transtypecode IN (
					'CLW'
					,'CLA'
					,'CLP'
					,'SCC'
					,'INC'
					,'CLL'
					,'CLC'
					,'CLR'
					)
			) --script will execute only for GFT transaction type
	BEGIN
		INSERT INTO CLANewPaymentDetails
		SELECT c.acctno
			,transvalue
			,transtypecode
			,getdate()
			,0
			,CASE 
				WHEN transvalue >= 0
					THEN dbo.fn_CLAGetTranstypeAcctNo(transtypecode, 1)
				WHEN transvalue < 0
					THEN dbo.fn_CLAGetTranstypeAcctNo(transtypecode, 1)
				END
		FROM INSERTED I
		JOIN acct C ON I.acctno = C.acctno
			AND i.transtypecode IN (
				'CLW'
				,'CLA'
				,'CLP'
				,'SCC'
				,'INC'
				,'CLL'
				,'CLC'
				,'CLR'
				)
			AND isAmortized = 1
			AND IsAmortizedOutStandingBal = 1
		
		UNION ALL
		
		SELECT c.acctno
			,- 1 * transvalue
			,transtypecode
			,getdate()
			,0
			,CASE 
				WHEN - 1 * transvalue >= 0
					THEN dbo.fn_CLAGetTranstypeAcctNo(transtypecode, - 1)
				WHEN - 1 * transvalue < 0
					THEN dbo.fn_CLAGetTranstypeAcctNo(transtypecode, -1)
				END
		FROM INSERTED I
		JOIN acct C ON I.acctno = C.acctno
			AND i.transtypecode IN (
				'CLW'
				,'CLA'
				,'CLP'
				,'SCC'
				,'INC'
				,'CLL'
				,'CLC'
				,'CLR'
				)
			AND isAmortized = 1
			AND IsAmortizedOutStandingBal = 1
		ORDER BY ACCTNO
	END
END