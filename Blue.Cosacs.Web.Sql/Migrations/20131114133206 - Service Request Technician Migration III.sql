ALTER TABLE [Service].[Request] DROP CONSTRAINT [FK_Request_WarrantySale]

SELECT
	CASE 
		WHEN Codedescript = 'No Fault Found' THEN 'No Fault Found' 
        WHEN Codedescript = 'Misuse by the Customer' THEN 'Misuse by the Customer'
		WHEN Codedescript = 'Event or Terms Not Covered' THEN 'Event or Terms Not Covered'
		WHEN Codedescript = 'Warranty Covered' THEN 'Warranty Covered' 
		ELSE NULL 
	END AS Evaluation,
	code, 
	Category,
	CASE
		WHEN Code IS NULL THEN ''
		ELSE Codedescript
	END	AS Calculated
INTO #Code
FROM
	Code

Insert into Service.Request
	(Id, CreatedOn, CreatedBy, Branch, [Type], [State], Account, InvoiceNumber, CustomerId, CustomerTitle, CustomerFirstName, CustomerLastName, 
						CustomerAddressLine1, CustomerAddressLine2, CustomerAddressLine3, CustomerPostcode, CustomerNotes, ItemId, ItemAmount, ItemSoldBy, 
						ItemDeliveredOn, ItemStockLocation, Item, ItemSupplier, ItemSerialNumber, WarrantyContractId, WarrantyLength, TransitNotes, 
						EvaluationClaimFoodLoss, EvaluationLocation, EvaluationAction, AllocationItemReceivedOn, AllocationPartExpectOn, AllocationInstructions, 
						Resolution, ResolutionDate, ResolutionSupplierToCharge, ResolutionCategory, ResolutionReport, ResolutionLabourCost, ResolutionAdditionalCost, 
						ResolutionTransportCost, ResolutionPrimaryCharge, FinalisedFailure, FinaliseReturnDate, CreatedById, LastUpdatedUser, LastUpdatedUserName, LastUpdatedOn, 
						ItemNumber, IsClosed, IsPaymentRequired, ItemModelNumber, ManWarrantyLength, ResolutionDelivererToCharge, WarrantyContractNo, Evaluation, WarrantyNumber, ResolutionFail)
