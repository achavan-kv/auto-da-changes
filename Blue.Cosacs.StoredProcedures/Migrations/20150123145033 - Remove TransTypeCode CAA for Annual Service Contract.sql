-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF EXISTS(select * from transtype where transtypecode = 'CAA')
BEGIN
    delete from transtype where transtypecode = 'CAA'
END
GO