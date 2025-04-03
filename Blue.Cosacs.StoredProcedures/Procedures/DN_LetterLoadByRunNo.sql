set ANSI_NULLS ON
set QUOTED_IDENTIFIER ON
go

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_LetterLoadByRunNo]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_LetterLoadByRunNo]
GO

CREATE PROCEDURE [dbo].[DN_LetterLoadByRunNo] 
		@runNo smallint , 
		@return int output
AS  
	SET @return = 0    --initialise return code	
	
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	SELECT distinct LT.lettercode, C.codedescript
	FROM letter LT
	INNER JOIN code C on C.category like 'lt%' AND C.code = LT.lettercode
	WHERE LT.runno = @runNo --Plz note origbr column has been used as runno

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO