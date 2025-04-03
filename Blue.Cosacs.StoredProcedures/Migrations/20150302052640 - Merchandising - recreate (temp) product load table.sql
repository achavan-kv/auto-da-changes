-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here


	
	IF EXISTS (SELECT Table_Name FROM INFORMATION_SCHEMA.tables
			   WHERE  Table_Name = 'temp_prodload')
	BEGIN
		DROP TABLE temp_prodload
	END	
	
	CREATE TABLE temp_prodload
	(
		warehouseno 	varchar(4)  default '' not null,--1
		itemno	 	 	varchar(10) default '' not null,
		suppliercode 	varchar(20) default '' not null,--3
		itemdescr1	 	varchar(30) default '' not null,
		itemdescr2	 	varchar(45) default '' not null,--5
		vunitpricehp	varchar(12) default '' not null,
		vunitpricecash	varchar(12) default '' not null,--7
		category		varchar(4)  default '' not null,
		supplier		varchar(12) default '' not null,--9
		prodstatus		varchar(2)  default '' not null,
		fwarrantable	varchar(2)  default '' not null,--11
		prodtype		varchar(2)  default '' not null,
		fprodtype		varchar(2)  default '' not null,--13
		vdutyfreeprice	float	default 0.00  not null,
		refcode 		varchar(3) default '' not null,--15
		barcode 		varchar(20) null,
		unitpricehp		float	default 0.00	not null,--17
		unitpricecash	float	default 0.00	not null,
		unitpricedutyfree float	default 0.00    not null,--19
		warrantable		smallint default 0	not null,
		leadtime		smallint null,--21
		warrantyrenewalflag	char(1) default 'N' not null,
		assemblyrequired char(1) default 'N' not null,--23
		deleted			char(1) default 'N' not null,
		taxrate			float default 0.00	not null,--25
		branchno		smallint default 0	not null,
		lirowsexist		smallint default 0	not null,--27
		rowprocessed	smallint default 0	not null,
		VCostprice	varchar(12) not null,--29
        SupplierName   varchar(64),--30
        Class   varchar(5),--31
        SubClass   varchar(5),--32
        VTaxRate	varchar(12) not null,--33
		CostPrice money
	)