SELECT DISTINCT
	Sr.Servicerequestno AS id,
	CASE
		WHEN DATEPART(hour, Sr.Datelogged) = '00' THEN Sr.Datelogged
		ELSE DATEADD(Hh, DATEDIFF(Hh, GETDATE(), GETUTCDATE()), CAST(Sr.Datelogged AS DATETIME))
	END AS CreatedOn,
	U.Fullname AS CreatedBy,
	Sr.Servicebranchno AS Branch,
	CASE
		WHEN Sr.Servicetype = 'C' THEN 'SI'
		WHEN Sr.Servicetype = 'N' THEN 'SE'
		WHEN Sr.Servicetype = 'S' THEN 'S'
		WHEN Sr.Servicetype = 'G' THEN 'SI'
	END AS [Type],
	CASE
		WHEN Sr.Status = 'N' THEN 'New'
		WHEN Sr.Status = 'D' THEN 'Awaiting deposit'
		WHEN Sr.Status = 'E' THEN 'Awaiting estimate'
		WHEN Sr.Status = 'B' THEN 'Unknown'
		WHEN Sr.Status = 'A' THEN 'Awaiting repair'
		WHEN Sr.Status = 'R' THEN 'Resolved'
		WHEN Sr.Status = 'C' THEN 'Closed'
		WHEN Sr.Status = 'U' THEN 'Unknown'
		WHEN Sr.Status = 'T' THEN 'Awaiting allocation'
		WHEN Sr.Status = 'H' THEN 'Awaiting repair'
		WHEN Sr.Status = ' ' THEN 'New'
		WHEN Sr.Status = 'S' THEN 'Unknown'
	END AS [State],

	CASE
		WHEN Sr.Invoiceno > 1 THEN NULL
		ELSE NULLIF(Sr.Acctno, '')
	END AS Account,
	CASE
		WHEN Sr.Invoiceno = 1 THEN NULLIF(Sr.Acctno, '')
		WHEN Sr.Invoiceno > 1 THEN NULLIF(CAST(Sr.Servicebranchno AS VARCHAR(4)) + '/' + CAST(Sr.Invoiceno AS VARCHAR(7)), '')
	END AS InvoiceNumber,
	CASE WHEN SR.Servicetype = 'S' THEN NULL ELSE SR.Custid END AS CustomerId,
	CASE WHEN SR.Servicetype = 'S' THEN NULL ELSE COALESCE(C.Title, Cu.Title) END AS CustomerTitle,
	CASE WHEN SR.Servicetype = 'S' THEN NULL ELSE COALESCE(C.Firstname, Cu.Firstname) END AS CustomerFirstName,
	CASE WHEN SR.Servicetype = 'S' THEN NULL ELSE COALESCE(LEFT(C.Lastname, 50), Cu.Name) END AS CustomerLastName,
	CASE WHEN SR.Servicetype = 'S' THEN NULL ELSE COALESCE(C.Address1, Ad.Cusaddr1) END AS CustomerAddressLine1,
	CASE WHEN SR.Servicetype = 'S' THEN NULL ELSE COALESCE(C.Address2, Ad.Cusaddr2) END AS CustomerAddressLine2,
	CASE WHEN SR.Servicetype = 'S' THEN NULL ELSE COALESCE(C.Address3, Ad.Cusaddr3) END AS CustomerAddressLine3,
	CASE WHEN SR.Servicetype = 'S' THEN NULL ELSE COALESCE(C.Addresspc, Ad.Cuspocode) END AS CustomerPostcode,
	CASE WHEN SR.Servicetype = 'S' THEN NULL ELSE COALESCE(C.Directions, Ad.Notes) END AS CustomerNotes,
	Sr.Itemid,
	Sr.Unitprice AS ItemAmount,
	U2.Fullname AS ItemSoldBy,
	Sr.Purchasedate AS ItemDeliveredOn,
	Sr.Stocklocn AS ItemStockLocation,
	CASE
		WHEN Sr.Servicetype = 'N' THEN Sr.Description
		ELSE RTRIM(Si.Itemdescr1) + ' ' + Itemdescr2
	END AS Item,
	Si.Supplier AS ItemSupplier,
	Sr.Serialno AS ItemSerialNumber,
	D.Itemid AS WarrantyContractId,
	W.Warrantylength AS WarrantyLength,
	Sr.Transitnotes AS TransitNotes,
	0 AS EvaluationClaimFoodLoss,
	sl.Calculated AS EvaluationLocation,
	sa.Calculated AS EvaluationAction,
	CASE
		WHEN Sr.Receiveddate = '1900-01-01' THEN NULL
		ELSE Sr.Receiveddate
	END AS AllocationItemReceivedOn,
	NULL AS AllocationPartExpectOn,
	NULL AS AllocationInstructions, 
	CASE
		WHEN R.Resolution = 'BER' THEN 'Beyond Economic Repair'
		WHEN R.Resolution = 'COS' THEN 'Cosmetic Defect'
		WHEN R.Resolution = 'DOD' THEN 'Damage On Delivery'
		WHEN R.Resolution = 'ELE' THEN 'Electrical Defect'
		WHEN R.Resolution = 'ENV' THEN 'Event or terms NOT covered'
		WHEN R.Resolution = 'EVC' THEN 'Event or terms NOT covered'
		WHEN R.Resolution = 'HDW' THEN 'Hardware'
		WHEN R.Resolution = 'INS' THEN 'Installation of New Product'
		WHEN R.Resolution = 'MEC' THEN 'Mechanical Defect'
		WHEN R.Resolution = 'MIS' THEN 'Misuse by the customer'
		WHEN R.Resolution = 'NFF' THEN 'No Fault Found'
		WHEN R.Resolution = 'RST' THEN 'Rusting'
		WHEN R.Resolution = 'SAC' THEN 'Save a Call'
		WHEN R.Resolution = 'SFW' THEN 'Software'
		WHEN R.Resolution = 'STR' THEN 'Structure'
		WHEN R.Resolution = 'UPH' THEN 'Upholstery Fabric Defect'
		WHEN R.Resolution = 'WAA' THEN 'WAA ???'
	END AS Resolution,
	CASE
		WHEN R.Dateclosed = '19000101' THEN NULL
		ELSE
			CASE
				WHEN DATEPART(hour, R.Dateclosed) = '00' THEN R.Dateclosed
				ELSE DATEADD(Hh, DATEDIFF(Hh, GETDATE(), GETUTCDATE()), CAST(R.Dateclosed AS DATETIME))
			END
	END AS ResolutionDate,
	R.Chargetomake AS ResolutionSupplierToCharge,
	R.Chargetomodel AS ResolutionCategory,
	Sr.Technicianreport AS ResolutionReport,
	R.Labourcost AS ResolutionLabourCost,
	R.Additionalcost,
	R.Transportcost AS ResolutionAdditionalCost,
	CASE
		WHEN R.Chargeto = 'CUS' THEN 'Customer'
		WHEN R.Chargeto = 'DEL' THEN 'Deliverer'
		WHEN R.Chargeto = 'EW' THEN 'EW'
		WHEN R.Chargeto = 'INT' THEN 'Internal'
		WHEN R.Chargeto = 'SUP' THEN 'Supplier'
	END AS ResolutionPrimaryCharge,
	CASE
		WHEN R.Resolution = 'BER' THEN 'Beyond Economic Repair'
		WHEN R.Resolution = 'COS' THEN 'Cosmetic Defect'
		WHEN R.Resolution = 'DOD' THEN 'Damage On Delivery'
		WHEN R.Resolution = 'ELE' THEN 'Electrical Defect'
		WHEN R.Resolution = 'ENV' THEN 'Event or terms NOT covered'
		WHEN R.Resolution = 'EVC' THEN 'Event or terms NOT covered'
		WHEN R.Resolution = 'HDW' THEN 'Hardware'
		WHEN R.Resolution = 'INS' THEN 'Installation of New Product'
		WHEN R.Resolution = 'MEC' THEN 'Mechanical Defect'
		WHEN R.Resolution = 'MIS' THEN 'Misuse by the customer'
		WHEN R.Resolution = 'NFF' THEN 'No Fault Found'
		WHEN R.Resolution = 'RST' THEN 'Rusting'
		WHEN R.Resolution = 'SAC' THEN 'Save a Call'
		WHEN R.Resolution = 'SFW' THEN 'Software'
		WHEN R.Resolution = 'STR' THEN 'Structure'
		WHEN R.Resolution = 'UPH' THEN 'Upholstery Fabric Defect'
		WHEN R.Resolution = 'WAA' THEN 'WAA ???'
	END AS FinalisedFailure,
	CASE
		WHEN R.Returndate = '1900-01-01' THEN NULL
		ELSE R.Returndate
	END AS FinaliseReturnDate,
	Sr.Loggedby AS CreatedById,
	NULL AS LastUpdatedUser,
	'' AS LastUpdatedUserName,
	CASE
		WHEN Sr.Datechange = '1900-01-01' THEN
			CASE
				WHEN DATEPART(hour, Sr.Datelogged) = '00' THEN Sr.Datelogged
				ELSE DATEADD(Hh, DATEDIFF(Hh, GETDATE(), GETUTCDATE()), CAST(Sr.Datelogged AS DATETIME))
			END
		ELSE DATEADD(Hh, DATEDIFF(Hh, GETDATE(), GETUTCDATE()), CAST(Sr.Datechange AS DATETIME))
	END AS LastUpdatedOn,
	Si.Iupc AS ItemNumber,
	CASE
		WHEN Sr.Status = 'C' THEN 1
		ELSE 0
	END AS IsClosed,
	NULL AS IsPaymentRequired,
	Sr.Modelno AS ItemModelNumber,
	ISNULL(Wl.Firstyearwarperiod, 0) AS ManWarrantyLength,
	Ca.Custid AS ResolutionDelivererToCharge,
	D.Contractno AS WarrantyContractNo,
	Eval.Evaluation AS Evaluation,
	D.Itemid AS WarrantyNumber,
	ISNULL(Resoltution.Fail, 0) AS ResolutionFail
