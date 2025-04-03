


SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS OFF
GO

IF EXISTS (SELECT * FROM dbo.SYSOBJECTS 
WHERE id = object_id('[dbo].[ServiceRequestAddComments]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
DROP PROCEDURE [dbo].ServiceRequestAddComments
GO

CREATE PROCEDURE ServiceRequestAddComments
-- **********************************************************************
-- Title: ServiceRequestAddComments.sql
-- Developer: Ilyas Parker
-- Date: 9/02/2011
-- Purpose: Procedure appends comments to the Comments column for an SR

-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 9/02/11   IP  Created
-- **********************************************************************
		@serviceRequestNo int,
		@user int,
		@dateAdded datetime,
		@comments varchar(500)
AS

	declare @empeename varchar(101)
	select @empeename = FullName from Admin.[User] where id = @user

	UPDATE SR_ServiceRequest
	SET comments = comments + ' ' + convert(varchar(20), @dateAdded, 103) + ' ' + convert(varchar(12), @dateAdded, 114) + ' :' + ' ' + 'by user' + ' ' + cast(@user as varchar(5)) + ' ' + @empeename + ' ' + + @comments 
	WHERE ServiceRequestNo = @serviceRequestNo
   

GO
SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO
