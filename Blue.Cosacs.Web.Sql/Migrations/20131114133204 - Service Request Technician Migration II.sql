IF EXISTS (SELECT * FROM Sys.Tables WHERE Name = 'MergedTechnician')
	DROP TABLE Dbo.Mergedtechnician

IF EXISTS (SELECT * FROM Sys.Tables WHERE Name = 'MigTechnician')
	DROP TABLE Dbo.Migtechnician

-- Technician
CREATE TABLE #Usercheck 
(
	Id INT IDENTITY
	,Userid INT
)
INSERT INTO #Usercheck
SELECT
	Id
FROM Admin.[User]
ORDER BY
	Id

DECLARE @Techuserstart INT = 
(
	SELECT TOP 1 A.Userid
	FROM #Usercheck AS A LEFT JOIN #Usercheck AS B ON A.Id + 1 = B.Id
	WHERE A.Userid > 0 AND B.Userid - A.Userid > (SELECT COUNT(*) FROM Techmigration AS T)
)

SELECT
	Id, 
	Technicians AS Mergedtechnicians,
	@Techuserstart + T.Id AS Newtechnicianid
INTO
	Mergedtechnician
FROM 
	Techmigration AS T
ORDER BY
	Id

-- insert into admin.user
SET IDENTITY_INSERT  Admin.[User] ON

INSERT INTO Admin.[User] 
(Id, Branchno, [Login], [Password], Lastchangepassword, Firstname, Lastname, Externallogin, Legacypassword,Email, Locked, Requirechangepassword, Addressline1, Addressline2, Addressline3, Postcode, Phone, Phonealternate)
SELECT
	@Techuserstart + T.Id, 
	T.Branch, 
	Username, 
	NULL, 
	GETDATE(), 
	REPLACE(T.Firstname, '''', ''), 
	REPLACE(T.Lastname, '''', ''), 
	NULL, 
	113,
	Email, 
	CASE
		WHEN Deleted != '1' THEN 0
		ELSE 1
	END, 
	1, 
	T.Address1, 
	T.Address2, 
	T.Address3, 
	LEFT(T.Addresspc, 10) AS Postcode, 
	T.Phoneno, 
	Mobileno
FROM 
	Techmigration AS T

SET IDENTITY_INSERT Admin.[User] OFF

INSERT INTO Service.Technician 
(Userid, Internal, Starttime, Endtime, Slots)
SELECT
	U.Newtechnicianid, 
	CASE
		WHEN T.Internal = '1' THEN 1
		ELSE 0
	END, 
	CASE
		WHEN T.Hoursfrom != ''THEN REPLACE(T.Hoursfrom, ':', '')
		ELSE '0800'
	END,
	CASE
		WHEN T.Hoursto != '' THEN REPLACE(T.Hoursto, ':', '')
		ELSE '1600'
	END, 
	T.Callsperday
FROM
	Techmigration AS T
	INNER JOIN Mergedtechnician AS U 
		ON T.Id = U.Id

INSERT INTO Admin.Additionaluserprofile 
(Userid, Profileid)
SELECT
	U.Newtechnicianid, 
	Ap.Id
FROM 
	Mergedtechnician AS U, 
	Admin.Additionalprofile AS Ap
WHERE
	Name = 'Technician'

INSERT INTO Service.Zoneuser 
(Userid, Zone)
SELECT
	U.Newtechnicianid, 
	T.Category
FROM
	Mergedtechnician AS U
	INNER JOIN Techmigration AS T 
		ON U.Id = T.Id

-- System settings - Service Technician Category

DECLARE @Valuetext VARCHAR(256) = 
(
	SELECT STUFF((SELECT DISTINCT '' + Category + CHAR(10) FROM Techmigration FOR XML PATH('')), 1, 0, '')
)

IF EXISTS(SELECT * FROM Config.Setting WHERE Id = 'ServiceZone')
	UPDATE Config.Setting
		SET Valuetext = CAST(@Valuetext AS NTEXT)
	WHERE
		Id = 'ServiceZone'
ELSE
select top 1 * from Config.Setting

	INSERT INTO Config.Setting
		([NameSpace], id, ValueText)
	SELECT
		'Blue.Cosacs.Service', 
		'ServiceZone', 
		CAST(@Valuetext AS NTEXT)

CREATE TABLE Migtechnician
(
	Id INT, 
	Oldtechid INT, 
	Newtechid INT
)
DECLARE Tech CURSOR FOR 
	SELECT *
	FROM Mergedtechnician

DECLARE 
	@Id INT,
	@Mergetechs VARCHAR(2000),
	@Newtechid INT

OPEN Tech
FETCH Next FROM Tech INTO @Id, @Mergetechs, @Newtechid
WHILE @@Fetch_Status = 0
BEGIN

	INSERT INTO Migtechnician
	(Id, Oldtechid, Newtechid)
	SELECT
		@Id, 
		M.Items, 
		@Newtechid
	FROM 
		Dbo.Splitfn(@Mergetechs, ',') AS M

	FETCH Next FROM Tech INTO @Id, @Mergetechs, @Newtechid
END

CLOSE Tech
DEALLOCATE Tech

