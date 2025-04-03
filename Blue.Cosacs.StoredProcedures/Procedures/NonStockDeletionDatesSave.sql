IF EXISTS (SELECT * FROM sysobjects WHERE NAME = 'NonStockDeletionDatesSave')
DROP PROCEDURE NonStockDeletionDatesSave
GO
CREATE PROCEDURE NonStockDeletionDatesSave 
@itemno VARCHAR(8),
@branchno INT,
@DeletionDate SMALLDATETIME

AS  

	UPDATE NonStockDeletionDates
	SET deletiondate = @DeletionDate
	WHERE branchno= @branchno
	AND itemno= @itemno
	
	IF @@ROWCOUNT = 0
	BEGIN
		INSERT INTO NonStockDeletionDates
		(itemno,branchno,deletiondate)
		VALUES (@itemno,@branchno,@DeletionDate)
	END 
	
GO 	