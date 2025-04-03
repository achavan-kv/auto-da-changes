IF EXISTS (SELECT * FROM sysobjects
               WHERE xtype = 'P'
               AND name = 'ProvisionsGetAllSP')
BEGIN
	DROP PROCEDURE ProvisionsGetAllSP
END
GO

CREATE PROCEDURE ProvisionsGetAllSP
AS
BEGIN
	SELECT Acctype ,
    StatusName ,
    StatusLower ,
    StatusUpper ,
    MonthsName ,
    MonthsLower ,
    MonthsUpper ,
    Provision  FROM provisions
END
GO