
IF EXISTS (SELECT * FROM sys.objects WHERE [object_id] = OBJECT_ID(N'[dbo].[trig_instalplan]')
               AND [type] = 'TR')
BEGIN
      DROP TRIGGER [dbo].[trig_instalplan]
END

GO
  

-- 
-- Trigger to remove and insert instalplan audit record
--
-- Modified on 	 Modified By	Comment
-- 20 May 2005   Rupal Desai    Modify days to Month when deleting old data
-- 09 Jan 2006   Rupal Desai    67847 Modified to insert data in instalplan audit for bulk update and 
--				added to insert data in instalplan_dateaudit table for datefirst change
-- 31 Jan 2006	 Alex Ayscough	67926 modified to resolve Performance issue for Jamaica
-- 26 July 2013	 Ilyas Parker   #14392 - Broker EOD Fail - Divide by 0
-- 15 July 2020  Rahul Sonawane  10.7 Feature(Ability to set the first payment date) kept Preference Day history for first Instalment date.

CREATE TRIGGER [dbo].[trig_instalplan]
ON [dbo].[instalplan]
FOR UPDATE, INSERT
AS DECLARE @acctno CHAR(12), @error VARCHAR (128)

-- Version		: 002
-- Prevent accounts with differences between agreement and instalplan being saved 
-- allowing up to one instalment difference the way this is structured allows for
-- checking of multiple updates-since inserted could have more than one row
--
/* AA removing for variable instalments*/

   SELECT @acctno = acctno FROM inserted

   --recording changes in agreement to separate audit table but remove old data first
   DECLARE @auditdataperiod INTEGER, @Dateinserted DATETIME

   --set @Dateinserted=getdate()
   SET @Dateinserted= (SELECT DateChange FROM Agreement WHERE acctno = @acctno)		--#14392

   -- Get value for Number of months Audit data to be stroed from country maintenance
   SELECT @auditdataperiod = CONVERT (INTEGER, value) FROM countrymaintenance WHERE codename = 'auditdataperiod'

   -- deleting old data from instalplanaudit where number of months over the value set in the auditdataperiod
   -- modified to months as the country paramater indicates months but trigger was checking for days
   DELETE FROM InstalplanAudit 
   WHERE datechange < DATEADD(MONTH,-@auditdataperiod,GETDATE()) 
	AND EXISTS (SELECT 1 FROM inserted WHERE inserted.acctno  =InstalplanAudit.acctno)
   --and acctno = @acctno

   --#14392
   --delete from InstalplanAudit 
   --where exists (select * from inserted 
		 --WHERE InstalplanAudit.acctno = inserted.acctno
	  --	 and   InstalplanAudit.datechange = @Dateinserted)


   -- inserting record in instalplan audit table for new changes only for instlno and instalamount
   INSERT INTO InstalplanAudit
   (acctno, agrmtno,Newinstalno,Newinstalment,
   Oldinstalno,Oldinstalment,empeenochange,datechange,systemusername,prefInstalmentDay,oldprefInstalmentDay)
   SELECT inserted.acctno,inserted.agrmtno,inserted.instalno,inserted.instalamount,
   	  deleted.instalno, deleted.instalamount,ISNULL(inserted.empeenochange,0),@Dateinserted,'',inserted.prefInstalmentDay,deleted.prefInstalmentDay		--#8621 
   FROM   inserted
   INNER JOIN deleted ON inserted.acctno = deleted.acctno
   AND    inserted.agrmtno = deleted.agrmtno
   WHERE  
   --inserted.acctno = deleted.acctno
   --AND    inserted.agrmtno = deleted.agrmtno
   --AND    
   (inserted.InstalNo != deleted.InstalNo
          OR inserted.InstalAmount != deleted.InstalAmount
		  OR inserted.prefInstalmentDay != deleted.prefInstalmentDay  
		  )

   -- RD 67847 Added to insert datefirst change record in the Instalplan_DateAudit table 
   --71599 audit datelast changes as well
   INSERT INTO Instalplan_DateAudit
   (acctno,datechanged,olddatefirst,newdatefirst,empeeno,olddatelast,newdatelast )
   SELECT i.acctno,@Dateinserted,d.datefirst, i.datefirst, ISNULL(i.empeenochange,0), d.datelast,i.datelast 
   FROM   inserted i 
   INNER JOIN deleted d ON i.acctno = d.acctno
   AND    i.agrmtno = d.agrmtno
   WHERE  
   --i.acctno = d.acctno
   --and    i.agrmtno = d.agrmtno
   --and    
   (i.datefirst != d.DATEFIRST
   OR i.datelast != d.datelast)
	AND NOT EXISTS(SELECT 1 FROM Instalplan_DateAudit WHERE datechanged=@Dateinserted)		-- #14630

GO

ALTER TABLE [dbo].[instalplan] ENABLE TRIGGER [trig_instalplan]
GO


