
--------------------------------------------------------------------------------
--
-- Project      : CoSACS .NET
-- File Name    : vw_PrivilegeClubTiers.sql
-- File Type    : MSSQL Server Upgrade Script
-- Title        : CR633 Report Views for new Privilege Club Tier1/2
-- Author       : D Richardson
-- Date         : 29 Mar 2006
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 29/09/06  DSR Add Customer Address and Telephone
--
--------------------------------------------------------------------------------


-- 1.    Customers currently in Tier1
IF EXISTS (SELECT Table_Name FROM INFORMATION_SCHEMA.Tables
           WHERE  Table_Name = 'vw_PCTier1Customer')
BEGIN
    DROP VIEW vw_PCTier1Customer
END
GO
CREATE VIEW vw_PCTier1Customer
AS SELECT
       t.CurrentPC, t.CustId, t.AcctNo, t.AcctType, LetterCode,
       c.Title + ' ' + c.FirstName + ' ' + c.Name AS CustomerName,
       ISNULL(LTRIM(RTRIM(ca.CusAddr1)),'') + ', ' + ISNULL(LTRIM(RTRIM(ca.CusAddr2)),'') + ', ' + ISNULL(LTRIM(RTRIM(ca.CusAddr3)),'') + ', ' + ISNULL(LTRIM(RTRIM(CusPoCode)),'') AS Address, 
       ISNULL(LTRIM(RTRIM(ct.DialCode)),'') + ' ' + ISNULL(LTRIM(RTRIM(ct.TelNo)),'') + ' ' + ISNULL(LTRIM(RTRIM(ct.ExtnNo)),'') AS Telephone,
       t.Tier1AvgStatus, t.Tier2AvgStatus, t.HighstStatus,
       t.DateLastPaid, t.DateLastDelivery
FROM   PCCustomerTiers t, Customer c
LEFT OUTER JOIN CustAddress ca
ON   ca.CustId = c.CustId AND ca.AddType = 'H' AND ISNULL(ca.DateMoved,'') = ''
LEFT OUTER JOIN CustTel ct
ON   ct.CustId = c.CustId AND ct.TelLocn = 'H' AND ISNULL(ct.DateDiscon,'') = ''
WHERE  t.CurrentPC = 'TIR1'
AND    c.CustId = t.CustId
GO

-- 2.    Customers currently in Tier2
IF EXISTS (SELECT Table_Name FROM INFORMATION_SCHEMA.Tables
           WHERE  Table_Name = 'vw_PCTier2Customer')
BEGIN
    DROP VIEW vw_PCTier2Customer
END
GO
CREATE VIEW vw_PCTier2Customer
AS SELECT
       t.CurrentPC, t.CustId, t.AcctNo, t.AcctType, LetterCode,
       c.Title + ' ' + c.FirstName + ' ' + c.Name AS CustomerName,
       ISNULL(LTRIM(RTRIM(ca.CusAddr1)),'') + ', ' + ISNULL(LTRIM(RTRIM(ca.CusAddr2)),'') + ', ' + ISNULL(LTRIM(RTRIM(ca.CusAddr3)),'') + ', ' + ISNULL(LTRIM(RTRIM(CusPoCode)),'') AS Address, 
       ISNULL(LTRIM(RTRIM(ct.DialCode)),'') + ' ' + ISNULL(LTRIM(RTRIM(ct.TelNo)),'') + ' ' + ISNULL(LTRIM(RTRIM(ct.ExtnNo)),'') AS Telephone,
       t.Tier1AvgStatus, t.Tier2AvgStatus, t.HighstStatus,
       t.DateLastPaid, t.DateLastDelivery
FROM   PCCustomerTiers t, Customer c
LEFT OUTER JOIN CustAddress ca
ON   ca.CustId = c.CustId AND ca.AddType = 'H' AND ISNULL(ca.DateMoved,'') = ''
LEFT OUTER JOIN CustTel ct
ON   ct.CustId = c.CustId AND ct.TelLocn = 'H' AND ISNULL(ct.DateDiscon,'') = ''
WHERE  t.CurrentPC = 'TIR2'
AND    c.CustId = t.CustId
GO

-- 3.    Customers previously removed from the Privilege Club currently not in Tier1 nor Tier2
IF EXISTS (SELECT Table_Name FROM INFORMATION_SCHEMA.Tables
           WHERE  Table_Name = 'vw_PCRemovedCustomer')
BEGIN
    DROP VIEW vw_PCRemovedCustomer
