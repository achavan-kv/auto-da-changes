if exists (select * from dbo.sysobjects where id = object_id('[dbo].[VE_DeliveryAuthorization]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
BEGIN
	DROP PROCEDURE [dbo].[VE_DeliveryAuthorization]
END
GO

Create PROCEDURE [dbo].[VE_DeliveryAuthorization]
                  @AccountNo VARCHAR(20) = N''
                , @DocType VARCHAR(20) = N''
                , @Message varchar(MAX) output
                , @Status varchar(5) output
AS
BEGIN
                SET NOCOUNT ON;
                IF(@DocType='Auth')
                BEGIN
                                SELECT 
                                                 'DeliveryAuthorization' AS 'resourceType'
                                                ,'Cosacs' AS 'source'
                                                ,isnull(T2.CheckOutId,0) AS 'CheckOutID'
                                                ,'true' AS 'Authorization'
                                                ,T2.OrderNo AS 'OrderNo'
                                FROM [dbo].[agreement] T0                        
                                LEFT OUTER JOIN VE_LineItem T2 ON T0.acctno=T2.acctno
                                WHERE  
                                                T0.holdprop='N'
                                                AND T2.acctno=@AccountNo and t2.quantity > 0
                END

                IF(@DocType='pck')
                BEGIN
                                SELECT 
                                                 'DeliveryAuthorization' AS 'resourceType'
                                                ,'Cosacs' AS 'source'
                                                ,isnull(T2.CheckOutId,0) AS 'CheckOutID'
                                                ,'false' AS 'Authorization'
                                                ,T2.OrderNo
                                FROM VE_LineItem T2 
                                WHERE T2.acctno=@AccountNo AND T2.PickCancel='true'
                END

                IF(@DocType='sch')
                BEGIN
                                SELECT 
                                                 'DeliveryAuthorization' AS 'resourceType'
                                                ,'Cosacs' AS 'source'
                                                ,isnull(T2.CheckOutId,0) AS 'CheckOutID'
                                                ,'false' AS 'Authorization'
                                                ,T2.OrderNo
                                FROM VE_LineItem T2 
                                WHERE T2.acctno=@AccountNo AND T2.ScheduleCancel='true'
                END
END