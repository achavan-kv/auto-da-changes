-- transaction: true

-- Get all Unprocessed Warranty Transactions from Financial.WarrantyMessage
SELECT M.*, hm.Routing
INTO #UnprocessedWarranties
FROM Financial.WarrantyMessage M
INNER JOIN Warranty.Warranty w
    ON M.WarrantyNo=w.Number
INNER JOIN Hub.[Message] hm
    ON M.MessageId = hm.Id
WHERE M.Department not in ('PCE', 'PCF')
    AND M.messageid != -1
    AND hm.Routing = 'Warranty.Sale.Submit'



-- Refresh Department column on temp table
UPDATE UW
SET UW.Department = CASE
        WHEN tag.Name='Electrical' THEN 'PCE'
        WHEN tag.Name='Furniture' THEN 'PCF'
        ELSE UW.Department
    END
FROM #UnprocessedWarranties UW
INNER JOIN Warranty.Warranty w ON w.Number=UW.WarrantyNo
LEFT JOIN Warranty.WarrantyTags wt ON wt.WarrantyId=w.Id
LEFT JOIN Warranty.Tag tag ON tag.Id=wt.TagId
WHERE UW.Department NOT IN ('PCE', 'PCF')



-- Process all 'Warranty.Sale.Submit' Transactions (apply changes)
-- Fixes second stage hub procession for SP Financial.ProcessMessageWarranty
INSERT INTO Financial.[Transaction]
		(Account, BranchNo, [Type], Amount, [Date], MessageId)
	SELECT
		w.Account,
		wm.BranchNo,
		w.TransactionType AS [Type],
		CASE w.CostOrSale
			WHEN 'Sale' THEN wm.SalePrice
			ELSE wm.CostPrice
		END *
		CASE
			WHEN w.CalculateAmount = 1 THEN w.Percentage
			ELSE 1
		END AS Amount,
		GETDATE() AS [Date],
		wm.MessageId AS MessageId
	FROM
		#UnprocessedWarranties wm
		INNER JOIN Financial.TransactionMappingWarranty w
			ON ((CASE wm.AccountType
					WHEN 'C' THEN 'Cash'
					WHEN 'S' THEN 'Cash'
					ELSE 'Credit' END) = w.AccountType OR w.AccountType = '*')
			AND w.Department = wm.Department
			AND w.Cancelation = 0
			AND w.Repossession = 0
    WHERE wm.Routing = 'Warranty.Sale.Submit'



-- Finish second stage processing on Financial.WarrantyMessage
UPDATE Financial.WarrantyMessage 
SET Department = Data.Department
FROM (
    SELECT u.Department, u.Id FROM #UnprocessedWarranties u
) Data
WHERE
    Financial.WarrantyMessage.Id = Data.Id
