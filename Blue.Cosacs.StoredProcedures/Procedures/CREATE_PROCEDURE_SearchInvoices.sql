if exists (select * from dbo.sysobjects where id = object_id('[dbo].[SearchInvoices]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
DROP PROCEDURE [dbo].[SearchInvoices] 
GO

/****** Object:  StoredProcedure [dbo].[SearchInvoices]]    Script Date: 04-01-2019 11:19:11 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

--SearchInvoices '7','01 Jun 1900','02 Jan 2019','78012180000002',null,0
-- ===============================================================
-- Author:		Raj Kishore
-- Create date: 10-12-2018
-- Description:	This procedure will search data as per given inputs. 
-- Modified By: Snehal Devadhe
-- Modified Date: 23-01-2020
-- Description: This will not overflowed an INT1 column for InvoiceVersion by casting as larger integer column.


-- ================================================================
CREATE PROCEDURE [dbo].[SearchInvoices] --'',0
	-- Add the parameters for the stored procedure here	
	
	@branchNo smallint =  null,
	@invoiceDateFrom datetime,
	@invoiceDateTo datetime,
	@invoiceNo nvarchar(20)=null,
	@accountNo char(12)=null,
	@versionNo char(2)=NULL,
 @return             INT OUTPUT
	
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	SET NOCOUNT ON;

if CHARINDEX('-',@invoiceNo) > 0   
begin
 
  SET @versionNo =   FORMAT(cast((substring(@invoiceNo, charindex('-',@invoiceNo) + 1,len(@invoiceNo) - charindex('-',@invoiceNo))) as int),'0','en-US')
  SET @invoiceNo = substring(@invoiceNo, 1, charindex('-',@invoiceNo) - 1)
end

SET @invoiceDateTo = DATEADD(day, 1, @invoiceDateTo)
SELECT  id.branchno as 'Branch No',convert(varchar, id.datedel, 106) as 'Invoice Date',-- convert(varchar, id.datedel, 106)as 'Invoice Date',
CASE
--WHEN (id.InvoiceVersion IS NOT NULL) THEN ag.AgreementInvoiceNumber +'-' + id.Invoiceversion ELSE ag.AgreementInvoiceNumber END as 'Ord/Invoice No' ,
WHEN (id.agrmtno = 1) THEN id.AgreementInvNoVersion +'-' + CAST(id.Invoiceversion AS VARCHAR(3)) ELSE id.AgreementInvNoVersion END as 'Ord/Invoice No' ,
id.acctno as 'Account No',convert(numeric(10,2), sum(id.quantity * id.price))  as 'Delivery Value',convert(numeric(10,2),sum(id.taxamt)) as 'Tax Amount',
	id.agrmtno as 'AgreementNo',act.accttype as 'AccountType' ,cstacct.custid as'Customer Id' 
	FROM invoiceDetails id with(nolock)
	LEFT JOIN agreement ag with(nolock) on ag.acctno = id.acctno and ag.agrmtno = id.agrmtno
	LEFT JOIN acct act with(nolock) on ag.acctno=act.acctno
	LEFT JOIN custacct cstacct with(nolock) on act.acctno=cstacct.acctno
	WHERE (id.branchno= @branchno) 
	AND id.datedel between  convert(datetime, @invoiceDateFrom, 0) and  convert(datetime, @invoiceDateTo, 0)
	AND (id.acctno = @accountNo OR ISNULL(@accountNo, '') = '')
	AND (id.AgreementInvNoVersion = @invoiceNo OR ISNULL(@invoiceNo, '') = '')
	AND (id.InvoiceVersion = @versionNo OR ISNULL(@versionNo, '') = '')
	AND (hldorjnt is NULL OR hldorjnt = 'H')

	--group by id.branchno,id.datedel, id.InvoiceVersion, ag.AgreementInvoiceNumber,id.acctno,ag.agrmtno, act.accttype,cstacct.custid
	group by id.branchno, convert(varchar, id.datedel, 106) , id.InvoiceVersion,id.agrmtno, id.AgreementInvNoVersion,id.acctno,ag.agrmtno, act.accttype,cstacct.custid
	ORDER BY id.acctno
 
	SET ROWCOUNT 0
    SET @return = @@error
END






