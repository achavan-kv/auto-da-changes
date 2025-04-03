
IF EXISTS (SELECT * FROM sysobjects WHERE NAME ='ProposalBSWriteSP')
	DROP PROCEDURE ProposalBSWriteSP
GO 	

CREATE PROCEDURE dbo.ProposalBSWriteSP
    @custid varchar(20),
    @acctno varchar(12),
    @dateprop smalldatetime,
    @points SMALLINT ,
    @propresult char(1),
    @reason char(2),
    @reason2 char(2),
    @reason3 char(2),
    @reason4 char(2),
    @reason5 char(2),
    @reason6 char(2),
    @notes varchar(1000),
    @user int,
    @newBand varchar(4) ,
    @scoretype VARCHAR(1), -- P for parallel run , 'R' for Rescore
    @newlimit MONEY,
    @oldband VARCHAR(4),
    @oldlimit MONEY, 
    @return int OUTPUT

AS

    declare @dlength integer,
            @currentBand       varchar(4), @runno INT 

    SET NOCOUNT ON
    SET @return = 0
    
    IF @scoretype = 'R'
		SELECT @runno= MAX(runno)
		FROM interfacecontrol 
		WHERE interface = 'BHSRescore'
		
    ELSE 
		SET @runno= 0 	
		
	
	-- Don't Calculate the new scoring band as this done in application
	
  --  EXEC DN_ScoringBandForAccountSP @AcctNo=@acctno , @currentBand= @CurrentBand out, @ScoringBand =@newBand OUT, @points = @points OUT ,@scoretype = 'B',
  --   @return= @Return OUT
    DECLARE @oldpoints SMALLINT
    SELECT @oldpoints=points FROM proposal WHERE custid = @custid 
    AND dateprop = (SELECT MAX(dateprop) FROM proposal p WHERE p.custid= @custid) 
    
    UPDATE  ProposalBS
    SET     points = @points,
            propresult = @propresult,
            reason = @reason,
            reason2 = @reason2,
            reason3 = @reason3,
            reason4 = @reason4,
            reason5 = @reason5,
            reason6 = @reason6,
            notes = @notes ,
            newlimit = @newlimit ,
            scoringband =@newband ,
            TYPE = @scoretype,
            runno = @runno,
            oldpoints = @oldpoints,
            oldlimit=@oldlimit,
            oldband = @oldband  
    WHERE   custid = @custid
    AND     dateprop = @dateprop
    AND     acctno = @acctno
	IF @@ROWCOUNT = 0 
	BEGIN
		INSERT INTO ProposalBS
		( custid,acctno,dateprop, points,
		 propresult,reason,Notes,empeenochange,
		 reason2,reason3,reason4,reason5,
		 reason6,ScoringBand,newlimit,applied ,
		 TYPE ,runno,oldpoints,oldlimit,oldband )
		VALUES 
		( @custid,@acctno,@dateprop, @points,
		 @propresult,@reason,@Notes,@user,
		 @reason2,@reason3,@reason4,@reason5,
		 @reason6,@newBand,@newlimit,'N',
		 @scoretype,@runno,@oldpoints,@oldlimit,@oldband)
	END	
		
    SET @return = @@error

    SET NOCOUNT OFF
    RETURN @Return

GO  
