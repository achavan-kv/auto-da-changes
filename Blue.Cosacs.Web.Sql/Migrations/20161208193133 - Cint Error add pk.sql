sp_RENAME '[Merchandising].[CintsExceptions]' , 'CintsError'
GO

ALTER TABLE [Merchandising].CintsError
ADD CONSTRAINT [PK_Merchandising_CintsError] PRIMARY KEY CLUSTERED (MessageId);
GO

