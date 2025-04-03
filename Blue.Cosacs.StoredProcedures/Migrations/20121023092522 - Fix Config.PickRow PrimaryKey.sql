
IF EXISTS(SELECT DISTINCT Constraint_Name AS [Constraint], Table_Schema AS [Schema], Table_Name AS [TableName]
            FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE
           WHERE Table_Schema='Config' AND Table_Name='PickRow' AND CONSTRAINT_NAME='PK_Config.PickRow')
BEGIN
    ALTER TABLE [Config].[PickRow]
    DROP CONSTRAINT [PK_Config.PickRow]
END

IF NOT EXISTS(SELECT DISTINCT
                     col_k1.Table_Schema [Schema], col_k1.Table_Name [TableName],
                     col_k1.COLUMN_NAME [Col1Name], col_k2.COLUMN_NAME [Col2Name], col_k1.Constraint_Name
                FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE col_k1, INFORMATION_SCHEMA.KEY_COLUMN_USAGE col_k2
               WHERE col_k1.Table_Schema = col_k2.Table_Schema AND col_k1.Table_Schema = 'Config' AND
                     col_k1.Table_Name = col_k2.Table_Name AND col_k1.Table_Name = 'PickRow' AND
                     col_k1.CONSTRAINT_NAME = col_k2.CONSTRAINT_NAME AND col_k1.CONSTRAINT_NAME = 'PK_StringsRow' AND
                     col_k1.COLUMN_NAME <> col_k2.COLUMN_NAME AND col_k1.COLUMN_NAME = 'ListId' AND col_k2.COLUMN_NAME='Order')
BEGIN
    ALTER TABLE [Config].[PickRow]
    ADD CONSTRAINT [PK_StringsRow] PRIMARY KEY ([ListId],[Order])
END
