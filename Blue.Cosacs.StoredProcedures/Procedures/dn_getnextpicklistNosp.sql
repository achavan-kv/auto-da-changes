
if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_GetNextPicklistNoSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_GetNextPicklistNoSP]
GO

CREATE PROCEDURE dbo.DN_GetNextPicklistNoSP
				@branchno smallint, -- current branch
				@user int,
				@picklisttype char(1),
				@picklistnumber int OUTPUT,
				@return int OUTPUT
AS
	SET @return = 0

	IF(@picklisttype = 'O')
	BEGIN
		UPDATE	branch 
		SET		highpicklistno = highpicklistno + 7  --incrementing by 7 to prevent wrong picklist being chosen
		WHERE	branchno = @branchno
	  
		SET @return = @@error
	   
		IF(@return = 0)
		BEGIN
			SELECT	@picklistnumber = highpicklistno 
			FROM	branch 
			WHERE	branchno = @branchno
		END
	END
	ELSE
	BEGIN
		UPDATE	branch 
		SET		hightranschedno = hightranschedno + 7  --incrementing by 7 to prevent wrong picklist being chosen
		WHERE	branchno = @branchno
	  
		SET @return = @@error
	   
		IF(@return = 0)
		BEGIN
			SELECT	@picklistnumber = hightranschedno 
			FROM	branch 
			WHERE	branchno = @branchno
		END
	END	

	SET @return = @@error
	
	IF(@return= 0)
	BEGIN
		INSERT INTO Picklist(picklistnumber, dateprinted, branchno, printedby, ordertransport)
		VALUES (@picklistnumber, getdate(), @branchno, @user, @picklisttype)
		SET @return = @@error
	END

GO

