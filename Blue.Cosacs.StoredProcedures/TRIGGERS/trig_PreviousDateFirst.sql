
if exists (select * from sysobjects where type = 'TR'
           and name = 'trig_PreviousDateFirst')

drop trigger trig_PreviousDateFirst
go

-- Removing the trigger "trig_PreviousDateFirst" as unable to notify delivery using
-- immediate delivery screen gettind duplicate key error  RD 12/01/06


/*
CREATE TRIGGER trig_PreviousDateFirst
ON dbo.instalplan
FOR update
AS
    DECLARE @new_acctno varchar(12),
            @new_datefirst datetime,
            @new_user int,
            @old_datefirst datetime

    SELECT  @new_acctno = acctno,
            @new_user = ISNULL(empeenochange,0),
            @new_datefirst = datefirst
    FROM    inserted 
         
    SELECT  @old_datefirst = datefirst
    FROM    deleted
    
    IF (@old_datefirst != @new_datefirst)
    BEGIN
        INSERT INTO instalplan_dateaudit
        (
            acctno,
            datechanged,
            olddatefirst,
            newdatefirst,
            empeeno
        )
        VALUES
        (
            @new_acctno,
            getdate(),
            @old_datefirst,
            @new_datefirst,    
            @new_user
        )
    END
*/
GO

