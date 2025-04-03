-- Script Comment : Script to add cust trigger.
--
-- Script Name : 69809_mal_cust_trigger.sql
--
-- Created For: Malaysia
-- Created By: Stephen Chong
-- Created On: 04 September 2008
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 30/11/11  IP  #8774 - Error when creating RF account. Columns were in the incorrect order for insert.
-- 06/11/18  KM     Saving Delivery Address with Title,FirstName,LastNames

SET XACT_ABORT ON
Go
SET IMPLICIT_TRANSACTIONS OFF
Go

IF EXISTS (SELECT * FROM sysobjects
		   WHERE name = 'trig_custadress_insertupdate'
		   AND xtype = 'TR')
BEGIN
DROP TRIGGER trig_custadress_insertupdate
END
GO

	CREATE TRIGGER [trig_custadress_insertupdate]
		ON [dbo].[custaddress]
		INSTEAD OF INSERT
		AS
		BEGIN
		
		   --#8774 - columns were not in the correct order.
		  INSERT INTO Custaddress(origbr, custid, addtype, datein,cusaddr1, cusaddr2, cusaddr3,
									cuspocode, custlocn, resstatus, mthlyrent, datemoved,  Email, PropType, empeenochange, datechange, Notes, deliveryarea,
									ZONE,DELTitleC,DELFirstname,DELLastname)
		  SELECT origbr, custid, addtype, datein, REPLACE(cusaddr1,',',' '), REPLACE(cusaddr2,',',' '),REPLACE(cusaddr3,',',' '), 
		  cuspocode, custlocn, resstatus, mthlyrent, datemoved, Email, PropType, empeenochange, datechange, Notes, deliveryarea,
		  ZONE,DELTitleC,DELFirstname,DELLastname --IP - 15/02/10 - CR1072 - Added Zone
		  FROM inserted
		  
		END



SET XACT_ABORT OFF
GO
