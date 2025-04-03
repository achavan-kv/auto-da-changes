IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.tables WHERE table_name  ='View_StoreCardHistory')
DROP VIEW View_StoreCardHistory 
GO

CREATE VIEW View_StoreCardHistory   
AS SELECT sc.Acctno,s.CardNumber,s.DateChanged,s.Notes,
s.StatusCode,s.Empeeno,
c.FullName AS EmployeeName, sl.Description AS Status  ,ISNULL(co.codedescript,sl.Description) AS Reason,co.code AS Code
FROM storecardstatus s   
JOIN StoreCard SC ON sc.CardNumber = s.cardNumber    
JOIN Admin.[User] c ON c.id = s.empeeno  
JOIN StoreCardCardStatus_Lookup sl ON sL.Status= s.StatusCode
LEFT JOIN bailaction b ON b.acctno = sc.acctno AND b.dateadded = s.DateChanged
LEFT JOIN code co ON b.code = co.code AND co.category ='STCR'
GO 