END
GO
CREATE VIEW vw_PCRemovedCustomer
AS SELECT
       t.CurrentPC, t.CustId, t.AcctNo, t.AcctType, LetterCode,
       c.Title + ' ' + c.FirstName + ' ' + c.Name AS CustomerName,
       ISNULL(LTRIM(RTRIM(ca.CusAddr1)),'') + ', ' + ISNULL(LTRIM(RTRIM(ca.CusAddr2)),'') + ', ' + ISNULL(LTRIM(RTRIM(ca.CusAddr3)),'') + ', ' + ISNULL(LTRIM(RTRIM(CusPoCode)),'') AS Address, 
       ISNULL(LTRIM(RTRIM(ct.DialCode)),'') + ' ' + ISNULL(LTRIM(RTRIM(ct.TelNo)),'') + ' ' + ISNULL(LTRIM(RTRIM(ct.ExtnNo)),'') AS Telephone,
       t.Tier1AvgStatus, t.Tier2AvgStatus, t.HighstStatus,
       t.DateLastPaid, t.DateLastDelivery
FROM   PCCustomerTiers t, OldPCMember opcm, Customer c
LEFT OUTER JOIN CustAddress ca
ON   ca.CustId = c.CustId AND ca.AddType = 'H' AND ISNULL(ca.DateMoved,'') = ''
LEFT OUTER JOIN CustTel ct
ON   ct.CustId = c.CustId AND ct.TelLocn = 'H' AND ISNULL(ct.DateDiscon,'') = ''
WHERE  t.CustId = opcm.CustId
AND    t.CurrentPC = ''
AND    c.CustId = t.CustId
GO

-- 4.    Customers that qualify to be moved to Tier1
IF EXISTS (SELECT Table_Name FROM INFORMATION_SCHEMA.Tables
           WHERE  Table_Name = 'vw_PCToQualifyTier1')
BEGIN
    DROP VIEW vw_PCToQualifyTier1
END
GO
CREATE VIEW vw_PCToQualifyTier1
AS SELECT
       t.CurrentPC, t.NewPC, t.CustId, t.AcctNo, t.AcctType, LetterCode,
       c.Title + ' ' + c.FirstName + ' ' + c.Name AS CustomerName,
       ISNULL(LTRIM(RTRIM(ca.CusAddr1)),'') + ', ' + ISNULL(LTRIM(RTRIM(ca.CusAddr2)),'') + ', ' + ISNULL(LTRIM(RTRIM(ca.CusAddr3)),'') + ', ' + ISNULL(LTRIM(RTRIM(CusPoCode)),'') AS Address, 
       ISNULL(LTRIM(RTRIM(ct.DialCode)),'') + ' ' + ISNULL(LTRIM(RTRIM(ct.TelNo)),'') + ' ' + ISNULL(LTRIM(RTRIM(ct.ExtnNo)),'') AS Telephone,
       t.Tier1AvgStatus, t.Tier2AvgStatus, t.HighstStatus,
       t.DateLastPaid, t.DateLastDelivery
FROM   PCCustomerTiers t, Customer c
LEFT OUTER JOIN CustAddress ca
ON   ca.CustId = c.CustId AND ca.AddType = 'H' AND ISNULL(ca.DateMoved,'') = ''
LEFT OUTER JOIN CustTel ct
ON   ct.CustId = c.CustId AND ct.TelLocn = 'H' AND ISNULL(ct.DateDiscon,'') = ''
WHERE  t.CurrentPC = ''
AND    t.NewPC = 'TIR1'
AND    c.CustId = t.CustId
GO

-- 5.    Customers that qualify to be moved to Tier2
IF EXISTS (SELECT Table_Name FROM INFORMATION_SCHEMA.Tables
           WHERE  Table_Name = 'vw_PCToQualifyTier2')
BEGIN
    DROP VIEW vw_PCToQualifyTier2
END
GO
CREATE VIEW vw_PCToQualifyTier2
AS SELECT
       t.CurrentPC, t.NewPC, t.CustId, t.AcctNo, t.AcctType, LetterCode,
       c.Title + ' ' + c.FirstName + ' ' + c.Name AS CustomerName,
       ISNULL(LTRIM(RTRIM(ca.CusAddr1)),'') + ', ' + ISNULL(LTRIM(RTRIM(ca.CusAddr2)),'') + ', ' + ISNULL(LTRIM(RTRIM(ca.CusAddr3)),'') + ', ' + ISNULL(LTRIM(RTRIM(CusPoCode)),'') AS Address, 
       ISNULL(LTRIM(RTRIM(ct.DialCode)),'') + ' ' + ISNULL(LTRIM(RTRIM(ct.TelNo)),'') + ' ' + ISNULL(LTRIM(RTRIM(ct.ExtnNo)),'') AS Telephone,
       t.Tier1AvgStatus, t.Tier2AvgStatus, t.HighstStatus,
       t.DateLastPaid, t.DateLastDelivery
