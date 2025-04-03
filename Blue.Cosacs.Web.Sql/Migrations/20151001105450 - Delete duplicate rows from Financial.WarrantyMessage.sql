-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

--Migration will delete duplicate rows from Financial.WarrantyMessage where there is a MessageId of -1 

    SELECT
         count(*) as NoDuplicates, ContractNumber
    INTO
         #DuplicatesToDelete
    FROM
         financial.WarrantyMessage
    GROUP BY
         ContractNumber
    HAVING
         count(*) > 1

    DELETE FROM
        financial.WarrantyMessage 
    WHERE
        ContractNumber IN (select ContractNumber
                            from #DuplicatesToDelete)
    AND MessageId = -1
    

    DROP TABLE #DuplicatesToDelete


