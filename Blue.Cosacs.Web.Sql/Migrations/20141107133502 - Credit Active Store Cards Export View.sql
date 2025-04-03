-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

CREATE VIEW [Credit].[ActiveStoreCardsView]
AS

    SELECT CAST(vSc.CardNumber AS VARCHAR) AS cardNumber,
           CAST(vSc.CardName AS VARCHAR) nameOnCard,
           CAST(vSc.StoreCardAvailable AS VARCHAR) AS availableSpend,
           CAST(vSc.Acctno AS VARCHAR) AS accountNumber
    FROM View_StoreCardWithPayments vSc
    WHERE LTRIM(RTRIM(CardStatus)) = 'A'
GO