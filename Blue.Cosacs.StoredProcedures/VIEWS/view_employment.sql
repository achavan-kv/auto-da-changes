IF EXISTS (SELECT * FROM sysobjects
		   WHERE xtype = 'V'
		   AND name = 'View_Employment')
DROP VIEW View_Employment
GO

CREATE VIEW View_Employment
AS
SELECT origbr ,
        custid ,
        dateemployed ,
        empyrno ,
        worktype ,
        empmtstatus ,
        fullorpart ,
        temporperm ,
        custempeeno ,
        payfreq ,
        annualgross ,
        dateleft ,
        PersDialCode ,
        PersTel ,
        StaffNo ,
        department ,
        CONVERT(VARCHAR(50),Industry) Industry ,
         CONVERT(VARCHAR(50),JobTitle) JobTitle ,
         CONVERT(VARCHAR(50),Organisation) Organisation ,
         CONVERT(VARCHAR(50),EducationLevel) EducationLevel ,
        DateChanged FROM employment
