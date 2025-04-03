SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_PropResultGetDataSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_PropResultGetDataSP]
GO

CREATE PROCEDURE 	dbo.DN_PropResultGetDataSP
			@acctno varchar(12),
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	SELECT	ADComment,
			ADReqd,
			Decision,
			FinalDec,
			ManualRefer,
			Override
			PolicyRule1,
			PolicyRule2,
			PolicyRule3,
			PolicyRule4,
			PolicyRule5,
			PolicyRule6,
			RiskCat,
			Score,
			SysRecommend,
			UWComment,
			Warning,
			CurNumber,
			CurWorst,
			SetNumber,
			SetWorst,
			ProdCat,
			ProdCode
	FROM 		PropResult
	WHERE 	Acctno = @acctno

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

