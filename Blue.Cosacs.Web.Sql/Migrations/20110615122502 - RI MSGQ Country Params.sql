-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
IF NOT EXISTS(SELECT 1 FROM dbo.CountryMaintenance WHERE CodeName = 'RIQTYpathRepo')
BEGIN
	INSERT INTO dbo.CountryMaintenance
			( CountryCode ,
			  ParameterCategory ,
			  Name ,
			  Value ,
			  Type ,
			  Precision ,
			  OptionCategory ,
			  OptionListName ,
			  Description ,
			  CodeName
			)
	SELECT CountryCode, ParameterCategory, REPLACE(Name, 'outbound', 'Outbound Repo'),REPLACE(Value, 'REG', 'REPO'), Type, Precision, OptionCategory, OptionListName,
					REPLACE(Description, 'path for', 'path for Repossession'), 'RIQTYpathRepo'
	FROM dbo.CountryMaintenance
	WHERE CodeName = 'RIQTYpath'
END


IF NOT EXISTS(SELECT 1 FROM dbo.CountryMaintenance WHERE CodeName = 'RIQTYpathRepoMSGQ')
BEGIN
	INSERT INTO dbo.CountryMaintenance
			( CountryCode ,
			  ParameterCategory ,
			  Name ,
			  Value ,
			  Type ,
			  Precision ,
			  OptionCategory ,
			  OptionListName ,
			  Description ,
			  CodeName
			)
	SELECT CountryCode, ParameterCategory, REPLACE(Name, 'outbound', 'Outbound Repo'),REPLACE(Value, 'RI21DB', 'RI81DB'), Type, Precision, OptionCategory, OptionListName,
					REPLACE(Description, 'path for', 'path for Repossession'), 'RIQTYpathRepoMSGQ'
	FROM dbo.CountryMaintenance
	WHERE CodeName = 'RIQTYpathMSGQ'
END

UPDATE dbo.CountryMaintenance
SET Name = REPLACE(Name, 'directory', 'argument'),
	Value = '"-D commandPostDst=/QIBM/UserData/mqm/MENSAJERIA400/IFCCosacsRi.sh" "-D fromPath={PATH}\{FILE}" "-D toPath=' + Value + '" main.sf.cmd.post',
	Description = 'MSGQ command argument for Delivery Transfer (DTF) export file with following placeholders' + CHAR(13) 
					+ '{PATH} - Directory path for outboud file' + CHAR(13) 
					+ '{FILE} - Name of the outbound file',
	CodeName = REPLACE(CodeName, 'path', '') + 'Argument'
WHERE codename = 'RIDTFpathMSGQ'

UPDATE dbo.CountryMaintenance
SET Name = REPLACE(Name, 'directory', 'argument'),
	Value = '"-D commandPostDst=/QIBM/UserData/mqm/MENSAJERIA400/IFCCosacsRi.sh" "-D fromPath={PATH}\{FILE}" "-D toPath=' + Value + '" main.sf.cmd.post',
	Description = 'MSGQ command argument for Repo Delivery Transfer (DTF) export file with following placeholders' + CHAR(13) 
					+ '{PATH} - Directory path for outboud file'+ CHAR(13) 
					+ '{FILE} - Name of the outbound file',
	CodeName = REPLACE(CodeName, 'path', '') + 'Argument'
WHERE codename = 'RIDTFpathRepoMSGQ'

UPDATE dbo.CountryMaintenance
SET Name = REPLACE(Name, 'directory', 'argument'),
	Value = '"-D commandPostDst=/QIBM/UserData/mqm/MENSAJERIA400/IFCCosacsRi.sh" "-D fromPath={PATH}\{FILE}" "-D toPath=' + Value + '" main.sf.cmd.post',
	Description = 'MSGQ command argument for Committed Stock (QTY) export file with following placeholders' + CHAR(13) 
					+ '{PATH} - Directory path for outboud file' + CHAR(13) 
					+ '{FILE} - Name of the outbound file',
	CodeName = REPLACE(CodeName, 'path', '') + 'Argument'
WHERE codename = 'RIQTYpathMSGQ'

UPDATE dbo.CountryMaintenance
SET Name = REPLACE(Name, 'directory', 'argument'),
	Value = '"-D commandPostDst=/QIBM/UserData/mqm/MENSAJERIA400/IFCCosacsRi.sh" "-D fromPath={PATH}\{FILE}" "-D toPath=' + Value + '" main.sf.cmd.post',
	Description = 'MSGQ command argument for Repo Committed Stock (QTY) export file with following placeholders' + CHAR(13) 
					+ '{PATH} - Directory path for outboud file' + CHAR(13) 
					+ '{FILE} - Name of the outbound file',
	CodeName = REPLACE(CodeName, 'path', '') + 'Argument'
WHERE codename = 'RIQTYpathRepoMSGQ'

UPDATE dbo.CountryMaintenance
SET Name = REPLACE(Name, 'directory', 'argument'),
	Value = '"-D commandPostDst=/QIBM/UserData/mqm/MENSAJERIA400/IFCCosacsRi.sh" "-D fromPath={PATH}\{FILE}" "-D toPath=' + Value + '" main.sf.cmd.post',
	Description = 'MSGQ command argument for Sales & Return (SAR) export file with following placeholders' + CHAR(13) 
					+ '{PATH} - Directory path for outboud file' + CHAR(13) 
					+ '{FILE} - Name of the outbound file',
	CodeName = REPLACE(CodeName, 'path', '') + 'Argument'
WHERE codename = 'RISARpathMSGQ'

UPDATE dbo.CountryMaintenance
SET Name = REPLACE(Name, 'directory', 'argument'),
	Value = '"-D commandPostDst=/QIBM/UserData/mqm/MENSAJERIA400/IFCCosacsRi.sh" "-D fromPath={PATH}\{FILE}" "-D toPath=' + Value + '" main.sf.cmd.post',
	Description = 'MSGQ command argument for Repo Sales & Return (SAR) export file with following placeholders' + CHAR(13) 
					+ '{PATH} - Directory path for outboud file' + CHAR(13) 
					+ '{FILE} - Name of the outbound file',
	CodeName = REPLACE(CodeName, 'path', '') + 'Argument'
WHERE codename = 'RISARpathRepoMSGQ'

UPDATE dbo.CountryMaintenance
SET Name = REPLACE(Name, 'directory', 'argument'),
	Value = '-t "-D fromPath={PATH}\{FILE}" "-D toPath=' + Value + '" main.sf',
	Description = 'MSGQ command argument for Repossessions(RPO) export file with following placeholders' + CHAR(13) 
					+ '{PATH} - Directory path for outboud file' + CHAR(13) 
					+ '{FILE} - Name of the outbound file',
	CodeName = REPLACE(CodeName, 'path', '') + 'Argument'
WHERE codename = 'RIRPOpathMSGQ'

