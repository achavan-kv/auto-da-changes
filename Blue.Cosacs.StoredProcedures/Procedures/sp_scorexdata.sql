SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[sp_scorexdata]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[sp_scorexdata]
GO



CREATE procedure sp_scorexdata	@origbr		smallint, 
						@acctno		varchar(12)	/* NOT NULL WITH DEFAULT */, 
						@noofaccts		smallint	/* NOT NULL WITH DEFAULT */, 
						@balofaccts		money		/* NOT NULL WITH DEFAULT */, 
						@agrtotaccts	money		/* NOT NULL WITH DEFAULT */, 
						@settledaccts	smallint 	/* NOT NULL WITH DEFAULT */,
						@wrstcurrstat	varchar(1) 	/* NOT NULL WITH DEFAULT */, 
						@wrstsettstat	varchar(1) 	/* NOT NULL WITH DEFAULT */, 
						@klettrcount	smallint 	/* NOT NULL WITH DEFAULT */, 
						@rindicset		varchar(1) 	/* NOT NULL WITH DEFAULT */, 
						@windicset		varchar(1) 	/* NOT NULL WITH DEFAULT */, 
						@ncindicset		varchar(1) 	/* NOT NULL WITH DEFAULT */, 
						@staffacct		varchar(1) 	/* NOT NULL WITH DEFAULT */,
						@datelastdel	datetime	/* NOT NULL WITH DEFAULT */, 
						@repoindic		varchar(1) 	/* NOT NULL WITH DEFAULT */,
						@countrycode	varchar(2) 	/* NOT NULL WITH DEFAULT */,
						@bigitemcat		smallint 	/* NOT NULL WITH DEFAULT */,
						@agrmttotal		money 	/* NOT NULL WITH DEFAULT */,
						@depositpcent	float		/* NOT NULL WITH DEFAULT */,
						@instalamount	money		/* NOT NULL WITH DEFAULT */,
						@paymethod		varchar(1)	/* NOT NULL WITH DEFAULT */,
						@instalno		smallint 	/* NOT NULL WITH DEFAULT */,
						@guarindic		varchar(1) 	/* NOT NULL WITH DEFAULT */,
						@totarrears		money 	/* NOT NULL WITH DEFAULT */, 
						@sex			varchar(1) 	/* NOT NULL WITH DEFAULT */, 
						@ethnicity		varchar(1) 	/* NOT NULL WITH DEFAULT */, 
						@timecurraddr	smallint 	/* NOT NULL WITH DEFAULT */, 
						@timeprevaddr	smallint 	/* NOT NULL WITH DEFAULT */,
						@currresstat	varchar(1) 	/* NOT NULL WITH DEFAULT */,
						@mthlyrent		money 	/* NOT NULL WITH DEFAULT */, 
						@prevresstat	varchar(1) 	/* NOT NULL WITH DEFAULT */,
						@dateborn		datetime	/* NOT NULL WITH DEFAULT */, 
						@hometel		varchar(1) 	/* NOT NULL WITH DEFAULT */, 
						@worktel		varchar(1) 	/* NOT NULL WITH DEFAULT */, 
						@worktype		varchar(2) 	/* NOT NULL WITH DEFAULT */, 
						@fullorpart		varchar(1) 	/* NOT NULL WITH DEFAULT */, 
						@empmtstatus	varchar(1) 	/* NOT NULL WITH DEFAULT */,
						@timecurrempl	smallint 	/* NOT NULL WITH DEFAULT */, 
						@timeprevempl	smallint 	/* NOT NULL WITH DEFAULT */, 
						@mthlyincome	money 	/* NOT NULL WITH DEFAULT */, 
						@bankcode		varchar(6) 	/* NOT NULL WITH DEFAULT */, 
						@timebank		smallint 	/* NOT NULL WITH DEFAULT */, 
						@maritalstat	varchar(1) 	/* NOT NULL WITH DEFAULT */, 
						@spousecount	smallint 	/* NOT NULL WITH DEFAULT */,
						@dependants		smallint 	/* NOT NULL WITH DEFAULT */,
						@appliccount	smallint 	/* NOT NULL WITH DEFAULT */, 
						@otherpmnts		money 	/* NOT NULL WITH DEFAULT */,
						@points		smallint 	/* NOT NULL WITH DEFAULT */, 
						@decision		varchar(1) 	/* NOT NULL WITH DEFAULT */, 
						@reason		varchar(2) 	/* NOT NULL WITH DEFAULT */,
						@refempeeno		integer 	/* NOT NULL WITH DEFAULT */, 
						@override		varchar(1) 	/* NOT NULL WITH DEFAULT */, 
						@dateprop		datetime	/* NOT NULL WITH DEFAULT */, 
						@acceptscore	smallint 	/* NOT NULL WITH DEFAULT */, 
						@scorecardno	smallint 	/* NOT NULL WITH DEFAULT */, 
						@jobcount		smallint 	/* NOT NULL WITH DEFAULT */, 
						@privclub		varchar(1) 	/* NOT NULL WITH DEFAULT */, 
						@age			smallint 	/* NOT NULL WITH DEFAULT */, 
						@instpcincome	float 	/* NOT NULL WITH DEFAULT */, 
						@timelastdel	smallint 	/* NOT NULL WITH DEFAULT */, 
						@prevcustind	varchar(1) 	/* NOT NULL WITH DEFAULT */, 
						@prevaddrind	varchar(1) 	/* NOT NULL WITH DEFAULT */,
						@bankacctind	varchar(1) 	/* NOT NULL WITH DEFAULT */, 
						@bankacctcode	varchar(1) 	/* NOT NULL WITH DEFAULT */, 
						@jntmthincome	money 	/* NOT NULL WITH DEFAULT */, 
						@othcrtinstal	money 	/* NOT NULL WITH DEFAULT */, 
						@agrmtsizcode	varchar(1) 	/* NOT NULL WITH DEFAULT */, 
						@setagrmtsiz	varchar(1) 	/* NOT NULL WITH DEFAULT */, 
						@title		varchar(25) /* NOT NULL WITH DEFAULT */, 
						@payfreq	varchar(1) 	/* NOT NULL WITH DEFAULT */, 
						@postcode	varchar(10) /* NOT NULL WITH DEFAULT */, 
						@postalarea	varchar(10) /* NOT NULL WITH DEFAULT */,
						@relcount	smallint 	/* NOT NULL WITH DEFAULT */, 
						@addtoflag	varchar(1) 	/* NOT NULL WITH DEFAULT */, 
						@branchno	smallint 	/* NOT NULL WITH DEFAULT */,
						@itemcount	smallint 	/* NOT NULL WITH DEFAULT */,
						@mobile	varchar(1 )/* NOT NULL WITH DEFAULT */,
						@spouseoccupation varchar(5), /* NOT NULL WITH DEFAULT */
                  @appnumber integer
