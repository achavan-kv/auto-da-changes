-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here



IF EXISTS(select * from codecat where category = 'RDYAST')
BEGIN
	
	UPDATE 
		CodeCat
	SET	
		AdditionalHeaderText = 'Coverage Value'
	WHERE 
		category = 'RDYAST'

END

IF EXISTS(select * from code where category = 'RDYAST')
BEGIN

	UPDATE 
		code
	SET 
		additional = 68.99
	WHERE 
		code = 'READY1' and category = 'RDYAST'

	UPDATE 
		code
	SET 
		additional = 65.54
	WHERE 
		code = 'READY2' and category = 'RDYAST'


	UPDATE 
		code
	SET 
		additional = 62.09
	WHERE 
		code = 'READY3' and category = 'RDYAST'
END