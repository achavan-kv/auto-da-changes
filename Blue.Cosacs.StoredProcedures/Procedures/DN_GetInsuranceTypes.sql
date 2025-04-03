SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID('[dbo].[DN_GetInsuranceTypes]') AND OBJECTPROPERTY(id,'IsProcedure') = 1)
DROP PROCEDURE [dbo].[DN_GetInsuranceTypes]
GO
-- ============================================================================================
-- Author:		Ilyas Parker & Nasmi Mohamed
-- Create date: 08/01/2009
-- Description:	Procedure that returns the Insurance Types from the 'INS' category.	
-- ============================================================================================
CREATE PROCEDURE [dbo].[DN_GetInsuranceTypes] 
				@return	int	OUTPUT
as
begin
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	set nocount on;
	
	set @return = 0

	select code, codedescript
	from code 
	where category = 'INS'

    if (@@error != 0)
    begin
        set @return = @@error
    end
end

GO
