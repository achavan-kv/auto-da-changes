-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
-- Related to issue: #13716 - CR12949


IF EXISTS(select * from codecat where category = 'RDYAST')
BEGIN

	UPDATE codecat set usermaint = 'Y',
					   ReferenceHeaderText = 'Contract Length',
					   CodeHeaderText = 'Ready Assist Items'
	WHERE category = 'RDYAST'

	UPDATE code 
	SET reference = '12'
	WHERE category = 'RDYAST' and code = 'READY1'

	UPDATE code 
	SET reference = '24'
	WHERE category = 'RDYAST' and code = 'READY2'

	UPDATE code 
	SET reference = '36'
	WHERE category = 'RDYAST' and code = 'READY3'

END

