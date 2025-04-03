
IF NOT EXISTS (SELECT * FROM   sys.columns WHERE  object_id = OBJECT_ID(N'[branch]') AND name = 'InvoiceSequenceNumber')
BEGIN
/*Add Column in Branch Table */
ALTER TABLE branch
ADD InvoiceSequenceNumber nvarchar(7);
END
