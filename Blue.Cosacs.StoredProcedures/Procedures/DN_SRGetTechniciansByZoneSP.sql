SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS OFF
GO

if exists (select * from dbo.sysobjects
where id = object_id('[dbo].[DN_SRGetTechniciansByZoneSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_SRGetTechniciansByZoneSP]
GO


CREATE PROCEDURE dbo.DN_SRGetTechniciansByZoneSP
    @DateAvailable      SMALLDATETIME,
    @Return             INTEGER OUTPUT

AS
    SET NOCOUNT ON
    SET @Return = 0

    -- Load the list of technicians in each zone
    -- The same technician may appear in multiple zones
    -- Filter on the availability date if a date was supplied
    SELECT  t.TechnicianId,
            t.FirstName,
            t.LastName,
            t.CallsPerDay,
            z.Code as Zone,
			t.HoursFrom,
			t.HoursTo
			,[VacationStartDate] as [UnavailableStartDate]
			,[VacationEndDate] as [UnavailableEndDate]
			,Comments
    FROM   SR_Zone z , SR_Technician t LEFT OUTER JOIN SR_TechnicianVacations tv ON t.TechnicianId = tv.TechnicianId
    WHERE  t.Deleted = 0
    AND    (ISNULL(@DateAvailable, CONVERT(DATETIME,'01 Jan 1900',106)) = CONVERT(DATETIME,'01 Jan 1900',106)
            OR t.CallsPerDay > ISNULL((SELECT COUNT(*) FROM SR_TechnicianDiary td
                                       WHERE  td.TechnicianId = t.TechnicianId
                                       AND    td.SlotDate = @DateAvailable),0))
    AND    z.TechnicianId = t.TechnicianId
    UNION
    -- Add a blank entry for each zone
    SELECT  0, "", "", 0, z2.Code,"","","","",""
    FROM    SR_Zone z2
    ORDER BY 5,1

    SET @Return = @@error

    SET NOCOUNT OFF
    RETURN @Return
GO
SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO
