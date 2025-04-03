IF EXISTS (SELECT * FROM sysobjects WHERE NAME = 'trig_TermsTypeBand')
DROP TRIGGER trig_TermsTypeBand
GO 
create trigger trig_TermsTypeBand on TermsTypeBand for insert, update, delete
as
insert TermsTypeBandAuditPrevious (
	CountryCode,			 Band,			 PointsFrom,
	PointsTo,			 ServiceCharge,			 DateImported,
	 ChangedBy,			 FileName,			 StartDate,
	ScoreType,	 Datechanged, loginName
) 

select
	d.CountryCode,			 d.Band,			 d.PointsFrom,
	d.PointsTo,			 d.ServiceCharge,			 d.DateImported,
	 d.ImportedBy,			 d.FileName,			 d.StartDate,
	d.ScoreType,	 GETDATE(),suser_sname()
 from deleted d
	LEFT join inserted i
		on i.countrycode = d.countrycode
 where i.Band <> d.Band
 OR i.PointsFrom <> d.PointsFrom
 OR i.PointsTo <> d.PointsTo
 OR I.ServiceCharge = D.serviceCharge 
 OR d.ScoreType <> i.ScoreType
 OR i.startdate <> d.startdate 
OR d.Band IS NULL OR i.Band IS NULL 
OR d.PointsFrom IS NULL OR i.PointsFrom IS NULL 
OR d.PointsTo IS NULL OR i.PointsTo IS NULL 
OR d.ServiceCharge IS NULL OR i.ServiceCharge IS NULL 
OR d.DateImported IS NULL OR i.DateImported IS NULL 
OR d.ImportedBy IS NULL OR i.ImportedBy IS NULL 
OR d.FileName IS NULL OR i.FileName IS NULL 
OR d.StartDate IS NULL OR i.StartDate IS NULL 
OR d.ScoreType IS NULL OR i.ScoreType IS NULL 
GO 

