SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_CodesGetCategorySP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_CodesGetCategorySP]
GO



CREATE PROCEDURE 	dbo.DN_CodesGetCategorySP
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	SELECT	category,
			code,
			CodeDescript
	FROM		code
	WHERE	category IN
			( 'IT1','TITLE','RS1','PT1','MS1','NT1',	
			N'EG1','ES1','WT1','BANK','BA1','PF1',
			N'LO1','RL1','PID',	N'PIN','PAD','PR1','FD1',
			N'OV1','AST','ET1','ASS','ASL','LT1','ASR','FUP',
                         'ASA','ASC','CC1','AC1','ASP', 'IT2','RS2','PT2','MS2','NT2',	
			N'EG2','ES2','WT2','BA2','PF2',
			N'LO2','RL2','PID','PIN','PAD','PR2','FD2',
			N'OV2','AST','ET2','LT2','CC2','AC1','PH1')					
            order by sortorder
	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