FROM   PCCustomerTiers t, Customer c
LEFT OUTER JOIN CustAddress ca
ON   ca.CustId = c.CustId AND ca.AddType = 'H' AND ISNULL(ca.DateMoved,'') = ''
LEFT OUTER JOIN CustTel ct
ON   ct.CustId = c.CustId AND ct.TelLocn = 'H' AND ISNULL(ct.DateDiscon,'') = ''
WHERE  (t.CurrentPC = '' OR t.CurrentPC = 'TIR1')
AND    t.NewPC = 'TIR2'
AND    c.CustId = t.CustId
GO

-- 6.    Customers to be demoted from Tier1 or Tier2
IF EXISTS (SELECT Table_Name FROM INFORMATION_SCHEMA.Tables
           WHERE  Table_Name = 'vw_PCToDemoteCustomer')
BEGIN
    DROP VIEW vw_PCToDemoteCustomer
END
GO
CREATE VIEW vw_PCToDemoteCustomer
AS SELECT
       t.CurrentPC, t.NewPC, t.CustId, t.AcctNo, t.AcctType, LetterCode,
       c.Title + ' ' + c.FirstName + ' ' + c.Name AS CustomerName,
       ISNULL(LTRIM(RTRIM(ca.CusAddr1)),'') + ', ' + ISNULL(LTRIM(RTRIM(ca.CusAddr2)),'') + ', ' + ISNULL(LTRIM(RTRIM(ca.CusAddr3)),'') + ', ' + ISNULL(LTRIM(RTRIM(CusPoCode)),'') AS Address, 
       ISNULL(LTRIM(RTRIM(ct.DialCode)),'') + ' ' + ISNULL(LTRIM(RTRIM(ct.TelNo)),'') + ' ' + ISNULL(LTRIM(RTRIM(ct.ExtnNo)),'') AS Telephone,
       t.Tier1AvgStatus, t.Tier2AvgStatus, t.HighstStatus,
       t.DateLastPaid, t.DateLastDelivery
FROM   PCCustomerTiers t, Customer c
LEFT OUTER JOIN CustAddress ca
ON   ca.CustId = c.CustId AND ca.AddType = 'H' AND ISNULL(ca.DateMoved,'') = ''
LEFT OUTER JOIN CustTel ct
ON   ct.CustId = c.CustId AND ct.TelLocn = 'H' AND ISNULL(ct.DateDiscon,'') = ''
WHERE  ((t.CurrentPC != '' AND t.NewPc = '') OR (t.CurrentPC = 'TIR2' AND t.NewPc = 'TIR1'))
AND    c.CustId = t.CustId
GO

-- 7.    Customers in the old Privilege Club who do not qualify for Tier1 nor Tier2
IF EXISTS (SELECT Table_Name FROM INFORMATION_SCHEMA.Tables
           WHERE  Table_Name = 'vw_PCToRemoveCustomer')
BEGIN
    DROP VIEW vw_PCToRemoveCustomer
END
GO
CREATE VIEW vw_PCToRemoveCustomer
AS SELECT
       t.CustId, t.AcctNo, t.AcctType, LetterCode,
       c.Title + ' ' + c.FirstName + ' ' + c.Name AS CustomerName,
       ISNULL(LTRIM(RTRIM(ca.CusAddr1)),'') + ', ' + ISNULL(LTRIM(RTRIM(ca.CusAddr2)),'') + ', ' + ISNULL(LTRIM(RTRIM(ca.CusAddr3)),'') + ', ' + ISNULL(LTRIM(RTRIM(CusPoCode)),'') AS Address, 
       ISNULL(LTRIM(RTRIM(ct.DialCode)),'') + ' ' + ISNULL(LTRIM(RTRIM(ct.TelNo)),'') + ' ' + ISNULL(LTRIM(RTRIM(ct.ExtnNo)),'') AS Telephone,
       t.Tier1AvgStatus, t.Tier2AvgStatus, t.HighstStatus,
       t.DateLastPaid, t.DateLastDelivery
FROM   PCCustomerTiers t, Customer c
LEFT OUTER JOIN CustAddress ca
ON   ca.CustId = c.CustId AND ca.AddType = 'H' AND ISNULL(ca.DateMoved,'') = ''
LEFT OUTER JOIN CustTel ct
ON   ct.CustId = c.CustId AND ct.TelLocn = 'H' AND ISNULL(ct.DateDiscon,'') = ''
WHERE  t.OldPCMember = 'Y'
AND    t.NewPC = ''
AND    c.CustId = t.CustId
GO

