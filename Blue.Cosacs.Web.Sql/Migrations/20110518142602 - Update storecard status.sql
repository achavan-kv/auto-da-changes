DELETE FROM storecardstatus
WHERE [STATUS] = 'I'

IF NOT EXISTS (SELECT * FROM StoreCardStatus
				WHERE Status = 'AA')
BEGIN
	INSERT INTO storecardstatus
	([STATUS], [DESCRIPTION])
	SELECT 'AA','Awaiting Activation' UNION ALL
	SELECT 'A','Active'
END