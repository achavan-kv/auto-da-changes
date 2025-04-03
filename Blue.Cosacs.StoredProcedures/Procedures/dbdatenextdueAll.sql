if exists (select * from dbo.sysobjects where id = object_id('[dbo].[dbdatenextdueALL]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[dbdatenextdueALL]
GO
-- Procedure to 
CREATE procedure dbdatenextdueALL @currentdate DATETIME = null 
AS    
-- AA Procedure to update date next due in bulk for performance reasons saves looooping
    
DECLARE @dndtab TABLE ( acctno CHAR(12) PRIMARY KEY ,datenextdue DATETIME, nsd INT,datefirst DATETIME, balance MONEY , datedel DATETIME ,
dnd DATETIME, iyear SMALLINT ,imonth SMALLINT)
 
if @currentdate = NULL 
BEGIN 
	SET @currentdate = GETDATE() 
	IF DATEPART(hour, GETDATE() ) > 14 
		SET @currentdate= DATEADD(DAY,1, GETDATE() )-- normally run from end of day so datenext due should be tomorrow.
	ELSE -- early hours of the morning
		SET @currentdate = GETDATE() --so datenextdue should be today. 
END 

INSERT INTO @dndtab 
(acctno,datenextdue,nsd,DATEFIRST, balance ,datedel,dnd )
SELECT a.acctno,g.datenextdue,0,i.[datefirst],a.outstbal,g.datedel,i.[datefirst]
FROM acct a 
JOIN instalplan i ON a.acctno = i.acctno
JOIN agreement g ON i.acctno = g.acctno
WHERE  a.outstbal >0 AND i.[datefirst] >'1-jan-1910'

UPDATE agreement 
SET datenextdue = dnd
FROM @dndtab d WHERE 
agreement.acctno = d.acctno AND d.DATEFIRST >= @currentdate AND dnd !=agreement.datenextdue

-- now removing these records as datefirst will be same as datenext due. 
DELETE FROM @dndtab WHERE [datefirst] >=@currentdate

-- set to due date to day this month
UPDATE @dndtab 
		SET dnd = CASE WHEN ISDATE ( CAST(DATEPART(yy,@currentdate) * 10000 + 
		                                   DATEPART(m,@currentdate) * 100 +  
		                                   DATEPART(dd, dnd) AS CHAR(8))
								     ) = 1 --check ISDATE valid if 31-dec but not 31-feb
					  THEN CAST( CAST( (DATEPART(yy,@currentdate) * 10000 + 
					                    DATEPART(m,@currentdate) * 100 + 
					                    DATEPART(d, dnd)) AS CHAR(8)) AS DATETIME)
					  ELSE CAST( CAST( (DATEPART(yy,@currentdate) * 10000 + 
					                    DATEPART(m,@currentdate) * 100 + 
					                    01) AS CHAR(8)) AS DATETIME) 
					  END
WHERE DATEPART(DAY,dnd) >= DATEPART(DAY,@currentdate) -- set the date to this month
		
UPDATE @dndtab 		SET iyear = DATEPART(yy,@currentdate),
	     imonth = DATEPART(m,@currentdate)

-- due date going to be next year 70427
UPDATE @dndtab 	SET iyear = iyear + 1 WHERE imonth = 12
	   
-- set due date to next month as past todays day
UPDATE @dndtab 				
		SET dnd = CASE WHEN ISDATE(CAST( (iyear * 10000 + 
                                 DATEPART(m,DATEADD(m,1,@currentdate)) * 100 + 
                                  DATEPART(d, dnd)) AS CHAR(8))) = 1 --check ISDATE valid if 31-dec but not 31-feb
					  THEN CAST( CAST( (iyear * 10000 + 
					       DATEPART(m,DATEADD(m,1,@currentdate)) * 100 + 
					       datepart(day, dnd)) AS CHAR(8)) AS DATETIME)
					  ELSE CAST( CAST( (iyear * 10000 + 
					       DATEPART(m,DATEADD(m,2,@currentdate)) * 100 + 
					       01) AS CHAR(8)) AS DATETIME) -- if invalid date then e.g 30- feb then set to 1st next month. 
					  END 
	WHERE DATEPART(DAY,dnd) < DATEPART(DAY,@currentdate)  
			
	UPDATE agreement 
	SET datenextdue = t.dnd 
	FROM @dndtab t 
	WHERE t.acctno = agreement.acctno
	AND t.dnd !=agreement.datenextdue

GO
