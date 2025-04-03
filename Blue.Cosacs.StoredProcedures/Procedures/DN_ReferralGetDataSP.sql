
SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS OFF
GO

if exists (select * from dbo.sysobjects
where id = object_id('[dbo].[DN_ReferralGetDataSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_ReferralGetDataSP]
GO


CREATE PROCEDURE [dbo].[DN_ReferralGetDataSP]
-- ================================================
-- Project      : CoSACS .NET
-- File Name    : DN_ReferralGetDataSP.PRC
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : 
-- Author       : ??
-- Date         : ??
--
-- This procedure will retrieve referral data for a customers account
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 19/05/11  IP RI Integration changes - CR1212 - #3666 - returning CreditPercentUplift.
-- ================================================
    @acctno varchar(12),
    @custid varchar(20),
    @dateprop smalldatetime,
    @return int OUTPUT

AS

    SET @return = 0            --initialise return code

	DECLARE @MmiLimit MONEY = 0

	SELECT  @MmiLimit = ISNULL(C.MmiLimit,0)
	FROM	[dbo].[CustomerMmi] AS C WITH(NOLOCK)
	WHERE	C.CustId = @CustId

    -- Get the referral data
    SELECT  P.custid as 'CustomerID',
			P.custid as custid,
            C.firstname as 'FirstName',
            C.name as 'LastName',
            P.dateprop as 'DateProp',
            P.propnotes,
            ISNULL(P.reason,'') AS reason,
            CD.codedescript,
            P.propresult,
            P.Points as 'score',
            C.RFCreditLimit,
            P.systemrecommendation as 'SysRecommend',
            p.acctno,
            isnull(p.CreditPercentUplift,'') as CreditPercentUplift,							--IP - 19/05/11 - CR1212 - #3666
			@MmiLimit as MMILimit
    INTO    #tmpRefData
    FROM    proposal P
    JOIN    customer C ON P.custid = C.custid
    LEFT JOIN code CD ON CD.code = P.reason AND CD.category = N'SN1'
    WHERE   P.acctno = @acctno AND 
			P.custid = @custid
			-- AND P.dateprop = @dateprop		IP - 20/05/10 UAT(176) UAT5.2.1.0 Log - Join not required as record uniquely identify by Custid and acctno
   

    -- Return the details
    SELECT customerid,FirstName,LastName,DateProp,propnotes,ISNULL(reason,'') AS reason,
           ISNULL(codedescript,'') AS codedescript,propresult,score,rfcreditlimit,sysrecommend, CreditPercentUplift		--IP - 19/05/11 - CR1212 - #3666
		   , MMILimit
    FROM #tmpRefData

    -- Return the audit details
    SELECT r.EmpeeNoCleared AS ClearedBy,
           r.DateCleared,
           r.UWResult,
           u.FullName AS UWName
    FROM   ReferralOverride r
    INNER JOIN Admin.[User] u ON u.id = r.EmpeeNoCleared
    WHERE  r.AcctNo = @acctno
    AND    r.DateProp = @dateprop
    AND    r.CustId = @custid
    ORDER BY DateCleared DESC


    IF (@@error != 0)
    BEGIN
        SET @return = @@error
    END
GO
SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO
