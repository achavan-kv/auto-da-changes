/****** Object:  StoredProcedure [dbo].[DeliveryCheckForSD_SP]    Script Date: 11/27/2007 09:06:43 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DeliveryCheckForSD_SP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DeliveryCheckForSD_SP]
GO
-- =============================================
-- Author:		Jez Hemans
-- Create date: 26/11/2007
-- Description:	Need to check for existence of SD item in Delivery table when rejecting an account
-- =============================================
CREATE PROCEDURE [dbo].[DeliveryCheckForSD_SP] 
	@acctno CHAR(12),
	@return INT OUTPUT 
AS
BEGIN
	SET NOCOUNT ON
    SET 	@return = 0
    
    SELECT 1 FROM Delivery WHERE itemno = 'SD' AND acctno = @acctno
    
END
