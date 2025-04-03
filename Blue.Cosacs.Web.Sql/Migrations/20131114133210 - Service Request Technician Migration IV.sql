/*
this migration come from 20130322085502, which was deletect.
so if the original one is already applyed this one should not run
*/
declare @AlredyRunOriginal int = 
(
	SELECT COUNT(*) FROM [$Migration] m WHERE m.Id IN (20130322085502, 20131114133208, 201311141332010)
)

IF @AlredyRunOriginal = 0
BEGIN
	DECLARE
		@Maxrequestno INT, 
		@Maxlo INT, 
		@Requestidend INT, 
		@Maxtestdb INT

	SELECT 
		@Maxrequestno = MAX(Servicerequestno) 
	FROM 
		Sr_Servicerequest

	SELECT
		@Maxlo = Maxlo
	FROM 
		Dbo.Hilo
	WHERE
		Sequence = 'Service.Request'

	-- set last ServiceRequest ID = to multiple of HiLo increment e.g. if highest ServiceRequest ID= 12345 and increment = 50 then Last ServiceRequest id = StartId + 300
	SELECT
		@Requestidend = @Maxrequestno + @Maxlo - @Maxrequestno % @Maxlo

	-- Live version						
	INSERT INTO Service.Technicianbooking 
		(Userid, Requestid, Date, Slot, Slotextend, Reject, Allocatedon)
	SELECT
		T.Newtechid,
		MAX(A.Servicerequestno),
		D.Slotdate,
		MAX
		(
			CASE
				WHEN D.Slotno > 10 THEN RIGHT(D.Slotno, 1)
				ELSE D.Slotno
			END
		),
		0,
		0,
		CASE
			WHEN DATEPART(hour, A.Dateallocated) = '00' THEN A.Dateallocated
			ELSE DATEADD(Hh, DATEDIFF(Hh, GETDATE(), GETUTCDATE()), CAST(A.Dateallocated AS DATETIME))
		END		
	FROM
		Sr_Allocation AS A
		INNER JOIN Sr_Servicerequest AS R 
			ON A.Servicerequestno = R.Servicerequestno
		INNER JOIN Migtechnician AS T 
			ON A.Technicianid = T.Oldtechid
		INNER JOIN Sr_Techniciandiary AS D 
			ON A.Servicerequestno = D.Servicerequestno
	WHERE
		R.Loggedby != 99999 AND
		D.Slotdate = 
		(
			SELECT MAX(Slotdate)
			FROM Sr_Techniciandiary AS D2
			WHERE D2.Servicerequestno = D.Servicerequestno
		) AND EXISTS
		(
			SELECT *
			FROM Service.Request AS Sr
			WHERE Sr.Id = R.Servicerequestno
		)
	GROUP BY
		A.Servicerequestno,
		T.Newtechid,
		D.Slotdate,
		A.Dateallocated				
	ORDER BY
		A.Servicerequestno, Newtechid

	-- Contacts
	INSERT INTO Service.Requestcontact 
		(Requestid, Type, Value)
	SELECT
		C.Servicerequestno,
		'HomePhone',
		LTRIM(REPLACE(Telhome, '  ', ' '))
	FROM
		Sr_Customer AS C
		INNER JOIN Sr_Servicerequest AS R ON
		C.Servicerequestno = R.Servicerequestno
	WHERE 
		Telhome != '' 
		AND R.Loggedby != 99999
	UNION
	SELECT
		C.Servicerequestno,
		'MobilePhone',
		LTRIM(REPLACE(Telmobile, '  ', ' '))
	FROM
		Sr_Customer AS C
		INNER JOIN Sr_Servicerequest AS R ON
		C.Servicerequestno = R.Servicerequestno
	WHERE 
		Telmobile != '' 
		AND R.Loggedby != 99999
	UNION
	SELECT
		C.Servicerequestno,
		'WorkPhone',
		LTRIM(REPLACE(Telwork, '  ', ' '))
	FROM
		Sr_Customer AS C
		INNER JOIN Sr_Servicerequest AS R ON
		C.Servicerequestno = R.Servicerequestno
	WHERE 
		Telwork != '' 
		AND R.Loggedby != 99999

	-- FoodLoss
	INSERT INTO Service.Requestfoodloss 
		(Requestid, Item, Value)
	SELECT
		F.Servicerequestno,
		Itemdescription,
		Itemvalue
	FROM
		Sr_Foodloss AS F
		INNER JOIN Sr_Servicerequest AS R ON
		F.Servicerequestno = R.Servicerequestno
	WHERE
		R.Loggedby != 99999

	-- Comments
	INSERT INTO Service.Comment 
		(Requestid, Date, Addedby, Text)
	SELECT
		R.Servicerequestno,
		[Date],
		U.Fullname,
		C.Comments
	FROM
		Sr_Customerinteraction AS C
		INNER JOIN Sr_Servicerequest AS R 
			ON C.Servicerequestno = R.Servicerequestno
		INNER JOIN Admin.[User] AS U 
			ON C.Empeeno = U.Id
	WHERE
		R.Loggedby != 99999

	-- Request Part
	INSERT INTO Service.Requestpart 
		(Requestid, Partnumber, Parttype, Quantity, Price, Description, Stockbranch)
	SELECT
		R.Servicerequestno,
		CASE
			WHEN R.Partno = '' THEN NULL
			ELSE R.Partno
		END,
		Parttype,
		Quantity,
		R.Unitprice,
		R.Description,
		CASE
			WHEN R.Stocklocn = 0 THEN S.Servicebranchno
			ELSE R.Stocklocn
		END
	FROM
		Sr_Partlistresolved AS R
		INNER JOIN Sr_Servicerequest AS S ON
		R.Servicerequestno = S.Servicerequestno
	WHERE
		S.Loggedby != 99999

	-- SR_Summary

	INSERT INTO Sr_Summary (
		Acctno,
		Servicerequestno,
		Itemid,
		Datelogged,
		Dateclosed,
		Stocklocn,
		Branch)
	SELECT
		Sr.Acctno,
		Sr.Servicerequestno,
		Sr.Itemid,
		DATEADD(Hh, DATEDIFF(Hh, GETDATE(), GETUTCDATE()), CAST(Sr.Datelogged AS DATETIME)),
		CASE
			WHEN Sr.Status = 'C' AND R.Returndate IS NOT NULL AND R.Returndate !< '1910-01-01' THEN R.Returndate
			WHEN Sr.Status = 'C' AND R.Returndate IS NULL THEN
				CASE
					WHEN DATEPART(hour, DATEADD(D, 1, Sr.Datelogged)) = '00' THEN DATEADD(D, 1, Sr.Datelogged)
					ELSE DATEADD(Hh, DATEDIFF(Hh, GETDATE(), GETUTCDATE()), CAST(DATEADD(D, 1, Sr.Datelogged) AS DATETIME))
				END
			WHEN Sr.Status = 'C' AND R.Dateclosed != '1900-01-01' THEN
				CASE
					WHEN DATEPART(hour, R.Dateclosed) = '00' THEN R.Dateclosed
					ELSE DATEADD(Hh, DATEDIFF(Hh, GETDATE(), GETUTCDATE()), CAST(R.Dateclosed AS DATETIME))
				END
			WHEN Sr.Status = 'C' AND R.Dateclosed = '1900-01-01' THEN
				CASE
					WHEN DATEPART(hour, DATEADD(D, 1, Sr.Datelogged)) = '00' THEN DATEADD(D, 1, Sr.Datelogged)
					ELSE DATEADD(Hh, DATEDIFF(Hh, GETDATE(), GETUTCDATE()), CAST(DATEADD(D, 1, Sr.Datelogged) AS DATETIME))
				END
			ELSE '1900-01-01'
		END,
		Sr.Stocklocn,
		Sr.Servicebranchno
	FROM
		Sr_Servicerequest AS Sr
		INNER JOIN Sr_Resolution AS R ON
		Sr.Servicerequestno = R.Servicerequestno
	WHERE
		Sr.Loggedby != 99999 AND
		Sr.Servicetype != 'S' AND
		Sr.Servicetype != 'N'

	-- Script Answer
	INSERT INTO Service.Requestscriptanswer 
		(Requestid, Question, Answer)
	SELECT
		Sr.Servicerequestno,
		SUBSTRING(C.Codedescript, 4, 53),
		'NA'
	FROM Sr_Servicerequest AS Sr, Code AS C
	WHERE
		Sr.Loggedby != 99999 AND
		C.Category = 'SRSCRIPT' AND EXISTS(
										   SELECT *
										   FROM Service.Request AS R
										   WHERE R.Id = Sr.Servicerequestno)				
	ORDER BY
		Sr.Servicerequestno, C.Sortorder

	-- Update HiLo

	SELECT
		@Maxtestdb = (
					  SELECT
						  Nexthi
					  FROM Hilo
					  WHERE
						  Sequence = 'Service.Request')
	IF
		@Maxtestdb > @Requestidend		--  cater for re-runs on test db
		SET @Requestidend = @Maxtestdb + @Maxlo - @Maxtestdb % @Maxlo

	UPDATE Hilo
	SET
		Nexthi = @Requestidend
	WHERE
		Sequence = 'Service.Request' 
END

	