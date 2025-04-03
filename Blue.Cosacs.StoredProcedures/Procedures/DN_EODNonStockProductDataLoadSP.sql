SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO
SET ANSI_WARNINGS ON
go

if exists (SELECT * FROM dbo.sysobjects WHERE id = object_id('[dbo].[DN_EODNonStockProductDataLoadSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_EODNonStockProductDataLoadSP]
GO

CREATE PROCEDURE DN_EODNonStockProductDataLoadSP
-- ================================================
-- Project      : CoSACS .NET
-- File Name    : DN_EODNonStockProductDataLoadSP.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : 
-- Author       : ?
-- Date         : ?
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- ================================================
        @return int OUTPUT

AS

    SET 	@return = 0			--initialise return code
    
    DECLARE @InterfaceOption varchar(5), @statement SQLText, @drive varchar(100)

    select @drive = value from CountryMaintenance where codename = 'systemdrive'

	select @InterfaceOption = value from countrymaintenance where codename ='RIInterfaceOptions'

	IF EXISTS (SELECT Table_Name FROM INFORMATION_SCHEMA.tables
			   WHERE  Table_Name = 'temp_nonstockprodload')
	BEGIN
		DROP TABLE temp_nonstockprodload
	END	
	
	CREATE TABLE temp_nonstockprodload
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
		CostPrice money,
        VTaxRate varchar(15) not null
	)
	
    SET @statement =
    'BULK INSERT temp_nonstockprodload
    FROM '''+@drive+'\nonstocks_prod.dat''
    WITH (
    DATAFILETYPE = ''char'',
    FORMATFILE = '''+@drive+'\nonstocks_prod.fmt'') '	

    EXEC sp_executesql @statement
	
	DELETE FROM temp_nonstockprodload 
	WHERE deleted = 'Y'
	AND EXISTS (SELECT * FROM temp_nonstockprodload	T
				WHERE T.deleted != 'Y'
				AND T.itemno = temp_nonstockprodload.itemno
				AND T.warehouseno = temp_nonstockprodload.warehouseno)	  -- ISSUE 69159 SC 27-7-07

	--IP - 14/10/11 - #8405 - LW74090
	IF(@InterfaceOption !='FACT')
	BEGIN
		DELETE FROM temp_nonstockprodload where category != 20
	END

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO
SET ANSI_WARNINGS Off

