
SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS OFF
GO

IF EXISTS (SELECT * FROM SysObjects
           WHERE id = object_id(N'[dbo].SummaryEODSP') and OBJECTPROPERTY(id, N'IsProcedure') = 1)

BEGIN
    DROP PROCEDURE SummaryEODSP
END
GO

CREATE PROCEDURE dbo.SummaryEODSP
/*****************************************************************************************************************
** Author	: I. Parker
** Date		: 7th Nov 2011
** Version	: 1.0
** Name		: SummaryEODSP.sql
** Details	: Procedure called from End Of Day job to run all the Summary files which are now procedures and 
**			  previously were scripts.
** Modified	:
** Who	When	  Description
** ---  ----      -----------
** IP  07/11/11 Creation
******************************************************************************************************************/


 @return     int OUTPUT
 
AS

set @return = 0

EXEC Summary1SP @return
if @return!=0
goto Error

EXEC Summary2SP @return
if @return!=0
goto Error

EXEC Summary3SP @return
if @return!=0
goto Error


EXEC Summary4SP @return
if @return!=0
goto Error

EXEC Summary4bSP @return
if @return!=0
goto Error

EXEC Summary4cSP @return
if @return!=0
goto Error

EXEC Summary_new4SP @return
if @return!=0
goto Error

EXEC Summary_new4bSP @return
if @return!=0
goto Error

EXEC Summary_new4cSP @return
if @return!=0
goto Error

/* No Longer Used

EXEC Summary6SP @return
if @return!=0
goto Error

EXEC Summary7SP @return
if @return!=0
goto Error

*/

EXEC Summary9SP @return
if @return!=0
goto Error

EXEC Summary10SP @return
if @return!=0
goto Error

EXEC Summary14SP @return
if @return!=0
goto Error

EXEC Summary15SP @return
if @return!=0
goto Error

EXEC Summary16SP @return
if @return!=0
goto Error

EXEC Summary17SP @return
if @return!=0
goto Error

EXEC Summary18SP

exec rp_vintage_new '2004-01-01'

Error:
SET @Return = @@ERROR

SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO
-- End End End End End End End End End End End End End End End End End End End End End End End End End End End End End End 