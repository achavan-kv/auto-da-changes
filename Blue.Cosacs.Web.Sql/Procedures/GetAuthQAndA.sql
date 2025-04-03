/*
--*********************************************************************** 
-- Script Name : GetAuthQAndA.sql 
-- Created For  : Unipay (T) 
-- Created By   : Zensar(Sagar Kute)
-- Created On   : 10/07/2018 
--***********************************************************************
-- Change Control 
-- -------------- 
-- Date(DD/MM/YYYY)		Changed By(FName LName)		Description 
-- ------------------------------------------------------------------------------------------------------- 
1. 05/03/2019			Zensar(Sagar Kute)			Removed phone number question.
2. 
--*********************************************************************************************************

*/

IF EXISTS ( SELECT * FROM sysobjects WHERE NAME = 'GetAuthQAndA' )
BEGIN
	DROP PROCEDURE [dbo].[GetAuthQAndA]
END
GO

CREATE PROCEDURE [dbo].[GetAuthQAndA]
	@CustId VARCHAR(20) = N''
AS
BEGIN
	Declare @recordCount AS integer
	Declare @Status As nvarchar(6)
	Declare @questionanswersRecords AS integer

	if Exists (select Custid from Customer where Custid=@CustId) 
	BEGIN
		Select @recordCount=Count(*) from acct AS A , custacct As B where A.acctno=B.acctno and B.custid=@CustId and accttype IN ('C', 'R')
		Print @recordCount
		if @recordCount=1 
		Begin
			Select @Status=accttype from acct AS A , custacct As B where A.acctno=B.acctno and B.custid=@CustId and accttype IN ('C', 'R')
			Print @Status
			If @Status='C'
			Begin
				Set @questionanswersRecords=3
			End
			Else
			Begin
				Set @questionanswersRecords=6
			End
		End
		Else
		Begin
			Set @questionanswersRecords=6
		End


		IF @questionanswersRecords=6
		Begin

			select '1' As qId,'What is your Date of Birth?' As question,convert(varchar, dateborn, 101) AS answers,'DATE' As inputType, 'NONE' AS inputCategory 
			from customer where custid=@CustId
			Union
			Select '2' As qId,'How many dependents do you have?' As question,convert(varchar,dependants) AS answers,'TEXTBOX' As inputType, 'NUMBER' AS inputCategory 
			from customer where custid=@CustId 
			
			--Union

			--	Select Top 1 '3' As qId,'What is your cell phone number?' As question, 
			--	convert(varchar,STUFF((SELECT ';' + telno 
			--  FROM custtel
			--  WHERE custid = @CustId
			--  FOR XML PATH('')), 1, 1, ''))  AS answers,'TEXTBOX' As inputType, 'PHONE' AS inputCategory 
			--	FROM dbo.custtel WHERE custid = @CustId

			Union

			select '3' As qId,'What is your Marital Status?' As question,ISNULL(Cod.codedescript, '') AS answers,'TEXTBOX' As inputType, 'NONE' AS inputCategory 
			from customer AS Cus,code As Cod  where Cus.maritalstat=Cod.code and Cus.custid=@CustId and Cod.category='MS1'

			Union

			select '4' As qId,'What is your Nationality?' As question,
			ISNULL(Cod.codedescript,'') AS answers,'TEXTBOX' As inputType, 'NONE' AS inputCategory 
			from customer AS Cus,code As Cod  where Cus.Nationality=Cod.code and Cus.custid=@CustId and Cod.category='NA2'
			Union

			Select '5' As qId,'What is your email address?' As question,
			convert(varchar,STUFF((SELECT ';' +  Email 
				FROM custaddress
				WHERE custid = @CustId and Email <> ''
				FOR XML PATH('')), 1, 1, ''))  AS answers,'TEXTBOX' As inputType, 'EMAIL' AS inputCategory 
			from custaddress where custid=@CustId 
		End

		Else
		Begin
			select '1' As qId,'What is your Date of Birth?' As question,convert(varchar, dateborn, 101) AS answers,'DATE' As inputType, 'NONE' AS inputCategory 
			from customer where custid=@CustId

			Union


			Select '2' As qId,'How many dependents do you have?' As question,convert(varchar,dependants) AS answers,'TEXTBOX' As inputType, 'NUMBER' AS inputCategory 
			from customer where custid=@CustId 
			
			--Union
		
			--	Select Top 1 '3' As qId,'What is your cell phone number?' As question, 
			--	convert(varchar,STUFF((SELECT ';' + telno 
			--  FROM custtel
			--  WHERE custid = @CustId
			--  FOR XML PATH('')), 1, 1, ''))  AS answers,'TEXTBOX' As inputType, 'PHONE' AS inputCategory 
			--	FROM dbo.custtel WHERE custid = @CustId
		End
	End
END