FROM
	Sr_Servicerequest AS Sr
	INNER LOOP JOIN 
	(
		SELECT title, firstname, name, custid FROM customer
	) AS Cu 
		ON Sr.Custid = Cu.Custid
	INNER JOIN Sr_Resolution AS R 
		ON Sr.Servicerequestno = R.Servicerequestno
	LEFT JOIN Custaddress AS Ad 
		ON Sr.Custid = Ad.Custid 
		AND Ad.Addtype = 'H' 
		AND Ad.Datemoved IS NULL
	LEFT JOIN Sr_Customer AS C 
		ON Sr.Servicerequestno = C.Servicerequestno
	LEFT JOIN Agreement AS Ag 
		ON Sr.Acctno = Ag.Acctno 
		AND Sr.Invoiceno = Ag.Agrmtno
	LEFT JOIN Admin.[User] AS U2 
		ON Ag.Empeenosale = U2.Id
	LEFT JOIN Stockinfo AS Si 
		ON Sr.Itemid = Si.Id
	LEFT JOIN Delivery AS D 
		ON Sr.Acctno = D.Acctno 
		AND Sr.Contractno = D.Contractno 
		AND Sr.Contractno != '' 
		AND D.Delorcoll = 'D' 
		AND Sr.Itemid = D.Parentitemid
	LEFT JOIN Warrantyband AS W 
		ON D.Itemid = W.Itemid 
		AND LEN(W.Waritemno) < 12 -- required for Belize (RI)
	LEFT JOIN Stockprice AS Sp 
		ON Sr.Itemid = Sp.Id 
		AND Sr.Stocklocn = Sp.Branchno
	LEFT JOIN Warrantyband AS Wl 
		ON Sp.Refcode = Wl.Refcode 
		AND LEN(Wl.Waritemno) < 12 -- required for Belize (RI)
	LEFT JOIN Sr_Chargeacct AS Ch 
		ON Sr.Servicerequestno = Ch.Servicerequestno 
		AND Ch.Chargetype = 'D'
	LEFT JOIN Custacct AS Ca 
		ON Ca.Acctno = Ch.Acctno
	LEFT JOIN #Code AS Sl 
		ON Sr.Servicelocn = Sl.Code 
		AND Sl.Category = 'SRSERVLCN'
	LEFT JOIN #Code AS Sa 
		ON Sr.Actionrequired = Sa.Code 
		AND Sa.Category = 'SRSERVACT'
	LEFT JOIN #Code Eval
		ON sr.ServiceEvaln = Eval.code 
		AND eval.category = 'SREVALN'
	INNER JOIN Admin.[User] AS U                                                 
		ON Sr.Loggedby = U.Id
	LEFT JOIN 
	(
		SELECT 
			[Description],
			Fail 
		FROM 
			[Service].Resolution
	) Resoltution
		ON ISNULL(R.Resolution, '') = Resoltution.[Description]

