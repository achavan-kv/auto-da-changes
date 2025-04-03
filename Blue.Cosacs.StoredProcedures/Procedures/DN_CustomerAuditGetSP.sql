SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects
where id = object_id('[dbo].[DN_CustomerAuditGetSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_CustomerAuditGetSP]
GO


CREATE PROCEDURE dbo.DN_CustomerAuditGetSP
    @piCustId    VARCHAR(20),
    @Return      INTEGER OUTPUT

AS DECLARE

    @NewCustID      VARCHAR(20),
    @OldCustID      VARCHAR(20),
    @DateChanged    DATETIME,
    @ChangedBy      INTEGER,
    @RowCount       INTEGER

BEGIN
    SET @NewCustId = @piCustId
    SET @OldCustId = @piCustId
    SET @RowCount = 1
    SET @Return = 0

    -- List all the previous cust ids
    -- A loop will have to select each old cust id in turn
    CREATE TABLE #tmpCustId
        (NewCustid      VARCHAR(20),
         OldCustid      VARCHAR(20),
         DateChanged    DATETIME,
         ChangedBy      INTEGER)

    -- The first insert will be for the current cust id
    -- so DateChanged and ChangedBy will be blank.


	INSERT INTO #tmpCustId
               (NewCustid, OldCustId, DateChanged, ChangedBy)
        VALUES (@NewCustId, @OldCustId, @DateChanged, @ChangedBy)

    WHILE (@OldCustId != '' AND @RowCount > 0)
    BEGIN

        SELECT @NewCustId   = ISNULL(NewCustId,''),
               @OldCustId   = ISNULL(OldCustId,''),
               @DateChanged = ISNULL(DateChange,''),
               @ChangedBy   = ISNULL(EmpeeNoChange,0)
        FROM   CustomerIdChanged
        WHERE  NewCustId = @OldCustId

        SET @RowCount = @@ROWCOUNT

	INSERT INTO #tmpCustId
               (NewCustid, OldCustId, DateChanged, ChangedBy)
        VALUES (@NewCustId, @OldCustId, @DateChanged, @ChangedBy)

        --If a new custid used to be an old custid then break out of the loop - Livewire Issue 68999 - JH 21/05/2007
        IF @OldCustId IN (SELECT NewCustId FROM #tmpCustId) AND  @NewCustId IN (SELECT OldCustId FROM #tmpCustId)
        BEGIN
        SET @RowCount = 0
        END

    END

    -- List all the name changes within each CustId
    -- The first part of the UNION is the CustId changes
    -- The second part is the name changes

    SELECT OldCustId, NewCustId,
           CONVERT(VARCHAR(100),'') AS OldName,
           CONVERT(VARCHAR(100),'') AS NewName,
           DateChanged, ChangedBy
    INTO   #tmpCustomerAudit
    FROM   #tmpCustId
    WHERE  ISNULL(DateChanged,'') != ''
    UNION
    SELECT p.CustId, p.CustId, p.PrevName, '', p.DateNameChge, p.EmpeeNoName
    FROM   #tmpCustId t, PrevName p
    WHERE  p.CustId = t.OldCustId

    -- Fill in the new name for each old name

    UPDATE #tmpCustomerAudit
    SET    NewName = (SELECT t2.OldName FROM #tmpCustomerAudit t2
                      WHERE  t2.OldName != ''
                      AND    t2.DateChanged = (SELECT MIN(t3.DateChanged)
                                               FROM   #tmpCustomerAudit t3
                                               WHERE  t3.OldName != ''
                                               AND    t3.DateChanged > #tmpCustomerAudit.DateChanged))

    -- The last name change was to the current name

    UPDATE #tmpCustomerAudit
    SET    NewName = c.FirstName + ' ' + c.Name
    FROM   Customer c
    WHERE  ISNULL(#tmpCustomerAudit.NewName,'') = ''
    AND    c.CustId = @piCustId

    -- Fill in the old names where only the custid changed

    UPDATE #tmpCustomerAudit
    SET    OldName = NewName
    WHERE  ISNULL(#tmpCustomerAudit.OldName,'') = ''

    -- Return the result set

    SELECT * FROM #tmpCustomerAudit ORDER BY DateChanged DESC

    SET @Return = @@ERROR

END
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO
