IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID('[dbo].[fn_ReadyAssistMonths]') AND xtype in ('FN', 'IF', 'TF'))
DROP FUNCTION [dbo].[fn_ReadyAssistMonths]

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Ilyas Parker
-- Create date: 30/06/14
-- Description:	Returns the used number of months for a Ready Assist
-- =============================================

CREATE FUNCTION fn_ReadyAssistMonths
(
  @startdate AS DATE,
  @enddate   AS DATE
) RETURNS INT
AS
BEGIN
  RETURN
    DATEDIFF(month, @startdate, @enddate)
      - CASE
          WHEN DAY(@enddate) < DAY(@startdate) THEN 1
          ELSE 0
        END
      + 1;
END
GO