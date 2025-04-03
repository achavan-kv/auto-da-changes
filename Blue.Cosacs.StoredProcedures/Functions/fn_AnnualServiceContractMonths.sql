IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID('[dbo].[fn_AnnualServiceContractMonths]') AND xtype in ('FN', 'IF', 'TF'))
DROP FUNCTION [dbo].[fn_AnnualServiceContractMonths]

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Ilyas Parker
-- Create date: 16/01/15
-- Description:	Returns the used number of months for a Annual Service Contract
-- =============================================

CREATE FUNCTION fn_AnnualServiceContractMonths
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