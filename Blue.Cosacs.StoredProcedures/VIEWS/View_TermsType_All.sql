
IF EXISTS (SELECT * FROM dbo.sysobjects
           WHERE ID = object_id('View_TermsType_All')
           AND OBJECTPROPERTY(id, 'IsView') = 1)
DROP VIEW View_TermsType_All
GO

SET QUOTED_IDENTIFIER OFF
SET ANSI_NULLS ON
GO

CREATE VIEW View_TermsType_All AS
-- **********************************************************************
-- Title: View_TermsType_All.sql
-- Developer: John Croft
-- Date: 16th September 2011
-- Purpose: View to replace TermsType (view)

-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 
-- **********************************************************************
    SELECT  * FROM TermsTypeAllBands
    WHERE   Band = 'A' OR Band = ''

GO

SET QUOTED_IDENTIFIER OFF
SET ANSI_NULLS ON
GO

GRANT SELECT on View_TermsType_All to PUBLIC
GO

-- End End End End End End End End End End End End End End End End End End End End End End End End End End End End End End End End End End End