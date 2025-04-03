-- Performance Trigger so that Telephone Action screen retrieves data quickly. 
IF EXISTS (SELECT * FROM sysobjects WHERE NAME= 'tr_bailactionMaxAction')
DROP TRIGGER tr_bailactionMaxAction
GO 
CREATE TRIGGER tr_bailactionMaxAction ON bailaction FOR INSERT,UPDATE 

AS  UPDATE M 
SET RecentCode = b.code,
dateadded = b.dateadded
FROM  bailactionMaxAction M, inserted  b ,code c 
WHERE m.acctno= b.acctno AND 
b.code = c.code AND c.category ='FUA' 
AND c.additional = 'Y' -- this is a positive action 

-- insert where missing 
INSERT INTO bailactionMaxAction (
	Acctno ,
	dateadded,
	RecentCode) 
SELECT d.acctno, MAX(d.dateadded) AS dateadded,
d.code 
FROM inserted d ,code c 
WHERE NOT EXISTS (SELECT * FROM bailactionMaxAction b 
WHERE b.acctno = d.acctno AND b.dateadded = d.dateadded) 
AND d.code = c.code AND c.category ='FUA' 
AND c.additional = 'Y' -- this is a positive action 
GROUP BY d.acctno,d.code 
GO