AS
DECLARE	@l_acctno	varchar(12);

BEGIN
	SELECT @l_acctno = acctno 
	FROM 	scorexdata 
	WHERE 	acctno = @acctno; 

	IF (@@rowcount = 0)
	BEGIN
		INSERT INTO scorexdata (	origbr, acctno, noofaccts, balofaccts, agrtotaccts, 
							settledaccts, wrstcurrstat, wrstsettstat, klettrcount,
							rindicset, windicset, ncindicset, staffacct, datelastdel, 
							repoindic, countrycode, bigitemcat, agrmttotal, depositpcent, 
							instalamount, paymethod, instalno, guarindic, totarrears, 
							sex, ethnicity, timecurraddr, timeprevaddr, currresstat, 
							mthlyrent, prevresstat, dateborn, hometel, worktel, 
							worktype, fullorpart, empmtstatus, timecurrempl, timeprevempl, 
							mthlyincome, bankcode, timebank, maritalstat, spousecount, 
							dependants, appliccount, otherpmnts, points, decision, 
							reason, refempeeno, override, dateprop, acceptscore, 
							scorecardno, jobcount, privclub, age, instpcincome,
							timelastdel, prevcustind, prevaddrind, bankacctind, bankacctcode,
							jntmthincome, othcrtinstal, agrmtsizcode, setagrmtsiz, title,
							payfreq, postcode, postalarea, relcount, addtoflag, 
							branchno, itemcount, mobile, spouseoccupation, appnumber)
		VALUES (	@origbr, @acctno, @noofaccts, @balofaccts, @agrtotaccts, 
				@settledaccts, @wrstcurrstat, @wrstsettstat, @klettrcount, 
				@rindicset, @windicset, @ncindicset, @staffacct, @datelastdel, 
				@repoindic, @countrycode, @bigitemcat, @agrmttotal, @depositpcent, 
				@instalamount, @paymethod, @instalno, @guarindic, @totarrears, 
				@sex, @ethnicity, @timecurraddr, @timeprevaddr, @currresstat, 
				@mthlyrent, @prevresstat, @dateborn, @hometel, @worktel, 
				@worktype, @fullorpart, @empmtstatus, @timecurrempl, @timeprevempl, 
				@mthlyincome, @bankcode, @timebank, @maritalstat, @spousecount, 
				@dependants, @appliccount, @otherpmnts, @points, @decision, 
				@reason, @refempeeno, @override, @dateprop, @acceptscore,
				@scorecardno, @jobcount, @privclub, @age, @instpcincome,
				@timelastdel, @prevcustind, @prevaddrind, @bankacctind, @bankacctcode,
				@jntmthincome, @othcrtinstal, @agrmtsizcode, @setagrmtsiz, @title,
				@payfreq, @postcode, @postalarea, @relcount, @addtoflag,
				@branchno, @itemcount, @mobile, @spouseoccupation, @appnumber  );
		IF (@@error != 0)
		BEGIN
			return @@error;
		END;
	END

	ELSE

	BEGIN
		UPDATE scorexdata 
		SET	origbr	=	@origbr, 
			acctno	=	@acctno, 
			noofaccts	=	@noofaccts,
			balofaccts	=	@balofaccts, 
			agrtotaccts	=	@agrtotaccts, 
			settledaccts =	@settledaccts, 
			wrstcurrstat =	@wrstcurrstat, 
			wrstsettstat =	@wrstsettstat, 
			klettrcount	=	@klettrcount, 
			rindicset	=	@rindicset,
			windicset	=	@windicset, 
			ncindicset	=	@ncindicset, 
			staffacct	=	@staffacct, 
			datelastdel	=	@datelastdel, 
			repoindic	=	@repoindic,
			countrycode	=	@countrycode, 
			bigitemcat	=	@bigitemcat, 
			agrmttotal	=	@agrmttotal, 
			depositpcent =	@depositpcent, 
			instalamount =	@instalamount, 
			paymethod	=	@paymethod, 
			instalno	=	@instalno,
			guarindic	=	@guarindic, 
			totarrears	=	@totarrears, 
			sex		=	@sex,
			ethnicity	=	@ethnicity, 
			timecurraddr =	@timecurraddr, 
			timeprevaddr =	@timeprevaddr, 
			currresstat	=	@currresstat, 
			mthlyrent	=	@mthlyrent,
			prevresstat	=	@prevresstat, 
			dateborn	=	@dateborn, 
			hometel	=	@hometel,
			worktel	=	@worktel, 
			worktype	=	@worktype, 
			fullorpart	=	@fullorpart,
			empmtstatus	=	@empmtstatus, 
			timecurrempl =	@timecurrempl,
			timeprevempl =	@timeprevempl, 
			mthlyincome	=	@mthlyincome, 
			bankcode	=	@bankcode, 
			timebank	=	@timebank, 
			maritalstat	=	@maritalstat,
			spousecount	=	@spousecount, 
			dependants	=	@dependants, 
			appliccount	=	@appliccount, 
			otherpmnts	=	@otherpmnts, 
			points	=	@points, 
			decision	=	@decision, 
			reason	=	@reason, 
			refempeeno	=	@refempeeno, 
			override	=	@override, 
			dateprop	=	@dateprop, 
			acceptscore	=	@acceptscore,
			scorecardno	=	@scorecardno, 
			jobcount	=	@jobcount, 
			privclub	=	@privclub, 
			age		=	@age, 
			instpcincome =	@instpcincome, 
			timelastdel	=	@timelastdel, 
			prevcustind	=	@prevcustind, 
			prevaddrind	=	@prevaddrind,
			bankacctind	=	@bankacctind, 
			bankacctcode =	@bankacctcode,
			jntmthincome =	@jntmthincome, 
			othcrtinstal =	@othcrtinstal,
			agrmtsizcode =	@agrmtsizcode, 
			setagrmtsiz	=	@setagrmtsiz, 
			title		=	@title, 
			payfreq	=	@payfreq, 
			postcode	=	@postcode, 
			postalarea	=	@postalarea, 
			relcount	=	@relcount, 
			addtoflag	=	@addtoflag, 
			branchno	=	@branchno, 
			itemcount	=	@itemcount ,
			mobile		= 	@mobile,
			spouseoccupation = 	@spouseoccupation,
         appnumber = @appnumber

		where acctno	=	@acctno;

		IF (@@error != 0)
		BEGIN
			return @@error; 
		END; 

	END;
END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

