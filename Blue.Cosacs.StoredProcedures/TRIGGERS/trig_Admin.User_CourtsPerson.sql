IF  EXISTS (SELECT * FROM sys.triggers WHERE object_id = OBJECT_ID(N'[Admin].[UserInsertCourtsPerson]'))
DROP TRIGGER [Admin].[UserInsertCourtsPerson]
GO 



CREATE TRIGGER Admin.UserInsertCourtsPerson ON Admin.[USER]
FOR INSERT
AS
BEGIN
	INSERT INTO	dbo.courtspersonTable
	        ( 
	          UserId ,
	          commndue ,
	          alloccount ,
	          serialno ,
	          datelstaudit ,
	          maxrow ,
	          lstcommn ,
	          UpliftCommissionRate ,
	          MinAccounts ,
	          MaxAccounts ,
	          AllocationRank ,
	          RICashierCode
	        )
SELECT Id , -- UserId - int
	          0.0 , -- commndue - float
	          0 , -- alloccount - int
	          0 , -- serialno - int
	          GETDATE(), -- datelstaudit - datetime
	          0 , -- maxrow - int
	          0.0 , -- lstcommn - money
	          0.0 , -- UpliftCommissionRate - float
	          0 , -- MinAccounts - int
	          0 , -- MaxAccounts - int
	          5 , -- AllocationRank - smallint
	          0  -- RICashierCode - int
	        FROM INSERTED i
	        WHERE NOT EXISTS (SELECT 1 FROM dbo.courtspersonTable c
	                          WHERE c.UserId = i.Id)
END
