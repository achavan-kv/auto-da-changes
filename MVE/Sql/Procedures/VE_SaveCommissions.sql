if exists (select * from dbo.sysobjects where id = object_id('[dbo].[VE_SaveCommissions]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
BEGIN
	DROP PROCEDURE [dbo].[VE_SaveCommissions]
END
GO

Create PROCEDURE [dbo].[VE_SaveCommissions]
     @Commissionsxml XML
,@isUpdate VARCHAR(MAX) 
,@Message VARCHAR(MAX) OUTPUT
,@StatusCode INT OUTPUT
AS
BEGIN
BEGIN TRY

SET NOCOUNT ON;
SET @Message = '';
SET @StatusCode = 0

DECLARE @EmployeeNo INT = N'',
@ItemText VARCHAR(20) = N'',
@CommissionType VARCHAR(20) = N'',
@Acctno VARCHAR(20) = N'',
@ItemNo VARCHAR(20) = N'',
@StockLocn INT = N'',
@NetCommissionValue MONEY = N'',
@CommissionPercent MONEY = N'',
@CommissionAmount MONEY = N'',
@AgrmtNo INT = N'',
@BuffNo INT = N'',
@ContractNo VARCHAR(20) = N'',
@ItemId INT = N'',
@RunDate DateTime = N'',
@cnt INT, 
@totalcount INT,
@Account VARCHAR(200) = N''

BEGIN TRANSACTION

SELECT 
@cnt = 1,
@totalcount = @Commissionsxml.value('count(/Commissions/CommissionsList/CommissionsList)','INT')
PRINT @totalcount;

SELECT @RunDate = T.c.value('Date[1]','DateTime')
FROM  @Commissionsxml.nodes('/Commissions') T(c)
print @RunDate

SELECT @Account = T.c.value('acctno[1]','VARCHAR(500)')
FROM  @Commissionsxml.nodes('/Commissions') T(c)
print @Account

IF OBJECT_ID('tempdb..#temp') IS NOT NULL
DROP TABLE #temp
  
  SELECT
    T.c.value('EmployeeNo[1]','INT') AS 'EmployeeNo',
    T.c.value('CommissionType[1]','VARCHAR(20)') AS 'CommissionType',     
    T.c.value('ItemNo[1]','VARCHAR(20)') AS 'ItemNo',
    T.c.value('StockLocn[1]','INT') AS 'StockLocn',
    T.c.value('NetCommissionValue[1]','MONEY') AS 'NetCommissionValue',
    T.c.value('CommissionPercent[1]','MONEY') AS 'CommissionPercent',
    T.c.value('CommissionAmount[1]','MONEY') AS 'CommissionAmount',
    DL.AgrmtNo,
    DL.buffNo,
    P.id,
    '' as DMLAction  
  INTO #temp  
  FROM @Commissionsxml.nodes('/Commissions/CommissionsList/CommissionsList') T(c)
  LEFT OUTER JOIN delivery DL ON acctno=@Account
    and DL.itemno=T.c.value('ItemNo[1]','VARCHAR(20)') 
    and DL.StockLocn=T.c.value('StockLocn[1]','INT') 
    and DL.delorcoll = 'D'
  LEFT OUTER JOIN Merchandising.Product P ON P.sku=T.c.value('ItemNo[1]','VARCHAR(20)')
    
  --NOTE: SET 'U' THE DMLACTION COLUMN VALUE WHICH ARE FOR UPDATE   
  Update #temp SET DMLAction = 'U'  
  FROM #temp t 
  INNER JOIN SalesCommission SC ON SC.ItemNo = t.ItemNo 
    AND t.StockLocn=SC.StockLocn
    AND SC.AcctNo=@Account
  
  ----NOTE: SET 'I' THE DMLACTION COLUMN VALUE WHICH ARE FOR INSERT   
  Update #temp SET DMLAction = 'I'  
  Where DMLAction != 'U'
  
  IF(@isUpdate='true')
  BEGIN
   INSERT INTO SalesCommission (
           Employee
          ,RunDate
          ,AcctNo
          ,AgrmtNo
          ,CommissionType
          ,ItemNo
          ,StockLocn
          ,CommissionAmount
          ,Buffno
          ,ContractNo
          ,UpliftRate
          ,NetCommissionValue
          ,ItemId
          ,RepossessedItem
          ,CommissionPcent
          ) 
       SELECT 
           EmployeeNo
          ,@RunDate
          ,@Account
          ,AgrmtNo
          ,CommissionType
          ,ItemNo
          ,StockLocn
          ,CommissionAmount
          ,BuffNo
          ,@ContractNo
          ,0
          ,NetCommissionValue
          ,id
          ,0
          ,CommissionPercent         
       FROM #temp WHERE DMLAction = 'I'    

   UPDATE D SET ftnotes='XX' 
   FROM delivery D 
   INNER JOIN SalesCommission SC ON D.acctno=SC.Acctno 
    and D.itemno=SC.ItemNo  
    and D.delorcoll = 'D' 
    and D.StockLocn=SC.StockLocn
   WHERE D.AcctNo=@Account;   

   UPDATE delivery SET ftnotes='XX' 
   WHERE acctno=@Account 
    and itemno IN ('DT','STAX') 
    and delorcoll = 'D'
  END

  IF(@isUpdate='false')
  BEGIN
   INSERT INTO SalesCommission (
           Employee
          ,RunDate
          ,AcctNo
          ,AgrmtNo
          ,CommissionType
          ,ItemNo
          ,StockLocn
          ,CommissionAmount
          ,Buffno
          ,ContractNo
          ,UpliftRate
          ,NetCommissionValue
          ,ItemId
          ,RepossessedItem
          ,CommissionPcent
          ) 
       SELECT 
           EmployeeNo
          ,@RunDate
          ,@Account
          ,AgrmtNo
          ,CommissionType
          ,ItemNo
          ,StockLocn
          ,CommissionAmount
          ,BuffNo
          ,@ContractNo
          ,0
          ,NetCommissionValue
          ,id
          ,0
          ,CommissionPercent         
       FROM #temp WHERE DMLAction = 'U'    

   UPDATE D SET ftnotes='XX' 
   FROM delivery D 
   INNER JOIN SalesCommission SC ON D.acctno=SC.Acctno 
    and D.itemno=SC.ItemNo  
    and D.delorcoll = 'D' 
    and D.StockLocn=SC.StockLocn
   WHERE D.AcctNo=@Account;   

   UPDATE delivery SET ftnotes='XX' 
   WHERE acctno=@Account 
    and itemno IN ('DT','STAX') 
    and delorcoll = 'D'
  END

   
  IF (@@error != 0)
   BEGIN
    ROLLBACK
    print'Transaction rolled back'
    SET @StatusCode = 500;           
   END
   ELSE
    BEGIN
    SET @StatusCode = 200;
    COMMIT
    print'Transaction committed'
   END
       END TRY 
       BEGIN CATCH 
              IF (@@error > 0)
              SET @StatusCode = 500;         
              SET @Message = ERROR_Message()
    ROLLBACK TRAN 
       END CATCH    
END