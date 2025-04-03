
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT * FROM SYSOBJECTS WHERE NAME = 'trig_inspcent')
DROP TRIGGER trig_inspcent
GO

CREATE TRIGGER trig_inspcent ON [dbo].[intratehistory]
FOR INSERT, UPDATE
AS

	DECLARE @error VARCHAR (256)

	IF EXISTS (	SELECT	1
				FROM	Inserted, TermsType 
				WHERE	Inserted.termstype = TermsType.termstype
				AND		Inserted.InsPcent > Inserted.IntRate
				AND		Inserted.InsIncluded = 1)
	BEGIN   
		ROLLBACK   
		SET @error ='The Insurance Percentage must not be more than the Service Percentage.'
		RAISERROR (@error, 16, 1)	
	END

    IF EXISTS (	SELECT	1
				FROM	Inserted, TermsType 
				WHERE	Inserted.termstype = TermsType.termstype
				AND		Inserted.InsPcent < 0)
	BEGIN   
		ROLLBACK   
		SET @error ='The Insurance Percentage cannot be negative.'
		RAISERROR (@error, 16, 1)	
	END
GO