SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS OFF
GO

if exists (select * from dbo.sysobjects
where id = object_id('[dbo].[DN_SRGetTechniciansSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_SRGetTechniciansSP]
GO


CREATE PROCEDURE dbo.DN_SRGetTechniciansSP
    @DateAvailable      SMALLDATETIME,
    @Return             INTEGER OUTPUT

AS
    SET NOCOUNT ON
    SET @Return = 0

     -- Load the list of technicians
    -- Filter on availability date if a date was supplied
    SELECT  TechnicianId,
            FirstName,
            LastName,
            CallsPerDay
     FROM   SR_Technician
     WHERE  Deleted = 0
     AND    (ISNULL(@DateAvailable, CONVERT(DATETIME,'01 Jan 1900',106)) = CONVERT(DATETIME,'01 Jan 1900',106)
            OR CallsPerDay > ISNULL((SELECT COUNT(*) FROM SR_TechnicianDiary td
                                     WHERE  td.TechnicianId = SR_Technician.TechnicianId
                                     AND    td.SlotDate = @DateAvailable),0))
	ORDER BY LastName

    SET @Return = @@error

    SET NOCOUNT OFF
    RETURN @Return
GO
SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO
