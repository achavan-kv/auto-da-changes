if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_ScheduleUpdatePicklistsSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_ScheduleUpdatePicklistsSP]
GO
CREATE PROCEDURE 	dbo.DN_ScheduleUpdatePicklistsSP
					@acctno char(12),
					@stocklocn smallint,
					@buffno integer,
					@itemId int,
					@Picklistnumber integer,
					@picklistbranchnumber integer,
					@type char(1),
					@return integer OUTPUT
AS
  
	set @return =0

	SELECT	@itemId
	
	IF(@type = 'O')
	BEGIN
		UPDATE	schedule 
		SET		Picklistnumber = @Picklistnumber,
				picklistbranchnumber = @picklistbranchnumber,
				datepicklistprinted = null
		WHERE	acctno = @acctno
		AND		(@itemId = '0' OR ItemID = @itemId)
		AND     (@stocklocn = (CASE WHEN ISNULL(retstocklocn,0) = 0 THEN stocklocn ELSE retstocklocn END) OR @stocklocn = 0)
		AND		buffno = @buffno
	END	
	ELSE
	BEGIN
		UPDATE	schedule 
		SET		transchedno = @Picklistnumber,
				transchednobranch = @picklistbranchnumber,
				datetranschednoprinted = null
		WHERE	acctno = @acctno
		AND		(@itemId = '0' OR ItemID = @itemId)
		AND     (@stocklocn = (CASE WHEN ISNULL(retstocklocn,0) = 0 THEN stocklocn ELSE retstocklocn END) OR @stocklocn = 0)
		AND		buffno = @buffno
	END	
 
	SET @return =@@error

GO