WHERE
	Sr.Loggedby != 99999

DROP TABLE #Code

/*20130423150353*/
UPDATE service.Request
SET RepairLimitWarning = t.warning
FROM (
	SELECT 
		r.id, 
		CASE 
			WHEN sum(r.ResolutionAdditionalCost) + sum(r.ResolutionTransportCost) + sum(r.ResolutionLabourCost) + value + labour + parts >  s.costprice * 0.500 THEN 1
            ELSE 0 END AS warning
	FROM 
		Service.Request r 
		inner join stockitem s on s.itemId = r.ItemId AND s.stocklocn = r.Branch
		inner join (select sum(value) value,sum(labour) labour,sum(partsother) parts, RequestId 
					from service.charge
					group by RequestId) as c on c.RequestId = r.id
		CROSS JOIN Service.Request r2 
	WHERE 
		r.Id != r2.Id 
		AND r.ItemNumber = r2.ItemNumber 
		AND r.ItemSupplier = r2.ItemSupplier
		AND 
		(
			r.ItemSerialNumber = r2.ItemSerialNumber AND r.ItemSerialNumber IS NOT NULL
			OR   r.Account = r2.Account 
		)
	group by 
		r.id, c.value, c.labour, c.parts, s.costprice) t
WHERE 
	t.Id = service.Request.id



/*20130612171041*/
UPDATE w 
SET w.ItemSerialNumber = r.ItemSerialNumber
FROM 
	warranty.Warrantysale w
	inner join service.Request r 
		on r.WarrantyContractId = w.WarrantyId 
        and w.WarrantyContractNo = r.WarrantyContractNo 
		and w.SaleBranch = r.ItemStockLocation
where 
	r.[ItemSerialNumber] is not null 
	and len(ltrim(rtrim(r.[ItemSerialNumber]))) > 0
