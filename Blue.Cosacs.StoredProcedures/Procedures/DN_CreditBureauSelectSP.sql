SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS OFF 
GO

IF EXISTS (select * from dbo.sysobjects where id = object_id('[dbo].[DN_CreditBureauSelectSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_CreditBureauSelectSP]
GO

-- 23-Nov-2006 //CR 843 Added fetch results for second credit bureau
CREATE PROCEDURE 	dbo.DN_CreditBureauSelectSP
			@custid varchar(20),
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	
	SELECT 
			case when isnull(A.scoredate, '1900-01-01') > isnull(B.scoredate, '1900-01-01') then A.ScoreDate else B.Scoredate end [scoredate],
			case when isnull(A.lawsuittimesincelast, -1) > isnull(B.lawsuittimesincelast, -1) then  A.lawsuittimesincelast else B.lawsuittimesincelast end [lawsuittimesincelast], 
			case when isnull(A.bankruptciestimesincelast, -1) > isnull(B.bankruptciestimesincelast, -1) then A.bankruptciestimesincelast else B.bankruptciestimesincelast end [bankruptciestimesincelast],
			case when isnull(A.lawsuits, -1) > isnull(B.lawsuits, -1)  then A.lawsuits else B.lawsuits end [lawsuits],
			case when isnull(A.lawsuits12months, -1) > isnull(B.lawsuits12months, -1) then A.lawsuits12months else B.lawsuits12months end [lawsuits12months],
			case when isnull(A.lawsuits24months, -1) > isnull(B.lawsuits24months, -1) then A.lawsuits24months else B.lawsuits24months end [lawsuits24months],
			case when isnull(A.lawsuitsavgvalue, -1) > isnull(B.lawsuitsavgvalue, -1) then A.lawsuitsavgvalue else B.lawsuitsavgvalue end [lawsuitsavgvalue],
			case when isnull(A.lawsuitstotalvalue, -1) > isnull(B.lawsuitstotalvalue, -1) then A.lawsuitstotalvalue else B.lawsuitstotalvalue end [lawsuitstotalvalue],
			case when isnull(A.bankruptcies, -1) > isnull(B.bankruptcies, -1) then A.bankruptcies else B.bankruptcies end [bankruptcies],
			case when isnull(A.bankruptcies12months, -1) > isnull(B.bankruptcies12months, -1) then A.bankruptcies12months else B.bankruptcies12months end [bankruptcies12months],
			case when isnull(A.bankruptcies24months, -1) > isnull(B.bankruptcies24months, -1) then A.bankruptcies24months else B.bankruptcies24months end [bankruptcies24months],
			case when isnull(A.bankruptciesavgvalue, -1) > isnull(B.bankruptciesavgvalue, -1) then A.bankruptciesavgvalue else B.bankruptciesavgvalue end [bankruptciesavgvalue],
			case when isnull(A.bankruptciestotalvalue, -1) > isnull(B.bankruptciestotalvalue, -1) then A.bankruptciestotalvalue else B.bankruptciestotalvalue end [bankruptciestotalvalue],
			case when isnull(A.previousenquiries, -1) > isnull(B.previousenquiries, -1) then A.previousenquiries else B.previousenquiries end [previousenquiries],
			case when isnull(A.previousenquiriestotalvalue, -1) > isnull(B.previousenquiriestotalvalue, -1) then A.previousenquiriestotalvalue else B.previousenquiriestotalvalue end [previousenquiriestotalvalue],
			case when isnull(A.previousenquiriesavgvalue, -1) > isnull(B.previousenquiriesavgvalue, -1) then A.previousenquiriesavgvalue else B.previousenquiriesavgvalue end [previousenquiriesavgvalue],
			case when isnull(A.previousenquiries12months, -1) > isnull(B.previousenquiries12months, -1) then A.previousenquiriesavgvalue else B.previousenquiries12months end [previousenquiries12months],
			case when isnull(A.previousenquiriesavgvalue12months, -1) > isnull(B.previousenquiriesavgvalue12months, -1) then A.previousenquiriesavgvalue12months else B.previousenquiriesavgvalue12months end [previousenquiriesavgvalue12months], 
			isnull(A.responsexml,'') [ResponseXML],
			isnull(B.responsexml,'') [ResponseXML2]
	FROM
		(SELECT top 1 * 
		FROM		CreditBureau
		WHERE	custid = @custid 
			AND Isnull(Source, '') IN ('', 'B')
		ORDER BY	scoredate DESC ) A
	FULL OUTER JOIN 
		(SELECT top 1 * 
		FROM 	CreditBureau
		WHERE	custid = @custid 
			AND Isnull(Source, '') = ('D')
		ORDER BY	scoredate DESC ) B
	
	ON A.CustId = B.CustId
	WHERE A.CustId IS NOT NULL 
		OR B.CustID IS NOT NULL
	
	SET @return = @@error
	
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

