IF EXISTS (SELECT * FROM sysobjects
			WHERE xtype = 'P'
			AND name = 'StoreCardPaymentDetailsSave')
BEGIN
	DROP PROCEDURE StoreCardPaymentDetailsSave
END
GO