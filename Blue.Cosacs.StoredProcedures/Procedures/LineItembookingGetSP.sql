SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[LineItemBookingGetSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[LineItemBookingGetSP]
GO

CREATE PROCEDURE 	dbo.LineItemBookingGetSP
-- ================================================
-- Project      : CoSACS .NET
-- File Name    : LineItembookingGetSP.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Get LineItembooking 
-- Author       : ??
-- Date         : 28/05/12
--
-- This procedure will get lineitemBooking details
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 
-- ================================================
	-- Add the parameters for the stored procedure here
			@acctno varchar(12)

AS
	SET NOCOUNT ON
	
	Select lb.*,l.ItemId, isnull(lf.BookingId,0) as FailureBookingId, lf.Quantity as FailureQuantity, lf.actioned as Actioned
	From LineItem l INNER JOIN LineItemBooking lb on l.id=lb.LineItemID
	LEFT JOIN LineItemBookingFailures lf on lf.OriginalBookingId = lb.ID		--#10535
	Where l.acctno=@acctno
	
GO

-- End End End End End End End End End End End End End End End End End End End End End End End End End End