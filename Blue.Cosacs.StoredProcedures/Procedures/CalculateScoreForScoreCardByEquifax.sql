IF EXISTS (SELECT * FROM sysobjects 
		   WHERE NAME = 'CalculateScoreForScoreCardByEquifax'
		   AND xtype = 'p')
BEGIN
	DROP PROCEDURE CalculateScoreForScoreCardByEquifax
END
GO

CREATE  PROCEDURE [dbo].[CalculateScoreForScoreCardByEquifax]
	
@accountNo nvarchar(50)='',
@customerID nvarchar(50)='',
@country nvarchar(10)='',
@customerage nvarchar(50)='',
@employmentempmtstatus nvarchar(50)='',
@proposalMaritalstat nvarchar(50)='',
@proposaldependants nvarchar(50)='',
@worktype nvarchar(50)='',
@prodcode nvarchar(50)='',
@custaddressresstatus nvarchar(50)='',
@addresstime nvarchar(50)='',
@employmenttime nvarchar(50)='',
@proposalnationality nvarchar(50)='',
@custaddresspocode nvarchar(50)='',
@paddresstime nvarchar(50)='',
@gender nvarchar(50)='',
@mobilephone nvarchar(50)='',
@NumAppsLst90 nvarchar(50)='',
@SCORE decimal(25,15) out,
@return int OUTPUT
 

AS

set @return =0
declare @scoreType nvarchar(10)=''

 if OBJECT_ID('tempdb..#Equifax_Variable') is not null
	 begin
	  drop table #Equifax_Variable
	   --SELECT * from  Equifax_Variable FROM Equifax_Variable 
	 end
	 --else
	 --begin
	 SELECT * INTO #Equifax_Variable FROM Equifax_Variable 
	 --end


--select @scoreType = value from CountryMaintenance where name ='Behavioural Scorecard'

 --*******************************************************************************************
 DECLARE @VAR_FLAG_CUSTOMERSTATUS_HIS_WOE CHAR(1)
 -- ****************************************** CALCULATE  Get 17 month data FOR TT,BB,BZ ***********************************************************
     if OBJECT_ID('tempdb..#TempTable') is not null
	 begin
	  drop table #TempTable
	 end
	
	 create table #TempTable 
     (    EXTRACT_DATE date,
		  CUSTOMER_ID varchar(20),
		  ACCOUNT_NUMBER char(12),
		  DATE_ACCOUNT_OPENED date,
		  OUTSTANDING_BALANCE money,
		  Date_Obs date,
		  BALANCE_ARREARS money,
		  DAYS_ARREARS int,
		  Agreement_Total money
	 )

Declare @historyinmonths int
 select @historyinmonths = value from countrymaintenance where name  like '%Behavioural history in months%'

	INSERT INTO #TempTable	EXEC Get17MonthsDataForScoreCard @customerID, @historyinmonths
	
	declare @scorecardType CHAR(1) 
	exec ScoreTypeToUse @customerID,@accountNo,@scorecardType out,''
	

	IF((SELECT COUNT(*) FROM #TempTable) > 0 and @scorecardType='D')
	BEGIN
		SET @VAR_FLAG_CUSTOMERSTATUS_HIS_WOE = 'E'
		SET @scoreType = 'D'
	END
	ELSE 
	BEGIN
		SET @VAR_FLAG_CUSTOMERSTATUS_HIS_WOE = 'N'
		SET @scoreType = 'C'
	END

-- ****************************************************************************************************

DECLARE @XML AS XML, @hDoc AS INT, @SQL NVARCHAR (MAX)


SELECT	TOP 1 @XML = RulesXML
	FROM	ScoringRules
	WHERE	CountryCode = @country
	AND		Region = ''
	AND    ISNULL((CONVERT(XML,rulesxml)).value('(/Rules/@ScoreType)[1]', 'varchar(1)'),@scoreType) = @scoreType
	ORDER BY DateImported DESC


EXEC sp_xml_preparedocument @hDoc OUTPUT, @XML

SELECT 
ScoreType, Country, DeclineScore,ReferScore,BureauMinimum,BureauMaximum,Region,InterceptScore,Type ,Result,State,RuleName,ApplyRF,ApplyHP,
ReferDeclined,ReferAccepted,RuleRejects,ReferToBureau,ClauseType,ClauseState,Operand,OlType,OlTableName,Operator,O2Operand,O2Type
into #tempTT 
FROM OPENXML(@hDoc, 'Rules/Rule/Clause/O1')
WITH 
(
ScoreType [nvarchar](10) '../../../@ScoreType',
Country [nvarchar](100) '../../../@Country',
DeclineScore [nvarchar](100) '../../../@DeclineScore',
ReferScore [nvarchar](100) '../../../@ReferScore',
BureauMinimum [nvarchar](100) '../../../@BureauMinimum',
BureauMaximum [nvarchar](100) '../../../@BureauMaximum',
Region [nvarchar](100) '../../../@Region',
InterceptScore [nvarchar](100) '../../../@InterceptScore',

Type [nvarchar](100) '../../@Type',
State [nvarchar](100) '../../@State',
Result [nvarchar](1000) '../../@Result',
RuleName [nvarchar](1000) '../../@RuleName',
ApplyRF [nvarchar](10) '../../@ApplyRF',
ApplyHP [nvarchar](10) '../../@ApplyHP',
ReferDeclined [nvarchar](10) '../../@ReferDeclined',
ReferAccepted [nvarchar](10) '../../@ReferAccepted',
RuleRejects [nvarchar](10) '../../@RuleRejects',
ReferToBureau [nvarchar](10) '../../@ReferToBureau',

ClauseType [nvarchar](10) '@Type',
ClauseState [nvarchar](50) '@State',

Operand [nvarchar](100) '@Operand',
OlType [nvarchar](100) '@Type',
OlTableName [nvarchar](100) '@TableName',

Operator [nvarchar](100) '../CO/@Operator',

O2Operand [nvarchar](100) '../O2/@Operand',
O2Type [nvarchar](100) '../O2/@Type'
)
-- 

-- select * from tempTT
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES 
				   WHERE TABLE_NAME = N'tempTT')  
		begin
			  
			  Delete from dbo.tempTT
			--SELECT * INTO tempTT from #tempTT
			INSERT INTO tempTT 
			(ScoreType, Country, DeclineScore,ReferScore,BureauMinimum,BureauMaximum,Region,InterceptScore,Type ,Result,State,RuleName,ApplyRF,ApplyHP,
			ReferDeclined,ReferAccepted,RuleRejects,ReferToBureau,ClauseType,ClauseState,Operand,OlType,OlTableName,Operator,O2Operand,O2Type)
			SELECT ScoreType, Country, DeclineScore,ReferScore,BureauMinimum,BureauMaximum,Region,InterceptScore,Type ,Result,State,RuleName,ApplyRF,ApplyHP,
			ReferDeclined,ReferAccepted,RuleRejects,ReferToBureau,ClauseType,ClauseState,Operand,OlType,OlTableName,Operator,O2Operand,O2Type 
			FROM  #tempTT 
		end	  
else
		begin
		SELECT * INTO dbo.tempTT from #tempTT
		
		end


EXEC sp_xml_removedocument @hDoc

--********************************************************************************************************
-- DROP TABLE #TEMP_SCORECARDDETAIL

if OBJECT_ID('tempdb..#TEMP_SCORECARDDETAIL') is not null
	 begin
	  drop table #TEMP_SCORECARDDETAIL
	 end
	
	DECLARE @Result INT
	CREATE TABLE #TEMP_SCORECARDDETAIL
	(
	customerage  INT,
	employmentempmtstatus CHAR(1),
	proposalMaritalstat CHAR(1),
	proposaldependants  INT,
	worktype  CHAR(2),
	prodcode  VARCHAR(8),
	custaddressresstatus VARCHAR(2),
	addresstime INT,
	employmenttime   INT,
	proposalnationality  VARCHAR (2),
	custaddresspocode  VARCHAR(6),
	paddresstime   INT,
	gender  CHAR(1),
	mobilephone  CHAR(1),
	NumAppsLst90 decimal(25,15)
	)

INSERT INTO #TEMP_SCORECARDDETAIL values
(
@customerage ,
@employmentempmtstatus ,
@proposalMaritalstat ,
@proposaldependants ,
@worktype  ,
@prodcode ,
@custaddressresstatus ,
@addresstime ,
@employmenttime ,
@proposalnationality  ,
@custaddresspocode  ,
@paddresstime  ,
@gender  ,
@mobilephone ,
@NumAppsLst90  
)

--  exec DN_GetScoreDetailsForScoreCardSP @accountNo , @Result out 
 --SELECT * FROM #TEMP_SCORECARDDETAIL


-- ****************************************************************************************************
DECLARE 
@InterceptScore decimal(25,15) = 0.00,
@LOGODDS decimal(25,15) =0.00 

SELECT TOP 1  @InterceptScore =  InterceptScore FROM #tempTT 

DECLARE @INTERCEPT NVARCHAR(10)='',
@LOGValue NVARCHAR(10)=''

IF(@scoreType = 'C')
BEGIN
	SELECT @INTERCEPT = VALUE FROM COUNTRYMAINTENANCE WHERE CODENAME  IN ('EquifaxAInterceptSign')
	SELECT @LOGValue = VALUE FROM COUNTRYMAINTENANCE WHERE CODENAME  IN ('EquifaxInterceptSign')
END

IF(@scoreType = 'D')
BEGIN
	SELECT @INTERCEPT = VALUE FROM COUNTRYMAINTENANCE WHERE CODENAME  IN ('EquifaxBInterceptSign')
	SELECT @LOGValue = VALUE FROM COUNTRYMAINTENANCE WHERE CODENAME  IN ('EquifaxLogValue')
END

IF(@INTERCEPT = '+')
BEGIN
	SET @InterceptScore = 0 + @InterceptScore
END
ELSE
BEGIN
	SET @InterceptScore = 0 - @InterceptScore
END

 --***********************************************************************************************

DECLARE @OCCUPATION_WOE decimal(25,15) = 0.00,
@RATIO_TCURRENTEMPLOY_TO_AGE decimal(25,15) = 0.00,
@MARITALSTATUS_WOE decimal(25,15) = 0.00,
@AGE decimal(25,15) = 0.00,
@RESIDENTIALSTATUS_WOE decimal(25,15) = 0.00,
@FLAG_CUSTOMERSTATUS_HIS_WOE decimal(25,15) = 0.00,
@MOBILENUMBER_WOE decimal(25,15) = 0.00,
@RATIO_NDEPENDENT_TO_AGE decimal(25,15) = 0.00,
@TIMECURRENTADDRESS decimal(25,15) = 0.00,
@TIMECURRENTADDRESS_LN decimal(25,15) = 0.00,
@EMPLOYMENTSTATUS_WOE decimal(25,15) = 0.00,
@GENDER_WOE decimal(25,15) = 0.00,
@POSTCODE_WOE decimal(25,15) = 0.00,
@NUMBERDEPENDENTS decimal(25,15) = 0.00,
@TIMECURRENTEMPLOYMENT_SR decimal(25,15) = 0.00,
@OLDEST_CREDIT_LN decimal(25,15) = 0.00,
@TIMECURRENTEMPLOYMENT_LN decimal(25,15) = 0.00,
@TIMECURRENTEMPLOYMENT decimal(25,15) = 0.00,
@NUMBER_ACCOUNT_OPENED_3M decimal(25,15) = 0.00,
@NUMBER_ACCOUNT_OPENED_3M_CR decimal(25,15) = 0.00,
@NUMBERDEPENDENTS_SQ decimal(25,15) = 0.00,
@NUMBERDEPENDENTS_CR decimal(25,15) = 0.00,
@MAX_PERC_OUTS_3M_SQ decimal(25,15) = 0.00,
@NEWEST_CREDIT_SQ decimal(25,15) = 0.00,
@BALANCEARREARS_POUND_6M_LN decimal(25,15) = 0.00,
@BALANCEARREARS_POUND_6M decimal(25,15) = 0.00,
@COUNT_DAYSARREAR_60MORE_17M_LN decimal(25,15) = 0.00,
@COUNT_DAYSARREAR_30MORE_17M_LN decimal(25,15) = 0.00,
@MAX_PERC_OUTSARREARS_6M_LN decimal(25,15) = 0.00,
@NUMBER_ACCOUNT_17M decimal(25,15) = 0.00,
@AVG_AGREEMENT_TOTAL_1M_SQ decimal(25,15) = 0.00,
@AVG_BALANCE_ARREARS_1M_LN decimal(25,15) = 0.00,
@DAYSARREARS_POUND_6M decimal(25,15) = 0.00,
@AVG_BALANCE_ARREARS_12M_LN decimal(25,15) = 0.00,
@TIMECURRENTADDRESS_WOE decimal(25,15) = 0.00,

--  *************************************************** EQUIFAX ***************************

@EX_OCCUPATION_WOE decimal(25,15) = ( SELECT ISNULL(Weightage,0.00) FROM #Equifax_Variable WHERE Flag_CustomerStatus = @VAR_FLAG_CUSTOMERSTATUS_HIS_WOE AND variable ='OCCUPATION_WOE' ),
@EX_RATIO_TCURRENTEMPLOY_TO_AGE decimal(25,15) = (SELECT ISNULL(Weightage,0.00) FROM #Equifax_Variable WHERE Flag_CustomerStatus = @VAR_FLAG_CUSTOMERSTATUS_HIS_WOE AND variable ='RATIO_TCURRENTEMPLOY_TO_AGE') ,
@EX_MARITALSTATUS_WOE decimal(25,15) = ( SELECT Weightage FROM #Equifax_Variable WHERE Flag_CustomerStatus = @VAR_FLAG_CUSTOMERSTATUS_HIS_WOE AND variable ='MARITALSTATUS_WOE' ),
@EX_AGE decimal(25,15) = ( SELECT Weightage FROM #Equifax_Variable WHERE Flag_CustomerStatus = @VAR_FLAG_CUSTOMERSTATUS_HIS_WOE AND variable ='AGE' ),
@EX_RESIDENTIALSTATUS_WOE decimal(25,15) = ( SELECT Weightage FROM #Equifax_Variable WHERE Flag_CustomerStatus = @VAR_FLAG_CUSTOMERSTATUS_HIS_WOE AND variable ='RESIDENTIALSTATUS_WOE' ),
@EX_FLAG_CUSTOMERSTATUS_HIS_WOE decimal(25,15) = ( SELECT Weightage FROM #Equifax_Variable WHERE Flag_CustomerStatus = @VAR_FLAG_CUSTOMERSTATUS_HIS_WOE AND variable ='FLAG_CUSTOMERSTATUS_HIS_WOE' ),
@EX_MOBILENUMBER_WOE decimal(25,15) = ( SELECT Weightage FROM #Equifax_Variable WHERE Flag_CustomerStatus = @VAR_FLAG_CUSTOMERSTATUS_HIS_WOE AND variable ='MOBILENUMBER_WOE' ),
@EX_RATIO_NDEPENDENT_TO_AGE decimal(25,15) = ( SELECT Weightage FROM #Equifax_Variable WHERE Flag_CustomerStatus = @VAR_FLAG_CUSTOMERSTATUS_HIS_WOE AND variable ='RATIO_NDEPENDENT_TO_AGE' ),
@EX_TIMECURRENTADDRESS decimal(25,15) =( SELECT Weightage FROM #Equifax_Variable WHERE Flag_CustomerStatus = @VAR_FLAG_CUSTOMERSTATUS_HIS_WOE AND variable ='TIMECURRENTADDRESS' ),
@EX_TIMECURRENTADDRESS_LN decimal(25,15) =( SELECT Weightage FROM #Equifax_Variable WHERE Flag_CustomerStatus = @VAR_FLAG_CUSTOMERSTATUS_HIS_WOE AND variable ='TIMECURRENTADDRESS_LN' ),
@EX_EMPLOYMENTSTATUS_WOE decimal(25,15) = ( SELECT Weightage FROM #Equifax_Variable WHERE Flag_CustomerStatus = @VAR_FLAG_CUSTOMERSTATUS_HIS_WOE AND variable ='EMPLOYMENTSTATUS_WOE' ),
@EX_GENDER_WOE decimal(25,15) = ( SELECT Weightage FROM #Equifax_Variable WHERE Flag_CustomerStatus = @VAR_FLAG_CUSTOMERSTATUS_HIS_WOE AND variable ='GENDER_WOE' ),
@EX_POSTCODE_WOE decimal(25,15) = ( SELECT Weightage FROM #Equifax_Variable WHERE Flag_CustomerStatus = @VAR_FLAG_CUSTOMERSTATUS_HIS_WOE AND variable ='POSTCODE_WOE' ),
@EX_NUMBERDEPENDENTS decimal(25,15) = ( SELECT Weightage FROM #Equifax_Variable WHERE Flag_CustomerStatus = @VAR_FLAG_CUSTOMERSTATUS_HIS_WOE AND variable ='NUMBERDEPENDENTS' ),
@EX_TIMECURRENTEMPLOYMENT_SR decimal(25,15) =( SELECT Weightage FROM #Equifax_Variable WHERE Flag_CustomerStatus = @VAR_FLAG_CUSTOMERSTATUS_HIS_WOE AND variable ='TIMECURRENTEMPLOYMENT_SR' ),
@EX_OLDEST_CREDIT_LN decimal(25,15) = ( SELECT Weightage FROM #Equifax_Variable WHERE Flag_CustomerStatus = @VAR_FLAG_CUSTOMERSTATUS_HIS_WOE AND variable ='OLDEST_CREDIT_LN' ),
@EX_TIMECURRENTEMPLOYMENT_LN decimal(25,15) = ( SELECT Weightage FROM #Equifax_Variable WHERE Flag_CustomerStatus = @VAR_FLAG_CUSTOMERSTATUS_HIS_WOE AND variable ='TIMECURRENTEMPLOYMENT_LN' ),
@EX_TIMECURRENTEMPLOYMENT decimal(25,15) =( SELECT Weightage FROM #Equifax_Variable WHERE Flag_CustomerStatus = @VAR_FLAG_CUSTOMERSTATUS_HIS_WOE AND variable ='TIMECURRENTEMPLOYMENT' ),
@EX_NUMBER_ACCOUNT_OPENED_3M decimal(25,15) = ( SELECT Weightage FROM #Equifax_Variable WHERE Flag_CustomerStatus = @VAR_FLAG_CUSTOMERSTATUS_HIS_WOE AND variable ='NUMBER_ACCOUNT_OPENED_3M' ),
@EX_NUMBER_ACCOUNT_OPENED_3M_CR decimal(25,15) = ( SELECT Weightage FROM #Equifax_Variable WHERE Flag_CustomerStatus = @VAR_FLAG_CUSTOMERSTATUS_HIS_WOE AND variable ='NUMBER_ACCOUNT_OPENED_3M_CR' ),
@EX_NUMBERDEPENDENTS_SQ decimal(25,15) = ( SELECT Weightage FROM #Equifax_Variable WHERE Flag_CustomerStatus = @VAR_FLAG_CUSTOMERSTATUS_HIS_WOE AND variable ='NUMBERDEPENDENTS_SQ' ),
@EX_NUMBERDEPENDENTS_CR decimal(25,15) = ( SELECT Weightage FROM #Equifax_Variable WHERE Flag_CustomerStatus = @VAR_FLAG_CUSTOMERSTATUS_HIS_WOE AND variable ='NUMBERDEPENDENTS_CR' ),
@EX_MAX_PERC_OUTS_3M_SQ decimal(25,15) = ( SELECT Weightage FROM #Equifax_Variable WHERE Flag_CustomerStatus = @VAR_FLAG_CUSTOMERSTATUS_HIS_WOE AND variable ='MAX_PERC_OUTS_3M_SQ' ),
@EX_NEWEST_CREDIT_SQ decimal(25,15) = ( SELECT Weightage FROM #Equifax_Variable WHERE Flag_CustomerStatus = @VAR_FLAG_CUSTOMERSTATUS_HIS_WOE AND variable ='NEWEST_CREDIT_SQ' ),
@EX_BALANCEARREARS_POUND_6M_LN decimal(25,15) = ( SELECT Weightage FROM #Equifax_Variable WHERE Flag_CustomerStatus = @VAR_FLAG_CUSTOMERSTATUS_HIS_WOE AND variable ='BALANCEARREARS_POUND_6M_LN' ),
@EX_BALANCEARREARS_POUND_6M decimal(25,15) = ( SELECT Weightage FROM #Equifax_Variable WHERE Flag_CustomerStatus = @VAR_FLAG_CUSTOMERSTATUS_HIS_WOE AND variable ='BALANCEARREARS_POUND_6M' ),
@EX_COUNT_DAYSARREAR_60MORE_17M_LN decimal(25,15) =( SELECT Weightage FROM #Equifax_Variable WHERE Flag_CustomerStatus = @VAR_FLAG_CUSTOMERSTATUS_HIS_WOE AND variable ='COUNT_DAYSARREAR_60MORE_17M_LN' ),
@EX_COUNT_DAYSARREAR_30MORE_17M_LN decimal(25,15) = ( SELECT Weightage FROM #Equifax_Variable WHERE Flag_CustomerStatus = @VAR_FLAG_CUSTOMERSTATUS_HIS_WOE AND variable ='COUNT_DAYSARREAR_30MORE_17M_LN' ),
@EX_MAX_PERC_OUTSARREARS_6M_LN decimal(25,15) = ( SELECT Weightage FROM #Equifax_Variable WHERE Flag_CustomerStatus = @VAR_FLAG_CUSTOMERSTATUS_HIS_WOE AND variable ='MAX_PERC_OUTSARREARS_6M_LN' ),
@EX_NUMBER_ACCOUNT_17M decimal(25,15) = ( SELECT Weightage FROM #Equifax_Variable WHERE Flag_CustomerStatus = @VAR_FLAG_CUSTOMERSTATUS_HIS_WOE AND variable ='NUMBER_ACCOUNT_17M' ),
@EX_AVG_AGREEMENT_TOTAL_1M_SQ decimal(25,15) = ( SELECT Weightage FROM #Equifax_Variable WHERE Flag_CustomerStatus = @VAR_FLAG_CUSTOMERSTATUS_HIS_WOE AND variable ='AVG_AGREEMENT_TOTAL_1M_SQ' ),
@EX_AVG_BALANCE_ARREARS_1M_LN decimal(25,15) = ( SELECT Weightage FROM #Equifax_Variable WHERE Flag_CustomerStatus = @VAR_FLAG_CUSTOMERSTATUS_HIS_WOE AND variable ='AVG_BALANCE_ARREARS_1M_LN' ),
@EX_DAYSARREARS_POUND_6M decimal(25,15) = ( SELECT Weightage FROM #Equifax_Variable WHERE Flag_CustomerStatus = @VAR_FLAG_CUSTOMERSTATUS_HIS_WOE AND variable ='DAYSARREARS_POUND_6M' ),
@EX_AVG_BALANCE_ARREARS_12M_LN decimal(25,15) = ( SELECT Weightage FROM #Equifax_Variable WHERE Flag_CustomerStatus = @VAR_FLAG_CUSTOMERSTATUS_HIS_WOE AND variable ='AVG_BALANCE_ARREARS_12M_LN' ),
@EX_TIMECURRENTADDRESS_WOE decimal(25,15) = ( SELECT Weightage FROM #Equifax_Variable WHERE Flag_CustomerStatus = @VAR_FLAG_CUSTOMERSTATUS_HIS_WOE AND variable ='TIMECURRENTADDRESS' ),
@EX_FLAG_CUSTOMERSTATUS_HIS_WOE_B decimal(25,15) = ( SELECT Weightage FROM #Equifax_Variable WHERE Flag_CustomerStatus = @VAR_FLAG_CUSTOMERSTATUS_HIS_WOE AND variable ='FLAG_CUSTOMERSTATUS_HIS_WOE' ),


--  *************************************************** RULE *************************************************

@RL_OCCUPATION_WOE decimal(25,15) = 0.00,
@RL_RATIO_TCURRENTEMPLOY_TO_AGE decimal(25,15) =  0.00,
@RL_MARITALSTATUS_WOE decimal(25,15) = 0.00,
@RL_AGE decimal(25,15) = 0.00,
@RL_RESIDENTIALSTATUS_WOE decimal(25,15) = 0.00,
@RL_FLAG_CUSTOMERSTATUS_HIS_WOE decimal(25,15) = 0.00,
@RL_MOBILENUMBER_WOE decimal(25,15) = 0.00,
@RL_RATIO_NDEPENDENT_TO_AGE decimal(25,15) = 0.00,
@RL_TIMECURRENTADDRESS decimal(25,15) = 0.00,
@RL_TIMECURRENTADDRESS_LN decimal(25,15) = 0.00,
@RL_EMPLOYMENTSTATUS_WOE decimal(25,15) = 0.00,
@RL_GENDER_WOE decimal(25,15) = 0.00,
@RL_POSTCODE_WOE decimal(25,15) = 0.00,
@RL_NUMBERDEPENDENTS decimal(25,15) = 0.00,
@RL_TIMECURRENTEMPLOYMENT_SR decimal(25,15) = 0.00,
@RL_OLDEST_CREDIT_LN decimal(25,15) = 0.00,
@RL_TIMECURRENTEMPLOYMENT_LN decimal(25,15) = 0.00,
@RL_TIMECURRENTEMPLOYMENT decimal(25,15) = 0.00,
@RL_NUMBER_ACCOUNT_OPENED_3M decimal(25,15) = 0.00,
@RL_NUMBER_ACCOUNT_OPENED_3M_CR decimal(25,15) = 0.00,
@RL_NUMBERDEPENDENTS_SQ decimal(25,15)  = 0.00,
@RL_NUMBERDEPENDENTS_CR decimal(25,15)  = 0.00,
@RL_MAX_PERC_OUTS_3M_SQ decimal(25,15)  = 0.00,
@RL_NEWEST_CREDIT_SQ decimal(25,15)  = 0.00,
@RL_BALANCEARREARS_POUND_6M_LN decimal(25,15)  =0.00,
@RL_BALANCEARREARS_POUND_6M decimal(25,15)  = 0.00,
@RL_COUNT_DAYSARREAR_60MORE_17M_LN decimal(25,15)  =0.00,
@RL_COUNT_DAYSARREAR_30MORE_17M_LN decimal(25,15)  =0.00,
@RL_NUMBER_ACCOUNT_17M decimal(25,15)  = 0.00,
@RL_AVG_AGREEMENT_TOTAL_1M_SQ decimal(25,15)  = 0.00,
@RL_AVG_BALANCE_ARREARS_1M_LN decimal(25,15)  = 0.00,
@RL_DAYSARREARS_POUND_6M decimal(25,15)  = 0.00,
@RL_AVG_BALANCE_ARREARS_12M_LN decimal(25,15)  = 0.00,
@RL_TIMECURRENTADDRESS_WOE decimal(25,15) = 0.00 ,
@RL_MAX_PERC_OUTSARREARS_6M_LN decimal(25,15) = 0.00

--	 ***************************** Calculate Variable : Flag Customer Status History;  ******************
/*
IF(@VAR_FLAG_CUSTOMERSTATUS_HIS_WOE = 'N') AND OBJECT_ID('tempdb..#TempTable') is  null
	BEGIN
		SET @FLAG_CUSTOMERSTATUS_HIS_WOE = @EX_FLAG_CUSTOMERSTATUS_HIS_WOE * @RL_FLAG_CUSTOMERSTATUS_HIS_WOE;
	END
ELSE
	BEGIN
		SET @FLAG_CUSTOMERSTATUS_HIS_WOE = @EX_FLAG_CUSTOMERSTATUS_HIS_WOE_B * @RL_FLAG_CUSTOMERSTATUS_HIS_WOE;
	END
*/
IF(@VAR_FLAG_CUSTOMERSTATUS_HIS_WOE = 'N') AND OBJECT_ID('tempdb..#TempTable') is  null
	BEGIN
		IF EXISTS(SELECT * FROM #tempTT WHERE Operand = 'FLAG_CUSTOMERSTATUS_HIS_WOE' AND O2Operand = 'N')
					BEGIN
						SET @RL_FLAG_CUSTOMERSTATUS_HIS_WOE = ( SELECT TOP (1) ISNULL(Result,0.00) FROM #tempTT WHERE Operand = 'FLAG_CUSTOMERSTATUS_HIS_WOE' AND O2Operand =   'N' )
						SET @FLAG_CUSTOMERSTATUS_HIS_WOE = @EX_FLAG_CUSTOMERSTATUS_HIS_WOE * @RL_FLAG_CUSTOMERSTATUS_HIS_WOE
END
	END
ELSE
	BEGIN
	SET @RL_FLAG_CUSTOMERSTATUS_HIS_WOE = ( SELECT TOP (1) ISNULL(Result,0.00) FROM #tempTT WHERE Operand = 'FLAG_CUSTOMERSTATUS_HIS_WOE' AND O2Operand =   'E' )
	SET @FLAG_CUSTOMERSTATUS_HIS_WOE = @EX_FLAG_CUSTOMERSTATUS_HIS_WOE_B * @RL_FLAG_CUSTOMERSTATUS_HIS_WOE;
END
-- *************************************** CALCULATE OCCUPATION_WOE (TN, TE **************************************************************

IF EXISTS(SELECT * FROM #tempTT WHERE Operand = 'OCCUPATION_WOE')
	BEGIN
		IF EXISTS(SELECT * FROM #tempTT WHERE Operand = 'OCCUPATION_WOE' AND O2Operand = (select worktype from #TEMP_SCORECARDDETAIL))
		BEGIN
			SET @RL_OCCUPATION_WOE = ( SELECT TOP (1) ISNULL(Result,0.00) FROM #tempTT WHERE Operand = 'OCCUPATION_WOE' AND O2Operand = (select worktype from #TEMP_SCORECARDDETAIL) )
			SET @OCCUPATION_WOE = @EX_OCCUPATION_WOE * @RL_OCCUPATION_WOE
		END
	    ELSE IF EXISTS(SELECT * FROM #tempTT WHERE Operand = 'OCCUPATION_WOE' AND O2Operand IN ('O','OT'))
		BEGIN
			SET @RL_OCCUPATION_WOE = ( SELECT TOP (1) ISNULL(Result,0.00) FROM #tempTT WHERE Operand = 'OCCUPATION_WOE' AND O2Operand IN ('O','OT') )
			SET @OCCUPATION_WOE = @EX_OCCUPATION_WOE * @RL_OCCUPATION_WOE
		END
	    ELSE
		BEGIN
			SET @RL_OCCUPATION_WOE = 0.00
			SET @OCCUPATION_WOE = 0.00
		END
	END
ELSE
	BEGIN
		SET @RL_OCCUPATION_WOE = 0.00
		SET @OCCUPATION_WOE = 0.00
	END

-- *************************************** CALCULATE MARITALSTATUS_WOE (TN,TE **************************************************************

IF EXISTS(SELECT * FROM #tempTT WHERE Operand = 'MARITALSTATUS_WOE')
	BEGIN
		IF EXISTS(SELECT * FROM #tempTT WHERE Operand = 'MARITALSTATUS_WOE' AND O2Operand = (select proposalmaritalstat from #TEMP_SCORECARDDETAIL))
		BEGIN
			SET @RL_MARITALSTATUS_WOE = ( SELECT TOP (1) ISNULL(Result,0.00) FROM #tempTT WHERE Operand = 'MARITALSTATUS_WOE' AND O2Operand = (select proposalmaritalstat from #TEMP_SCORECARDDETAIL) )
			SET @MARITALSTATUS_WOE = @EX_MARITALSTATUS_WOE * @RL_MARITALSTATUS_WOE
		END
		ELSE IF EXISTS(SELECT * FROM #tempTT WHERE Operand = 'MARITALSTATUS_WOE' AND O2Operand IN ('O','OT'))
		BEGIN
			SET @RL_MARITALSTATUS_WOE = ( SELECT TOP (1) ISNULL(Result,0.00) FROM #tempTT WHERE Operand = 'MARITALSTATUS_WOE' AND O2Operand IN ('O','OT') )
			SET @MARITALSTATUS_WOE = @EX_MARITALSTATUS_WOE * @RL_MARITALSTATUS_WOE
		END
		ELSE
		BEGIN
			SET @RL_MARITALSTATUS_WOE = 0.00
			SET @MARITALSTATUS_WOE = 0.00
		END
	END
ELSE
	BEGIN
		SET @RL_MARITALSTATUS_WOE = 0.00
		SET @MARITALSTATUS_WOE = 0.00
	END

-- *************************************** CALCULATE AGE( TN,TE **************************************************************

IF EXISTS(SELECT * FROM #tempTT WHERE Operand = 'AGE')
	BEGIN
		IF EXISTS(SELECT * FROM #tempTT WHERE Operand = 'AGE')
		BEGIN
			SET @RL_AGE = (select [dbo].[GetRESULTFORSCORECARD] ('AGE',@customerage) from #TEMP_SCORECARDDETAIL)
			SET @AGE = @EX_AGE * @RL_AGE
		END
		ELSE 
		BEGIN
			SET @RL_AGE = 0.00
			SET @AGE = @EX_AGE * @RL_AGE

		END
	END
ELSE
	BEGIN
		SET @RL_AGE = 0.00
		SET @AGE =  0.00
	END

-- *************************************** CALCULATE RESIDENTIALSTATUS_WOE (TN,TE  **************************************************************

IF EXISTS(SELECT * FROM #tempTT WHERE Operand = 'RESIDENTIALSTATUS_WOE')
	BEGIN
		IF EXISTS(SELECT * FROM #tempTT WHERE Operand = 'RESIDENTIALSTATUS_WOE' AND O2Operand = (select custaddressresstatus from #TEMP_SCORECARDDETAIL))
		BEGIN
			SET @RL_RESIDENTIALSTATUS_WOE = ( SELECT TOP (1) ISNULL(Result,0.00) FROM #tempTT WHERE Operand = 'RESIDENTIALSTATUS_WOE' AND O2Operand = (select custaddressresstatus from #TEMP_SCORECARDDETAIL) )
			SET @RESIDENTIALSTATUS_WOE = @EX_RESIDENTIALSTATUS_WOE * @RL_RESIDENTIALSTATUS_WOE
		END
		ELSE IF EXISTS(SELECT * FROM #tempTT WHERE Operand = 'RESIDENTIALSTATUS_WOE' AND O2Operand IN ('O','OT'))
		BEGIN
			SET @RL_RESIDENTIALSTATUS_WOE = ( SELECT TOP (1) ISNULL(Result,0.00) FROM #tempTT WHERE Operand = 'RESIDENTIALSTATUS_WOE' AND O2Operand IN ('O','OT') )
			SET @RESIDENTIALSTATUS_WOE = @EX_RESIDENTIALSTATUS_WOE * @RL_RESIDENTIALSTATUS_WOE
		END
		ELSE
		BEGIN
			SET @RL_RESIDENTIALSTATUS_WOE = 0.00
		  SET @RESIDENTIALSTATUS_WOE = 0.00
		END
	END
ELSE
	BEGIN
		SET @RL_RESIDENTIALSTATUS_WOE = 0.00
		SET @RESIDENTIALSTATUS_WOE = 0.00
	END

-- *************************************** CALCULATE FLAG_CUSTOMERSTATUS_HIS_WOE (TN **************************************************************

IF EXISTS(SELECT * FROM #tempTT WHERE Operand = 'FLAG_CUSTOMERSTATUS_HIS_WOE')
	BEGIN
		IF EXISTS(SELECT * FROM #tempTT WHERE Operand = 'FLAG_CUSTOMERSTATUS_HIS_WOE' AND O2Operand = @VAR_FLAG_CUSTOMERSTATUS_HIS_WOE)
		BEGIN
			SET @RL_FLAG_CUSTOMERSTATUS_HIS_WOE = ( SELECT TOP (1) ISNULL(Result,0.00) FROM #tempTT WHERE Operand = 'FLAG_CUSTOMERSTATUS_HIS_WOE' AND O2Operand = @VAR_FLAG_CUSTOMERSTATUS_HIS_WOE )
			SET @FLAG_CUSTOMERSTATUS_HIS_WOE = @EX_FLAG_CUSTOMERSTATUS_HIS_WOE * @RL_FLAG_CUSTOMERSTATUS_HIS_WOE
		END
		ELSE 
		BEGIN
			SET @RL_FLAG_CUSTOMERSTATUS_HIS_WOE = 0.00
			SET @FLAG_CUSTOMERSTATUS_HIS_WOE = 0.00

		END
	END
ELSE
	BEGIN
		SET @RL_FLAG_CUSTOMERSTATUS_HIS_WOE = 0.00
		SET @FLAG_CUSTOMERSTATUS_HIS_WOE = 0.00
	END

-- *************************************** CALCULATE MOBILENUMBER_WOE (TN **************************************************************

IF EXISTS(SELECT * FROM #tempTT WHERE Operand = 'MOBILENUMBER_WOE')
	BEGIN
		IF EXISTS(SELECT * FROM #tempTT WHERE Operand = 'MOBILENUMBER_WOE' AND O2Operand = (select mobilephone from #TEMP_SCORECARDDETAIL))
		BEGIN
			SET @RL_MOBILENUMBER_WOE = ( SELECT TOP (1) ISNULL(Result,0.00) FROM #tempTT WHERE Operand = 'MOBILENUMBER_WOE' AND O2Operand = (select mobilephone from #TEMP_SCORECARDDETAIL) )
			SET @MOBILENUMBER_WOE = @EX_MOBILENUMBER_WOE * @RL_MOBILENUMBER_WOE
		END
		ELSE 
		BEGIN
			SET @RL_MOBILENUMBER_WOE = 0.00
			SET @MOBILENUMBER_WOE = 0.00

		END
	END
ELSE
	BEGIN
		SET @RL_MOBILENUMBER_WOE = 0.00
		SET @MOBILENUMBER_WOE = 0.00
	END

-- *************************************** CALCULATE TIMECURRENTADDRESS **************************************************************
--select 'TIMECURRENTADDRESS table',* FROM #tempTT WHERE Operand = 'TIMECURRENTADDRESS'
--select @EX_TIMECURRENTADDRESS as '@EX_TIMECURRENTADDRESS'
 
IF EXISTS(SELECT * FROM #tempTT WHERE Operand = 'TIMECURRENTADDRESS')
	BEGIN
		IF EXISTS(SELECT * FROM #tempTT WHERE Operand = 'TIMECURRENTADDRESS' )
		BEGIN
			SET @RL_TIMECURRENTADDRESS = ( SELECT TOP(1) [dbo].[GetRESULTFORSCORECARD] (Operand, (select addresstime from #TEMP_SCORECARDDETAIL)) FROM #tempTT WHERE Operand = 'TIMECURRENTADDRESS' )
			
			SET @TIMECURRENTADDRESS = @EX_TIMECURRENTADDRESS * @RL_TIMECURRENTADDRESS
		END
		ELSE 
		BEGIN
			SET @RL_TIMECURRENTADDRESS = 0.00
			SET @TIMECURRENTADDRESS = 0.00

		END
	END
ELSE
	BEGIN
		SET @RL_TIMECURRENTADDRESS = 0.00
		SET @TIMECURRENTADDRESS = 0.00
	END
	--select @RL_TIMECURRENTADDRESS as '@RL_TIMECURRENTADDRESS'
	--select @TIMECURRENTADDRESS as '@TIMECURRENTADDRESS'
-- *************************************** CALCULATE TIMECURRENTADDRESS_WOE **************************************************************
/*
IF EXISTS(SELECT * FROM #tempTT WHERE Operand = 'TIMECURRENTADDRESS_WOE')
	BEGIN
		IF EXISTS(SELECT * FROM #tempTT WHERE Operand = 'TIMECURRENTADDRESS_WOE' )
		BEGIN
			SET @RL_TIMECURRENTADDRESS = ( SELECT TOP(1) [dbo].[GetRESULTFORSCORECARD] (Operand, (select addresstime from #TEMP_SCORECARDDETAIL)) FROM #tempTT WHERE Operand = 'TIMECURRENTADDRESS_WOE' )
			
			SET @TIMECURRENTADDRESS = @EX_TIMECURRENTADDRESS * @RL_TIMECURRENTADDRESS
		END
		ELSE 
		BEGIN
			SET @RL_TIMECURRENTADDRESS = 0.00
			SET @TIMECURRENTADDRESS = 0.00

		END
	END
ELSE
	BEGIN
		SET @RL_TIMECURRENTADDRESS = 0.00
		SET @TIMECURRENTADDRESS = 0.00
	END
	*/
-- *************************************** CALCULATE TIMECURRENTADDRESS_LN **************************************************************

IF EXISTS(SELECT * FROM #tempTT WHERE Operand = 'TIMECURRENTADDRESS_LN')
	BEGIN
		IF EXISTS(SELECT * FROM #tempTT WHERE Operand = 'TIMECURRENTADDRESS_LN' )
		BEGIN
			SET @RL_TIMECURRENTADDRESS_LN = ( SELECT TOP(1) [dbo].[GetRESULTFORSCORECARD] (Operand, (select addresstime from #TEMP_SCORECARDDETAIL)) FROM #tempTT WHERE Operand = 'TIMECURRENTADDRESS_LN' )
			SET @TIMECURRENTADDRESS_LN = @EX_TIMECURRENTADDRESS_LN * LOG ( @RL_TIMECURRENTADDRESS_LN + 1 )
		END
		ELSE 
		BEGIN
			SET @RL_TIMECURRENTADDRESS_LN = 0.00
			SET @TIMECURRENTADDRESS_LN = 0.00
		END
	END
ELSE
	BEGIN
		SET @RL_TIMECURRENTADDRESS_LN = 0.00
		SET @TIMECURRENTADDRESS_LN = 0.00
	END

-- *************************************** CALCULATE EMPLOYMENTSTATUS_WOE **************************************************************

IF EXISTS(SELECT * FROM #tempTT WHERE Operand = 'EMPLOYMENTSTATUS_WOE')
	BEGIN
		IF EXISTS(SELECT * FROM #tempTT WHERE Operand = 'EMPLOYMENTSTATUS_WOE' AND O2Operand = (select employmentempmtstatus from #TEMP_SCORECARDDETAIL))
		BEGIN
			SET @RL_EMPLOYMENTSTATUS_WOE = ( SELECT TOP (1) ISNULL(Result,0.00) FROM #tempTT WHERE Operand = 'EMPLOYMENTSTATUS_WOE' AND O2Operand = (select employmentempmtstatus from #TEMP_SCORECARDDETAIL) )
			SET @EMPLOYMENTSTATUS_WOE = @EX_EMPLOYMENTSTATUS_WOE * @RL_EMPLOYMENTSTATUS_WOE
		END
		ELSE IF EXISTS(SELECT * FROM #tempTT WHERE Operand = 'EMPLOYMENTSTATUS_WOE' AND O2Operand IN ('O','OT'))
		BEGIN
			SET @RL_EMPLOYMENTSTATUS_WOE = ( SELECT TOP (1) ISNULL(Result,0.00) FROM #tempTT WHERE Operand = 'EMPLOYMENTSTATUS_WOE' AND O2Operand IN ('O','OT') )
			SET @EMPLOYMENTSTATUS_WOE = @EX_EMPLOYMENTSTATUS_WOE * @RL_EMPLOYMENTSTATUS_WOE
		END
		ELSE
		BEGIN
			SET @RL_EMPLOYMENTSTATUS_WOE = 0.00
			SET @EMPLOYMENTSTATUS_WOE = 0.00
		END
	END
ELSE
	BEGIN
		SET @RL_EMPLOYMENTSTATUS_WOE = 0.00
		SET @EMPLOYMENTSTATUS_WOE = 0.00
	END

-- *************************************** CALCULATE GENDER_WOE **************************************************************

IF EXISTS(SELECT * FROM #tempTT WHERE Operand = 'GENDER_WOE')
	BEGIN
		IF EXISTS(SELECT * FROM #tempTT WHERE Operand = 'GENDER_WOE' AND O2Operand = (select gender from #TEMP_SCORECARDDETAIL))
		BEGIN
			SET @RL_GENDER_WOE = ( SELECT TOP (1) ISNULL(Result,0.00) FROM #tempTT WHERE Operand = 'GENDER_WOE' AND O2Operand = (select gender from #TEMP_SCORECARDDETAIL) )
			SET @GENDER_WOE = @EX_GENDER_WOE * @RL_GENDER_WOE
		END
		ELSE IF EXISTS(SELECT * FROM #tempTT WHERE Operand = 'GENDER_WOE' AND O2Operand IN ('O','OT'))
		BEGIN
			SET @RL_GENDER_WOE = ( SELECT TOP (1) ISNULL(Result,0.00) FROM #tempTT WHERE Operand = 'GENDER_WOE' AND O2Operand IN ('O','OT') )
			SET @GENDER_WOE = @EX_GENDER_WOE * @RL_GENDER_WOE
		END
		ELSE
		BEGIN
			SET @RL_GENDER_WOE = 0.00
			SET @GENDER_WOE = 0.00
		END
	END
ELSE
	BEGIN
		SET @RL_GENDER_WOE = 0.00
		SET @GENDER_WOE = 0.00
	END

-- *************************************** CALCULATE POSTCODE_WOE **************************************************************

IF EXISTS(SELECT * FROM #tempTT WHERE Operand = 'POSTCODE_WOE')
	BEGIN
		IF EXISTS(SELECT * FROM #tempTT WHERE Operand = 'POSTCODE_WOE' AND O2Operand like '%' + (select custaddresspocode from #TEMP_SCORECARDDETAIL) + '%')
		BEGIN
			SET @RL_POSTCODE_WOE = ( SELECT TOP (1) ISNULL(Result,0.00) FROM #tempTT WHERE Operand = 'POSTCODE_WOE' AND O2Operand like '%' + (select custaddresspocode from #TEMP_SCORECARDDETAIL) + '%' )
			SET @POSTCODE_WOE = @EX_POSTCODE_WOE * @RL_POSTCODE_WOE
		END
		ELSE IF EXISTS(SELECT * FROM #tempTT WHERE Operand = 'POSTCODE_WOE' AND O2Operand IN ('O','OT','OTHER'))
		BEGIN
			SET @RL_POSTCODE_WOE = ( SELECT TOP (1) ISNULL(Result,0.00) FROM #tempTT WHERE Operand = 'POSTCODE_WOE' AND O2Operand IN ('O','OT','OTHER') )
			SET @POSTCODE_WOE = @EX_POSTCODE_WOE * @RL_POSTCODE_WOE
		END
		ELSE
		BEGIN
			SET @RL_POSTCODE_WOE = 0.00
			SET @POSTCODE_WOE = 0.00
		END
	END
ELSE
	BEGIN
		SET @RL_POSTCODE_WOE = 0.00
		SET @POSTCODE_WOE = 0.00
	END

-- *************************************** CALCULATE NUMBERDEPENDENTS **************************************************************

IF EXISTS(SELECT * FROM #tempTT WHERE Operand = 'NUMBERDEPENDENTS')
	BEGIN
		IF EXISTS(SELECT * FROM #tempTT WHERE Operand = 'NUMBERDEPENDENTS')
		BEGIN
			SET @RL_NUMBERDEPENDENTS = ( SELECT  TOP(1) [dbo].[GetRESULTFORSCORECARD] (Operand,(select proposaldependants from #TEMP_SCORECARDDETAIL))  FROM #tempTT WHERE Operand = 'NUMBERDEPENDENTS' )
			
			SET @NUMBERDEPENDENTS = @EX_NUMBERDEPENDENTS * @RL_NUMBERDEPENDENTS
		END
		ELSE 
		BEGIN
			SET @RL_NUMBERDEPENDENTS = 0.00
			SET @NUMBERDEPENDENTS =  0.00
		END
	END
ELSE
	BEGIN
		SET @RL_NUMBERDEPENDENTS = 0.00
		SET @NUMBERDEPENDENTS =  0.00
	END

-- *************************************** CALCULATE TIMECURRENTEMPLOYMENT_SR **************************************************************

IF EXISTS(SELECT * FROM #tempTT WHERE Operand = 'TIMECURRENTEMPLOYMENT')
	BEGIN
		IF EXISTS(SELECT * FROM #tempTT WHERE Operand = 'TimeCurrentEmployment')
		BEGIN
			SET @RL_TIMECURRENTEMPLOYMENT_SR = (SELECT TOP(1) [dbo].[GetRESULTFORSCORECARD] (Operand,(select employmenttime from #TEMP_SCORECARDDETAIL))  FROM #tempTT WHERE Operand = 'TimeCurrentEmployment')
			
			SET @TIMECURRENTEMPLOYMENT_SR = @EX_TIMECURRENTEMPLOYMENT_SR * SQRT(@RL_TIMECURRENTEMPLOYMENT_SR)
		END
		ELSE 
		BEGIN
			SET @RL_TIMECURRENTEMPLOYMENT_SR = 0.00
			SET @TIMECURRENTEMPLOYMENT_SR = 0.00
		END
	END
ELSE
	BEGIN
		SET @RL_TIMECURRENTEMPLOYMENT_SR = 0.00
		SET @TIMECURRENTEMPLOYMENT_SR = 0.00
	END
	

-- *************************************** CALCULATE TIMECURRENTEMPLOYMENT_LN **************************************************************

IF EXISTS(SELECT * FROM #tempTT WHERE Operand = 'TIMECURRENTEMPLOYMENT_LN')
	BEGIN
		IF EXISTS(SELECT * FROM #tempTT WHERE Operand = 'TIMECURRENTEMPLOYMENT_LN')
		BEGIN
			SET @RL_TIMECURRENTEMPLOYMENT_LN = (SELECT TOP(1) [dbo].[GetRESULTFORSCORECARD] (Operand,(select employmenttime from #TEMP_SCORECARDDETAIL))  FROM #tempTT WHERE Operand = 'TIMECURRENTEMPLOYMENT_LN')
			
			SET @TIMECURRENTEMPLOYMENT_LN = @EX_TIMECURRENTEMPLOYMENT_LN * LOG ( @RL_TIMECURRENTEMPLOYMENT_LN + 1 )
		END
		ELSE 
		BEGIN
			SET @RL_TIMECURRENTEMPLOYMENT_LN = 0.00
			SET @TIMECURRENTEMPLOYMENT_LN = 0.00
		END
	END
ELSE
	BEGIN
		SET @RL_TIMECURRENTEMPLOYMENT_LN = 0.00
		SET @TIMECURRENTEMPLOYMENT_LN = 0.00
	END

-- *************************************** CALCULATE TIMECURRENTEMPLOYMENT **************************************************************

IF EXISTS(SELECT * FROM #tempTT WHERE Operand = 'TIMECURRENTEMPLOYMENT')
	BEGIN
		IF EXISTS(SELECT * FROM #tempTT WHERE Operand = 'TIMECURRENTEMPLOYMENT')
		BEGIN
			SET @RL_TIMECURRENTEMPLOYMENT = (SELECT TOP(1) [dbo].[GetRESULTFORSCORECARD] (Operand,(select employmenttime from #TEMP_SCORECARDDETAIL))  FROM #tempTT WHERE Operand = 'TIMECURRENTEMPLOYMENT')
			
			SET @TIMECURRENTEMPLOYMENT = @EX_TIMECURRENTEMPLOYMENT * @RL_TIMECURRENTEMPLOYMENT
		END
		ELSE 
		BEGIN
			SET @RL_TIMECURRENTEMPLOYMENT = 0.00
			SET @TIMECURRENTEMPLOYMENT = 0.00
		END
	END
ELSE
	BEGIN
		SET @RL_TIMECURRENTEMPLOYMENT = 0.00
		SET @TIMECURRENTEMPLOYMENT = 0.00
	END

	  -- where Operand= 'TIMECURRENTEMPLOYMENT'

-- *************************************** CALCULATE NUMBER_ACCOUNT_OPENED_3M (TE **************************************************************

IF EXISTS(SELECT * FROM #tempTT WHERE Operand = 'NUMBER_ACCOUNT_OPENED_3M')
	BEGIN
		IF EXISTS(SELECT * FROM #tempTT WHERE Operand = 'NUMBER_ACCOUNT_OPENED_3M' AND O2Operand = (select NumAppsLst90 from #TEMP_SCORECARDDETAIL))
		BEGIN
			SET @RL_NUMBER_ACCOUNT_OPENED_3M = ( SELECT  TOP (1) ISNULL(Result,0.00) FROM #tempTT WHERE Operand = 'NUMBER_ACCOUNT_OPENED_3M' AND O2Operand = (select NumAppsLst90 from #TEMP_SCORECARDDETAIL) )
			SET @NUMBER_ACCOUNT_OPENED_3M = @EX_NUMBER_ACCOUNT_OPENED_3M * @RL_NUMBER_ACCOUNT_OPENED_3M
		END
		ELSE 
		BEGIN
			SET @RL_NUMBER_ACCOUNT_OPENED_3M = 0.00
			SET @NUMBER_ACCOUNT_OPENED_3M = 0.00
		END
	END
ELSE
	BEGIN
		SET @RL_NUMBER_ACCOUNT_OPENED_3M = 0.00
		SET @NUMBER_ACCOUNT_OPENED_3M = 0.00
	END

-- *************************************** CALCULATE NUMBER_ACCOUNT_OPENED_3M_CR **************************************************************

IF EXISTS(SELECT * FROM #tempTT WHERE Operand = 'NUMBER_ACCOUNT_OPENED_3M_CR')
	BEGIN
	IF EXISTS(SELECT * FROM #tempTT WHERE Operand = 'NUMBER_ACCOUNT_OPENED_3M_CR' AND O2Operand = (select NumAppsLst90 from #TEMP_SCORECARDDETAIL))
		BEGIN
			SET @RL_NUMBER_ACCOUNT_OPENED_3M_CR = ( SELECT TOP (1) ISNULL(Result,0.00) FROM #tempTT WHERE Operand = 'NUMBER_ACCOUNT_OPENED_3M_CR' AND O2Operand = (select NumAppsLst90 from #TEMP_SCORECARDDETAIL) )
			SET @NUMBER_ACCOUNT_OPENED_3M_CR = @EX_NUMBER_ACCOUNT_OPENED_3M_CR * POWER(@RL_NUMBER_ACCOUNT_OPENED_3M_CR,3)
		END
		ELSE 
		BEGIN
			SET @RL_NUMBER_ACCOUNT_OPENED_3M_CR = 0.00
			SET @NUMBER_ACCOUNT_OPENED_3M_CR =  0.00
		END
	END
ELSE
	BEGIN
		SET @RL_NUMBER_ACCOUNT_OPENED_3M_CR = 0.00
		SET @NUMBER_ACCOUNT_OPENED_3M_CR =  0.00
	END

-- *************************************** CALCULATE NUMBERDEPENDENTS_SQ **************************************************************

IF EXISTS(SELECT * FROM #tempTT WHERE Operand = 'NUMBERDEPENDENTS_SQ')
	BEGIN
		IF EXISTS(SELECT * FROM #tempTT WHERE Operand = 'NUMBERDEPENDENTS_SQ' AND O2Operand = (select proposaldependants from #TEMP_SCORECARDDETAIL))
		BEGIN
			SET @RL_NUMBERDEPENDENTS_SQ = ( SELECT  TOP(1) [dbo].[GetRESULTFORSCORECARD] (Operand,(select proposaldependants from #TEMP_SCORECARDDETAIL))  FROM #tempTT WHERE Operand = 'NUMBERDEPENDENTS_SQ' )
			
			SET @NUMBERDEPENDENTS_SQ = @EX_NUMBERDEPENDENTS_SQ * SQUARE(@RL_NUMBERDEPENDENTS_SQ)
		END
		ELSE 
		BEGIN
			SET @RL_NUMBERDEPENDENTS_SQ = 0.00
			SET @NUMBERDEPENDENTS_SQ =  0.00
		END
	END
ELSE
	BEGIN
		SET @RL_NUMBERDEPENDENTS_SQ = 0.00
		SET @NUMBERDEPENDENTS_SQ =  0.00
	END

-- *************************************** CALCULATE NUMBERDEPENDENTS_CR **************************************************************

IF EXISTS(SELECT * FROM #tempTT WHERE Operand = 'NUMBERDEPENDENTS_CR')
	BEGIN
		IF EXISTS(SELECT * FROM #tempTT WHERE Operand = 'NUMBERDEPENDENTS_CR' AND O2Operand = (select proposaldependants from #TEMP_SCORECARDDETAIL))
		BEGIN
			SET @RL_NUMBERDEPENDENTS_CR =  ( SELECT TOP(1) [dbo].[GetRESULTFORSCORECARD] (Operand,(select proposaldependants from #TEMP_SCORECARDDETAIL))  FROM #tempTT WHERE Operand = 'NUMBERDEPENDENTS_CR' )
			
			SET @NUMBERDEPENDENTS_CR = @EX_NUMBERDEPENDENTS_CR * POWER(@RL_NUMBERDEPENDENTS_CR, 3)
		END
		ELSE 
		BEGIN
			SET @RL_NUMBERDEPENDENTS_CR = 0.00
			SET @NUMBERDEPENDENTS_CR = 0.00
		END
	END
ELSE
	BEGIN
		SET @RL_NUMBERDEPENDENTS_CR = 0.00
		SET @NUMBERDEPENDENTS_CR = 0.00
	END

-- ********************************** CALCULATE RATIO_TCURRENTEMPLOY_TO_AGE **************************	
/*
IF EXISTS(SELECT * FROM #tempTT WHERE Operand = 'RATIO_TCURRENTEMPLOY_TO_AGE')
	BEGIN
		IF(@TIMECURRENTEMPLOYMENT != 0 OR @customerage != 0 )
		BEGIN 
			SET @RATIO_TCURRENTEMPLOY_TO_AGE =  @TIMECURRENTEMPLOYMENT / @customerage
		END
		ELSE
		BEGIN
			SET @RATIO_TCURRENTEMPLOY_TO_AGE = 0.00
		END
	END
ELSE
	BEGIN
		SET @RATIO_TCURRENTEMPLOY_TO_AGE = 0.00
	END*/
IF EXISTS(SELECT * FROM #tempTT WHERE Operand = 'RATIO_TCURRENTEMPLOY_TO_AGE')
	BEGIN
		IF EXISTS(SELECT * FROM #tempTT WHERE Operand = 'TIMECURRENTEMPLOYMENT')
		BEGIN
			SET @RL_TIMECURRENTEMPLOYMENT = (SELECT TOP(1) [dbo].[GetRESULTFORSCORECARD] (Operand,(select employmenttime from #TEMP_SCORECARDDETAIL))  FROM #tempTT WHERE Operand = 'TIMECURRENTEMPLOYMENT')
			IF(@RL_TIMECURRENTEMPLOYMENT != 0 and @customerage != 0 )
			BEGIN 
			SET @RATIO_TCURRENTEMPLOY_TO_AGE =  @RL_TIMECURRENTEMPLOYMENT / @customerage
			END
		END
				
		ELSE
		BEGIN
			SET @RATIO_TCURRENTEMPLOY_TO_AGE = 0.00
		END
END

-- ********************************** CALCULATE RATIO_NDEPENDENT_TO_AGE ********************************

IF EXISTS(SELECT * FROM #tempTT WHERE Operand = 'RATIO_NDEPENDENT_TO_AGE')
	BEGIN
		IF(@NUMBERDEPENDENTS != 0 OR @AGE != 0 )
		BEGIN 
			SET @RATIO_NDEPENDENT_TO_AGE =  @NUMBERDEPENDENTS / @customerage
		END
		ELSE
		BEGIN
			SET @RATIO_NDEPENDENT_TO_AGE = 0.00
		END
	END
ELSE
	BEGIN
		SET @RATIO_NDEPENDENT_TO_AGE = 0.00
	END

--**************************************CALCULATE OLDEST_CREDIT_LN *********************************************

IF EXISTS(SELECT * FROM #tempTT WHERE Operand = 'OLDEST_CREDIT_LN')
	BEGIN
		IF EXISTS(SELECT DBO.GetRESULTFORSCORECARD(OPERAND,(select ABS(datediff(MM,CAST(MIN(DATE_ACCOUNT_OPENED) as date), 
		CAST(MIN(DBO.FirstDayOfCurrentMonth(GETDATE())) as date))) from #TempTable ))
		FROM #tempTT WHERE OPERAND ='OLDEST_CREDIT_LN')
		BEGIN
			set @RL_OLDEST_CREDIT_LN  = ( SELECT DBO.GetRESULTFORSCORECARD(OPERAND,(select ABS(datediff(MM,CAST(MIN(DATE_ACCOUNT_OPENED) as date), 
										  CAST(MIN(DBO.FirstDayOfCurrentMonth(GETDATE())) as date))) from #TempTable ))
										  FROM #tempTT WHERE OPERAND ='OLDEST_CREDIT_LN')
			set @OLDEST_CREDIT_LN  = @EX_OLDEST_CREDIT_LN * LOG( @RL_OLDEST_CREDIT_LN + 1 )
		END
		ELSE 
		BEGIN
			SET @RL_OLDEST_CREDIT_LN = 0.00
			SET @OLDEST_CREDIT_LN    = 0.00
		END
	END
ELSE
	BEGIN
		SET @RL_OLDEST_CREDIT_LN = 0.00
		SET @OLDEST_CREDIT_LN    = 0.00
	END

----**************************************************NEWEST_CREDIT_SQ TT BZ*******************************************************

IF EXISTS(SELECT * FROM #tempTT WHERE Operand = 'NEWEST_CREDIT_SQ')
	BEGIN
		IF EXISTS(SELECT DBO.GetRESULTFORSCORECARD(OPERAND,(select ABS(datediff(MM,CAST(MAX(DATE_ACCOUNT_OPENED) as date), 
		CAST(MIN(DBO.FirstDayOfCurrentMonth(GETDATE())) as date))) from #TempTable))
		FROM #tempTT WHERE OPERAND ='NEWEST_CREDIT_SQ')
		BEGIN
			set @RL_NEWEST_CREDIT_SQ  =( SELECT DBO.GetRESULTFORSCORECARD(OPERAND,(select ABS(datediff(MM,CAST(MAX(DATE_ACCOUNT_OPENED) as date), 
										 CAST(MIN(DBO.FirstDayOfCurrentMonth(GETDATE())) as date))) from #TempTable))
										 FROM #tempTT WHERE OPERAND ='NEWEST_CREDIT_SQ')
			set @NEWEST_CREDIT_SQ  =  @EX_NEWEST_CREDIT_SQ * SQUARE (@RL_NEWEST_CREDIT_SQ)
		END
		ELSE 
		BEGIN
			SET @RL_NEWEST_CREDIT_SQ = 0.00
			set @NEWEST_CREDIT_SQ  =   0.00
		END
	END
ELSE
	BEGIN
		SET @RL_NEWEST_CREDIT_SQ = 0.00
		set @NEWEST_CREDIT_SQ  =   0.00
	END

---****************************************** COUNT_DAYSARREAR_60MORE_17M_LN (TE ***************************************************************

IF EXISTS(SELECT * FROM #tempTT WHERE Operand = 'COUNT_DAYSARREAR_60MORE_17M_LN')
	BEGIN
		IF EXISTS(SELECT DBO.GetRESULTFORSCORECARD(OPERAND,(select count(Account_Number) from #TempTable where days_arrears > = 60))
		FROM #tempTT WHERE OPERAND ='COUNT_DAYSARREAR_60MORE_17M_LN')
		BEGIN
			SET @RL_COUNT_DAYSARREAR_60MORE_17M_LN = ( SELECT DBO.GetRESULTFORSCORECARD(OPERAND,(select count(Account_Number) from #TempTable where days_arrears > = 60))
													   FROM #tempTT WHERE OPERAND ='COUNT_DAYSARREAR_60MORE_17M_LN')
			set @COUNT_DAYSARREAR_60MORE_17M_LN  =  @EX_COUNT_DAYSARREAR_60MORE_17M_LN * LOG( @RL_COUNT_DAYSARREAR_60MORE_17M_LN + 1 )
		END
		ELSE 
		BEGIN
			SET @RL_COUNT_DAYSARREAR_60MORE_17M_LN = 0.00
			set @COUNT_DAYSARREAR_60MORE_17M_LN  = 0.00
		END
	END
ELSE
	BEGIN
		SET @RL_COUNT_DAYSARREAR_60MORE_17M_LN = 0.00
		set @COUNT_DAYSARREAR_60MORE_17M_LN  = 0.00
	END

--****************************************** COUNT_DAYSARREAR_30MORE_17M_LN BB***************************************************************

IF EXISTS(SELECT * FROM #tempTT WHERE Operand = 'COUNT_DAYSARREAR_30MORE_17M_LN')
	BEGIN
		IF EXISTS(SELECT DBO.GetRESULTFORSCORECARD(OPERAND,(select count(Account_Number) from #TempTable where days_arrears > = 30)) 
		FROM #tempTT WHERE OPERAND ='COUNT_DAYSARREAR_30MORE_17M_LN' )
		BEGIN
			set @RL_COUNT_DAYSARREAR_30MORE_17M_LN = ( SELECT DBO.GetRESULTFORSCORECARD(OPERAND,(select count(Account_Number) from #TempTable where days_arrears > = 30)) 
													   FROM #tempTT WHERE OPERAND ='COUNT_DAYSARREAR_30MORE_17M_LN' )
			set @COUNT_DAYSARREAR_30MORE_17M_LN =  @EX_COUNT_DAYSARREAR_30MORE_17M_LN * LOG(@RL_COUNT_DAYSARREAR_30MORE_17M_LN + 1)
		END
		ELSE 
		BEGIN
			SET @RL_COUNT_DAYSARREAR_30MORE_17M_LN = 0.00
			set @COUNT_DAYSARREAR_30MORE_17M_LN = 0.00
		END
	END
ELSE
	BEGIN
		SET @RL_COUNT_DAYSARREAR_30MORE_17M_LN = 0.00
		set @COUNT_DAYSARREAR_30MORE_17M_LN = 0.00
	END

--***************************************** NUMBER_ACCOUNT_17M BZ *********************************************

IF EXISTS(SELECT * FROM #tempTT WHERE Operand = 'NUMBER_ACCOUNT_17M')
	BEGIN
		IF EXISTS(select DBO.GetRESULTFORSCORECARD(OPERAND,(select count(Account_Number) from #TempTable where OUTSTANDING_BALANCE > 0))
		FROM #tempTT WHERE OPERAND ='NUMBER_ACCOUNT_17M')
		BEGIN
			set @RL_NUMBER_ACCOUNT_17M = (select DBO.GetRESULTFORSCORECARD(OPERAND,(select count(Account_Number) from #TempTable where OUTSTANDING_BALANCE > 0))
										  FROM #tempTT WHERE OPERAND ='NUMBER_ACCOUNT_17M')
			set @NUMBER_ACCOUNT_17M = @EX_NUMBER_ACCOUNT_17M * @RL_NUMBER_ACCOUNT_17M
		END
		ELSE 
		BEGIN
			SET @RL_NUMBER_ACCOUNT_17M = 0.00
			set @NUMBER_ACCOUNT_17M = 0.00
		END
	END
ELSE
	BEGIN
		SET @RL_NUMBER_ACCOUNT_17M = 0.00
		set @NUMBER_ACCOUNT_17M = 0.00
	END

--************************************************** MAX_PERC_OUTS_3M_SQ (TT BZ *****************************************************

 if OBJECT_ID('tempdb..#calc') is not null
	 begin
	  drop table #calc
	 end

select ((x.agrmttotal/x.outstbal)*100) as AO100, x.mymonth  
	     into #calc
	     from (select SUM(AGREEMENT_TOTAL) as agrmttotal,SUM(OUTSTANDING_BALANCE) as outstbal, month(EXTRACT_DATE) 
	     as mymonth from #TempTable where OUTSTANDING_BALANCE>0  and EXTRACT_DATE < DATEADD(MONTH, DATEDIFF(MONTH, 0, DBO.FirstDayOfCurrentMonth(GETDATE())) - 3, 0)
	     group by month(EXTRACT_DATE) 
	   ) as x 

	IF EXISTS(SELECT * FROM #tempTT WHERE Operand = 'MAX_PERC_OUTS_3M_SQ')
		BEGIN
			IF EXISTS(SELECT DBO.GetRESULTFORSCORECARD(OPERAND,(select (MAX(AO100))  from #calc))
			FROM #tempTT WHERE OPERAND ='MAX_PERC_OUTS_3M_SQ')
			BEGIN
				set @RL_MAX_PERC_OUTS_3M_SQ = ( SELECT DBO.GetRESULTFORSCORECARD(OPERAND,(select (MAX(AO100))  from #calc))
												FROM #tempTT WHERE OPERAND ='MAX_PERC_OUTS_3M_SQ')
				set @MAX_PERC_OUTS_3M_SQ = @EX_MAX_PERC_OUTS_3M_SQ * SQUARE (@RL_MAX_PERC_OUTS_3M_SQ)
			END
			ELSE 
			BEGIN
				SET @RL_MAX_PERC_OUTS_3M_SQ = 0.00
				set @MAX_PERC_OUTS_3M_SQ = 0.00
			END
		END
	ELSE
		BEGIN
			SET @RL_MAX_PERC_OUTS_3M_SQ = 0.00
			set @MAX_PERC_OUTS_3M_SQ = 0.00
		END

drop table #calc


--*************************************************** MAX_PERC_OUTSARREARS_6M_LN  BB ************************************************

  if OBJECT_ID('tempdb..#calc7') is not null
	 begin
	  drop table #calc7
	 end

select ((x.balancearrears/x.outstbal)*100) as BO100, x.mymonth  
         into #calc7
         from (select SUM(case when BALANCE_ARREARS > 0 then BALANCE_ARREARS else 0 end) as balancearrears,SUM(OUTSTANDING_BALANCE) as outstbal, month(EXTRACT_DATE) 
         as mymonth from #TempTable where OUTSTANDING_BALANCE>0  and EXTRACT_DATE < DATEADD(MONTH, DATEDIFF(MONTH, 0, DBO.FirstDayOfCurrentMonth(GETDATE())) - 6, 0)
         group by month(EXTRACT_DATE) 
       ) as x 

IF EXISTS(SELECT * FROM #tempTT WHERE Operand = 'MAX_PERC_OUTSARREARS_6M_LN')
	BEGIN
		IF EXISTS(SELECT DBO.GetRESULTFORSCORECARD(OPERAND,(select (MAX(BO100))  from #calc7))
		FROM #tempTT WHERE OPERAND ='MAX_PERC_OUTSARREARS_6M_LN')
		BEGIN
			set @RL_MAX_PERC_OUTSARREARS_6M_LN = (SELECT DBO.GetRESULTFORSCORECARD(OPERAND,(select (MAX(BO100))  from #calc7))
												  FROM #tempTT WHERE OPERAND ='MAX_PERC_OUTSARREARS_6M_LN')
			set @MAX_PERC_OUTSARREARS_6M_LN = @EX_MAX_PERC_OUTSARREARS_6M_LN * LOG(@RL_MAX_PERC_OUTSARREARS_6M_LN+1)
		END
		ELSE 
		BEGIN
			SET @RL_MAX_PERC_OUTSARREARS_6M_LN =  0.00
			set @MAX_PERC_OUTSARREARS_6M_LN = 0.00
		END
	END
ELSE
	BEGIN
		SET @RL_MAX_PERC_OUTSARREARS_6M_LN = 0.00
		set @MAX_PERC_OUTSARREARS_6M_LN = 0.00
	END

drop table #calc7

--*******************************************AVG_AGREEMENT_TOTAL_1M_SQ  BZ ***********************************
	
IF EXISTS(SELECT * FROM #tempTT WHERE Operand = 'AVG_AGREEMENT_TOTAL_1M_SQ')
	BEGIN
		IF EXISTS(SELECT DBO.GetRESULTFORSCORECARD(OPERAND,(select (SUM(AGREEMENT_TOTAL)) 
				  from #TempTable where OUTSTANDING_BALANCE > 0  and EXTRACT_DATE < DATEADD(MONTH, DATEDIFF(MONTH, 0, DBO.FirstDayOfCurrentMonth(GETDATE())) - 1, 0))) 
				  FROM #tempTT WHERE OPERAND ='AVG_AGREEMENT_TOTAL_1M_SQ')
		BEGIN
			set @RL_AVG_AGREEMENT_TOTAL_1M_SQ = (SELECT DBO.GetRESULTFORSCORECARD(OPERAND,(select (SUM(AGREEMENT_TOTAL)) 
												 from #TempTable where OUTSTANDING_BALANCE > 0  and EXTRACT_DATE < DATEADD(MONTH, DATEDIFF(MONTH, 0, DBO.FirstDayOfCurrentMonth(GETDATE())) - 1, 0))) 
												 FROM #tempTT WHERE OPERAND ='AVG_AGREEMENT_TOTAL_1M_SQ')
			set  @AVG_AGREEMENT_TOTAL_1M_SQ   = @EX_AVG_AGREEMENT_TOTAL_1M_SQ * SQUARE(@RL_AVG_AGREEMENT_TOTAL_1M_SQ)
		END
		ELSE 
		BEGIN
			SET @RL_AVG_AGREEMENT_TOTAL_1M_SQ = 0.00
			set  @AVG_AGREEMENT_TOTAL_1M_SQ   = 0.00
		END
	END
ELSE
	BEGIN
		SET @RL_AVG_AGREEMENT_TOTAL_1M_SQ = 0.00
		set  @AVG_AGREEMENT_TOTAL_1M_SQ   = 0.00
	END

--******************************************* AVG_BALANCE_ARREARS_1M_LN BZ **************************************************

IF EXISTS(SELECT * FROM #tempTT WHERE Operand = 'AVG_BALANCE_ARREARS_1M_LN')
	BEGIN
		IF EXISTS(SELECT DBO.GetRESULTFORSCORECARD(OPERAND,(select SUM(case when BALANCE_ARREARS > 0 then BALANCE_ARREARS else 0 end)  from #TempTable where 
				  EXTRACT_DATE < DATEADD(MONTH, DATEDIFF(MONTH, 0, DBO.FirstDayOfCurrentMonth(GETDATE())) - 1, 0)))
				  FROM #tempTT WHERE OPERAND ='AVG_BALANCE_ARREARS_1M_LN')
		BEGIN
			set  @RL_AVG_BALANCE_ARREARS_1M_LN   = ( SELECT DBO.GetRESULTFORSCORECARD(OPERAND,(select SUM(case when BALANCE_ARREARS > 0 then BALANCE_ARREARS else 0 end)  from #TempTable where 
													 EXTRACT_DATE < DATEADD(MONTH, DATEDIFF(MONTH, 0, DBO.FirstDayOfCurrentMonth(GETDATE())) - 1, 0)))
													 FROM #tempTT WHERE OPERAND ='AVG_BALANCE_ARREARS_1M_LN')
			Set  @AVG_BALANCE_ARREARS_1M_LN   = @EX_AVG_BALANCE_ARREARS_1M_LN * LOG(@RL_AVG_BALANCE_ARREARS_1M_LN + 1)
		END
		ELSE 
		BEGIN
			SET @RL_AVG_BALANCE_ARREARS_1M_LN = 0.00
			Set  @AVG_BALANCE_ARREARS_1M_LN   = 0.00
		END
	END
ELSE
	BEGIN
		SET @RL_AVG_BALANCE_ARREARS_1M_LN = 0.00
		Set @AVG_BALANCE_ARREARS_1M_LN   = 0.00
	END

--******************************************* AVG_BALANCE_ARREARS_12M_LN ***************************************************************

IF EXISTS(SELECT * FROM #tempTT WHERE Operand = 'AVG_BALANCE_ARREARS_12M_LN')
	BEGIN
		IF EXISTS(SELECT DBO.GetRESULTFORSCORECARD1(OPERAND,( select (SUM(case when BALANCE_ARREARS > 0 then BALANCE_ARREARS else 0 end)/12)  from #TempTable where 
				  EXTRACT_DATE < DATEADD(MONTH, DATEDIFF(MONTH, 0, DBO.FirstDayOfCurrentMonth(GETDATE())) + 12, 0)))
				  FROM #tempTT WHERE OPERAND ='AVG_BALANCE_ARREARS_12M_LN')
		BEGIN
			set  @RL_AVG_BALANCE_ARREARS_12M_LN   = (SELECT DBO.GetRESULTFORSCORECARD1(OPERAND,( select (SUM(case when BALANCE_ARREARS > 0 then BALANCE_ARREARS else 0 end)/12)  from #TempTable where 
													 EXTRACT_DATE < DATEADD(MONTH, DATEDIFF(MONTH, 0, DBO.FirstDayOfCurrentMonth(GETDATE())) + 12, 0)))
													 FROM #tempTT WHERE OPERAND ='AVG_BALANCE_ARREARS_12M_LN')
			set  @AVG_BALANCE_ARREARS_12M_LN   = @EX_AVG_BALANCE_ARREARS_12M_LN * LOG(@RL_AVG_BALANCE_ARREARS_12M_LN + 1)
		END
		ELSE 
		BEGIN
			SET @RL_AVG_BALANCE_ARREARS_12M_LN = 0.00
			set  @AVG_BALANCE_ARREARS_12M_LN   =  0.00
		END
	END
ELSE
	BEGIN
		SET @RL_AVG_BALANCE_ARREARS_12M_LN = 0.00
		set  @AVG_BALANCE_ARREARS_12M_LN   =  0.00
	END

--************************************************* BALANCEARREARS_POUND_6M_LN (TTE ---BALANCEARREARS_POUND_6M BB  **********************************************************
  
    if OBJECT_ID('tempdb..#calc2') is not null
	 begin
	  drop table #calc2
	 end
if OBJECT_ID('tempdb..#calcvalues') is not null
	begin
		drop table #calcvalues
	end
	  
create tablE #calc2(ID int IDENTITY PRIMARY KEY,BALANCEARREARS money, MYMONTH int, MYYEAR int)
insert into #calc2
select SUM(case when BALANCE_ARREARS > 0 then BALANCE_ARREARS else 0 end) as balancearrears, month(EXTRACT_DATE) as mymonth, year(EXTRACT_DATE) as myYear
from #TempTable where EXTRACT_DATE < DATEADD(MONTH, DATEDIFF(MONTH, 0, DBO.FirstDayOfCurrentMonth(GETDATE())) - 6, 0)
group by datepart(month,EXTRACT_DATE), year(EXTRACT_DATE) 

create table #calcvalues(ROW_ID int  PRIMARY KEY, Value decimal(18,8))
insert into #calcvalues
SELECT 1,0.275
UNION ALL SELECT 2,0.225
UNION ALL SELECT 3,0.2
UNION ALL SELECT 4,0.15
UNION ALL SELECT 5,0.1
UNION ALL SELECT 6,0.05

IF EXISTS(SELECT * FROM #tempTT WHERE Operand = 'BALANCEARREARS_POUND_6M_LN')
	BEGIN
		IF EXISTS(SELECT DBO.GetRESULTFORSCORECARD(OPERAND,(select sum(BALANCEARREARS*Value)  from #calc2 c inner join #calcvalues cv on c.id = cv.row_id))
		FROM #tempTT WHERE OPERAND ='BALANCEARREARS_POUND_6M_LN')
		BEGIN
			set  @RL_BALANCEARREARS_POUND_6M_LN   = (SELECT DBO.GetRESULTFORSCORECARD(OPERAND,(select sum(BALANCEARREARS*Value)  from #calc2 c inner join #calcvalues cv on c.id = cv.row_id))
			FROM #tempTT WHERE OPERAND ='BALANCEARREARS_POUND_6M_LN')
			set  @BALANCEARREARS_POUND_6M_LN   = @EX_BALANCEARREARS_POUND_6M_LN * LOG(@RL_BALANCEARREARS_POUND_6M_LN + 1)
		END
		ELSE 
		BEGIN
			SET @BALANCEARREARS_POUND_6M_LN = 0.00
			set  @BALANCEARREARS_POUND_6M_LN   =0.00
		END
	END
ELSE
	BEGIN
		SET @BALANCEARREARS_POUND_6M_LN = 0.00
		set  @BALANCEARREARS_POUND_6M_LN   =0.00
	END
   
drop table #calc2
drop table #calcvalues
   
--************************************************* BALANCEARREARS_POUND_6M_LN TT ---BALANCEARREARS_POUND_6M BB  **********************************************************
  if OBJECT_ID('tempdb..#calc8') is not null
	 begin
	  drop table #calc8
	 end
if OBJECT_ID('tempdb..#calcvalues8') is not null
	begin
		drop table #calcvalues8
	end
	   
create tablE #calc8(ID int IDENTITY PRIMARY KEY,BALANCEARREARS money, MYMONTH int, MYYEAR int)
insert into #calc8
select SUM(case when BALANCE_ARREARS > 0 then BALANCE_ARREARS else 0 end) as balancearrears, month(EXTRACT_DATE) as mymonth, year(EXTRACT_DATE) as myYear
from #TempTable where EXTRACT_DATE < DATEADD(MONTH, DATEDIFF(MONTH, 0, DBO.FirstDayOfCurrentMonth(GETDATE())) - 6, 0)
group by datepart(month,EXTRACT_DATE), year(EXTRACT_DATE) 

create table #calcvalues8(ROW_ID int  PRIMARY KEY, Value decimal(18,8))
insert into #calcvalues8
SELECT 1,0.275
UNION ALL SELECT 2,0.225
UNION ALL SELECT 3,0.2
UNION ALL SELECT 4,0.15
UNION ALL SELECT 5,0.1
UNION ALL SELECT 6,0.05

IF EXISTS(SELECT * FROM #tempTT WHERE Operand = 'BALANCEARREARS_POUND_6M')
	BEGIN
		IF EXISTS(SELECT DBO.GetRESULTFORSCORECARD(OPERAND,(select sum(BALANCEARREARS*Value)  from #calc8 c inner join #calcvalues8 cv on c.id = cv.row_id))
		          FROM #tempTT WHERE OPERAND ='BALANCEARREARS_POUND_6M')
		BEGIN
			set  @RL_BALANCEARREARS_POUND_6M   = (SELECT DBO.GetRESULTFORSCORECARD(OPERAND,(select sum(BALANCEARREARS*Value)  from #calc8 c inner join #calcvalues8 cv on c.id = cv.row_id))
			                                      FROM #tempTT WHERE OPERAND ='BALANCEARREARS_POUND_6M')
			set  @BALANCEARREARS_POUND_6M   = @EX_BALANCEARREARS_POUND_6M * @RL_BALANCEARREARS_POUND_6M
		END
		ELSE 
		BEGIN
			SET @BALANCEARREARS_POUND_6M = 0.00
			set  @BALANCEARREARS_POUND_6M   =0.00
		END
	END
ELSE
	BEGIN
		SET @BALANCEARREARS_POUND_6M = 0.00
		set  @BALANCEARREARS_POUND_6M   =0.00
	END
   	
drop table #calc8
drop table #calcvalues8
   
--************************************************ DAYSARREARS_POUND_6M bz ************************************************************

 if OBJECT_ID('tempdb..#calc3') is not null
	 begin
		 drop table #calc3
	 end

if OBJECT_ID('tempdb..#calcvalues1') is not null
	begin
		drop table #calcvalues1
	end

create tablE #calc3(ID int IDENTITY PRIMARY KEY,DAYS_ARREARS int, MYMONTH int, MYYEAR int)
insert into #calc3
select SUM(DAYS_ARREARS) as days_arrears, month(EXTRACT_DATE) as mymonth, year(EXTRACT_DATE) as myYear
from #TempTable where EXTRACT_DATE < DATEADD(MONTH, DATEDIFF(MONTH, 0, DBO.FirstDayOfCurrentMonth(GETDATE())) - 6, 0)
group by datepart(month,EXTRACT_DATE), year(EXTRACT_DATE) 

create table #calcvalues1(ROW_ID int  PRIMARY KEY, Value decimal(18,8))
insert into #calcvalues1
SELECT 1,0.275
UNION ALL SELECT 2,0.225
UNION ALL SELECT 3,0.2
UNION ALL SELECT 4,0.15
UNION ALL SELECT 5,0.1
UNION ALL SELECT 6,0.05

IF EXISTS(SELECT * FROM #tempTT WHERE Operand = 'DAYSARREARS_POUND_6M')
	BEGIN
		IF EXISTS(SELECT DBO.GetRESULTFORSCORECARD(OPERAND,(select sum(days_arrears*Value)  from #calc3 c inner join #calcvalues1 cv on c.id = cv.row_id))
		          FROM #tempTT WHERE OPERAND ='DAYSARREARS_POUND_6M')
		BEGIN
			set  @RL_DAYSARREARS_POUND_6M    = (SELECT DBO.GetRESULTFORSCORECARD(OPERAND,(select sum(days_arrears*Value)  from #calc3 c inner join #calcvalues1 cv on c.id = cv.row_id))
			                                    FROM #tempTT WHERE OPERAND ='DAYSARREARS_POUND_6M')
			set  @DAYSARREARS_POUND_6M    = @EX_DAYSARREARS_POUND_6M * @RL_DAYSARREARS_POUND_6M
		END
		ELSE 
		BEGIN
			SET @RL_DAYSARREARS_POUND_6M = 0.00
			set @DAYSARREARS_POUND_6M    = 0.00
		END
	END
ELSE
	BEGIN
		SET @RL_DAYSARREARS_POUND_6M = 0.00
		set @DAYSARREARS_POUND_6M    = 0.00
	END

drop table #calc3
drop table #calcvalues1
			
--********************************************* NUMBER_ACCOUNT_OPENED_3M **************************
			
IF EXISTS(SELECT * FROM #tempTT WHERE Operand = 'NUMBER_ACCOUNT_OPENED_3M')
	BEGIN
		IF EXISTS(SELECT (DBO.GetRESULTFORSCORECARD(OPERAND,(SELECT count(*)													--TT FINAL 
				  FROM custacct ca INNER JOIN acct a on ca.acctno = a.acctno
				  where cast(a.dateacctopen as date)  between (select  CAST(EOMONTH(DATEADD(month, -(3+1), GETDATE()))AS date))
				  and  dbo.FirstDayOfCurrentMonth(getdate()) 
				  and ca.custid = @customerID)))
				  FROM #tempTT WHERE OPERAND ='NUMBER_ACCOUNT_OPENED_3M')
		BEGIN
			Set @RL_NUMBER_ACCOUNT_OPENED_3M =	( SELECT (DBO.GetRESULTFORSCORECARD(OPERAND,(SELECT count(*)													--TT FINAL 
												  FROM custacct ca INNER JOIN acct a on ca.acctno = a.acctno
												  where cast(a.dateacctopen as date)  between (select  CAST(EOMONTH(DATEADD(month, -(3+1), GETDATE()))AS date))
												  and  dbo.FirstDayOfCurrentMonth(getdate()) 
												  and ca.custid = @customerID)))
												  FROM #tempTT WHERE OPERAND ='NUMBER_ACCOUNT_OPENED_3M')
			Set @NUMBER_ACCOUNT_OPENED_3M = @EX_NUMBER_ACCOUNT_OPENED_3M * 	@RL_NUMBER_ACCOUNT_OPENED_3M	
		END
		ELSE 
		BEGIN
			SET @RL_NUMBER_ACCOUNT_OPENED_3M = 0.00
			Set @NUMBER_ACCOUNT_OPENED_3M = 0.00
		END
	END
ELSE
	BEGIN
		SET @RL_NUMBER_ACCOUNT_OPENED_3M = 0.00
		Set @NUMBER_ACCOUNT_OPENED_3M = 0.00
	END

--************************************************* NUMBER_ACCOUNT_OPENED_3M_CR *****************************************************************************

IF EXISTS(SELECT * FROM #tempTT WHERE Operand = 'NUMBER_ACCOUNT_OPENED_3M_CR')
	BEGIN
		IF EXISTS(SELECT (DBO.GetRESULTFORSCORECARD(OPERAND,(SELECT count(*)													--TT FINAL 
				  FROM custacct ca INNER JOIN acct a on ca.acctno = a.acctno
				  where cast(a.dateacctopen as date)  between (select  CAST(EOMONTH(DATEADD(month, -(3+1), GETDATE()))AS date))
				  and  dbo.FirstDayOfCurrentMonth(getdate()) 
				  and ca.custid = @customerID)))
				  FROM #tempTT WHERE OPERAND ='NUMBER_ACCOUNT_OPENED_3M_CR')
		BEGIN
			Set @RL_NUMBER_ACCOUNT_OPENED_3M_CR =	( SELECT (DBO.GetRESULTFORSCORECARD(OPERAND,(SELECT count(*)													--TT FINAL 
													  FROM custacct ca INNER JOIN acct a on ca.acctno = a.acctno
													  where cast(a.dateacctopen as date)  between (select  CAST(EOMONTH(DATEADD(month, -(3+1), GETDATE()))AS date))
													  and  dbo.FirstDayOfCurrentMonth(getdate()) 
													  and ca.custid = @customerID)))
													  FROM #tempTT WHERE OPERAND ='NUMBER_ACCOUNT_OPENED_3M_CR')
			Set @NUMBER_ACCOUNT_OPENED_3M_CR = @EX_NUMBER_ACCOUNT_OPENED_3M_CR * POWER(@RL_NUMBER_ACCOUNT_OPENED_3M_CR,3)
		END
		ELSE 
		BEGIN
			SET @RL_NUMBER_ACCOUNT_OPENED_3M_CR = 0.00
			Set @NUMBER_ACCOUNT_OPENED_3M_CR = 0.00
		END
	END
ELSE
	BEGIN
		SET @RL_NUMBER_ACCOUNT_OPENED_3M_CR = 0.00
		Set @NUMBER_ACCOUNT_OPENED_3M_CR = 0.00
	END

-- *****************************************************************************************************

BEGIN
	
	SET NOCOUNT ON;
	DECLARE @CHK decimal(25,15)
	/*DECLARE @CHK decimal(25,15) = (@InterceptScore
+@AGE --Added by nilesh
+@Flag_CustomerStatus_His_WOE  --Added by nilesh
+ @OCCUPATION_WOE
+ @MARITALSTATUS_WOE
+ @RESIDENTIALSTATUS_WOE
+ @MOBILENUMBER_WOE
+ @TIMECURRENTADDRESS
+ @TIMECURRENTADDRESS_LN
+ @TIMECURRENTADDRESS_WOE 
+ @EMPLOYMENTSTATUS_WOE
+ @GENDER_WOE
+ @POSTCODE_WOE
- @NUMBERDEPENDENTS
- @NUMBERDEPENDENTS_SQ 
- @NUMBERDEPENDENTS_CR
+ @TIMECURRENTEMPLOYMENT_SR
+ @TIMECURRENTEMPLOYMENT_LN 
+ @TIMECURRENTEMPLOYMENT
+ @RATIO_TCURRENTEMPLOY_TO_AGE
+ @RATIO_NDEPENDENT_TO_AGE
- @NUMBER_ACCOUNT_OPENED_3M 
- @NUMBER_ACCOUNT_OPENED_3M_CR 
+ @OLDEST_CREDIT_LN 
- @NEWEST_CREDIT_SQ
- @COUNT_DAYSARREAR_60MORE_17M_LN 
- @COUNT_DAYSARREAR_30MORE_17M_LN 
+ @NUMBER_ACCOUNT_17M
- @MAX_PERC_OUTS_3M_SQ 
- @EX_MAX_PERC_OUTSARREARS_6M_LN 
+ @AVG_AGREEMENT_TOTAL_1M_SQ 
- @AVG_BALANCE_ARREARS_1M_LN 
- @AVG_BALANCE_ARREARS_12M_LN
- @BALANCEARREARS_POUND_6M_LN 
- @BALANCEARREARS_POUND_6M 
- @DAYSARREARS_POUND_6M )*/
 If (@country='B' AND @scoreType = 'C')
BEGIN 
SET @CHK =(@InterceptScore
+@Age
+@TimeCurrentAddress
-@NumberDependents
+@Gender_WOE
+@Flag_CustomerStatus_His_WOE
+@MaritalStatus_WOE
+@Occupation_WOE
+@EmploymentStatus_WOE
+@PostCode_WOE
+@Ratio_TCurrentEmploy_to_age)
END
ELSE If (@country='B' AND @scoreType = 'D')
BEGIN 
SET @CHK = (@InterceptScore
+@Oldest_Credit_ln
+@TimeCurrentEmployment_ln
+@Occupation_WOE
+@Age
+@TimeCurrentAddress_ln
+@PostCode_WOE
+@MaritalStatus_WOE
-@NumberDependents
-@Max_Perc_OutsArrears_6m_ln
-@Count_DaysArrear_30more_17m_ln
-@BalanceArrears_Pound_6M)
END
ELSE If (@country='Z' AND @scoreType = 'C')
BEGIN 
SET @CHK = (@InterceptScore
+@MaritalStatus_WOE
+@Occupation_WOE
+@Flag_CustomerStatus_His_WOE
+@TimeCurrentEmployment_sr
+@TimeCurrentAddress_ln
+@ResidentialStatus_WOE)
END
ELSE If (@country='Z' AND @scoreType = 'D')
BEGIN
SET @CHK =(@InterceptScore
+@MaritalStatus_WOE
+@Occupation_WOE
+@ResidentialStatus_WOE
+@EmploymentStatus_WOE
+@Oldest_Credit_ln
-@Newest_Credit_sq
+@Number_Account_17m
-@Number_Account_Opened_3m_cr
+@TimeCurrentEmployment
+@TimeCurrentAddress_ln
-@NumberDependents_cr
+@Avg_Agreement_Total_1m_sq
-@Avg_Balance_Arrears_1m_ln
-@Avg_Balance_Arrears_12m_ln
-@DaysArrears_Pound_6M
-@Max_Perc_Outs_3m_sq) 
END
ELSE
BEGIN
 --By default score will calculate by TT RULE
	If (@scoreType = 'C')
	BEGIN 
	SET @CHK = (@InterceptScore
	+@Age
	+@Flag_CustomerStatus_His_WOE
	+@MaritalStatus_WOE
	+@Occupation_WOE
	+@ResidentialStatus_WOE
	+@MobileNumber_WOE
	+@Ratio_TCurrentEmploy_to_age
	-@Ratio_Ndependent_to_age)
	END
	ELSE --If (scoreType = 'D')
	BEGIN 
	SET @CHK = (@InterceptScore
	+@Occupation_WOE
	+@ResidentialStatus_WOE
	+@MaritalStatus_WOE
	+@Oldest_Credit_ln
	+@Age
	-@Max_Perc_Outs_3m_sq
	+@TimeCurrentEmployment_sr
	-@Number_Account_Opened_3m
	-@NumberDependents_sq
	-@Newest_Credit_sq
	-@Count_DaysArrear_60more_17m_ln
	-@BalanceArrears_Pound_6M_ln)
	END
END

--IF(@CHK > 0)
--BEGIN

SET @LOGODDS  = @CHK -  @LOGValue
 

SET @SCORE =  round (((exp (@LOGODDS) / (1 + exp (@LOGODDS))) * 1000),0)

--END
--ELSE
--BEGIN
--SET @SCORE = 0
--END

	

IF @SCORE < 1
BEGIN
SET @SCORE =1
END
ELSE IF @SCORE > 999
BEGIN
SET @SCORE = 999
END
ELSE 
BEGIN
SET @SCORE = @SCORE
END

/*
IF EXISTS (SELECT * FROM [dbo].[EquifaxScore] WHERE CustId =@customerID AND AccountNo = @accountNo AND Country = @country AND ScoreCard = @scoreType )
BEGIN

	UPDATE [dbo].[EquifaxScore]
	SET 
	InterceptScore = @InterceptScore,
	LOGODDS = @LOGODDS,
	[LOG] = @LOGValue,
	SCORE = @SCORE,
	AGE = @AGE,
	OCCUPATION_WOE = @OCCUPATION_WOE,
	MARITALSTATUS_WOE = @MARITALSTATUS_WOE ,
	RESIDENTIALSTATUS_WOE = @RESIDENTIALSTATUS_WOE,
	MOBILENUMBER_WOE = @MOBILENUMBER_WOE,
	TIMECURRENTADDRESS = @TIMECURRENTADDRESS ,
	TIMECURRENTADDRESS_LN = @TIMECURRENTADDRESS_LN,
	TIMECURRENTADDRESS_WOE = @TIMECURRENTADDRESS_WOE,
	EMPLOYMENTSTATUS_WOE =@EMPLOYMENTSTATUS_WOE,
	GENDER_WOE = @GENDER_WOE,
	POSTCODE_WOE = @POSTCODE_WOE ,
	NUMBERDEPENDENTS =@NUMBERDEPENDENTS,
	NUMBERDEPENDENTS_SQ =@NUMBERDEPENDENTS_SQ ,
	NUMBERDEPENDENTS_CR =@NUMBERDEPENDENTS_CR,
	TIMECURRENTEMPLOYMENT_SR =@TIMECURRENTEMPLOYMENT_SR ,
	TIMECURRENTEMPLOYMENT_LN =@TIMECURRENTEMPLOYMENT_LN ,
	TIMECURRENTEMPLOYMENT = @TIMECURRENTEMPLOYMENT ,
	RATIO_TCURRENTEMPLOY_TO_AGE =@RATIO_TCURRENTEMPLOY_TO_AGE,
	RATIO_NDEPENDENT_TO_AGE = @RATIO_NDEPENDENT_TO_AGE,
	NUMBER_ACCOUNT_OPENED_3M =@NUMBER_ACCOUNT_OPENED_3M ,
	NUMBER_ACCOUNT_OPENED_3M_CR =@NUMBER_ACCOUNT_OPENED_3M_CR ,
	OLDEST_CREDIT_LN  =@OLDEST_CREDIT_LN,
	NEWEST_CREDIT_SQ =@NEWEST_CREDIT_SQ,
	COUNT_DAYSARREAR_60MORE_17M_LN =@COUNT_DAYSARREAR_60MORE_17M_LN,
	COUNT_DAYSARREAR_30MORE_17M_LN =@COUNT_DAYSARREAR_30MORE_17M_LN ,
	NUMBER_ACCOUNT_17M =@NUMBER_ACCOUNT_17M ,
	MAX_PERC_OUTS_3M_SQ =@MAX_PERC_OUTS_3M_SQ ,
	MAX_PERC_OUTSARREARS_6M_LN =@MAX_PERC_OUTSARREARS_6M_LN ,
	AVG_AGREEMENT_TOTAL_1M_SQ =@AVG_AGREEMENT_TOTAL_1M_SQ,
	AVG_BALANCE_ARREARS_1M_LN =@AVG_BALANCE_ARREARS_1M_LN ,
	AVG_BALANCE_ARREARS_12M_LN = @AVG_BALANCE_ARREARS_12M_LN,
	BALANCEARREARS_POUND_6M_LN =@BALANCEARREARS_POUND_6M_LN,
	BALANCEARREARS_POUND_6M =@BALANCEARREARS_POUND_6M ,
	DAYSARREARS_POUND_6M  =@DAYSARREARS_POUND_6M,
	FLAG_CUSTOMERSTATUS_HIS_WOE  =@FLAG_CUSTOMERSTATUS_HIS_WOE,  --NILESH

	RL_AGE =@RL_AGE,
	RL_OCCUPATION_WOE =@RL_OCCUPATION_WOE,
	RL_MARITALSTATUS_WOE =@RL_MARITALSTATUS_WOE,
	RL_RESIDENTIALSTATUS_WOE =@RL_RESIDENTIALSTATUS_WOE,
	RL_MOBILENUMBER_WOE =@RL_MOBILENUMBER_WOE,
	RL_TIMECURRENTADDRESS =@RL_TIMECURRENTADDRESS,
	RL_TIMECURRENTADDRESS_LN =@RL_TIMECURRENTADDRESS_LN,
	RL_TIMECURRENTADDRESS_WOE =@RL_TIMECURRENTADDRESS_WOE ,
	RL_EMPLOYMENTSTATUS_WOE =@RL_EMPLOYMENTSTATUS_WOE ,
	RL_GENDER_WOE =@RL_GENDER_WOE,
	RL_POSTCODE_WOE =@RL_POSTCODE_WOE,
	RL_NUMBERDEPENDENTS =@RL_NUMBERDEPENDENTS,
	RL_NUMBERDEPENDENTS_SQ =@RL_NUMBERDEPENDENTS_SQ ,
	RL_NUMBERDEPENDENTS_CR =@RL_NUMBERDEPENDENTS_CR ,
	RL_TIMECURRENTEMPLOYMENT_SR =@RL_TIMECURRENTEMPLOYMENT_SR,
	RL_TIMECURRENTEMPLOYMENT_LN  =@RL_TIMECURRENTEMPLOYMENT_LN,
	RL_TIMECURRENTEMPLOYMENT =@RL_TIMECURRENTEMPLOYMENT,
	RL_RATIO_TCURRENTEMPLOY_TO_AGE =@RL_RATIO_TCURRENTEMPLOY_TO_AGE,
	RL_RATIO_NDEPENDENT_TO_AGE =@RL_RATIO_NDEPENDENT_TO_AGE,
	RL_NUMBER_ACCOUNT_OPENED_3M  =@RL_NUMBER_ACCOUNT_OPENED_3M ,
	RL_NUMBER_ACCOUNT_OPENED_3M_CR =@RL_NUMBER_ACCOUNT_OPENED_3M_CR,
	RL_OLDEST_CREDIT_LN =@RL_OLDEST_CREDIT_LN ,
	RL_NEWEST_CREDIT_SQ =@RL_NEWEST_CREDIT_SQ,
	RL_COUNT_DAYSARREAR_60MORE_17M_LN =@RL_COUNT_DAYSARREAR_60MORE_17M_LN ,
	RL_COUNT_DAYSARREAR_30MORE_17M_LN =@RL_COUNT_DAYSARREAR_30MORE_17M_LN,
	RL_NUMBER_ACCOUNT_17M =@RL_NUMBER_ACCOUNT_17M,
	RL_MAX_PERC_OUTS_3M_SQ =@RL_MAX_PERC_OUTS_3M_SQ,
	RL_MAX_PERC_OUTSARREARS_6M_LN =@RL_MAX_PERC_OUTSARREARS_6M_LN ,
	RL_AVG_AGREEMENT_TOTAL_1M_SQ  =@RL_AVG_AGREEMENT_TOTAL_1M_SQ,
	RL_AVG_BALANCE_ARREARS_1M_LN =@RL_AVG_BALANCE_ARREARS_1M_LN,
	RL_AVG_BALANCE_ARREARS_12M_LN =@RL_AVG_BALANCE_ARREARS_12M_LN,
	RL_BALANCEARREARS_POUND_6M_LN =@RL_BALANCEARREARS_POUND_6M_LN,
	RL_BALANCEARREARS_POUND_6M =@RL_BALANCEARREARS_POUND_6M,
	RL_DAYSARREARS_POUND_6M  =@RL_DAYSARREARS_POUND_6M,
	RL_FLAG_CUSTOMERSTATUS_HIS_WOE  =@RL_FLAG_CUSTOMERSTATUS_HIS_WOE, --NILESH

	EX_AGE =@EX_AGE,
	EX_OCCUPATION_WOE =@EX_OCCUPATION_WOE ,
	EX_MARITALSTATUS_WOE =@EX_MARITALSTATUS_WOE,
	EX_RESIDENTIALSTATUS_WOE =@EX_RESIDENTIALSTATUS_WOE,
	EX_MOBILENUMBER_WOE = @EX_MOBILENUMBER_WOE,
	EX_TIMECURRENTADDRESS =@EX_TIMECURRENTADDRESS,
	EX_TIMECURRENTADDRESS_LN =@EX_TIMECURRENTADDRESS_LN,
	EX_TIMECURRENTADDRESS_WOE = @EX_TIMECURRENTADDRESS_WOE,
	EX_EMPLOYMENTSTATUS_WOE =@EX_EMPLOYMENTSTATUS_WOE,
	EX_GENDER_WOE =@EX_GENDER_WOE,
	EX_POSTCODE_WOE =@EX_POSTCODE_WOE,
	EX_NUMBERDEPENDENTS =@EX_NUMBERDEPENDENTS,
	EX_NUMBERDEPENDENTS_SQ =@EX_NUMBERDEPENDENTS_SQ ,
	EX_NUMBERDEPENDENTS_CR =@EX_NUMBERDEPENDENTS_CR,
	EX_TIMECURRENTEMPLOYMENT_SR =@EX_TIMECURRENTEMPLOYMENT_SR,
	EX_TIMECURRENTEMPLOYMENT_LN =@EX_TIMECURRENTEMPLOYMENT_LN,
	EX_TIMECURRENTEMPLOYMENT =@EX_TIMECURRENTEMPLOYMENT,
	EX_RATIO_TCURRENTEMPLOY_TO_AGE =@EX_RATIO_TCURRENTEMPLOY_TO_AGE,
	EX_RATIO_NDEPENDENT_TO_AGE =@EX_RATIO_NDEPENDENT_TO_AGE,
	EX_NUMBER_ACCOUNT_OPENED_3M =@EX_NUMBER_ACCOUNT_OPENED_3M,
	EX_NUMBER_ACCOUNT_OPENED_3M_CR =@EX_NUMBER_ACCOUNT_OPENED_3M_CR ,
	EX_OLDEST_CREDIT_LN =@EX_OLDEST_CREDIT_LN,
	EX_NEWEST_CREDIT_SQ =@EX_NEWEST_CREDIT_SQ,
	EX_COUNT_DAYSARREAR_60MORE_17M_LN =@EX_COUNT_DAYSARREAR_60MORE_17M_LN ,
	EX_COUNT_DAYSARREAR_30MORE_17M_LN =@EX_COUNT_DAYSARREAR_30MORE_17M_LN,
	EX_NUMBER_ACCOUNT_17M =@EX_NUMBER_ACCOUNT_17M,
	EX_MAX_PERC_OUTS_3M_SQ  =@EX_MAX_PERC_OUTS_3M_SQ,
	EX_MAX_PERC_OUTSARREARS_6M_LN  =@EX_MAX_PERC_OUTSARREARS_6M_LN,
	EX_AVG_AGREEMENT_TOTAL_1M_SQ =@EX_AVG_AGREEMENT_TOTAL_1M_SQ,
	EX_AVG_BALANCE_ARREARS_1M_LN =@EX_AVG_BALANCE_ARREARS_1M_LN,
	EX_AVG_BALANCE_ARREARS_12M_LN =@EX_AVG_BALANCE_ARREARS_12M_LN,
	EX_BALANCEARREARS_POUND_6M_LN =@EX_BALANCEARREARS_POUND_6M_LN,
	EX_BALANCEARREARS_POUND_6M =@EX_BALANCEARREARS_POUND_6M,
	EX_DAYSARREARS_POUND_6M =@EX_DAYSARREARS_POUND_6M,
	EX_FLAG_CUSTOMERSTATUS_HIS_WOE =@EX_FLAG_CUSTOMERSTATUS_HIS_WOE,  --nILESH
	EX_FLAG_CUSTOMERSTATUS_HIS_WOE_B =@EX_FLAG_CUSTOMERSTATUS_HIS_WOE_B, --NILESH
	SCOREDATE = GETDATE()
	WHERE CustId =@customerID AND AccountNo = @accountNo AND Country = @country AND ScoreCard = @scoreType

END
ELSE
BEGIN
*/
-- select * from EquifaxScore
insert into [dbo].[EquifaxScore] VALUES (

@customerID ,
@accountNo ,
@country ,
@scoreType,	
@InterceptScore ,
@LOGODDS ,
@LOGValue ,
@SCORE ,
@AGE ,
@OCCUPATION_WOE,
@MARITALSTATUS_WOE ,
@RESIDENTIALSTATUS_WOE ,
@MOBILENUMBER_WOE ,
@TIMECURRENTADDRESS ,
@TIMECURRENTADDRESS_LN ,
@TIMECURRENTADDRESS_WOE ,
@EMPLOYMENTSTATUS_WOE ,
@GENDER_WOE ,
@POSTCODE_WOE ,
@NUMBERDEPENDENTS ,
@NUMBERDEPENDENTS_SQ  ,
@NUMBERDEPENDENTS_CR ,
@TIMECURRENTEMPLOYMENT_SR ,
@TIMECURRENTEMPLOYMENT_LN ,
@TIMECURRENTEMPLOYMENT ,
@RATIO_TCURRENTEMPLOY_TO_AGE ,
@RATIO_NDEPENDENT_TO_AGE,
@NUMBER_ACCOUNT_OPENED_3M  ,
@NUMBER_ACCOUNT_OPENED_3M_CR  ,
@OLDEST_CREDIT_LN  ,
@NEWEST_CREDIT_SQ ,
@COUNT_DAYSARREAR_60MORE_17M_LN ,
@COUNT_DAYSARREAR_30MORE_17M_LN  ,
@NUMBER_ACCOUNT_17M ,
@MAX_PERC_OUTS_3M_SQ  ,
@MAX_PERC_OUTSARREARS_6M_LN  ,
@AVG_AGREEMENT_TOTAL_1M_SQ ,
@AVG_BALANCE_ARREARS_1M_LN  ,
@AVG_BALANCE_ARREARS_12M_LN,
@BALANCEARREARS_POUND_6M_LN ,
@BALANCEARREARS_POUND_6M  ,
@DAYSARREARS_POUND_6M  ,
@Flag_Customerstatus_His_Woe, --NILESH

@RL_AGE ,
@RL_OCCUPATION_WOE ,
@RL_MARITALSTATUS_WOE ,
@RL_RESIDENTIALSTATUS_WOE ,
@RL_MOBILENUMBER_WOE ,
@RL_TIMECURRENTADDRESS ,
@RL_TIMECURRENTADDRESS_LN ,
@RL_TIMECURRENTADDRESS_WOE  ,
@RL_EMPLOYMENTSTATUS_WOE ,
@RL_GENDER_WOE ,
@RL_POSTCODE_WOE ,
@RL_NUMBERDEPENDENTS ,
@RL_NUMBERDEPENDENTS_SQ  ,
@RL_NUMBERDEPENDENTS_CR ,
@RL_TIMECURRENTEMPLOYMENT_SR ,
@RL_TIMECURRENTEMPLOYMENT_LN  ,
@RL_TIMECURRENTEMPLOYMENT ,
@RL_RATIO_TCURRENTEMPLOY_TO_AGE,
@RL_RATIO_NDEPENDENT_TO_AGE ,
@RL_NUMBER_ACCOUNT_OPENED_3M  ,
@RL_NUMBER_ACCOUNT_OPENED_3M_CR ,
@RL_OLDEST_CREDIT_LN ,
@RL_NEWEST_CREDIT_SQ ,
@RL_COUNT_DAYSARREAR_60MORE_17M_LN  ,
@RL_COUNT_DAYSARREAR_30MORE_17M_LN ,
@RL_NUMBER_ACCOUNT_17M ,
@RL_MAX_PERC_OUTS_3M_SQ ,
@RL_MAX_PERC_OUTSARREARS_6M_LN  ,
@RL_AVG_AGREEMENT_TOTAL_1M_SQ  ,
@RL_AVG_BALANCE_ARREARS_1M_LN ,
@RL_AVG_BALANCE_ARREARS_12M_LN ,
@RL_BALANCEARREARS_POUND_6M_LN ,
@RL_BALANCEARREARS_POUND_6M ,
@RL_DAYSARREARS_POUND_6M  ,
@Rl_Flag_Customerstatus_His_Woe, --NILESH

@EX_AGE ,
@EX_OCCUPATION_WOE ,
@EX_MARITALSTATUS_WOE ,
@EX_RESIDENTIALSTATUS_WOE ,
@EX_MOBILENUMBER_WOE,
@EX_TIMECURRENTADDRESS ,
@EX_TIMECURRENTADDRESS_LN ,
@EX_TIMECURRENTADDRESS_WOE ,
@EX_EMPLOYMENTSTATUS_WOE ,
@EX_GENDER_WOE ,
@EX_POSTCODE_WOE ,
@EX_NUMBERDEPENDENTS ,
@EX_NUMBERDEPENDENTS_SQ  ,
@EX_NUMBERDEPENDENTS_CR ,
@EX_TIMECURRENTEMPLOYMENT_SR ,
@EX_TIMECURRENTEMPLOYMENT_LN ,
@EX_TIMECURRENTEMPLOYMENT ,
@EX_RATIO_TCURRENTEMPLOY_TO_AGE ,
@EX_RATIO_NDEPENDENT_TO_AGE ,
@EX_NUMBER_ACCOUNT_OPENED_3M ,
@EX_NUMBER_ACCOUNT_OPENED_3M_CR  ,
@EX_OLDEST_CREDIT_LN ,
@EX_NEWEST_CREDIT_SQ ,
@EX_COUNT_DAYSARREAR_60MORE_17M_LN  ,
@EX_COUNT_DAYSARREAR_30MORE_17M_LN ,
@EX_NUMBER_ACCOUNT_17M ,
@EX_MAX_PERC_OUTS_3M_SQ  ,
@EX_MAX_PERC_OUTSARREARS_6M_LN  ,
@EX_AVG_AGREEMENT_TOTAL_1M_SQ ,
@EX_AVG_BALANCE_ARREARS_1M_LN ,
@EX_AVG_BALANCE_ARREARS_12M_LN ,
@EX_BALANCEARREARS_POUND_6M_LN ,
@EX_BALANCEARREARS_POUND_6M ,
@EX_DAYSARREARS_POUND_6M ,
@Ex_Flag_Customerstatus_His_Woe, --NILESH
@Ex_Flag_Customerstatus_His_Woe_B,--NILESH
getdate()
)

END 

IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END

	select @SCORE
return @return

--***********************************************************************************************************************************

--select @InterceptScore as '@InterceptScore' , @TIMECURRENTEMPLOYMENT_SR as '@TIMECURRENTEMPLOYMENT_SR', @TIMECURRENTEMPLOYMENT_LN as '@TIMECURRENTEMPLOYMENT_LN',
--@TIMECURRENTEMPLOYMENT as '@TIMECURRENTEMPLOYMENT', @OCCUPATION_WOE as '@OCCUPATION_WOE', @RESIDENTIALSTATUS_WOE as '@RESIDENTIALSTATUS_WOE',
--@MARITALSTATUS_WOE as '@MARITALSTATUS_WOE', @AGE as '@AGE' , @NUMBERDEPENDENTS as '@NUMBERDEPENDENTS', @NUMBERDEPENDENTS_SQ as '@NUMBERDEPENDENTS_SQ',
--@NUMBERDEPENDENTS_CR as '@NUMBERDEPENDENTS_CR', @NUMBER_ACCOUNT_OPENED_3M as '@NUMBER_ACCOUNT_OPENED_3M', @NUMBER_ACCOUNT_OPENED_3M_CR as '@NUMBER_ACCOUNT_OPENED_3M_CR',
--@TIMECURRENTADDRESS_WOE as '@TIMECURRENTADDRESS_WOE',  @TIMECURRENTADDRESS_LN  as '@TIMECURRENTADDRESS_LN'
--, @POSTCODE_WOE  as '@POSTCODE_WOE'
--, @EMPLOYMENTSTATUS_WOE as '@EMPLOYMENTSTATUS_WOE'
--, @OLDEST_CREDIT_LN as '@OLDEST_CREDIT_LN'
--, @NEWEST_CREDIT_SQ as '@NEWEST_CREDIT_SQ'
--, @COUNT_DAYSARREAR_60MORE_17M_LN as '@COUNT_DAYSARREAR_60MORE_17M_LN'
--, @COUNT_DAYSARREAR_30MORE_17M_LN  as '@COUNT_DAYSARREAR_30MORE_17M_LN'
--, @NUMBER_ACCOUNT_17M as '@NUMBER_ACCOUNT_17M'
--, @MAX_PERC_OUTS_3M_SQ as '@MAX_PERC_OUTS_3M_SQ'
--, @EX_MAX_PERC_OUTSARREARS_6M_LN as '@EX_MAX_PERC_OUTSARREARS_6M_LN'
--, @AVG_AGREEMENT_TOTAL_1M_SQ as '@AVG_AGREEMENT_TOTAL_1M_SQ'
--, @AVG_BALANCE_ARREARS_1M_LN as '@AVG_BALANCE_ARREARS_1M_LN'
--, @AVG_BALANCE_ARREARS_12M_LN as '@AVG_BALANCE_ARREARS_12M_LN'
--, @BALANCEARREARS_POUND_6M_LN as '@BALANCEARREARS_POUND_6M_LN'
--, @BALANCEARREARS_POUND_6M as '@BALANCEARREARS_POUND_6M'
--, @DAYSARREARS_POUND_6M as '@BALANCEARREARS_POUND_6M'
--, @NUMBER_ACCOUNT_OPENED_3M as '@NUMBER_ACCOUNT_OPENED_3M'
--, @NUMBER_ACCOUNT_OPENED_3M_CR as '@NUMBER_ACCOUNT_OPENED_3M_CR'
--, @OCCUPATION_WOE AS '@OCCUPATION_WOE', @MARITALSTATUS_WOE AS '@MARITALSTATUS_WOE'
--, @AGE AS '@AGE'
--, @RESIDENTIALSTATUS_WOE AS '@RESIDENTIALSTATUS_WOE'
--, @MOBILENUMBER_WOE AS '@MOBILENUMBER_WOE'
--, @TIMECURRENTADDRESS AS '@TIMECURRENTADDRESS'
--, @TIMECURRENTADDRESS_LN AS '@TIMECURRENTADDRESS_LN'
--, @EMPLOYMENTSTATUS_WOE AS '@EMPLOYMENTSTATUS_WOE'
--, @GENDER_WOE AS '@GENDER_WOE'
--, @POSTCODE_WOE AS '@POSTCODE_WOE'
--, @NUMBERDEPENDENTS AS '@NUMBERDEPENDENTS'
--, @TIMECURRENTEMPLOYMENT_SR AS '@TIMECURRENTEMPLOYMENT_SR'
--, @RATIO_TCURRENTEMPLOY_TO_AGE AS '@RATIO_TCURRENTEMPLOY_TO_AGE'
--, @RATIO_NDEPENDENT_TO_AGE AS '@RATIO_NDEPENDENT_TO_AGE'

--***********************************************************************************************************************************

--END

