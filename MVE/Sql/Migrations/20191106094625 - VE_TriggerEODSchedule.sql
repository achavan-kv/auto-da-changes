IF EXISTS (SELECT * FROM sysobjects WHERE NAME = 'VE_TriggerEODSchedule' AND type = 'TR')
	BEGIN
		DROP Trigger [dbo].[VE_TriggerEODSchedule]
	END
GO

CREATE TRIGGER [dbo].[VE_TriggerEODSchedule]
ON [dbo].[InterfaceControl]
AFTER UPDATE
AS 
BEGIN
	DECLARE @Eligible VARCHAR(50)
    SELECT @Eligible=Result
    FROM INSERTED T0
	WHERE T0.Interface IN ('FACTCOS')
	AND Result='P'
	AND CONVERT(date, DateFinish)=CONVERT(date, getdate());
		IF @Eligible='P'
		BEGIN
			INSERT INTO VE_taskschedular(ServiceCode,Code,IsInsertRecord,IsEODRecords,[Status])
			VALUES('EOD',1,1,0,0)
		END
END