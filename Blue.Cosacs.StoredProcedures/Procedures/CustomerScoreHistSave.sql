-- SELECT * FROM CountryMaintenance WHERE NAME LIKE '%behav%'
-- updates the proposalBSApply table to mark the scores/Bands as applied. Actual application will be applied at the end of day. 
IF EXISTS (SELECT * FROM sysobjects WHERE NAME ='CustomerScoreHistSave')
	DROP PROCEDURE CustomerScoreHistSave
GO 	
CREATE PROCEDURE CustomerScoreHistSave
@custid VARCHAR(20), @dateprop DATETIME,@scorecard CHAR(1), @points int, 
@creditlimit money, @band VARCHAR(2), @empeeno INT , @reasonchanged varchar(12), @acctno CHAR(12), @return INT OUTPUT
AS 
	SET NOCOUNT ON 
	SET @return = 0 
    
    DECLARE @BehaveApplyEodImmediate VARCHAR(6)
    SET @BehaveApplyEodImmediate =''
    IF @reasonchanged LIKE '%rescore%'
    BEGIN
		SELECT @BehaveApplyEodImmediate =value  FROM CountryMaintenance WHERE CodeName LIKE 'BehaveApplyEodImmediate'
	END 

	
	IF ISNULL(@scorecard,'') = ''
	BEGIN
		SELECT @scorecard = scorecardtype
		FROM customer
		WHERE custid = @custid
	END 
	ELSE 
	BEGIN
		IF @BehaveApplyEodImmediate !='False' 
			UPDATE 
			customer SET scorecardtype = @scorecard 
			WHERE custid =@custid 
	END 
	
	IF @points IS NULL
	BEGIN
		SELECT @points = p.points
		FROM proposal p
		WHERE p.custid = @custid
		AND dateprop = (SELECT MAX(p2.dateprop) 
		                FROM proposal p2
		                WHERE p2.custid = p.custid)
	END 
	declare @applyscore tinyint 
	IF @BehaveApplyEodImmediate ='False'
		set @applyscore = 0
	else 
		set @applyscore = 1
	
	
	
	 
		INSERT INTO CustomerScoreHist (
			Custid,	acctno,	Dateprop,
			Scorecard,	Points,	CreditLimit,
			Band,	DateChange,	Empeeno,
			ReasonChanged, applied 
		) VALUES (@Custid,	@acctno,	@Dateprop,
			@Scorecard,	@Points,	@CreditLimit,
			@Band,	GETDATE(),	@Empeeno,
			@ReasonChanged ,@applyscore ) 

	SET @return = @@ERROR

GO 


--SELECT DISTINCT(reasonchanged) FROM CustomerScoreHist