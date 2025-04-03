

IF EXISTS (SELECT * FROM dbo.SYSOBJECTS 
	   WHERE  ID = Object_Id ('dbo.TR_Letter_Addto_Remove_ReFin') 
	   AND    OBJECTPROPERTY (id,'IsTrigger') = 1 )
DROP TRIGGER dbo.TR_Letter_Addto_Remove_ReFin
Go

SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO

CREATE TRIGGER TR_Letter_AddTo_Remove_ReFin
-- **********************************************************************
-- Title: TR_Letter_Addto_Remove_ReFin.sql
-- Developer: Rupal Desai
-- Date: 28 April 2005
-- Purpose: Creating Trigger to prevent add-to letter being sent to Re-Financed account

-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 25/07/11  IP  RI System Integration
--***********************************************************************
ON Letter
FOR INSERT
AS
DELETE FROM Letter WHERE EXISTS (SELECT * FROM inserted, acct, lineitem li, stockinfo si --IP - 25/07/11 - RI
				 WHERE  inserted.acctno = letter.acctno
				 AND	acct.acctno = letter.acctno
				 AND	letter.acctno = li.acctno
				 AND	letter.lettercode in ('M','N','O', '1')
				 AND	inserted.lettercode = letter.lettercode
				 AND	li.ItemID = si.ID								--IP - 25/07/11 - RI
				 --AND	li.itemno = 'REFIN')
				 AND	si.iupc = 'REFIN')								--IP - 25/07/11 - RI
Go

SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
Go

	