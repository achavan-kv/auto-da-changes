
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[BERItemsForReplacementView]'))
	DROP VIEW [dbo].[BERItemsForReplacementView]
GO

CREATE VIEW BERItemsForReplacementView 
-- ================================================      
-- Project      : CoSACS .NET      
-- File Name    : BERItemsForReplacementView.sql      
-- File Type    : MSSQL Server Stored Procedure Script      
-- Title        :   
-- Author       : Ilyas Parker    
-- Date         : 20/03/13     
--      
-- Change Control      
-- --------------      
-- Date      By  Description      
-- ----      --  -----------      
-- =================================================================================   

AS 


SELECT distinct ss.Acctno, 
				ss.ServiceRequestNo, 
				CAST(LEFT(ss.acctno, 3) AS SMALLINT) AS Branch, 
				s.IUPC, 
				s.Itemdescr1, 
				s.Itemdescr2, 
				rtrim(c.title)+' '+rtrim(c.firstname)+ ' ' +rtrim(c.name) as CustomerName,
				c.custid AS CustId,
				'(H):' + RTRIM(ISNULL(cth.DialCode,'')) + ' ' + RTRIM(ISNULL(cth.telno,'')) + ' ' +
				'(W):' + RTRIM(ISNULL(ctw.DialCode,'')) + ' ' + RTRIM(ISNULL(ctw.telno,'')) + ' ' +
				'(M):' + RTRIM(ISNULL(ctm.DialCode,'')) + ' ' + RTRIM(ISNULL(ctm.telno,'')) + ' ' AS CusTel,
				CONVERT(VARCHAR(10), ss.DateClosed, 120)  AS DateAuth
				
FROM SR_Summary ss
		INNER JOIN lineitem l ON ss.ItemId = l.ItemId AND ss.StockLocn = l.StockLocn AND ss.acctno = l.acctno
		INNER JOIN StockInfo s ON ss.ItemId = s.Id
		INNER JOIN Custacct ca ON ca.acctno = l.acctno AND ca.hldorjnt = 'H'
		INNER JOIN Customer c ON ca.custid = c.custid
		LEFT JOIN Custtel cth ON c.custid = cth.custid AND cth.datediscon IS NULL AND cth.tellocn = 'H'
		LEFT JOIN Custtel ctw ON c.custid = ctw.custid AND ctw.datediscon IS NULL AND ctw.tellocn = 'W'
		LEFT JOIN Custtel ctm ON c.custid = ctm.custid AND ctm.datediscon IS NULL AND ctm.tellocn = 'M'
WHERE ss.ReplacementIssued = 1
AND (ss.ReplacementActioned IS NULL OR ss.ReplacementActioned = 0)


GO 

-- End End End End End End End End End End End End End End End End End End End End End End End End End End End End