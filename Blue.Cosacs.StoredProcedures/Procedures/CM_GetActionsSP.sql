set ANSI_NULLS ON
set QUOTED_IDENTIFIER ON
go

if exists (select * from sysobjects where name= 'CM_GetActionsSP')
drop procedure CM_GetActionsSP
go

-- =============================================
-- Author:		Jez Hemans
-- Create date: 24/03/2007
-- Description:	Returns all the actions listed in the code table
-- =============================================
create PROCEDURE [dbo].[CM_GetActionsSP]
	@return	int	OUTPUT
AS
BEGIN
	SET @return = 0    --initialise return code

    	SELECT	codedescript,
			CONVERT(bit,0) as 'Assigned',
            --CASE WHEN WorkList IS NULL THEN CONVERT(bit,0) ELSE CONVERT(bit,1) END AS assigned,
			code,
            CONVERT(bit,0) AS  'Exit',
			ISNULL(A.Strategy,'N_A') AS Strategy,
			ISNULL(S.Description,'No Strategy') AS Description 
	FROM	code c
	LEFT JOIN CMStrategyActions  A ON c.code = a.actioncode
	LEFT JOIN CMStrategy S ON A.Strategy = S.Strategy
	WHERE	category = 'FUA'
	AND		statusflag = 'L'
	ORDER BY sortorder, codedescript

    IF (@@error != 0)
    BEGIN
        SET @return = @@error
    END
END




