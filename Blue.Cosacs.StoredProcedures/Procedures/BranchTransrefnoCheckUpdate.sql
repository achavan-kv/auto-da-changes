IF EXISTS (SELECT * FROM sysobjects
           WHERE name = 'BranchTransrefnoCheckUpdate'
           AND xtype = 'P')
BEGIN
	DROP PROCEDURE BranchTransrefnoCheckUpdate
END
GO           

CREATE PROCEDURE BranchTransrefnoCheckUpdate
@acctno VARCHAR(12),
@branchno INT,
@transrefno INT OUT,
@return INT OUT

AS

BEGIN

	IF EXISTS (SELECT 1 from fintrans 
			   WHERE acctno =@acctno
			   AND transrefno =@transrefno 
			   AND branchno = @branchno) OR @transrefno <= 0 OR @transrefno IS NULL 
	BEGIN		   
		   UPDATE branch 
		   SET  @transrefno = hirefno = hirefno + 1 
		   WHERE branchno =@branchno
	END
	
	SELECT @transrefno
	
END



