/*****************************************/
/***   Clean the table from old data   ***/
/*****************************************/
DELETE Service.Charge 

/**************************/
/***   Add new values   ***/
/**************************/
;WITH Data (RequestId, [type], Account, Tax, Value, Labour, PartsOther) AS
(
	SELECT
		Sr.Id,
		'Internal',
		Ca.Acctno,
		0.00,
		Ct5.Internal,
		Ct4.Internal,
		Ct2.Internal
	FROM
		Service.Request AS Sr
		INNER JOIN Sr_Chargeto AS Ct2 
			ON Sr.Id = Ct2.Servicerequestno 
			AND Ct2.Sortorder = 2
		INNER JOIN Sr_Chargeto AS Ct4 
			ON Sr.Id = Ct4.Servicerequestno 
			AND Ct4.Sortorder = 4
		INNER JOIN Sr_Chargeto AS Ct5 
			ON Sr.Id = Ct5.Servicerequestno 
			AND Ct5.Sortorder = 5
		LEFT OUTER JOIN Sr_Chargeacct AS Ca 
			ON Sr.Id = Ca.Servicerequestno 
			AND Ca.Chargetype = 'I'
	WHERE
		Ct5.Internal + Ct4.Internal + Ct2.Internal > 0
	UNION
	SELECT
		Sr.Id,
		'EW',
		Ca.Acctno,
		0.00,
		Ct5.Extwarranty,
		Ct4.Extwarranty,
		Ct2.Extwarranty
	FROM
		Service.Request AS Sr
		INNER JOIN Sr_Chargeto AS Ct2 
			ON Sr.Id = Ct2.Servicerequestno 
			AND Ct2.Sortorder = 2
		INNER JOIN Sr_Chargeto AS Ct4 
			ON Sr.Id = Ct4.Servicerequestno 
			AND Ct4.Sortorder = 4
		INNER JOIN Sr_Chargeto AS Ct5 
			ON Sr.Id = Ct5.Servicerequestno 
			AND Ct5.Sortorder = 5
		LEFT OUTER JOIN Sr_Chargeacct AS Ca 
			ON Sr.Id = Ca.Servicerequestno 
			AND Ca.Chargetype = 'W'
	WHERE
		Ct5.Extwarranty + Ct4.Extwarranty + Ct2.Extwarranty > 0
	UNION
	SELECT
		Sr.Id,
		'Supplier',
		Ca.Acctno,
		0.00,
		Ct5.Supplier,
		Ct4.Supplier,
		Ct2.Supplier
	FROM
		Service.Request AS Sr
		INNER JOIN Sr_Chargeto AS Ct2 
			ON Sr.Id = Ct2.Servicerequestno 
			AND Ct2.Sortorder = 2
		INNER JOIN Sr_Chargeto AS Ct4 
			ON Sr.Id = Ct4.Servicerequestno 
			AND Ct4.Sortorder = 4
		INNER JOIN Sr_Chargeto AS Ct5 
			ON Sr.Id = Ct5.Servicerequestno 
			AND Ct5.Sortorder = 5
		LEFT OUTER JOIN Sr_Chargeacct AS Ca 
			ON Sr.Id = Ca.Servicerequestno 
			AND Ca.Chargetype = 'S'
	WHERE
		Ct5.Supplier + Ct4.Supplier + Ct2.Supplier > 0
	UNION
	SELECT
		Sr.Id,
		'Deliverer',
		NULL,
		0.00,
		Ct5.Deliverer,
		Ct4.Deliverer,
		Ct2.Deliverer
	FROM
		Service.Request AS Sr
		INNER JOIN Sr_Chargeto AS Ct2 
			ON Sr.Id = Ct2.Servicerequestno 
			AND Ct2.Sortorder = 2
		INNER JOIN Sr_Chargeto AS Ct4 
			ON Sr.Id = Ct4.Servicerequestno 
			AND Ct4.Sortorder = 4
		INNER JOIN Sr_Chargeto AS Ct5 
			ON Sr.Id = Ct5.Servicerequestno 
			AND Ct5.Sortorder = 5
	WHERE
		Ct5.Deliverer + Ct4.Deliverer + Ct2.Deliverer > 0
	UNION
	SELECT
		Sr.Id,
		'Customer',
		NULL,
		0.00,
		Ct5.Customer,
		Ct4.Customer,
		Ct2.Customer
	FROM
		Service.Request AS Sr
		INNER JOIN Sr_Chargeto AS Ct2 
			ON Sr.Id = Ct2.Servicerequestno 
			AND Ct2.Sortorder = 2
		INNER JOIN Sr_Chargeto AS Ct4 
			ON Sr.Id = Ct4.Servicerequestno 
			AND Ct4.Sortorder = 4
		INNER JOIN Sr_Chargeto AS Ct5 ON
		Sr.Id = Ct5.Servicerequestno AND
		Ct5.Sortorder = 5
	WHERE
		Ct5.Customer + Ct4.Customer + Ct2.Customer > 0
)
INSERT INTO Service.Charge
	(RequestId, Type, Account, Tax, Value, IsExternal, Label, Cost)
SELECT
	data.RequestId, data.type, data.Account, data.Tax, data.Value, data.IsExternal, data.Label, data.Value AS Cost
FROM 
(
	SELECT 
		d.RequestId, 
		d.type, 
		d.Account, 
		d.Tax, 
		d.Value - d.Labour AS Value, 
		CAST(d.PartsOther AS BIT) as IsExternal, 
		CASE 
			WHEN d.PartsOther = 0 THEN 'Parts Cosacs' 
			ELSE 'Parts Other' 
		END AS Label 
	FROM 
		Data d 
	UNION ALL
	SELECT 
		d.RequestId, 
		d.type, 
		d.Account, 
		d.Tax, 
		d.Labour AS Value, 
		NULL as IsExternal, 
		'Labour' AS Label 
	FROM 
		Data d 
	UNION ALL
	SELECT 
		d.RequestId, 
		d.type, 
		d.Account, 
		d.Tax, 
		(d.PartsOther + d.Labour) - d.Value AS Value, 
		CAST(0 AS BIT) as IsExternal, 
		'Parts Cosacs' AS Label 
	FROM 
		Data d 
	WHERE 
		d.PartsOther + d.Labour < d.Value 
		AND d.PartsOther > 0
) AS data

