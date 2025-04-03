IF EXISTS (SELECT * 
           FROM   sysobjects 
           WHERE  NAME = 'VE_TriggerPayment' 
                  AND type = 'TR') 
  BEGIN 
      DROP TRIGGER [dbo].[VE_TriggerPayment] 
  END 

go 

CREATE TRIGGER [dbo].[VE_TriggerPayment] 
ON [dbo].[agreement] 
after UPDATE 
AS 
    IF UPDATE (dateauth) 
      BEGIN 
          DECLARE @AcctNo VARCHAR(50) 
          DECLARE @holdprop VARCHAR(50) 

          SELECT @AcctNo = T0.acctno, 
                 @holdprop = T0.holdprop 
          FROM   inserted T0 
                 LEFT OUTER JOIN deleted T1 
                              ON T0.acctno = T1.acctno 
                 INNER JOIN ve_lineitem T2 
                         ON T0.acctno = T2.acctno 
          WHERE  T0.holdprop = 'N' 
                 AND T0.dateauth IS NOT NULL 
                 AND Isnull(T2.authorize, 'false') = 'false' 

          IF @AcctNo != '' 
             AND @holdprop = 'N' 
            BEGIN 
                IF NOT EXISTS (SELECT * 
                               FROM   ve_taskschedular 
                               WHERE  servicecode = 'Auth' 
                                      AND code = @AcctNo) 
                  BEGIN 
                      INSERT INTO ve_taskschedular 
                                  (servicecode, 
                                   code, 
                                   isinsertrecord, 
                                   iseodrecords, 
                                   status) 
                      VALUES      ( 'Auth', 
                                    @AcctNo, 
                                    1, 
                                    0, 
                                    0 ) 

                      UPDATE ve_lineitem 
                      SET    authorize = 'true' 
                      WHERE  acctno = @AcctNo 
                  END 
            END 
      END 