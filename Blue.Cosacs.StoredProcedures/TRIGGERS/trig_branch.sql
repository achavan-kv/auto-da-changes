
/****** Object:  Trigger [dbo].[trig_branch]    Script Date: 10/25/2007 17:19:18 ******/


IF EXISTS (SELECT * FROM sysobjects WHERE TYPE ='TR'
		AND NAME = 'trig_branch')

DROP TRIGGER trig_branch
GO


SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

Create Trigger [dbo].[trig_branch] on [dbo].[branch]
for update
AS
DECLARE 
@hirefno INTEGER, 
@warehouseno INTEGER,
@hibuffno INTEGER,
@branchno INTEGER

SELECT @hirefno = hirefno,
	   @branchno = branchno, 
	   @hibuffno = hibuffno
FROM inserted

IF @hirefno > 999999
BEGIN
	UPDATE branch 
	SET hirefno = 1 
	WHERE branchno =@branchno
END

IF @hibuffno > 999999
BEGIN
	UPDATE branch 
	SET hibuffno = 1 
	WHERE branchno =@branchno
END

