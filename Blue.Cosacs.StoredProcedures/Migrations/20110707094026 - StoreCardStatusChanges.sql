DELETE FROM StoreCardStatus_Lookup
WHERE status IN ('LS','NA','TBR','S')

SELECT * 
INTO StoreCardCardStatus_Lookup
FROM StoreCardStatus_Lookup

DROP TABLE StoreCardStatus_Lookup

CREATE TABLE StoreCardAccountStatus_Lookup
(
	Status VARCHAR(5) NOT NULL,
	Description varchar(50) 
)

INSERT INTO StoreCardAccountStatus_Lookup
SELECT 'S', 'Suspended' UNION ALL
SELECT 'TBI', 'To be issued' UNION ALL
SELECT 'AA', 'Awaiting Activation' UNION ALL
SELECT 'A', 'Active' UNION ALL
SELECT 'B', 'Blocked' UNION ALL
SELECT 'C', 'Cancelled'


