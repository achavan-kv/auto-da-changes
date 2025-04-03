
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE  Table_Name = 'EquifaxScore' AND   Column_Name = 'Flag_Customerstatus_His_Woe' AND 
 Column_Name = 'Rl_Flag_Customerstatus_His_Woe' AND
 Column_Name = 'EX_Flag_Customerstatus_His_Woe' AND
 Column_Name = 'Ex_Flag_Customerstatus_His_Woe_B')
BEGIN
	ALTER TABLE EquifaxScore 
	ADD Flag_Customerstatus_His_Woe nvarchar(50) NULL,
	RL_Flag_Customerstatus_His_Woe nvarchar(50) NULL,
	EX_Flag_Customerstatus_His_Woe nvarchar(50) NULL,
	Ex_Flag_Customerstatus_His_Woe_B nvarchar(50) NULL
END


