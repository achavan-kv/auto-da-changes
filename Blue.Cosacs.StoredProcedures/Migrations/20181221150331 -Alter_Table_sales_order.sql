IF NOT EXISTS (SELECT * FROM  sys.columns WHERE  object_id = OBJECT_ID(N'[sales].[order]') AND name = 'AgreementInvoiceNumber')
BEGIN
ALTER TABLE [sales].[order]
ADD AgreementInvoiceNumber nvarchar(14);
END