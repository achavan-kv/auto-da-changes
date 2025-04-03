-- transaction: true

-- Default's on Web Cosacs
DECLARE @setting1 VARCHAR(50)='7L0001'
DECLARE @setting2 VARCHAR(50)='7SPA01'
DECLARE @setting3 VARCHAR(50)='7L0002'
-- Default's on Web Cosacs

DECLARE @SettingsNamespace VARCHAR(30)='Blue.Cosacs.Service'
DECLARE @CodeName1 VARCHAR(30)='ServiceItemLabour'
DECLARE @CodeName2 VARCHAR(30)='ServiceItemPartsCourts'
DECLARE @CodeName3 VARCHAR(30)='ServiceItemPartsOther'

IF NOT EXISTS(select * from StockInfo where itemno=@setting1) --LABOUR CHARGES
   AND EXISTS(select * from CountryMaintenance where CodeName=@CodeName1)
BEGIN    
    SELECT @setting1=[Value]
    FROM countryMaintenance
    WHERE CodeName=@CodeName1;
    INSERT INTO Config.Setting ([Namespace], Id, ValueString)
    VALUES (@SettingsNamespace, @CodeName1, @setting1)
END

IF NOT EXISTS(select * from StockInfo where itemno=@setting2) -- PARTS COURTS
   AND EXISTS(select * from CountryMaintenance where CodeName=@CodeName2)
BEGIN
    SELECT @setting2=[Value]
    FROM countryMaintenance
    WHERE CodeName=@CodeName2;
    INSERT INTO Config.Setting ([Namespace], Id, ValueString)
    VALUES (@SettingsNamespace, @CodeName2, @setting2)
END

IF NOT EXISTS(select * from StockInfo where itemno=@setting3) -- PARTS OTHER
   AND EXISTS(select * from CountryMaintenance where CodeName=@CodeName3)
BEGIN
    SELECT @setting3=[Value]
    FROM countryMaintenance
    WHERE CodeName=@CodeName3;
    INSERT INTO Config.Setting ([Namespace], Id, ValueString)
    VALUES (@SettingsNamespace, @CodeName3, @setting3)
END
