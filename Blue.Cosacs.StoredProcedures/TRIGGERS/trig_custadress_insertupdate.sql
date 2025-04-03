
--Script for Address Standardization CR2019 - 025

IF EXISTS (SELECT NAME FROM SYSOBJECTS WHERE NAME = 'trig_custadress_insertupdate')

DROP TRIGGER trig_custadress_insertupdate 
GO


SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--06/03/2020	Address Standardization CR Latitude and Longitude columns added for Address Standardization CR2019 - 025

	CREATE TRIGGER [dbo].[trig_custadress_insertupdate]
		ON [dbo].[custaddress]
		INSTEAD OF INSERT
		AS
		BEGIN
		
		   --#8774 - columns were not in the correct order.
		  INSERT INTO Custaddress(origbr, custid, addtype, datein,cusaddr1, cusaddr2, cusaddr3,
									cuspocode, custlocn, resstatus, mthlyrent, datemoved,  Email, PropType, empeenochange, datechange, Notes, deliveryarea,
									ZONE,DELTitleC,DELFirstname,DELLastname,Latitude,Longitude)
		  SELECT origbr, custid, addtype, datein, REPLACE(cusaddr1,',',' '), REPLACE(cusaddr2,',',' '),REPLACE(cusaddr3,',',' '), 
		  cuspocode, custlocn, resstatus, mthlyrent, datemoved, Email, PropType, empeenochange, datechange, Notes, deliveryarea,
		  ZONE,DELTitleC,DELFirstname,DELLastname, --IP - 15/02/10 - CR1072 - Added Zone 
		  Latitude,Longitude --Address Standardization CR2019 - 025
		  FROM inserted
		  
		END



SET XACT_ABORT OFF
