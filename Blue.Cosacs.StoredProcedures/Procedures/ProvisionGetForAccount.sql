IF EXISTS (SELECT * FROM sysobjects
           WHERE name = 'ProvisionGetForAccount'
           AND xtype = 'P')
BEGIN
	DROP PROCEDURE ProvisionGetForAccount
END
GO

CREATE PROCEDURE ProvisionGetForAccount
@acctno VARCHAR(12),
@return int output

AS
BEGIN
	

	SELECT provision 
	FROM View_Provision
	WHERE View_Provision.acctno = @acctno

	set @return = 0
END