SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_LoadLetterFieldsByCode]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_LoadLetterFieldsByCode]
GO

CREATE PROCEDURE [dbo].[DN_LoadLetterFieldsByCode] 
		@letterCode varchar(10),
		@runNo smallint, 
		@storeType char(1),
		@spouseDetailRequired bit,
		@guarantorDetailRequired bit,
		@return int output
AS  
	SET @return = 0    --initialise return code	
	
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

--IF @letterCode = 'C2'
	BEGIN
		SELECT top 30 LT.acctno AS acctno,
			   CU.title AS title,
			   CU.Name AS Name,
			   CU.Name AS LastName,
			   CAD.cusaddr1 AS CusAddr1,
			   CAD.cusaddr2 AS CusAddr2,
			   CAD.cusaddr3 AS CusAddr3,
			   LT.dateAcctLttr AS dateAcctLttr,
			   AC.Arrears As ArrearsCharges,
			   ISNULL(IP.InstalAmount,'') AS InstalAmount,
			   ISNULL(IP.DueDay,'') AS Due_Day			   
		FROM letter LT
		INNER JOIN Acct AC on AC.acctno = LT.acctno
		INNER JOIN Branch BR on AC.branchno = BR.branchno and BR.storeType = @storeType
		LEFT OUTER JOIN InstalPlan IP on IP.acctno = AC.acctno
		INNER JOIN custacct CA on AC.acctno = CA.acctno and CA.hldorjnt = 'H'
		INNER JOIN Customer CU on CU.custid = CA.custid
		INNER JOIN CustAddress CAD on CAD.custid = CU.custid and CAD.addType = 'H' and CAD.datemoved is NULL 
		WHERE LT.runno = @runNo and LT.lettercode = @letterCode
	END

--ELSE IF @letterCode = 'C4'
--	BEGIN
--		
--	END
--
--ELSE IF @letterCode = 'C4'
--	BEGIN
--		
--	END



	SET @return = @@error

GO
SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO