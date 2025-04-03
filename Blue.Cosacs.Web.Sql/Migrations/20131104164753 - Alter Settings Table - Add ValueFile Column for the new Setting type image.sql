
IF (OBJECT_ID('Config.Setting', 'U') IS NOT NULL AND
    OBJECT_ID('File', 'U') IS NOT NULL)
BEGIN
    ALTER TABLE [Config].[Setting]
    ADD [ValueFile] UNIQUEIDENTIFIER NULL
        CONSTRAINT FK_ConfigSetting_ValueFile_File_Id
            FOREIGN KEY (ValueFile) REFERENCES [File](id);
END
