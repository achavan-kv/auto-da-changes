-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
-- Related to issue: #12560

IF EXISTS(SELECT * FROM code WHERE category = 'SRSERVLCN')
BEGIN
	DELETE FROM codecat WHERE category = 'SRSERVLCN'
	
	DELETE FROM code WHERE category = 'SRSERVLCN'
END


IF EXISTS(SELECT * FROM code WHERE category = 'SRSERVACT')
BEGIN
	DELETE FROM codecat WHERE category = 'SRSERVACT'
	
	DELETE FROM code WHERE category = 'SRSERVACT'
END

IF EXISTS(SELECT * FROM code WHERE category = 'SRSUPPLIER')
BEGIN
	DELETE FROM codecat WHERE category = 'SRSUPPLIER'
	
	DELETE FROM code WHERE category = 'SRSUPPLIER'
END
