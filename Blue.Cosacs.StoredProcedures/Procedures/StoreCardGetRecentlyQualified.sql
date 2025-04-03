--*************************************************
-- Script Name : 4761758_StoreCardGetRecentlyQualified.sql
--
-- Created For	: Barbados
-- Created By	: Manoj Harne
-- Created On	: 26/02/2018
--*************************************************
--
-- Modified On	Modified By	Comment
-- DATE   Modify_By    Remark 
--*************************************************
/****** Object:  StoredProcedure [dbo].[StoreCardGetRecentlyQualified]  Script Date: 26/02/2018 9:18:52 AM ******/
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[StoreCardGetRecentlyQualified]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[StoreCardGetRecentlyQualified]
GO

CREATE PROCEDURE [dbo].[StoreCardGetRecentlyQualified]
AS 

--#log no #4761758 --26/02/2018 fix for Storecard new number generation lock issue.
declare @isCount int
set @isCount =0
select @isCount = count(lastnumber) from countrymaintenance_Stageing
if(@isCount != 0)
begin
 update CountryMaintenance set value =(select  max(lastnumber) from countrymaintenance_Stageing)
 where codename  = 'StoreCardNumber'

 truncate table countrymaintenance_Stageing 

 end

SELECT distinct  a.custid, 
a.Title, 
a.FirstName, 
a.Name,  
a.cusaddr1, 
a.cusaddr2, 
a.cusaddr3, 
a.cuspocode, 
a.branchname, 
a.branchaddr1, 
a.branchaddr2, 
a.branchaddr3, 
a.StoreCardLimit,
a.ApprovalDate AS OfferApprovalDate, 
a.OfferExpiryDate, 
a.runno, 
a.HomePhone, 
a.MobilePhone, 
t.telno AS WorkPhone,
a.branchno, s.AcctNo,s.CardNumber ,
a.CreditScore , 
a.MonthsCreditHistory , 
a.ArrearsInst , 
a.MaxArrearsInstEver,
a.rflimit  FROM StoreCardLastAppSuccess A
LEFT JOIN custacct ca ON a.custid = ca.custid AND ca.acctno LIKE '___9%'
LEFT JOIN custtel T ON  T.CUSTID = A.custid  and T.DATEDISCON IS NULL AND T.tellocn = 'W'

LEFT JOIN StoreCard s ON s.AcctNo = ca.acctno

GO



