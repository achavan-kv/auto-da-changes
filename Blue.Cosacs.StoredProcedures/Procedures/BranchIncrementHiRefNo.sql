IF EXISTS (SELECT * FROM sysobjects
		   WHERE xtype = 'P'
		   AND name = 'BranchIncrementHiRefNo')
BEGIN 
DROP PROCEDURE BranchIncrementHiRefNo
END
GO

CREATE PROCEDURE BranchIncrementHiRefNo
@branchno INT,
@IncrementBranchNo INT

AS

DECLARE @origRef INT 

SELECT @origRef = hirefno 
FROM branch
WHERE branchno = @branchno

UPDATE branch
SET hirefno = hirefno + @IncrementBranchNo
WHERE branchno = @branchno

SELECT @origRef

