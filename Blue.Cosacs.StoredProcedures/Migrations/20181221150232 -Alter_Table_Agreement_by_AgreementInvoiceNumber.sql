
IF NOT EXISTS (SELECT * FROM   sys.columns WHERE  object_id = OBJECT_ID(N'[Agreement]') AND name = 'AgreementInvoiceNumber')
BEGIN
/*Update Agreement Table with  AgreementInvoiceNumber column */
ALTER TABLE Agreement
ADD AgreementInvoiceNumber nvarchar(14);
END

