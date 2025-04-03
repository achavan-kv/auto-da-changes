IF OBJECT_ID('Communication.InsertBlackEmailList') IS NOT NULL
	DROP PROCEDURE Communication.InsertBlackEmailList
GO

CREATE PROCEDURE Communication.InsertBlackEmailList
	@Xml VarChar(Max),
	@Date Date
AS
	SET NOCOUNT ON

	IF OBJECT_ID('tempdb..#Data','U') IS NOT NULL 
		DROP TABLE #Data


	DECLARE @Data XML = @xml
	--Saves the original value
	DECLARE @ArithabortState Int = 64 & @@OPTIONS
	
	--------------------------------
	---    Loads data from XML   ---
	--------------------------------
	SELECT 
		NewMails.Email, 
		NewMails.Reason,
		@Date AS CreatedOn,
		NewMails.Provider
	INTO #Data
	FROM 
	(
		SELECT 
		  data.r.value('@Value', 'VarChar(255)') AS Email,
		  data.r.value('@Reason', 'VarChar(255)') as Reason,
		  data.r.value('@Provider', 'VarChar(255)') as Provider
		FROM
			@Data.nodes('Emails/Email') AS Data(r)
	) NewMails

	--------------------------------------------------------------------------------
	---   Deletes any existing e-mail on BlackEmailList that is not on the xml   ---
	--------------------------------------------------------------------------------
	DELETE Communication.BlackEmailList
	WHERE Id IN 
	(
		SELECT 
			i.Id
		FROM 
			#Data d
			RIGHT JOIN Communication.BlackEmailList i
				ON d.Email = i.Email
				AND d.Provider = i.Provider
		WHERE
			d.Email IS NULL
			AND i.Provider IN 
			(
				SELECT DISTINCT Provider FROM #Data
			)
	)

	--------------------------------------
	---   Insert new unsubscriptions   ---
	--------------------------------------
	INSERT INTO Communication.BlackEmailList
		(Email, Reason, CreatedOn, Provider)
	SELECT 
		d.Email, 
		d.Reason, 
		d.CreatedOn, 
		d.Provider
	FROM 
		#Data d
		LEFT JOIN Communication.BlackEmailList i
			ON d.Email = i.Email
			AND d.Provider = i.Provider
	WHERE
		i.Id IS NULL

	IF OBJECT_ID('tempdb..#Data','U') IS NOT NULL 
		DROP TABLE #Data

	IF (@ArithabortState & 64) = 64
		SET ARITHABORT ON
	ELSE
		SET ARITHABORT OFF

GO



/*
declare @XMLParam VarChar(max)
set @XMLParam = '<Emails>
                     <Email Value="100" Reason="x" Provider="Internal" />
                     <Email Value="200" Reason="y" Provider="Mandrill" />
                     <Email Value="300"  Provider="Mailchimp" />
					 <Email Value="henry.pires@bluebridgeltd.com"  Provider="New Provider" />
					 
                 </Emails>'

exec Communication.InsertBlackEmailList @XMLParam, '20150101'
select * from Communication.BlackEmailList

*/

